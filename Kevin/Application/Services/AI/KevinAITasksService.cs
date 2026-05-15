using DocumentFormat.OpenXml.Wordprocessing;
using Hangfire;
using Hangfire.Storage;
using kevin.AI.AgentFramework.Tasks;
using System.ComponentModel;

namespace kevin.Application.Services.AI
{
    public class KevinAITasksService : BaseService, IKevinAITasksService
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly JobStorage _jobStorage;
        public KevinAITasksService(IHttpContextAccessor _httpContextAccessor, IRecurringJobManager recurringJobManager, JobStorage jobStorage) : base(_httpContextAccessor)
        {
            _recurringJobManager = recurringJobManager;
            _jobStorage = jobStorage; // 可通过 DI 注入；若为 null，会回退到 JobStorage.Current
        }
        public Task<string> AddOrUpdateCronTask([Description("name：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结")] string name, [Description("content：可传入具体的任务内容，不可为空 比如：自动搜索并总结AI领域的热门资讯，包括技术突破、产品发布、行业动态等")] string content, [Description("cron表达式：用于定义任务的执行周期，不可为空 比如用户需要每六分钟执行一次则传入：0 0/6 0/1 * * ?  ")] string cronExpression)
        {
            _recurringJobManager.AddOrUpdate<IKevinAITasksService>(
               recurringJobId: CurrentUser.UserId + name,      // 唯一的 ID，用于后续修改或删除
               (s) => s.RunTask(name, content),    // 要执行的任务
               cronExpression, new RecurringJobOptions
               {
                   TimeZone = TimeZoneInfo.Local,        // 指定时区（默认UTC） 
               }
           );
            Console.WriteLine(CurrentUser.UserId + "：添加或更新定时任务" + name + content + cronExpression);
            return Task.FromResult("添加或更新定时任务成功：" + name + content + cronExpression);
        }

        public Task<List<string>> GetTaskList()
        {
            Console.WriteLine(CurrentUser.UserId + "：查询任务列表");
            // 优先使用注入的 JobStorage；若未注入则回退到静态 JobStorage.Current（需确保已初始化）
            var storage = _jobStorage ?? JobStorage.Current;
            if (storage == null)
            {
                return Task.FromResult(new List<string> { "Hangfire JobStorage 未初始化" });
            }

            var connection = storage.GetConnection();
            var recurringJobs = connection.GetRecurringJobs(); // 返回 IList<RecurringJobDto>

            var result = recurringJobs.Where(t => t.Id.StartsWith(CurrentUser.UserId.ToString())).Select(r =>
            {
                var next = r.NextExecution?.ToLocalTime().ToString("u") ?? "null";
                var last = r.LastExecution?.ToLocalTime().ToString("u") ?? "null";
                return $"name:{r.Id} | Cron:{r.Cron} | Next:{next} | Last:{last} | TimeZone:{r.TimeZoneId}";
            }).ToList();
            return Task.FromResult(result);
        }
        public Task<string> RemoveCronTask([Description("name：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结")] string name)
        {
            _recurringJobManager.RemoveIfExists(CurrentUser.UserId + name);
            Console.WriteLine(CurrentUser.UserId + "；移除定时任务" + name);
            return Task.FromResult("移除定时任务成功：" + name);
        }

        public Task<string> TriggerCronTask([Description("name：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结")] string name)
        {
            _recurringJobManager.Trigger(CurrentUser.UserId + name);
            Console.WriteLine(CurrentUser.UserId + "：执行定时任务" + name);
            return Task.FromResult("执行定时任务成功：" + name);
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="taskName">任务名称</param>
        /// <param name="taskContent">任务内容</param>
        /// <returns></returns>
        public Task<string> RunTask(string taskName, string taskContent)
        {
            //这里可以根据任务名称和内容执行具体的业务逻辑，比如调用AI接口、处理数据等。当前示例仅打印日志并返回结果。 自行处理
            if (CurrentUser != default)
            {
                Console.WriteLine(CurrentUser.UserId + "：执行任务" + taskName + taskContent);
            } 
            return Task.FromResult($"执行任务：{taskName}，内容：{taskContent}");
        }
    }
}
