using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.TextLimits;
using kevin.Cache.Service;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using Kevin.log4Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// OpenAI 协议兼容接口的协议异常：携带 HTTP 状态码与 OpenAI 错误类型，
    /// 由 Web 层在入口处统一转为 OpenAI 错误响应（JSON 或流式 error 帧）。
    /// </summary>
    public class OpenAIProtocolException : Exception
    {
        public int StatusCode { get; }
        public string ErrType { get; }
        public string? Code { get; }

        public OpenAIProtocolException(int statusCode, string message, string errType = "invalid_request_error", string? code = null) : base(message)
        {
            StatusCode = statusCode;
            ErrType = errType;
            Code = code;
        }
    }

    /// <summary>
    /// OpenAI 协议兼容接口的聊天服务实现：
    /// 解析 OpenAI Chat Completions 请求（校验、智能体定位、会话衔接、工具透传），
    /// 复用智能体处理管线（<see cref="IAIChatHistorysService.AddWithStreamOutput"/>）产出回答。
    /// <para>
    /// 流式分片由本服务自建 chat.completion.chunk 适配器直接写入当前 HTTP 响应
    /// （与 <c>AddSSE</c> 的服务内输出同一模式）；非流式组装完整结果返回给 Web 层。
    /// </para>
    /// </summary>
    public class OpenAICompatibleChatService : BaseService, IOpenAICompatibleChatService
    {
        public IAIChatHistorysService AIChatHistorysService { get; set; }
        public IAIChatsService AIChatsService { get; set; }
        public IAIAppsService AIAppsService { get; set; }
        public ICacheService CacheService { get; set; }

        public OpenAICompatibleChatService(
            IHttpContextAccessor httpContextAccessor,
            IAIChatHistorysService aIChatHistorysService,
            IAIChatsService aIChatsService,
            IAIAppsService aIAppsService,
            ICacheService cacheService) : base(httpContextAccessor)
        {
            AIChatHistorysService = aIChatHistorysService;
            AIChatsService = aIChatsService;
            AIAppsService = aIAppsService;
            CacheService = cacheService;
        }

        /// <summary>非流式：分片全部忽略，从落库的最终回复组装完整结果（含透传工具调用与 Token 用量）</summary>
        public async Task<OpenAICompatibleChatOutcome> ChatAsync(OpenAICompatibleChatRequest req, string? headerConversationId, CancellationToken cancellationToken)
        {
            var par = await PrepareAsync(req, headerConversationId);
            var setting = OpenAICompatibleSetting.Current;
            var output = new OpenAICompatibleChatStreamOutput(GetResponseOrThrow(), req.Model!, setting.ReasoningOutput, streamEnabled: false);
            var result = await AIChatHistorysService.AddWithStreamOutput(par, _ => output, cancellationToken);
            var content = result?.Content ?? "";
            if (string.IsNullOrWhiteSpace(content) && !string.IsNullOrWhiteSpace(result?.FailReason))
            {
                // 失败路径没有可展示的正文：按错误回传，客户端可据 message 提示重试
                throw new OpenAIProtocolException(500, result!.FailReason!, "server_error");
            }
            // 透传工具调用轮：按 OpenAI 协议以 message.tool_calls + finish_reason=tool_calls 返回，
            // 客户端执行完工具后把 role=tool 结果消息发回继续对话
            var toolCalls = output.CollectedToolCalls;
            return new OpenAICompatibleChatOutcome
            {
                Content = content,
                ReasoningContent = setting.ReasoningOutput && !string.IsNullOrWhiteSpace(result?.AIReasoningContent) ? result!.AIReasoningContent : null,
                ToolCalls = toolCalls.Count > 0 ? toolCalls.ToList() : null,
                PromptTokens = result?.InputTokenCount ?? 0,
                CompletionTokens = result?.OutputTokenCount ?? 0,
                TotalTokens = result?.TotalTokenCount ?? ((result?.InputTokenCount ?? 0) + (result?.OutputTokenCount ?? 0)),
            };
        }

        /// <summary>流式：分片按 chat.completion.chunk 帧实时写入当前响应，结束补 finish 帧 + [DONE]</summary>
        public async Task ChatStreamAsync(OpenAICompatibleChatRequest req, string? headerConversationId, CancellationToken cancellationToken)
        {
            var setting = OpenAICompatibleSetting.Current;
            var output = new OpenAICompatibleChatStreamOutput(GetResponseOrThrow(), req.Model!, setting.ReasoningOutput, streamEnabled: true);
            try
            {
                var par = await PrepareAsync(req, headerConversationId);
                await AIChatHistorysService.AddWithStreamOutput(par, _ => output, cancellationToken);
                await output.CompleteAsync();
            }
            catch (OpenAIProtocolException ex)
            {
                // 流已开始只能按 OpenAI 流式约定塞 error 数据帧；未开始则抛回 Web 层写 JSON 错误
                if (!output.StreamStarted) throw;
                await output.WriteErrorAsync(ex.Message);
            }
            catch (UserFriendlyException ex)
            {
                if (!output.StreamStarted) throw new OpenAIProtocolException(400, ex.Message);
                await output.WriteErrorAsync(ex.Message);
            }
            catch (OperationCanceledException)
            {
                // 客户端主动断开：响应已无意义，静默结束（管线内部会自行落一条失败记录供网页端查阅）
            }
            catch (Exception ex)
            {
                LogHelper.logger.Error("OpenAI兼容接口流式处理异常:" + ex.Message, ex);
                if (!output.StreamStarted) throw new OpenAIProtocolException(500, "服务内部异常：" + ex.Message, "server_error");
                await output.WriteErrorAsync("服务内部异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 请求准备：校验 → 提取本轮提问/工具结果 → 智能体定位 → 会话衔接 → 工具透传解析，
        /// 产出与网页端同构的聊天入参。
        /// </summary>
        private async Task<AIChatHistorysDto> PrepareAsync(OpenAICompatibleChatRequest req, string? headerConversationId)
        {
            if (req?.Messages == null || req.Messages.Count == 0)
            {
                throw new OpenAIProtocolException(400, "messages 不能为空");
            }
            if (string.IsNullOrWhiteSpace(req.Model))
            {
                throw new OpenAIProtocolException(400, "model 不能为空：请传智能体名称或 Id（可从 GET /v1/models 获取）");
            }

            // 最后一条 user 消息即本轮提问；首条 user 消息文本用于"无任何会话标识"时的窗口指纹
            var lastUser = FindLastUserMessage(req.Messages);
            // 工具结果续轮：尾部连续的 role=tool 消息（客户端执行完上一轮 tool_calls 回传的结果）
            var toolResults = CollectTrailingToolResults(req.Messages);
            // 既没有 user 文本/图片、也没有工具结果时，没有可处理的内容
            var hasUserContent = lastUser != null && (!string.IsNullOrWhiteSpace(lastUser.Text) || lastUser.ImageUrls.Count > 0);
            if (!hasUserContent && toolResults.Count == 0)
            {
                throw new OpenAIProtocolException(400, "messages 中未找到有效的 user 消息内容");
            }
            var firstUserText = FindFirstUserText(req.Messages);

            // 智能体定位 + 当前身份可用性校验（密钥绑定用户的权限范围）
            var myApps = await AIAppsService.GetMyALLList();
            var app = ResolveAgent(myApps, req.Model!);
            if (app == null)
            {
                throw new OpenAIProtocolException(404, $"模型 {req.Model} 不存在或当前身份无权使用（可用列表见 GET /v1/models）", code: "model_not_found");
            }

            // 会话衔接：命中已有会话则续聊，否则自动新建（API 会话对网页端隐藏）
            var aiChatsId = await ResolveOrCreateSessionAsync(app, req, headerConversationId, firstUserText);

            // 透传工具（OpenAI tools）：本轮附带的调用方工具声明，网关只透传不执行
            var externalTools = ParseExternalTools(req.Tools);

            var par = new AIChatHistorysDto
            {
                AIChatsId = aiChatsId,
                // 工具续轮：本轮真正的模型输入是工具结果（不是新提问），提问行用占位文案，避免会话里落一条重复问题
                Content = toolResults.Count > 0 ? BuildToolTurnPlaceholder(toolResults) : lastUser!.Text,
                IsSend = true,
                // 推荐问题是给网页端的额外模型调用，API 场景关闭
                IsRecommendQuestion = false,
                // 工具续轮不是新的用户提问：不携带上一轮的图片
                ContentFileUrls = toolResults.Count == 0 && (lastUser?.ImageUrls.Count ?? 0) > 0 ? string.Join(",", lastUser!.ImageUrls) : null,
                ExternalTools = externalTools,
                ExternalMessages = toolResults.Count > 0 ? toolResults : null,
            };
            // tool_choice 只在透传工具场景生效：避免影响智能体自身工具能力的默认行为
            if (externalTools != null)
            {
                ApplyToolChoice(req.ToolChoice, par);
            }
            return par;
        }

        /// <summary>取当前 HTTP 响应上下文（与 AddSSE 同模式）；非 HTTP 上下文调用直接拒绝</summary>
        private HttpResponse GetResponseOrThrow()
        {
            var response = HttpContextAccessor.HttpContext?.Response;
            if (response == default)
            {
                throw new UserFriendlyException("OpenAI 兼容接口输出需要在 HTTP 请求上下文中调用");
            }
            return response;
        }

        #region 会话衔接

        /// <summary>
        /// 会话解析：按会话键查 Redis 映射 → 校验会话存在则续期复用；否则新建隐藏会话并写入映射。
        /// <para>Redis 抖动只退化为"本轮新建会话"，不打断对话。</para>
        /// </summary>
        private async Task<long> ResolveOrCreateSessionAsync(AIAppsDto app, OpenAICompatibleChatRequest req, string? headerConversationId, string firstUserText)
        {
            var conversationKey = BuildConversationKeyRaw(req, headerConversationId, firstUserText);
            var cacheKey = BuildSessionCacheKey(app.Id, conversationKey);

            var cachedChatId = await TryGetSessionAsync(cacheKey);
            if (cachedChatId > 0)
            {
                try
                {
                    // 校验会话仍存在且属于当前租户；失效（被删除/跨租户/Redis 脏数据）则删映射走新建
                    await AIChatsService.GetDetails(cachedChatId);
                    TouchSession(cacheKey, cachedChatId);
                    return cachedChatId;
                }
                catch (Exception ex)
                {
                    LogHelper.logger.Info($"OpenAI兼容接口会话映射已失效（chatId={cachedChatId}，按新会话处理）：{ex.Message}");
                    TryRemoveSession(cacheKey);
                }
            }

            var created = await AIChatsService.Add(new AIChatsDto
            {
                AppId = app.Id,
                Name = BuildSessionName(firstUserText),
                IsHidden = OpenAICompatibleSetting.Current.AIChatIsHidden,
                // 新建后同请求内立刻发首条消息：不做后台预热，避免预热与首条消息并发重复加载 MCP/技能
                IsAsyncLoadAgent = false,
            });
            var chatId = created.AIChatsId;
            TouchSession(cacheKey, chatId);
            return chatId;
        }

        /// <summary>会话键原始值：请求头 &gt; 扩展字段（下划线/驼峰）&gt; OpenAI 标准 user 字段 &gt; 首条 user 消息指纹</summary>
        private string BuildConversationKeyRaw(OpenAICompatibleChatRequest req, string? headerConversationId, string firstUserText)
        {
            if (!string.IsNullOrWhiteSpace(headerConversationId)) return "hdr:" + headerConversationId.Trim();
            if (!string.IsNullOrWhiteSpace(req.ConversationId)) return "ext:" + req.ConversationId.Trim();
            if (!string.IsNullOrWhiteSpace(req.ConversationIdCamel)) return "ext:" + req.ConversationIdCamel.Trim();
            if (!string.IsNullOrWhiteSpace(req.User)) return "usr:" + req.User.Trim();
            // 指纹兜底：同一聊天窗口首条用户消息恒定，哈希后即可跨请求稳定识别同一会话；
            // 客户端重发完整历史时天然满足，历史被裁剪导致指纹变化时按新会话处理（可传会话标识避免）
            return "fp:" + Sha256Hex(firstUserText);
        }

        /// <summary>会话映射 Redis 键：按"用户 + 智能体"隔离，防止不同用户/智能体的指纹碰撞串会话</summary>
        private string BuildSessionCacheKey(long appId, string conversationKeyRaw)
            => $"openai_compatible:chat:{CurrentUser.UserId}:{appId}:{Sha256Hex(conversationKeyRaw)}";

        private async Task<long> TryGetSessionAsync(string cacheKey)
        {
            try
            {
                var cached = await CacheService.GetStringAsync(cacheKey);
                return long.TryParse(cached, out var chatId) ? chatId : 0;
            }
            catch (Exception ex)
            {
                LogHelper.logger.Info("OpenAI兼容接口读取会话映射失败（按新会话处理）：" + ex.Message);
                return 0;
            }
        }

        /// <summary>写入/续期会话映射（滑动过期：每次命中都按配置时长重新计时）</summary>
        private void TouchSession(string cacheKey, long chatId)
        {
            try
            {
                var ttl = TimeSpan.FromHours(Math.Max(1, OpenAICompatibleSetting.Current.SessionExpirationHours));
                CacheService.SetString(cacheKey, chatId.ToString(), ttl);
            }
            catch (Exception ex)
            {
                // 写映射失败只影响下次能否复用会话，不影响本轮对话
                LogHelper.logger.Info("OpenAI兼容接口写入会话映射失败（不影响本轮对话）：" + ex.Message);
            }
        }

        private void TryRemoveSession(string cacheKey)
        {
            try
            {
                CacheService.Remove(cacheKey);
            }
            catch (Exception ex)
            {
                LogHelper.logger.Info("OpenAI兼容接口删除会话映射失败：" + ex.Message);
            }
        }

        private static string BuildSessionName(string firstUserText)
        {
            var name = (firstUserText ?? "").Trim().ReplaceLineEndings(" ");
            if (string.IsNullOrEmpty(name)) return "API对话";
            return name.Length <= CharLimits.OpenAISessionName ? name : name.Substring(0, CharLimits.OpenAISessionName);
        }

        #endregion

        #region 协议解析

        /// <summary>智能体解析：model 默认即智能体名称；兼容智能体 Id 与 agent-{id} 历史写法（名称先区分大小写、再忽略大小写）</summary>
        private static AIAppsDto? ResolveAgent(List<AIAppsDto> myApps, string model)
        {
            var name = model.Trim();
            // 主用法：model 即智能体名称（GET /v1/models 的 id 字段），按名称定位智能体
            var byName = myApps.FirstOrDefault(t => string.Equals(t.Name?.Trim(), name, StringComparison.Ordinal))
                ?? myApps.FirstOrDefault(t => string.Equals(t.Name?.Trim(), name, StringComparison.OrdinalIgnoreCase));
            if (byName != null) return byName;
            // 兼容旧用法：model 传智能体 Id（雪花 id 字符串）或 agent-{id} 写法
            var idText = name.StartsWith("agent-", StringComparison.OrdinalIgnoreCase) ? name.Substring("agent-".Length) : name;
            if (long.TryParse(idText, out var agentId))
            {
                return myApps.FirstOrDefault(t => t.Id == agentId);
            }
            return null;
        }

        private static CapturedUserMessage? FindLastUserMessage(List<OpenAICompatibleMessage> messages)
        {
            for (var i = messages.Count - 1; i >= 0; i--)
            {
                var message = messages[i];
                if (message == null || !string.Equals(message.Role, "user", StringComparison.OrdinalIgnoreCase)) continue;
                var (text, imageUrls) = ExtractContent(message.Content);
                return new CapturedUserMessage(text, imageUrls);
            }
            return null;
        }

        private static string FindFirstUserText(List<OpenAICompatibleMessage> messages)
        {
            foreach (var message in messages)
            {
                if (message == null || !string.Equals(message.Role, "user", StringComparison.OrdinalIgnoreCase)) continue;
                var (text, imageUrls) = ExtractContent(message.Content);
                if (string.IsNullOrWhiteSpace(text) && imageUrls.Count > 0)
                {
                    // 纯图片消息：用图片地址参与指纹，避免所有纯图片会话指纹相同
                    return "img:" + imageUrls[0];
                }
                if (!string.IsNullOrWhiteSpace(text)) return text;
            }
            return "";
        }

        /// <summary>
        /// 收集尾部待决的工具结果消息：从末尾往前取连续 role=tool 的消息（恢复原顺序返回）。
        /// 再往前的 assistant 工具调用帧已随上一轮回复入库，历史加载时会先行下发，这里不再重复携带；
        /// 多批调用时只有最后一批（最后一个 assistant 调用帧之后的 tool 消息）属于本轮待决。
        /// </summary>
        private static List<AIExternalMessageDto> CollectTrailingToolResults(List<OpenAICompatibleMessage> messages)
        {
            var results = new List<AIExternalMessageDto>();
            for (var i = messages.Count - 1; i >= 0; i--)
            {
                var message = messages[i];
                if (message == null || !string.Equals(message.Role, "tool", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                if (string.IsNullOrEmpty(message.ToolCallId))
                {
                    // 缺 tool_call_id 的结果无法关联 assistant 调用帧，忽略该条继续扫描
                    continue;
                }
                var (text, _) = ExtractContent(message.Content);
                results.Add(new AIExternalMessageDto
                {
                    Role = "tool",
                    ToolCallId = message.ToolCallId.Trim(),
                    Content = text,
                });
            }
            results.Reverse();
            return results;
        }

        /// <summary>
        /// 解析请求 tools：只接受 type=function 且 name 非空的条目（parameters 保留 JSON Schema 原文）；
        /// 返回 null 表示本轮没有工具透传需求（行为与改造前完全一致）。
        /// </summary>
        private static List<AIExternalToolDto>? ParseExternalTools(List<OpenAICompatibleTool>? tools)
        {
            if (tools == null || tools.Count == 0) return null;
            var list = new List<AIExternalToolDto>(tools.Count);
            foreach (var tool in tools)
            {
                if (tool == null || !string.Equals(tool.Type, "function", StringComparison.OrdinalIgnoreCase)) continue;
                var name = tool.Function?.Name?.Trim();
                if (string.IsNullOrEmpty(name)) continue;
                list.Add(new AIExternalToolDto
                {
                    Name = name,
                    Description = tool.Function!.Description,
                    ParametersJson = tool.Function.Parameters is { } parameters && parameters.ValueKind != JsonValueKind.Undefined
                        ? parameters.GetRawText()
                        : null,
                });
            }
            return list.Count > 0 ? list : null;
        }

        /// <summary>
        /// tool_choice → 本轮工具选择策略："none"/"required" 直接映射，
        /// {"type":"function","function":{"name":"..."}} 映射为指定函数，其余（含 auto/缺省）按默认 auto。
        /// </summary>
        private static void ApplyToolChoice(JsonElement? toolChoice, AIChatHistorysDto par)
        {
            if (toolChoice == null) return;
            var choice = toolChoice.Value;
            if (choice.ValueKind == JsonValueKind.String)
            {
                var value = choice.GetString()?.Trim().ToLowerInvariant();
                if (value == "none") par.ToolMode = "none";
                else if (value == "required") par.ToolMode = "required";
                else par.ToolMode = "auto";
                return;
            }
            var function = default(JsonElement);
            if (choice.ValueKind == JsonValueKind.Object
                && choice.TryGetProperty("function", out function)
                && function.ValueKind == JsonValueKind.Object
                && function.TryGetProperty("name", out var nameEl)
                && nameEl.ValueKind == JsonValueKind.String
                && !string.IsNullOrWhiteSpace(nameEl.GetString()))
            {
                par.ToolMode = "function";
                par.ToolModeFunctionName = nameEl.GetString()!.Trim();
            }
        }

        /// <summary>工具续轮提问行占位文案：让会话记录一眼能看出这轮是工具结果回传，而不是重复提问</summary>
        private static string BuildToolTurnPlaceholder(List<AIExternalMessageDto> toolResults)
        {
            var preview = toolResults.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.Content))?.Content ?? "";
            preview = preview.ReplaceLineEndings(" ").Trim();
            if (preview.Length > CharLimits.ToolTurnPlaceholderPreview) preview = preview.Substring(0, CharLimits.ToolTurnPlaceholderPreview);
            return string.IsNullOrEmpty(preview) ? "（外部工具结果回传）" : $"（外部工具结果回传）{preview}";
        }

        /// <summary>
        /// 解析消息内容：content 可能是字符串（常规）或内容块数组（多模态）。
        /// 文本块（text/input_text）合并为正文；图片块（image_url）只接受 http(s) 远程地址
        /// （data: 内联图片会随请求体膨胀且不符合管线取数方式，直接忽略），交给管线的文件处理复用既有模态能力。
        /// </summary>
        private static (string Text, List<string> ImageUrls) ExtractContent(JsonElement content)
        {
            var text = new StringBuilder();
            var imageUrls = new List<string>();
            switch (content.ValueKind)
            {
                case JsonValueKind.String:
                    text.Append(content.GetString());
                    break;
                case JsonValueKind.Array:
                    foreach (var part in content.EnumerateArray())
                    {
                        if (part.ValueKind != JsonValueKind.Object) continue;
                        var type = part.TryGetProperty("type", out var typeEl) && typeEl.ValueKind == JsonValueKind.String ? typeEl.GetString() : null;
                        if (string.Equals(type, "text", StringComparison.OrdinalIgnoreCase) || string.Equals(type, "input_text", StringComparison.OrdinalIgnoreCase))
                        {
                            if (part.TryGetProperty("text", out var textEl) && textEl.ValueKind == JsonValueKind.String)
                            {
                                if (text.Length > 0) text.Append('\n');
                                text.Append(textEl.GetString());
                            }
                        }
                        else if (string.Equals(type, "image_url", StringComparison.OrdinalIgnoreCase))
                        {
                            // 兼容 {"image_url":{"url":...}} 与 {"image_url":"..."} 两种写法
                            string? url = null;
                            if (part.TryGetProperty("image_url", out var imageEl))
                            {
                                if (imageEl.ValueKind == JsonValueKind.Object && imageEl.TryGetProperty("url", out var urlEl) && urlEl.ValueKind == JsonValueKind.String)
                                {
                                    url = urlEl.GetString();
                                }
                                else if (imageEl.ValueKind == JsonValueKind.String)
                                {
                                    url = imageEl.GetString();
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(url)
                                && (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
                            {
                                imageUrls.Add(url);
                            }
                        }
                    }
                    break;
            }
            return (text.ToString(), imageUrls);
        }

        private static string Sha256Hex(string text)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text ?? ""));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        private sealed record CapturedUserMessage(string Text, List<string> ImageUrls);

        #endregion
    }
}
