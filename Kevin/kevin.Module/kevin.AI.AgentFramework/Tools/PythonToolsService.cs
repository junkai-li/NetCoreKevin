using Common;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tools;
using Kevin.Common.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        // 🛡️ 安全护栏：受限制的配置文件
        private static readonly string[] restrictedFiles = [
            "appsettings.json",
            "appsettings.development.json",
            "appsettings.test.json"
        ]; 
        private readonly IAIShareInfoService _aIShareInfoService;

        public PythonToolsService(IAIShareInfoService aIShareInfoService)
        {
            _aIShareInfoService=aIShareInfoService; 
        }


        /// <summary>
        /// 检查代码中是否包含受限制的配置文件路径
        /// </summary>
        /// <param name="code">Python代码或脚本路径</param>
        /// <returns>是否包含受限制路径</returns>
        private bool ContainsRestrictedFile(string code)
        {
            return restrictedFiles.Any(file =>
                code.IndexOf(file, StringComparison.OrdinalIgnoreCase) >= 0);
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
            try
            {
                if (_aIShareInfoService.GetData().AuthorizedDomainsList.Count == 0)
                    return; // 没有有效的前缀，等同于允许所有

                // 从代码中提取所有URL
                var matches = UrlRegex.Matches(code);
                foreach (Match match in matches)
                {
                    var url = match.Value;
                    var isAllowed = _aIShareInfoService.GetData().AuthorizedDomainsList.Any(prefix => url.Contains(prefix, StringComparison.OrdinalIgnoreCase));
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

        [Description("用于执行Python代码。包含安全护栏：配置文件访问拦截（禁止访问 appsettings.json 等）、HTTP请求域名白名单。")]
        public async Task<string> RunPythonCode([Description("需要执行的python代码。例如：'def main(name): return 'Hello ' + name.title() + '!'")]
                                         [Required]string code, [Description("超时时间（单位秒）：默认600秒")] int seconds = 600)
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

                // 🛡️ 安全护栏：受限制配置文件检查
                if (ContainsRestrictedFile(code))
                {
                    return "❌ 安全拦截：禁止访问配置文件（appsettings.json 等）。";
                }

                if (_aIShareInfoService.GetData().IsSecurityIntercept)
                {
                    var validationResult = PythonSecurityValidator.ValidatePythonCode(code);
                    if (!validationResult.IsValid)
                    {
                        var blockedList = string.Join("; ", validationResult.BlockedItems);
                        return $"❌ 安全校验失败: {blockedList}";
                    }
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
                using var process = Process.Start(start);
                if (process == null)
                {
                    return "❌ 无法启动 Shell 进程";
                }
                var stdout = process.StandardOutput.ReadToEndAsync();
                var stderr = process.StandardError.ReadToEndAsync();

                // 🛡️ 安全护栏 3：超时控制（600秒）
                if (!process.WaitForExit(seconds * 1000))
                {
                    process.Kill(entireProcessTree: true);
                    return $"❌ 命令执行超时（{seconds}秒），已强制终止。";
                }
                // 获取输出
                output = stdout.Result;
                string error = stderr.Result;
                process.WaitForExit(); // 等待进程结束 
                if (!string.IsNullOrEmpty(error))
                {
                    return $"❌ 执行失败: {error}";
                }
                if (string.IsNullOrWhiteSpace(output))
                {
                    output = "Python脚本执行完成，但没有输出结果。";
                }
                return $"Python脚本已保存路径为：{saveResult}  \n 执行结果如下：\n" + (output.Length > _aIShareInfoService.GetData().ContentLengthLimit ? SystemPrompt.ContentLimitPromptText + StringHelper.SubstringText(output, _aIShareInfoService.GetData().ContentLengthLimit) : output);

            }
            catch (Exception ex)
            {
                return $"❌ 执行失败: {ex.Message}";
            }
        }

        [Description("把传入的python代码保存为 .py 文件，返回保存的完整路径，失败返回以 ❌ 开头的错误信息")]
        public async Task<string> SavePythonToFile([Required][Description("需要保存的python代码。例如：'def main(name): return 'Hello ' + name.title() + '!'\"")] string code, string relativeDir = "Skills/python-skills/tmp", string? fileName = null)
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