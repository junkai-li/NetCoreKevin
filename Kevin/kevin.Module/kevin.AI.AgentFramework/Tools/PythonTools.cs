using System.ComponentModel;
using System.Diagnostics;
using System.Text;
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
                Console.WriteLine();
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

        [Description("用于执行Python代码")]
        public static string RunPythonCode([Description("需要执行的python代码。例如：'def main(name): return 'Hello ' + name.title() + '!'")]
                                        string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return "执行Py代码为空。";
                }
                Console.WriteLine($"🔧 正在执行Py代码 {code}");
                var saveResult = SavePythonToFile(code, "Pys", "");
                Console.WriteLine($"🔧 正在执行Py脚本: {saveResult}");
                string output = "";
                // 设置进程信息
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = @"python"; // Python解释器的路径，例如 "python" 或 "python3"
                start.Arguments = $"{saveResult}"; // 传递参数 
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

        [Description("把传入的python代码保存为 .py 文件，返回保存的完整路径，失败返回以 ❌ 开头的错误信息")]
        public static string SavePythonToFile(string code, string relativeDir = "Skills/python-skills/tmp", string fileName = null)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(code))
                {
                    return "❌ 保存失败: 代码内容为空。";
                }

                // 规范化相对目录分隔符
                relativeDir = relativeDir.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
                string baseDir = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
                string fullDir = Path.Combine(baseDir, relativeDir);

                // 确保目录存在
                Directory.CreateDirectory(fullDir);

                // 自动生成文件名如果未提供
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = $"py_{System.DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}.py";
                }
                else
                {
                    if (!fileName.EndsWith(".py", StringComparison.OrdinalIgnoreCase))
                    {
                        fileName = fileName + ".py";
                    }
                }

                string fullPath = Path.Combine(fullDir, fileName);

                // 以 UTF-8 无 BOM 保存，保证跨平台兼容且 Python 能正确识别
                File.WriteAllText(fullPath, code, new UTF8Encoding(false));
                Console.WriteLine($"🔧 Python脚本已保存到: {fullPath}");
                return fullPath;
            }
            catch (Exception ex)
            {
                return $"❌ 保存失败: {ex.Message}";
            }
        }
    }
}
