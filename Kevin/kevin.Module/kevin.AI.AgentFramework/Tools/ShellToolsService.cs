using Common;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tools;
using Microsoft.Agents.AI.Tools.Shell;
using System.ComponentModel;
using System.Text;

namespace kevin.AI.AgentFramework.Tools
{
    /// <summary>
    /// 基于 <see cref="LocalShellExecutor"/>（Microsoft.Agents.AI.Tools.Shell）的 Shell 工具实现：
    /// 进程启动、stdout/stderr 采集、输出首尾截断、超时终止进程树等由框架执行器负责；
    /// 业务护栏（危险命令、受限配置文件、授权域名白名单）通过 <see cref="ShellPolicy"/> 承载，
    /// 策略闭包按次读取 Scoped 的 <see cref="IAIShareInfoService"/>，保证按应用动态生效。
    /// </summary>
    public class ShellToolsService : IShellToolsService
    {
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        // run_shell — 一个 Shell 工具做一切（含安全护栏）
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        // 护栏规则（黑名单 / 受限文件归一化匹配 / 域名白名单）统一在 CommandGuardrails，
        // 与 PythonToolsService、技能脚本 Runner 共用一份实现

        // 🔑 保持与历史实现一致的 Shell 方言：Windows 用 cmd、Linux/Mac 用 bash。
        //   框架默认在 Windows 上优先解析为 pwsh/powershell，而 SKILL.md 里的命令本身就是
        //   "pwsh -File ..."，再套一层 PowerShell 会造成 pwsh 嵌套与 && 语法差异。
        //   cmd 不支持 Persistent 模式，因此这里统一使用 ShellMode.Stateless。
        private static readonly string DefaultShell = OperatingSystem.IsWindows() ? "cmd" : "bash";

        private readonly IAIShareInfoService _aIShareInfoService;

        // 安全护栏策略：deny 时 Reason 即为返回给模型的中文提示
        private readonly ShellPolicy _policy;

        // 单条输出流的缓冲上限，防止极端输出撑爆内存
        private const int MaxStreamBufferBytes = 8 * 1024 * 1024;

        // 下限：ContentLengthLimit 配得极小时也保证执行器侧不会先把内容切碎
        private const int MinStreamBufferBytes = 4 * 1024;

        // 临时脚本可能含敏感串（如 Bearer Token），进程内首次写入前清理一次历史残留
        private static int _staleScriptsSwept;

        // 把控制台输出代码页切到 UTF-8（并丢弃 chcp 自己的提示语）。
        // 实测：同一个 cmd 实例里 chcp 后再 echo，仍按旧编码写输出（尽管 chcp 回报 65001），
        // 只有它新起的子 cmd 会吃到新代码页，所以必须“先 chcp 再嵌套一层 cmd”才能拿到 UTF-8
        private const string Utf8CodePageLine = "chcp 65001>nul";

        private const string Utf8CodePageSwitch = Utf8CodePageLine + " & cmd /c ";

        private static readonly UTF8Encoding NoBomUtf8 = new(false);

        public ShellToolsService(IAIShareInfoService aIShareInfoService)
        {
            _aIShareInfoService = aIShareInfoService;
            _policy = new ShellPolicy(custom: request => GuardrailOutcome(request.Command));
        }

        /// <summary>
        /// 汇总所有护栏规则，返回允许/拒绝结论（拒绝时携带提示语）
        /// </summary>
        private ShellPolicyOutcome? GuardrailOutcome(string command)
        {
            var sharedInfo = _aIShareInfoService.GetData();
            if (sharedInfo is { IsSecurityIntercept: true })
            {
                // 🛡️ 安全护栏 1：危险命令检查
                if (CommandGuardrails.ContainsDangerousPattern(command))
                {
                    return ShellPolicyOutcome.Deny(CommandGuardrails.DangerousCommandBlockedMessage);
                }

                // 🛡️ 安全护栏 1.5：受限制配置文件检查
                if (CommandGuardrails.ContainsRestrictedFile(command))
                {
                    return ShellPolicyOutcome.Deny(CommandGuardrails.RestrictedFileBlockedMessage);
                }
            }

            // 🛡️ 安全护栏 2：HTTP请求域名白名单检查（不受 IsSecurityIntercept 开关影响）
            return AuthorizedDomainsOutcome(command);
        }

