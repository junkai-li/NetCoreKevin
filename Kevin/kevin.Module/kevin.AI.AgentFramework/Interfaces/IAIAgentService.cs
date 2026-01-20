using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using OpenAI;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Interfaces
{
    public interface IAIAgentService
    {
        /// <summary>
        /// OpenAI智能体
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="name">智能体名称</param>
        /// <param name="prompt">提示词</param>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <param name="keySecret"></param>
        /// <returns></returns>
        Task<AIAgent> CreateOpenAIAgent(string name, string prompt, string description, string url, string model, string keySecret,
               IList<AITool>? tools = null,
                ChatOptions? chatOptions = default,
                Func<IChatClient, IChatClient>? clientFactory = null,
               ILoggerFactory? loggerFactory = null,
                    IServiceProvider? services = null
            );
        Task<AIAgent> CreateOpenAIAgent(string msg, string url, string model, string keySecret, ChatClientAgentOptions chatClientAgentOptions);
        /// <summary>
        /// 创建智能体并发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="name"></param>
        /// <param name="prompt"></param>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <param name="keySecret"></param>
        /// <param name="tools"></param>
        /// <param name="chatResponseFormat"></param>
        /// <param name="clientFactory"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        Task<(AIAgent, AgentRunResponse)> CreateOpenAIAgentAndSendMSG(string msg, string name, string prompt, string description, string url, string model, string keySecret, 
            IList<AITool>? tools = null, ChatResponseFormat? chatResponseFormat = null, Func<IChatClient, IChatClient>? clientFactory = null, ILoggerFactory? loggerFactory = null, IServiceProvider? services = null);
        Task<(AIAgent, string)> CreateOpenAIAgentAndSendMSG(string msg, string url, string model, string keySecret, ChatClientAgentOptions chatClientAgentOptions, bool isStreame = false, Action<string> streameCallback = default);
        /// <summary>
        /// 智能体转换为McpServerTool
        /// </summary>
        /// <param name="aIAgent">智能体</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
         McpServerTool AIAgentAsMcpServerTool(AIAgent aIAgent);

        /// <summary>
        /// 获取代理
        /// </summary>
        /// <returns></returns>
        public IChatClient GetChatClient(string url, string model, string keySecret);
    }
}
