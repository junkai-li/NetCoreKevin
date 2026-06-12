using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kevin.AI.AgentFramework.Interfaces.Tools
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // RunPython — 一个 执行Python脚本的工具
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public interface IPythonToolsService : IBaseAIToolService
    {
        [Description("执行Python脚本。通过System.Diagnostics.Process类来启动一个新的进程，并运行Python.py的脚本。这种方法适用于Windows和Linux系统。")]
        Task<string> RunPythonPy([Description("需要执行的python脚本路径。例如：'Skills\\python-skills\\hello-python\\scripts\\hello-python.py'")]
                                        [Required]string scriptPath,
           [Description("需要传入python脚本的参数。例如：['你好','word']")]
            List<string> args = default
           );
        [Description("用于执行Python代码")]
        Task<string> RunPythonCode([Description("需要执行的python代码。例如：'def main(name): return 'Hello ' + name.title() + '!'")][Required]
                                        string code);
        [Description("把传入的python代码保存为 .py 文件，返回保存的完整路径，失败返回以 ❌ 开头的错误信息")]
        Task<string> SavePythonToFile([Required] string code, [Required] string relativeDir = "Skills/python-skills/tmp", string fileName = null);
    }
}
