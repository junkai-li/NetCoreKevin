using kevin.AI.AgentFramework.Agent.KevinChatMessageStore;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.ImageGeneration;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.ScriptRunners;
using kevin.AI.AgentFramework.TextLimits;
using kevin.AI.AgentFramework.TokenEstimation;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using kevin.Domain.Share.Enums;
using Kevin.AI.Dto;
using Kevin.Common.Extension;
using Kevin.log4Net;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Text.Json;

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
        public readonly IPySubprocessScriptRunner _pySubprocessScriptRunner;

        /// <summary>
        /// 文生图工具工厂：按智能体绑定的模型配置生成 GenerateImage AIFunction。
        /// <para>
        /// 独立于 <see cref="IAIAgentToolSkillService"/> 的通用工具管道：文生图工具依赖具体模型配置（endpoint/key/model），
        /// 需要在挂载时按 <c>aiapp.ImageGenModelID</c> 动态解析，不适合作为静态工具库注册。
        /// </para>
        /// </summary>
        private readonly IImageGenToolFactory _imageGenToolFactory;

        /// <summary>
        /// 知识库搜索工具工厂：智能体绑定了知识库（KmsId）时挂载一个 SearchKnowledge 可搜索工具，
        /// 由模型按需检索，替代旧版发消息前固定预注入知识库上下文的方案。
        /// </summary>
        private readonly IAKnowledgeSearchToolService _knowledgeSearchToolService;

        /// <summary>
        /// 记忆服务：用于按当前提问自动召回高相关记忆（不依赖模型主动调用 SearchMemory）
        /// </summary>
        private readonly IAIAgentMemoryService _aIAgentMemoryService;

        public AIAppsService(IHttpContextAccessor _httpContextAccessor, IAIAppsRp _aIAppsRp,
            IAISkillToolManagementService aISkillToolManagementService, IAISkillToolBindIdService aISkillToolBindIdService, IAIAppsBindIdService aIAppsBindIdService,
            IKevinAIChatMessageStore kevinAIChatMessageStore, IAIAgentToolSkillService aIAgentToolSkillService, IAIModelsService aIModelsService, IAIPromptsService aIPromptsService,
            IAIAgentService aIAgentService, IAIChatMessageStoreRp aIChatMessageStoreRp, IAIChatMessageStoreCompactionRp aIChatMessageStoreCompactionRp,
            IAIChatMessageStoreCompactionService aIChatMessageStoreCompactionService, IPySubprocessScriptRunner pySubprocessScriptRunner,
            IImageGenToolFactory imageGenToolFactory, IAKnowledgeSearchToolService knowledgeSearchToolService, IAIAgentMemoryService aIAgentMemoryService) : base(_httpContextAccessor)
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
            this._pySubprocessScriptRunner = pySubprocessScriptRunner;
            this._imageGenToolFactory = imageGenToolFactory;
            this._knowledgeSearchToolService = knowledgeSearchToolService;
            this._aIAgentMemoryService = aIAgentMemoryService;
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
                data = data.Where(t => t.Name.Contains(dtoPage.searchKey)||t.Describe.Contains(dtoPage.searchKey));
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
        /// 获取ai应用
        /// </summary>
        /// <param name="id"></param> 
        /// <returns></returns> 
        public async Task<AIAppsDto> GetNoPerDetails(long id)
        {
            var data = (await aIAppsRp.Query(isDataPer: false, isTenant: false).FirstOrDefaultAsync(t => t.IsDelete == false && t.Id == id)).MapTo<AIAppsDto>();
            if (data == default)
            {
                throw new UserFriendlyException("ai应用数据不存在或已删除");
            }
            var skills = await aISkillToolManagementService.GetNotDataPerAllSkills();
            var tools = await aISkillToolManagementService.GetNotDataPerAllTools();
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
                var AppName = aIAppsRp.Query().Where(t => t.Name == par.Name && t.IsDelete == false).FirstOrDefault();
                //验证唯一不允许添加
                if (AppName != null && par.Id != AppName.Id)
                {
                    throw new UserFriendlyException("智能体名称已存在");
                }
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
                    var AppName = aIAppsRp.Query().Where(t => t.Name == par.Name && t.IsDelete == false).FirstOrDefault();
                    //验证唯一不允许添加
                    if (AppName != null && par.Id != AppName.Id)
                    {
                        throw new UserFriendlyException("智能体名称已存在");
                    }
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
                    msg.MaxMatchesCount = par.MaxMatchesCount;
                    msg.RerankCount = par.RerankCount;
                    msg.AIPromptID = par.AIPromptID;
                    msg.IsAITools = par.IsAITools;
                    msg.IsSkill = par.IsSkill;
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
                    msg.CompactionTriggerRatio = par.CompactionTriggerRatio;
                    msg.IsAIMessageCompaction = par.IsAIMessageCompaction;
                    msg.IsAutoGetAIMessageCompaction = par.IsAutoGetAIMessageCompaction;
                    msg.AIMessageCompactionPrompt = par.AIMessageCompactionPrompt;
                    msg.ResponseFormat = par.ResponseFormat;
                    msg.ReasoningEffort = par.ReasoningEffort;
                    msg.ReasoningOutput = par.ReasoningOutput;
                    msg.IsMcp = par.IsMcp;
                    msg.IsMemory = par.IsMemory;
                    msg.IsImageGeneration = par.IsImageGeneration;
                    msg.ImageGenModelID = par.ImageGenModelID;   
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }

            }
            await aIAppsRp.SaveChangesAsync();
            var ids = par.Skills.Where(t => t.IsSelect).Select(t => t.AISkillToolManagementId).ToList();
            ids.AddRange(par.Tools.Where(t => t.IsSelect).Select(t => t.AISkillToolManagementId).ToList());
            ids.AddRange(par.Mcps.Where(t => t.IsSelect).Select(t => t.AISkillToolManagementId).ToList());
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
        /// 获取AI应用的聊天选项配置
        /// </summary>
        /// <param name="aiapp"></param>
        /// <param name="systemPrompt"></param>
        /// <returns></returns>
        public ChatOptions GetAppChatOptions(AIAppsDto aiapp, AIModelsDto aiModel, string systemPrompt)
        {
            ReasoningOptions? reasoning = default;
            if (aiapp.ReasoningEffort >= 0 || aiapp.ReasoningOutput >= 0)
            {
                reasoning = new ReasoningOptions();
                if (aiapp.ReasoningEffort >= 0)
                {
                    reasoning.Effort = (ReasoningEffort)(aiapp.ReasoningEffort ?? 0);
                }
                if (aiapp.ReasoningOutput >= 0)
                {
                    reasoning.Output = (ReasoningOutput)(aiapp.ReasoningOutput ?? 0);
                }

            }
            ChatResponseFormat? responseFormat = default;
            if (!string.IsNullOrEmpty(aiapp.ResponseFormat))
            {
                switch (aiapp.ResponseFormat)
                {
                    case "Json":
                        responseFormat = ChatResponseFormat.Json;
                        break;
                    case "Text":
                        responseFormat = ChatResponseFormat.Text;
                        break;
                    default:
                        break;
                }
            }
            return new Microsoft.Extensions.AI.ChatOptions
            {
                MaxOutputTokens = aiModel.AnswerTokens,
                Temperature = (float)(aiapp.Temperature / 100),
                Instructions = systemPrompt,
                Reasoning = reasoning,
                ResponseFormat = responseFormat
            };
        }
        /// <summary>
        /// 解析调用方提供的参数 JSON Schema 原文；非法或缺省时退化为空对象 schema（{"type":"object","properties":{}}），
        /// 保证工具照常声明给模型（参数由模型自由生成）
        /// </summary>
        private static JsonElement BuildExternalToolSchema(string? parametersJson)
        {
            if (!string.IsNullOrWhiteSpace(parametersJson))
            {
                try
                {
                    using var doc = JsonDocument.Parse(parametersJson);
                    if (doc.RootElement.ValueKind == JsonValueKind.Object) return doc.RootElement.Clone();
                }
                catch (JsonException)
                {
                    // 调用方传入的 schema 不是合法 JSON：忽略，走下方兜底
                }
            }
            using var fallback = JsonDocument.Parse("{\"type\":\"object\",\"properties\":{}}");
            return fallback.RootElement.Clone();
        }

        /// <summary>
        /// 压缩摘要的字符上限：可用提问预算的 1/5（摘要也占 systemPrompt 预算），最少 2000 字符；
        /// 模型未配置提问预算时给 20000（仅作兜底，不代表真实预算）。
        /// </summary>
        private static int GetCompactionMaxLength(int maxAskPromptSize, int answerTokens, string? systemPrompt)
        {
            var promptBudget = TokenEstimator.ResolveAskBudget(maxAskPromptSize, answerTokens, systemPrompt);
            return promptBudget > 0 ? Math.Max(CharLimits.CompactionSummaryMin, TokenEstimator.ToCharCount(promptBudget / 5)) : CharLimits.CompactionSummaryNoBudget;
        }
        /// <summary>
        /// 获取ai应用配置
        /// </summary>
        /// <param name="aiapp"></param>
        /// <param name="aIPrompts"></param>
        /// <param name="systemPrompt"></param>
        /// <param name="par"></param>
        /// <param name="parAi"></param>
        /// <param name="withCapabilities">是否挂载工具/技能/记忆/文生图：一次性元任务（如推荐问题）传 false，省掉一轮工具枚举</param>
        /// <param name="readOnlyHistory">会话历史是否只读：true 时本次调用读得到历史但不写回去，不至于把元指令污染进对话</param>
        /// <param name="userQuery">当前用户提问：开启记忆时用于自动召回高相关记忆，为空则跳过召回</param>
        /// <returns></returns>
        public async Task<ChatClientAgentOptions> GetAppAIAgentOptions(AIAppsDto aiapp, AIPromptsDto aIPrompts, string systemPrompt, AIChatHistorysDto par, CancellationToken cancellationToken = default, bool withCapabilities = true, bool readOnlyHistory = false, string? userQuery = null)
        {
            #region 记忆管理协议提示词（仅在开启智能体记忆 IsMemory 时注入）
            if (aiapp.IsMemory)
            {
                systemPrompt += "\n" + SystemPrompt.MemoryPromptText;
            }
            #endregion
            #region 记忆自动召回（按当前提问召回高相关记忆并注入，避免依赖模型主动调用 SearchMemory）
            if (aiapp.IsMemory && withCapabilities && !string.IsNullOrWhiteSpace(userQuery))
            {
                var recallContext = await _aIAgentMemoryService.BuildAutoRecallContextAsync(userQuery!);
                if (!string.IsNullOrWhiteSpace(recallContext))
                {
                    systemPrompt += "\n" + recallContext;
                }
            }
            #endregion
            #region 文生图协议提示词（仅在开启 IsImageGeneration 时注入，约束模型把工具返回的 markdown 图片链接原样透传）
            if (aiapp.IsImageGeneration)
            {
                systemPrompt += "\n" + SystemPrompt.ImageGenerationPromptText;
            }
            #endregion
            // Token配置来自模型配置（提问最大token数/回答最大token数）
            // Auto模式：解析为实际模型
            if (string.Equals(aiapp.ChatModelID, "auto", StringComparison.OrdinalIgnoreCase))
            {
                var allModels = await aIModelsService.GetNoPerALLList(1);
                if (allModels.Count == 0)
                {
                    throw new UserFriendlyException("当前没有可用的聊天模型，请联系管理员配置模型。");
                }
                aiapp.ChatModelID = allModels[new Random().Next(allModels.Count)].Id.ToString();
            }
            var aiModel = await aIModelsService.GetNoPerDetails(aiapp.ChatModelID.ToTryInt64());
            #region 获取压缩聊天记录提示词
            if (aiapp.IsAutoGetAIMessageCompaction && aiapp.IsAIMessageCompaction)
            {
                // 摘要也占 systemPrompt 预算：先按不含摘要的提示词估算可用预算，摘要最多占其中的 1/5
                var compactionMaxLength = GetCompactionMaxLength(aiModel.MaxAskPromptSize, aiModel.AnswerTokens, systemPrompt);
                systemPrompt += "\n" + await _aIChatMessageStoreCompactionService.GetThreadPrompt(par.AIChatsId.ToString(), compactionMaxLength);
            }
            #endregion
            var historyProvider = new KevinChatMessageStore(kevinAIChatMessageStore, par.AIChatsId.ToString(), aiapp.IsAIMessageCompaction ? aiapp.ConversationTurnsExceed : 0, TokenEstimator.ResolveAskBudget(aiModel.MaxAskPromptSize, aiModel.AnswerTokens, systemPrompt), aiapp.ContentLengthLimit)
            {
                ReadOnlyHistory = readOnlyHistory
            };
            var chatAgOs = new ChatClientAgentOptions
            {
                Name = aiapp.Name,
                Description = aIPrompts.Description ?? "你是一个智能体,请根据你的问题进行相关回答",
                ChatOptions = GetAppChatOptions(aiapp, aiModel, systemPrompt),
                ChatHistoryProvider = historyProvider
            };
            #region AI配置
            // 一次性元任务（推荐问题等）不挂能力：既要工具也没用，还会白跑一轮 MCP 工具枚举；
            // 下面全部能力挂载都在本 region 内，到这里 chatAgOs 已经完整，直接返回即可
            if (!withCapabilities) return chatAgOs;
            if (aiapp.IsAITools)
            {
                if (chatAgOs.ChatOptions != default)
                {
                    // 🔑 能力层：工具
                    chatAgOs.ChatOptions.Tools ??= new List<AITool>();
                    chatAgOs.ChatOptions.Tools.AddRange(await _aIAgentToolSkillService.GetUserAIAgentToolsAsync(aiapp.Id.ToString(), (CurrentUser?.UserId ?? 0).ToString()));
                    if (aiapp.BindIds.Where(x => x.Contains("agent_")).Count() > 0)
                    {
                        var agentIds = aiapp.BindIds.Where(x => x.Contains("agent_")).Select(t => t.Replace("agent_", "")).ToList();
                        foreach (var item in agentIds)
                        {
                            var appitem = await GetNoPerDetails(item.ToTryInt64());
                            var aIAgent = await GetAppAIAgent(appitem, par, cancellationToken);
                            chatAgOs.ChatOptions.Tools.AddRange(aIAgent.AsAIFunction());
                        }
                    }
                    if (!aiapp.IsAutoGetAIMessageCompaction && aiapp.IsAIMessageCompaction)
                    { 
                        chatAgOs.ChatOptions.Tools.Add(AIFunctionFactory.Create(_aIChatMessageStoreCompactionService.GetAIToolThreadPrompt, new AIFunctionFactoryOptions
                        {
                            Name = "GetAIToolThreadPrompt",
                            Description = "获取聊天对话历史记录，当用户询问聊天记录时调用，返回用户历史对话(压缩摘要版本)。"
                        }
      ));
                    }
                }
            }
            if (aiapp.IsMcp)
            {
                if (chatAgOs.ChatOptions != default)
                {
                    chatAgOs.ChatOptions.Tools ??= new List<AITool>();
                    chatAgOs.ChatOptions.Tools.AddRange(await _aIAgentToolSkillService.GetUserAIAgentMcpToolsAsync(aiapp.Id.ToString(), (CurrentUser?.UserId ?? 0).ToString()));
                }
            }
            #region 知识库工具（绑定了 KmsId 就挂载，独立于 IsAITools）
            await TryMountKnowledgeSearchToolAsync(aiapp, chatAgOs);
            #endregion
            #region 记忆工具（仅在开启智能体记忆 IsMemory 时注入，独立于 IsAITools）
            if (aiapp.IsMemory)
            {
                chatAgOs.ChatOptions.Tools ??= new List<AITool>();
                chatAgOs.ChatOptions.Tools.AddRange(await _aIAgentToolSkillService.GetMemoryToolsAsync());
            }
            #endregion
            #region 文生图工具（仅在开启 IsImageGeneration 时注入，独立于 IsAITools）
            await TryMountImageGenToolAsync(aiapp, chatAgOs);
            #endregion
            if (aiapp.IsSkill)
            {
                var skillPaths = await _aIAgentToolSkillService.GetUserAIAgentSkillsAsync(aiapp.Id.ToString(), (CurrentUser?.UserId ?? 0).ToString());
                var skillsProvider = new AgentSkillsProviderBuilder()
                                         .UseOptions(t =>
                                         {
                                             t.DisableLoadSkillApproval = true;
                                             t.DisableReadSkillResourceApproval = true;
                                             t.DisableRunSkillScriptApproval = true;
                                         })
                                         .UseFileScriptRunner(_pySubprocessScriptRunner.StaticRunAsync);
                foreach (var skillPath in skillPaths)
                {
                    skillsProvider.UseFileSkill(Path.Combine(AppContext.BaseDirectory, "Skills", skillPath), new AgentFileSkillsSourceOptions
                    {
                        SearchDepth = 2
                    });
                }
                var sk = skillsProvider.Build();
                chatAgOs.AIContextProviders = [sk];
            }
            #endregion
            #region 外部声明型工具（OpenAI function calling 透传：调用方声明的工具以“只声明、不可执行”注入，模型调用后由网关原样交回调用方执行）
            if (par.ExternalTools is { Count: > 0 } && chatAgOs.ChatOptions != default)
            {
                chatAgOs.ChatOptions.Tools ??= new List<AITool>();
                foreach (var externalTool in par.ExternalTools)
                {
                    if (externalTool == default || string.IsNullOrWhiteSpace(externalTool.Name)) continue;
                    // 与智能体自身工具同名时以外部声明为准：移除同名可执行工具，避免被框架当成可执行工具而误执行
                    foreach (var existed in chatAgOs.ChatOptions.Tools.Where(t => string.Equals(t.Name, externalTool.Name, StringComparison.Ordinal)).ToList())
                    {
                        chatAgOs.ChatOptions.Tools.Remove(existed);
                    }
                    chatAgOs.ChatOptions.Tools.Add(AIFunctionFactory.CreateDeclaration(externalTool.Name, externalTool.Description ?? "", BuildExternalToolSchema(externalTool.ParametersJson), null));
                }
            }
            if (chatAgOs.ChatOptions != default && !string.IsNullOrWhiteSpace(par.ToolMode))
            {
                chatAgOs.ChatOptions.ToolMode = par.ToolMode switch
                {
                    "none" => ChatToolMode.None,
                    "required" => ChatToolMode.RequireAny,
                    "function" when !string.IsNullOrWhiteSpace(par.ToolModeFunctionName) => ChatToolMode.RequireSpecific(par.ToolModeFunctionName!),
                    _ => ChatToolMode.Auto,
                };
            }
            #endregion
            return chatAgOs;
        }

        /// <summary>
        /// 仅问答模式能力清单：按智能体已开启的配置，把工具/MCP/技能/子智能体/记忆/文生图/知识库的名称与描述拼成文本。
        /// 仅问答模式不挂载执行工具，模型只能靠这份清单了解自己的工具箱，避免误答“没有可用工具”。
        /// </summary>
        public async Task<string> BuildAskCapabilityListAsync(AIAppsDto aiapp, CancellationToken cancellationToken = default)
        {
            var lines = new List<string>();
            var userId = (CurrentUser?.UserId ?? 0).ToString();
            if (aiapp.IsAITools)
            {
                foreach (var tool in await _aIAgentToolSkillService.GetUserAIAgentToolsAsync(aiapp.Id.ToString(), userId))
                {
                    lines.Add($"- 工具 {tool.Name}：{ShortDescribe(tool.Description)}");
                }
                foreach (var agentId in aiapp.BindIds.Where(x => x.Contains("agent_")).Select(t => t.Replace("agent_", "")).ToList())
                {
                    var appitem = await GetNoPerDetails(agentId.ToTryInt64());
                    if (appitem != default)
                    {
                        lines.Add($"- 子智能体 {appitem.Name}：{ShortDescribe(appitem.Describe)}");
                    }
                }
            }
            if (aiapp.IsMcp)
            {
                foreach (var tool in await _aIAgentToolSkillService.GetUserAIAgentMcpToolsAsync(aiapp.Id.ToString(), userId))
                {
                    lines.Add($"- MCP工具 {tool.Name}：{ShortDescribe(tool.Description)}");
                }
            }
            if (aiapp.IsSkill)
            {
                var skillPaths = await _aIAgentToolSkillService.GetUserAIAgentSkillsAsync(aiapp.Id.ToString(), userId);
                foreach (var skillPath in skillPaths.Take(20))
                {
                    lines.Add($"- 技能 {Path.GetFileName(skillPath)}");
                }
                if (skillPaths.Count > 20)
                {
                    lines.Add($"- 技能（另有 {skillPaths.Count - 20} 个未列出）");
                }
            }
            if (aiapp.IsMemory)
            {
                lines.Add("- 记忆管理（跨会话保存/检索/更新/删除用户偏好与经验）");
            }
            if (aiapp.IsImageGeneration)
            {
                lines.Add("- 文生图 GenerateImage（根据文字描述生成图片）");
            }
            if (aiapp.KmsId.HasValue && aiapp.KmsId.Value > 0)
            {
                lines.Add("- 知识库检索 SearchKnowledge（检索已绑定知识库的文档资料）");
            }
            var result = lines.Count > 0 ? string.Join("\n", lines) : "";
            // 清单总长受控：工具过多时截断，避免挤占提问 Token 预算
            return result.Length > CharLimits.AskCapabilityList
                ? result.Substring(0, CharLimits.AskCapabilityList) + "\n（能力较多，清单已截断）"
                : result;
        }

        /// <summary>
        /// 能力清单单行描述截断：只保留名称后的简要说明，超长截断避免清单过长
        /// </summary>
        private static string ShortDescribe(string? description, int maxLength = 120)
        {
            var text = string.IsNullOrWhiteSpace(description) ? "暂无描述" : description;
            return text.Length > maxLength ? text.Substring(0, maxLength) + "…" : text;
        }

        /// <summary>
        /// 一次性调用当前智能体用的配置：与主对话同模型、同系统提示词、同会话历史窗口，
        /// 但不挂工具/技能/记忆，且会话历史只读。
        /// <para>
        /// 用于“推荐问题”这类元任务：它的输入输出不是真实对话，写回去会污染后续上下文；
        /// 又必须是一份独立实例，因为 CreateOpenAIAgentAndSendMSG 会按入参就地清空传入 options 的工具与技能上下文，
        /// 与主回答共用同一个对象会把本轮回答的能力一起清掉。
        /// </para>
        /// </summary>
        public Task<ChatClientAgentOptions> GetOneShotAIAgentOptions(AIAppsDto aiapp, AIPromptsDto aIPrompts, string systemPrompt, AIChatHistorysDto par, CancellationToken cancellationToken = default)
            => GetAppAIAgentOptions(aiapp, aIPrompts, systemPrompt, par, cancellationToken, withCapabilities: false, readOnlyHistory: true);

        /// <summary>
        /// 获取子ai应用
        /// </summary>
        /// <param name="aiapp"></param>
        /// <param name="parAi"></param>
        /// <param name="par"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="referenceDepth">深度为0时可以获取到子ai应用，深度为1时可以获取到子ai应用的子ai应用，以此类推 最多八级引用</param>
        /// <returns></returns>
        public async Task<AIAgent> GetAppAIAgent(AIAppsDto aiapp, AIChatHistorysDto par, CancellationToken cancellationToken = default, int referenceDepth = 0, int MaxReferenceDepth = 8)
        {
            // Auto模式：子智能体随机选择一个模型
            if (string.Equals(aiapp.ChatModelID, "auto", StringComparison.OrdinalIgnoreCase))
            {
                var allModels = await aIModelsService.GetNoPerALLList(1);
                if (allModels.Count == 0)
                {
                    throw new UserFriendlyException("当前没有可用的聊天模型，请联系管理员配置模型。");
                }
                aiapp.ChatModelID = allModels[new Random().Next(allModels.Count)].Id.ToString();
            }
            var aIModels = await aIModelsService.GetNoPerDetails(aiapp.ChatModelID.ToTryInt64());
            var aIPrompts = await aIPromptsService.GetNoPerDetails(aiapp.AIPromptID);
            string systemPrompt = SystemPrompt.SystemPromptText + "\n 智能体提示词规则：\n" + aIPrompts.Prompt;
            // 记忆管理协议提示词（仅在开启智能体记忆 IsMemory 时注入）
            if (aiapp.IsMemory)
            {
                systemPrompt += "\n" + SystemPrompt.MemoryPromptText;
            }
            // 文生图协议提示词（仅在开启 IsImageGeneration 时注入，约束模型把工具返回的 markdown 图片链接原样透传）
            if (aiapp.IsImageGeneration)
            {
                systemPrompt += "\n" + SystemPrompt.ImageGenerationPromptText;
            }
            // 获取压缩聊天记录提示词
            if (aiapp.IsAutoGetAIMessageCompaction && aiapp.IsAIMessageCompaction)
            {
                // 摘要也占 systemPrompt 预算：先按不含摘要的提示词估算可用预算，摘要最多占其中的 1/5
                var compactionMaxLength = GetCompactionMaxLength(aIModels.MaxAskPromptSize, aIModels.AnswerTokens, systemPrompt);
                systemPrompt += "\n" + await _aIChatMessageStoreCompactionService.GetThreadPrompt(par.AIChatsId.ToString() + "_agent_" + aiapp.Id.ToString(), compactionMaxLength);
            } 
            var chatAgOs = new ChatClientAgentOptions
            {
                Name = aiapp.Name,
                Description = aIPrompts.Description ?? "你是一个智能体,请根据你的问题进行相关回答",
                ChatOptions = GetAppChatOptions(aiapp, aIModels, systemPrompt),
                ChatHistoryProvider = new KevinChatMessageStore(kevinAIChatMessageStore, par.AIChatsId.ToString() + "_agent_" + aiapp.Id.ToString(), aiapp.IsAIMessageCompaction ? aiapp.ConversationTurnsExceed : 0, TokenEstimator.ResolveAskBudget(aIModels.MaxAskPromptSize, aIModels.AnswerTokens, systemPrompt), aiapp.ContentLengthLimit)
            };
            #region AI配置
            if (aiapp.IsAITools)
            {
                if (chatAgOs.ChatOptions != default)
                {
                    // 🔑 能力层：工具
                    chatAgOs.ChatOptions.Tools ??= new List<AITool>();
                    chatAgOs.ChatOptions.Tools.AddRange(await _aIAgentToolSkillService.GetUserAIAgentToolsAsync(aiapp.Id.ToString(), (CurrentUser?.UserId ?? 0).ToString()));
                    if (referenceDepth < MaxReferenceDepth)
                    {
                        if (aiapp.BindIds.Where(x => x.Contains("agent_")).Count() > 0)
                        {
                            referenceDepth++;
                            var agentIds = aiapp.BindIds.Where(x => x.Contains("agent_")).Select(t => t.Replace("agent_", "")).ToList();
                            foreach (var item in agentIds)
                            {
                                var appitem = await GetNoPerDetails(item.ToTryInt64());
                                var aIAgent = await GetAppAIAgent(appitem, par, cancellationToken, referenceDepth);
                                chatAgOs.ChatOptions.Tools.AddRange(aIAgent.AsAIFunction());
                            }
                        }
                    }
                }
            }
            if (aiapp.IsMcp)
            {
                if (chatAgOs.ChatOptions != default)
                {
                    chatAgOs.ChatOptions.Tools ??= new List<AITool>();
                    chatAgOs.ChatOptions.Tools.AddRange(await _aIAgentToolSkillService.GetUserAIAgentMcpToolsAsync(aiapp.Id.ToString(), (CurrentUser?.UserId ?? 0).ToString()));
                }
            }
            #region 知识库工具（绑定了 KmsId 就挂载，独立于 IsAITools）
            await TryMountKnowledgeSearchToolAsync(aiapp, chatAgOs);
            #endregion
            #region 记忆工具（仅在开启智能体记忆 IsMemory 时注入，独立于 IsAITools）
            if (aiapp.IsMemory)
            {
                chatAgOs.ChatOptions.Tools ??= new List<AITool>();
                chatAgOs.ChatOptions.Tools.AddRange(await _aIAgentToolSkillService.GetMemoryToolsAsync());
            }
            #endregion
            #region 文生图工具（仅在开启 IsImageGeneration 时注入，独立于 IsAITools）
            await TryMountImageGenToolAsync(aiapp, chatAgOs);
            #endregion
            if (aiapp.IsSkill)
            {
                var skillPaths = await _aIAgentToolSkillService.GetUserAIAgentSkillsAsync(aiapp.Id.ToString(), (CurrentUser?.UserId ?? 0).ToString());
                var skillsProvider = new AgentSkillsProviderBuilder()
                                       .UseOptions(t =>
                                       {
                                           t.DisableLoadSkillApproval = true;
                                           t.DisableReadSkillResourceApproval = true;
                                           t.DisableRunSkillScriptApproval = true;
                                       })
                                       .UseFileScriptRunner(_pySubprocessScriptRunner.StaticRunAsync);
                foreach (var skillPath in skillPaths)
                {
                    skillsProvider.UseFileSkill(Path.Combine(AppContext.BaseDirectory, "Skills", skillPath), new AgentFileSkillsSourceOptions
                    {
                        SearchDepth = 2
                    });
                }
                var sk = skillsProvider.Build();
                chatAgOs.AIContextProviders = [sk];
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
                IsAITools = aiapp.IsAITools,
                IsMcpTools = aiapp.IsMcp,
                IsMemory = aiapp.IsMemory,
                IsKnowledgeBase = aiapp.KmsId.HasValue && aiapp.KmsId.Value > 0,
            }, chatAgOs,
          cancellationToken: cancellationToken));
        }

        /// <summary>
        /// 按智能体绑定的知识库挂载 SearchKnowledge AIFunction（<see cref="GetAppAIAgentOptions"/> 与 <see cref="GetAppAIAgent"/> 共用）。
        /// <para>
        /// 容错策略与文生图工具一致：未绑定知识库、知识库不存在或构建异常都不抛错打断主流程，只写警告日志跳过挂载。
        /// </para>
        /// </summary>
        private async Task TryMountKnowledgeSearchToolAsync(AIAppsDto aiapp, ChatClientAgentOptions chatAgOs)
        {
            if (aiapp.KmsId == default) return;
            if (chatAgOs.ChatOptions == null) return;
            try
            {
                var kmsTool = await _knowledgeSearchToolService.BuildSearchFunction(aiapp);
                if (kmsTool != null)
                {
                    chatAgOs.ChatOptions.Tools ??= new List<AITool>();
                    chatAgOs.ChatOptions.Tools.Add(kmsTool);
                }
                else
                {
                    LogHelper.logger.Warn($"智能体 {aiapp.Name}(Id={aiapp.Id}) 绑定了知识库 KmsId={aiapp.KmsId} 但知识库不存在或不可用，跳过 SearchKnowledge 工具挂载");
                }
            }
            catch (Exception ex)
            {
                LogHelper.logger.Warn($"智能体 {aiapp.Name}(Id={aiapp.Id}) 挂载知识库搜索工具失败，跳过: {ex.Message}");
            }
        }

        /// <summary>
        /// 按智能体配置挂载 GenerateImage AIFunction（<see cref="GetAppAIAgentOptions"/> 与 <see cref="GetAppAIAgent"/> 共用）。
        /// <para>
        /// 容错策略：任何一环缺失都不抛异常打断主流程 ——
        /// <list type="bullet">
        ///   <item><description><c>IsImageGeneration=false</c>：直接跳过</description></item>
        ///   <item><description><c>ImageGenModelID</c> 未设置：跳过并写警告日志（避免用户开了开关没选模型时对话直接失败）</description></item>
        ///   <item><description>模型不存在或类型不含 <see cref="AIModelType.ImageGeneration"/> 标记：跳过并写警告日志</description></item>
        ///   <item><description>模型 EndPoint 为空：跳过并写警告日志</description></item>
        /// </list>
        /// 只有配置齐全时才真正把工具加进 <see cref="ChatClientAgentOptions"/>，避免模型看到工具但调用时才发现配置错误。
        /// </para>
        /// </summary>
        private async Task TryMountImageGenToolAsync(AIAppsDto aiapp, ChatClientAgentOptions chatAgOs)
        {
            if (!aiapp.IsImageGeneration) return;
            if (chatAgOs.ChatOptions == null) return;

            if (!aiapp.ImageGenModelID.HasValue || aiapp.ImageGenModelID.Value <= 0)
            {
                LogHelper.logger.Warn($"智能体 {aiapp.Name}(Id={aiapp.Id}) 开启了 IsImageGeneration 但未绑定 ImageGenModelID，跳过 GenerateImage 工具挂载");
                return;
            }

            AIModelsDto imageModel;
            try
            {
                imageModel = await aIModelsService.GetNoPerDetails(aiapp.ImageGenModelID.Value);
            }
            catch (Exception ex)
            {
                LogHelper.logger.Warn($"智能体 {aiapp.Name}(Id={aiapp.Id}) 绑定的文生图模型 Id={aiapp.ImageGenModelID} 解析失败，跳过 GenerateImage 工具挂载: {ex.Message}");
                return;
            }

            if (!imageModel.AIModelType.HasFlag(AIModelType.ImageGeneration))
            {
                LogHelper.logger.Warn($"智能体 {aiapp.Name}(Id={aiapp.Id}) 绑定的模型 {imageModel.ModelName} AIModelType={imageModel.AIModelType} 不含 ImageGeneration 标记，跳过挂载");
                return;
            }
            if (string.IsNullOrWhiteSpace(imageModel.EndPoint))
            {
                LogHelper.logger.Warn($"智能体 {aiapp.Name}(Id={aiapp.Id}) 绑定的文生图模型 {imageModel.ModelName} EndPoint 为空，跳过挂载");
                return;
            }

            var config = new ImageGenModelConfig(imageModel.EndPoint, imageModel.ModelName, imageModel.ModelKey);
            chatAgOs.ChatOptions.Tools ??= new List<AITool>();
            chatAgOs.ChatOptions.Tools.Add(_imageGenToolFactory.BuildGenerateFunction(config));
        }
    }
}
