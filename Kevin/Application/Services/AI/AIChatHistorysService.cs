
using Common;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Safety;
using kevin.AI.AgentFramework.Modality;
using kevin.AI.AgentFramework.Tools;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using kevin.Domain.Share.Enums;
using Kevin.AI.Dto;
using Kevin.log4Net;
using Kevin.RAG.Interfaces;
using Kevin.RAG.Ollama;
using Kevin.SignalR.Service;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI;
using Repository.Database;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text;
using System.Text.Json;
namespace kevin.Application.Services.AI
{

    public class AIChatHistorysService : BaseService, IAIChatHistorysService
    {
        public IAIChatHistorysRp aIChatHistorysRp { get; set; }
        public IAIAgentService aIAgentService { get; set; }
        public IAIModelsService aIModelsService { get; set; }

        public IAIPromptsService aIPromptsService { get; set; }
        public IAIChatsService aIChatsService { get; set; }
        public IAIAppsService aIAppsService { get; set; }
        private IRAGService rAGServicevice { get; set; }
        public IKevinAIChatMessageStore kevinAIChatMessageStore { get; set; }
        public ISignalRMsgService signalRMsgService { get; set; }

        public IAIKmssService aIKmssService { get; set; }

        public IHttpClientFactory httpClientFactory { get; set; }

        private IOllamaApiService ollamaApiService;

        private readonly IAIChatHistorysBindLogService _aIChatHistorysBindLogService;

        private readonly IAIShareInfoService _aIShareInfoService;

        private readonly IAIInputOutputSafetyService _aIInputOutputSafetyService;

        /// <summary>
        /// 多模态内容构造器：把远程文件 URL 转成 AIContent（图片/音频）或纯文本片段（文档）。
        /// <para>
        /// 抽出去之前 AIFileUrlsHandle 直接调 FileHelper.DetermineFileType + 5 个 Reader，
        /// 加音频模态时不得不动 Service 主流程；封装后 Service 只管 await 结果并按 Kind 分桶归集。
        /// </para>
        /// </summary>
        private readonly IModalityContentBuilder _modalityContentBuilder;

        public AIChatHistorysService(IHttpContextAccessor _httpContextAccessor, IAIChatHistorysRp _aIChatHistorysRp,
            IAIAgentService _aIAgentService, IAIModelsService _aIModelsService, IAIPromptsService _aIPromptsService,
            IAIChatsService _aIChatsService, IAIAppsService _aIAppsService, IKevinAIChatMessageStore _kevinAIChatMessageStore,
            IRAGService _rAGService, IAIKmssService _aIKmssService, IOllamaApiService _ollamaApiService, ISignalRMsgService _signalRMsgService,
            IHttpClientFactory _httpClientFactory, IAIChatHistorysBindLogService _aIChatHistorysBindLogService,
            IAIChatMessageStoreCompactionService _aIChatMessageStoreCompactionService, IAIShareInfoService aIShareInfoService, IAIInputOutputSafetyService aIInputOutputSafetyService,
            IModalityContentBuilder modalityContentBuilder
            ) : base(_httpContextAccessor)
        {
            this.aIChatHistorysRp = _aIChatHistorysRp;
            this.aIChatsService = _aIChatsService;
            this.aIAgentService = _aIAgentService;
            this.aIModelsService = _aIModelsService;
            this.aIPromptsService = _aIPromptsService;
            this.aIAppsService = _aIAppsService;
            this.kevinAIChatMessageStore = _kevinAIChatMessageStore;
            this.rAGServicevice = _rAGService;
            this.aIKmssService = _aIKmssService;
            this.ollamaApiService = _ollamaApiService;
            this.signalRMsgService = _signalRMsgService;
            this.httpClientFactory = _httpClientFactory;
            this._aIChatHistorysBindLogService = _aIChatHistorysBindLogService;
            this._aIShareInfoService = aIShareInfoService;
            this._aIInputOutputSafetyService = aIInputOutputSafetyService;
            this._modalityContentBuilder = modalityContentBuilder;
        }

        /// <summary>
        /// 获取我的ai聊天列表
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<dtoPageData<AIChatHistorysDto>> GetPageData(dtoPagePar<string> dtoPage)
        {
            var result = new dtoPageData<AIChatHistorysDto>();
            int skip = dtoPage.GetSkip();
            var data = aIChatHistorysRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            if (dtoPage.whereId > 0)
            {
                data = data.Where(t => t.AIChatsId == dtoPage.whereId);
            }
            else
            {
                throw new UserFriendlyException("必须传入聊天Id");
            }
            result.total = await data.CountAsync();
            result.data = (await data.OrderByDescending(x => x.CreateTime).Skip(skip).Take(dtoPage.pageSize).ToListAsync()).MapToList<TAIChatHistorys, AIChatHistorysDto>();
            var logdata = await _aIChatHistorysBindLogService.GetByIds(result.data.Select(t => t.Id).ToList());
            foreach (var item in result.data)
            {
                item.aIChatHistorysBindLogs = logdata.Where(t => t.AIChatHistorysId == item.Id).ToList();
            }
            return result;
        }

        /// <summary>
        /// 向累计日志追加文本：达到上限后不再追加（首次触顶时补一句说明）。
        /// <para>
        /// 字段虽然是 longtext，但单行 INSERT 的参数总量超过 MySQL max_allowed_packet（默认 4MB）就会以
        /// DbUpdateException（Error submitting 4MB packet）失败并中断整个对话请求；中文按 3 字节/字估算，
        /// 默认把回答 + 工具日志 + 思考日志合计控制在约 85 万字符（≤2.6MB）以内，留出协议开销余量
        /// （各项上限可在 appsettings 的 AIChatStorageSetting 节调整）。
        /// </para>
        /// </summary>
        private static string AppendWithCap(string? current, string msg, int maxLength)
        {
            current ??= "";
            if (string.IsNullOrEmpty(msg)) return current;
            if (current.Length >= maxLength) return current;
            var room = maxLength - current.Length;
            if (msg.Length <= room) return current + msg;
            // 截断说明也要占额度，否则封顶后的实际长度反而超过 maxLength
            var note = $"\n（日志已达 {maxLength} 字符入库上限，后续内容不再记录）";
            return current + msg.Substring(0, Math.Max(0, room - note.Length)) + note;
        }

