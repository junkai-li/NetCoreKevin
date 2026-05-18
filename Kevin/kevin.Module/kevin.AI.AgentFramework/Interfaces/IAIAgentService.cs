using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace kevin.AI.AgentFramework.Interfaces
{
    public interface IAIAgentService
    {
        /// <summary>
        /// 创建代理并发送消息
        /// </summary>
        /// <param name="_serviceProvider"></param>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <param name="keySecret"></param>
        /// <param name="chatClientAgentOptions"></param>
        /// <param name="data">传递自定义数据</param>
        /// <param name="isStreame"></param>
        /// <param name="streameCallback"></param>
        /// <returns></returns>
        Task<(AIAgent, string)> CreateOpenAIAgentAndSendMSG(IServiceProvider? _serviceProvider, string msg, string url, string model, string keySecret, ChatClientAgentOptions chatClientAgentOptions, object data = default, bool isStreame = false, Action<string> streameCallback = default);
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
