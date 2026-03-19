using System.ComponentModel;
using System.Diagnostics;
namespace kevin.AI.AgentFramework.Tools
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // RunPython — 一个 执行Python脚本d的工具
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public class PythonTools
    {
        [Description("执行Python脚本。通过System.Diagnostics.Process类来启动一个新的进程，并运行Python.py的脚本。这种方法适用于Windows和Linux系统。")]
        public static string RunPythonPy([Description("需要执行的python脚本路径。例如：'C:/path/to/script.py'")]
                                        string scriptPath)
        {
            try
            {
                string output = "";
                Console.WriteLine($"🔧 正在执行Py脚本 {scriptPath}");
                // 创建新的进程启动信息
                ProcessStartInfo start = new ProcessStartInfo
                {
                    FileName = @"C:\Users\XRH-183\PyCharmMiscProject\.venv\Scripts\python.exe", // Python解释器的路径，例如 "python" 或 "python3"
                    Arguments = scriptPath, // Python脚本的路径
                    UseShellExecute = false, // 不使用Shell启动进程
                    RedirectStandardOutput = true, // 重定向输出流
                    CreateNoWindow = true // 不创建新窗口
                };

                // 启动进程
                if (start != default)
                {
                    using (Process process = Process.Start(start))
                    {
                        // 读取输出
                        output = process?.StandardOutput.ReadToEnd() ?? ""; 
                        process?.WaitForExit(); // 等待进程结束 
                    }
                }
                if (string.IsNullOrWhiteSpace(output))
                {
                    output = "Python脚本执行完成，但没有输出结果。";
                }
                return output;
            }
            catch (Exception ex)
            {
                return $"❌ 执行失败: {ex.Message}";
            }
        }

        [Description("执行Python代码。使用IronPython库直接执行Python代码 必须定义为main函数")]
        public static string RunPythonCode([Description("需要执行的python代码。例如：'def main(name): return 'Hello ' + name.title() + '!'")]
                                        string code)
        {
            try
            {
                Console.WriteLine($"🔧 正在执行Py代码 {code}");
                // 创建Python引擎和作用域
                var eng = IronPython.Hosting.Python.CreateEngine();
                var scope = eng.CreateScope();
                eng.Execute(code, scope);
                dynamic main = scope.GetVariable("main");
                return main(); 
            }
            catch (Exception ex)
            {
                return $"❌ 执行失败: {ex.Message}";
            }
        }
    }
}