        /// <summary>
        /// 入库前按上限截断（不超限时原样返回），只用于兜住单行 INSERT 体积
        /// </summary>
        private static string ShrinkForDb(string? text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return "";
            if (maxLength <= 0 || text.Length <= maxLength) return text;
            // 同样把说明文字自身计入额度，保证返回值总长不超过 maxLength
            var note = $"\n（内容已达 {maxLength} 字符入库上限，后续部分未入库）";
            return text.Substring(0, Math.Max(0, maxLength - note.Length)) + note;
        }


        /// <summary>
        /// 新建聊天（默认输出通道）：流式分片经 SignalR 按提问记录 Id 推送给前端。
        /// </summary>
        public Task<AIChatHistorysDto> Add(AIChatHistorysDto par, CancellationToken cancellationToken)
            => AddCoreAsync(par, identity => new SignalRChatStreamOutput(signalRMsgService, identity), cancellationToken);

        /// <summary>
        /// 新建聊天（SSE 流式输出）：处理逻辑与 <see cref="Add"/> 完全共用 <see cref="AddCoreAsync"/>，
        /// 仅把处理过程中的流式分片改为以 SSE（text/event-stream）写回当前 HTTP 响应。
        /// <para>
        /// 下发的事件名与 SignalR 通道一一对应：<c>processmsg</c> / <c>aimsg</c> / <c>aIToolsContentMsg</c> /
        /// <c>aIReasoningContentMsg</c>；本轮结束时额外下发 <c>done</c>（data 为最终回复记录的 JSON），
        /// 业务异常则以 <c>error</c> 事件回传可读提示。
        /// </para>
        /// </summary>
        public async Task AddSSE(AIChatHistorysDto par, CancellationToken cancellationToken)
        {
            var response = HttpContextAccessor.HttpContext?.Response;
            if (response == default)
            {
                throw new UserFriendlyException("SSE 输出需要在 HTTP 请求上下文中调用");
            }
            if (!response.HasStarted)
            {
                response.ContentType = "text/event-stream";
                response.Headers["Cache-Control"] = "no-cache";
                response.Headers["Connection"] = "keep-alive";
                // Nginx 等反向代理默认会缓冲响应，关掉才能让分片实时下发
                response.Headers["X-Accel-Buffering"] = "no";
            }
            var sseOutput = new SseChatStreamOutput(response);
            try
            {
                var result = await AddCoreAsync(par, _ => sseOutput, cancellationToken);
                await sseOutput.WriteAsync("done", SerializeForClient(result));
            }
            catch (UserFriendlyException ex)
            {
                // 响应头已下发，无法再走全局异常中间件返回 400，改以 SSE error 事件回传可读提示
                await sseOutput.WriteAsync("error", ex.Message);
            }
            // 写完终止事件后显式结束响应，让 Kestrel 正常下发 chunked 终止块（0\r\n\r\n），
            // 而不是等连接回收时才中断——否则前端 fetch 会把它判成 ERR_INCOMPLETE_CHUNKED_ENCODING。
            // 客户端中途断开时 CompleteAsync 自身会抛，这里吞掉即可（流本就已断，无法再写）。
            try
            {
                await response.CompleteAsync();
            }
            catch (Exception ex)
            {
                LogHelper.logger.Warn("SSE 响应收尾失败（可能客户端已断开）:", ex);
            }
        }

        /// <summary>
        /// 把最终记录序列化为 <c>done</c> 事件体：与 <c>Add</c> 接口的 HTTP 返回复用 MVC 同一套 JsonSerializerOptions
        /// （camelCase + <see cref="Common.Json.LongConverter"/> long→字符串），
        /// 保证前端从 SSE <c>done</c> 解出的对象与 SignalR 路径下的 <c>reulst.data</c> 字段完全一致，
        /// 不会因默认序列化退化成 PascalCase、也不会把雪花 Id 写成会丢精度的数字。</summary>
        private string SerializeForClient(AIChatHistorysDto result)
        {
            var jsonOptions = _serviceProvider
                ?.GetService<IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>()?.Value?.JsonSerializerOptions;
            return JsonSerializer.Serialize(result, jsonOptions);
        }

        /// <summary>
        /// 聊天处理主流程：提问落库、RAG、文件、联网、模型调用、回复落库等逻辑全部在此，两种输出方式共用。
        /// </summary>
        /// <param name="outputFactory">入参为提问记录 Id（身份标识），返回本次请求使用的流式输出通道；
        /// 由 <see cref="Add"/> 传入 SignalR 通道、由 <see cref="AddSSE"/> 传入 SSE 通道</param>
        private async Task<AIChatHistorysDto> AddCoreAsync(AIChatHistorysDto par, Func<string, IChatStreamOutput> outputFactory, CancellationToken cancellationToken)
        {
            var reslutUserCheck = await _aIInputOutputSafetyService.CheckUserInput(par.Content);
            if (!reslutUserCheck.Item1)
            {
                throw new UserFriendlyException($"输入内容非法：{reslutUserCheck.Item2?.SafetyMessage}");
            }
            var aichas = await aIChatsService.GetDetails(par.AIChatsId);
            var aiapp = await aIAppsService.GetDetails(aichas.AppId);

            #region 重试：先废弃上次失败的问答，避免历史里出现两条相同提问
            var retryCount = 0;
            if (par.RetryOfId != default)
            {
                var oldAsk = await aIChatHistorysRp.Query().FirstOrDefaultAsync(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId
                    && t.AIChatsId == par.AIChatsId && t.Id == par.RetryOfId && t.IsSend == true, cancellationToken);
                // 请求层就失败（断网、被全局异常拦下）时前端也会带着 retryOfId 重试，那条旧记录根本没落库；
                // 这里按一次普通发送处理，而不是抛业务异常把重试按钮点死
                if (oldAsk != default)
                {
                    // 该提问之后的回复行一并软删（含上次记下报错的那条），旧行仍是 IsDelete=false 保留，可事后查失败记录
                    var oldAnswers = await aIChatHistorysRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId
                            && t.AIChatsId == par.AIChatsId && t.IsSend == false && t.CreateTime >= oldAsk.CreateTime).ToListAsync(cancellationToken);
                    oldAsk.IsDelete = true;
                    oldAsk.DeleteTime = DateTime.Now;
                    foreach (var oldAnswer in oldAnswers)
                    {
                        oldAnswer.IsDelete = true;
                        oldAnswer.DeleteTime = DateTime.Now;
                    }
                    retryCount = oldAsk.RetryCount + 1;
                    await aIChatHistorysRp.SaveChangesAsync(cancellationToken);
                }
            }
            #endregion

