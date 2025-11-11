using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Agent
{
    /// <summary>
    /// 智能体接口
    /// </summary>
    public interface IAgent
    { 

        /// <summary>
        /// 把智能体变成mcp服务
        /// </summary>
        /// <param name="aIAgent"></param>
        /// <returns></returns>
        McpServerTool AIAgentAsMcpServerTool(AIAgent aIAgent);
        /// <summary>
        /// 获取代理
        /// </summary>
        /// <returns></returns>
        IChatClient GetChatClient();

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
        AIAgent CreateAIAgent(
        string instructions = "你是第一个讲笑话的AI智能体",
        string name = "讲笑话",
        string description = "智能体",
        IList<AITool>? tools = null,
        Microsoft.Extensions.AI.ChatResponseFormat chatResponseFormat = default,
        Func<IChatClient, IChatClient>? clientFactory = null,
        ILoggerFactory? loggerFactory = null,
        IServiceProvider? services = null);
    }
}
