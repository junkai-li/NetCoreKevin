using Common.Json;
using DocumentFormat.OpenXml.Wordprocessing;
using Hangfire;
using Hangfire.Storage;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tasks;
using kevin.AI.AgentFramework.ScriptRunners;
using kevin.AI.AgentFramework.Tools;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using kevin.Domain.Share.Enums;
using Kevin.AI.Dto;
using Kevin.Common.Extension;
using Kevin.log4Net;
using Medallion.Threading;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace kevin.Application.Services.AI.Tools
{
    public class KevinAITasksService : BaseService, IKevinAITaskService
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly JobStorage _jobStorage;

        private readonly IMessageService _messageService;

        private readonly IAIAgentService _aIAgentService;
        private readonly IAIChatsRp aIChatsRp;
        private readonly IAIModelsRp _aIModelsRp;
        private readonly IAIPromptsRp _aIPromptsRp;
        private readonly IAIShareInfoService _aIShareInfoService;
        private readonly IAITaskConfigRp _aITaskConfigRp;
        private IDistributedLockProvider distLock { get; set; }

        public KevinAITasksService(IHttpContextAccessor _httpContextAccessor, IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient, JobStorage jobStorage, IMessageService messageService,
            IAIAgentService aIAgentService, IAIModelsRp aIModelsRp, IAIPromptsRp aIPromptsRp, IAIChatsRp aIChatsRp, IServiceProvider serviceProvider,
            IDistributedLockProvider distLock, IAIShareInfoService aIShareInfoService, IAITaskConfigRp aITaskConfigRp) : base(_httpContextAccessor)
        {
            _serviceProvider = serviceProvider;
            _recurringJobManager = recurringJobManager;
            _backgroundJobClient = backgroundJobClient;
            _jobStorage = jobStorage; // 可通过 DI 注入；若为 null，会回退到 JobStorage.Current
            _messageService = messageService;
            _aIAgentService = aIAgentService;
            this._aIModelsRp = aIModelsRp;
            this._aIPromptsRp = aIPromptsRp;
            this.aIChatsRp = aIChatsRp;
            this.distLock = distLock;
            this._aIShareInfoService = aIShareInfoService;
            this._aITaskConfigRp = aITaskConfigRp;
        }

        /// <summary>
        /// 按 用户+名称 取任务配置（唯一键；忽略数据权限/租户过滤，调用方自行保证归属口径）
        /// </summary>
        private async Task<TAITaskConfig?> GetTaskConfigAsync(long userId, string name)
        {
            return await _aITaskConfigRp.Query(isDataPer: false, isTenant: false)
                .Where(t => t.IsDelete == false && t.UserId == userId && t.Name == name)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// 新增或更新任务配置（TAITaskConfig 为任务权威注册表；Hangfire 侧只负责调度，暂停状态与恢复元数据都在这里）
        /// </summary>
        private async Task UpsertTaskConfigAsync(long userId, string name, string taskType, string cronExpression, DateTime? executeTime, string content, AIShareInfoDto taskdata)
        {
            var config = await GetTaskConfigAsync(userId, name);
            if (config == null)
            {
                config = new TAITaskConfig
                {
                    // 项目惯例：主键在应用层赋雪花id（全库统一，避免依赖单库自增）；CreateTime 同惯例手动赋值
                    Id = SnowflakeIdService.GetNextId(),
                    UserId = userId,
                    Name = name,
                    TaskType = taskType,
                    CronExpression = cronExpression ?? "",
                    ExecuteTime = executeTime,
                    TaskContent = content ?? "",
                    TaskData = taskdata?.ToJson() ?? "",
                    IsPaused = false,
                    CreateTime = DateTime.Now
                };
                _aITaskConfigRp.Add(config);
            }
            else
            {
                config.TaskType = taskType;
                config.CronExpression = cronExpression ?? "";
                config.ExecuteTime = executeTime;
                config.TaskContent = content ?? "";
                config.TaskData = taskdata?.ToJson() ?? config.TaskData;
                config.IsPaused = false; // 重新添加/更新即恢复
            }
            await _aITaskConfigRp.SaveChangesAsync();
        }

        /// <summary>
        /// 按落库配置注册周期性任务到 Hangfire（恢复/启动对账共用；taskdata 从配置 JSON 还原）
        /// </summary>
        private void RegisterCronJobFromConfig(TAITaskConfig config)
        {
            var taskdata = string.IsNullOrWhiteSpace(config.TaskData) ? new AIShareInfoDto() : config.TaskData.ToObject<AIShareInfoDto>() ?? new AIShareInfoDto();
            _recurringJobManager.AddOrUpdate<IKevinAITaskService>(
                recurringJobId: config.UserId + config.Name,
                (s) => s.RunTask(config.UserId.ToString(), config.Name, config.TaskContent, taskdata),
                config.CronExpression, new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                });
        }

        /// <summary>
        /// 按落库配置重新调度一次性任务（仅当执行时间在未来且 Hangfire 侧无同名未执行作业时才补排，双存储幂等）
        /// </summary>
        /// <returns>true=已补排；false=已过期或已存在未执行作业</returns>
        private bool ScheduleOnceJobFromConfig(TAITaskConfig config, IDictionary<string, List<string>> onceJobIndex)
        {
            if (config.ExecuteTime == null || config.ExecuteTime <= DateTime.Now)
            {
                return false;
            }
            var key = config.UserId + "|" + config.Name;
            if (onceJobIndex != null && onceJobIndex.ContainsKey(key) && onceJobIndex[key].Count > 0)
            {
                return false; // Hangfire 侧已有同名未执行作业（Redis 持久化场景重启后仍在），不重复调度
            }
            var taskdata = string.IsNullOrWhiteSpace(config.TaskData) ? new AIShareInfoDto() : config.TaskData.ToObject<AIShareInfoDto>() ?? new AIShareInfoDto();
            _backgroundJobClient.Schedule<IKevinAITaskService>(
                (s) => s.RunTask(config.UserId.ToString(), config.Name, config.TaskContent, taskdata),
                DateTime.SpecifyKind(config.ExecuteTime.Value, DateTimeKind.Local));
            return true;
        }

        /// <summary>
        /// 启动对账：以 TAITaskConfig 为权威源，把任务状态同步到 Hangfire。
        /// <para>双存储兼容：内存存储重启后作业全丢（由本方法重建）；Redis 持久化存储作业仍在（AddOrUpdate 幂等重放等于 no-op，一次性任务先扫 Scheduled 队列去重防双排）。</para>
        /// <para>暂停任务一律 RemoveIfExists/删除作业，确保任何存储下都不会复活。</para>
        /// </summary>
        public async Task<string> ReconcileTasksFromConfigAsync()
        {
            var configs = await _aITaskConfigRp.Query(isDataPer: false, isTenant: false)
                .Where(t => t.IsDelete == false)
                .ToListAsync();
            // 一次性任务：一次 O(N) 扫描 Scheduled 队列建立 (userId|name) -> jobIds 索引，避免逐任务全量扫描
            var onceJobIndex = IndexOnceJobs();
            var registered = 0; var removed = 0; var rescheduled = 0;
            foreach (var config in configs)
            {
                try
                {
                    if (config.TaskType == "cron")
                    {
                        if (config.IsPaused)
                        {
                            _recurringJobManager.RemoveIfExists(config.UserId + config.Name);
                            removed++;
                        }
                        else
                        {
                            RegisterCronJobFromConfig(config);
                            registered++;
                        }
                    }
                    else
                    {
                        if (config.IsPaused)
                        {
                            foreach (var jobId in GetOnceJobIds(config.UserId, config.Name, onceJobIndex))
                            {
                                _backgroundJobClient.Delete(jobId);
                            }
                            removed++;
                        }
                        else if (ScheduleOnceJobFromConfig(config, onceJobIndex))
                        {
                            rescheduled++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.logger.Error($"自动任务启动对账失败：userId={config.UserId} name={config.Name}，异常信息：{ex.Message}", ex);
                }
            }
            var msg = $"自动任务启动对账完成：注册周期性 {registered}，补排一次性 {rescheduled}，清理(含暂停) {removed}，配置总数 {configs.Count}";
            LogHelper.logger.Info(msg);
            return msg;
        }

        /// <summary>
        /// 任务注册/移除/暂停/恢复后立即失效索引缓存，避免 30 秒内列表显示陈旧的下次执行时间
        /// </summary>
        private void InvalidateRecurringIndexCache()
        {
            lock (RecurringIndexLock)
            {
                _recurringIndexCacheTime = DateTime.MinValue;
            }
        }

        private static readonly object RecurringIndexLock = new object();
        private static Dictionary<string, RecurringJobDto>? _recurringIndexCache;
        private static DateTime _recurringIndexCacheTime = DateTime.MinValue;
        private const int RecurringIndexCacheTtlSeconds = 30;

        /// <summary>
        /// 读取全部 RecurringJob 并按 jobId 建索引（30 秒进程级缓存）。
        /// GetRecurringJobs 是 Hangfire 全量读取（无前缀查询能力），数千任务时单次可达秒级，缓存是列表接口保持毫秒级响应的关键
        /// </summary>
        private Dictionary<string, RecurringJobDto> GetRecurringIndexCached()
        {
            lock (RecurringIndexLock)
            {
                if (_recurringIndexCache != null && (DateTime.Now - _recurringIndexCacheTime).TotalSeconds < RecurringIndexCacheTtlSeconds)
                {
                    return _recurringIndexCache;
                }
                var storage = _jobStorage ?? JobStorage.Current;
                var dict = new Dictionary<string, RecurringJobDto>(StringComparer.Ordinal);
                if (storage != null)
                {
                    dict = storage.GetConnection().GetRecurringJobs()
                        .GroupBy(r => r.Id).ToDictionary(g => g.Key, g => g.First());
                }
                _recurringIndexCache = dict;
                _recurringIndexCacheTime = DateTime.Now;
                return dict;
            }
        }

        /// <summary>
        /// 建立当前 Scheduled 队列中 AI 任务作业的 (userId|name) -> jobIds 索引（一次全量扫描）
        /// </summary>
        private Dictionary<string, List<string>> IndexOnceJobs()
        {
            var index = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            try
            {
                var storage = _jobStorage ?? JobStorage.Current;
                if (storage == null) return index;
                foreach (var t in storage.GetMonitoringApi().ScheduledJobs(0, int.MaxValue))
                {
                    if (t.Value?.Job?.Type != typeof(IKevinAITaskService)) continue;
                    var args = t.Value.Job.Args;
                    if (args.Count < 2) continue;
                    var key = args[0]?.ToString() + "|" + args[1]?.ToString();
                    if (!index.TryGetValue(key, out var list))
                    {
                        index[key] = list = new List<string>();
                    }
                    list.Add(t.Key);
                }
            }
            catch (Exception ex)
            {
                LogHelper.logger.Warn("启动对账扫描 Scheduled 队列失败（不影响周期性任务对账）：" + ex.Message);
            }
            return index;
        }

        /// <summary>
        /// 从索引取指定用户+名称的一次性作业 jobIds
        /// </summary>
        private List<string> GetOnceJobIds(long userId, string name, Dictionary<string, List<string>> onceJobIndex)
        {
            return onceJobIndex.TryGetValue(userId + "|" + name, out var list) ? list : new List<string>();
        }
        public static bool IsValidCronExpression(string cronExpression)
        {
            if (string.IsNullOrWhiteSpace(cronExpression))
                return false;

            // 匹配 5 位（分 时 日 月 周）或 6 位（秒 分 时 日 月 周）
            // 允许数字、*、?、-、,、/、L、W、# 等特殊字符
            string pattern = @"^(\S+\s){4,5}\S+$";
            if (!Regex.IsMatch(cronExpression.Trim(), pattern))
                return false;

            // 简单检查字段数量
            var fields = cronExpression.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length < 5 || fields.Length > 7)
                return false;

            return true;
        }
        public async Task<string> AddOrUpdateCronTask(
            [Description("可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结")] string name,
            [Description("可传入具体的任务内容（禁止传入自动任务相关词汇，只能传入任务步骤！！！）。 比如：第一步：搜索并总结AI领域的热门资讯，包括技术突破、产品发布、行业动态等，第二步：生成总结报告为MkD格式")] string content,
            [Description("cron表达式：用于定义任务的执行周期，不可为空 比如用户需要每六分钟执行一次则传入：0 0/6 0/1 * * ?  ")] string cronExpression)
        {
            try
            {
                if (content.Contains(name))
                {
                    return "添加或更新定时任务失败：" + name + content + cronExpression + "，异常信息：cronExpression格式错误";
                }
                //校验cronExpression
                if (IsValidCronExpression(cronExpression) == false)
                {
                    return "添加或更新定时任务失败：" + name + content + cronExpression + "，异常信息：cronExpression格式错误";
                }
                _recurringJobManager.AddOrUpdate<IKevinAITaskService>(
                         recurringJobId: CurrentUser.UserId + name,      // 唯一的 ID，用于后续修改或删除
                         (s) => s.RunTask(CurrentUser.UserId.ToString(), name, content, _aIShareInfoService.GetData()),    // 要执行的任务
                         cronExpression, new RecurringJobOptions
                         {
                             TimeZone = TimeZoneInfo.Local,        // 指定时区（默认UTC）
                         }
                     );
                await UpsertTaskConfigAsync(CurrentUser.UserId, name, "cron", cronExpression, null, content, _aIShareInfoService.GetData());
                InvalidateRecurringIndexCache();
                return "添加或更新定时任务成功：" + name + content + cronExpression;
            }
            catch (Exception ex)
            {

                return "添加或更新定时任务失败：" + name + content + cronExpression + "，异常信息：" + ex.Message;
            }

        }

        /// <summary>
        /// 获取当前用户的未执行一次性任务（Scheduled 状态延迟作业），可按任务名称过滤
        /// RunTask 参数顺序：[0]=userId [1]=taskName [2]=taskContent [3]=taskdata
        /// </summary>
        private List<KeyValuePair<string, Hangfire.Storage.Monitoring.ScheduledJobDto>> GetMyOnceJobs(string userId, string? name = null)
        {
            var storage = _jobStorage ?? JobStorage.Current;
            if (storage == null)
            {
                return new List<KeyValuePair<string, Hangfire.Storage.Monitoring.ScheduledJobDto>>();
            }
            return storage.GetMonitoringApi().ScheduledJobs(0, int.MaxValue)
                .Where(t => t.Value?.Job?.Type == typeof(IKevinAITaskService)
                    && t.Value.Job.Args.FirstOrDefault()?.ToString() == userId
                    && (name == null || (t.Value.Job.Args.Count > 1 && t.Value.Job.Args[1]?.ToString() == name)))
                .ToList();
        }

        /// <summary>
        /// 创建一次性任务：在指定的未来时间点执行一次后自动结束，不会重复执行，也无需移除；同名的未执行一次性任务会被覆盖（等同于更新执行时间）
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="content">任务内容</param>
        /// <param name="executeTime">执行时间点字符串，支持多种格式：yyyy-MM-dd HH:mm、yyyy-MM-dd HH:mm:ss、ISO 8601</param>
        /// <returns></returns>
        public async Task<string> AddOnceTask(
            [Description("可传入具体的任务名称，不可为空 比如：明天上午九点总结AI热门资讯，同名重复添加会覆盖旧任务等同于更新执行时间")] string name,
            [Description("可传入具体的任务内容（禁止传入自动任务相关词汇，只能传入任务步骤！！！）。 比如：第一步：搜索并总结AI领域的热门资讯，包括技术突破、产品发布、行业动态等，第二步：生成总结报告为MkD格式")] string content,
            [Description("执行时间点字符串，不可为空，必须是未来的时间。支持格式：yyyy-MM-dd HH:mm，yyyy-MM-dd HH:mm:ss，ISO 8601。例如：2026-08-27 09:00")] string executeTime)
        {
            try
            {
                // 解析执行时间字符串，支持多种格式
                if (string.IsNullOrWhiteSpace(executeTime))
                {
                    return "添加一次性任务失败：" + name + "，异常信息：执行时间不能为空，请传入未来的时间点，格式如：2026-08-27 09:00";
                }
                string[] formats = { "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ssZ", "o" };
                if (!DateTime.TryParseExact(executeTime.Trim(), formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedTime)
                    && !DateTime.TryParse(executeTime.Trim(), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedTime))
                {
                    return "添加一次性任务失败：" + name + "，异常信息：执行时间格式无法识别，请使用格式如：2026-08-27 09:00 或 2026-08-27 09:00:00";
                }
                parsedTime = DateTime.SpecifyKind(parsedTime, DateTimeKind.Local);
                //校验执行时间必须在未来
                if (parsedTime <= DateTime.Now)
                {
                    return "添加一次性任务失败：" + name + " " + content + " " + parsedTime.ToString("yyyy-MM-dd HH:mm") + "，异常信息：执行时间必须大于当前时间，如果要立即执行请使用 TriggerCronTask";
                }
                // 同名一次性任务视为"更新"：先移除旧的未执行任务再创建，与 AddOrUpdateCronTask 的同名覆盖语义保持一致
                var updateCount = 0;
                foreach (var job in GetMyOnceJobs(CurrentUser.UserId.ToString(), name))
                {
                    if (_backgroundJobClient.Delete(job.Key))
                    {
                        updateCount++;
                    }
                }
                //使用 Hangfire 延迟作业（Scheduled Job）在指定时间点执行一次，执行完后 Hangfire 自动结束该任务，无需重复也无需移除
                _backgroundJobClient.Schedule<IKevinAITaskService>(
                         (s) => s.RunTask(CurrentUser.UserId.ToString(), name, content, _aIShareInfoService.GetData()),    // 要执行的任务
                         parsedTime      // 指定本地时区的执行时间点
                     );
                await UpsertTaskConfigAsync(CurrentUser.UserId, name, "once", "", parsedTime, content, _aIShareInfoService.GetData());
                var updateMsg = updateCount > 0 ? $"，已覆盖同名旧任务 {updateCount} 个（等同更新执行时间）" : "";
                return "添加一次性任务成功：" + name + "，执行时间：" + parsedTime.ToString("yyyy-MM-dd HH:mm") + updateMsg + "，该任务将在指定时间点执行一次后自动结束";
            }
            catch (Exception ex)
            {
                return "添加一次性任务失败：" + name + " " + content + " " + executeTime + "，异常信息：" + ex.Message;
            }
        }

        public Task<List<string>> GetTaskList()
        {
            try
            {
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
                    return $"name:{r.Id.Replace(CurrentUser.UserId.ToString(), "")} | Type:周期性任务 | Cron:{r.Cron} | Next:{next} | Last:{last} | TimeZone:{r.TimeZoneId}";
                }).ToList();
                // 查询一次性任务（Scheduled 状态的延迟作业，执行完后会自动结束不再列表中）
                var onceJobs = GetMyOnceJobs(CurrentUser.UserId.ToString()).Select(t =>
                {
                    var executeAt = t.Value.EnqueueAt.ToLocalTime().ToString("u");
                    var taskName = t.Value.Job.Args.Count > 1 ? t.Value.Job.Args[1]?.ToString() : "";
                    return $"name:{taskName} | Cron:一次性任务 | Next:{executeAt} | JobId:{t.Key}";
                }).ToList();
                result.AddRange(onceJobs);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                return Task.FromResult((new List<string> { "查询任务列表失败：异常信息：" + ex.Message }));
            }


        }
        public async Task<string> RemoveCronTask([Description("name：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结（同时兼容周期性任务和一次性任务，已执行完成的一次性任务会自动结束无需移除）")] string name)
        {
            try
            {
                // 1. 移除同名周期性任务（如存在，不存在也不抛异常）
                _recurringJobManager.RemoveIfExists(CurrentUser.UserId + name);
                // 2. 移除同名未执行的一次性任务（Scheduled 状态延迟作业）
                var onceRemoved = 0;
                foreach (var job in GetMyOnceJobs(CurrentUser.UserId.ToString(), name))
                {
                    if (_backgroundJobClient.Delete(job.Key))
                    {
                        onceRemoved++;
                    }
                }
                var onceMsg = onceRemoved > 0 ? $"，并移除了 {onceRemoved} 个同名未执行的一次性任务" : "";
                // 3. 软删任务配置（TAITaskConfig 为权威注册表，删除即从管理端列表消失且启动对账不再重建）
                var config = await GetTaskConfigAsync(CurrentUser.UserId, name);
                if (config != null)
                {
                    config.IsDelete = true;
                    config.DeleteTime = DateTime.Now;
                    await _aITaskConfigRp.SaveChangesAsync();
                }
                InvalidateRecurringIndexCache();
                return "移除任务成功：" + name + onceMsg + "（注：已执行完成的一次性任务会自动结束，无需移除）";
            }
            catch (Exception ex)
            {
                return "移除定时任务失败：" + name + ",异常信息：" + ex.Message;
            }

        }

        public async Task<string> TriggerCronTask([Description("name：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结（同时兼容周期性任务和一次性任务）")] string name)
        {
            try
            {
                var triggered = new List<string>();
                // 1. 立即触发同名未执行的一次性任务（Scheduled 状态延迟作业，Requeue 后立即执行且不会再在原定时间重复执行）
                foreach (var job in GetMyOnceJobs(CurrentUser.UserId.ToString(), name))
                {
                    if (_backgroundJobClient.Requeue(job.Key))
                    {
                        triggered.Add($"一次性任务(JobId:{job.Key})已立即触发");
                    }
                }
                // 2. 触发同名周期性任务——先确认作业真实存在再 Trigger：
                // RecurringJobManager.Trigger 对不存在的 jobId 是空操作且不抛异常，无条件报成功会让"已执行完的一次性任务"也能返回成功（误导用户）
                var recurringJobId = CurrentUser.UserId + name;
                if (GetRecurringIndexCached().ContainsKey(recurringJobId))
                {
                    _recurringJobManager.Trigger(recurringJobId);
                    triggered.Add("周期性任务已触发");
                }
                if (triggered.Count == 0)
                {
                    // 无任何作业可触发时给出精确原因，避免"假成功"
                    var config = await GetTaskConfigAsync(CurrentUser.UserId, name);
                    if (config != null && config.TaskType == "once" && config.IsPaused)
                    {
                        return "执行任务失败：" + name + "，异常信息：任务处于暂停状态，请先恢复后再执行";
                    }
                    if (config != null && config.TaskType == "once" && config.ExecuteTime != null && config.ExecuteTime <= DateTime.Now)
                    {
                        return "执行任务失败：" + name + "，异常信息：该一次性任务已执行结束（执行一次后自动归档），如需再次执行请重新创建同名任务";
                    }
                    return "执行任务失败：" + name + "，异常信息：未找到该名称的未执行任务，可先通过 GetTaskList 查询任务列表确认任务名称";
                }
                // 3. 手动触发一次性任务成功后：把 ExecuteTime 置为触发时刻（视为已执行归档），
                // 让任务从管理端列表消失，避免"作业已被消耗但任务还挂在列表里"的幽灵状态误导用户反复点击
                var triggeredConfig = await GetTaskConfigAsync(CurrentUser.UserId, name);
                if (triggeredConfig != null && triggeredConfig.TaskType == "once")
                {
                    // 同样用原子 UPDATE：RunTask 收尾写 LastRunTime 与此处归档写 ExecuteTime 互相都是并发写，
                    // 变更跟踪 + 乐观令牌会互相撞出 0 行异常
                    await _aITaskConfigRp.Query(isDataPer: false, isTenant: false)
                        .Where(t => t.Id == triggeredConfig.Id)
                        .ExecuteUpdateAsync(s => s.SetProperty(c => c.ExecuteTime, DateTime.Now));
                }
                return "执行任务成功：" + name + "，" + string.Join("；", triggered);
            }
            catch (Exception ex)
            {
                return "执行定时任务失败：" + name + ",异常信息：" + ex.Message;
            }
        }

        /// <summary>
        /// 管理端：分页查询当前用户的任务（权威源 TAITaskConfig；Hangfire 仅补充周期任务的下次/上次执行时间，O(页大小) 而非 O(全量任务)）
        /// </summary>
        public async Task<dtoAITaskPage> GetMyTaskPage(dtoAITaskPagePar par)
        {
            // 管理端纯 HTTP 请求用 CurrentUser（AIShareInfoService 仅用于 AI 工具调用场景）
            if (CurrentUser == null || CurrentUser.UserId <= 0)
            {
                throw new UserFriendlyException("无法获取当前登录用户，请重新登录后再操作自动任务");
            }
            par ??= new dtoAITaskPagePar();
            if (par.pageSize <= 0) par.pageSize = 20;
            if (par.pageNum <= 0) par.pageNum = 1;
            var now = DateTime.Now;
            var query = _aITaskConfigRp.Query(isDataPer: false, isTenant: false)
                .Where(t => t.IsDelete == false && t.UserId == CurrentUser.UserId);
            // 一次性任务：已到期执行过（未暂停）的不出现在列表；周期性任务与暂停任务始终展示
            query = query.Where(t => t.TaskType == "cron" || t.IsPaused || (t.ExecuteTime != null && t.ExecuteTime > now));
            if (!string.IsNullOrWhiteSpace(par.searchKey))
            {
                query = query.Where(t => t.Name.Contains(par.searchKey.Trim()));
            }
            if (string.Equals(par.state, "paused", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(t => t.IsPaused);
            }
            else if (string.Equals(par.state, "running", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(t => t.IsPaused == false);
            }
            var result = new dtoAITaskPage
            {
                total = await query.CountAsync(),
                list = new List<dtoAITaskItem>()
            };
            var pageData = await query
                .OrderByDescending(t => t.CreateTime)
                .Skip((par.pageNum - 1) * par.pageSize)
                .Take(par.pageSize)
                .ToListAsync();
            // 仅对当页周期性任务补充调度信息；暂停任务 Hangfire 侧无作业，显示空。
            // GetRecurringJobs 是 O(全量任务) 读取（内存存储 3000 任务实测 ~0.9s），加 30 秒进程级缓存避免列表页反复全量读取
            Dictionary<string, RecurringJobDto> recurringIndex;
            try
            {
                recurringIndex = GetRecurringIndexCached();
            }
            catch (Exception ex)
            {
                LogHelper.logger.Warn("任务列表读取 Hangfire 调度信息失败（仅展示落库信息）：" + ex.Message);
                recurringIndex = new Dictionary<string, RecurringJobDto>();
            }
            foreach (var config in pageData)
            {
                var item = new dtoAITaskItem
                {
                    name = config.Name,
                    taskType = config.TaskType,
                    cron = config.TaskType == "cron" ? config.CronExpression : "一次性任务",
                    content = config.TaskContent,
                    isPaused = config.IsPaused,
                    last = FormatDisplayTime(config.LastRunTime, isUtcSource: false),
                    timeZone = TimeZoneInfo.Local.Id
                };
                if (config.TaskType == "once")
                {
                    item.next = FormatDisplayTime(config.ExecuteTime, isUtcSource: false);
                }
                else if (!config.IsPaused && recurringIndex != null && recurringIndex.TryGetValue(CurrentUser.UserId + config.Name, out var job))
                {
                    // Hangfire dto 的 NextExecution/LastExecution 是真正的 UTC（Kind=Utc），需要 ToLocalTime 换算
                    item.next = FormatDisplayTime(job.NextExecution, isUtcSource: true);
                    if (job.LastExecution != null)
                    {
                        item.last = FormatDisplayTime(job.LastExecution, isUtcSource: true);
                    }
                }
                result.list.Add(item);
            }
            return result;
        }

        /// <summary>
        /// 时间展示格式化（本地时区，不带误导性的 Z 后缀）
        /// </summary>
        /// <param name="value">时间值</param>
        /// <param name="isUtcSource">
        /// true：值本身是真 UTC（Hangfire 调度信息），用 ToLocalTime 换算成当地时间；
        /// false：值来自数据库本地时间。注意 Oracle MySql.EntityFrameworkCore 会把 datetime 物化成 Kind=Utc，
        /// 但库里存的本来就是本地时间——绝不能再 ToLocalTime（会被加时区偏移，08:30 显示成 16:30），只改 Kind 标签
        /// </param>
        private static string FormatDisplayTime(DateTime? value, bool isUtcSource)
        {
            if (value == null)
            {
                return "";
            }
            var dt = value.Value;
            if (isUtcSource)
            {
                dt = dt.Kind == DateTimeKind.Utc || dt.Kind == DateTimeKind.Unspecified
                    ? dt.ToLocalTime()
                    : dt;
            }
            else if (dt.Kind == DateTimeKind.Utc)
            {
                // EF 物化标签错位（存的是本地时间却标成 Utc），仅纠正标签
                dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
            }
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 管理端：暂停任务——周期性任务 RemoveIfExists、一次性任务删除未执行 Scheduled 作业，Hangfire 侧不再触发；
        /// 配置与元数据保留在 TAITaskConfig（IsPaused=true），恢复时按落库元数据重新注册。内存/Redis 双存储语义一致
        /// </summary>
        public async Task<string> PauseTask(string name)
        {
            if (CurrentUser == null || CurrentUser.UserId <= 0)
            {
                throw new UserFriendlyException("无法获取当前登录用户，请重新登录后再操作自动任务");
            }
            var config = await GetTaskConfigAsync(CurrentUser.UserId, name);
            if (config == null)
            {
                return "暂停任务失败：" + name + "，异常信息：未找到该名称的任务，可先查询任务列表确认任务名称";
            }
            if (config.IsPaused)
            {
                return "任务已是暂停状态：" + name;
            }
            config.IsPaused = true;
            await _aITaskConfigRp.SaveChangesAsync();
            if (config.TaskType == "cron")
            {
                _recurringJobManager.RemoveIfExists(CurrentUser.UserId + name);
                InvalidateRecurringIndexCache();
                return "暂停任务成功：" + name + "，任务已停止执行，可在任务列表中恢复";
            }
            var onceIndex = IndexOnceJobs();
            var deleted = 0;
            foreach (var jobId in GetOnceJobIds(CurrentUser.UserId, name, onceIndex))
            {
                if (_backgroundJobClient.Delete(jobId))
                {
                    deleted++;
                }
            }
            return $"暂停任务成功：{name}，已移除 {deleted} 个未执行作业，恢复时若执行时间已过将不再执行";
        }

        /// <summary>
        /// 管理端：恢复任务——按 TAITaskConfig 落库元数据重新注册到 Hangfire（周期性 AddOrUpdate；一次性若执行时间在未来且无同名未执行作业则补排）。
        /// 双存储兼容：内存存储重启丢失的作业也走同一路径重建
        /// </summary>
        public async Task<string> ResumeTask(string name)
        {
            if (CurrentUser == null || CurrentUser.UserId <= 0)
            {
                throw new UserFriendlyException("无法获取当前登录用户，请重新登录后再操作自动任务");
            }
            var config = await GetTaskConfigAsync(CurrentUser.UserId, name);
            if (config == null)
            {
                return "恢复任务失败：" + name + "，异常信息：未找到该名称的任务";
            }
            if (!config.IsPaused)
            {
                return "任务不是暂停状态，无需恢复：" + name;
            }
            config.IsPaused = false;
            await _aITaskConfigRp.SaveChangesAsync();
            if (config.TaskType == "cron")
            {
                RegisterCronJobFromConfig(config);
                InvalidateRecurringIndexCache();
                return "恢复任务成功：" + name + "，已按 cron 表达式重新注册，下次执行时间可在任务列表查看";
            }
            var onceIndex = IndexOnceJobs();
            if (ScheduleOnceJobFromConfig(config, onceIndex))
            {
                return "恢复任务成功：" + name + "，执行时间：" + config.ExecuteTime?.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
            }
            return "恢复任务完成：" + name + "，但原执行时间已过（一次性任务不会重复执行），如需再次执行请重新创建任务或使用立即执行";
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="taskName">任务名称</param>
        /// <param name="taskContent">任务内容</param>
        /// <returns></returns>
        public async Task<string> RunTask(string userId, string taskName, string taskContent, AIShareInfoDto taskdata)
        {
            var lock1 = distLock.TryAcquireLock("kevin.Application.Services.AI.RunTask:" + userId + taskName);
            if (lock1 == null)
            {
                return "执行定时任务失败：" + userId + taskName + ",异常信息：任务正在执行中，请勿重复执行";
            }
            using (lock1)
            {
                try
                {
                    // 暂停守卫：暂停瞬间的在途作业直接跳过（正常情况下暂停时 Hangfire 侧作业已移除，此为兜底）
                    var taskConfig = await GetTaskConfigAsync(userId.ToTryInt64(), taskName);
                    if (taskConfig != null && taskConfig.IsPaused)
                    {
                        return "执行定时任务跳过：" + userId + taskName + "，任务处于暂停状态";
                    }
                    _aIShareInfoService.InitData(taskdata);
                    //这里可以根据任务名称和内容执行具体的业务逻辑，比如调用AI接口、处理数据等。当前示例仅打印日志并返回结果。 自行处理 
                    var messageContent = $"AI:{userId}RunTask：执行任务" + taskName + taskContent + taskdata.ToJson();
                    if (JsonHelper.GetValueByKey(taskdata.ToJson(), "ai_chats_id").ToTryInt64() != default)
                    {
                        var aichat = aIChatsRp.FirstOrDefault(t => t.Id == JsonHelper.GetValueByKey(taskdata.ToJson(), "ai_chats_id").ToTryInt64(), isDataPer: false, isTenant: false);
                        var _aIAppsService = _serviceProvider?.GetService<IAIAppsService>();
                        if (_aIAppsService != null)
                        {
                            var aiapp = await _aIAppsService.GetNoPerDetails(aichat.AppId);
                            // Auto模式解析：与聊天链路（AIChatHistorysService）同语义——随机选主模型，其余全部进备选池。
                            // 失败（限流429/网络/5xx）时由 AIAgentService 自动换未用过的备选模型整轮重试，避免任务卡死在被打满的模型上；参数类错误不浪费备选
                            List<AIFallbackModel> taskFallbackModels = new();
                            if (string.Equals(aiapp.ChatModelID, "auto", StringComparison.OrdinalIgnoreCase))
                            {
                                var _aIModelsService = _serviceProvider?.GetService<IAIModelsService>();
                                var allModels = _aIModelsService == null ? new System.Collections.Generic.List<AIModelsDto>() : await _aIModelsService.GetNoPerALLList(1);
                                if (allModels.Count == 0)
                                    throw new UserFriendlyException("当前没有可用的聊天模型，请联系管理员配置模型。");
                                var mainIndex = Random.Shared.Next(allModels.Count);
                                aiapp.ChatModelID = allModels[mainIndex].Id.ToString();
                                taskFallbackModels = allModels
                                    .Where((m, i) => i != mainIndex)
                                    .Select(m => new AIFallbackModel
                                    {
                                        AIUrl = m.EndPoint,
                                        AIKeySecret = m.ModelKey,
                                        AIDefaultModel = m.ModelName,
                                        MaxAskPromptSize = m.MaxAskPromptSize,
                                        AnswerTokens = m.AnswerTokens
                                    }).ToList();
                            }
                            var aIModels = _aIModelsRp.FirstOrDefault(t => t.Id == aiapp.ChatModelID.ToTryInt64(), isDataPer: false, isTenant: false);
                            var aIPrompts = _aIPromptsRp.FirstOrDefault(t => t.Id == aiapp.AIPromptID, isDataPer: false, isTenant: false).MapTo<AIPromptsDto>();
                            string systemPrompt = SystemPrompt.SystemPromptText + "\n 智能体提示词规则：\n" + aIPrompts.Prompt;
                            var chatAgOs = await _aIAppsService.GetAppAIAgentOptions(aiapp, aIPrompts, systemPrompt, new AIChatHistorysDto
                            {
                                AIChatsId = SnowflakeIdService.GetNextId(),
                                Id = SnowflakeIdService.GetNextId(),
                                CreateTime = DateTime.Now
                            });
                            switch (aIModels.AIType)
                            {
                                case Domain.Share.Enums.AIType.OpenAI:
                                case Domain.Share.Enums.AIType.ZhiPuAI:
                                case Domain.Share.Enums.AIType.AzureOpenAI:
                                default:
                                    messageContent = (await _aIAgentService.CreateOpenAIAgentAndSendMSG(new AISetting
                                    {
                                        AIUrl = aIModels.EndPoint,
                                        AIKeySecret = aIModels.ModelKey,
                                        AIDefaultModel = aIModels.ModelName,
                                        IsStreame = false,
                                        IsHttpLog = aiapp.IsHttpLog,
                                        MaxRetries = aiapp.MaxRetries,
                                        IsAISkills = aiapp.IsSkill,
                                        IsAITools = aiapp.IsAITools,
                                        IsMcpTools = aiapp.IsMcp,
                                        IsMemory = aiapp.IsMemory,
                                        IsKnowledgeBase = aiapp.KmsId.HasValue && aiapp.KmsId.Value > 0,
                                        NetworkTimeout = aiapp.NetworkTimeout,
                                        FallbackModels = taskFallbackModels,
                                    }, chatAgOs, new ChatMessage(ChatRole.User, [new TextContent($"{taskContent} \n 必须根据相关技能，一次性完成所有步骤在返回结果")]))).Item2;
                                    break;
                            }
                        }
                    }
                   await _messageService.AddAIMessage(new Domain.Share.Dtos.Msg.MessageDto
                    {
                        SendUserId = userId,
                        Title = "AI:RunTask：执行任务" + taskName,
                        MessageContent = messageContent,
                        SysMsgType = MessageType.AI,
                        RecipientUserId = userId
                    });
                    if (taskConfig != null)
                    {
                        // 收尾记账不走变更跟踪：执行期间该行可能被并发修改（手动触发归档/暂停恢复等），
                        // 乐观并发令牌必然过期导致 0 行异常；改为原子 UPDATE 只刷 LastRunTime，行被删时静默跳过
                        await _aITaskConfigRp.Query(isDataPer: false, isTenant: false)
                            .Where(t => t.Id == taskConfig.Id)
                            .ExecuteUpdateAsync(s => s.SetProperty(c => c.LastRunTime, DateTime.Now));
                    }
                    return $"执行任务：{taskName}，内容：{taskContent} 返回结果：{messageContent} 执行结果：成功";
                }
                catch (Exception ex)
                {
                    LogHelper.logger.Error("执行定时任务失败：" + userId + taskName + ",异常信息：" + ex.Message, ex);
                    return "执行定时任务失败：" + userId + taskName + ",异常信息：" + ex.Message;
                }
            }
        }

    }
}
