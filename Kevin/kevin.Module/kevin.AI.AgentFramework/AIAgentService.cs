using Azure;
using HttpMataki.NET.Auto;
using kevin.AI.AgentFramework.Agent;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Tools;
using Kevin.AI.Dto;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using OpenAI;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace kevin.AI.AgentFramework
{
    /// <summary>
    /// AI服务
    /// </summary>
    public class AIAgentService : IAIAgentService
    {
        public static string AIRulePrompt = "";
        public static bool IsHttpLog = false;
        public AIAgentService()
        {
        }
        public AIAgentService(IOptionsMonitor<AISetting> config)
        {
            IsHttpLog = config.CurrentValue.IsHttpLog;
        }
        /// <summary>
        /// 智能体转换为McpServerTool
        /// </summary>
        /// <param name="aIAgent">智能体</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public McpServerTool AIAgentAsMcpServerTool(AIAgent aIAgent)
        {
            return McpServerTool.Create(aIAgent.AsAIFunction());
        }

        /// <summary>
        /// 获取代理
        /// </summary>
        /// <returns></returns>
        public IChatClient GetChatClient(string url, string model, string keySecret)
        {
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions();
            openAIClientOptions.Endpoint = new Uri(url);
            var ai = new OpenAIClient(new ApiKeyCredential(model), openAIClientOptions);
            return ai.GetChatClient(keySecret).AsIChatClient();
        }
        public async Task<(AIAgent, string)> CreateOpenAIAgentAndSendMSG(string msg, string url, string model, string keySecret, ChatClientAgentOptions chatClientAgentOptions, bool isStreame = false, Action<string> streameCallback = default)
        {
            if (IsHttpLog)
            {
                HttpClientAutoInterceptor.StartInterception();
            }
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions();
            openAIClientOptions.Endpoint = new Uri(url);
            var ai = new OpenAIClient(new ApiKeyCredential(keySecret), openAIClientOptions);
            if (chatClientAgentOptions.ChatOptions != default && (chatClientAgentOptions.ChatOptions?.Tools == default || chatClientAgentOptions.ChatOptions?.Tools.Count == 0))
            {
                // 🔑 能力层：工具
                chatClientAgentOptions.ChatOptions.Tools = new List<AITool>() {
                    AIFunctionFactory.Create(KevinBasicAI.GetNetCoreKevinInfo,new AIFunctionFactoryOptions{ Name = "GetNetCoreKevinInfo",Description = "获取NetCoreKevin框架的介绍信息" }),
                    AIFunctionFactory.Create(ShellTools.RunShell,new AIFunctionFactoryOptions{ Name = "RunShell",Description = "执行 Shell 命令。通过操作系统原生 Shell 执行命令（Windows 用 cmd，Linux/Mac 用 bash）。包含安全护栏：危险命令阻止、输出截断（50KB）、超时控制（60秒）。" })
                    };
            }
            if (chatClientAgentOptions.AIContextProviders == default)
            {
#pragma warning disable MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
                var skillsProvider = new FileAgentSkillsProvider(
                    skillPaths: [Path.Combine(AppContext.BaseDirectory + "/Skills", "expense-report-skills"), 
                        Path.Combine(AppContext.BaseDirectory + "/Skills", "system-ops-skills")],
                    options: new FileAgentSkillsProviderOptions
                    {
                        SkillsInstructionPrompt = """
                                                你可以使用以下技能获取领域知识和操作指引。
                                                每个技能提供专业指令、参考文档和可执行脚本
                                                它们如下:
                                                {0}

                                                使用 `expense-report` 这个技能用于 按照NetCoreKevin科技公司政策填写和审核员工费用报销。适用于费用报销、报销规则、收据要求、支出限额或费用类别等相关问题。 

                                                使用 `system-ops` 这个技能 工作流程：
                                                1. 当用户任务匹配技能描述时，使用 `load_skill` 加载该技能的完整指令
                                                2. 技能指令中会标明可用脚本及其执行命令
                                                3. 使用 `run_shell` 工具执行技能中标注的命令
                                                4. 需要时使用 `read_skill_resource` 读取参考资料。
                                                重要原则：先加载知识，再执行操作。

                                                """
                    });
#pragma warning restore MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
                chatClientAgentOptions.AIContextProviders = [skillsProvider];
                Console.WriteLine("📂 Skills 已从文件系统加载");
                Console.WriteLine("✅ FileAgentSkillsProvider 创建成功（知识层）");
                Console.WriteLine("📂 自动注册工具: load_skill, read_skill_resource");
                Console.WriteLine();
            }

            var aiAgent = ai.GetChatClient(model).AsIChatClient().AsAIAgent(chatClientAgentOptions);
            var reslut = new AgentResponse();
            var resultText = string.Empty;
            if (isStreame)
            {
                if (streameCallback != default)
                {
                    await foreach (var update in aiAgent.RunStreamingAsync(msg))
                    {
                        if (!string.IsNullOrEmpty(update.Text))
                        {
                            streameCallback.Invoke(update.Text);
                            resultText += update.Text;
                        }
                    }
                }
            }
            else
            {
                reslut = await aiAgent.RunAsync(msg);
                resultText = reslut.Text;
            }
            if (IsHttpLog)
            {
                HttpClientAutoInterceptor.StopInterception();
            }
            return (aiAgent, resultText);
        }
    }
}
