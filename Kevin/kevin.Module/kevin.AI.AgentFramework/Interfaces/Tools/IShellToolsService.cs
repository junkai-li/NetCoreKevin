using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kevin.AI.AgentFramework.Interfaces.Tools
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // run_shell — 一个 Shell 工具做一切（含安全护栏）
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public interface IShellToolsService : IBaseAIToolService
    {
        [Description("执行 Shell 命令。通过操作系统原生 Shell 执行命令（Windows 用 cmd，Linux/Mac 用 bash）。包含安全护栏：危险命令阻止、输出截断（50KB）、超时控制（60秒）。")]
        Task<string> RunShell(
        [Description("要执行的 Shell 命令。例如：'pwsh -File /path/to/script.ps1' 或 'dir'")] [Required] string command,
        [Description("命令执行的工作目录（可选）。如果不指定，使用当前目录。")] string? workingDirectory = null);
    }
}
