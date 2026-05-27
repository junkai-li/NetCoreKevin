using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Runtime.InteropServices;

namespace kevin.AI.AgentFramework.ScriptRunners
{
    public class PySubprocessScriptRunner
    {  
        // 公共的 ProcessStartInfo 构建器，避免重复配置
        private static ProcessStartInfo CreateStartInfo(string fileName, string arguments)
        {
            // 在 Windows 上使用系统默认编码（通常是 ANSI/OEM），以避免从外部进程收到的输出出现乱码。
            // 在非 Windows 平台优先使用 UTF-8。
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Encoding outputEncoding = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Encoding.Default : Encoding.UTF8;

            return new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardInputEncoding = Encoding.UTF8,
                StandardOutputEncoding = outputEncoding,
                StandardErrorEncoding = outputEncoding,
                CreateNoWindow = true
            };
        }

#pragma warning disable MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
        public static async Task<object?> StaticRunAsync(AgentFileSkill skill,  AgentFileSkillScript script,    JsonElement? arguments, IServiceProvider? serviceProvider, CancellationToken cancellationToken)
        {
            Console.WriteLine("执行脚本文件: {0}", script.FullPath);
            // 1. 构造脚本文件的完整路径
            string scriptFullPath = Path.Combine(skill.Path, script.FullPath);
            if (!File.Exists(scriptFullPath))
            {
                throw new FileNotFoundException($"Script not found: {scriptFullPath}");
            }
            Console.WriteLine("脚本文件存在: {0}", scriptFullPath);
            // 2. 将 AIFunctionArguments 转换为命令行参数
            //    这里简单地将参数序列化为 JSON 字符串，通过标准输入传递
            string inputJson = System.Text.Json.JsonSerializer.Serialize(arguments);

            // 3. 根据脚本后缀选择合适的解释器/命令并配置进程启动信息（静态方法中无法访问实例的 pythonPath）
            string ext2 = Path.GetExtension(scriptFullPath).ToLowerInvariant();
            ProcessStartInfo startInfo2;

            switch (ext2)
            {
                case ".py":
                    startInfo2 = CreateStartInfo("python", $"\"{scriptFullPath}\"");
                    break;

                case ".sh":
                    startInfo2 = CreateStartInfo(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bash" : "bash", $"\"{scriptFullPath}\"");
                    break;

                case ".ps1":
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        startInfo2 = CreateStartInfo("powershell", $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -File \"{scriptFullPath}\"");
                    }
                    else
                    {
                        startInfo2 = CreateStartInfo("pwsh", $"-NoProfile -NonInteractive -File \"{scriptFullPath}\"");
                    }
                    break;

                default:
                    startInfo2 = CreateStartInfo(scriptFullPath, string.Empty);
                    break;
            }

            // 4. 启动进程并异步处理
            using var process = new Process { StartInfo = startInfo2 };
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null) outputBuilder.AppendLine(e.Data);
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null) errorBuilder.AppendLine(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // 将参数写入标准输入（如果脚本需要从 stdin 读取）
            await process.StandardInput.WriteAsync(inputJson);
            process.StandardInput.Close();

            // 等待进程结束或取消
            await process.WaitForExitAsync(cancellationToken);

            // 5. 处理执行结果
            if (process.ExitCode != 0)
            {
                string errorMsg = errorBuilder.ToString();
                throw new InvalidOperationException($"Script execution failed: {errorMsg}");
            }

            string result = outputBuilder.ToString().Trim();
            Console.WriteLine("Script {ScriptName} returned: {Result}", script.Name, result);

            // 6. 返回 object?，可尝试反序列化为 JSON 对象，或直接返回字符串
            //    若脚本输出是 JSON，可以解析后返回，否则直接返回字符串
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<object>(result);
            }
            catch
            {
                return result;
            }
        }
    }
}
