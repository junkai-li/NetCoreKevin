using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Entities.AI;
namespace kevin.Domain.Interfaces.IServices.AI
{
    /// <summary>
    /// 智能体记忆服务接口（用户级长期记忆）
    /// </summary>
    public interface IAIAgentMemoryService : IBaseService, IBaseAIToolService
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
        /// <param name="memoryType">记忆类型</param>
        /// <param name="importance">重要程度0~10</param>
        /// <returns></returns>
        [Description("保存用户的长期记忆。当用户表达个人偏好、习惯、重要事实或需要以后记住的事项时调用。")]
        Task<string> SaveMemoryAsync(
            [Description("记忆内容，用一句话完整描述要记住的信息")][Required] string content,
            [Description("记忆关键词，逗号分隔，便于以后检索，如：咖啡,口味偏好")][Required] string keywords,
            [Description("记忆类型：preference偏好/fact事实/task任务/other其他")] string memoryType = "other",
            [Description("重要程度0~10，默认5")] int importance = 5);

        /// <summary>
        /// 搜索记忆
        /// </summary>
        /// <param name="keyword">检索关键词</param>
        /// <returns></returns>
        [Description("搜索当前用户的长期记忆。需要回忆用户偏好、历史事实、约定事项时先调用本工具。")]
        Task<string> SearchMemoryAsync([Description("检索关键词，可以是多个词用逗号分隔")][Required] string keyword);

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
