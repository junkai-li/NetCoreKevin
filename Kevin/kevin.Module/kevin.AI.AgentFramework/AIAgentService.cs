using HttpMataki.NET.Auto;
using kevin.AI.AgentFramework.Agent.KevinChatMessageStore;
using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces;
using Kevin.AI.Dto;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Responses;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Reflection;
using System.Text;
using System.Text.Json;
namespace kevin.AI.AgentFramework
{
    /// <summary>
    /// AI服务
    /// </summary>
    public class AIAgentService : IAIAgentService
    {
        private readonly ILogger<AIAgentService> Ailogger;
        public AIAgentService(ILogger<AIAgentService> _logger)
        {
            Ailogger = _logger;
        }
        /// <summary>
        /// 创建代理并发送消息
        /// </summary>
        /// <param name="aISetting"></param>
        /// <param name="chatClientAgentOptions"></param>
        /// <param name="msg"></param>
        /// <param name="OtherContents">其他内容：用来存放一些需要传递给代理的内容 比如文件内容 互联网信息等</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<(AIAgent, string, TokenConsumptionInfo)> CreateOpenAIAgentAndSendMSG(AISetting aISetting, ChatClientAgentOptions chatClientAgentOptions, ChatMessage messages, CancellationToken cancellationToken = default)
            => CreateOpenAIAgentAndSendMSG(aISetting, chatClientAgentOptions, new[] { messages }, cancellationToken);

