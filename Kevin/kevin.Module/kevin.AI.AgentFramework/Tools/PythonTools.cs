using System.ComponentModel;
using System.Diagnostics;
namespace kevin.AI.AgentFramework.Tools
{
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // RunPython — 一个 执行Python脚本的工具
    // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    public class PythonTools
    {
        [Description("执行Python脚本。通过System.Diagnostics.Process类来启动一个新的进程，并运行Python.py的脚本。这种方法适用于Windows和Linux系统。")]
        public static string RunPythonPy([Description("需要执行的python脚本路径。例如：'Skills\\python-skills\\hello-python\\scripts\\hello-python.py'")]
                                        string scriptPath,
            [Description("需要传入python脚本的参数。例如：['你好','word']")]
            List<string> args = default
            )
        {
            try
            {
                string output = "";
                scriptPath = AppContext.BaseDirectory + scriptPath.Replace(@"/", @"\");
                Console.WriteLine($"🔧 正在执行Py脚本 {scriptPath}");
                // 设置进程信息
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = @"python"; // Python解释器的路径，例如 "python" 或 "python3"
                start.Arguments = $"{scriptPath}"; // 传递参数
                if (args != default)
                {
                    foreach (var item in args)
                    {
                        start.Arguments += $" {item} \" ";
                    }

                }

                start.UseShellExecute = false; // 不使用操作系统外壳启动
                start.RedirectStandardOutput = true; // 重定向标准输出
                start.RedirectStandardError = true; // 重定向标准错误
                using (Process process = Process.Start(start))
                {
                    // 获取输出
                    output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit(); // 等待进程结束 
                    if (!string.IsNullOrEmpty(error))
                    {
                        return $"❌ 执行失败: {error}";
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
