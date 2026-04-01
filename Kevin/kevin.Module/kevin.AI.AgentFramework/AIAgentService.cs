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
                    AIFunctionFactory.Create(AgentHttpClientTools.GetAsync,new AIFunctionFactoryOptions{ Name = "GetAsync",Description = "通用 HTTP 工具 发送 GET 请求" }),
                    AIFunctionFactory.Create(AgentHttpClientTools.PostAsync,new AIFunctionFactoryOptions{ Name = "PostAsync",Description = "通用 HTTP 工具 发送 POST 请求" }),
                    AIFunctionFactory.Create(AgentHttpClientTools.PutAsync,new AIFunctionFactoryOptions{ Name = "PutAsync",Description = "通用 HTTP 工具 发送 PUT 请求" }),
                    AIFunctionFactory.Create(AgentHttpClientTools.DeleteAsync,new AIFunctionFactoryOptions{ Name = "DeleteAsync",Description = "通用 HTTP 工具 发送 DELETE 请求" }),
                    AIFunctionFactory.Create(ShellTools.RunShell,new AIFunctionFactoryOptions{ Name = "RunShell",Description = "执行 Shell 命令。通过操作系统原生 Shell 执行命令(Windows 用 cmd也可以执行bash相关命令，Linux/Mac 用 bash）。包含安全护栏：危险命令阻止、输出截断（50KB）、超时控制（60秒）。" }),
                    AIFunctionFactory.Create(PythonTools.RunPythonPy,new AIFunctionFactoryOptions{ Name = "RunPythonPy",Description = "执行Python脚本。 可以帮助Skills工具执行scripts中含有后缀.py脚本的能力 通过PythonNet库来调用Python.py的脚本,并返回执行脚本结果 如果执行返回结果为空或者报错 可以使用RunShell来提取脚本代码然后自行调整定义main函数使用RunPythonCode来执行" }),
                    AIFunctionFactory.Create(PythonTools.RunPythonCode,new AIFunctionFactoryOptions{ Name = "RunPythonCode",Description = "执行Python代码。" }),
                    AIFunctionFactory.Create(CommonTools.GetRuntimePlatform,new AIFunctionFactoryOptions{ Name = "GetRuntimePlatform",Description = "获取系统。用于获取当前运行在什么系统平台上" }),
                    AIFunctionFactory.Create(CommonTools.GetDesktopPath,new AIFunctionFactoryOptions{ Name = "GetDesktopPath",Description = "获取当前系统桌面路径。 用于获取当前用户的桌面路径" }),
                    AIFunctionFactory.Create(CommonTools.WriteTextToDesktop,new AIFunctionFactoryOptions{ Name = "WriteTextToDesktop",Description = "输出文件到系统桌面。 用于把各种文件输出到桌面" }),
                    };
            }
            if (chatClientAgentOptions.AIContextProviders == default)
            {
#pragma warning disable MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
                var skillsProvider = new FileAgentSkillsProvider(
                     skillPaths: [  
                        Path.Combine(AppContext.BaseDirectory + "/Skills", "all-skills"), 
                        ],
                     options: new FileAgentSkillsProviderOptions
                     {
                         SkillsInstructionPrompt = """
                                        # Skills Instruction Prompt  
                                                你可以使用以下技能获取领域知识和操作指引。
                                                所有文件目录都在服务器的/Skills 文件夹下，技能文件夹命名为 技能名称-skills，技能文件夹内包含技能指令文件 instruction.txt、参考资料文件夹 resources 和可执行脚本文件夹 scripts。
                                                脚本文件夹 scripts 如何包含python脚本，则使用RunPythonPy来执行或者RunPythonCode来执行，否则使用RunShell来执行。
                                                    
                                        ## 可用技能列表 
                                        {0} 

                                        每个技能包含：
                                        - **名称**：技能的唯一标识
                                        - **描述**：技能的功能说明
                                        - **参数**：调用时所需的输入字段（类型、含义）

                                       #重要流程：
                                                1.每个技能提供专业指令、参考文档和可执行脚本 
                                                2.技能指令中会标明可用脚本及其执行命令 
                                                3.重要原则：先加载知识，再执行操作
                                                4.可以使用load_skill、read_skill_resource来获取技能指令和资源内容，理解后再决定是否调用工具执行脚本。

                                        ## 工作流程 
                                        1. **理解用户意图**：分析用户的问题，确定目标。
                                        2. **判断是否需要技能**：
                                           - 如果任务需要获取实时数据、执行计算、操作外部系统或访问你自身不具备的知识，则必须调用对应技能。
                                           - 如果可以直接用通用知识回答，则直接回答。
                                        3. **选择技能并构造调用**：从技能列表中选择最合适的技能，并按照技能描述构造输入参数。

                                        如果技能调用失败，需根据错误信息调整或给出友好提示。 
                                        根据技能返回结果生成最终答案：将技能返回的数据整合为自然语言回复用户。 

                                        #注意事项
                                        一次只能调用一个技能，如需多个步骤，分步进行。 
                                        若用户未明确所需参数，请主动询问澄清。 
                                        技能调用结果必须完整、准确地反映在最终回答中。
                                        调用工具不需要二次确认，直接调用即可，不需要询问用户是否需要调用工具。

                                        #Tools技能优先级
                                         1.当技能需要使用Tools工具时，通用 HTTP 工具优先级大于RunShell、RunPythonPy、RunPythonCode。
                                         2.当技能需要使用RunShell、RunPythonPy、RunPythonCode时，RunPythonCode优先级大于RunPythonPy。
                                         3.RunShell优先级最小，只有在技能指令中明确说明需要使用RunShell来执行且不适合使用RunPythonCode和RunPythonPy来执行时才使用RunShell。

                                        示例
                                        用户：北京今天天气怎么样？
                                        思考：用户需要实时天气，必须调用天气查询技能。
                                        输出：

                                        json
                                        " 
                                          "action": "get_weather",
                                          "action_input":  
                                            "city": "北京"
                                           
                                         "
                                        技能返回：晴，25℃，微风
                                        最终回答：北京今天晴天，气温25摄氏度，微风。
                        
                        """
                     });
#pragma warning restore MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
                chatClientAgentOptions.AIContextProviders = [skillsProvider]; 
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
