
using Common;
using kevin.AI.AgentFramework.Agent.KevinChatMessageStore;
using kevin.AI.AgentFramework.Const;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.ScriptRunners;
using kevin.AI.AgentFramework.Tools;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using kevin.Domain.Share.Enums;
using Kevin.AI.Dto;
using Kevin.Common.Extension;
using Kevin.RAG.Interfaces;
using Kevin.RAG.Ollama;
using Kevin.SignalR.Service;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using NetCore.Util;
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

        private readonly IAIChatMessageStoreCompactionService _aIChatMessageStoreCompactionService;

        public AIChatHistorysService() { }
        public AIChatHistorysService(IHttpContextAccessor _httpContextAccessor, IAIChatHistorysRp _aIChatHistorysRp,
            IAIAgentService _aIAgentService, IAIModelsService _aIModelsService, IAIPromptsService _aIPromptsService,
            IAIChatsService _aIChatsService, IAIAppsService _aIAppsService, IKevinAIChatMessageStore _kevinAIChatMessageStore,
            IRAGService _rAGService, IAIKmssService _aIKmssService, IOllamaApiService _ollamaApiService, ISignalRMsgService _signalRMsgService,
            IHttpClientFactory _httpClientFactory, IAIChatHistorysBindLogService _aIChatHistorysBindLogService, IAIChatMessageStoreCompactionService _aIChatMessageStoreCompactionService
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
            this._aIChatMessageStoreCompactionService = _aIChatMessageStoreCompactionService;
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
        /// 新建聊天
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<AIChatHistorysDto> Add(AIChatHistorysDto par, CancellationToken cancellationToken)
        {

            var aichas = await aIChatsService.GetDetails(par.AIChatsId);
            var aiapp = await aIAppsService.GetDetails(aichas.AppId);
            var count = await aIChatHistorysRp.Query().Where(t => t.IsDelete == false && t.AIChatsId == par.AIChatsId).CountAsync(cancellationToken);
            if (count >= aiapp.ChatMessageLimit)
            {
                throw new UserFriendlyException($"聊天记录已达上限{aiapp.ChatMessageLimit}条，为了更好的体验，建议新建聊天对话噢！");
            }
            if ((await aIAppsService.GetMyALLList()).Any(t => t.Id == aichas.AppId) == false)
            {
                throw new UserFriendlyException("智能体权限不足，无法使用");
            }
            var aIModels = await aIModelsService.GetDetails(aiapp.ChatModelID.ToTryInt64());
            var aIPrompts = await aIPromptsService.GetDetails(aiapp.AIPromptID);
            var add = par.MapTo<TAIChatHistorys>();
            add.Id = par.Id == default ? SnowflakeIdService.GetNextId() : par.Id;
            add.IsDelete = false;
            add.CreateTime = DateTime.Now;
            add.CreateUserId = CurrentUser.UserId;
            add.TenantId = CurrentUser.TenantId;
            add.IsSend = true;
            aIChatHistorysRp.Add(add);
            await aIChatHistorysRp.SaveChangesAsync(cancellationToken);
            //回复消息
            var addAi = new TAIChatHistorys();
            addAi.Id = SnowflakeIdService.GetNextId();
            addAi.IsDelete = false;
            addAi.CreateTime = DateTime.Now;
            addAi.CreateUserId = CurrentUser.UserId;
            addAi.TenantId = CurrentUser.TenantId;
            addAi.IsSend = false;
            addAi.AIChatsId = par.AIChatsId;
            string systemPrompt = SystemPrompt.SystemPromptText + "\n 智能体提示词规则：\n" + aIPrompts.Prompt;
            await _aIChatHistorysBindLogService.AddEdit(new TAIChatHistorysBindLog() { AIChatHistorysId = addAi.Id, LogContent = systemPrompt, LogType = AIChatHistorysBindLogEnums.SystemPrompt });
            List<string> OtherContents = new List<string>();

            if (aiapp.KmsId != default)
            {
                var ksmData = await KmsRag(add, aiapp, addAi);
                if (ksmData.Count > 0)
                {
                    OtherContents.AddRange(ksmData);
                }
            }
            #region 文件处理

            var ImgUrls = new List<string>();
            var aiFilData = await AIFileUrlsHandle(add, aiapp, addAi);
            if (aiFilData.Item1.Count > 0)
                OtherContents.AddRange(aiFilData.Item1);

            if (aiFilData.Item2.Count > 0)
                ImgUrls.AddRange(aiFilData.Item2);

            #endregion

            #region 联网搜索
            if (par.IsOnlineSearch)
            {
                await signalRMsgService.SendIdentityIdMsg("processmsg", add.Id.ToString(), "正在联网搜索....");
                var http = new HttpClientFunction(aIAgentService, _serviceProvider);
                var webseoData = await http.GetSeoAsync(add.Content, aIModels.EndPoint, aIModels.ModelName, aIModels.ModelKey);
                await _aIChatHistorysBindLogService.AddEdit(new TAIChatHistorysBindLog() { AIChatHistorysId = addAi.Id, LogContent = webseoData, LogType = AIChatHistorysBindLogEnums.WebSeo });
                OtherContents.Add(StringHelper.SubstringText(webseoData, aiapp.ContentLengthLimit));
            }
            #endregion
            ChatMessage mgs = new(ChatRole.User, [new TextContent($"{add.Content}"),
                        .. OtherContents.Where(t => !string.IsNullOrEmpty(t)).Select(t => new TextContent(t)).ToList(),
                        .. ImgUrls.Where(t => !string.IsNullOrEmpty(t)).Select(url => DataContent.LoadFromAsync(FileHelper.GetRemoteFileStreamAsync(url).Result).Result).ToList()]);
            var parAi = new { AIChatsId = add.AIChatsId, AppId = aiapp.Id, UserId = CurrentUser.UserId, AuthorizedDomains = aiapp.AuthorizedDomains, ContentLengthLimit = aiapp.ContentLengthLimit, IsSecurityIntercept = aiapp.IsSecurityIntercept };
            var chatAgOs = await aIAppsService.GetAppAIAgentOptions(aiapp, aIPrompts, systemPrompt, par, parAi);
            switch (aIModels.AIType)
            {
                case Domain.Share.Enums.AIType.OpenAI:
                case Domain.Share.Enums.AIType.ZhiPuAI:
                case Domain.Share.Enums.AIType.AzureOpenAI:
                default:
                    await signalRMsgService.SendIdentityIdMsg("processmsg", add.Id.ToString(), "正在结合相关信息思考....");
                    var reslut = (await aIAgentService.CreateOpenAIAgentAndSendMSG(new AISetting
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
                        StreameCallback = async (msg) =>
                        {
                            await signalRMsgService.SendIdentityIdMsg("aimsg", add.Id.ToString(), msg);
                        },
                        ToolStreameCallback = async (msg) =>
                        {
                            addAi.AIToolsContent += msg;
                            if (aiapp.IsToolLog)
                            {
                                await signalRMsgService.SendIdentityIdMsg("aIToolsContentMsg", add.Id.ToString(), StringHelper.SubstringText(msg, aiapp.ContentLengthLimit));
                            }
                        },
                        ReasoningStreameCallback = async (msg) =>
                        {
                            addAi.AIReasoningContent += msg;
                            if (aiapp.IsThinkingLog)
                            {
                                await signalRMsgService.SendIdentityIdMsg("aIReasoningContentMsg", add.Id.ToString(), StringHelper.SubstringText(msg, aiapp.ContentLengthLimit));
                            }
                        },
                    }, chatAgOs, mgs, cancellationToken: cancellationToken));
                    addAi.Content = reslut.Item2 ?? "";
                    if (reslut.Item3 != default)
                    {
                        addAi.CachedInputTokenCount = reslut.Item3.CachedInputTokenCount;
                        addAi.InputTokenCount = reslut.Item3.InputTokenCount;
                        addAi.OutputTokenCount = reslut.Item3.OutputTokenCount;
                        addAi.TotalTokenCount = reslut.Item3.TotalTokenCount;
                        addAi.ReasoningTokenCount = reslut.Item3.ReasoningTokenCount;
                    }
                    break;
            }
            var logdata = await _aIChatHistorysBindLogService.GetByIds(new List<long> { addAi.Id });
            aIChatHistorysRp.Add(addAi);
            await aIChatsService.UpdateNameAndMsg(par.AIChatsId, count == 1 ? par.Content : "", addAi.Content, cancellationToken);
            await aIChatHistorysRp.SaveChangesAsync(cancellationToken);
            Task.Run(() => { 
               MessageStoreCompaction(aiapp, aIModels, par.AIChatsId.ToString());
            }); 
            var data = addAi.MapTo<AIChatHistorysDto>();
            data.aIChatHistorysBindLogs = logdata;
            return data;
        }

        /// <summary>
        /// 知识库搜索
        /// </summary>
        private async Task<List<string>> KmsRag(TAIChatHistorys add, AIAppsDto aiapp, TAIChatHistorys addAi)
        {
            var OtherContents = new List<string>();
            await signalRMsgService.SendIdentityIdMsg("processmsg", add.Id.ToString(), "正在查询知识库....");
            var kmss = await aIKmssService.GetDetails(aiapp.KmsId.GetValueOrDefault());
            if (kmss != default)
            {
                if (kmss.aIModelsId != default)
                {
                    var aimode = await aIModelsService.GetDetails(kmss.aIModelsId.GetValueOrDefault());
                    if (aimode?.AIModelType == AIModelType.Embedding)
                    {
                        ollamaApiService = new OllamaApiService(aimode.EndPoint, aimode.ModelName, aimode.ModelKey);
                    }
                }
                await signalRMsgService.SendIdentityIdMsg("processmsg", add.Id.ToString(), "正在检索相关文档...");
                if (kmss.aIRerankModelsId == default)
                {
                    var systemPromptData = await rAGServicevice.GetRAGSystemPrompt("AIKmss-" + kmss.Id.ToString(),
                        await ollamaApiService.GetEmbedding(add.Content), add.Content, false, aiapp.MaxMatchesCount, (aiapp.Relevance / 100));
                    if (systemPromptData.Item1)
                    {
                        await _aIChatHistorysBindLogService.AddEdit(new TAIChatHistorysBindLog() { AIChatHistorysId = addAi.Id, LogContent = systemPromptData.Item2, LogType = AIChatHistorysBindLogEnums.Kmss });
                        OtherContents.Add(StringHelper.SubstringText(systemPromptData.Item2, aiapp.ContentLengthLimit));
                        await signalRMsgService.SendIdentityIdMsg("processmsg", add.Id.ToString(), $"找到 {systemPromptData.Item3.Count} 个相关文档");
                    }
                }
                else
                {
                    var aIReankModels = await aIModelsService.GetDetails(kmss.aIRerankModelsId.ToTryInt64());
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
                                    await signalRMsgService.SendIdentityIdMsg("processmsg", add.Id.ToString(), $"找到 {systemPromptData.Item3.Count} 个相关文档");
                                }
                                break;
                        }
                    }
                }

            }
            return OtherContents;

        }
        /// <summary>
        /// AI文件url处理
        /// </summary>
        private async Task<(List<string>, List<string>)> AIFileUrlsHandle(TAIChatHistorys add, AIAppsDto aiapp, TAIChatHistorys addAi)
        {
            var OtherContents = new List<string>();
            var ImgUrls = new List<string>();
            if (!string.IsNullOrWhiteSpace(add.ContentFileUrls))
            {
                var fileUrls = add.ContentFileUrls.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var fileNames = !string.IsNullOrWhiteSpace(add.FileNames)
                    ? add.FileNames.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    : new string[fileUrls.Length];

                await signalRMsgService.SendIdentityIdMsg("processmsg", add.Id.ToString(), $"正在处理 {fileUrls.Length} 个上传文件...");

                var fileContents = new StringBuilder();
                for (int i = 0; i < fileUrls.Length; i++)
                {
                    var fileUrl = fileUrls[i].Trim();
                    var fileName = i < fileNames.Length ? fileNames[i].Trim() : Path.GetFileName(fileUrl);

                    try
                    {
                        await signalRMsgService.SendIdentityIdMsg("processmsg", add.Id.ToString(), $"正在提取文件内容: {fileName}");
                        var fileType = FileHelper.DetermineFileType(fileName);
                        if (fileType != "image")
                        {
                            var stream = await FileHelper.GetRemoteFileStreamAsync(fileUrl);
                            string content = "";
                            switch (fileType)
                            {
                                case "excel":
                                    content = ExcelReader.ReadExcelToMarkdown(stream, fileName);
                                    break;
                                case "pdf":
                                    content = PDFReader.ReadPdfToMarkdown(stream);
                                    break;
                                case "word":
                                    content = WordReader.ReadParagraphs(stream);
                                    break;
                                case "html":
                                    content = await HtmlReader.ExtractTextFromStreamAsync(stream);
                                    break;
                                case "markdown":
                                    content = TextStreamReader.ReadMarkdownFromStream(stream).RawContent;
                                    break;
                                case "text":
                                default:
                                    content = TextStreamReader.ReadTextFromStream(stream);
                                    break;
                            }
                            if (i == 0)
                            {
                                fileContents.AppendLine("\n用户上传文件内容：");
                            }
                            fileContents.AppendLine($"\n文件名：【{fileName}】\n文件地址：【{fileUrl}】\n文件内容如下：");
                            fileContents.AppendLine(content);
                        }
                        else
                        {
                            ImgUrls.Add(fileUrl);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (i == 0)
                        {
                            fileContents.AppendLine("\n用户上传文件内容：");
                        }
                        fileContents.AppendLine($"\n文件名：【{fileName}】\n文件地址：【{fileUrl}】\n(读取失败: {ex.Message})");
                    }
                }
                await _aIChatHistorysBindLogService.AddEdit(new TAIChatHistorysBindLog() { AIChatHistorysId = addAi.Id, LogContent = fileContents.ToString(), LogType = AIChatHistorysBindLogEnums.FileContent });
                OtherContents.Add(StringHelper.SubstringText(fileContents.ToString(), aiapp.ContentLengthLimit));
            }
            return (OtherContents, ImgUrls);
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
                        ChatOptions = new Microsoft.Extensions.AI.ChatOptions
                        {
                            MaxOutputTokens = aiapp.AnswerTokens,
                            Temperature = (float)(aiapp.Temperature / 100),
                            ResponseFormat = ChatResponseFormat.Text,
                            Instructions = aiapp.AIMessageCompactionPrompt
                        },
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
                                if (role == "assistant")
                                {
                                    foreach (JsonElement itemmsg in contents.EnumerateArray())
                                    {
                                        string type = itemmsg.GetProperty("$type").GetRawText() ?? "";
                                        if (type == "reasoning")
                                        {
                                            content.AppendLine("思考过程:" + itemmsg.GetProperty("Text").GetRawText());
                                        }
                                        else if (type == "text")
                                        {
                                            content.AppendLine("AI回复:" + itemmsg.GetProperty("Text").GetRawText());
                                        }
                                    }
                                }
                                else if (role == "user")
                                {
                                    foreach (JsonElement itemmsg in contents.EnumerateArray())
                                    {
                                        content.AppendLine("用户对话:" + itemmsg.GetProperty("Text").GetRawText());
                                    }
                                }
                                else if (role == "tool")   // ✅ 新增
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
                                CreateUserId = aiapp.CreateUserId,
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
