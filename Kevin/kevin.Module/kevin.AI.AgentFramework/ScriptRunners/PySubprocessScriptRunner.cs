using Microsoft.Agents.AI;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace kevin.AI.AgentFramework.ScriptRunners
{
    public class PySubprocessScriptRunner
    {
        /// <summary>
        /// 创建进程启动信息
        /// </summary>
        private static ProcessStartInfo CreateStartInfo(string fileName, string arguments)
        {
            // 注册编码提供程序以支持 GBK 等旧编码（Windows 常见）
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Windows 下默认使用 Default (ANSI/OEM)，非 Windows 使用 UTF-8
            // 注意：对于 PowerShell，我们稍后会在参数中强制指定 UTF-8 输出，以覆盖此设置可能的不一致
            Encoding outputEncoding = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Encoding.Default
                : Encoding.UTF8;

            return new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false, // 必须为 false 才能重定向流
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardInputEncoding = Encoding.UTF8,
                StandardOutputEncoding = outputEncoding,
                StandardErrorEncoding = outputEncoding,
                CreateNoWindow = true
            };
        }

#pragma warning disable MAAI001
        public static async Task<object?> StaticRunAsync(
            AgentFileSkill skill,
            AgentFileSkillScript script,
            JsonElement? arguments,
            IServiceProvider? serviceProvider,
            CancellationToken cancellationToken)
        {
            // 1. 构造脚本文件的完整路径
            string scriptFullPath = Path.Combine(skill.Path, script.FullPath);
            if (!File.Exists(scriptFullPath))
            {
                throw new FileNotFoundException($"Script not found: {scriptFullPath}");
            } 
            // 2. 序列化参数为 JSON
            // 如果 arguments 为 null，发送一个空对象或空字符串，避免发送 "null" 字面量导致某些解析器报错
            string inputJson = arguments.HasValue
                ? JsonSerializer.Serialize(arguments.Value)
                : "{}";

            // 3. 根据后缀选择解释器
            string ext = Path.GetExtension(scriptFullPath).ToLowerInvariant();
            ProcessStartInfo startInfo;

            switch (ext)
            {
                case ".py":
                    // 建议：在实际项目中，最好能从配置中获取 python 路径，而不是硬编码 "python"
                    startInfo = CreateStartInfo("python3", $"\"{scriptFullPath}\"");
                    break;

                case ".sh":
                    string shell = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bash" : "bash";
                    // 注意：Windows 上运行 bash 通常需要 WSL 或 Git Bash 且在 PATH 中
                    startInfo = CreateStartInfo(shell, $"\"{scriptFullPath}\"");
                    break;

                case ".ps1":
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        // 关键优化：强制 PowerShell 使用 UTF-8 输出，防止中文乱码
                        // $OutputEncoding 和 [Console]::OutputEncoding 设置为 UTF-8
                        string psArgs = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"& {{ [Console]::OutputEncoding = [System.Text.Encoding]::UTF8; $OutputEncoding = [System.Text.Encoding]::UTF8; & '{scriptFullPath}' }}\"";
                        startInfo = CreateStartInfo("powershell", psArgs);
                    }
                    else
                    {
                        // Linux/macOS 使用 pwsh (PowerShell Core)
                        startInfo = CreateStartInfo("pwsh", $"-NoProfile -NonInteractive -File \"{scriptFullPath}\"");
                    }
                    break;

                default:
                    // 尝试直接执行（适用于 .exe, .bat, .cmd 等）
                    startInfo = CreateStartInfo(scriptFullPath, string.Empty);
                    break;
            }

            // 4. 启动进程并异步处理
            using var process = new Process { StartInfo = startInfo };

            // 使用 TaskCompletionSource 来确保在进程退出前，所有输出流都已读取完毕
            var tcs = new TaskCompletionSource<bool>();
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            // 注册事件处理
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null) outputBuilder.AppendLine(e.Data);
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null) errorBuilder.AppendLine(e.Data);
            };

            // 当进程退出时，标记任务完成
            process.Exited += (sender, e) =>
            {
                tcs.TrySetResult(true);
            };

            // 启用引发退出事件
            process.EnableRaisingEvents = true;

            try
            {
                process.Start();

                // 立即开始异步读取输出流
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // 异步写入标准输入
                // 注意：如果脚本不读取 stdin，写入操作通常会成功但被忽略，或者如果缓冲区满且无人读取可能会阻塞。
                // 为了安全，我们使用 WriteAsync 并关闭流。
                await process.StandardInput.WriteAsync(inputJson);
                await process.StandardInput.FlushAsync();
                process.StandardInput.Close(); // 关闭 stdin 以告知脚本输入结束

                // 等待进程退出或取消
                // 使用 WaitForExitAsync 结合 CancellationToken
                await process.WaitForExitAsync(cancellationToken);

                // 额外等待一小段时间以确保事件队列处理完毕（可选，但推荐）
                // 或者更严谨的做法是等待 tcs，但 WaitForExitAsync 通常足够保证流读取完成
                // 这里我们依赖 WaitForExitAsync 的行为：它保证在返回前，重定向的流已被读完。
            }
            catch (OperationCanceledException)
            {
                // 如果取消，尝试杀死进程
                if (!process.HasExited)
                {
                    try { process.Kill(); } catch { }
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
    }
}
