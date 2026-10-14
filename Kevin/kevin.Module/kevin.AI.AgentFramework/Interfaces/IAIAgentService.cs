using kevin.AI.AgentFramework.Dto;
using Kevin.AI.Dto;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

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
        Task<(AIAgent, string, TokenConsumptionInfo)> CreateOpenAIAgentAndSendMSG(AISetting aISetting, ChatClientAgentOptions chatClientAgentOptions, ChatMessage messages, CancellationToken cancellationToken = default);

        /// <summary>
        /// 创建代理并发送消息（多消息重载）：支持 OpenAI function calling 续轮，把调用方回传的工具结果作为本轮输入
        /// </summary>
        /// <param name="aISetting"></param>
        /// <param name="chatClientAgentOptions"></param>
        /// <param name="messages">本轮发送给模型的消息集合：普通提问为单条 user 消息，工具续轮为工具结果（tool）消息</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<(AIAgent, string, TokenConsumptionInfo)> CreateOpenAIAgentAndSendMSG(AISetting aISetting, ChatClientAgentOptions chatClientAgentOptions, IEnumerable<ChatMessage> messages, CancellationToken cancellationToken = default);

        /// <summary>
        /// 创建AI代理 
        /// </summary>
        /// <param name="aISetting"></param>
        /// <param name="chatClientAgentOptions"></param> 
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
         Task<AIAgent> CreateOpenAIAgent(AISetting aISetting, ChatClientAgentOptions chatClientAgentOptions, CancellationToken cancellationToken = default);


    }
}
