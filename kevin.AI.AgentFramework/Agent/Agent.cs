using Kevin.AI.Dto;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using OpenAI;
using System.ClientModel;

namespace kevin.AI.AgentFramework.Agent
{
    /// <summary>
    /// 智能体统一处理器
    /// </summary>
    public class Agent : IAgent
    {
        private readonly AISetting AIConfig;
        public Agent(IOptionsMonitor<AISetting> config)
        {
            AIConfig = config.CurrentValue;
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
        /// 创建智能体
        /// </summary>
        /// <param name="instructions"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="tools"></param>
        /// <param name="clientFactory"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        public AIAgent CreateAIAgent(
        string instructions = "你是第一个讲笑话的AI智能体",
        string name = "讲笑话",
        string description = "智能体",
        IList<AITool>? tools = null,
        Microsoft.Extensions.AI.ChatResponseFormat? chatResponseFormat = default,
        Func<IChatClient, IChatClient>? clientFactory = null,
        ILoggerFactory? loggerFactory = null,
        IServiceProvider? services = null)
        {
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions();
            openAIClientOptions.Endpoint = new Uri(AIConfig.AIUrl);
            var ai = new OpenAIClient(new ApiKeyCredential(AIConfig.AIKeySecret), openAIClientOptions);
            if (chatResponseFormat != default)
            {
                ChatOptions chatOptions = new()
                {
                    ResponseFormat = chatResponseFormat
                };
                return ai.GetChatClient(AIConfig.AIDefaultModel).CreateAIAgent(new ChatClientAgentOptions()
                {
                    Name = name,
                    Instructions = instructions,
                    ChatOptions = chatOptions,
                    Description = description
                });
            }
            return ai.GetChatClient(AIConfig.AIDefaultModel).CreateAIAgent(instructions: instructions, name: name, description, tools, clientFactory, loggerFactory, services);
        }

        /// <summary>
        /// 获取代理
        /// </summary>
        /// <returns></returns>
        public IChatClient GetChatClient()
        {
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions();
            openAIClientOptions.Endpoint = new Uri(AIConfig.AIUrl);
            var ai = new OpenAIClient(new ApiKeyCredential(AIConfig.AIKeySecret), openAIClientOptions);
            return ai.GetChatClient(AIConfig.AIDefaultModel).AsIChatClient();
        }
    }
}
