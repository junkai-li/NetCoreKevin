using Common.Json;
using Consul;
using Cronos;
using DocumentFormat.OpenXml.Wordprocessing;
using Hangfire;
using Hangfire.Storage;
using HarmonyLib;
using kevin.AI.AgentFramework;
using kevin.AI.AgentFramework.Agent.KevinChatMessageStore;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tasks;
using kevin.AI.AgentFramework.ScriptRunners;
using kevin.AI.AgentFramework.SkillClass;
using kevin.AI.AgentFramework.Tools;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Enums;
using kevin.RepositorieRps.Repositories.AI;
using Kevin.AI.Dto;
using Kevin.Common.Extension;
using Kevin.log4Net;
using Kevin.RAG.Ollama;
using Kevin.SignalR.Service;
using Medallion.Threading;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Text.RegularExpressions;
using static Qdrant.Client.Grpc.BinaryQuantizationQueryEncoding.Types;

namespace kevin.Application.Services.AI
{
    public class KevinAITasksService : BaseService, IKevinAITaskService
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly JobStorage _jobStorage;

        private readonly IMessageService _messageService;

        private readonly IAIAgentService _aIAgentService;

        private readonly IAIAppsRp _aIAppsRp;
        private readonly IAIChatsRp aIChatsRp;
        private readonly IAIModelsRp _aIModelsRp;
        private readonly IAIPromptsRp _aIPromptsRp;

        private IDistributedLockProvider distLock { get; set; }

        private object _data; // 用于存储初始化数据

        public void InitData(object data)
        {
            _data = data;
        }

        public KevinAITasksService(IHttpContextAccessor _httpContextAccessor, IRecurringJobManager recurringJobManager, JobStorage jobStorage, IMessageService messageService,
            IAIAgentService aIAgentService, IAIAppsRp aIAppsRp, IAIModelsRp aIModelsRp, IAIPromptsRp aIPromptsRp, IAIChatsRp aIChatsRp, IServiceProvider serviceProvider,
            IDistributedLockProvider distLock) : base(_httpContextAccessor)
        {
            _serviceProvider = serviceProvider;
            _recurringJobManager = recurringJobManager;
            _jobStorage = jobStorage; // 可通过 DI 注入；若为 null，会回退到 JobStorage.Current
            _messageService = messageService;
            _aIAgentService = aIAgentService;
            this._aIAppsRp = aIAppsRp;
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
        public Task<string> AddOrUpdateCronTask([Description("name：可传入具体的任务名称，不可为空 比如：每六分钟AI热门资讯总结")] string name, [Description("content：可传入具体的任务内容，不可为空 比如：第一步：搜索并总结AI领域的热门资讯，包括技术突破、产品发布、行业动态等，第二步：生成总结报告为MkD格式")] string content, [Description("cron表达式：用于定义任务的执行周期，不可为空 比如用户需要每六分钟执行一次则传入：0 0/6 0/1 * * ?  ")] string cronExpression)
        {
            try
            {
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
                    return $"name:{r.Id.Replace(CurrentUser.UserId.ToString(), "")} | Cron:{r.Cron} | Next:{next} | Last:{last} | TimeZone:{r.TimeZoneId}";
                }).ToList();
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
            var lock1 = distLock.TryAcquireLock("kevin.Application.Services.AI.RunTask:" + taskName);
            if (lock1 == null)
            {
                return Task.FromResult("执行定时任务失败：" + taskName + ",异常信息：任务正在执行中，请勿重复执行");
            }
            using (lock1)
            {
                try
                {
                    //这里可以根据任务名称和内容执行具体的业务逻辑，比如调用AI接口、处理数据等。当前示例仅打印日志并返回结果。 自行处理 
                    var messageContent = $"AI:{userId}RunTask：执行任务" + taskName + taskContent + taskdata.ToJson();
                    Console.WriteLine(messageContent);
                    if (JsonHelper.GetValueByKey(taskdata.ToJson(), "ai_chats_id").ToTryInt64() != default)
                    {
                        var aichat = aIChatsRp.FirstOrDefault(t => t.Id == JsonHelper.GetValueByKey(taskdata.ToJson(), "ai_chats_id").ToTryInt64(), isDataPer: false, isTenant: false);
                        var aiapp = _aIAppsRp.FirstOrDefault(t => t.Id == aichat.AppId, isDataPer: false, isTenant: false);
                        var aIModels = _aIModelsRp.FirstOrDefault(t => t.Id == aiapp.ChatModelID.ToTryInt64(), isDataPer: false, isTenant: false);
                        var aIPrompts = _aIPromptsRp.FirstOrDefault(t => t.Id == aiapp.AIPromptID, isDataPer: false, isTenant: false);
                        string systemPrompt = SystemPrompt.SystemPromptText;
                        var chatAgOs = new ChatClientAgentOptions
                        {
                            Name = aiapp.Name,
                            Description = aIPrompts.Description ?? "你是一个智能体,请根据你的问题进行相关回答",
                            ChatOptions = new Microsoft.Extensions.AI.ChatOptions
                            {
                                MaxOutputTokens = aiapp.MaxAskPromptSize,
                                Temperature = (float)(aiapp.Temperature / 100),
                                ResponseFormat = ChatResponseFormat.Text,
                                Instructions = (aIPrompts.Prompt + systemPrompt),
                            },
                        };
                        var _aIAgentToolSkillService = _serviceProvider.GetService<IAIAgentToolSkillService>();
                        if (aiapp.IsAITools && _aIAgentToolSkillService != default)
                        {
                            if (chatAgOs.ChatOptions != default)
                            {
                                // 🔑 能力层：工具
                                chatAgOs.ChatOptions.Tools ??= new List<AITool>();
                                chatAgOs.ChatOptions.Tools.AddRange(_aIAgentToolSkillService.GetUserAIAgentToolsAsync(taskdata, aiapp.Id.ToString(), userId).Result);
                            }
                        }
                        if (aiapp.IsSkill)
                        {
                            var skillPaths = _aIAgentToolSkillService.GetUserAIAgentSkillsAsync(taskdata, aiapp.Id.ToString(), userId).Result;
#pragma warning disable MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。 

                            var aiContextProviders = new List<AIContextProvider>();
                            foreach (var skillPath in skillPaths)
                            {
                                var skillsProvider = new AgentSkillsProviderBuilder()
                                                                               .UseFileScriptRunner(PySubprocessScriptRunner.StaticRunAsync)
                                                                              .UseFileSkill(Path.Combine(AppContext.BaseDirectory + skillPath),//"/Skills/all-skills"
                                                                              new AgentFileSkillsSourceOptions
                                                                              {
                                                                                  AllowedScriptExtensions = [".py", ".sh", ".ps1", ".sh"],
                                                                                  ScriptDirectories = ["scripts", "tools", "templates"],
                                                                              })
                                                                              .UseOptions(options => options.DisableCaching = true)
                                                                              .Build();
                                aiContextProviders.Add(skillsProvider);
                            }
                            chatAgOs.AIContextProviders = aiContextProviders;
                        }
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
                                    NetworkTimeout = aiapp.NetworkTimeout,
                                }, chatAgOs, taskContent).Result.Item2;
                                break;
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
                    LogHelper.logger.Error("执行定时任务失败：" + taskName + ",异常信息：" + ex.Message, ex);
                    return Task.FromResult("执行定时任务失败：" + taskName + ",异常信息：" + ex.Message);
                }
            }
        }

    }
}
