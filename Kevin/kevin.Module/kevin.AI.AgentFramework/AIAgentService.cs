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
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions();
            openAIClientOptions.Endpoint = new Uri(url);
            var ai = new OpenAIClient(new ApiKeyCredential(keySecret), openAIClientOptions);
            if (chatClientAgentOptions.ChatOptions != default && (chatClientAgentOptions.ChatOptions?.Tools == default || chatClientAgentOptions.ChatOptions?.Tools.Count == 0))
            {
                // 🔑 能力层：工具
                chatClientAgentOptions.ChatOptions.Tools = new List<AITool>() {
                    AIFunctionFactory.Create(KevinBasicAI.GetNetCoreKevinInfo,new AIFunctionFactoryOptions{ Name = "GetNetCoreKevinInfo",Description = "获取NetCoreKevin框架的介绍信息" }),
                    AIFunctionFactory.Create(ShellTools.RunShell,new AIFunctionFactoryOptions{ Name = "RunShell",Description = "执行 Shell 命令。通过操作系统原生 Shell 执行命令(Windows 用 cmd也可以执行bash相关命令，Linux/Mac 用 bash）。包含安全护栏：危险命令阻止、输出截断（50KB）、超时控制（60秒）。" }),
                    AIFunctionFactory.Create(PythonTools.RunPythonPy,new AIFunctionFactoryOptions{ Name = "RunPythonPy",Description = "执行Python脚本。 可以帮助Skills工具执行scripts中含有后缀.py脚本的能力 通过PythonNet库来调用Python.py的脚本,并返回执行脚本结果 如果执行返回结果为空或者报错 可以使用RunShell来提取脚本代码然后自行调整定义main函数使用RunPythonCode来执行" }),
                     AIFunctionFactory.Create(PythonTools.RunPythonCode,new AIFunctionFactoryOptions{ Name = "RunPythonCode",Description = "执行Python代码。使用IronPython库直接执行Python代码 必须定义为main函数" })
                    };
            }
            if (chatClientAgentOptions.AIContextProviders == default)
            {
#pragma warning disable MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
                var skillsProvider = new FileAgentSkillsProvider(
                     skillPaths: [Path.Combine(AppContext.BaseDirectory + "/Skills", "expense-report-skills"),
                        Path.Combine(AppContext.BaseDirectory + "/Skills", "system-ops-skills"),
                        Path.Combine(AppContext.BaseDirectory + "/Skills", "hello-python-skills"),
                          Path.Combine(AppContext.BaseDirectory + "/Skills", "agent-browser-skills")
                        ],
                     options: new FileAgentSkillsProviderOptions
                     {
                         SkillsInstructionPrompt = """
                                                你可以使用以下技能获取领域知识和操作指引。
                                                所有文件目录都在服务器的/Skills 文件夹下，技能文件夹命名为 技能名称-skills，技能文件夹内包含技能指令文件 instruction.txt、参考资料文件夹 resources 和可执行脚本文件夹 scripts。
                                                脚本文件夹 scripts 如何包含python脚本，则使用RunPythonPy来执行或者RunPythonCode来执行，否则使用RunShell来执行。
                                               
                                                重要流程：
                                                每个技能提供专业指令、参考文档和可执行脚本
                                                当用户任务匹配技能描述时，使用 `load_skill` 加载该技能的完整指令 
                                                技能指令中会标明可用脚本及其执行命令
                                                需要时使用 `read_skill_resource` 读取参考资料
                                                重要原则：先加载知识，再执行操作

                                                #相关技能文档如果需要执行命令可以通过RunShell来执行不需要用户二次确认

                                                它们如下:
                                                {0}  

                        
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
