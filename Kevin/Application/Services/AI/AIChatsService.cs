using Common;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using Kevin.AI.Dto;
using Kevin.log4Net;
using Kevin.SignalR.Service;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Repository.Database;
using TencentCloud.Lowcode.V20210108.Models;

namespace kevin.Application.Services.AI
{
    public class AIChatsService : BaseService, IAIChatsService
    {
        public IAIChatsRp aIChatsRp { get; set; }

        public IAIAppsService aIAppsService { get; set; }
        public IAIChatHistorysRp aIChatHistorysRp { get; set; }
        public IAIAgentService aIAgentService { get; set; }
        /// public IAIClient aIClient { get; set; }
        public IAIModelsService aIModelsService { get; set; }

        public IAIPromptsService aIPromptsService { get; set; }
        public ISignalRMsgService signalRMsgService { get; set; }
        public IKevinAIChatMessageStore kevinAIChatMessageStore { get; set; }

        public AIChatsService(IHttpContextAccessor _httpContextAccessor, IAIChatsRp _aIChatsRp,
            IAIAgentService _aIAgentService,
            IAIChatHistorysRp _aIChatHistorysRp, IAIAppsService _aIAppsService,
            IAIModelsService _aIModelsService, IAIPromptsService aIPromptsService, IKevinAIChatMessageStore _kevinAIChatMessageStore, ISignalRMsgService _signalRMsgService
            // IAIClient _aIClient
            ) : base(_httpContextAccessor)
        {
            this.aIChatsRp = _aIChatsRp;
            this.aIAgentService = _aIAgentService;
            this.aIChatHistorysRp = _aIChatHistorysRp;
            this.aIAppsService = _aIAppsService;
            this.aIModelsService = _aIModelsService;
            this.aIPromptsService = aIPromptsService;
            this.kevinAIChatMessageStore = _kevinAIChatMessageStore;
            this.signalRMsgService = _signalRMsgService;
            // this.aIClient = _aIClient;
        }

        /// <summary>
        /// 获取我的ai聊天列表
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<dtoPageData<AIChatsDto>> GetMyPageData(dtoPagePar<string> dtoPage)
        {
            var result = new dtoPageData<AIChatsDto>();
            int skip = dtoPage.GetSkip();
            var data = aIChatsRp.Query().Where(t => t.IsDelete == false && t.UserId == CurrentUser.UserId && t.TenantId == CurrentUser.TenantId && t.IsHidden == false);
            if (!string.IsNullOrEmpty(dtoPage.searchKey))
            {
                data = data.Where(t => (t.Name ?? "").Contains(dtoPage.searchKey));
            }
            result.total = await data.CountAsync();
            result.data = (await data.OrderByDescending(x => x.CreateTime).Skip(skip).Take(dtoPage.pageSize).ToListAsync()).MapToList<TAIChats, AIChatsDto>();
            return result;
        }


