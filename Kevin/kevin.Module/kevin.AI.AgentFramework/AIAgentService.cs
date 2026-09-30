using HttpMataki.NET.Auto;
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
        public async Task<(AIAgent, string, TokenConsumptionInfo)> CreateOpenAIAgentAndSendMSG(AISetting aISetting, ChatClientAgentOptions chatClientAgentOptions, ChatMessage messages, CancellationToken cancellationToken = default)
        {

            cancellationToken.ThrowIfCancellationRequested();//是否已经中止，若已请求取消则抛出异常
            if (aISetting.IsHttpLog)
            {
                HttpClientAutoInterceptor.StartInterception();
            }
            #region AI工具
            if (!aISetting.IsAITools && !aISetting.IsMcpTools && !aISetting.IsMemory && !aISetting.IsKnowledgeBase)
            {
                if (chatClientAgentOptions.ChatOptions != default)
                {
                    chatClientAgentOptions.ChatOptions.Tools = new List<AITool>();
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
                        await foreach (var update in aiAgent.RunStreamingAsync(messages, cancellationToken: cancellationToken))
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
                                                    break;

                                                case FunctionResultContent funcResult:
                                                    // 2. 工具执行完毕返回结果 
                                                    var errr = funcResult.Exception != default ? ("异常信息：" + funcResult.Exception?.Message) : "";
                                                    if (aISetting.ToolStreameCallback != default)
                                                    {
                                                        await aISetting.ToolStreameCallback.Invoke($"\n [工具返回] 调用ID：{funcResult.CallId}，结果：{funcResult.Result?.ToString()} {errr} ");
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
                    }
                }
                else
                {
                    reslut = await aiAgent.RunAsync(messages, cancellationToken: cancellationToken);

                    resultText = reslut.Text;
                    if (reslut.Usage != default)
                    {
                        tokenConsumptionInfo.CachedInputTokenCount = reslut.Usage.CachedInputTokenCount;
                        tokenConsumptionInfo.InputTokenCount = reslut.Usage.InputTokenCount;
                        tokenConsumptionInfo.OutputTokenCount = reslut.Usage.OutputTokenCount;
                        tokenConsumptionInfo.TotalTokenCount = reslut.Usage.TotalTokenCount;
                        tokenConsumptionInfo.ReasoningTokenCount = reslut.Usage.ReasoningTokenCount;
                    }
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
                    chatClientAgentOptions.ChatOptions.Tools = new List<AITool>();
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
