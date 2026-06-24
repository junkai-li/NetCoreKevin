using kevin.AI.AgentFramework.Agent.KevinChatMessageStore;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.ScriptRunners;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using Kevin.AI.Dto;
using Kevin.Common.Extension;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp.Tools;
using OpenAI;
using Repository.Database;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace kevin.Application.Services.AI
{
    public class AIAppsService : BaseService, IAIAppsService
    {
        public IAIAppsRp aIAppsRp { get; set; }

        public readonly IAISkillToolManagementService aISkillToolManagementService;

        public readonly IAISkillToolBindIdService aISkillToolBindIdService;

        public readonly IAIAppsBindIdService aIAppsBindIdService;
        public readonly IAIAgentService aIAgentService;
        public IAIModelsService aIModelsService { get; set; }

        public IAIPromptsService aIPromptsService { get; set; }
        public IKevinAIChatMessageStore kevinAIChatMessageStore { get; set; }
        private readonly IAIAgentToolSkillService _aIAgentToolSkillService;
        public readonly IAIChatMessageStoreRp _aIChatMessageStoreRp;

        public readonly IAIChatMessageStoreCompactionRp _aIChatMessageStoreCompactionRp;
        public readonly IAIChatMessageStoreCompactionService _aIChatMessageStoreCompactionService;
        public AIAppsService(IHttpContextAccessor _httpContextAccessor, IAIAppsRp _aIAppsRp,
            IAISkillToolManagementService aISkillToolManagementService, IAISkillToolBindIdService aISkillToolBindIdService, IAIAppsBindIdService aIAppsBindIdService,
            IKevinAIChatMessageStore kevinAIChatMessageStore, IAIAgentToolSkillService aIAgentToolSkillService, IAIModelsService aIModelsService, IAIPromptsService aIPromptsService,
            IAIAgentService aIAgentService, IAIChatMessageStoreRp aIChatMessageStoreRp, IAIChatMessageStoreCompactionRp aIChatMessageStoreCompactionRp, IAIChatMessageStoreCompactionService aIChatMessageStoreCompactionService) : base(_httpContextAccessor)
        {
            this.aIAppsRp = _aIAppsRp;
            this.aISkillToolManagementService = aISkillToolManagementService;
            this.aISkillToolBindIdService = aISkillToolBindIdService;
            this.aIAppsBindIdService = aIAppsBindIdService;
            this.kevinAIChatMessageStore = kevinAIChatMessageStore;
            _aIAgentToolSkillService = aIAgentToolSkillService;
            this.aIModelsService = aIModelsService;
            this.aIPromptsService = aIPromptsService;
            this.aIAgentService = aIAgentService;
            this._aIChatMessageStoreRp = aIChatMessageStoreRp;
            this._aIChatMessageStoreCompactionRp = aIChatMessageStoreCompactionRp;
            this._aIChatMessageStoreCompactionService = aIChatMessageStoreCompactionService;
        }

        /// <summary>
        /// 获取ai应用列表
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<dtoPageData<AIAppsDto>> GetPageData(dtoPagePar<string> dtoPage)
        {
            var result = new dtoPageData<AIAppsDto>();
            int skip = dtoPage.GetSkip();
            var data = aIAppsRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            if (!string.IsNullOrEmpty(dtoPage.searchKey))
            {
                data = data.Where(t => (t.Name ?? "").Contains(dtoPage.searchKey));
            }
            result.total = await data.CountAsync();
            var dbdata = await data.OrderByDescending(x => x.CreateTime).Skip(skip).Take(dtoPage.pageSize).Include(t => t.CreateUser).Include(t => t.UpdateUser).ToListAsync();
            result.data = dbdata.MapToList<TAIApps, AIAppsDto>();
            result.data.ForEach(t =>
            {
                t.CreateUser = dbdata.FirstOrDefault(d => d.Id == t.Id)?.CreateUser?.Name;
                t.UpdateUser = dbdata.FirstOrDefault(d => d.Id == t.Id)?.UpdateUser?.Name;
            });
            return result;
        }


        /// <summary>
        /// 获取ai应用
        /// </summary>
        /// <param name="id"></param> 
        /// <returns></returns> 
        public async Task<AIAppsDto> GetDetails(long id)
        {
            var data = (await aIAppsRp.Query(isDataPer: false).FirstOrDefaultAsync(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId && t.Id == id)).MapTo<AIAppsDto>();
            if (data == default)
            {
                throw new UserFriendlyException("ai应用数据不存在或已删除");
            }
            var skills = await aISkillToolManagementService.GetAllSkills();
            var tools = await aISkillToolManagementService.GetAllTools();
            var myIds = await aISkillToolBindIdService.GetListById(data.Id.ToString());
            data.BindIds = (await aIAppsBindIdService.GetListByBindId(data.Id.ToString())).Select(t => t.BindId).ToList();
            data.AISkillsToolsBindIds = myIds.Select(t => t.AISkillToolManagementId.ToString()).ToList();
            return data;
        }
        /// <summary>
        /// 获取ai应用列表
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<List<AIAppsDto>> GetALLList()
        {
            var result = new List<AIAppsDto>();
            var data = aIAppsRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            result = (await data.OrderByDescending(x => x.CreateTime).ToListAsync()).MapToList<TAIApps, AIAppsDto>();
            return result;
        }

        /// <summary>
        /// 获取我可用的ai应用列表
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<List<AIAppsDto>> GetMyALLList()
        {
            if (!CurrentUser.IsSuperAdmin)
            {
                var result = new List<AIAppsDto>();
                List<string> bingIds = new List<string> {
            "user_"+CurrentUser.UserId.ToString()
            };
                if (CurrentUser.RoleIds?.Count > 0)
                {
                    bingIds.AddRange(CurrentUser.RoleIds.Select(t => "role_" + t.ToString()).ToList());
                }
                var appIds = (await aIAppsBindIdService.GetListById(bingIds)).Select(t => t.TAIAppsId).ToList();
                var data = aIAppsRp.Query(isDataPer: false).Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId && appIds.Contains(t.Id));
                result = (await data.OrderByDescending(x => x.CreateTime).ToListAsync()).MapToList<TAIApps, AIAppsDto>();
                return result;
            }
            else
            {
                return await GetALLList();
            }


        }

        /// <summary>
        /// 编辑或添加ai应用
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> AddEdit(AIAppsDto par)
        {
            par.Check();
            var isAdd = par.Id == default;
            if (!isAdd)
            {
                var msg = aIAppsRp.Query().Where(t => t.IsDelete == false && t.Id == par.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = par.MapTo<TAIApps>();
                add.Id = par.Id == default ? SnowflakeIdService.GetNextId() : par.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                aIAppsRp.Add(add);
            }
            else
            {
                var msg = aIAppsRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == par.Id).FirstOrDefault();
                if (msg != default)
                {
                    msg.UpdateTime = DateTime.Now;
                    msg.UpdateUserId = CurrentUser.UserId;
                    msg.TenantId = CurrentUser.TenantId;
                    msg.Name = par.Name;
                    msg.Describe = par.Describe;
                    msg.Icon = par.Icon;
                    msg.Type = par.Type;
                    msg.ChatModelID = par.ChatModelID;
                    msg.RerankModelID = par.RerankModelID;
                    msg.Temperature = par.Temperature;
                    msg.KmsId = par.KmsId;
                    msg.SecretKey = par.SecretKey;
                    msg.Relevance = par.Relevance;
                    msg.MsgType = par.MsgType;
                    msg.MaxAskPromptSize = par.MaxAskPromptSize;
                    msg.MaxMatchesCount = par.MaxMatchesCount;
                    msg.RerankCount = par.RerankCount;
                    msg.AnswerTokens = par.AnswerTokens;
                    msg.AIPromptID = par.AIPromptID;
                    msg.IsAITools = par.IsAITools;
                    msg.IsSkill = par.IsSkill;
                    msg.MaxAskPromptSize = par.MaxAskPromptSize;
                    msg.NetworkTimeout = par.NetworkTimeout;
                    msg.IsHttpLog = par.IsHttpLog;
                    msg.AuthorizedDomains = par.AuthorizedDomains;
                    msg.ChatMessageLimit = par.ChatMessageLimit;
                    msg.IsToolLog = par.IsToolLog;
                    msg.IsThinkingLog = par.IsThinkingLog;
                    msg.ContentLengthLimit = par.ContentLengthLimit;
                    msg.IsSecurityIntercept = par.IsSecurityIntercept;
                    msg.MaxRetries = par.MaxRetries;
                    msg.ConversationTurnsExceed = par.ConversationTurnsExceed;
                    msg.IsAIMessageCompaction = par.IsAIMessageCompaction;
                    msg.IsAutoGetAIMessageCompaction = par.IsAutoGetAIMessageCompaction;
                    msg.AIMessageCompactionPrompt = par.AIMessageCompactionPrompt;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }

            }
            await aIAppsRp.SaveChangesAsync();
            var ids = par.Skills.Where(t => t.IsSelect).Select(t => t.AISkillToolManagementId).ToList();
            ids.AddRange(par.Tools.Where(t => t.IsSelect).Select(t => t.AISkillToolManagementId).ToList());
            await aISkillToolBindIdService.BatchAddIds(par.Id.ToString(), ids);
            await aIAppsBindIdService.BatchAddIds(par.Id.ToString(), par.BindIds);
            return true;
        }


        /// <summary>
        ///新增初始化
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<AIAppsDto> NewInitialization()
        {
            var data = new AIAppsDto();
            data.Id = SnowflakeIdService.GetNextId();
            data.CreateTime = DateTime.Now;
            data.CreateUserId = CurrentUser.UserId;
            var skills = await aISkillToolManagementService.GetAllSkills();
            var tools = await aISkillToolManagementService.GetAllTools();
            var myIds = await aISkillToolBindIdService.GetListById(data.Id.ToString());
            data.Skills = skills.Select(t => new AIAppsBindSkillToolsDto
            {
                IsSelect = myIds.Any(x => x.AISkillToolManagementId == t.Id),
                AISkillToolManagementName = t.Name,
                AISkillToolManagementDescription = t.Description,
                AISkillToolManagementId = t.Id
            }).ToList();
            data.Tools = tools.Select(t => new AIAppsBindSkillToolsDto
            {
                IsSelect = myIds.Any(x => x.AISkillToolManagementId == t.Id),
                AISkillToolManagementDescription = t.Description,
                AISkillToolManagementName = t.Name,
                AISkillToolManagementId = t.Id
            }).ToList();
            return data;
        }

        /// <summary>
        /// 删除ai应用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> Delete(long id)
        {
            var like = await aIAppsRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (like != null)
            {
                like.IsDelete = true;
                like.DeleteTime = DateTime.Now;
                aIAppsRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }

        /// <summary>
        /// 获取ai应用配置
        /// </summary>
        /// <param name="aiapp"></param>
        /// <param name="aIPrompts"></param>
        /// <param name="systemPrompt"></param>
        /// <param name="par"></param>
        /// <param name="parAi"></param>
        /// <returns></returns>
        public async Task<ChatClientAgentOptions> GetAppAIAgentOptions(AIAppsDto aiapp, AIPromptsDto aIPrompts, string systemPrompt, AIChatHistorysDto par, object parAi, CancellationToken cancellationToken = default)
        {
            #region 获取压缩聊天记录提示词
            if (aiapp.IsAutoGetAIMessageCompaction && aiapp.IsAIMessageCompaction)
            {
                systemPrompt += "\n" + await _aIChatMessageStoreCompactionService.GetThreadPrompt(par.AIChatsId.ToString());
            }
            #endregion

            var chatAgOs = new ChatClientAgentOptions
            {
                Name = aiapp.Name,
                Description = aIPrompts.Description ?? "你是一个智能体,请根据你的问题进行相关回答",
                ChatOptions = new Microsoft.Extensions.AI.ChatOptions
                {
                    MaxOutputTokens = aiapp.AnswerTokens,
                    Temperature = (float)(aiapp.Temperature / 100),
                    ResponseFormat = ChatResponseFormat.Text,
                    Instructions = systemPrompt,
                },
                ChatHistoryProvider = new KevinChatMessageStore(kevinAIChatMessageStore, par.AIChatsId.ToString(), aiapp.IsAIMessageCompaction ? aiapp.ConversationTurnsExceed : 0)
            };
            #region AI配置
            if (aiapp.IsAITools)
            {
                if (chatAgOs.ChatOptions != default)
                {
                    // 🔑 能力层：工具
                    chatAgOs.ChatOptions.Tools ??= new List<AITool>();
                    chatAgOs.ChatOptions.Tools.AddRange(_aIAgentToolSkillService.GetUserAIAgentToolsAsync(parAi, aiapp.Id.ToString(), CurrentUser.UserId.ToString()).Result);
                    if (aiapp.BindIds.Where(x => x.Contains("agent_")).Count() > 0)
                    {
                        var agentIds = aiapp.BindIds.Where(x => x.Contains("agent_")).Select(t => t.Replace("agent_", "")).ToList();
                        foreach (var item in agentIds)
                        {
                            var appitem = await GetDetails(item.ToTryInt64());
                            var aIAgent = await GetAppAIAgent(appitem, parAi, par, cancellationToken);
                            chatAgOs.ChatOptions.Tools.AddRange(aIAgent.AsAIFunction());
                        }
                    }
                    if (!aiapp.IsAutoGetAIMessageCompaction && aiapp.IsAIMessageCompaction)
                    {
                        _aIChatMessageStoreCompactionService.InitData(par.AIChatsId.ToString());
                        chatAgOs.ChatOptions.Tools.Add(AIFunctionFactory.Create(_aIChatMessageStoreCompactionService.GetAIToolThreadPrompt, new AIFunctionFactoryOptions
                        {
                            Name = "GetAIToolThreadPrompt",
                            Description = "获取聊天对话历史记录，当用户询问聊天记录时调用，返回用户历史对话(压缩摘要版本)。"
                        }
      ));
                    }
                }
            }
            if (aiapp.IsSkill)
            {
                var skillPaths = _aIAgentToolSkillService.GetUserAIAgentSkillsAsync(parAi, aiapp.Id.ToString(), CurrentUser.UserId.ToString()).Result;
#pragma warning disable MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。  
                var skillsProvider = new AgentSkillsProviderBuilder()
                                               .UseFileScriptRunner(PySubprocessScriptRunner.StaticRunAsync)
                                               .UseOptions(options => options.DisableCaching = true);
                foreach (var skillPath in skillPaths)
                {
                    skillsProvider.UseFileSkill(Path.Combine(AppContext.BaseDirectory, "Skills", skillPath));
                }
                var sk = skillsProvider.Build();
                chatAgOs.AIContextProviders = [sk];
#pragma warning restore MAAI001
            }
            #endregion
            return chatAgOs;
        }

        /// <summary>
        /// 获取子ai应用
        /// </summary>
        /// <param name="aiapp"></param>
        /// <param name="parAi"></param>
        /// <param name="par"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="referenceDepth">深度为0时可以获取到子ai应用，深度为1时可以获取到子ai应用的子ai应用，以此类推 最多三级引用</param>
        /// <returns></returns>
        public async Task<AIAgent> GetAppAIAgent(AIAppsDto aiapp, object parAi, AIChatHistorysDto par, CancellationToken cancellationToken = default, int referenceDepth = 0)
        {
            var aIModels = await aIModelsService.GetDetails(aiapp.ChatModelID.ToTryInt64());
            var aIPrompts = await aIPromptsService.GetDetails(aiapp.AIPromptID);
            string systemPrompt = SystemPrompt.SystemPromptText + "\n 智能体提示词规则：\n" + aIPrompts.Prompt;
            // 获取压缩聊天记录提示词
            systemPrompt += "\n" + await _aIChatMessageStoreCompactionService.GetThreadPrompt(par.AIChatsId.ToString() + "_agent_" + aiapp.Id.ToString());
            var chatAgOs = new ChatClientAgentOptions
            {
                Name = aiapp.Name,
                Description = aIPrompts.Description ?? "你是一个智能体,请根据你的问题进行相关回答",
                ChatOptions = new Microsoft.Extensions.AI.ChatOptions
                {
                    MaxOutputTokens = aiapp.AnswerTokens,
                    Temperature = (float)(aiapp.Temperature / 100),
                    ResponseFormat = ChatResponseFormat.Text,
                    Instructions = systemPrompt,
                },
                ChatHistoryProvider = new KevinChatMessageStore(kevinAIChatMessageStore, par.AIChatsId.ToString() + "_agent_" + aiapp.Id.ToString(), aiapp.IsAIMessageCompaction ? aiapp.ConversationTurnsExceed : 0)
            };
            #region AI配置
            if (aiapp.IsAITools)
            {
                if (chatAgOs.ChatOptions != default)
                {
                    // 🔑 能力层：工具
                    chatAgOs.ChatOptions.Tools ??= new List<AITool>();
                    chatAgOs.ChatOptions.Tools.AddRange(_aIAgentToolSkillService.GetUserAIAgentToolsAsync(parAi, aiapp.Id.ToString(), CurrentUser.UserId.ToString()).Result);
                    if (referenceDepth < 3)
                    {
                        if (aiapp.BindIds.Where(x => x.Contains("agent_")).Count() > 0)
                        {
                            referenceDepth++;
                            var agentIds = aiapp.BindIds.Where(x => x.Contains("agent_")).Select(t => t.Replace("agent_", "")).ToList();
                            foreach (var item in agentIds)
                            {
                                var appitem = await GetDetails(item.ToTryInt64());
                                var aIAgent = await GetAppAIAgent(appitem, parAi, par, cancellationToken, referenceDepth);
                                chatAgOs.ChatOptions.Tools.AddRange(aIAgent.AsAIFunction());
                            }
                        }
                    }
                }
            }
            if (aiapp.IsSkill)
            {
                var skillPaths = _aIAgentToolSkillService.GetUserAIAgentSkillsAsync(parAi, aiapp.Id.ToString(), CurrentUser.UserId.ToString()).Result;
#pragma warning disable MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。  
                var skillsProvider = new AgentSkillsProviderBuilder()
                                               .UseFileScriptRunner(PySubprocessScriptRunner.StaticRunAsync)
                                               .UseOptions(options => options.DisableCaching = true);
                foreach (var skillPath in skillPaths)
                {
                    skillsProvider.UseFileSkill(Path.Combine(AppContext.BaseDirectory, "Skills", skillPath));
                }
                var sk = skillsProvider.Build();
                chatAgOs.AIContextProviders = [sk];
#pragma warning restore MAAI001  
            }
            #endregion

            return (await aIAgentService.CreateOpenAIAgent(new AISetting
            {
                AIUrl = aIModels.EndPoint,
                AIKeySecret = aIModels.ModelKey,
                AIDefaultModel = aIModels.ModelName,
                IsStreame = aiapp.MsgType == 2,
                IsHttpLog = aiapp.IsHttpLog,
                MaxRetries = aiapp.MaxRetries,
                NetworkTimeout = aiapp.NetworkTimeout,
                IsAISkills = aiapp.IsSkill,
                IsAITools = aiapp.IsAITools
            }, chatAgOs,
          cancellationToken: cancellationToken));
        }
    }
}
