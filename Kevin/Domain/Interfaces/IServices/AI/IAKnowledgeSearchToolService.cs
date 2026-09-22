using kevin.Domain.Share.Dtos.AI;
using Microsoft.Extensions.AI;

namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// 知识库搜索工具工厂：按智能体绑定的知识库（KmsId）构造 <c>SearchKnowledge</c> <see cref="AITool"/>。
    /// <para>
    /// 知识库不再是发消息前固定检索一次并把结果拼进上下文（预注入），而是作为一个可搜索工具挂载给智能体，
    /// 由模型自行决定检索时机与检索词，一轮对话内可以多次检索。
    /// </para>
    /// </summary>
    public interface IAKnowledgeSearchToolService : IBaseService
    {
        /// <summary>
        /// 按智能体配置构造 SearchKnowledge AIFunction；未绑定知识库或知识库不存在时返回 null（调用方跳过挂载）。
        /// </summary>
        /// <param name="aiapp">智能体配置（读取 KmsId/MaxMatchesCount/Relevance/ContentLengthLimit）</param>
        /// <returns>可直接加入 <c>ChatOptions.Tools</c> 的 AITool，无可用知识库时为 null</returns>
        Task<AITool?> BuildSearchFunction(AIAppsDto aiapp);
    }
}
