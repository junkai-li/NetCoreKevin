using kevin.Domain.Entities.AI;

namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// 智能体记忆向量服务接口 —— 基于 Qdrant 向量数据库的语义检索层。
    /// <para>
    /// 作为记忆的「优先通道」，由 <see cref="IAIAgentMemoryService"/> 实现类调用：
    /// 搜索时优先语义检索，失败时降级到数据库关键词搜索；
    /// 写入/更新/删除时异步同步向量到 Qdrant，失败不影响数据库主流程。
    /// </para>
    /// </summary>
    public interface IAIQdrantAgentMemoryService
    {
        /// <summary>
        /// Qdrant 客户端是否可用（配置正确且连接可用时为 true）
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// 将记忆向量写入 Qdrant（新增或覆盖，使用 Upsert）
        /// </summary>
        /// <param name="memory">已完成数据库持久化的记忆实体</param>
        Task UpsertMemoryVectorAsync(TAIAgentMemory memory);

        /// <summary>
        /// 语义搜索记忆（Qdrant 向量检索 + 可选 Rerank 重排）
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="userId">当前用户Id</param>
        /// <param name="tenantId">当前租户Id</param>
        /// <param name="aiAppsId">当前智能体Id</param>
        /// <param name="memoryType">记忆类型过滤（可选，逗号分隔多类型）</param>
        /// <returns>格式化的记忆文本；null 表示无结果或异常（调用方应降级到数据库搜索）</returns>
        Task<string?> SearchMemoryVectorAsync(string keyword, long userId, int tenantId, long aiAppsId, string? memoryType = null);

        /// <summary>
        /// 从 Qdrant 删除记忆向量（记忆被删除或软删除时调用）
        /// </summary>
        /// <param name="memoryId">记忆Id</param>
        /// <param name="tenantId">租户Id</param>
        Task DeleteMemoryVectorAsync(long memoryId, int tenantId);
    }
}
