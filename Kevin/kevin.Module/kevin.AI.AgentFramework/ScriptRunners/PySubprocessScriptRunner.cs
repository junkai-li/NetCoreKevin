using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Tools;
using Microsoft.Agents.AI;
using System.Text.Json;

namespace kevin.AI.AgentFramework.ScriptRunners
{
    public class PySubprocessScriptRunner : IPySubprocessScriptRunner
    {
#pragma warning disable MAAI001
        public async Task<object?> StaticRunAsync(
            AgentFileSkill skill,
            AgentFileSkillScript script,
            JsonElement? arguments,
            IServiceProvider? serviceProvider,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. 解析并校验脚本文件路径（禁止越出技能目录）
                string scriptFullPath = ScriptProcessRunner.ResolveScriptFullPath(skill.Path, script.FullPath);

                // 🛡️ 安全护栏：脚本内容是仓库固定文件，参数是模型唯一可控的输入面，
                //    拿它去读受限配置文件（如 --file appsettings.json）同样拦住
                if (arguments.HasValue && CommandGuardrails.ContainsRestrictedFile(arguments.Value.ToString()))
                {
                    return CommandGuardrails.RestrictedFileBlockedMessage;
                }

                // 2. 先把模型给的参数收齐（.ps1 要把参数编进命令文本，不能事后追加）
                var scriptArguments = new List<string>();
                if (arguments != null)
                {
                    var kind = arguments.Value.ValueKind;
                    if (kind == JsonValueKind.Object)
                    {
                        foreach (var prop in arguments.Value.EnumerateObject())
                        {
                            scriptArguments.Add($"--{prop.Name}");
                            scriptArguments.Add(prop.Value.ToString());
                        }
                    }
                    else if (kind == JsonValueKind.Array)
                    {
                        // 数组格式：直接逐个添加每个元素（元素已包含完整参数）
                        foreach (var element in arguments.Value.EnumerateArray())
                        {
                            scriptArguments.Add(element.ToString());
                        }
                    }
                    else
                    {
                        scriptArguments.Add(arguments.Value.ToString());
                    }
                }

                // 3. 根据后缀选择解释器
                var (startInfo, payloadArguments) = ScriptProcessRunner.CreateLaunch(scriptFullPath, scriptArguments);
                foreach (var payload in payloadArguments)
                {
                    startInfo.ArgumentList.Add(payload);
                }

                // 4. 执行并取结果（超时、进程树回收、输出上限由 ScriptProcessRunner 统一处理）
                return await ScriptProcessRunner.RunAsync(startInfo, script.Name, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                // 捕获所有异常并返回错误信息字符串
                return $"❌ 执行脚本时发生错误: {ex.Message}";
            }
        }
    }
}
