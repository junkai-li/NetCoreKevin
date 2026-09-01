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

namespace kevin.Application.Services.AI
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
        private IDistributedLockProvider distLock { get; set; } 

        public KevinAITasksService(IHttpContextAccessor _httpContextAccessor, IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient, JobStorage jobStorage, IMessageService messageService,
            IAIAgentService aIAgentService, IAIModelsRp aIModelsRp, IAIPromptsRp aIPromptsRp, IAIChatsRp aIChatsRp, IServiceProvider serviceProvider,
            IDistributedLockProvider distLock, IAIShareInfoService aIShareInfoService) : base(_httpContextAccessor)
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
        public Task<string> AddOrUpdateCronTask(
            [Description("可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结")] string name,
            [Description("可传入具体的任务内容（禁止传入自动任务相关词汇，只能传入任务步骤！！！）。 比如：第一步：搜索并总结AI领域的热门资讯，包括技术突破、产品发布、行业动态等，第二步：生成总结报告为MkD格式")] string content,
            [Description("cron表达式：用于定义任务的执行周期，不可为空 比如用户需要每六分钟执行一次则传入：0 0/6 0/1 * * ?  ")] string cronExpression)
        {
            try
            {
                if (content.Contains(name))
                {
                    return Task.FromResult("添加或更新定时任务失败：" + name + content + cronExpression + "，异常信息：cronExpression格式错误");
                }
                //校验cronExpression
                if (IsValidCronExpression(cronExpression) == false)
                {
                    return Task.FromResult("添加或更新定时任务失败：" + name + content + cronExpression + "，异常信息：cronExpression格式错误");
                }
                _recurringJobManager.AddOrUpdate<IKevinAITaskService>(
                         recurringJobId: CurrentUser.UserId + name,      // 唯一的 ID，用于后续修改或删除
                         (s) => s.RunTask(CurrentUser.UserId.ToString(), name, content, _aIShareInfoService.GetData()),    // 要执行的任务
                         cronExpression, new RecurringJobOptions
                         {
                             TimeZone = TimeZoneInfo.Local,        // 指定时区（默认UTC） 
                         }
                     );
                return Task.FromResult("添加或更新定时任务成功：" + name + content + cronExpression);
            }
            catch (Exception ex)
            {

                return Task.FromResult("添加或更新定时任务失败：" + name + content + cronExpression + "，异常信息：" + ex.Message);
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
        public Task<string> AddOnceTask(
            [Description("可传入具体的任务名称，不可为空 比如：明天上午九点总结AI热门资讯，同名重复添加会覆盖旧任务等同于更新执行时间")] string name,
            [Description("可传入具体的任务内容（禁止传入自动任务相关词汇，只能传入任务步骤！！！）。 比如：第一步：搜索并总结AI领域的热门资讯，包括技术突破、产品发布、行业动态等，第二步：生成总结报告为MkD格式")] string content,
            [Description("执行时间点字符串，不可为空，必须是未来的时间。支持格式：yyyy-MM-dd HH:mm，yyyy-MM-dd HH:mm:ss，ISO 8601。例如：2026-08-27 09:00")] string executeTime)
        {
            try
            {
                // 解析执行时间字符串，支持多种格式
                if (string.IsNullOrWhiteSpace(executeTime))
                {
                    return Task.FromResult("添加一次性任务失败：" + name + "，异常信息：执行时间不能为空，请传入未来的时间点，格式如：2026-08-27 09:00");
                }
                string[] formats = { "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ssZ", "o" };
                if (!DateTime.TryParseExact(executeTime.Trim(), formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedTime)
                    && !DateTime.TryParse(executeTime.Trim(), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedTime))
                {
                    return Task.FromResult("添加一次性任务失败：" + name + "，异常信息：执行时间格式无法识别，请使用格式如：2026-08-27 09:00 或 2026-08-27 09:00:00");
                }
                parsedTime = DateTime.SpecifyKind(parsedTime, DateTimeKind.Local);
                //校验执行时间必须在未来
                if (parsedTime <= DateTime.Now)
                {
                    return Task.FromResult("添加一次性任务失败：" + name + " " + content + " " + parsedTime.ToString("yyyy-MM-dd HH:mm") + "，异常信息：执行时间必须大于当前时间，如果要立即执行请使用 TriggerCronTask");
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
                var updateMsg = updateCount > 0 ? $"，已覆盖同名旧任务 {updateCount} 个（等同更新执行时间）" : "";
                return Task.FromResult("添加一次性任务成功：" + name + "，执行时间：" + parsedTime.ToString("yyyy-MM-dd HH:mm") + updateMsg + "，该任务将在指定时间点执行一次后自动结束");
            }
            catch (Exception ex)
            {
                return Task.FromResult("添加一次性任务失败：" + name + " " + content + " " + executeTime + "，异常信息：" + ex.Message);
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
        public Task<string> RemoveCronTask([Description("name：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结（同时兼容周期性任务和一次性任务，已执行完成的一次性任务会自动结束无需移除）")] string name)
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
                return Task.FromResult("移除任务成功：" + name + onceMsg + "（注：已执行完成的一次性任务会自动结束，无需移除）");
            }
            catch (Exception ex)
            {
                return Task.FromResult("移除定时任务失败：" + name + ",异常信息：" + ex.Message);
            }

        }

        public Task<string> TriggerCronTask([Description("name：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结（同时兼容周期性任务和一次性任务）")] string name)
        {
            try
            {
                var triggered = new List<string>();
                // 1. 立即触发同名未执行的一次性任务（Scheduled 状态延迟作业，触发后立即执行且不会再在原定时间重复执行）
                foreach (var job in GetMyOnceJobs(CurrentUser.UserId.ToString(), name))
                {
                    if (_backgroundJobClient.Requeue(job.Key))
                    {
                        triggered.Add($"一次性任务(JobId:{job.Key})已立即触发");
                    }
                }
                // 2. 触发同名周期性任务（如存在）
                try
                {
                    _recurringJobManager.Trigger(CurrentUser.UserId + name);
                    triggered.Add("周期性任务已触发");
                }
                catch
                {
                    // 同名周期性任务不存在时忽略（只要一次性任务触发成功即可），两者都不存在时下面统一报错
                }
                if (triggered.Count == 0)
                {
                    return Task.FromResult("执行任务失败：" + name + "，异常信息：未找到该名称的任务，可先通过 GetTaskList 查询任务列表确认任务名称");
                }
                return Task.FromResult("执行任务成功：" + name + "，" + string.Join("；", triggered));
            }
            catch (Exception ex)
            {
                return Task.FromResult("执行定时任务失败：" + name + ",异常信息：" + ex.Message);
            }
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="taskName">任务名称</param>
        /// <param name="taskContent">任务内容</param>
        /// <returns></returns>
        public Task<string> RunTask(string userId, string taskName, string taskContent, AIShareInfoDto taskdata)
        {
            var lock1 = distLock.TryAcquireLock("kevin.Application.Services.AI.RunTask:" + userId + taskName);
            if (lock1 == null)
            {
                return Task.FromResult("执行定时任务失败：" + userId + taskName + ",异常信息：任务正在执行中，请勿重复执行");
            }
            using (lock1)
            {
                try
                {
                    _aIShareInfoService.InitData(taskdata);
                    //这里可以根据任务名称和内容执行具体的业务逻辑，比如调用AI接口、处理数据等。当前示例仅打印日志并返回结果。 自行处理 
                    var messageContent = $"AI:{userId}RunTask：执行任务" + taskName + taskContent + taskdata.ToJson();
                    if (JsonHelper.GetValueByKey(taskdata.ToJson(), "ai_chats_id").ToTryInt64() != default)
                    {
                        var aichat = aIChatsRp.FirstOrDefault(t => t.Id == JsonHelper.GetValueByKey(taskdata.ToJson(), "ai_chats_id").ToTryInt64(), isDataPer: false, isTenant: false);
                        var _aIAppsService = _serviceProvider?.GetService<IAIAppsService>();
                        if (_aIAppsService != null)
                        {
                            var aiapp = _aIAppsService.GetNoPerDetails(aichat.AppId).Result;
                            // Auto模式解析：随机选择一个可用模型
                            if (string.Equals(aiapp.ChatModelID, "auto", StringComparison.OrdinalIgnoreCase))
                            {
                                var _aIModelsService = _serviceProvider?.GetService<IAIModelsService>();
                                var allModels = _aIModelsService?.GetNoPerALLList(1).Result ?? new System.Collections.Generic.List<AIModelsDto>();
                                if (allModels.Count == 0)
                                    throw new UserFriendlyException("当前没有可用的聊天模型，请联系管理员配置模型。");
                                aiapp.ChatModelID = allModels[new Random().Next(allModels.Count)].Id.ToString();
                            }
                            var aIModels = _aIModelsRp.FirstOrDefault(t => t.Id == aiapp.ChatModelID.ToTryInt64(), isDataPer: false, isTenant: false);
                            var aIPrompts = _aIPromptsRp.FirstOrDefault(t => t.Id == aiapp.AIPromptID, isDataPer: false, isTenant: false).MapTo<AIPromptsDto>();
                            string systemPrompt = SystemPrompt.SystemPromptText + "\n 智能体提示词规则：\n" + aIPrompts.Prompt;
                            var chatAgOs = _aIAppsService.GetAppAIAgentOptions(aiapp, aIPrompts, systemPrompt, new Domain.Share.Dtos.AI.AIChatHistorysDto
                            {
                                AIChatsId = SnowflakeIdService.GetNextId(),
                                Id = SnowflakeIdService.GetNextId(),
                                CreateTime = DateTime.Now
                            }).Result;
                            switch (aIModels.AIType)
                            {
                                case Domain.Share.Enums.AIType.OpenAI:
                                case Domain.Share.Enums.AIType.ZhiPuAI:
                                case Domain.Share.Enums.AIType.AzureOpenAI:
                                default:
                                    messageContent = _aIAgentService.CreateOpenAIAgentAndSendMSG(new AISetting
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
                                        NetworkTimeout = aiapp.NetworkTimeout,
                                    }, chatAgOs, new(ChatRole.User, [new TextContent($"{taskContent} \n 必须根据相关技能，一次性完成所有步骤在返回结果")])).Result.Item2;
                                    break;
                            }
                        }
                    }
                    _messageService.AddAIMessage(new Domain.Share.Dtos.Msg.MessageDto
                    {
                        SendUserId = userId,
                        Title = "AI:RunTask：执行任务" + taskName,
                        MessageContent = messageContent,
                        SysMsgType = MessageType.AI,
                        RecipientUserId = userId
                    });
                    return Task.FromResult($"执行任务：{taskName}，内容：{taskContent} 返回结果：{messageContent} 执行结果：成功");
                }
                catch (Exception ex)
                {
                    LogHelper.logger.Error("执行定时任务失败：" + userId + taskName + ",异常信息：" + ex.Message, ex);
                    return Task.FromResult("执行定时任务失败：" + userId + taskName + ",异常信息：" + ex.Message);
                }
            }
        }

    }
}
