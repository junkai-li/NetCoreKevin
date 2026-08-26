using Common.Json;
using DocumentFormat.OpenXml.Wordprocessing;
using Hangfire;
using Hangfire.Storage;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tasks;
using kevin.AI.AgentFramework.ScriptRunners;
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
        private IDistributedLockProvider distLock { get; set; }

        private object? _data; // 用于存储初始化数据

        public void InitData(object data)
        {
            _data = data;
        }

        public KevinAITasksService(IHttpContextAccessor _httpContextAccessor, IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient, JobStorage jobStorage, IMessageService messageService,
            IAIAgentService aIAgentService, IAIModelsRp aIModelsRp, IAIPromptsRp aIPromptsRp, IAIChatsRp aIChatsRp, IServiceProvider serviceProvider,
            IDistributedLockProvider distLock) : base(_httpContextAccessor)
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
                         (s) => s.RunTask(CurrentUser.UserId.ToString(), name, content, _data),    // 要执行的任务
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
        /// 创建一次性任务：在指定的未来时间点执行一次后自动结束，不会重复执行，也无需移除
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="content">任务内容</param>
        /// <param name="executeTime">执行时间点，必须是未来时间</param>
        /// <returns></returns>
        public Task<string> AddOnceTask(
            [Description("可传入具体的任务名称，不可为空 比如：明天上午九点总结AI热门资讯")] string name,
            [Description("可传入具体的任务内容（禁止传入自动任务相关词汇，只能传入任务步骤！！！）。 比如：第一步：搜索并总结AI领域的热门资讯，包括技术突破、产品发布、行业动态等，第二步：生成总结报告为MkD格式")] string content,
            [Description("执行时间点，不可为空，必须是未来的时间，格式：yyyy-MM-dd HH:mm 比如：2026-08-27 09:00 表示2026年8月27日上午9点执行一次")] DateTime executeTime)
        {
            try
            {
                //校验执行时间必须在未来
                if (executeTime <= DateTime.Now)
                {
                    return Task.FromResult("添加一次性任务失败：" + name + content + executeTime.ToString("yyyy-MM-dd HH:mm") + "，异常信息：执行时间必须大于当前时间，如果要立即执行请使用 TriggerCronTask");
                }
                //使用 Hangfire 延迟作业（Scheduled Job）在指定时间点执行一次，执行完后 Hangfire 自动结束该任务，无需重复也无需移除
                _backgroundJobClient.Schedule<IKevinAITaskService>(
                         (s) => s.RunTask(CurrentUser.UserId.ToString(), name, content, _data),    // 要执行的任务
                         DateTime.SpecifyKind(executeTime, DateTimeKind.Local)      // 指定本地时区的执行时间点
                     );
                return Task.FromResult("添加一次性任务成功：" + name + "，执行时间：" + executeTime.ToString("yyyy-MM-dd HH:mm") + "，该任务将在指定时间点执行一次后自动结束");
            }
            catch (Exception ex)
            {
                return Task.FromResult("添加一次性任务失败：" + name + content + executeTime.ToString("yyyy-MM-dd HH:mm") + "，异常信息：" + ex.Message);
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
                var monitoring = storage.GetMonitoringApi();
                var onceJobs = monitoring.ScheduledJobs(0, int.MaxValue)
                    .Where(t => t.Value?.Job?.Type == typeof(IKevinAITaskService)
                        && t.Value.Job.Args.FirstOrDefault()?.ToString() == CurrentUser.UserId.ToString())
                    .Select(t =>
                    {
                        var executeAt = t.Value.EnqueueAt.ToLocalTime().ToString("u");
                        // RunTask 参数顺序：[0]=userId [1]=taskName [2]=taskContent [3]=taskdata
                        var taskName = t.Value.Job.Args.Count > 1 ? t.Value.Job.Args[1]?.ToString() : "";
                        return $"name:{taskName} | Type:一次性任务 | ExecuteAt:{executeAt} | JobId:{t.Key}";
                    }).ToList();
                result.AddRange(onceJobs);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                return Task.FromResult((new List<string> { "查询任务列表失败：异常信息：" + ex.Message }));
            }


        }
        public Task<string> RemoveCronTask([Description("name：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结")] string name)
        {
            try
            {
                _recurringJobManager.RemoveIfExists(CurrentUser.UserId + name);
                return Task.FromResult("移除定时任务成功：" + name);
            }
            catch (Exception ex)
            {
                return Task.FromResult("移除定时任务失败：" + name + ",异常信息：" + ex.Message);
            }

        }

        public Task<string> TriggerCronTask([Description("name：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结")] string name)
        {
            try
            {
                _recurringJobManager.Trigger(CurrentUser.UserId + name);
                return Task.FromResult("执行定时任务成功：" + name);
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
        public Task<string> RunTask(string userId, string taskName, string taskContent, object taskdata)
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
                            }, taskdata).Result;
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
                                        IsMcpTools= aiapp.IsMcp,
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
