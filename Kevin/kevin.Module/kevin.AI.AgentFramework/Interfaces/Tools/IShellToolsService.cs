using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kevin.AI.AgentFramework.Interfaces.Tools
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // run_shell — 一个 Shell 工具做一切（含安全护栏）
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public interface IShellToolsService  
    {
        [Description("执行 Shell 命令。通过操作系统原生 Shell 执行命令（Windows 用 cmd，Linux/Mac 用 bash），每次调用为独立 Shell 进程。包含安全护栏：危险命令阻止、配置文件访问拦截（禁止访问 appsettings.json 等）、HTTP请求域名白名单、输出截断、超时控制。")]
        Task<string> RunShell(
        [Description("要执行的 Shell 命令。例如：'pwsh -File /path/to/script.ps1' 或 'dir'")][Required] string command,
        [Description("命令执行的工作目录（可选）。如果不指定，使用当前目录。")] string? workingDirectory = null,
       [Description("超时时间（单位秒）：默认600秒")] int seconds = 600);
    }
}
