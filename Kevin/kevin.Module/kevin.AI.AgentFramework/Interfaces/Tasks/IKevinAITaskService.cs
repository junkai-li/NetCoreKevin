using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kevin.AI.AgentFramework.Interfaces.Tasks
{

    /// <summary>
    /// 用于给AI使用的自动任务服务接口，提供自动任务相关的功能和操作 你可以让它在每天、每周、每月，或者某个固定时间自动运行，帮助你完成常见的日常工作。 
    /// </summary>
    [Description("用于给AI使用的自动任务服务接口，提供自动任务相关的功能和操作 你可以让它在每天、每周、每月，或者某个固定时间自动运行，帮助你完成常见的日常工作。")]
    public interface IKevinAITaskService : IBaseAIToolService
    {

        /// <summary>
        /// 创建或更新一个周期性自动任务 
        /// <param name="cronExpression">cron表达式：用于定义任务的执行周期，不可为空</param>
        /// </summary>
        Task<string> AddOrUpdateCronTask([Description("可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结")][Required] string name,
      [Description("可传入具体的任务内容（禁止传入自动任务相关词汇，只能传入任务步骤！！！）。 比如：第一步：搜索并总结AI领域的热门资讯，包括技术突破、产品发布、行业动态等，第二步：生成总结报告为MkD格式")] string content,
            [Description("cron表达式：用于定义任务的执行周期，不可为空 比如用户需要每六分钟执行一次则传入：0 0/6 0/1 * * ?  ")][Required] string cronExpression
         );

        /// <summary>
        /// 创建一个一次性任务：在指定的未来时间点执行一次后自动结束，不会重复执行，也无需移除
        /// </summary>
        Task<string> AddOnceTask([Description("可传入具体的任务名称，不可为空 比如：明天上午九点总结AI热门资讯")][Required] string name,
      [Description("可传入具体的任务内容（禁止传入自动任务相关词汇，只能传入任务步骤！！！）。 比如：第一步：搜索并总结AI领域的热门资讯，包括技术突破、产品发布、行业动态等，第二步：生成总结报告为MkD格式")] string content,
            [Description("执行时间点，不可为空，必须是未来的时间，格式：yyyy-MM-dd HH:mm 比如：2026-08-27 09:00 表示2026年8月27日上午9点执行一次")][Required] DateTime executeTime
         );

        /// <summary>
        /// 移除周期性任务（如果存在） 
        /// <param name="name">name ：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结"</param>
        /// </summary>
        Task<string> RemoveCronTask([Description("name：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结")][Required] string name);

        /// <summary>
        /// 立即触发某个周期性任务一次 
        /// <param name="name">name ：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结"</param>
        /// </summary>
        Task<string> TriggerCronTask([Description("name：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结")][Required] string name);

        /// <summary>
        /// 获取我的所有周期性任务列表，返回任务名称列表 
        /// </summary>
        Task<List<string>> GetTaskList();

        /// <summary>
        /// 执行任务, 返回任务执行结果
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="taskContent">任务内容</param>
        /// <returns></returns>
        [Description("执行任务, 返回任务执行结果")]
        public Task<string> RunTask([Required][Description("用户ID")] string userId, [Required][Description("任务名称")] string taskName, [Required][Description("任务内容")] string taskContent, [Required] object taskdata);
    }

}