            var count = await aIChatHistorysRp.Query().Where(t => t.IsDelete == false && t.AIChatsId == par.AIChatsId).CountAsync(cancellationToken);
            if (count >= aiapp.ChatMessageLimit)
            {
                throw new UserFriendlyException($"聊天记录已达上限{aiapp.ChatMessageLimit}条，为了更好的体验，建议新建聊天对话噢！");
            }
            // 提问内容长度守卫：整行 INSERT 受 MySQL max_allowed_packet（默认 4MB）限制，过长提问会在写入用户消息时直接整请求失败，
            // 这里提前拦下给出可读提示，而不是静默截断用户原文（上限取自 AIChatStorageSetting）
            var askLength = par.Content?.Length ?? 0;
            var askLimit = AIChatStorageSetting.Current.AskContentMaxLength;
            if (askLength > askLimit)
            {
                throw new UserFriendlyException($"单条消息内容过长（{askLength} 字符，上限 {askLimit} 字符），请精简或分批发送。");
            }
            if ((await aIAppsService.GetMyALLList()).Any(t => t.Id == aichas.AppId) == false)
            {
                throw new UserFriendlyException("智能体权限不足，无法使用");
            }
            // Auto模式：解析为实际模型并构建备选模型列表
            List<AIFallbackModel> fallbackModels = new();
            if (string.Equals(aiapp.ChatModelID, "auto", StringComparison.OrdinalIgnoreCase))
            {
                var allModels = await aIModelsService.GetNoPerALLList(1);
                if (allModels.Count == 0)
                {
                    throw new UserFriendlyException("当前没有可用的聊天模型，请联系管理员配置模型。");
                }
                // 随机打乱模型顺序
                var random = new Random();
                allModels = allModels.OrderBy(_ => random.Next()).ToList();
                // 第一个模型作为主模型
                aiapp.ChatModelID = allModels[0].Id.ToString();
                // 剩余模型作为备选
                fallbackModels = allModels.Skip(1).Select(m => new AIFallbackModel
                {
                    AIUrl = m.EndPoint,
                    AIKeySecret = m.ModelKey,
                    AIDefaultModel = m.ModelName,
                    MaxAskPromptSize = m.MaxAskPromptSize,
                    AnswerTokens = m.AnswerTokens
                }).ToList();
            }
            var aIModels = await aIModelsService.GetDetails(aiapp.ChatModelID.ToTryInt64());
            var aIPrompts = await aIPromptsService.GetDetails(aiapp.AIPromptID);
            var add = par.MapTo<TAIChatHistorys>();
            add.Id = par.Id == default ? SnowflakeIdService.GetNextId() : par.Id;
            // 拿到提问记录 Id 后才能确定身份标识，据此构建本次请求的流式输出通道（SignalR 或 SSE）
            var output = outputFactory(add.Id.ToString());
            add.IsDelete = false;
            add.CreateTime = DateTime.Now;
            add.CreateUserId = CurrentUser.UserId;
            add.TenantId = CurrentUser.TenantId;
            add.IsSend = true;
            add.RetryCount = retryCount;
            //回复消息
            var addAi = new TAIChatHistorys
            {
                Id = SnowflakeIdService.GetNextId(),
                IsDelete = false,
                CreateTime = DateTime.Now,
                CreateUserId = CurrentUser.UserId,
                TenantId = CurrentUser.TenantId,
                IsSend = false,
                AIChatsId = par.AIChatsId
            };
            // 两条记录都先按“失败”落库（重试中则记 2），拿到回复后才改回成功：RAG/文件/联网/模型/入库任一环节异常
            // （包括没人接的 500），这条提问都已经是一条带失败原因的可重试记录，
            // 不会再像以前那样 Add(add) 排在模型调用之后、一失败就整轮查无此事
            var initialStatus = retryCount > 0 ? AIChatHistorysSendStatusEnums.Retrying : AIChatHistorysSendStatusEnums.Fail;
            add.SendStatus = initialStatus;
            add.FailReason = SendInterruptedReason;
            addAi.SendStatus = initialStatus;
            addAi.FailReason = SendInterruptedReason;
            aIChatHistorysRp.Add(add);
            await aIChatHistorysRp.SaveChangesAsync(cancellationToken);
            AISetting? aiSetting = default;
            try
            {
                string systemPrompt = SystemPrompt.SystemPromptText + "\n 智能体提示词规则：\n" + aIPrompts.Prompt;
                await _aIChatHistorysBindLogService.AddEdit(new TAIChatHistorysBindLog() { AIChatHistorysId = addAi.Id, LogContent = systemPrompt, LogType = AIChatHistorysBindLogEnums.SystemPrompt });
                List<string> OtherContents = new List<string>();

                if (aiapp.KmsId != default)
                {
                    var ksmData = await KmsRag(add, aiapp, addAi, output);
                    if (ksmData.Count > 0)
                    {
                        OtherContents.AddRange(ksmData);
                    }
                }
                _aIShareInfoService.InitData(new AIShareInfoDto
                {
                    AIAppsId = aiapp.Id,
                    AIChatsId = add.AIChatsId,
                    UserId = CurrentUser.UserId,
                    UserName = CurrentUser.UserName,
                    TenantId = CurrentUser.TenantId,
                    AuthorizedDomains = aiapp.AuthorizedDomains,
                    ContentLengthLimit = aiapp.ContentLengthLimit,
                    IsSecurityIntercept = aiapp.IsSecurityIntercept,
                    ChatMessageLimit = aiapp.ChatMessageLimit
                });
                #region 文件处理

                // 音频模态门禁：由模型 AIModelType 位标记决定；关闭时 Builder 会把音频转成文本提示，避免向不支持音频的模型发送 DataContent 触发 400
                var enableAudioInput = aIModels.AIModelType.HasFlag(AIModelType.AudioUnderstanding);
                var aiFilData = await AIFileUrlsHandle(add, aiapp, addAi, enableAudioInput, output, cancellationToken);
                if (aiFilData.ExtractedTexts.Count > 0)
                    OtherContents.AddRange(aiFilData.ExtractedTexts);

                #endregion

                #region 联网搜索
                if (par.IsOnlineSearch)
                {
                    await output.WriteAsync("processmsg", "正在联网搜索....");
                    var http = new HttpClientFunction(aIAgentService, _serviceProvider);
                    var webseoData = await http.GetSeoAsync(add.Content, aIModels.EndPoint, aIModels.ModelName, aIModels.ModelKey);
                    await _aIChatHistorysBindLogService.AddEdit(new TAIChatHistorysBindLog() { AIChatHistorysId = addAi.Id, LogContent = webseoData, LogType = AIChatHistorysBindLogEnums.WebSeo });
                    OtherContents.Add(StringHelper.SubstringText(webseoData, aiapp.ContentLengthLimit));
                }
                #endregion
                // 按提问Token预算裁剪补充上下文，确保输入总量不超过模型的上下文窗口预算（模型配置的MaxAskPromptSize）
                OtherContents = TrimContentsByAskTokenBudget(OtherContents, systemPrompt, add.Content, aIModels.MaxAskPromptSize, aIModels.AnswerTokens);
                // 空输入保护：模型的输入长度区间是双边的（如 “Range of input length should be [1, N]”），正文、上下文、图片/音频全为空时输入长度为 0，同样会被模型直接拒掉，
                // 这里按业务异常提前拦下，让前端拿到可读提示而不是模型报错
                if (string.IsNullOrWhiteSpace(add.Content) && OtherContents.Count == 0 && aiFilData.MediaContents.Count == 0)
                {
                    throw new UserFriendlyException("请输入要咨询的内容（或上传可解析的文件）后再发送。");
                }
                // 零 .Result 构造消息：媒体内容已在 AIFileUrlsHandle 内部 await 完成（Bug 1 根治点）；
                // DataContent 携带显式 MIME，不再依赖 MemoryStream.Name 推断（Bug 2 根治点）
                var userContents = new List<AIContent>(1 + OtherContents.Count + aiFilData.MediaContents.Count)
                {
                    new TextContent(add.Content ?? "")
                };
                foreach (var text in OtherContents)
                {
                    if (!string.IsNullOrEmpty(text)) userContents.Add(new TextContent(text));
                }
                userContents.AddRange(aiFilData.MediaContents);
                ChatMessage mgs = new(ChatRole.User, userContents);
                var chatAgOs = await aIAppsService.GetAppAIAgentOptions(aiapp, aIPrompts, systemPrompt, par);
                switch (aIModels.AIType)
                {
                    case Domain.Share.Enums.AIType.OpenAI:
                    case Domain.Share.Enums.AIType.ZhiPuAI:
                    case Domain.Share.Enums.AIType.AzureOpenAI:
                    default:
                        await output.WriteAsync("processmsg", "正在结合相关信息思考....");
                        aiSetting = new AISetting
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
                            // 音频输入能力传递给 AIAgentService：模型不支持时可用于日志/降级判定
                            EnableAudioInput = enableAudioInput,
                            FallbackModels = fallbackModels,
                            StreameCallback = async (msg) =>
                            {
                                await output.WriteAsync("aimsg", msg);
                            },
                            ToolStreameCallback = async (msg) =>
                            {
                                // 单条事件按智能体内容长度上限截断后累加，同时整个字段封顶：
                                // 一次几十万字的工具输出不能把整行记录撑到 MySQL 无法写入
                                var toolLog = StringHelper.SubstringText(msg, aiapp.ContentLengthLimit);
                                addAi.AIToolsContent = AppendWithCap(addAi.AIToolsContent, toolLog, AIChatStorageSetting.Current.ToolsLogMaxLength);
                                if (aiapp.IsToolLog)
                                {
                                    await output.WriteAsync("aIToolsContentMsg", toolLog);
                                }
                            },
                            ReasoningStreameCallback = async (msg) =>
                            {
                                var reasoningLog = StringHelper.SubstringText(msg, aiapp.ContentLengthLimit);
                                addAi.AIReasoningContent = AppendWithCap(addAi.AIReasoningContent, reasoningLog, AIChatStorageSetting.Current.ReasoningLogMaxLength);
                                if (aiapp.IsThinkingLog)
                                {
                                    await output.WriteAsync("aIReasoningContentMsg", reasoningLog);
                                }
                            },
                        };
                        var reslut = (await aIAgentService.CreateOpenAIAgentAndSendMSG(aiSetting, chatAgOs, mgs, cancellationToken: cancellationToken));
                        var reslutAiCheck = await _aIInputOutputSafetyService.CheckAIOutput(reslut.Item2);
                        if (!reslutAiCheck.Item1)
                        {
                            throw new UserFriendlyException($"AI输出内容非法：{reslutAiCheck.Item2?.SafetyMessage}");
                        }
                        addAi.Content = ShrinkForDb(reslut.Item2, AIChatStorageSetting.Current.AnswerContentMaxLength);
                        if (reslut.Item3 != default)
                        {
                            addAi.CachedInputTokenCount = reslut.Item3.CachedInputTokenCount;
                            addAi.InputTokenCount = reslut.Item3.InputTokenCount;
                            addAi.OutputTokenCount = reslut.Item3.OutputTokenCount;
                            addAi.TotalTokenCount = reslut.Item3.TotalTokenCount;
                            addAi.ReasoningTokenCount = reslut.Item3.ReasoningTokenCount;
                        }
                        // 模型层的异常已经被转成“❌ 友好文案”当正文返回，HTTP 仍是 200，
                        // 只能靠 AISetting.LastError 区分“真回复”与“报错文案”，否则这条记录会被当成成功、永远出不来重试按钮
                        var sendFail = aiSetting.LastError != default;
                        addAi.FailReason = sendFail ? BuildFailReason(aiSetting.LastError!, aiSetting, add.RetryCount) : null;
                        SetSendStatus(add, addAi, sendFail, addAi.FailReason);
                        break;
                }
            }
            catch (UserFriendlyException)
            {
                // 业务类错误（内容为空、重试目标不存在等）仍按 HTTP 400 交给前端弹窗处理：
                // 把提前落库的提问一并软删，效果等同于改造前的“整条都没写进去”
                await DropEarlyMessages(add);
                throw;
            }
            catch (Exception ex)
            {
                // 非业务异常（模型客户端构造失败、上下文处理报错、客户端中止等）：以失败态收尾并正常返回，
                // 让前端当场就能在提问上重试并看到报错，而不是只弹一条 message.error 后刷新就什么都看不到了
                var failReason = BuildFailReason(ex, aiSetting, add.RetryCount);
                await output.WriteAsync("processmsg", "发送失败，可点红色按钮重试");
                SetSendStatus(add, addAi, true, failReason);
                aIChatHistorysRp.Add(addAi);
                // 这里不能用 cancellationToken：客户端中止进来时它已被置位，用它写库会直接抛回异常
                await aIChatHistorysRp.SaveChangesAsync(CancellationToken.None);
                var failData = addAi.MapTo<AIChatHistorysDto>();
                failData.aIChatHistorysBindLogs = await _aIChatHistorysBindLogService.GetByIds(new List<long> { addAi.Id });
                return failData;
            }
            var logdata = await _aIChatHistorysBindLogService.GetByIds(new List<long> { addAi.Id });
            aIChatHistorysRp.Add(addAi);
            await aIChatHistorysRp.SaveChangesAsync(cancellationToken);
            await aIChatsService.UpdateNameAndMsg(par.AIChatsId, count == 1 ? par.Content : "", addAi.Content);
            var BindApps = new Dictionary<AIAppsDto, AIModelsDto>();
            if (aiapp.BindIds.Where(x => x.Contains("agent_")).Count() > 0)
            {
                var agentIds = aiapp.BindIds.Where(x => x.Contains("agent_")).Select(t => t.Replace("agent_", "")).ToList();
                foreach (var item in agentIds)
                {
                    var appitem = await aIAppsService.GetDetails(item.ToTryInt64());
                    if (appitem != default)
                    {
                        // Auto模式解析：子智能体如果是auto则随机选一个模型
                        if (string.Equals(appitem.ChatModelID, "auto", StringComparison.OrdinalIgnoreCase))
                        {
                            var allModels = await aIModelsService.GetNoPerALLList(1);
                            if (allModels.Count == 0)
                                throw new UserFriendlyException("当前没有可用的聊天模型，请联系管理员配置模型。");
                            appitem.ChatModelID = allModels[new Random().Next(allModels.Count)].Id.ToString();
                        }
                        BindApps.Add(appitem, await aIModelsService.GetNoPerDetails(appitem.ChatModelID.ToTryInt64()));
                    }
                }
            }
            Task.Run(() =>
            {
                MessageStoreCompaction(aiapp, aIModels, par.AIChatsId.ToString());
                //压缩绑定智能体聊天记录
                foreach (var item in BindApps)
                {
                    MessageStoreCompaction(item.Key, item.Value, par.AIChatsId.ToString() + "_agent_" + item.Key.Id.ToString());
                }
            });
            var data = addAi.MapTo<AIChatHistorysDto>();
            data.aIChatHistorysBindLogs = logdata;
            return data;
        }

        /// <summary>
        /// 提问落库时的默认失败原因：本轮收尾若仍是这个值，说明连回复都来得及产出（异常没人接住或进程被中断）
        /// </summary>
        private const string SendInterruptedReason = "发送中断：未收到模型回复，可点击重试";

        /// <summary>
        /// 失败原因入库长度上限：诊断文本不需要 longtext 的完整体积，也要给整行 INSERT 留出余量
        /// </summary>
        private const int FailReasonMaxLength = 2000;

        /// <summary>
        /// 设置一轮问答（提问行 + 回复行）的发送状态：两条保持一致，前端在哪一条上展示报错和重试入口都能对上
        /// </summary>
        private static void SetSendStatus(TAIChatHistorys add, TAIChatHistorys addAi, bool isFail, string? failReason)
        {
            var status = isFail ? AIChatHistorysSendStatusEnums.Fail : AIChatHistorysSendStatusEnums.Success;
            add.SendStatus = status;
            add.FailReason = failReason;
            addAi.SendStatus = status;
            addAi.FailReason = failReason;
        }

        /// <summary>
        /// 组装入库并展示的失败原因：异常类型 + 消息 + 实际使用的模型与尝试次数
        /// <para>
        /// Auto 模式下异常可能已跨过好几个备选模型（<see cref="AISetting.AIDefaultModel"/> 保存的是最后一次尝试的模型），
        /// 光看正文里的“❌ …”无法定位是哪个模型报的错，这里把上下文一起存下来。
        /// </para>
        /// </summary>
        private static string BuildFailReason(Exception ex, AISetting? aiSetting, int retryCount)
        {
            if (ex is OperationCanceledException)
            {
                return "发送已中止（客户端取消或请求超时）";
            }
            var detail = $"{ex.GetType().Name}: {ex.InnerException?.Message ?? ex.Message}";
            if (aiSetting != default)
            {
                detail += $"｜模型：{aiSetting.AIDefaultModel}｜本次尝试 {aiSetting.AttemptCount} 次";
            }
            if (retryCount > 0)
            {
                detail += $"｜已是第 {retryCount + 1} 次发送";
            }
            return StringHelper.SubstringText(detail, FailReasonMaxLength, "...");
        }

        /// <summary>
        /// 业务类异常时把提前落库的提问软删，保证“前端弹提示 + 库里不留半截数据”和改造前一致。
        /// <para>固定不用请求的 CancellationToken：中止场景下它已被置位，用它写库会直接抛回异常。</para>
        /// </summary>
        private async Task DropEarlyMessages(TAIChatHistorys add)
        {
            try
            {
                add.IsDelete = true;
                add.DeleteTime = DateTime.Now;
                add.FailReason = null;
                await aIChatHistorysRp.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                LogHelper.logger.Error("回滚提前落库的聊天提问记录失败:", ex);
            }
        }

        /// <summary>
        /// 知识库搜索
        /// </summary>
        private async Task<List<string>> KmsRag(TAIChatHistorys add, AIAppsDto aiapp, TAIChatHistorys addAi, IChatStreamOutput output)
        {
            var OtherContents = new List<string>();
            await output.WriteAsync("processmsg", "正在查询知识库....");
            var kmss = await aIKmssService.GetDetails(aiapp.KmsId.GetValueOrDefault());
            if (kmss != default)
            {
                if (kmss.aIModelsId != default)
                {
                    var aimode = await aIModelsService.GetNoPerDetails(kmss.aIModelsId.GetValueOrDefault());
                    if (aimode?.AIModelType == AIModelType.Embedding)
                    {
                        ollamaApiService = new OllamaApiService(aimode.EndPoint, aimode.ModelName, aimode.ModelKey);
                    }
                }
                await output.WriteAsync("processmsg", "正在检索相关文档...");
                if (kmss.aIRerankModelsId == default)
                {
                    var systemPromptData = await rAGServicevice.GetRAGSystemPrompt("AIKmss-" + kmss.Id.ToString(),
                        await ollamaApiService.GetEmbedding(add.Content), add.Content, false, aiapp.MaxMatchesCount, (aiapp.Relevance / 100));
                    if (systemPromptData.Item1)
                    {
                        await _aIChatHistorysBindLogService.AddEdit(new TAIChatHistorysBindLog() { AIChatHistorysId = addAi.Id, LogContent = systemPromptData.Item2, LogType = AIChatHistorysBindLogEnums.Kmss });
                        OtherContents.Add(StringHelper.SubstringText(systemPromptData.Item2, aiapp.ContentLengthLimit));
                        await output.WriteAsync("processmsg", $"找到 {systemPromptData.Item3.Count} 个相关文档");
                    }
                }
                else
                {
                    var aIReankModels = await aIModelsService.GetNoPerDetails(kmss.aIRerankModelsId.ToTryInt64());
                    if (aIReankModels.AIModelType == AIModelType.Rerank)
                    {
                        switch (aIReankModels.AIType)
                        {
                            case AIType.AliRerank:
                            case AIType.BgeRerank:
                            default:
                                var systemPromptData = await rAGServicevice.GetRAGAliReankSystemPrompt("AIKmss-" + kmss.Id.ToString(),
                                await ollamaApiService.GetEmbedding(add.Content), add.Content, aiapp.MaxMatchesCount, (aiapp.Relevance / 100), aIReankModels.EndPoint, aIReankModels.ModelKey, aIReankModels.ModelName);
                                if (systemPromptData.Item1)
                                {
                                    OtherContents.Add(StringHelper.SubstringText(systemPromptData.Item2, aiapp.ContentLengthLimit));
                                    await _aIChatHistorysBindLogService.AddEdit(new TAIChatHistorysBindLog() { AIChatHistorysId = addAi.Id, LogContent = systemPromptData.Item2, LogType = AIChatHistorysBindLogEnums.Kmss });
                                    await output.WriteAsync("processmsg", $"找到 {systemPromptData.Item3.Count} 个相关文档");
                                }
                                break;
                        }
                    }
                }

            }
            return OtherContents;

        }
        /// <summary>
        /// AI文件url处理结果：把"文本上下文"和"二进制媒体附件"分桶归集，
        /// 让 <see cref="Add"/> 主流程能直接拼 <see cref="ChatMessage"/>，不需要在业务层再判 fileType。
        /// </summary>
        /// <param name="ExtractedTexts">文档解析后的文本片段（已按 ContentLengthLimit 截断），后续按 Token 预算再裁一次</param>
        /// <param name="MediaContents">图片/音频的 <see cref="AIContent"/>（内部是 DataContent + 显式 MIME），可直接送入模型</param>
        /// <param name="MediaCount">实际归集的媒体附件数，用于日志/统计</param>
        private sealed record FileHandleResult(List<string> ExtractedTexts, List<AIContent> MediaContents, int MediaCount);

        /// <summary>
        /// AI文件url处理。
        /// <para>
        /// 改造要点（相对旧实现）：
        /// 1. Bug 1 根治：所有文件并发 <see cref="Task.WhenAll(IEnumerable{Task})"/> 处理，主流程零 <c>.Result</c> 阻塞；
        /// 2. Bug 2 根治：<see cref="IModalityContentBuilder"/> 内部走 <c>FileHelper.GetRemoteFileAsync</c> 拿到 HTTP Content-Type，
        ///    再传给 <c>DataContent.LoadFromAsync(stream, mimeType)</c>，避免 MemoryStream 无 Name 导致 MIME 推断失败；
        /// 3. 音频模态：模型不支持时（<paramref name="enableAudioInput"/>=false）转成文本提示，不把二进制塞给不懂音频的模型；
        /// 4. 附件数量守卫：单条消息媒体附件超过 <see cref="ModalityOptions.MaxMediaAttachmentsPerMessage"/> 时后续忽略。
        /// </para>
        /// </summary>
        private async Task<FileHandleResult> AIFileUrlsHandle(TAIChatHistorys add, AIAppsDto aiapp, TAIChatHistorys addAi, bool enableAudioInput, IChatStreamOutput output, CancellationToken ct)
        {
            var extractedTexts = new List<string>();
            var mediaContents = new List<AIContent>();
            if (string.IsNullOrWhiteSpace(add.ContentFileUrls))
            {
                return new FileHandleResult(extractedTexts, mediaContents, 0);
            }

            var fileUrls = add.ContentFileUrls.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var fileNames = !string.IsNullOrWhiteSpace(add.FileNames)
                ? add.FileNames.Split(',', StringSplitOptions.RemoveEmptyEntries)
                : new string[fileUrls.Length];

            await output.WriteAsync("processmsg", $"正在处理 {fileUrls.Length} 个上传文件...");

            // 并发处理所有文件：单个失败已在 ModalityContentBuilder 内部转为 Error 结果，不会抛异常打断整批
            var tasks = new List<Task<ModalityContentResult>>(fileUrls.Length);
            for (int i = 0; i < fileUrls.Length; i++)
            {
                var url = fileUrls[i].Trim();
                var name = i < fileNames.Length ? fileNames[i].Trim() : null;
                tasks.Add(_modalityContentBuilder.BuildAsync(url, string.IsNullOrWhiteSpace(name) ? null : name, ct));
            }
            ModalityContentResult[] results;
            try
            {
                results = await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                throw; // 客户端中止要向上冒泡，让 Add 主流程走既有的取消处理路径
            }

            var options = ModalityOptions.Current;
            var fileContents = new StringBuilder();
            var mediaCount = 0;
            var headerWritten = false;

            for (int i = 0; i < results.Length; i++)
            {
                var r = results[i];
                var originalUrl = fileUrls[i].Trim();
                var originalName = i < fileNames.Length && !string.IsNullOrWhiteSpace(fileNames[i])
                    ? fileNames[i].Trim()
                    : r.FileName;

                // 音频但模型不支持：转文本提示，不把 DataContent 塞给不懂音频的模型（会 400）
                if (r.Kind == ModalityKind.Audio && !enableAudioInput)
                {
                    EnsureHeader();
                    fileContents.AppendLine($"\n文件名：【{originalName}】\n文件地址：【{originalUrl}】\n(当前对话模型不支持音频输入，请管理员在模型配置上勾选 AudioUnderstanding 能力后重试)");
                    continue;
                }

                // 媒体附件：数量守卫 + 直接归集
                if (r.Content != null)
                {
                    if (mediaCount >= options.MaxMediaAttachmentsPerMessage)
                    {
                        EnsureHeader();
                        fileContents.AppendLine($"\n文件名：【{originalName}】\n(超出单条消息媒体附件上限 {options.MaxMediaAttachmentsPerMessage}，已忽略)");
                        continue;
                    }
                    mediaContents.Add(r.Content);
                    mediaCount++;
                    EnsureHeader();
                    fileContents.AppendLine($"\n文件名：【{originalName}】\n文件地址：【{originalUrl}】\n({r.Kind} 附件，已作为多模态内容直接送入模型)");
                    continue;
                }

                // 文档 / 错误：拼进文本上下文
                EnsureHeader();
                if (!string.IsNullOrEmpty(r.Error))
                {
                    fileContents.AppendLine($"\n文件名：【{originalName}】\n文件地址：【{originalUrl}】\n({r.Error})");
                }
                else if (!string.IsNullOrEmpty(r.ExtractedText))
                {
                    fileContents.AppendLine($"\n文件名：【{originalName}】\n文件地址：【{originalUrl}】\n文件内容如下：");
                    fileContents.AppendLine(r.ExtractedText);
                }
            }

            void EnsureHeader()
            {
                if (headerWritten) return;
                fileContents.AppendLine("\n用户上传文件内容：");
                headerWritten = true;
            }

            if (fileContents.Length > 0)
            {
                await _aIChatHistorysBindLogService.AddEdit(new TAIChatHistorysBindLog()
                {
                    AIChatHistorysId = addAi.Id,
                    LogContent = fileContents.ToString(),
                    LogType = AIChatHistorysBindLogEnums.FileContent
                });
                extractedTexts.Add(StringHelper.SubstringText(fileContents.ToString(), aiapp.ContentLengthLimit));
            }
            return new FileHandleResult(extractedTexts, mediaContents, mediaCount);
        }

        /// <summary>
        /// 删除聊天记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> Delete(long id)
        {
            var like = await aIChatHistorysRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (like != null)
            {
                like.IsDelete = true;
                like.DeleteTime = DateTime.Now;
                aIChatHistorysRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }

        /// <summary>
        /// 获取AI应用的聊天选项配置（Token配置来自模型配置）
        /// </summary>
        /// <param name="aiapp"></param>
        /// <param name="aiModel"></param>
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
        /// 估算文本Token数（保守估算：1个字符≈1个Token，确保不超预算）
        /// </summary>
        private static int EstimateTokenCount(string text)
        {
            return string.IsNullOrEmpty(text) ? 0 : text.Length;
        }

        /// <summary>
        /// 按提问Token预算裁剪补充上下文（RAG/文件/联网搜索结果）：
        /// 预算 = 提问Token上限 - 系统提示词 - 用户问题 - 预留回答Token，
        /// 按顺序填充，不够放的内容截断后不再纳入后续内容，保证排在前面的高优先级内容完整。
        /// </summary>
        private static List<string> TrimContentsByAskTokenBudget(List<string> contents, string systemPrompt, string userContent, int maxAskPromptSize, int answerTokens)
        {
            var result = new List<string>();
            if (contents == null || contents.Count == 0)
            {
                return result;
            }
            // 模型未配置提问Token上限时没有预算依据，按“不裁剪”全部保留（原实现这里返回空集合，会把知识库、文件、联网搜索上下文静默丢弃）
            if (maxAskPromptSize <= 0)
            {
                return contents;
            }
            var budget = maxAskPromptSize - EstimateTokenCount(systemPrompt) - EstimateTokenCount(userContent) - answerTokens;
            foreach (var content in contents)
            {
                if (string.IsNullOrEmpty(content) || budget <= 0)
                {
                    continue;
                }
                var tokens = EstimateTokenCount(content);
                if (tokens <= budget)
                {
                    result.Add(content);
                    budget -= tokens;
                }
                else
                {
                    // 当前段放不下时截断保留，后续内容不再纳入，避免半截上下文干扰模型
                    result.Add(content.Substring(0, budget));
                    break;
                }
            }
            return result;
        }
        /// <summary>
        /// 异步消息压缩
        /// </summary>
        /// <returns></returns>
        private async Task<bool> MessageStoreCompaction(AIAppsDto aiapp, AIModelsDto aIModels, string thread_id)
        {
            //获取是否自动压缩
            if (!string.IsNullOrEmpty(thread_id) && aiapp.IsAIMessageCompaction)
            {
                using var db = new KevinDbContext();
                //获取需要压缩的记录  
                var msgData = await db.Set<TAIChatMessageStore>().Where(t => t.IsDelete == false && t.ThreadId == thread_id && t.IsCompaction == false).OrderByDescending(t => t.Timestamp).ToListAsync();
                var comDataDic = new Dictionary<string, List<string>>();
                var comDataList = new List<TAIChatMessageStore>();
                int userTurns = aiapp.ConversationTurnsExceed;
                foreach (var item in msgData)
                {
                    if (userTurns > 0)
                    {
                        if (item.Role == ChatRole.User.Value)
                        {
                            userTurns--;
                        }
                    }
                    else
                    {
                        comDataList.Add(item);
                        if (item.Role == ChatRole.User.Value)
                        {
                            item.IsCompaction = true;
                            item.UpdateTime = DateTime.Now;
                            comDataDic.Add(item.Timestamp?.ToString() ?? Guid.NewGuid().ToString(), comDataList.Where(t => !string.IsNullOrEmpty(t.SerializedMessage)).Select(t => t.SerializedMessage ?? "").ToList());
                            comDataList = new List<TAIChatMessageStore>();
                        }
                    }
                }
                if (comDataDic.Count > 0)
                {
                    #region 压缩

                    OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions()
                    {
                        Endpoint = new Uri(aIModels.EndPoint),
                        NetworkTimeout = TimeSpan.FromMinutes(10),// 设置网络超时时间为10分钟，适用于可能需要较长时间处理的请求
                        RetryPolicy = new ClientRetryPolicy(maxRetries: 2)//重试次数和延迟
                        {
                            // 可自定义延迟，默认指数退避
                        }
                    };
                    // 当无 keySecret（本地模型无鉴权）时，尝试使用不带凭据的客户端；若构造失败则给出明确异常提示  
                    var ai = new OpenAIClient(new ApiKeyCredential(string.IsNullOrWhiteSpace(aIModels.ModelKey) ? "local" : aIModels.ModelKey), openAIClientOptions);
                    var aiAgent = ai.GetChatClient(aIModels.ModelName).AsIChatClient().AsAIAgent(new ChatClientAgentOptions
                    {

                        Name = " 你是一款专业的压缩消息记录工具。",
                        Description = aiapp.AIMessageCompactionPrompt,
                        ChatOptions = GetAppChatOptions(aiapp, aIModels, aiapp.AIMessageCompactionPrompt)
                    });
                    var snowflakeIdService1 = new Kevin.SnowflakeId.Service.SnowflakeIdService();
                    var addList = new List<TAIChatMessageStoreCompaction>();
                    foreach (var item in comDataDic)
                    {
                        if (item.Value.Count > 0)
                        {
                            var content = new StringBuilder();
                            content.Append("内容如下：\n");
                            foreach (var itemValue in item.Value)
                            {
                                JsonElement msg = JsonSerializer.Deserialize<JsonElement>(itemValue);
                                string role = msg.GetProperty("Role").GetString() ?? "";
                                JsonElement contents = msg.GetProperty("Contents");
                                if (role.Contains("assistant"))
                                {
                                    foreach (JsonElement itemmsg in contents.EnumerateArray())
                                    {
                                        string type = itemmsg.GetProperty("$type").GetRawText() ?? "";
                                        if (type.Contains("reasoning"))
                                        {
                                            //  content.AppendLine("思考过程:" + itemmsg.GetProperty("Text").GetRawText());
                                        }
                                        else if (type.Contains("text"))
                                        {
                                            content.AppendLine("AI回复:" + itemmsg.GetProperty("Text").GetRawText());
                                        }
                                    }
                                }
                                else if (role.Contains("user"))
                                {
                                    foreach (JsonElement itemmsg in contents.EnumerateArray())
                                    {
                                        content.AppendLine("用户提问:" + itemmsg.GetProperty("Text").GetRawText());
                                    }
                                }
                                else if (role.Contains("tool"))   // ✅ 新增
                                {
                                    foreach (JsonElement itemmsg in contents.EnumerateArray())
                                    {
                                        if (itemmsg.GetProperty("$type").GetString() == "functionResult")
                                        {
                                            string result = itemmsg.GetProperty("Result").GetRawText() ?? "";
                                            string callId = itemmsg.GetProperty("CallId").GetRawText() ?? "";
                                            content.AppendLine($"工具执行结果：[{callId}] {result}");
                                        }
                                    }
                                }
                            }

                            var reslut = await aiAgent.RunAsync(content.ToString());
                            addList.Add(new TAIChatMessageStoreCompaction
                            {
                                Id = snowflakeIdService1.GetNextId(),
                                IsDelete = false,
                                CreateTime = DateTime.Now,
                                TenantId = aiapp.TenantId,
                                ThreadId = thread_id,
                                CompactionMessageText = item.Value.SerializeToJson(),
                                CompactionResultMessageText = reslut.Text.ToString(),
                            });
                        }
                    }
                    db.Set<TAIChatMessageStoreCompaction>().AddRange(addList.ToList());
                    db.SaveChanges();
                    #endregion
                }

            }
            return true;
        }

    }
}
