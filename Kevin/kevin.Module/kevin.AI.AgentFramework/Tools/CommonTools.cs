using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Tools
{
    /// <summary>
    /// 常用工具类，包含一些常用的静态方法和属性，可以被智能体调用的用于提供一些常用的功能。
    /// </summary>
    public class CommonTools
    {

        /// <summary>
        /// 获取当前系统。返回运行时平台的友好名称（例如 "Windows"、"Linux"、"macOS"、"Unknown"）。
        /// </summary>
        [Description("获取当前系统,返回运行时平台的友好名称（例如 \"Windows\"、\"Linux\"、\"macOS\"、\"Unknown\"）。")]
        public static string GetRuntimePlatform()
        {
            Console.WriteLine();
            Console.WriteLine($"🔧 正在调用 GetRuntimePlatform ");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "MacOS";
            }

            return "Unknown";
        }

        /// <summary>
        /// 获取当前系统桌面路径。 用于获取当前用户的桌面路径
        /// </summary>
        [Description("获取当前系统桌面路径。 用于获取当前用户的桌面路径，若 ensureExists 为 true 则确保目录存在。")]
        public static string GetDesktopPath([Description("ensureExists 默认为 true 则确保目录存在。")] bool ensureExists = true)
        {
            Console.WriteLine();
            Console.WriteLine($"🔧 正在调用 GetDesktopPath ");
            // 先使用 SpecialFolder（在大多数环境下有效）
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (string.IsNullOrWhiteSpace(desktop))
            {
                // 回退到 UserProfile + "Desktop"
                string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                if (!string.IsNullOrWhiteSpace(home))
                {
                    desktop = Path.Combine(home, "Desktop");
                }
            }

            // 最终兜底：当前目录（不太可能）
            if (string.IsNullOrWhiteSpace(desktop))
            {
                desktop = Directory.GetCurrentDirectory();
            }

            if (ensureExists)
            {
                try
                {
                    Directory.CreateDirectory(desktop);
                }
                catch
                {
                    // 忽略创建失败，让调用方处理
                }
            }

            return desktop;
        }

        /// <summary>
        /// 输出文件到系统桌面。 用于把各种文件输出到桌面
        /// </summary>
        [Description("输出文件到系统桌面。 用于把各种文件输出到桌面，返回完整路径或以 ❌ 开头的错误信息。")]
        public static string WriteTextToDesktop([Description("文件名称如（xx.html,xx.txt）支持各种文件类型")] string fileName, [Description("内容")] string content, [Description("文件是否不存在 默认是")] bool overwrite = true)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine($"🔧 正在调用 WriteTextToDesktop ");
                if (string.IsNullOrWhiteSpace(fileName))
                    return "❌ 保存失败: fileName 不能为空。";

                var encoding = new UTF8Encoding(false); // 默认 UTF-8 无 BOM

                string desktop = GetDesktopPath(true);
                string safeName = MakeSafeFileName(fileName);
                // 若 fileName 包含目录分隔符，则按子目录处理
                string combined = Path.IsPathRooted(safeName) ? safeName : Path.Combine(desktop, safeName);
                string dir = Path.GetDirectoryName(combined) ?? desktop;
                Directory.CreateDirectory(dir);

                if (File.Exists(combined) && !overwrite)
                    return "❌ 保存失败: 文件已存在，overwrite = false。";

                File.WriteAllText(combined, content ?? string.Empty, encoding);
                return combined;
            }
            catch (Exception ex)
            {
                return $"❌ 保存失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 将字节数组保存到桌面指定文件，返回完整路径或以 "❌" 开头的错误信息。
        /// </summary>
        [Description("将字节数组保存到桌面指定文件，返回完整路径或以 ❌ 开头的错误信息。")]
        public static string WriteBytesToDesktop(string fileName, byte[] data, bool overwrite = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    return "❌ 保存失败: fileName 不能为空。";
                if (data == null)
                    return "❌ 保存失败: data 不能为空。";

                string desktop = GetDesktopPath(true);
                string safeName = MakeSafeFileName(fileName);
                string combined = Path.IsPathRooted(safeName) ? safeName : Path.Combine(desktop, safeName);
                string dir = Path.GetDirectoryName(combined) ?? desktop;
                Directory.CreateDirectory(dir);

                if (File.Exists(combined) && !overwrite)
                    return "❌ 保存失败: 文件已存在，overwrite = false。";

                File.WriteAllBytes(combined, data);
                return combined;
            }
            catch (Exception ex)
            {
                return $"❌ 保存失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 将流内容保存到桌面指定文件，返回完整路径或以 "❌" 开头的错误信息。
        /// </summary>
        [Description("将流内容保存到桌面指定文件，返回完整路径或以 ❌ 开头的错误信息。")]
        public static string WriteStreamToDesktop(string fileName, Stream stream, bool overwrite = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    return "❌ 保存失败: fileName 不能为空。";
                if (stream == null)
                    return "❌ 保存失败: stream 不能为空。";

                string desktop = GetDesktopPath(true);
                string safeName = MakeSafeFileName(fileName);
                string combined = Path.IsPathRooted(safeName) ? safeName : Path.Combine(desktop, safeName);
                string dir = Path.GetDirectoryName(combined) ?? desktop;
                Directory.CreateDirectory(dir);

                if (File.Exists(combined) && !overwrite)
                    return "❌ 保存失败: 文件已存在，overwrite = false。";

                // 将流写入文件（从当前位置开始读取）
                using (var fs = new FileStream(combined, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    stream.CopyTo(fs);
                }

                return combined;
            }
            catch (Exception ex)
            {
                return $"❌ 保存失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 从源路径复制文件到桌面（可重命名），返回目标完整路径或以 "❌" 开头的错误信息。
        /// </summary>
        [Description("从源路径复制文件到桌面（可重命名），返回目标完整路径或以 ❌ 开头的错误信息。")]
        public static string CopyFileToDesktop(string sourcePath, string fileName = null, bool overwrite = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                    return "❌ 复制失败: 源文件不存在。";

                string desktop = GetDesktopPath(true);
                string targetName = string.IsNullOrWhiteSpace(fileName) ? Path.GetFileName(sourcePath) : MakeSafeFileName(fileName);
                string targetPath = Path.Combine(desktop, targetName);

                if (File.Exists(targetPath) && !overwrite)
                    return "❌ 复制失败: 目标文件已存在，overwrite = false。";

                File.Copy(sourcePath, targetPath, overwrite);
                return targetPath;
            }
            catch (Exception ex)
            {
                return $"❌ 复制失败: {ex.Message}";
            }
        }

        // 将文件名或相对路径做最基本的安全化处理（移除非法字符）
        private static string MakeSafeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return fileName ?? string.Empty;

            // 如果传入包含目录分隔符，保留子目录结构但清理每个段
            char[] seps = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
            var parts = fileName.Split(seps, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                var invalid = Path.GetInvalidFileNameChars();
                foreach (var c in invalid)
                {
                    part = part.Replace(c, '_');
                }
                parts[i] = part;
            }

            return string.Join(Path.DirectorySeparatorChar.ToString(), parts);
        }
    }
}
