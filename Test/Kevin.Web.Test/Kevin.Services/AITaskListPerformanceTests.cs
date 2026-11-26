using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Storage;
using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces.Tasks;
using Xunit.Abstractions;

namespace Kevin.Web.Test.Kevin.Services
{
    /// <summary>
    /// 自动任务规模化压测（AI 验证成千上万任务会不会卡顿）：
    /// 在 Hangfire MemoryStorage 上模拟数千任务，量化旧列表路径（GetRecurringJobs 全量拉取+前缀过滤）、
    /// 旧一次性路径（ScheduledJobs 全量扫描）、新管理端路径（TAITaskConfig 数据库分页 + 仅当页 Hangfire 索引匹配）的耗时差异；
    /// 并验证暂停语义：RemoveIfExists 后作业确从存储消失。
    /// </summary>
    public class AITaskListPerformanceTests
    {
        private readonly ITestOutputHelper _output;

        public AITaskListPerformanceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private const int RecurringCount = 3000;
        private const int OnceCount = 2000;
        private const string TestUserPrefix = "perfuser_";

        private static void SeedRecurringJobs(MemoryStorage storage)
        {
            var manager = new RecurringJobManager(storage);
            for (int i = 0; i < RecurringCount; i++)
            {
                var name = "压测周期任务" + i.ToString("D5");
                manager.AddOrUpdate<IKevinAITaskService>(
                    TestUserPrefix + name,
                    s => s.RunTask(TestUserPrefix, name, "压测内容" + i, new AIShareInfoDto()),
                    "0 0/6 * * * ?",
                    new RecurringJobOptions());
            }
        }

        private static void SeedOnceJobs(MemoryStorage storage)
        {
            var client = new BackgroundJobClient(storage);
            for (int i = 0; i < OnceCount; i++)
            {
                var name = "压测一次性任务" + i.ToString("D5");
                client.Schedule<IKevinAITaskService>(
                    s => s.RunTask(TestUserPrefix, name, "压测内容" + i, new AIShareInfoDto()),
                    TimeSpan.FromHours(2 + i));
            }
        }

        [Fact]
        public void ThousandsOfTasks_OldListPath_IsFullScan()
        {
            var storage = new MemoryStorage();
            var sw = System.Diagnostics.Stopwatch.StartNew();
            SeedRecurringJobs(storage);
            sw.Stop();
            _output.WriteLine("注册 " + RecurringCount + " 个周期任务耗时: " + sw.ElapsedMilliseconds + " ms");

            sw.Restart();
            var all = storage.GetConnection().GetRecurringJobs();
            var mine = all.Where(t => t.Id.StartsWith(TestUserPrefix)).ToList();
            sw.Stop();
            _output.WriteLine("旧路径全量拉取+过滤（总量 " + all.Count + "，我的 " + mine.Count + "）耗时: " + sw.ElapsedMilliseconds + " ms");

            Assert.Equal(RecurringCount, mine.Count);
            Assert.Equal(RecurringCount, all.Count);
        }

        [Fact]
        public void ThousandsOfTasks_OldOncePath_IsFullScan()
        {
            var storage = new MemoryStorage();
            SeedOnceJobs(storage);

            var sw = System.Diagnostics.Stopwatch.StartNew();
            var scheduled = storage.GetMonitoringApi().ScheduledJobs(0, int.MaxValue);
            var mine = scheduled.Where(t => t.Value?.Job?.Type == typeof(IKevinAITaskService)
                && t.Value.Job.Args.FirstOrDefault()?.ToString() == TestUserPrefix).ToList();
            sw.Stop();
            _output.WriteLine("旧路径 ScheduledJobs 全量扫描（队列总量 " + scheduled.Count + "，我的 " + mine.Count + "）耗时: " + sw.ElapsedMilliseconds + " ms");

            Assert.Equal(OnceCount, mine.Count);
        }

        [Fact]
        public void ThousandsOfTasks_NewPagedListPath_IsPageCost()
        {
            var storage = new MemoryStorage();
            SeedRecurringJobs(storage);
            SeedOnceJobs(storage);

            // 新管理端路径：权威源 TAITaskConfig 走 SQL 分页（内存列表等价模拟 Skip/Take），Hangfire 只补当页调度信息
            // 配置名与种子作业名对齐，验证页内 jobId 匹配
            var configs = Enumerable.Range(0, RecurringCount + OnceCount)
                .Select(i => new
                {
                    Name = i < RecurringCount ? "压测周期任务" + i.ToString("D5") : "压测一次性任务" + (i - RecurringCount).ToString("D5"),
                    Type = i < RecurringCount ? "cron" : "once"
                })
                .ToList();

            var sw = System.Diagnostics.Stopwatch.StartNew();
            const int pageSize = 20;
            var page = configs
                .OrderByDescending(t => t.Name)
                .Skip(0)
                .Take(pageSize)
                .ToList();
            sw.Stop();
            var pagedMs = sw.ElapsedMilliseconds;

            // 当页补充调度信息：一次 GetRecurringJobs 建索引 + 页内 O(1) 匹配
            sw.Restart();
            var recurringIndex = storage.GetConnection().GetRecurringJobs()
                .GroupBy(r => r.Id).ToDictionary(g => g.Key, g => g.First());
            var matched = 0;
            foreach (var item in page)
            {
                if (item.Type == "cron" && recurringIndex.ContainsKey(TestUserPrefix + item.Name))
                {
                    matched++;
                }
            }
            sw.Stop();
            var hangfireMs = sw.ElapsedMilliseconds;

            _output.WriteLine("新路径 数据库分页（总 " + configs.Count + " 条，页 " + page.Count + " 条）耗时: " + pagedMs + " ms；"
                + "Hangfire 索引构建+当页匹配（" + RecurringCount + " 个作业）耗时: " + hangfireMs + " ms，命中 " + matched);
            Assert.Equal(pageSize, page.Count);
            Assert.Equal(Math.Min(pageSize, RecurringCount), matched);

            // 结论性断言：分页取页本身必须是毫秒级（<50ms），与全量任务数解耦
            Assert.True(pagedMs < 50, "新路径分页应在毫秒级，实际 " + pagedMs + " ms");
        }

        [Fact]
        public void Pause_RemovesJobFromStorage_Consistently()
        {
            var storage = new MemoryStorage();
            var manager = new RecurringJobManager(storage);
            var name = "可暂停任务";
            var jobId = TestUserPrefix + name;
            manager.AddOrUpdate<IKevinAITaskService>(
                jobId,
                s => s.RunTask(TestUserPrefix, name, "暂停语义验证", new AIShareInfoDto()),
                "0 0/6 * * * ?",
                new RecurringJobOptions());
            Assert.True(storage.GetConnection().GetRecurringJobs().Any(t => t.Id == jobId), "注册后作业应存在");

            // 暂停 = RemoveIfExists：作业从存储消失，任何存储下都不会再触发
            manager.RemoveIfExists(jobId);

            var remaining = storage.GetConnection().GetRecurringJobs().Any(t => t.Id == jobId);
            Assert.False(remaining, "暂停后作业不应仍存在于存储中");
        }
    }
}
