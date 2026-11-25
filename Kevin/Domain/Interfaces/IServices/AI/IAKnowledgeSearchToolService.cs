using kevin.Domain.Share.Dtos.AI;
using Microsoft.Extensions.AI;

namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// 知识库搜索工具工厂：按智能体绑定的知识库（KmsId）构造知识库检索 AIFunction（<see cref="AITool"/>，工具名 SearchKnowledge_{知识库名后缀}）。
    /// <para>
    /// 知识库不再是发消息前固定检索一次并把结果拼进上下文（预注入），而是作为一个可搜索工具挂载给智能体，
    /// 由模型自行决定检索时机与检索词，一轮对话内可以多次检索。
    /// </para>
    /// </summary>
    public interface IAKnowledgeSearchToolService : IBaseService
    {
        /// <summary>
        /// 按智能体配置构造知识库检索 AIFunction；未绑定知识库或知识库不存在时返回 null（调用方跳过挂载）。
        /// </summary>
        /// <param name="aiapp">智能体配置（读取 KmsId/MaxMatchesCount/Relevance/ContentLengthLimit）</param>
        /// <returns>可直接加入 <c>ChatOptions.Tools</c> 的 AITool，无可用知识库时为 null</returns>
        Task<AITool?> BuildSearchFunction(AIAppsDto aiapp);

        /// <summary>
        /// 按指定知识库构造 SearchKnowledge AIFunction（多库合并挂载用）：检索配置（MaxMatchesCount/Relevance/ContentLengthLimit）
        /// 仍取自智能体，知识库按传入 kmsId 经 <c>GetDetails</c>（内部 isDataPer 数据权限）加载，
        /// 知识库不存在或无权限时返回 null（调用方跳过挂载并记 Warn 日志）。
        /// <para>工具名为 SearchKnowledge_{知识库名净化后缀}（中文名兜底 kms_{kmsId}），同一请求内重名时追加 _2/_3 序号保证 function name 唯一；单参方法与本重载走同一命名逻辑。</para>
        /// </summary>
        /// <param name="aiapp">智能体配置（读取 MaxMatchesCount/Relevance/ContentLengthLimit）</param>
        /// <param name="kmsId">要挂载的知识库 Id</param>
        /// <returns>可直接加入 <c>ChatOptions.Tools</c> 的 AITool，无可用知识库时为 null</returns>
        Task<AITool?> BuildSearchFunction(AIAppsDto aiapp, long kmsId);
    }
}