        /// <summary>
        /// 新建聊天
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<AIChatHistorysDto> Add(AIChatsDto par)
        {
            var aiapp = await aIAppsService.GetDetails(par.AppId);
            if ((await aIAppsService.GetMyALLList()).Any(t => t.Id == par.AppId) == false)
            {
                throw new UserFriendlyException("智能体权限不足，无法使用");
            }
            // Auto模式解析：随机选择一个可用模型
            if (string.Equals(aiapp.ChatModelID, "auto", StringComparison.OrdinalIgnoreCase))
            {
                var allModels = await aIModelsService.GetNoPerALLList(1);
                if (allModels.Count == 0)
                    throw new UserFriendlyException("当前没有可用的聊天模型，请联系管理员配置模型。");
                aiapp.ChatModelID = allModels[new Random().Next(allModels.Count)].Id.ToString();
            }
            var aIModels = await aIModelsService.GetDetails(aiapp.ChatModelID.ToTryInt64());
            var aIPrompts = await aIPromptsService.GetDetails(aiapp.AIPromptID);
            var add = par.MapTo<TAIChats>();
            add.Id = par.Id == default ? SnowflakeIdService.GetNextId() : par.Id;
            add.IsDelete = false;
            add.CreateTime = DateTime.Now;
            add.CreateUserId = CurrentUser.UserId;
            add.TenantId = CurrentUser.TenantId;
            add.UserId = CurrentUser.UserId;
            add.IsHidden = par.IsHidden;
            aIChatsRp.Add(add);
            await aIChatsRp.SaveChangesAsync();

            var addHist = new TAIChatHistorys
            {
                Id = SnowflakeIdService.GetNextId(),
                IsDelete = false,
                CreateTime = DateTime.Now,
                CreateUserId = CurrentUser.UserId,
                TenantId = CurrentUser.TenantId,
                IsSend = false,
                AIChatsId = add.Id,
                Content = "你好,请开始你的对话..."
            };
            aIChatHistorysRp.Add(addHist);
            await aIChatHistorysRp.SaveChangesAsync();
            // 选择“异步加载智能体”：在线程池里先把该智能体的工具/MCP/技能挂载好并调用一次，
            // 用户第一次发消息时不再承担冷启动加载耗时（实现方式与 AIChatHistorysService 里后台智能体调用同源）
            if (par.IsAsyncLoadAgent)
            {
                LoadAgentInBackground(aiapp, aIPrompts, aIModels, add.Id, addHist.Id, CurrentUser.UserId, CurrentUser.UserName, CurrentUser.TenantId);
            }
            return addHist.MapTo<AIChatHistorysDto>();
        }

