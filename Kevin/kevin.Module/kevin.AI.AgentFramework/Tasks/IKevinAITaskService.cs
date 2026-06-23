using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Tasks
{

    /// <summary>
    /// 用于给AI使用的自动任务服务接口，提供自动任务相关的功能和操作 你可以让它在每天、每周、每月，或者某个固定时间自动运行，帮助你完成常见的日常工作。 
    /// </summary>
    [Description("用于给AI使用的自动任务服务接口，提供自动任务相关的功能和操作 你可以让它在每天、每周、每月，或者某个固定时间自动运行，帮助你完成常见的日常工作。")]
    public interface IKevinAITaskService
    {

        /// <summary>
        /// 初始化数据 用于AI前传递数据
        /// </summary>
        /// <param name="data"></param>
        public void InitData(object data);

        /// <summary>
        /// 创建或更新一个周期性自动任务 
        /// <param name="cronExpression">cron表达式：用于定义任务的执行周期，不可为空</param>
        /// </summary>
        Task<string> AddOrUpdateCronTask([Description("name：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结")] string name,
            [Description("content：可传入具体的任务内容，不可为空 比如：第一步：搜索并总结AI领域的热门资讯，包括技术突破、产品发布、行业动态等，第二步：生成总结报告为MkD格式")] string content,
            [Description("cron表达式：用于定义任务的执行周期，不可为空 比如用户需要每六分钟执行一次则传入：0 0/6 0/1 * * ?  ")] string cronExpression
         );

        /// <summary>
        /// 移除周期性任务（如果存在） 
        /// <param name="name">name ：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结"</param>
        /// </summary>
        Task<string> RemoveCronTask([Description("name：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结")] string name);

        /// <summary>
        /// 立即触发某个周期性任务一次 
        /// <param name="name">name ：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结"</param>
        /// </summary>
        Task<string> TriggerCronTask([Description("name：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结")] string name);

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
        public Task<string> RunTask(string userId, string taskName, string taskContent, object taskdata);
    }

}
