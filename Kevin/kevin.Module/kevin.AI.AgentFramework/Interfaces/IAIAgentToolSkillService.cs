using Microsoft.Extensions.AI;

namespace kevin.AI.AgentFramework.Interfaces
{
    public interface IAIAgentToolSkillService
    {
        /// <summary>
        /// 获取智能体的可用工具
        /// </summary>
        /// <param name="data">用于传递数据</param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        Task<List<AITool>> GetAIAgentToolsAsync(object data, string agentId);

        /// <summary>
        /// 获取用户和智能体的可用工具
        /// </summary>
        /// <param name="data">用于传递数据</param>
        /// <param name="agentId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<AITool>> GetUserAIAgentToolsAsync(object data, string agentId, string userId);

        /// <summary>
        /// 获取所有的工具
        /// </summary> 
        /// <param name="data">用于传递数据</param>
        /// <returns></returns>
        Task<List<AITool>> GetAllAIAgentToolsAsync(object data);


        /// <summary>
        /// 获取智能体的Skill技能 skillPath地址
        /// </summary>
        /// <param name="data">用于传递数据</param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        Task<List<string>> GetAIAgentSkillsAsync(object data, string agentId);

        /// <summary>
        /// 获取用户和智能体的Skill技能 skillPath地址
        /// </summary>
        /// <param name="data">用于传递数据</param>
        /// <param name="agentId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<string>> GetUserAIAgentSkillsAsync(object data, string agentId, string userId);

        /// <summary>
        /// 获取所有的Skill技能 skillPath地址
        /// </summary> 
        /// <param name="data">用于传递数据</param>
        /// <returns></returns>
        Task<List<string>> GetAllAIAgentSkillsAsync(object data);
    }
}
