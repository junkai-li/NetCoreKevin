using kevin.AI.AgentFramework.Interfaces;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        public AIAgentService()
        {
        }
        public async Task<AIAgent> CreateOpenAIAgent(string name, string prompt, string description, string url, string model, string keySecret,
            IList<AITool>? tools = null, ChatOptions? chatOptions = null, Func<IChatClient, IChatClient>? clientFactory = null, ILoggerFactory? loggerFactory = null, IServiceProvider? services = null)
        {
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions();
            openAIClientOptions.Endpoint = new Uri(url);
            var ai = new OpenAIClient(new ApiKeyCredential(keySecret), openAIClientOptions);
            if (chatOptions != default)
            { 
                return ai.GetChatClient(model).CreateAIAgent(new ChatClientAgentOptions()
                {
                    Name = name,
                    Instructions = prompt,
                    ChatOptions = chatOptions,
                    Description = description
                });
            }
            return ai.GetChatClient(model).CreateAIAgent(instructions: prompt, name: name, prompt, tools, clientFactory, loggerFactory, services);
        }

        public async Task<AIAgent> CreateOpenAIAgent(string msg, string url, string model, string keySecret, ChatClientAgentOptions chatClientAgentOptions)
        {
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions();
            openAIClientOptions.Endpoint = new Uri(url);
            var ai = new OpenAIClient(new ApiKeyCredential(keySecret), openAIClientOptions); 
            return ai.GetChatClient(model).CreateAIAgent(chatClientAgentOptions);
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

        public async Task<(AIAgent, AgentRunResponse)> CreateOpenAIAgentAndSendMSG(string msg, string name, string prompt, string description, string url, string model, string keySecret,
            IList<AITool>? tools = null, ChatResponseFormat? chatResponseFormat = null, Func<IChatClient, IChatClient>? clientFactory = null, ILoggerFactory? loggerFactory = null, IServiceProvider? services = null)
        {
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions();
            openAIClientOptions.Endpoint = new Uri(url);
            var ai = new OpenAIClient(new ApiKeyCredential(keySecret), openAIClientOptions);
            var aiAgent = ai.GetChatClient(model).CreateAIAgent(instructions: prompt, name: name, prompt, tools, clientFactory, loggerFactory, services);
            if (chatResponseFormat != default)
            {
                ChatOptions chatOptions = new()
                {
                    ResponseFormat = chatResponseFormat
                };
                aiAgent = ai.GetChatClient(model).CreateAIAgent(new ChatClientAgentOptions()
                {
                    Name = name,
                    Instructions = prompt,
                    ChatOptions = chatOptions,
                    Description = description
                });
            }
            var reslut = await aiAgent.RunAsync(msg);
            return (aiAgent, reslut);
        }

        public async Task<(AIAgent, AgentRunResponse)> CreateOpenAIAgentAndSendMSG(string msg, string url, string model, string keySecret, ChatClientAgentOptions chatClientAgentOptions)
        {
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions();
            openAIClientOptions.Endpoint = new Uri(url);
            var ai = new OpenAIClient(new ApiKeyCredential(keySecret), openAIClientOptions); 
            var aiAgent = ai.GetChatClient(model).CreateAIAgent(chatClientAgentOptions); 
            var reslut = await aiAgent.RunAsync(msg);
            return (aiAgent, reslut);
        }
    }
}
