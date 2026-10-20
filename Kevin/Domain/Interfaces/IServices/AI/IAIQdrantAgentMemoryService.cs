using kevin.Domain.Entities.AI;

namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// Qdrant 记忆检索命中结果（结构化，供自动召回等需要 Id/分数 的调用方使用）
    /// </summary>
    /// <param name="Id">记忆Id</param>
    /// <param name="Content">记忆内容</param>
    /// <param name="Keywords">记忆关键词</param>
    /// <param name="MemoryType">记忆类型</param>
    /// <param name="Importance">重要程度 0-10</param>
    /// <param name="Score">综合相关度得分（相关性 × 70% + 重要度 × 30%）</param>
    public sealed record AIQdrantMemoryHit(long Id, string Content, string Keywords, string MemoryType, int Importance, double Score);

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
        /// <param name="maxReturnCount">最多返回条数（默认 10）</param>
        /// <returns>格式化的记忆文本；null 表示无结果或异常（调用方应降级到数据库搜索）</returns>
        Task<string?> SearchMemoryVectorAsync(string keyword, long userId, int tenantId, long aiAppsId, string? memoryType = null, int maxReturnCount = 10);

        /// <summary>
        /// 语义搜索记忆并返回结构化命中（自动召回用：走更高的相似度阈值、不做 Rerank，控制时延与噪声）
        /// </summary>
        /// <param name="keyword">用于生成查询向量的文本（通常是用户当前提问）</param>
        /// <param name="userId">当前用户Id</param>
        /// <param name="tenantId">当前租户Id</param>
        /// <param name="aiAppsId">当前智能体Id</param>
        /// <param name="topN">最多返回条数</param>
        /// <param name="minScore">Qdrant 相似度阈值</param>
        /// <returns>按综合分降序的命中列表；无结果返回空列表</returns>
        Task<List<AIQdrantMemoryHit>> SearchMemoryVectorHitsAsync(string keyword, long userId, int tenantId, long aiAppsId, int topN = 3, float minScore = 0.55f);

        /// <summary>
        /// 从 Qdrant 删除记忆向量（记忆被删除或软删除时调用）
        /// </summary>
        /// <param name="memoryId">记忆Id</param>
        /// <param name="tenantId">租户Id</param>
        /// <param name="aiAppsId">智能体Id</param>
        /// <param name="userId">用户Id</param>
        Task DeleteMemoryVectorAsync(long memoryId, int tenantId, long aiAppsId, long userId);
    }
}
