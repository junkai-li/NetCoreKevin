using Common;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Interfaces.Tools;
using Kevin.Common.Helper;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace kevin.AI.AgentFramework.Tools
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // RunPython — 一个 执行Python脚本的工具
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public class PythonToolsService : IPythonToolsService
    {
        // 用于从代码中提取URL的正则表达式
        private static readonly Regex UrlRegex = new Regex(@"https?://[\w\-._~:/?#\[\]@!$&'()*+,;=%]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private object? _data { get; set; }
        private int _contentLengthLimit = 0;//  内容长度限制，超过限制后会进行截断
        private List<string> _authorizedDomains = new List<string>(); // 授权域名列表
        public void InitData(object data)
        {
            _data = data;
            if (_data != default)
            {
                try
                {
                    var jsonDoc = JsonDocument.Parse(JsonSerializer.Serialize(_data));
                    var authorizedDomains = jsonDoc.RootElement.GetProperty("AuthorizedDomains").GetString();
                    if (!string.IsNullOrWhiteSpace(authorizedDomains) && authorizedDomains.Trim() != "*")
                    {
                        authorizedDomains.Split(',')
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .ToList()
                            .ForEach(domain => this._authorizedDomains.Add(domain));
                    }
                    jsonDoc.RootElement.GetProperty("ContentLengthLimit").TryGetInt32(out _contentLengthLimit); 
                }
                catch (Exception)
                {
                    _contentLengthLimit = 0; // 解析失败则不限制内容长度
                }

            }
        }

        /// <summary>
        /// 检查代码中的URL是否在授权域名白名单中
        /// 支持授权域名格式：
        /// - example.com (仅域名)
        /// - https://example.com (带协议)
        /// - https://example.com/api (带路径前缀)
        /// </summary>
        /// <param name="code">Python代码或脚本路径</param>
        /// <exception cref="UnauthorizedAccessException"></exception>
        private void AuthorizedDomainsCheck(string code)
        {
            if (_data == default) return;

            try
            {
                if (_authorizedDomains.Count == 0)
                    return; // 没有有效的前缀，等同于允许所有

                // 从代码中提取所有URL
                var matches = UrlRegex.Matches(code);
                foreach (Match match in matches)
                {
                    var url = match.Value;
                    var isAllowed = _authorizedDomains.Any(prefix => url.Contains(prefix, StringComparison.OrdinalIgnoreCase)); 
                    if (!isAllowed)
                        throw new UnauthorizedAccessException($"URL '{url}' 不在授权域名单中。");
                }
            }
            catch (KeyNotFoundException)
            {
                // 如果未配置 AuthorizedDomains，视为允许所有
                return;
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

        [Description("执行Python脚本。通过System.Diagnostics.Process类来启动一个新的进程，并运行Python.py的脚本。这种方法适用于Windows和Linux系统。包含安全护栏：HTTP请求域名白名单。")]
        public async Task<string> RunPythonPy([Description("需要执行的python脚本路径。例如：'Skills\\python-skills\\hello-python\\scripts\\hello-python.py'")]
                                        string scriptPath,
            [Description("需要传入python脚本的参数。例如：['你好','word']")]
            List<string> args = default
            )
        {
            try
            {
                //传入非完整的路径
                if (!scriptPath.Contains(":"))
                {
                    scriptPath = AppContext.BaseDirectory + scriptPath.Replace(@"/", @"\");
                }

                // 🛡️ 安全护栏：HTTP请求域名白名单检查（检查脚本路径和参数）
                try
                {
                    string fullCommand = scriptPath;
                    if (args != default)
                    {
                        fullCommand += " " + string.Join(" ", args);
                    }
                    AuthorizedDomainsCheck(fullCommand);
                }
                catch (UnauthorizedAccessException ex)
                {
                    return $"❌ 授权拦截：{ex.Message}";
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
                string output = "";
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
                return output.Length > _contentLengthLimit ? SystemPrompt.ContentLimitPromptText + StringHelper.SubstringText(output, _contentLengthLimit) : output;
            }
            catch (Exception ex)
            {
                return $"❌ 执行失败: {ex.Message}";
            }
        }

        [Description("用于执行Python代码。包含安全护栏：HTTP请求域名白名单。")]
        public async Task<string> RunPythonCode([Description("需要执行的python代码。例如：'def main(name): return 'Hello ' + name.title() + '!'")]
                                        string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return "执行Py代码为空。";
                }

                // 🛡️ 安全护栏：HTTP请求域名白名单检查
                try
                {
                    AuthorizedDomainsCheck(code);
                }
                catch (UnauthorizedAccessException ex)
                {
                    return $"❌ 授权拦截：{ex.Message}";
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
                return output.Length > _contentLengthLimit ? SystemPrompt.ContentLimitPromptText + StringHelper.SubstringText(output, _contentLengthLimit) : output;

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
                return fullPath;
            }
            catch (Exception ex)
            {
                return $"❌ 保存失败: {ex.Message}";
            }
        }


    }
}