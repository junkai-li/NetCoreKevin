using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Tools
{
    public class ShellTools
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

        [Description("执行 Shell 命令。通过操作系统原生 Shell 执行命令（Windows 用 cmd，Linux/Mac 用 bash）。包含安全护栏：危险命令阻止、输出截断（50KB）、超时控制（60秒）。")]
        public static string RunShell(
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

                // 🛡️ 安全护栏 2：输出截断（50KB）
                const int maxOutputLength = 50_000;
                if (output.Length > maxOutputLength)
                {
                    output = output[..maxOutputLength] + "\n... (输出已截断，超过 50KB 上限)";
                }

                return output;
            }
            catch (Exception ex)
            {
                return $"❌ 执行失败: {ex.Message}";
            }
        }
    }
}
