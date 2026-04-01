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
                                        ## 可用技能列表 
                                        {0} 

                                        每个技能包含：
                                        - **名称**：技能的唯一标识
                                        - **描述**：技能的功能说明
                                        - **参数**：调用时所需的输入字段（类型、含义）

                                       #自动化工作流程：
                                                1.每个技能提供专业指令、参考文档和可执行脚本 
                                                2.技能指令中会标明可用脚本及其执行命令 
                                                3.重要原则：先加载知识，再执行操作
                                                4.自动使用load_skill、read_skill_resource来获取技能指令和资源内容，理解后再决定是否调用工具执行脚本。
                                                5.**需求分析**：自动识别用户问题是否需要工具支持。
                                                6.**工具选择**：优先选择最匹配的工具
                                                7.**工具调用**：根据工具描述构造输入参数，调用工具
                                                8.**自主调用**：直接执行，无需用户干预。若失败，需根据错误信息调整或给出友好提示。
                                                9.根据技能返回结果生成最终答案：将技能返回的数据整合为自然语言回复用户。  
                                                 

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
