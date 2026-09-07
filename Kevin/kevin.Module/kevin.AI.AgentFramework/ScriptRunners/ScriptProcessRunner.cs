using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace kevin.AI.AgentFramework.ScriptRunners
{
    /// <summary>
    /// 技能脚本子进程执行核心。
    /// PySubprocessScriptRunner 与 MyPySubprocessScriptRunner 共用：
    /// 两份实现此前各自复制了一遍进程启动/读流/收尾逻辑，已经漂移出同类问题
    /// （无超时、Kill 不带进程树、stdin 开而不关），统一到这里后只会修一处。
    /// </summary>
    public static class ScriptProcessRunner
    {
        /// <summary>
        /// 脚本默认最长执行时间。
        /// 旧实现只跟随调用方 CancellationToken，调用方不传令牌时脚本挂死即永久占用请求。
        /// </summary>
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(10);

        /// <summary>
        /// 单个输出流保留的最大字符数，超出部分丢弃并标记截断，避免刷屏脚本打爆内存。
        /// </summary>
        public const int MaxOutputChars = 200_000;

        /// <summary>
        /// 探测系统命令是否可用时的超时，避免 where/which 自身卡住拖死整次调用
        /// </summary>
        private static readonly TimeSpan CommandProbeTimeout = TimeSpan.FromSeconds(5);

        private static int _encodingProviderRegistered;

        // 解释器可用性只探测一次并缓存：服务进程生命周期内 PATH 不会变，
        // 否则每次执行技能脚本都要额外起 where/which 子进程
        private static readonly Lazy<string?> _pythonCommand = new(
            static () => IsCommandAvailable("python3") ? "python3" : IsCommandAvailable("python") ? "python" : null,
            LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// 解析技能脚本绝对路径，并确认它没有越过技能目录。
        /// 脚本内容目前是仓库固定文件、模型不可控，这里做纵深防御：
        /// 一旦将来 skill/script 名称改为外部可传，越界执行任意文件的路径先被堵死。
        /// </summary>
        public static string ResolveScriptFullPath(string skillPath, string scriptRelativePath)
        {
            var root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(skillPath));
            var fullPath = Path.GetFullPath(Path.Combine(skillPath, scriptRelativePath));
            var comparison = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            if (!fullPath.StartsWith(root + Path.DirectorySeparatorChar, comparison)
                && !string.Equals(fullPath, root, comparison))
            {
                throw new InvalidOperationException(
                    $"脚本 {scriptRelativePath} 不在技能目录 {root} 内，已拒绝执行");
            }
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Script not found: {fullPath}");
            }
            return fullPath;
        }

        /// <summary>
        /// 创建进程启动信息。
        /// 输入流默认重定向并在启动后立即关闭（见 RunAsync），探测类命令可传 redirectInput:false。
        /// </summary>
        public static ProcessStartInfo CreateStartInfo(
            string fileName,
            Encoding? outputEncoding = null,
            bool redirectInput = true)
        {
            // 注册编码提供程序以支持 GBK 等旧编码（Windows 常见），只需一次
            if (Interlocked.Exchange(ref _encodingProviderRegistered, 1) == 0)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            }

            // Windows 下默认使用 Default (ANSI/OEM)，非 Windows 使用 UTF-8
            // outputEncoding 用于覆盖该默认值：子进程被强制以 UTF-8 输出时，父进程必须同编码解码
            Encoding resolvedEncoding = outputEncoding
                ?? (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Encoding.Default : Encoding.UTF8);

            // 参数逐个走 ArgumentList，不拼接命令行字符串（由框架处理转义）
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,

                UseShellExecute = false, // 必须为 false 才能重定向流
                RedirectStandardInput = redirectInput,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = resolvedEncoding,
                StandardErrorEncoding = resolvedEncoding,
                CreateNoWindow = true
            };
            if (redirectInput)
            {
                // 只能在 RedirectStandardInput 为 true 时赋值，不能事后关掉否则抛异常
                startInfo.StandardInputEncoding = Encoding.UTF8;
            }
            return startInfo;
        }

        /// <summary>
        /// -EncodedCommand 的 Base64 长度上限。
        /// 整条命令行受 CreateProcess 的 32767 字符限制，UTF-16 + Base64 会把参数放大近 3 倍，
        /// 超限前主动报错，胜过抛一句看不出原因的 Win32Exception。
        /// </summary>
        private const int MaxEncodedCommandChars = 30_000;

        /// <summary>
        /// 按脚本后缀构造解释器启动信息，scriptArguments 是脚本自身的命令行参数（不含脚本路径）。
        /// PayloadArguments 必须以独立参数逐个追加到 ArgumentList：
        /// 把一串 "-NoProfile ... -Command \"...\"" 当作单个元素传，会被转义成一个整体参数，
        /// PowerShell 会把 "-NoProfile" 当命令名解析，.ps1 技能直接退出码 1。
        /// </summary>
        /// <remarks>
        /// 参数必须在建启动信息时一并给出：Windows 的 .ps1 走 -EncodedCommand，
        /// 参数是编进命令文本里的，事后往 ArgumentList 追加会被 PowerShell 当成残缺命令行。
        /// </remarks>
        public static (ProcessStartInfo StartInfo, IReadOnlyList<string> PayloadArguments) CreateLaunch(
            string scriptFullPath,
            IReadOnlyList<string>? scriptArguments = null)
        {
            scriptArguments ??= Array.Empty<string>();
            var ext = Path.GetExtension(scriptFullPath).ToLowerInvariant();
            switch (ext)
            {
                case ".py":
                    string? pythonCmd = AvailablePythonCommand;
                    if (pythonCmd == null)
                    {
                        throw new InvalidOperationException(
                            "Python environment is not installed. Please install Python (python3 or python) and ensure it is available in the system PATH."
                        );
                    }
                    var pyStartInfo = CreateStartInfo(pythonCmd, outputEncoding: Encoding.UTF8);
                    ForceUtf8Output(pyStartInfo, "PYTHONIOENCODING", "utf-8");
                    return (pyStartInfo, WithFirst(scriptFullPath, scriptArguments));

                case ".sh":
                    // Windows 上运行 bash 通常需要 WSL 或 Git Bash 且在 PATH 中，两者默认都是 UTF-8
                    var shStartInfo = CreateStartInfo("bash", outputEncoding: Encoding.UTF8);
                    return (shStartInfo, WithFirst(scriptFullPath, scriptArguments));

                case ".ps1":
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        // 命令里把 [Console]::OutputEncoding 强制为 UTF-8，父进程也按 UTF-8 解码，两边对齐
                        var psStartInfo = CreateStartInfo("powershell", outputEncoding: Encoding.UTF8);
                        var commandText = BuildWindowsPsCommand(scriptFullPath, scriptArguments);
                        var encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(commandText));
                        if (encoded.Length > MaxEncodedCommandChars)
                        {
                            throw new InvalidOperationException(
                                $"技能脚本参数过长（编码后 {encoded.Length} 字符，上限 {MaxEncodedCommandChars}），请减少参数或改用 Python 脚本");
                        }
                        return (psStartInfo, new[]
                        {
                            "-NoProfile",
                            "-NonInteractive",
                            "-ExecutionPolicy",
                            "Bypass",
                            "-EncodedCommand",
                            encoded
                        });
                    }
                    // Linux/macOS 使用 pwsh (PowerShell Core)：-File 之后的参数逐个原样进脚本 argv，且默认 UTF-8
                    var pwshArguments = new List<string>(scriptArguments.Count + 4)
                    {
                        "-NoProfile",
                        "-NonInteractive",
                        "-File",
                        scriptFullPath
                    };
                    pwshArguments.AddRange(scriptArguments);
                    return (CreateStartInfo("pwsh", outputEncoding: Encoding.UTF8), pwshArguments);

                default:
                    // 直接执行（适用于 .exe, .bat, .cmd 等），参数原样进 argv；
                    // 原生命令行工具输出跟随系统代码页，沿用默认解码
                    return (CreateStartInfo(scriptFullPath), scriptArguments);
            }
        }

        private static IReadOnlyList<string> WithFirst(string first, IReadOnlyList<string> rest)
        {
            var list = new List<string>(rest.Count + 1) { first };
            list.AddRange(rest);
            return list;
        }

        /// <summary>
        /// 把子进程输出钉到指定编码（已在环境里配置过的不覆盖），避免父子两边编码不一致。
        /// </summary>
        private static void ForceUtf8Output(ProcessStartInfo startInfo, string variableName, string value)
        {
            foreach (var key in startInfo.Environment.Keys.Cast<string>())
            {
                if (string.Equals(key, variableName, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }
            startInfo.Environment[variableName] = value;
        }

        /// <summary>
        /// Windows 下 PowerShell 的 -Command / -EncodedCommand 命令体（含脚本参数）。
        /// 路径含单引号会截断这里的单引号包裹，直接拒绝而不是转义出意料之外的命令。
        /// </summary>
        /// <remarks>
        /// 实测（本机 Windows PowerShell 5.1）对比三种传参形态：
        /// -Command "^& ^{ ... }" --k v ：命令之后的 argv 被空格重切，含空格的值会裂成两个参数；
        /// -File script --k v ：参数精确，但子进程按 GBK 输出中文，父进程按 UTF-8 解码即乱码；
        /// -EncodedCommand ：命令文本走 Base64 进 argv，值精确、可强制 UTF-8 输出，故采用。
        /// </remarks>
        public static string BuildWindowsPsCommand(string scriptFullPath, IReadOnlyList<string>? scriptArguments = null)
        {
            if (scriptFullPath.Contains('\'', StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"脚本路径包含单引号，无法安全传给 PowerShell：{scriptFullPath}");
            }
            var builder = new StringBuilder();
            builder.Append("& { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8; ")
                .Append("$OutputEncoding = [System.Text.Encoding]::UTF8; ")
                .Append("& ").Append(QuotePsLiteral(scriptFullPath));
            if (scriptArguments != null)
            {
                foreach (var argument in scriptArguments)
                {
                    builder.Append(' ').Append(ToPsArgument(argument));
                }
            }
            builder.Append(" }");
            return builder.ToString();
        }

        /// <summary>
        /// 把单个参数转成 PowerShell 命令文本里的写法。
        /// 开关必须从 --name 改写成 -name：实测 & 'x.ps1' --name v 不按名绑定，
        /// 而是把 "--name" 当位置参数塞给第一个参数（脚本 param([string]$name) 收到的是字符串 --name），
        /// 而 -name v 能正确绑定，脚本没有同名参数时也只是原样落进 $args，不会报错。
        /// 其余值一律走单引号字面量：单引号串内不展开变量、反引号不转义，只需把 ' 翻倍。
        /// </summary>
        private static string ToPsArgument(string argument)
        {
            if (IsPsSwitch(argument))
            {
                return "-" + argument[2..];
            }
            return QuotePsLiteral(argument);
        }

        /// <summary>
        /// 形如 --name / --max-size 的纯开关（双连字符 + 字母开头的标识符），可安全地按参数名传给 PowerShell
        /// </summary>
        private static bool IsPsSwitch(string argument)
        {
            if (argument.Length < 4 || !argument.StartsWith("--", StringComparison.Ordinal))
            {
                return false;
            }
            for (int i = 2; i < argument.Length; i++)
            {
                char c = argument[i];
                bool legal = i == 2
                    ? char.IsAsciiLetter(c)
                    : char.IsAsciiLetterOrDigit(c) || c is '_' or '-' or '.';
                if (!legal)
                {
                    return false;
                }
            }
            return true;
        }

        private static string QuotePsLiteral(string value) =>
            "'" + value.Replace("'", "''", StringComparison.Ordinal) + "'";

        /// <summary>
        /// 获取可用的 Python 命令（优先 python3，其次 python），都不可用返回 null
        /// </summary>
        public static string? AvailablePythonCommand => _pythonCommand.Value;

        /// <summary>
        /// 检测指定命令是否存在于系统 PATH 中
        /// </summary>
        public static bool IsCommandAvailable(string command)
        {
            try
            {
                var startInfo = CreateStartInfo(
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which",
                    redirectInput: false); // 探测命令不读输入，关掉重定向避免留一个没人管的输入管道
                startInfo.ArgumentList.Add(command); // 命令名走参数列表，拼接字符串会在含空格的路径上裂开
                using var checkProcess = new Process { StartInfo = startInfo };
                checkProcess.Start();
                if (!checkProcess.WaitForExit(CommandProbeTimeout))
                {
                    KillProcessTree(checkProcess);
                    return false;
                }
                return checkProcess.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 子进程执行结果：调用方自行决定怎么解读退出码与输出。
        /// TimedOut / Canceled 时 ExitCode 可能取不到（进程已被强杀），值为 -1。
        /// </summary>
        public sealed record ProcessExecution(
            string StdOut,
            string StdErr,
            int ExitCode,
            bool TimedOut,
            bool Canceled);
        
        /// <summary>
        /// 启动进程、采集输出、超时后连同子孙进程一起回收。
        /// 不抛业务异常（启动失败仍抛），供 RunAsync 与 Python 工具等复用同一套超时/回收/编码纪律。
        /// </summary>
        public static async Task<ProcessExecution> ExecuteAsync(
            ProcessStartInfo startInfo,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            var executedTimeout = timeout ?? DefaultTimeout;
            using var process = new Process { StartInfo = startInfo };
            var output = new CappedBuffer(MaxOutputChars);
            var error = new CappedBuffer(MaxOutputChars);
        
            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data != null) output.Append(e.Data);
            };
            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data != null) error.Append(e.Data);
            };
        
            // 调用方令牌与超时令牌合并：任一触发都要把进程收掉，不能只抹异常留个孤儿
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(executedTimeout);
        
            try
            {
                process.Start();
                if (startInfo.RedirectStandardInput)
                {
                    // 参数已全部走 ArgumentList，脚本永不从 stdin 取数据；
                    // 不关就等于给 read/input 类脚本留一个永久阻塞点
                    process.StandardInput.Close();
                }
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                // .NET 的 WaitForExitAsync 会等待异步读流排空后才返回，
                // 无需再靶 Exited 事件 + TaskCompletionSource 手工同步（旧实现建了却没 await，是死代码）
                await process.WaitForExitAsync(timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                bool timedOut = !cancellationToken.IsCancellationRequested;
                KillProcessTree(process);
                return new ProcessExecution(output.ToString(), error.ToString(), ReadExitCode(process), timedOut, !timedOut);
            }
            catch
            {
                // 启动失败、Win32Exception 等：已派生的子进程不能留在服务器上
                KillProcessTree(process);
                throw;
            }
        
            return new ProcessExecution(output.ToString(), error.ToString(), process.ExitCode, false, false);
        }
        
        /// <summary>
        /// 启动脚本进程、按技能脚本约定解释结果。
        /// 失败一律抛异常，由调用方转成 “❌ …” 文本，保持既有对模型的返回契约不变。
        /// </summary>
        public static async Task<object?> RunAsync(
            ProcessStartInfo startInfo,
            string scriptName,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            var executedTimeout = timeout ?? DefaultTimeout;
            var execution = await ExecuteAsync(startInfo, executedTimeout, cancellationToken);
        
            if (execution.TimedOut || execution.Canceled)
            {
                var reason = execution.TimedOut
                    ? $"脚本 {scriptName} 执行超过 {(int)executedTimeout.TotalSeconds} 秒已被终止"
                    : $"脚本 {scriptName} 执行被取消";
                throw new InvalidOperationException(BuildFailureMessage(reason, null, execution));
            }
        
            if (execution.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    BuildFailureMessage($"脚本 {scriptName} 执行失败", execution.ExitCode, execution));
            }
        
            // 尝试解析 JSON 返回结果，不是 JSON 就按原样返回字符串
            string stdOut = execution.StdOut;
            if (string.IsNullOrWhiteSpace(stdOut))
            {
                return null;
            }
            try
            {
                using var doc = JsonDocument.Parse(stdOut);
                return JsonSerializer.Deserialize<object>(stdOut);
            }
            catch (JsonException)
            {
                return stdOut;
            }
        }
        
        /// <summary>
        /// 被强杀的进程可能已不可查，读不到退出码时返回 -1 而不是抛二次异常掩盖超时原因
        /// </summary>
        private static int ReadExitCode(Process process)
        {
            try
            {
                return process.ExitCode;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 连同子孙进程一起结束。
        /// 只 Kill 直接子进程时，python/bash 派生的孙进程会留在服务器上继续跑。
        /// </summary>
        private static void KillProcessTree(Process process)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch
            {
                // 进程已退出、或系统拒绝访问，都不该覆盖原本的超时/取消异常
            }
            try
            {
                process.WaitForExit(TimeSpan.FromSeconds(5));
            }
            catch
            {
            }
        }

        private static string BuildFailureMessage(
            string reason, int? exitCode, ProcessExecution execution)
        {
            var builder = new StringBuilder();
            builder.Append(exitCode.HasValue
                ? $"{reason}，退出码 {exitCode.Value}"
                : reason);
            if (!string.IsNullOrWhiteSpace(execution.StdErr))
            {
                builder.Append($"。错误输出: {execution.StdErr}");
            }
            if (!string.IsNullOrWhiteSpace(execution.StdOut))
            {
                builder.Append($"。标准输出: {execution.StdOut}");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 有上限的输出缓冲
        /// </summary>
        private sealed class CappedBuffer
        {
            private readonly StringBuilder _builder = new();
            private readonly int _maxChars;
            private int _droppedLines;

            public CappedBuffer(int maxChars)
            {
                _maxChars = maxChars;
            }

            public void Append(string line)
            {
                if (_builder.Length >= _maxChars)
                {
                    _droppedLines++;
                    return;
                }
                _builder.AppendLine(line);
            }

            public override string ToString()
            {
                var text = _builder.ToString().Trim();
                if (_droppedLines <= 0)
                {
                    return text;
                }
                return $"{text}{Environment.NewLine}…（输出超过 {_maxChars} 字符，后续 {_droppedLines} 行已丢弃）";
            }
        }
    }
}