        /// <summary>
        /// 检查命令中的URL是否在授权域名白名单中
        /// 支持授权域名格式：
        /// - example.com (仅域名)
        /// - https://example.com (带协议)
        /// - https://example.com/api (带路径前缀)
        /// </summary>
        /// <param name="command">要执行的命令</param>
        /// <returns>拒绝时返回带提示语的结论，允许时返回 null</returns>
        private ShellPolicyOutcome? AuthorizedDomainsOutcome(string command)
        {
            var unauthorizedUrl = CommandGuardrails.FindUnauthorizedUrl(
                command, _aIShareInfoService.GetData()?.AuthorizedDomainsList);
            return unauthorizedUrl is null
                ? null
                : ShellPolicyOutcome.Deny(CommandGuardrails.UnauthorizedDomainMessage(unauthorizedUrl));
        }

        /// <summary>
        /// ⚠️ Windows cmd 兼容处理（两件事，一并在此完成）：
        /// 1) 引号：cmd.exe 不遵守 .NET ArgumentList 的 \" 转义规则，含双引号的命令会被多一层引号
        ///    传给子进程（如 <c>powershell -Command "1+1"</c> 会退化为字符串字面量），因此这类命令
        ///    落到临时 .cmd 脚本，由 cmd 按原生规则解析脚本内容，既保留双引号，也保留
        ///    <c>&amp;&amp;</c>、<c>%VAR%</c>、<c>dir /b</c> 等 cmd 语法（bash 走 execve 无此问题）。
        /// 2) 输出编码：cmd 默认按系统 OEM 代码页（中文机器上 936/GBK）写 stdout/stderr，
        ///    而 <see cref="LocalShellExecutor"/> 固定按 UTF-8 解码（<c>LocalShellExecutorOptions</c>
        ///    共 11 个属性，没有任何编码旋钮），中文输出会全变成替换符。唯一可行的修法是让 cmd 这边
        ///    改成输出 UTF-8 字节：
        ///    不含双引号的命令用 <c>chcp 65001&gt;nul &amp; cmd /c 原命令</c>——同一个 cmd 实例
        ///    改了代码页仍按旧编码输出，必须嵌套一层子 cmd 才吃到 65001；嵌套同时保留了 cmd /c
        ///    原生语义（<c>for %f</c> 不需改写成 <c>%%f</c>，<c>%CD%</c>、<c>&amp;&amp;</c> 照常）；
        ///    含双引号的命令落到临时 .cmd 脚本，批处理里的文本是字节透传，首行再补一句 chcp
        ///    （实测 <c>dir</c> 这类 cmd 自己排版输出的命令仍依赖代码页，不能省）。
        /// </summary>
        /// <param name="command">安全护栏已放行的原始命令</param>
        /// <param name="scriptPath">生成的临时脚本路径，未使用临时脚本时为 null（调用方负责清理）</param>
        /// <returns>实际交给 Shell 执行的命令</returns>
        private static string BuildExecutableCommand(string command, out string? scriptPath)
        {
            scriptPath = null;
            if (!OperatingSystem.IsWindows() || DefaultShell != "cmd")
            {
                return command;
            }

            if (!command.Contains('"'))
            {
                // 嵌套一层子 cmd：外层只负责切代码页，原命令在新代码页下执行；
                // 用 & 而不是 &&，chcp 失败也要让原命令照跑
                return Utf8CodePageSwitch + command;
            }

            try
            {
                var scriptDir = Path.Combine(Path.GetTempPath(), "kevin_ai_shell");
                _ = Directory.CreateDirectory(scriptDir);
                // 进程内只扫一次，清掉上次异常退出（进程被杀、容器重启）残留的脚本
                if (Interlocked.Exchange(ref _staleScriptsSwept, 1) == 0)
                {
                    SweepStaleScripts(scriptDir);
                }
                // 路径自身不含双引号，.NET 只会做最外层包裹，cmd 可正常解析（含空格时也一样）
                scriptPath = Path.Combine(scriptDir, $"{Guid.NewGuid():N}.cmd");
                // 脚本固定按 UTF-8 无 BOM 写：chcp 在首行生效，后续行 cmd 就按 UTF-8 读，
                // 命令里的中文字面量不会在执行前变形；但 BOM 会让 cmd 把首行当非法命令
                File.WriteAllText(scriptPath,
                    $"@echo off\r\n{Utf8CodePageLine}\r\n{command}\r\n", NoBomUtf8);
                return scriptPath;
            }
            catch (Exception ex)
            {
                // 写临时脚本失败（目录无写权限、磁盘满、被杀软/AppLocker 拦截等）时回退为直接执行。
                // 回退会让含双引号的命令退化成旧的错误语义，必须留痕，否则线上无从判断为何失效。
                // scriptPath 若已赋值则保留，交给调用方 finally 清掉写一半的残留文件。
                Console.WriteLine($"⚠️ 临时脚本写入失败，已回退为直接执行（双引号命令可能解析异常）：{ex.Message}");
                return command;
            }
        }