        /// <summary>
        /// 异步（后台）加载智能体：另开 DI 作用域，走与 <see cref="AIChatHistorysService"/> 相同的智能体初始化流程——
        /// <c>GetAppAIAgentOptions</c> 挂载工具/MCP/记忆/技能，再创建智能体调用一次，让技能与工具连接提前就绪。
        /// <para>
        /// 请求作用域会在响应结束后释放，所以后台任务必须自己建作用域；无 HttpContext 时 MCP 取 Authorization
        /// 依赖 <see cref="IAIShareInfoService"/> 里的用户信息（与定时任务 RunTask 同做法）。
        /// </para>
        /// <para>
        /// 预热**不传 readOnlyHistory**（默认写入）：这一轮“加载技能”的问答会落进消息存储 <c>TAIChatMessageStore</c>，
        /// 从第一次真实消息起就作为上下文提交给 AI，模型据此知道 load_skill / 工具结果已就绪，无需再重走加载轮次
        /// ——这才是预热的价值所在。它只进消息存储、不进聊天列表（<c>TAIChatHistorys</c> 仍只有 Add 写的那条欢迎语），
        /// 所以不会出现在对话窗口里。
        /// </para>
        /// <para>
        /// 后台任务内异常全部接住只记日志：预热失败则退化为第一次发消息时现加载，不影响正常对话。
        /// </para>
        /// </summary>
        private void LoadAgentInBackground(AIAppsDto aiapp, AIPromptsDto aIPrompts, AIModelsDto aIModels, long aiChatsId, long historyId, long userId, string userName, int tenantId)
        {
            var scopeFactory = _serviceProvider?.GetService<IServiceScopeFactory>();
            if (scopeFactory == default)
            {
                return;
            }
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var provider = scope.ServiceProvider;
                    provider.GetService<IAIShareInfoService>()?.InitData(new AIShareInfoDto
                    {
                        AIAppsId = aiapp.Id,
                        AIChatsId = aiChatsId,
                        UserId = userId,
                        UserName = userName,
                        TenantId = tenantId,
                        AuthorizedDomains = aiapp.AuthorizedDomains,
                        ContentLengthLimit = aiapp.ContentLengthLimit,
                        IsSecurityIntercept = aiapp.IsSecurityIntercept,
                        ChatMessageLimit = aiapp.ChatMessageLimit
                    });
                    string systemPrompt = SystemPrompt.SystemPromptText + "\n 智能体提示词规则：\n" + aIPrompts.Prompt;
                    var chatAgOs = await provider.GetRequiredService<IAIAppsService>().GetAppAIAgentOptions(aiapp, aIPrompts, systemPrompt,
                        new AIChatHistorysDto { Id = historyId, AIChatsId = aiChatsId, CreateTime = DateTime.Now });
                    var aiSetting = new AISetting
                    {
                        AIUrl = aIModels.EndPoint,
                        AIKeySecret = aIModels.ModelKey,
                        AIDefaultModel = aIModels.ModelName,
                        // 预热不需要流式输出与推送回调：同步跑完一轮即可，回复由历史提供器落进消息存储供后续复用
                        IsStreame = false,
                        // 不开 HTTP 日志：避免与其他并发请求的拦截器互相干扰
                        IsHttpLog = false,
                        MaxRetries = aiapp.MaxRetries,
                        NetworkTimeout = aiapp.NetworkTimeout,
                        // 能力开关与真实对话（AIChatHistorysService.AddCoreAsync）保持一致，预热的就是用户第一次发消息时会用到的那套
                        IsAISkills = aiapp.IsSkill,
                        IsAITools = aiapp.IsAITools,
                        IsMcpTools = aiapp.IsMcp,
                        IsMemory = aiapp.IsMemory,
                    };
                    await provider.GetRequiredService<IAIAgentService>().CreateOpenAIAgentAndSendMSG(aiSetting, chatAgOs,
                        new ChatMessage(ChatRole.User, "系统初始化预热：请调用 load_skill、read_skill_resource 获取技能指令与资源内容，加载完所有技能后仅回复“初始化完成”。"));
                }
                catch (Exception ex)
                {
                    // 后台任务里异常绝不能外逃（等同 async void 冒到线程池会崩进程），只记日志
                    LogHelper.logger.Error($"异步加载智能体失败（不影响对话，首次发消息时会现加载）AppId:{aiapp.Id},ChatsId:{aiChatsId}:", ex);
                }
            });
        }
        /// <summary>
        /// 修改对话的主题和最后一条消息
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Name"></param>
        /// <param name="LastMessage"></param>
        /// <returns></returns>
        public async Task<bool> UpdateNameAndMsg(long Id, string Name = "", string LastMessage = "", CancellationToken cancellationToken = default)
        {
            try
            {
                using var db = new KevinDbContext();
                Name = StringHelper.SubstringText(Name, 200, "...");
                LastMessage = StringHelper.SubstringText(LastMessage, 300, "...");
                var ai = await db.Set<TAIChats>().Where(t => t.IsDelete == false && t.Id == Id).FirstOrDefaultAsync();
                if (ai != null)
                {
                    if (!string.IsNullOrEmpty(Name))
                    {
                        ai.Name = Name;
                    }
                    if (!string.IsNullOrEmpty(LastMessage))
                    {
                        ai.LastMessage = LastMessage;
                    }
                    ai.UpdateTime = DateTime.Now;
                    ai.UpdateUserId = CurrentUser.UserId;
                    db.Set<TAIChats>().Update(ai);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.logger.Error("修改对话的主题和最后一条消息失败:", ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取ai聊天对话
        /// </summary>
        /// <param name="id"></param> 
        /// <returns></returns> 
        public async Task<AIChatsDto> GetDetails(long id)
        {
            var data = (await aIChatsRp.Query().FirstOrDefaultAsync(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId && t.Id == id)).MapTo<AIChatsDto>();
            if (data == default)
            {
                throw new UserFriendlyException("获取ai聊天对话不存在或已删除");
            }
            return data;
        }

        /// <summary>
        /// 删除ai聊天
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> Delete(long id)
        {
            var like = await aIChatsRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (like != null)
            {
                like.IsDelete = true;
                like.DeleteTime = DateTime.Now;
                aIChatsRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }


}
