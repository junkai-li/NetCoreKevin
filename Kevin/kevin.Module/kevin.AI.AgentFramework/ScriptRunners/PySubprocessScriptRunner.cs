using kevin.AI.AgentFramework.Interfaces;
using Microsoft.Agents.AI;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace kevin.AI.AgentFramework.ScriptRunners
{
    public class PySubprocessScriptRunner : IPySubprocessScriptRunner
    {
        /// <summary>
        /// 脚本执行超时时间，防止子进程长时间挂起
        /// </summary>
        private static readonly TimeSpan ScriptExecutionTimeout = TimeSpan.FromSeconds(600);

        /// <summary>
        /// 创建进程启动信息
        /// </summary>
        private ProcessStartInfo CreateStartInfo(string fileName)
        {
            // 统一使用 UTF-8 读取子进程输出。
            // 注意：.NET Core/5+ 中 Encoding.Default 已经是 UTF-8（不再是 ANSI/OEM 代码页），
            // 因此这里显式使用 UTF-8，并通过环境变量强制 Python 子进程以 UTF-8 输出，避免中文乱码。
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                UseShellExecute = false, // 必须为 false 才能重定向流
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                CreateNoWindow = true
            };

            // 强制 Python 以 UTF-8 输出（对非 Python 进程无副作用）
            startInfo.EnvironmentVariables["PYTHONUTF8"] = "1";
            startInfo.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8";

            return startInfo;
        }

        /// <summary>
        /// 检测指定命令是否存在于系统 PATH 中
        /// </summary>
        private bool IsCommandAvailable(string command)
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
        private string? GetAvailablePythonCommand()
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

        /// <summary>
        /// 获取可用的 PowerShell 命令。
        /// 优先 pwsh (PowerShell 7+)：默认以 UTF-8 读取无 BOM 脚本与输出，可避免中文字面量乱码；
        /// Windows 上若未安装 pwsh 则回退到内置的 Windows PowerShell 5.1 (powershell)。
        /// 都不可用时返回 null。
        /// </summary>
        private string? GetAvailablePowerShellCommand()
        {
            // 优先检查 pwsh (PowerShell 7+)
            if (IsCommandAvailable("pwsh"))
            {
                return "pwsh";
            }

            // Windows 回退到内置的 Windows PowerShell 5.1
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && IsCommandAvailable("powershell"))
            {
                return "powershell";
            }

            // 都不存在返回 null
            return null;
        } 
        public async Task<object?> StaticRunAsync(
            AgentFileSkill skill,
            AgentFileSkillScript script,
            JsonElement? arguments,
            IServiceProvider? serviceProvider,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. 构造脚本文件的完整路径
                string scriptFullPath = Path.Combine(skill.Path, script.FullPath);
                if (!File.Exists(scriptFullPath))
                {
                    throw new FileNotFoundException($"Script not found: {scriptFullPath}");
                }

                // 2. 根据后缀选择解释器，并把脚本路径与各开关作为独立参数加入 ArgumentList。
                //    不能把 "-NoProfile -Command ..." 整串塞进一个参数，否则会被转义成单个 token 导致解释器无法解析。
                string ext = Path.GetExtension(scriptFullPath).ToLowerInvariant();
                ProcessStartInfo startInfo;
                switch (ext)
                {
                    case ".py":
                        string? pythonCmd = GetAvailablePythonCommand();
                        if (pythonCmd == null)
                        {
                            throw new InvalidOperationException(
                                "Python environment is not installed. Please install Python (python3 or python) and ensure it is available in the system PATH."
                            );
                        }
                        startInfo = CreateStartInfo(pythonCmd);
                        startInfo.ArgumentList.Add(scriptFullPath);
                        break;

                    case ".sh":
                        // Windows 上运行 bash 通常需要 WSL 或 Git Bash 且在 PATH 中
                        startInfo = CreateStartInfo("bash");
                        startInfo.ArgumentList.Add(scriptFullPath);
                        break;

                    case ".ps1":
                        string? psCmd = GetAvailablePowerShellCommand();
                        if (psCmd == null)
                        {
                            throw new InvalidOperationException(
                                "PowerShell is not available. Please install PowerShell (pwsh) or ensure powershell/pwsh is available in the system PATH."
                            );
                        }
                        startInfo = CreateStartInfo(psCmd);
                        startInfo.ArgumentList.Add("-NoProfile");
                        startInfo.ArgumentList.Add("-NonInteractive");
                        startInfo.ArgumentList.Add("-ExecutionPolicy");
                        startInfo.ArgumentList.Add("Bypass");
                        if (string.Equals(psCmd, "pwsh", StringComparison.OrdinalIgnoreCase))
                        {
                            // PowerShell 7+：默认 UTF-8 读取脚本与输出，直接 -File，后续参数自然透传给脚本
                            startInfo.ArgumentList.Add("-File");
                            startInfo.ArgumentList.Add(scriptFullPath);
                        }
                        else
                        {
                            // Windows PowerShell 5.1：会按 ANSI/GBK 读取无 BOM 脚本，这里强制 UTF-8 输出防止中文乱码；
                            // 末尾 @args 用于把后续追加的参数透传给脚本。
                            startInfo.ArgumentList.Add("-Command");
                            startInfo.ArgumentList.Add(
                                $"[Console]::OutputEncoding = [System.Text.Encoding]::UTF8; $OutputEncoding = [System.Text.Encoding]::UTF8; & '{scriptFullPath}' @args");
                        }
                        break;

                    default:
                        // 尝试直接执行（适用于 .exe, .bat, .cmd 等）
                        startInfo = CreateStartInfo(scriptFullPath);
                        break;
                }

                // 3. 追加脚本参数（通过命令行传递，形如 --name value）
                if (arguments != null)
                {
                    var kind = arguments.Value.ValueKind;
                    if (kind == JsonValueKind.Object)
                    {
                        foreach (var prop in arguments.Value.EnumerateObject())
                        {
                            startInfo.ArgumentList.Add($"--{prop.Name}");
                            startInfo.ArgumentList.Add(prop.Value.ToString());
                        }
                    }
                    else if (kind == JsonValueKind.Array)
                    {
                        // 数组格式：直接逐个添加每个元素（元素已包含完整参数）
                        foreach (var element in arguments.Value.EnumerateArray())
                        {
                            startInfo.ArgumentList.Add(element.ToString());
                        }
                    }
                    else
                    {
                        startInfo.ArgumentList.Add(arguments.Value.ToString());
                    }
                }

                // 4. 启动进程并异步处理
                using var process = new Process { StartInfo = startInfo };

                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();

                // 异步读取输出/错误流，避免子进程缓冲区写满而阻塞
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null) outputBuilder.AppendLine(e.Data);
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null) errorBuilder.AppendLine(e.Data);
                };

                // 组合外部取消令牌与执行超时，防止脚本长时间挂起
                // （WaitForExitAsync 会同时等待输出流读取完毕，无需再用 Exited 事件）
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(ScriptExecutionTimeout);

                try
                {
                    process.Start();

                    // 立即开始异步读取输出流
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    await process.WaitForExitAsync(timeoutCts.Token);
                }
                catch (OperationCanceledException)
                {
                    // 超时或外部取消：先杀死进程树，再区分失败原因
                    if (!process.HasExited)
                    {
                        try { process.Kill(entireProcessTree: true); } catch { }
                    }

                    // 外部未请求取消，说明是本次执行超时
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        throw new TimeoutException(
                            $"Script execution timed out after {ScriptExecutionTimeout.TotalSeconds} seconds.");
                    }

                    throw;
                }

                // 5. 处理执行结果
                string stdOut = outputBuilder.ToString().Trim();
                string stdErr = errorBuilder.ToString().Trim();

                if (process.ExitCode != 0)
                {
                    // 将标准错误和标准输出都包含在异常信息中，以便调试
                    throw new InvalidOperationException(
                        $"Script execution failed with exit code {process.ExitCode}. " +
                        $"Error: {stdErr}. " +
                        $"Output: {stdOut}"
                    );
                }
                Console.WriteLine($"Script {script.Name} executed successfully.");
                // 6. 尝试解析 JSON 返回结果
                if (string.IsNullOrWhiteSpace(stdOut))
                {
                    return null;
                }

                try
                {
                    // 尝试反序列化为 JsonElement 或 object
                    // 使用 JsonDocument 可以先验证是否是有效 JSON，避免异常开销过大
                    using var doc = JsonDocument.Parse(stdOut);
                    return JsonSerializer.Deserialize<object>(stdOut);
                }
                catch (JsonException)
                {
                    // 如果不是 JSON，直接返回字符串
                    return stdOut;
                }
            }
            catch (Exception ex)
            {
                // 捕获所有异常并返回错误信息字符串
                return $"❌ 执行脚本时发生错误: {ex.Message}";
            }
        }
    }
}