        /// <summary>
        /// 创建代理并发送消息（多消息重载）：支持 OpenAI function calling 续轮，把调用方回传的工具结果作为本轮输入
        /// </summary>
        /// <param name="aISetting"></param>
        /// <param name="chatClientAgentOptions"></param>
        /// <param name="messages">本轮发送给模型的消息集合：普通提问为单条 user 消息，工具续轮为工具结果（tool）消息</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<(AIAgent, string, TokenConsumptionInfo)> CreateOpenAIAgentAndSendMSG(AISetting aISetting, ChatClientAgentOptions chatClientAgentOptions, IEnumerable<ChatMessage> messages, CancellationToken cancellationToken = default)
        {

            cancellationToken.ThrowIfCancellationRequested();//是否已经中止，若已请求取消则抛出异常
            // 流式/非流式分支只消费一次入参，先物化避免 IEnumerable 被延迟多次枚举
            var turnMessages = messages as IReadOnlyList<ChatMessage> ?? messages.ToList();
            if (aISetting.IsHttpLog)
            {
                HttpClientAutoInterceptor.StartInterception();
            }
            #region AI工具
            if (!aISetting.IsAITools && !aISetting.IsMcpTools && !aISetting.IsMemory && !aISetting.IsKnowledgeBase)
            {
                if (chatClientAgentOptions.ChatOptions != default)
                {
                    // 外部声明型工具（function calling 透传）不受能力开关影响：只声明不执行、由调用方执行，清掉将永远收不到 tool_calls
                    chatClientAgentOptions.ChatOptions.Tools = KeepDeclaredOnlyTools(chatClientAgentOptions.ChatOptions.Tools);
                }
            }
            #endregion

            #region AI技能
            if (!aISetting.IsAISkills)
            {
                chatClientAgentOptions.AIContextProviders = default;
            }
            #endregion

            var maxRetries = aISetting.MaxRetries;
            // Auto模式：确保重试次数足够尝试所有备选模型
            if (aISetting.FallbackModels?.Count > 0)
            {
                maxRetries = Math.Max(maxRetries, aISetting.FallbackModels.Count);
            }
            var retries = 1;
        aiRun:
            aISetting.AttemptCount = retries;
            // 工具调用收集器：收集本轮全部调用（外部+自带），每次模型尝试（含失败重试/切换模型）都重置，避免上一轮残留混入本轮回传。
            // 轮结束时分类处理：外部工具调用回传调用方执行；自带工具在"终止轮"（存在外部调用交还、框架跳过全部执行）做补偿执行
            var collectedToolCalls = new Dictionary<string, (string Name, Dictionary<string, object?> Arguments)>(StringComparer.Ordinal);
            // 框架已执行完成的调用集合：补偿执行时据此跳过，避免重复执行
            var completedToolCallIds = new HashSet<string>(StringComparer.Ordinal);
            // openAIClientOptions 必须在 aiRun 内创建，确保模型切换后使用新的 Endpoint
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions()
            {
                Endpoint = new Uri(aISetting.AIUrl),
                NetworkTimeout = TimeSpan.FromMinutes(aISetting.NetworkTimeout),// 设置网络超时时间为10分钟，适用于可能需要较长时间处理的请求
                // SDK 内置重试只处理 408/429/5xx，400 这类参数错误不会在它内部重试；
                // 这里保持为单请求重试次数（不能用 maxRetries）：外层 goto aiRun 还会整轮重试，两者相乘会放大成 maxRetries^2 次请求
                RetryPolicy = new ClientRetryPolicy(maxRetries: aISetting.MaxRetries)//重试次数和延迟
                {
                    // 可自定义延迟，默认指数退避
                }
            };
            // 当无 keySecret（本地模型无鉴权）时，尝试使用不带凭据的客户端；若构造失败则给出明确异常提示  
            var ai = new OpenAIClient(new ApiKeyCredential(string.IsNullOrWhiteSpace(aISetting.AIKeySecret) ? "local" : aISetting.AIKeySecret), openAIClientOptions);

            var aiAgent = ai.GetChatClient(aISetting.AIDefaultModel).AsIChatClient().AsAIAgent(chatClientAgentOptions)
                .AsBuilder()
                .UseToolApproval(new ToolApprovalAgentOptions
                {

                    AutoApprovalRules = new Func<ToolAutoApprovalRuleContext, ValueTask<bool>>[]
                                        {
                                            // 先添加一个全匹配规则（注意安全风险）
                                            context => new ValueTask<bool>(true),
                                            // 或保留原有规则并放在后面作为备选
                                            AgentSkillsProvider.AllToolsAutoApprovalRule
                                        }
                })
                .Build();
            var reslut = new AgentResponse();
            var tokenConsumptionInfo = new TokenConsumptionInfo();
            var resultText = string.Empty;

            try
            {
                if (aISetting.IsStreame)
                {
                    if (aISetting.StreameCallback != default)
                    {
                        await foreach (var update in aiAgent.RunStreamingAsync(turnMessages, cancellationToken: cancellationToken))
                        {
                            if (update != default)
                            {
                                if (update.Contents != default)
                                {
                                    foreach (var content in update.Contents)
                                    {
                                        if (content != default)
                                        {
                                            switch (content)
                                            {
                                                case FunctionCallContent funcCall:
                                                    // 1. 模型决定调用工具  
                                                    var err = funcCall.Exception != default ? ("异常信息：" + funcCall.Exception?.Message) : "";
                                                    if (aISetting.ToolStreameCallback != default)
                                                    {
                                                        await aISetting.ToolStreameCallback.Invoke($"\n [工具调用] 名称：{funcCall.Name}，调用ID：{funcCall.CallId}，参数：（ {string.Join(", ", funcCall.Arguments?.Select(a => $"{a.Key}: {a.Value}") ?? [])}） {err}");
                                                    }
                                                    // 收集全部工具调用，轮结束时分类：外部工具回传调用方执行；自带工具在终止轮由网关补偿执行
                                                    AccumulateToolCall(collectedToolCalls, funcCall);
                                                    break;

                                                case FunctionResultContent funcResult:
                                                    // 2. 工具执行完毕返回结果 
                                                    var errr = funcResult.Exception != default ? ("异常信息：" + funcResult.Exception?.Message) : "";
                                                    if (aISetting.ToolStreameCallback != default)
                                                    {
                                                        await aISetting.ToolStreameCallback.Invoke($"\n [工具返回] 调用ID：{funcResult.CallId}，结果：{funcResult.Result?.ToString()} {errr} ");
                                                    }
                                                    // 记录框架已执行完成的调用：终止轮补偿执行时据此跳过，避免重复执行
                                                    if (funcResult.CallId != default)
                                                    {
                                                        completedToolCallIds.Add(funcResult.CallId);
                                                    }
                                                    break;

                                                case TextContent textContent:
                                                    // 3. 普通文本输出 
                                                    break;
                                                case TextReasoningContent reasoningContent:
                                                    // 4. 思考过程输出 
                                                    if (aISetting.ReasoningStreameCallback != default)
                                                    {
                                                        await aISetting.ReasoningStreameCallback.Invoke(reasoningContent.Text);
                                                    }
                                                    break;
                                                case UsageContent usageContent:
                                                    // 5. 消耗token信息
                                                    tokenConsumptionInfo.CachedInputTokenCount = usageContent.Details.CachedInputTokenCount;
                                                    tokenConsumptionInfo.InputTokenCount = usageContent.Details.InputTokenCount;
                                                    tokenConsumptionInfo.OutputTokenCount = usageContent.Details.OutputTokenCount;
                                                    tokenConsumptionInfo.TotalTokenCount = usageContent.Details.TotalTokenCount;
                                                    tokenConsumptionInfo.ReasoningTokenCount = usageContent.Details.ReasoningTokenCount;
                                                    break;
                                            }
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(update.Text))
                                {
                                    // 回调必须 await：既保证分片按模型输出顺序下发，也让回调内异常回到下面的 catch
                                    await aISetting.StreameCallback.Invoke(update.Text);
                                    resultText += update.Text;
                                }
                            }
                        }
                        // 一轮模型交互结束后，把外部工具调用一次性回传：调用方执行工具后以工具结果回传续轮
                        await EmitExternalToolCallsAsync(aISetting, collectedToolCalls);
                        // 终止轮补偿：框架交还外部工具调用时会跳过本轮全部工具调用，这里把被跳过的自带工具就地执行并落库，
                        // 让自带工具照常直接运行、且下一轮续轮时工具调用与结果完整配对
                        await ExecuteSkippedLocalToolCallsAsync(aISetting, chatClientAgentOptions, collectedToolCalls, completedToolCallIds, cancellationToken);
                    }
                }
                else
                {
                    reslut = await aiAgent.RunAsync(turnMessages, cancellationToken: cancellationToken);

                    resultText = reslut.Text;
                    if (reslut.Usage != default)
                    {
                        tokenConsumptionInfo.CachedInputTokenCount = reslut.Usage.CachedInputTokenCount;
                        tokenConsumptionInfo.InputTokenCount = reslut.Usage.InputTokenCount;
                        tokenConsumptionInfo.OutputTokenCount = reslut.Usage.OutputTokenCount;
                        tokenConsumptionInfo.TotalTokenCount = reslut.Usage.TotalTokenCount;
                        tokenConsumptionInfo.ReasoningTokenCount = reslut.Usage.ReasoningTokenCount;
                    }
                    if (reslut.Messages.Count > 0)
                    {
                        foreach (var message in reslut.Messages)
                        {
                            if (message.Contents.Count > 0)
                            {
                                foreach (var content in message.Contents)
                                {
                                    if (content != default)
                                    {
                                        switch (content)
                                        {
                                            case FunctionCallContent funcCall:
                                                //模型决定调用工具  
                                                var err = funcCall.Exception != default ? ("异常信息：" + funcCall.Exception?.Message) : "";
                                                if (aISetting.ToolStreameCallback != default)
                                                {
                                                    await aISetting.ToolStreameCallback.Invoke($"\n [工具调用] 名称：{funcCall.Name}，调用ID：{funcCall.CallId}，参数：（ {string.Join(", ", funcCall.Arguments?.Select(a => $"{a.Key}: {a.Value}") ?? [])}） {err}");
                                                }
                                                // 收集全部工具调用（与流式同逻辑），轮结束时分类：外部回传调用方，自带在终止轮补偿执行
                                                AccumulateToolCall(collectedToolCalls, funcCall);
                                                break;

                                            case FunctionResultContent funcResult:
                                                //工具执行完毕返回结果 
                                                var errr = funcResult.Exception != default ? ("异常信息：" + funcResult.Exception?.Message) : "";
                                                if (aISetting.ToolStreameCallback != default)
                                                {
                                                    await aISetting.ToolStreameCallback.Invoke($"\n [工具返回] 调用ID：{funcResult.CallId}，结果：{funcResult.Result?.ToString()} {errr} ");
                                                }
                                                // 记录框架已执行完成的调用：终止轮补偿执行时据此跳过，避免重复执行
                                                if (funcResult.CallId != default)
                                                {
                                                    completedToolCallIds.Add(funcResult.CallId);
                                                }
                                                break; 
                                            case TextReasoningContent reasoningContent:
                                                //思考过程输出 
                                                if (aISetting.ReasoningStreameCallback != default)
                                                {
                                                    await aISetting.ReasoningStreameCallback.Invoke(reasoningContent.Text);
                                                }
                                                break; 
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // 非流式路径同样回传外部工具调用：模型仅调工具不产出正文时 resultText 为空，调用必须独立回传
                    await EmitExternalToolCallsAsync(aISetting, collectedToolCalls);
                    // 终止轮补偿（与流式同逻辑）：被跳过的自带工具就地执行并落库
                    await ExecuteSkippedLocalToolCallsAsync(aISetting, chatClientAgentOptions, collectedToolCalls, completedToolCallIds, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                // 客户端主动中止（前端点“停止”）不是模型故障：必须原样抛出，否则下面会把它当普通异常重试、甚至换模型继续跑，
                // 前端早已断开等待，只会留下一条无意义的“报错回复”
                if (ex is OperationCanceledException && cancellationToken.IsCancellationRequested)
                {
                    if (aISetting.IsHttpLog)
                    {
                        HttpClientAutoInterceptor.StopInterception();
                    }
                    Ailogger?.LogWarning("模型调用已被客户端中止，模型：{Model}，第{Retries}次尝试。", aISetting.AIDefaultModel, retries);
                    throw;
                }
                Ailogger?.LogError(ex, "模型调用失败，模型：{Model}，第{Retries}次尝试。", aISetting.AIDefaultModel, retries);
                // 参数类错误（输入长度越界、max_tokens 超模型范围）由请求内容本身决定，重试或换模型都不会改变结果，直接终止并友好提示；
                // 其他错误（网络、限流、5xx）才按 Auto 模式切换备选模型继续重试
                if (!IsParameterInvalidException(ex) && retries <= maxRetries)
                {
                    // Auto模式：从备选模型中随机切换一个未使用过的模型
                    if (aISetting.FallbackModels?.Count > 0)
                    {
                        // 用 Random.Shared：避免短时间内连续 new Random() 拿到相同种子而反复选中同一个模型
                        var index = Random.Shared.Next(aISetting.FallbackModels.Count);
                        var nextModel = aISetting.FallbackModels[index];
                        aISetting.FallbackModels.RemoveAt(index);
                        // 更新当前模型配置
                        aISetting.AIUrl = nextModel.AIUrl;
                        aISetting.AIKeySecret = nextModel.AIKeySecret;
                        aISetting.AIDefaultModel = nextModel.AIDefaultModel;
                        // 同步更新Token配置：切换到新模型的回答Token上限
                        if (chatClientAgentOptions?.ChatOptions != null)
                        {
                            chatClientAgentOptions.ChatOptions.MaxOutputTokens = nextModel.AnswerTokens;
                        }
                        // 通知前端模型切换
                        if (aISetting.IsStreame && aISetting.ToolStreameCallback != default)
                        {
                            await aISetting.ToolStreameCallback.Invoke($"\n⚠️ 模型调用失败，正在自动切换到备用模型: {nextModel.AIDefaultModel}...\n");
                        }
                    }
                    retries++;
                    goto aiRun;
                }
                // 走到这里有两种情况：参数类错误（未重试），或非参数错误但重试次数已耗尽。
                // 统一把异常转成友好提示，避免前端只看到空回复
                var friendlyMsg = BuildFriendlyAIMsg(ex);
                // 友好文案照常推给前端，同时把原始异常带出去：调用方据此把本轮标记为失败并给出重试入口，
                // 而不是让报错以“正常回复”的形态落库、事后无法区分
                aISetting.LastError = ex;
                if (aISetting.IsStreame && aISetting.StreameCallback != default)
                {
                    await aISetting.StreameCallback.Invoke(friendlyMsg);
                }
                resultText += friendlyMsg;
            }
            if (aISetting.IsHttpLog)
            {
                HttpClientAutoInterceptor.StopInterception();
            }

            return (aiAgent, resultText, tokenConsumptionInfo);
        }

        /// <summary>
        /// 智能体未开启任何工具能力时清空工具，但保留外部声明型工具（AIFunctionDeclaration：只声明、不执行）：
        /// 它们是 OpenAI function calling 的透传载体，由调用方执行，一并清掉会导致调用方永远收不到 tool_calls
        /// </summary>
        private static List<AITool> KeepDeclaredOnlyTools(IEnumerable<AITool>? tools)
        {
            var declaredOnly = tools?.Where(t => t is AIFunctionDeclaration && t is not AIFunction).ToList();
            return declaredOnly is { Count: > 0 } ? declaredOnly : new List<AITool>();
        }

        /// <summary>
        /// 收集模型发出的工具调用（外部+自带都收，按 CallId 聚合以兼容流式分片），轮结束时再分类：
        /// 外部工具（AISetting.ExternalToolNames 命中的声明型工具）由 FunctionInvokingChatClient 原样交还、
        /// 回传给调用方执行；自带工具在终止轮被框架跳过，由网关补偿执行
        /// </summary>
        private static void AccumulateToolCall(Dictionary<string, (string Name, Dictionary<string, object?> Arguments)> collectedToolCalls, FunctionCallContent funcCall)
        {
            if (funcCall == default || funcCall.CallId == default) return;
            var hasEntry = collectedToolCalls.TryGetValue(funcCall.CallId, out var entry);
            if (!string.IsNullOrEmpty(funcCall.Name))
            {
                if (!hasEntry)
                {
                    entry = (funcCall.Name, new Dictionary<string, object?>(StringComparer.Ordinal));
                }
                entry.Name = funcCall.Name;
            }
            else if (!hasEntry)
            {
                // 无名帧且尚未建立条目：无法判定归属，忽略（流式首帧必带名称）
                return;
            }
            if (funcCall.Arguments != default)
            {
                foreach (var kv in funcCall.Arguments)
                {
                    entry.Arguments[kv.Key] = kv.Value;
                }
            }
            collectedToolCalls[funcCall.CallId] = entry;
        }

        /// <summary>
        /// 一轮模型交互结束后，把收集到的外部工具调用逐个回调给调用方（网关侧），由调用方执行并决定是否以工具结果续轮；
        /// 只有外部工具才回传：自带工具由网关就地执行，调用方只认识自己声明的工具
        /// </summary>
        private static async Task EmitExternalToolCallsAsync(AISetting aISetting, Dictionary<string, (string Name, Dictionary<string, object?> Arguments)> collectedToolCalls)
        {
            if (aISetting.ExternalToolCallCallback == default || collectedToolCalls.Count == 0) return;
            foreach (var kv in collectedToolCalls)
            {
                if (!IsExternalToolCall(aISetting, kv.Value.Name)) continue;
                // 回调格式与 OpenAI tool_calls 对齐：arguments 为 JSON 字符串
                var payload = System.Text.Json.JsonSerializer.Serialize(new
                {
                    id = kv.Key,
                    name = kv.Value.Name,
                    arguments = System.Text.Json.JsonSerializer.Serialize(kv.Value.Arguments)
                });
                await aISetting.ExternalToolCallCallback.Invoke(payload);
            }
        }

        /// <summary>
        /// 判断是否为外部工具调用（调用方声明的透传工具：由调用方执行，网关只负责回传 tool_calls）
        /// </summary>
        private static bool IsExternalToolCall(AISetting aISetting, string name)
            => !string.IsNullOrEmpty(name) && aISetting.ExternalToolNames?.Contains(name) == true;

        /// <summary>
        /// 终止轮补偿执行：本轮把外部工具调用交还调用方时，FunctionInvokingChatClient 会跳过本轮全部工具调用
        /// （含智能体自带工具，模型同轮混调两类工具即命中），这里把"被跳过且未执行"的自带工具就地执行：
        /// 1) 结果追加落库（紧跟终止轮的助手调用帧之后），保证调用方续轮时工具调用与结果完整配对；
        /// 2) 执行结果照常输出"工具返回"日志，前端观感与框架执行一致。
        /// 补偿过程吞掉自身异常（执行失败转为错误结果），不影响本轮正常返回、不触发外层重试
        /// </summary>
        private async Task ExecuteSkippedLocalToolCallsAsync(AISetting aISetting, ChatClientAgentOptions chatClientAgentOptions, Dictionary<string, (string Name, Dictionary<string, object?> Arguments)> collectedToolCalls, HashSet<string> completedToolCallIds, CancellationToken cancellationToken)
        {
            if (collectedToolCalls.Count == 0) return;
            // 只有"存在外部工具调用交还"才可能发生跳过；否则自带工具已被框架正常执行完毕
            var hasExternal = false;
            var pending = new List<(string CallId, string Name, Dictionary<string, object?> Arguments)>();
            foreach (var kv in collectedToolCalls)
            {
                if (IsExternalToolCall(aISetting, kv.Value.Name))
                {
                    hasExternal = true;
                    continue;
                }
                if (completedToolCallIds.Contains(kv.Key)) continue;
                pending.Add((kv.Key, kv.Value.Name, kv.Value.Arguments));
            }
            if (!hasExternal || pending.Count == 0) return;

            var tools = chatClientAgentOptions.ChatOptions?.Tools;
            var results = new List<FunctionResultContent>(pending.Count);
            foreach (var (callId, name, arguments) in pending)
            {
                object? result = default;
                string? errorMsg = default;
                // 与框架执行一致：只执行可执行的 AIFunction；声明型/找不到的给出错误结果兜底（保证配对完整）
                var function = tools?.OfType<AIFunction>().FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.Ordinal));
                if (function == default)
                {
                    result = $"Error: Requested function \"{name}\" not found.";
                }
                else
                {
                    try
                    {
                        result = await function.InvokeAsync(new AIFunctionArguments(arguments), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        errorMsg = ex.Message;
                        result = $"Error: Function failed. Exception: {ex.Message}";
                    }
                }
                result ??= "Success: Function completed.";
                if (aISetting.ToolStreameCallback != default)
                {
                    try
                    {
                        // 与框架执行的"工具返回"日志格式保持一致
                        await aISetting.ToolStreameCallback.Invoke($"\n [工具返回] 调用ID：{callId}，结果：{result} {(string.IsNullOrEmpty(errorMsg) ? "" : "异常信息：" + errorMsg)} ");
                    }
                    catch
                    {
                        // 日志回调异常不影响补偿结果落库
                    }
                }
                results.Add(new FunctionResultContent(callId, result));
            }
            // 结果落库：框架的 StoreChatHistoryAsync 已先落助手调用帧，这里追加更晚写入的工具结果，读取顺序正确
            if (chatClientAgentOptions.ChatHistoryProvider is KevinChatMessageStore kevinChatMessageStore)
            {
                try
                {
                    await kevinChatMessageStore.AppendToolResultsAsync(results, cancellationToken);
                }
                catch (Exception ex)
                {
                    Ailogger?.LogWarning(ex, "终止轮自带工具结果落库失败，下一轮续轮可能缺少工具结果配对，模型：{Model}", aISetting.AIDefaultModel);
                }
            }
        }

        /// <summary>
        /// 判断是否为请求参数类错误（如输入长度越界、max_tokens 超出模型支持范围），此类错误由请求内容本身决定，重试或换模型都无意义，且多为配置问题需友好提示
        /// </summary>
        private static bool IsParameterInvalidException(Exception ex)
        {
            var msg = (ex.Message ?? "") + " " + (ex.InnerException?.Message ?? "");
            return msg.Contains("invalid_parameter") || msg.Contains("InvalidParameter") || msg.Contains("invalid_request_error");
        }

        /// <summary>
        /// 将模型调用异常转成用户可读的友好提示
        /// </summary>
        private static string BuildFriendlyAIMsg(Exception ex)
        {
            var msg = ex.InnerException?.Message ?? ex.Message ?? "";
            if (msg.Contains("max_tokens") || msg.Contains("max_completion_tokens"))
            {
                // 提取模型返回的允许范围，如 "Range of max_tokens should be [1, 32768]"
                return $"\n❌ 回答Token设置超出了当前模型支持的最大输出长度{ExtractRange(msg)}，请到智能体设置中调小“回答Token”后重试。";
            }
            // 百炼/DashScope 用 "Range of input length should be [1, N]" 表达输入越界：区间是双边的，超出上限与输入长度为 0（空内容）都会命中
            if (msg.Contains("input length") || msg.Contains("input_length"))
            {
                return $"\n❌ 本次请求的输入为空或超出了模型最大输入长度{ExtractRange(msg)}，请补充问题内容，或新建对话、到模型设置中调大“提问Token”后重试。";
            }
            if (msg.Contains("context_length") || msg.Contains("context length") || msg.Contains("maximum context"))
            {
                return "\n❌ 提问内容（含历史对话与上下文）超出了模型的上下文窗口，请到智能体设置中调大“提问Token”或新建对话重试。";
            }
            if (msg.Contains("401") || msg.Contains("API key") || msg.Contains("api_key") || msg.Contains("Unauthorized") || msg.Contains("InvalidApiKey"))
            {
                return "\n❌ 模型鉴权失败（API Key 无效或已过期），请检查模型配置后重试。";
            }
            if (msg.Contains("429"))
            {
                return "\n❌ 模型服务限流或余额不足，请稍后重试或检查模型供应商配置。";
            }
            return $"\n❌ 模型调用失败：{msg}";
        }

        /// <summary>
        /// 从模型返回的错误文案中提取允许区间，如 "Range of max_tokens should be [1, 32768]" → " [1, 32768]"；取不到时返回空串
        /// </summary>
        private static string ExtractRange(string msg)
        {
            var start = msg.IndexOf('[');
            var end = msg.IndexOf(']');
            return (start > 0 && end > start) ? " " + msg.Substring(start, end - start + 1) : "";
        }

        /// <summary>
        /// 创建AI代理 
        /// </summary>
        /// <param name="aISetting"></param>
        /// <param name="chatClientAgentOptions"></param> 
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AIAgent> CreateOpenAIAgent(AISetting aISetting, ChatClientAgentOptions chatClientAgentOptions, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();//是否已经中止，若已请求取消则抛出异常
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions()
            {
                Endpoint = new Uri(aISetting.AIUrl),
                NetworkTimeout = TimeSpan.FromMinutes(aISetting.NetworkTimeout),// 设置网络超时时间为10分钟，适用于可能需要较长时间处理的请求
                RetryPolicy = new ClientRetryPolicy(maxRetries: aISetting.MaxRetries)//重试次数和延迟
                {
                    // 可自定义延迟，默认指数退避
                }
            };
            #region AI工具
            if (!aISetting.IsAITools && !aISetting.IsMcpTools && !aISetting.IsMemory && !aISetting.IsKnowledgeBase)
            {
                if (chatClientAgentOptions.ChatOptions != default)
                {
                    // 外部声明型工具（function calling 透传）不受能力开关影响：只声明不执行、由调用方执行，清掉将永远收不到 tool_calls
                    chatClientAgentOptions.ChatOptions.Tools = KeepDeclaredOnlyTools(chatClientAgentOptions.ChatOptions.Tools);
                }
            }
            #endregion

            #region AI技能
            if (!aISetting.IsAISkills)
            {
                chatClientAgentOptions.AIContextProviders = default;
            }
            #endregion 
            // 当无 keySecret（本地模型无鉴权）时，尝试使用不带凭据的客户端；若构造失败则给出明确异常提示  
            var ai = new OpenAIClient(new ApiKeyCredential(string.IsNullOrWhiteSpace(aISetting.AIKeySecret) ? "local" : aISetting.AIKeySecret), openAIClientOptions);
            var aiAgent = ai.GetChatClient(aISetting.AIDefaultModel).AsIChatClient().AsAIAgent(chatClientAgentOptions).AsBuilder()
                .UseToolApproval(new ToolApprovalAgentOptions
                {
                    AutoApprovalRules = new Func<ToolAutoApprovalRuleContext, ValueTask<bool>>[]
                                            {
                                                // 先添加一个全匹配规则（注意安全风险）
                                                context => new ValueTask<bool>(true),
                                                // 或保留原有规则并放在后面作为备选
                                                AgentSkillsProvider.AllToolsAutoApprovalRule
                                            }
                })
                .Build();
            return aiAgent;
        }
    }
}
