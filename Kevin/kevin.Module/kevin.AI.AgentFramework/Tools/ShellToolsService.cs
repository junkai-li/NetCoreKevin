using Common;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Interfaces.Tools;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace kevin.AI.AgentFramework.Tools
{
    public class ShellToolsService : IShellToolsService
    {
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // run_shell — 一个 Shell 工具做一切（含安全护栏）
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        // 🛡️ 安全护栏 1：危险命令黑名单
        private static string[] dangerousPatterns = [
            "rm -rf /", "rm -rf /*",       // 删除根目录
        "sudo ",                        // 提权
        "shutdown", "reboot",           // 系统操作
        "> /dev/",                      // 设备写入
        ":(){ :|:& };:",                // Fork bomb
        "mkfs.",                        // 格式化
        "dd if=",                       // 磁盘覆写
        "format ",                      // Windows 格式化
        "del /f /s /q",                 // Windows 递归删除
    ];

        // 用于从命令中提取URL的正则表达式
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
        /// 检查命令中的URL是否在授权域名白名单中
        /// 支持授权域名格式：
        /// - example.com (仅域名)
        /// - https://example.com (带协议)
        /// - https://example.com/api (带路径前缀)
        /// </summary>
        /// <param name="command">要执行的命令</param>
        /// <exception cref="UnauthorizedAccessException"></exception>
        private void AuthorizedDomainsCheck(string command)
        {
            if (_data == default) return;

            try
            {
                if (_authorizedDomains.Count == 0)
                    return; // 没有有效的前缀，等同于允许所有  
                // 从命令中提取所有URL
                var matches = UrlRegex.Matches(command);
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
        /// 检查单个URL是否在授权白名单中
        /// </summary>
        /// <param name="url">要检查的URL</param>
        /// <param name="allowedPrefixes">授权的域名前缀列表</param>
        /// <returns></returns>
        private bool IsUrlAllowed(string url, List<string> allowedPrefixes)
        {
            foreach (var prefix in allowedPrefixes)
            {
                // 检查方式1：URL直接包含授权前缀（支持各种格式）
                if (url.IndexOf(prefix, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }

                // 检查方式2：提取URL的域名部分进行匹配
                // 处理授权域名不带协议的情况，如 "example.com"
                try
                {
                    var uri = new Uri(url);
                    var host = uri.Host; // 获取域名，如 "example.com"

                    // 如果授权前缀是域名格式（不带协议），直接匹配域名
                    if (!prefix.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                        !prefix.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        // 检查域名是否匹配或域名是否以授权前缀结尾（处理子域名情况）
                        if (host.Equals(prefix, StringComparison.OrdinalIgnoreCase) ||
                            host.EndsWith($".{prefix}", StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
                catch { }
            }

            return false;
        }

        [Description("执行 Shell 命令。通过操作系统原生 Shell 执行命令（Windows 用 cmd，Linux/Mac 用 bash）。包含安全护栏：危险命令阻止、HTTP请求域名白名单、输出截断（50KB）、超时控制（60秒）。")]
        public async Task<string> RunShell(
            [Description("要执行的 Shell 命令。例如：'pwsh -File /path/to/script.ps1' 或 'dir'")] string command,
            [Description("命令执行的工作目录（可选）。如果不指定，使用当前目录。")] string? workingDirectory = null)
        {
            try
            {
                // 🛡️ 安全护栏 1：危险命令检查
                if (dangerousPatterns.Any(d => command.Contains(d, StringComparison.OrdinalIgnoreCase)))
                {
                    return "❌ 安全拦截：检测到危险命令，已阻止执行。";
                }

                // 🛡️ 安全护栏 2：HTTP请求域名白名单检查
                try
                {
                    AuthorizedDomainsCheck(command);
                }
                catch (UnauthorizedAccessException ex)
                {
                    return $"❌ 授权拦截：{ex.Message}";
                }

                Console.WriteLine($"🔧 正在执行 Shell 命令：{command}");

                // 🔑 跨平台 Shell 适配：
                //   Windows → cmd /c "command"   （原生命令提示符）
                //   Linux/Mac → bash -c "command" （原生 Bash）
                //
                // 为什么不直接用 pwsh？
                //   SKILL.md 中的命令已经是 "pwsh -File ..."
                //   如果 RunShell 也用 pwsh -Command 包裹 → pwsh 套 pwsh（冗余嵌套）
                //   用原生 shell 分发 → pwsh -File 直接执行，零嵌套 
                var isWindows = OperatingSystem.IsWindows();
                var processInfo = new ProcessStartInfo
                {
                    FileName = isWindows ? "cmd" : "bash",
                    Arguments = isWindows ? $"/c {command}" : $"-c \"{command.Replace("\"", "\\\"")}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // 设置工作目录
                if (!string.IsNullOrWhiteSpace(workingDirectory) && Directory.Exists(workingDirectory))
                {
                    processInfo.WorkingDirectory = workingDirectory;
                }

                using var process = Process.Start(processInfo);
                if (process == null)
                {
                    return "❌ 无法启动 Shell 进程";
                }

                var stdout = process.StandardOutput.ReadToEnd();
                var stderr = process.StandardError.ReadToEnd();

                // 🛡️ 安全护栏 3：超时控制（60秒）
                if (!process.WaitForExit(60_000))
                {
                    process.Kill(entireProcessTree: true);
                    return "❌ 命令执行超时（60秒），已强制终止。";
                }

                var result = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(stdout))
                {
                    result.AppendLine(stdout.Trim());
                }
                if (!string.IsNullOrWhiteSpace(stderr))
                {
                    result.AppendLine($"⚠️ stderr: {stderr.Trim()}");
                }
                if (process.ExitCode != 0)
                {
                    result.AppendLine($"⚠️ 退出码: {process.ExitCode}");
                }

                var output = result.Length > 0 ? result.ToString() : "(命令执行成功，无输出)";
                return output.Length > _contentLengthLimit ? SystemPrompt.ContentLimitPromptText + StringHelper.SubstringText(output, _contentLengthLimit) : output;
            }
            catch (Exception ex)
            {
                return $"❌ 执行失败: {ex.Message}";
            }
        }
    }
}