        /// <summary>
        /// 清理超过 1 小时的残留临时脚本（尽力而为，失败不影响主流程）
        /// </summary>
        private static void SweepStaleScripts(string scriptDir)
        {
            try
            {
                var expireBefore = DateTime.UtcNow - TimeSpan.FromHours(1);
                foreach (var file in Directory.EnumerateFiles(scriptDir, "*.cmd"))
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
        /// 输出缓冲上限换算：MaxOutputBytes 以 UTF-8 <b>字节</b>计，而 ContentLengthLimit 按<b>字符</b>截断。
        /// 一个汉字占 3 字节，直接透传会让中文输出实际可用量缩水到约 1/3（实测 1000 字节仅得 363 字符），
        /// 因此放宽 4 倍，再由调用方按字符做与历史实现一致的兜底截断。
        /// </summary>
        private static int ResolveMaxOutputBytes(int contentLimit)
        {
            if (contentLimit <= 0)
            {
                return 64 * 1024;
            }
            var bytes = (long)contentLimit * 4;
            if (bytes < MinStreamBufferBytes)
            {
                return MinStreamBufferBytes;
            }
            return bytes > MaxStreamBufferBytes ? MaxStreamBufferBytes : (int)bytes;
        }

        /// <summary>
        /// 清理临时脚本（尽力而为）
        /// </summary>
        private static void CleanupScriptFile(string? scriptPath)
        {
            if (scriptPath is null)
            {
                return;
            }
            try
            {
                if (File.Exists(scriptPath))
                {
                    File.Delete(scriptPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ 临时脚本清理失败：{scriptPath}，{ex.Message}");
            }
        }

        [Description("执行 Shell 命令。通过操作系统原生 Shell 执行命令（Windows 用 cmd，Linux/Mac 用 bash），每次调用为独立 Shell 进程。包含安全护栏：危险命令阻止、配置文件访问拦截（禁止访问 appsettings.json 等）、HTTP请求域名白名单、输出截断、超时控制。")]
        public async Task<string> RunShell(
            [Description("要执行的 Shell 命令。例如：'pwsh -File /path/to/script.ps1' 或 'dir'")] string command,
            [Description("命令执行的工作目录（可选）。如果不指定，使用当前目录。")] string? workingDirectory = null,
               [Description("超时时间（单位秒）：默认600秒")] int seconds = 600
            )
        {
            // 临时脚本路径先声明在这个作用域：下面两个 catch 都要能读到它做补清理
            string? scriptPath = null;
            try
            {
                if (string.IsNullOrWhiteSpace(command))
                {
                    return "❌ 命令为空，无可执行内容。";
                }

                // 🛡️ 安全护栏：危险命令 / 受限配置文件 / 域名白名单统一由 ShellPolicy 求值
                var decision = _policy.Evaluate(new ShellRequest(command, workingDirectory));
                if (!decision.Allowed)
                {
                    return string.IsNullOrWhiteSpace(decision.Reason) ? "❌ 安全拦截：命令已被安全策略阻止。" : decision.Reason!;
                }

                Console.WriteLine($"🔧 正在执行 Shell 命令：{command}");

                var contentLimit = _aIShareInfoService.GetData()?.ContentLengthLimit ?? 0;
                var timeoutSeconds = seconds > 0 ? seconds : 600;
                // 🛡️ Windows cmd：在此统一完成双引号兜底与“输出代码页切到 UTF-8”。
                // 写脚本与下面那个带 finally 的 try 之间还有“建执行器”一步，
                // 那一步失败时内层 finally 还进不去，所以要由外层 catch 补清理
                var executableCommand = BuildExecutableCommand(command, out scriptPath);
                // ⚠️ 执行器拿到的是改写后的字符串（GUID 脚本路径 / chcp 前缀），对它求值会失真：
                //    脚本路径分支下护栏整条 vacuously 放行，前缀分支下 "&&" 一类规则又会误拦。
                //    因此两种分支都锁定回原始 command 复核。
                var executorPolicy = new ShellPolicy(custom: _ => GuardrailOutcome(command));

                var executor = new LocalShellExecutor(new LocalShellExecutorOptions
                {
                    Mode = ShellMode.Stateless,
                    Shell = DefaultShell,
                    Policy = executorPolicy,
                    // 工作目录不存在时不传，由执行器回退到当前进程目录（与历史实现一致）
                    WorkingDirectory = !string.IsNullOrWhiteSpace(workingDirectory) && Directory.Exists(workingDirectory)
                        ? workingDirectory
                        : null,
                    // 该项仅对 Persistent 模式生效（Stateless 下是 no-op）；显式写 false，
                    // 以免日后切换到 Persistent 时被默认值 true 悄悄改变命令语义
                    ConfineWorkingDirectory = false,
                    // 🛡️ 安全护栏 6：不继承宿主进程环境变量，防止 <c>set</c> / <c>echo %XXX%</c> 直接 dump
                    //    部署时用环境变量注入的密钥。实测 shell 自定位所需的 PATH / SystemRoot / 当前目录均保留，
                    //    外部工具（dir、powershell 等）调用不受影响
                    CleanEnvironment = true,
                    // 🛡️ 安全护栏 3：超时控制（超时后执行器会终止整个进程树）
                    Timeout = TimeSpan.FromSeconds(timeoutSeconds),
                    // 🛡️ 安全护栏 4：输出上限（按流保留首尾），未配置时沿用框架默认 64KiB
                    MaxOutputBytes = ResolveMaxOutputBytes(contentLimit)
                });

                try
                {
                    // 🛡️ 安全护栏 5：Stateless 下不常驻 Shell 进程，用完即放，避免多会话间状态串台
                    await using (executor)
                    {
                        var result = await executor.RunAsync(executableCommand).ConfigureAwait(false);

                        if (result.TimedOut)
                        {
                            return $"❌ 命令执行超时（{timeoutSeconds}秒），已强制终止。";
                        }

                        var outputBuilder = new StringBuilder();
                        if (!string.IsNullOrWhiteSpace(result.Stdout))
                        {
                            outputBuilder.AppendLine(result.Stdout.Trim());
                        }
                        if (!string.IsNullOrWhiteSpace(result.Stderr))
                        {
                            outputBuilder.AppendLine($"⚠️ stderr: {result.Stderr.Trim()}");
                        }
                        if (result.Truncated)
                        {
                            outputBuilder.AppendLine("⚠️ 输出超出上限，已保留首尾内容。");
                        }
                        if (result.ExitCode != 0)
                        {
                            outputBuilder.AppendLine($"⚠️ 退出码: {result.ExitCode}");
                        }

                        var output = outputBuilder.Length > 0 ? outputBuilder.ToString() : "(命令执行成功，无输出)";
                        // 真正发生截断时才追加截断提示词，避免误导模型
                        return contentLimit > 0 && output.Length > contentLimit
                            ? SystemPrompt.ContentLimitPromptText + StringHelper.SubstringText(output, contentLimit)
                            : output;
                    }
                }
                finally
                {
                    CleanupScriptFile(scriptPath);
                }
            }
            catch (ShellCommandRejectedException ex)
            {
                // 执行器内部策略未放行（与上面的预检查同源）。改写阶段可能已落了临时脚本，补一次清理（幂等）
                CleanupScriptFile(scriptPath);
                // 包会给消息加上 "Command rejected by policy: " 英文前缀，
                // 而我们的拦截原因本身已带 ❌ 提示语，取 ❌ 之后的原文返回，避免前缀重复与英文泄漏。
                var reason = ex.Message;
                var flagIndex = reason.IndexOf('❌');
                return flagIndex >= 0 ? reason[flagIndex..] : $"❌ 安全拦截：{reason}";
            }
            catch (Exception ex)
            {
                // 执行器构造失败等发生在内层 finally 之前，这里兜住避免脚本残留（内容可能含敏感串）
                CleanupScriptFile(scriptPath);
                return $"❌ 执行失败: {ex.Message}";
            }
        }
    }
}