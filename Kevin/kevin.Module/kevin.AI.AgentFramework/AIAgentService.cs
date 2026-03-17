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
                chatClientAgentOptions.ChatOptions.Tools = new List<AITool>() {
                    AIFunctionFactory.Create(KevinBasicAI.GetNetCoreKevinInfo,new AIFunctionFactoryOptions{ Name = "GetNetCoreKevinInfo",Description = "获取NetCoreKevin框架的介绍信息" })
                    };
            }
            if (chatClientAgentOptions.AIContextProviders == default)
            {
#pragma warning disable MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
                var skillsProvider = new FileAgentSkillsProvider(
                    skillPaths: [Path.Combine(AppContext.BaseDirectory, "multi-search-engine-2.0.1"), Path.Combine(AppContext.BaseDirectory, "weather-1.0.0"),],
                    options: new FileAgentSkillsProviderOptions
                    {
                        SkillsInstructionPrompt = """
            You have skills available. Here they are:
            {0}
            Use the `multi-search-engine-2.0.1` function Multi search engine integration with 17 engines (8 CN + 9 Global). Supports advanced search operators, time filters, site search, privacy engines, and WolframAlpha knowledge queries. No API keys required.
            Use the `weather-1.0.0` function Get current weather and forecasts (no API key required).
            """
                    });
#pragma warning restore MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
                chatClientAgentOptions.AIContextProviders = [skillsProvider];
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
