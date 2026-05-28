using Kevin.AI.Dto;
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
        /// <param name="chatClientAgentOptions"></param> 
        /// <returns></returns>
        Task<(AIAgent, string)> CreateOpenAIAgentAndSendMSG(AISetting aISetting, ChatClientAgentOptions chatClientAgentOptions, string msg, CancellationToken cancellationToken = default);
    }
}
