using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Kevin.Common.Helper
{
    public class MemoryHelper : IDisposable
    {
        // --- Windows API 声明 ---
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesWritten);

        private readonly Process _process;
        private readonly IntPtr _processHandle;
        private readonly IntPtr _mainModuleBase;
        private readonly bool _is64Bit;

        /// <summary>
        /// 初始化内存助手
        /// </summary>
        /// <param name="processName">进程名（不带.exe）</param>
        public MemoryHelper(string processName)
        {
            _process = Process.GetProcessesByName(processName).FirstOrDefault();
            if (_process == null) throw new Exception($"未找到进程: {processName}");

            _processHandle = _process.Handle;
            _mainModuleBase = _process.MainModule!.BaseAddress;

            // 自动判断目标程序是32位还是64位，决定指针读取长度
            _is64Bit = Environment.Is64BitProcess && IsWow64(_process) == false;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool IsWow64Process(IntPtr hProcess, out bool lpSystemInfo);
        private bool IsWow64(Process process) { IsWow64Process(process.Handle, out bool result); return result; }

        // --- 核心逻辑：多级指针寻址 ---

        /// <summary>
        /// 根据基址偏移和偏移链，获取最终的数据内存地址
        /// </summary>
        public IntPtr GetFinalAddress(int baseOffset, int[] offsets)
        {
            IntPtr currentAddr = IntPtr.Add(_mainModuleBase, baseOffset);

            for (int i = 0; i < offsets.Length; i++)
            {
                // 读取当前地址指向的下一个指针
                long ptr = _is64Bit ? Read<long>(currentAddr) : Read<int>(currentAddr);

                if (ptr == 0) return IntPtr.Zero; // 指针断裂保护

                // 最后一个偏移量只加不读（即指向最终数据的地址）
                currentAddr = (IntPtr)(ptr + offsets[i]);
            }
            return currentAddr;
        }

        // --- 泛型读取与写入 ---

        /// <summary>
        /// 通用读取方法：支持 int, float, double, byte, long 等
        /// </summary>
        public T Read<T>(IntPtr address) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            ReadProcessMemory(_processHandle, address, buffer, size, out _);
            return ByteArrayToStructure<T>(buffer);
        }

        /// <summary>
        /// 通用写入方法
        /// </summary>
        public void Write<T>(IntPtr address, T value) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = StructureToByteArray(value);
            WriteProcessMemory(_processHandle, address, buffer, size, out _);
        }

        // --- 快捷方法：直接读取指针链对应的值 ---

        public T ReadPointer<T>(int baseOffset, int[] offsets) where T : struct
        {
            IntPtr addr = GetFinalAddress(baseOffset, offsets);
            return addr == IntPtr.Zero ? default : Read<T>(addr);
        }

        // --- 内部转换工具 ---

        private T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try { return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T)); }
            finally { handle.Free(); }
        }

        private byte[] StructureToByteArray<T>(T value) where T : struct
        {
            int size = Marshal.SizeOf(value);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(value, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
                return arr;
            }
            finally { Marshal.FreeHGlobal(ptr); }
        }

        public void Dispose() => _process?.Dispose();
    }
}
