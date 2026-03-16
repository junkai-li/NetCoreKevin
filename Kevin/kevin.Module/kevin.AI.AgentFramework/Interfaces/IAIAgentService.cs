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
