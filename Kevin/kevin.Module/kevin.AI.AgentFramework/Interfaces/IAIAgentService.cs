using Kevin.AI.Dto;
using Microsoft.Agents.AI;

namespace kevin.AI.AgentFramework.Interfaces
{
    public interface IAIAgentService
    {
        /// <summary>
        /// 创建代理并发送消息
        /// </summary> 
        /// <param name="chatClientAgentOptions"></param> 
        /// <param name="OtherContents">其他内容：用来存放一些需要传递给代理的内容 比如文件内容 互联网信息等</param>
        /// <returns></returns>
        Task<(AIAgent, string)> CreateOpenAIAgentAndSendMSG(AISetting aISetting, ChatClientAgentOptions chatClientAgentOptions, string msg, List<string>? OtherContents = default, CancellationToken cancellationToken = default);
    }
}
