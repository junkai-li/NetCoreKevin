using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Entities.AI;
namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// 智能体记忆服务接口（用户级长期与短期记忆）
    /// </summary>
    public interface IAIAgentMemoryService : IBaseService
    {
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="dtoPagePar"></param>
        /// <returns></returns>
        Task<dtoPageData<TAIAgentMemory>> GetPageData(dtoPagePar<string> dtoPagePar);

        /// <summary>
        /// 保存记忆
        /// </summary>
        /// <param name="content">记忆内容</param>
        /// <param name="keywords">关键词，逗号分隔</param>
        /// <param name="memoryType">记忆类型（必填，7 种合法值；task 为短期记忆）</param>
        /// <param name="importance">重要程度 0~10（必填）</param>
        /// <param name="expireTime">task 必填；其他长期记忆不得传入</param>
        /// <returns></returns>
        [Description("保存用户的长期或短期记忆。调用前必须先 SearchMemory 检查是否已存在类似记忆，若存在则改用 UpdateMemory。长期记忆须满足可复用、非显然、稳定、用户意图明确；task 为有过期时间的短期记忆。失败返回以 ❌ 开头的错误信息")]
        Task<string> SaveMemoryAsync(
            [Description("记忆内容，用一句话完整描述要记住的信息，说明“是什么/为什么”")][Required] string content,
            [Description("记忆关键词，2-5 个核心实体/概念/技术术语，英文逗号分隔，优先专有名词避免泛词，如：UserFriendlyException,HTTP 400,登录失败")][Required] string keywords,
            [Description("记忆类型（必填，禁止全用 other）：preference偏好/fact事实/task短期记忆/decision决策/pitfall踩坑/skill技能/other其他。详见系统提示词 4.3 分类表")][Required] string memoryType,
            [Description("重要程度 0-10（必填）：9-10核心约束/7-8重要决策偏好/5-6一般事实经验/3-4边缘信息/0-2低价值不该保存。详见系统提示词 4.4 打分表")][Required] int importance,
            [Description("短期记忆（task）必须传过期时间字符串；长期记忆不得传。支持格式：yyyy-MM-dd HH:mm，yyyy-MM-dd HH:mm:ss，ISO 8601。例如：2026-12-31 23:59。必须大于当前时间，否则拒绝")] string expireTime = "");

        /// <summary>
        /// 搜索记忆
        /// </summary>
        /// <param name="keyword">检索关键词</param>
        /// <param name="memoryType">记忆类型过滤（可选，逗号分隔多类型）</param>
        /// <returns></returns>
        [Description("搜索当前用户的有效记忆（长期与未过期短期记忆）。需要回忆用户偏好、历史事实、约定事项时先调用本工具。支持按 memoryType 过滤精准检索")]
        Task<string> SearchMemoryAsync(
            [Description("检索关键词，可以是多个词用逗号分隔")][Required] string keyword,
            [Description("记忆类型过滤（可选）：preference/fact/task/decision/pitfall/skill/other，多个用逗号分隔如 decision,pitfall。空字符串或不传表示不过滤，搜索全部类型")] string memoryType = "");

        /// <summary>
        /// 更新记忆
        /// </summary>
        /// <param name="id">记忆Id（先通过搜索获取）</param>
        /// <param name="content">新的记忆内容</param>
        /// <param name="keywords">新的关键词</param>
        /// <returns></returns>
        [Description("更新已有的长期记忆。当记忆内容发生变化时调用，Id 需要先通过搜索记忆获取。")]
        Task<string> UpdateMemoryAsync(
            [Description("要更新的记忆Id")][Required] long id,
            [Description("新的记忆内容")][Required] string content,
            [Description("新的关键词，逗号分隔")] string keywords = "");

        /// <summary>
        /// 删除记忆
        /// </summary>
        /// <param name="id">记忆Id（先通过搜索获取）</param>
        /// <returns></returns>
        [Description("删除不再需要的长期记忆。当用户明确要求忘记某事或记忆已失效时调用，Id 需要先通过搜索记忆获取。")]
        Task<string> DeleteMemoryAsync([Description("要删除的记忆Id")][Required] long id);

        /// <summary>
        /// 添加或编辑
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddEdit(TAIAgentMemory data);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);
    }
}
