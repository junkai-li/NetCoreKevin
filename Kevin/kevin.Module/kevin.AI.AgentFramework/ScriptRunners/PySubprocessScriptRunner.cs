using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.ScriptRunners
{
    public  class PySubprocessScriptRunner 
    {
        private readonly ILogger<PySubprocessScriptRunner> _logger;
        private readonly string _pythonPath; // 例如 "python" 或 "/usr/bin/python3"
        public PySubprocessScriptRunner(string pythonPath = "python")
        {
            _pythonPath = pythonPath;
        }
        public async Task<object?> RunAsync(
#pragma warning disable MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
                AgentFileSkill skill, 
                AgentFileSkillScript script,
                AIFunctionArguments arguments,
                CancellationToken cancellationToken)
        {
            Console.WriteLine("执行script脚本文件: {0}", script.FullPath);
            // 1. 构造脚本文件的完整路径
            string scriptFullPath = Path.Combine(skill.Path, script.FullPath);
            if (!File.Exists(scriptFullPath))
            {
                throw new FileNotFoundException($"Script not found: {scriptFullPath}");
            }

            // 2. 将 AIFunctionArguments 转换为命令行参数
            //    这里简单地将参数序列化为 JSON 字符串，通过标准输入传递
            string inputJson = System.Text.Json.JsonSerializer.Serialize(arguments.ToDictionary());

            // 3. 配置进程启动信息
            var startInfo = new ProcessStartInfo
            {
                FileName = _pythonPath,
                Arguments = $"\"{scriptFullPath}\"",  // 脚本路径作为参数
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardInputEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                CreateNoWindow = true
            };

            // 4. 启动进程并异步处理
            using var process = new Process { StartInfo = startInfo };
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
#pragma warning restore MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
    }
}
