using kevin.AI.AgentFramework.Dto;
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
        Task<List<AITool>> GetAIAgentToolsAsync(string agentId);

        /// <summary>
        /// 获取用户和智能体的可用工具
        /// </summary>
        /// <param name="data">用于传递数据</param>
        /// <param name="agentId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<AITool>> GetUserAIAgentToolsAsync(string agentId, string userId);

        /// <summary>
        /// 获取所有的工具
        /// </summary> 
        /// <param name="data">用于传递数据</param>
        /// <returns></returns>
        Task<List<AITool>> GetAllAIAgentToolsAsync();


        /// <summary>
        /// 获取智能体的Skill技能 skillPath地址
        /// </summary>
        /// <param name="data">用于传递数据</param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        Task<List<string>> GetAIAgentSkillsAsync(string agentId);

        /// <summary>
        /// 获取用户和智能体的Skill技能 skillPath地址
        /// </summary>
        /// <param name="data">用于传递数据</param>
        /// <param name="agentId"></param>
        /// <param name="userId"></param>
        /// <param name="additionalSkillIds">本轮追加挂载的 Skill Id 列表（聊天发送时动态选择；查不到的 Id 跳过并记 Warn 日志，不阻断消息）</param>
        /// <param name="excludeDefault">true 时剔除智能体默认绑定的 Skill，仅挂载追加的</param>
        /// <returns>去重后的 Skill Name 列表（Name 即 Skills/ 目录名）</returns>
        Task<List<string>> GetUserAIAgentSkillsAsync(string agentId, string userId, List<long>? additionalSkillIds = null, bool excludeDefault = false);

        /// <summary>
        /// 获取所有的Skill技能 skillPath地址
        /// </summary> 
        /// <param name="data">用于传递数据</param>
        /// <returns></returns>
        Task<List<string>> GetAllAIAgentSkillsAsync();

        /// <summary>
        /// 获取智能体的MCP工具（含本轮 / 弹层动态追加的 MCP）
        /// </summary>
        /// <param name="agentId">智能体 Id</param>
        /// <param name="userId">当前用户 Id</param>
        /// <param name="additionalMcpIds">本轮追加挂载的 MCP Id（聊天 / 弹层动态选择，经 isDataPer 按 Id 加载，查不到静默跳过）</param>
        /// <param name="excludeDefault">true 时本轮忽略智能体默认绑定的 MCP，只挂载追加部分</param>
        /// <returns>合并后的 MCP 工具列表</returns>
        Task<List<AITool>> GetUserAIAgentMcpToolsAsync(string agentId, string userId, List<long>? additionalMcpIds = null, bool excludeDefault = false);

        /// <summary>
        /// 获取智能体记忆工具（SaveMemory/SearchMemory/UpdateMemory/DeleteMemory）。
        /// 仅在智能体开启记忆开关（IsMemory）时由 AIAppsService 注入，不作为可手动绑定的普通工具。
        /// </summary>
        /// <returns>4 个记忆工具</returns>
        Task<List<AITool>> GetMemoryToolsAsync();
    }
}
