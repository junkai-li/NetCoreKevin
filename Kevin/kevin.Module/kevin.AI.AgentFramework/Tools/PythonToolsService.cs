using Common;
using kevin.AI.AgentFramework.Interfaces.Tools;
using Kevin.Common.Helper;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
namespace kevin.AI.AgentFramework.Tools
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // RunPython — 一个 执行Python脚本的工具
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public class PythonToolsService : IPythonToolsService
    {
        private object? _data { get; set; }
        private int _contentLengthLimit = 0;//  内容长度限制，超过限制后会进行截断
        public void InitData(object data)
        {
            _data = data;
            if (_data != default)
            {
                try
                {
                    JsonDocument.Parse(JsonSerializer.Serialize(_data)).RootElement.GetProperty("ContentLengthLimit").TryGetInt32(out _contentLengthLimit);
                }
                catch (Exception)
                {
                    _contentLengthLimit = 0; // 解析失败则不限制内容长度
                }

            }
        }
        /// <summary>
        /// 检测指定命令是否存在于系统 PATH 中
        /// </summary>
        private static bool IsCommandAvailable(string command)
        {
            try
            {
                var checkProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which",
                        Arguments = command,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                checkProcess.Start();
                checkProcess.WaitForExit();
                return checkProcess.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取可用的 Python 命令（优先 python3，其次 python）
        /// 如果都不可用，返回 null
        /// </summary>
        private static string? GetAvailablePythonCommand()
        {
            // 优先检查 python3
            if (IsCommandAvailable("python3"))
            {
                return "python3";
            }

            // 其次检查 python
            if (IsCommandAvailable("python"))
            {
                return "python";
            }

            // 都不存在返回 null
            return null;
        }

        [Description("执行Python脚本。通过System.Diagnostics.Process类来启动一个新的进程，并运行Python.py的脚本。这种方法适用于Windows和Linux系统。")]
        public async Task<string> RunPythonPy([Description("需要执行的python脚本路径。例如：'Skills\\python-skills\\hello-python\\scripts\\hello-python.py'")]
                                        string scriptPath,
            [Description("需要传入python脚本的参数。例如：['你好','word']")]
            List<string> args = default
            )
        {
            try
            {
                string output = "";
                //传入非完整的路径
                if (!scriptPath.Contains(":"))
                {
                    scriptPath = AppContext.BaseDirectory + scriptPath.Replace(@"/", @"\");
                } 
                var validationResult = PythonSecurityValidator.ValidatePythonFile(scriptPath);
                if (!validationResult.IsValid)
                {
                    var blockedList = string.Join("; ", validationResult.BlockedItems);
                    return $"❌ 安全校验失败: {blockedList}";
                }
                Console.WriteLine();
                Console.WriteLine($"🔧 正在执行Py脚本 {scriptPath}");
                // 设置进程信息
                ProcessStartInfo start = new ProcessStartInfo();
                // 检测可用的 Python 环境
                string? pythonCmd = GetAvailablePythonCommand();
                if (pythonCmd == null)
                {
                    throw new InvalidOperationException(
                        "Python environment is not installed. Please install Python (python3 or python) and ensure it is available in the system PATH."
                    );
                }
                start.FileName = pythonCmd; // Python解释器的路径，例如 "python" 或 "python3"
                start.Arguments = $"{scriptPath}"; // 传递参数
                if (args != default)
                {
                    foreach (var item in args)
                    {
                        start.Arguments += $" {item} \" ";
                    }

                }

                start.UseShellExecute = false; // 不使用操作系统外壳启动
                start.RedirectStandardOutput = true; // 重定向标准输出
                start.RedirectStandardError = true; // 重定向标准错误
                using (Process process = Process.Start(start))
                {
                    // 获取输出
                    output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit(); // 等待进程结束 
                    if (!string.IsNullOrEmpty(error))
                    {
                        return $"❌ 执行失败: {error}";
                    }
                }
                if (string.IsNullOrWhiteSpace(output))
                {
                    output = "Python脚本执行完成，但没有输出结果。";
                }
                return StringHelper.SubstringText(output, _contentLengthLimit);
            }
            catch (Exception ex)
            {
                return $"❌ 执行失败: {ex.Message}";
            }
        }

        [Description("用于执行Python代码")]
        public async Task<string> RunPythonCode([Description("需要执行的python代码。例如：'def main(name): return 'Hello ' + name.title() + '!'")]
                                        string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return "执行Py代码为空。";
                }

                var validationResult = PythonSecurityValidator.ValidatePythonCode(code);
                if (!validationResult.IsValid)
                {
                    var blockedList = string.Join("; ", validationResult.BlockedItems);
                    return $"❌ 安全校验失败: {blockedList}";
                }
                Console.WriteLine();
                Console.WriteLine($"🔧 正在执行Py代码");
                var saveResult = await SavePythonToFile(code, "Pys", "");
                Console.WriteLine($"🔧 正在执行Py脚本: {saveResult}");
                string output = "";
                // 设置进程信息
                ProcessStartInfo start = new ProcessStartInfo();
                // 检测可用的 Python 环境
                string? pythonCmd = GetAvailablePythonCommand();
                if (pythonCmd == null)
                {
                    throw new InvalidOperationException(
                        "Python environment is not installed. Please install Python (python3 or python) and ensure it is available in the system PATH."
                    );
                }
                start.FileName = pythonCmd; // Python解释器的路径，例如 "python" 或 "python3"
                start.Arguments = $"{saveResult}"; // 传递参数 
                start.UseShellExecute = false; // 不使用操作系统外壳启动
                start.RedirectStandardOutput = true; // 重定向标准输出
                start.RedirectStandardError = true; // 重定向标准错误
                using (Process process = Process.Start(start))
                {
                    // 获取输出
                    output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit(); // 等待进程结束 
                    if (!string.IsNullOrEmpty(error))
                    {
                        return $"❌ 执行失败: {error}";
                    }
                }
                if (string.IsNullOrWhiteSpace(output))
                {
                    output = "Python脚本执行完成，但没有输出结果。";
                }
                return StringHelper.SubstringText(output, _contentLengthLimit);

            }
            catch (Exception ex)
            {
                return $"❌ 执行失败: {ex.Message}";
            }
        }

        [Description("把传入的python代码保存为 .py 文件，返回保存的完整路径，失败返回以 ❌ 开头的错误信息")]
        public async Task<string> SavePythonToFile(string code, string relativeDir = "Skills/python-skills/tmp", string fileName = null)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(code))
                {
                    return "❌ 保存失败: 代码内容为空。";
                }

                // 规范化相对目录分隔符
                relativeDir = relativeDir.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
                string baseDir = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
                string fullDir = Path.Combine(baseDir, relativeDir);

                // 确保目录存在
                Directory.CreateDirectory(fullDir);

                // 自动生成文件名如果未提供
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = $"py_{System.DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}.py";
                }
                else
                {
                    if (!fileName.EndsWith(".py", StringComparison.OrdinalIgnoreCase))
                    {
                        fileName = fileName + ".py";
                    }
                }

                string fullPath = Path.Combine(fullDir, fileName);

                // 以 UTF-8 无 BOM 保存，保证跨平台兼容且 Python 能正确识别
                File.WriteAllText(fullPath, code, new UTF8Encoding(false));
                Console.WriteLine($"🔧 Python脚本已保存到: {fullPath}");
                return StringHelper.SubstringText(fullPath, _contentLengthLimit);
            }
            catch (Exception ex)
            {
                return $"❌ 保存失败: {ex.Message}";
            }
        }


    }
}
