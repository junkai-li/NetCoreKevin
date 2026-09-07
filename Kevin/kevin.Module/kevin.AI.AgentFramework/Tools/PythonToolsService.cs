using Common;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tools;
using kevin.AI.AgentFramework.ScriptRunners;
using Kevin.Common.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace kevin.AI.AgentFramework.Tools
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // RunPython — 一个 执行Python脚本的工具
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public class PythonToolsService : IPythonToolsService
    {
        // RunPythonCode 生成的临时脚本目录（沿用历史位置：应用目录下 Pys）
        private const string TempScriptDir = "Pys";

        private const int DefaultTimeoutSeconds = 600;

        // 上限：防止模型传入超大秒数把一个请求吊住不放（旧实现是 seconds*1000 传给 WaitForExit(int)，
        // 超过约 24.8 天会整数溢出成负数，WaitForExit 直接不等待）
        private static readonly TimeSpan MaxTimeout = TimeSpan.FromHours(1);

        // 临时脚本可能含敏感串，进程内首次写入前清理一次历史残留
        private static int _staleScriptsSwept;

        private readonly IAIShareInfoService _aIShareInfoService;

        public PythonToolsService(IAIShareInfoService aIShareInfoService)
        {
            _aIShareInfoService = aIShareInfoService;
        }

        [Description("用于执行Python代码。包含安全护栏：配置文件访问拦截（禁止访问 appsettings.json 等）、HTTP请求域名白名单。")]
        public async Task<string> RunPythonCode([Description("需要执行的python代码。例如：'def main(name): return 'Hello ' + name.title() + '!'")]
                                         [Required]string code, [Description("超时时间（单位秒）：默认600秒")] int seconds = 600)
        {
            string? tempScriptPath = null;
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return "执行Py代码为空。";
                }

                // 🛡️ 安全护栏：HTTP请求域名白名单检查
                var unauthorizedUrl = CommandGuardrails.FindUnauthorizedUrl(
                    code, _aIShareInfoService.GetData()?.AuthorizedDomainsList);
                if (unauthorizedUrl is not null)
                {
                    return CommandGuardrails.UnauthorizedDomainMessage(unauthorizedUrl);
                }

                // 🛡️ 安全护栏：受限制配置文件检查
                // 与 run_shell 共用 CommandGuardrails：通配符、8.3 短名、引号拆分、变量拼接等变体同样拦住
                if (CommandGuardrails.ContainsRestrictedFile(code))
                {
                    return CommandGuardrails.RestrictedFileBlockedMessage;
                }

                if (_aIShareInfoService.GetData()?.IsSecurityIntercept == true)
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
                tempScriptPath = CreateTempScript(code);
                Console.WriteLine($"🔧 正在执行Py脚本: {tempScriptPath}");

                var timeout = ResolveTimeout(seconds);
                // 解释器选择、UTF-8 对齐、超时与进程树回收统一走 ScriptProcessRunner，
                // 不再在这里自己拼命令行（旧写法 start.Arguments = 路径，遇到含空格的应用目录会被拆断）
                var (startInfo, payloadArguments) = ScriptProcessRunner.CreateLaunch(tempScriptPath);
                foreach (var payload in payloadArguments)
                {
                    startInfo.ArgumentList.Add(payload);
                }

                var execution = await ScriptProcessRunner.ExecuteAsync(startInfo, timeout);
                if (execution.TimedOut)
                {
                    return $"❌ 命令执行超时（{(int)timeout.TotalSeconds}秒），已强制终止。";
                }
                if (execution.Canceled)
                {
                    return "❌ 执行已取消。";
                }
                if (!string.IsNullOrEmpty(execution.StdErr))
                {
                    return $"❌ 执行失败: {execution.StdErr}";
                }
                if (execution.ExitCode != 0)
                {
                    return $"❌ 执行失败: 退出码 {execution.ExitCode}";
                }

                var output = string.IsNullOrWhiteSpace(execution.StdOut)
                    ? "Python脚本执行完成，但没有输出结果。"
                    : execution.StdOut;
                var contentLimit = _aIShareInfoService.GetData()?.ContentLengthLimit ?? 0;
                // 只有真的截断时才拼截断提示语：ContentLengthLimit 配成 0 时旧实现会把整段输出
                // 换成一句提示词，模型看到的是“数据量过大无法展示”，而实际一个字都没截
                var body = contentLimit > 0 && output.Length > contentLimit
                    ? SystemPrompt.ContentLimitPromptText + StringHelper.SubstringText(output, contentLimit)
                    : output;
                return $"执行结果如下：\n{body}";
            }
            catch (Exception ex)
            {
                return $"❌ 执行失败: {ex.Message}";
            }
            finally
            {
                // 临时脚本用完即删：旧实现每次调用都在 Pys 目录留一个 .py，只写不删
                CleanupTempScript(tempScriptPath);
            }
        }

        /// <summary>
        /// 超时时间兜底：非法值回落到默认值，并夹到上限
        /// </summary>
        private static TimeSpan ResolveTimeout(int seconds)
        {
            var safeSeconds = seconds <= 0 ? DefaultTimeoutSeconds : seconds;
            var maxSeconds = (int)MaxTimeout.TotalSeconds;
            if (safeSeconds > maxSeconds)
            {
                safeSeconds = maxSeconds;
            }
            return TimeSpan.FromSeconds(safeSeconds);
        }

        /// <summary>
        /// 写入本次执行的临时脚本
        /// </summary>
        private static string CreateTempScript(string code)
        {
            var baseDir = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
            var fullDir = Path.Combine(baseDir, TempScriptDir);
            Directory.CreateDirectory(fullDir);
            // 进程内只扫一次，清掉上次异常退出（进程被杀、容器重启）残留的脚本
            if (Interlocked.Exchange(ref _staleScriptsSwept, 1) == 0)
            {
                SweepStaleScripts(fullDir);
            }
            var fullPath = Path.Combine(fullDir, $"py_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}.py");
            // 以 UTF-8 无 BOM 保存，保证跨平台兼容且 Python 能正确识别
            File.WriteAllText(fullPath, code, new UTF8Encoding(false));
            return fullPath;
        }

        /// <summary>
        /// 清理超过 1 小时的残留临时脚本（尽力而为，失败不影响主流程）
        /// </summary>
        private static void SweepStaleScripts(string fullDir)
        {
            try
            {
                var expireBefore = DateTime.UtcNow - TimeSpan.FromHours(1);
                foreach (var file in Directory.EnumerateFiles(fullDir, "*.py"))
                {
                    try
                    {
                        if (File.GetLastWriteTimeUtc(file) < expireBefore)
                        {
                            File.Delete(file);
                        }
                    }
                    catch
                    {
                        // 单个文件被占用或已删除，跳过
                    }
                }
            }
            catch
            {
                // 目录不可枚举，忽略
            }
        }

        /// <summary>
        /// 删除本次执行的临时脚本（尽力而为）
        /// </summary>
        private static void CleanupTempScript(string? tempScriptPath)
        {
            if (string.IsNullOrEmpty(tempScriptPath))
            {
                return;
            }
            try
            {
                if (File.Exists(tempScriptPath))
                {
                    File.Delete(tempScriptPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ 临时脚本清理失败：{tempScriptPath}，{ex.Message}");
            }
        }

        [Description("把传入的python代码保存为 .py 文件，返回保存的完整路径，失败返回以 ❌ 开头的错误信息")]
        public Task<string> SavePythonToFile([Required][Description("需要保存的python代码。例如：'def main(name): return 'Hello ' + name.title() + '!'\"")] string code, string relativeDir = "Skills/python-skills/tmp", string? fileName = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return Task.FromResult("❌ 保存失败: 代码内容为空。");
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
                return Task.FromResult(fullPath);
            }
            catch (Exception ex)
            {
                return Task.FromResult($"❌ 保存失败: {ex.Message}");
            }
        }


    }
}
