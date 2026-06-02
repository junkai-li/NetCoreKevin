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
        public async Task<(AIAgent, string, TokenConsumptionInfo)> CreateOpenAIAgentAndSendMSG(AISetting aISetting, ChatClientAgentOptions chatClientAgentOptions, string msg, List<string>? OtherContents = default, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(msg))
            {
                throw new Exception("msg不能为空");
            }
            cancellationToken.ThrowIfCancellationRequested();//是否已经中止，若已请求取消则抛出异常
            if (aISetting.IsHttpLog)
            {
                HttpClientAutoInterceptor.StartInterception();
            }
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
            if (!aISetting.IsAITools)
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
            var aiAgent = ai.GetChatClient(aISetting.AIDefaultModel).AsIChatClient().AsAIAgent(chatClientAgentOptions);
            var reslut = new AgentResponse();
            var tokenConsumptionInfo = new TokenConsumptionInfo();
            var resultText = string.Empty;

            ChatMessage message = new(ChatRole.User, [new TextContent(msg)]);
            if (OtherContents != default && OtherContents.Count > 0)
            {
                message = new(ChatRole.User, [.. OtherContents.Where(t => !string.IsNullOrEmpty(t)).Select(t => new TextContent(t)).ToList(), new TextContent(msg)]);
            }
            if (aISetting.IsStreame)
            {
                if (aISetting.StreameCallback != default)
                {
                    try
                    {
                        await foreach (var update in aiAgent.RunStreamingAsync(message, cancellationToken: cancellationToken))
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
                                                    if (aISetting.ToolStreameCallback != default)
                                                    {
                                                        aISetting.ToolStreameCallback.Invoke($"\n [工具调用] 名称：{funcCall.Name}，调用ID：{funcCall.CallId}，参数：{funcCall.Arguments?.SerializeToJson()}");
                                                    }
                                                    break;

                                                case FunctionResultContent funcResult:
                                                    // 2. 工具执行完毕返回结果 
                                                    if (aISetting.ToolStreameCallback != default)
                                                    {
                                                        aISetting.ToolStreameCallback.Invoke($"\n [工具返回] 调用ID：{funcResult.CallId}，结果：{funcResult.Result?.SerializeToJson()}");
                                                    }
                                                    break;

                                                case TextContent textContent:
                                                    // 3. 普通文本输出 
                                                    break;
                                            }
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(update.Text))
                                {
                                    aISetting.StreameCallback.Invoke(update.Text);
                                    resultText += update.Text;
                                }
                                else
                                {
                                    if (aISetting.ReasoningStreameCallback != default)
                                    {
                                        var reasoningStr = await GetReasoningTextAsync(update);
                                        if (!string.IsNullOrEmpty(reasoningStr))
                                        {
                                            aISetting.ReasoningStreameCallback.Invoke(reasoningStr);
                                        }
                                    }

                                }
                                if (TryExtractUsageFromUpdate(update, out var usage))
                                {
                                    tokenConsumptionInfo = usage;
                                }
                            }
                        }
                    }
                    catch (ClientResultException cre)
                    {
                        // Log and fall back to non-streaming; this avoids the request failing completely when streaming errors occur
                        Ailogger?.LogError(cre, "Streaming call failed, falling back to non-streaming RunAsync. Error: {Message}", cre.Message);
                        try
                        {
                            reslut = await aiAgent.RunAsync(message, cancellationToken: cancellationToken);
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
                        catch (Exception ex2)
                        {
                            Ailogger?.LogError(ex2, "Fallback RunAsync after streaming failure also failed.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Unexpected exception: try to fallback as well
                        Ailogger?.LogError(ex, "Unexpected streaming error, falling back to non-streaming RunAsync.");
                        try
                        {
                            reslut = await aiAgent.RunAsync(message, cancellationToken: cancellationToken);
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
                        catch (Exception ex2)
                        {
                            Ailogger?.LogError(ex2, "Fallback RunAsync after unexpected streaming error also failed.");
                        }
                    }
                }
            }
            else
            {
                reslut = await aiAgent.RunAsync(message, cancellationToken: cancellationToken);
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
            if (aISetting.IsHttpLog)
            {
                HttpClientAutoInterceptor.StopInterception();
            }

            return (aiAgent, resultText, tokenConsumptionInfo);
        }

        /// <summary>
        ///获取模型思考过程文本（适用于流式输出时从原始响应中提取reasoning字段）
        /// </summary>
        /// <param name="update">流式更新对象</param>
        /// <returns>思考过程文本</returns>
        private async Task<string> GetReasoningTextAsync(AgentResponseUpdate update)
        {
            try
            {
                var reasoningBuilder = new StringBuilder();
                // 仅处理无文本输出、包含原始响应的更新
                if (update.RawRepresentation is Microsoft.Extensions.AI.ChatResponseUpdate streamingChatCompletionUpdate
                    && streamingChatCompletionUpdate.RawRepresentation is OpenAI.Chat.StreamingChatCompletionUpdate chatCompletionUpdate)
                {
                    // 从原始JSON中提取 reasoning 字段
#pragma warning disable SCME0001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
                    ref JsonPatch patch = ref chatCompletionUpdate.Patch;
#pragma warning restore SCME0001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。  
                    // 1. 从 patch 中获取更上层的 choices 片段，然后遍历每个 choice 的 delta 查找 reasoning
                    var jsonPathBytes = System.Text.Encoding.UTF8.GetBytes("$.choices");
                    var jsonPathSpan = new ReadOnlySpan<byte>(jsonPathBytes);
                    // 2. 调用 TryGetJson（获取 choices 数组片段）
                    if (patch.TryGetJson(jsonPathSpan, out var data))
                    {
                        // 将 ReadOnlyMemory<byte> 转为字符串，便于调试输出
                        var jsonString = System.Text.Encoding.UTF8.GetString(data.ToArray());

                        using var doc = JsonDocument.Parse(jsonString);
                        var root = doc.RootElement;

                        // 遍历 choices 数组，查找 delta 下的 reasoning 字段（兼容字符串或对象）
                        if (root.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var choice in root.EnumerateArray())
                            {
                                if (choice.TryGetProperty("delta", out var delta))
                                {
                                    // 支持两种字段名：reasoning 或 reasoning_content
                                    if (!delta.TryGetProperty("reasoning", out var reasoningProp)
                                        && !delta.TryGetProperty("reasoning_content", out reasoningProp))
                                    {
                                        continue;
                                    }

                                    string reasoningText = reasoningProp.ValueKind == JsonValueKind.String
                                        ? reasoningProp.GetString() ?? string.Empty
                                        : reasoningProp.GetRawText();

                                    if (!string.IsNullOrEmpty(reasoningText))
                                    {
                                        reasoningBuilder.Append(reasoningText);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // 如果返回的不是数组，尝试按对象结构解析（兼容性保护）
                            if (root.TryGetProperty("choices", out var choices) && choices.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var choice in choices.EnumerateArray())
                                {
                                    if (choice.TryGetProperty("delta", out var delta))
                                    {
                                        // 支持两种字段名：reasoning 或 reasoning_content
                                        if (!delta.TryGetProperty("reasoning", out var reasoningProp)
                                            && !delta.TryGetProperty("reasoning_content", out reasoningProp))
                                        {
                                            continue;
                                        }

                                        string reasoningText = reasoningProp.ValueKind == JsonValueKind.String
                                            ? reasoningProp.GetString() ?? string.Empty
                                            : reasoningProp.GetRawText();
                                        if (!string.IsNullOrEmpty(reasoningText))
                                        {
                                            reasoningBuilder.Append(reasoningText);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return (reasoningBuilder?.ToString() ?? "").Replace("null", "\n");
            }
            catch (Exception)
            {

                return "";
            }

        }

        /// <summary>
        /// 从更新中提取使用信息
        /// </summary>
        /// <param name="update"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private static bool TryExtractUsageFromUpdate(AgentResponseUpdate update, out TokenConsumptionInfo info)
        {
            try
            {
                info = new TokenConsumptionInfo();
                if (update == null) return false;

                // 1) Common case: wrapper ChatResponseUpdate -> underlying OpenAI streaming update
                if (update.RawRepresentation is Microsoft.Extensions.AI.ChatResponseUpdate chatResp &&
                    chatResp.RawRepresentation is OpenAI.Chat.StreamingChatCompletionUpdate openAiInner)
                {
                    if (openAiInner.Usage != null)
                    {
                        info.InputTokenCount = openAiInner.Usage.InputTokenCount;
                        info.OutputTokenCount = openAiInner.Usage.OutputTokenCount;
                        info.TotalTokenCount = openAiInner.Usage.TotalTokenCount;
                        return true;
                    }
                }

                // 2) Raw is directly OpenAI streaming update
                if (update.RawRepresentation is OpenAI.Chat.StreamingChatCompletionUpdate openAiDirect)
                {
                    if (openAiDirect.Usage != null)
                    {
                        info.InputTokenCount = openAiDirect.Usage.InputTokenCount;
                        info.OutputTokenCount = openAiDirect.Usage.OutputTokenCount;
                        info.TotalTokenCount = openAiDirect.Usage.TotalTokenCount;
                        return true;
                    }
                }

                // 3) Try reflection: a Usage property on update or its RawRepresentation
                object? candidate = null;
                var usageProp = update.GetType().GetProperty("Usage") ?? update.GetType().GetProperty("usage");
                if (usageProp != null) candidate = usageProp.GetValue(update);
                else if (update.RawRepresentation != null)
                {
                    var rpType = update.RawRepresentation.GetType();
                    var rpUsageProp = rpType.GetProperty("Usage") ?? rpType.GetProperty("usage");
                    if (rpUsageProp != null) candidate = rpUsageProp.GetValue(update.RawRepresentation);
                }

                if (candidate != null)
                {
                    int? GetInt(object? o, string name)
                    {
                        var p = o?.GetType().GetProperty(name);
                        if (p == null) return null;
                        var v = p.GetValue(o);
                        return v switch
                        {
                            int i => i,
                            long l => (int)l,
                            System.Text.Json.JsonElement je when je.ValueKind == System.Text.Json.JsonValueKind.Number && je.TryGetInt32(out var vi) => vi,
                            _ => null
                        };
                    }

                    info.InputTokenCount = GetInt(candidate, "InputTokenCount") ?? GetInt(candidate, "PromptTokens") ?? 0;
                    info.OutputTokenCount = GetInt(candidate, "OutputTokenCount") ?? GetInt(candidate, "CompletionTokens") ?? 0;
                    info.TotalTokenCount = GetInt(candidate, "TotalTokenCount") ?? GetInt(candidate, "TotalTokens") ?? (info.InputTokenCount + info.OutputTokenCount);
                    return info.TotalTokenCount > 0;
                }

                // 4) 最后：尝试把 RawRepresentation 序列化为 JSON 并查找 usage 节点
                try
                {
                    string? json = null;
                    if (update.RawRepresentation is string s) json = s;
                    else if (update.RawRepresentation is System.Text.Json.JsonElement je) json = je.GetRawText();
                    else if (update.RawRepresentation != null) json = System.Text.Json.JsonSerializer.Serialize(update.RawRepresentation);

                    if (!string.IsNullOrEmpty(json))
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(json);
                        if (TryFindUsageElement(doc.RootElement, out var usageEl))
                        {
                            int? GetIntFromJson(System.Text.Json.JsonElement el, string name)
                            {
                                if (el.ValueKind != System.Text.Json.JsonValueKind.Object) return null;
                                if (el.TryGetProperty(name, out var p) && p.ValueKind == System.Text.Json.JsonValueKind.Number && p.TryGetInt32(out var v)) return v;
                                return null;
                            }

                            info.InputTokenCount = GetIntFromJson(usageEl, "prompt_tokens") ?? GetIntFromJson(usageEl, "input_tokens") ?? 0;
                            info.OutputTokenCount = GetIntFromJson(usageEl, "completion_tokens") ?? GetIntFromJson(usageEl, "output_tokens") ?? 0;
                            info.TotalTokenCount = GetIntFromJson(usageEl, "total_tokens") ?? (info.InputTokenCount + info.OutputTokenCount);
                            return info.TotalTokenCount > 0;
                        }
                    }
                }
                catch
                {
                    // ignore parse errors
                }
                return false;
            }
            catch (Exception)
            {
                info = new TokenConsumptionInfo();
            }
            return false;
            static bool TryFindUsageElement(System.Text.Json.JsonElement root, out System.Text.Json.JsonElement usage)
            {
                if (root.ValueKind == System.Text.Json.JsonValueKind.Object && root.TryGetProperty("usage", out usage)) return true;
                var stack = new Stack<System.Text.Json.JsonElement>();
                stack.Push(root);
                while (stack.Count > 0)
                {
                    var node = stack.Pop();
                    if (node.ValueKind == System.Text.Json.JsonValueKind.Object)
                    {
                        foreach (var prop in node.EnumerateObject())
                        {
                            if (string.Equals(prop.Name, "usage", StringComparison.OrdinalIgnoreCase))
                            {
                                usage = prop.Value;
                                return true;
                            }
                            if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.Object || prop.Value.ValueKind == System.Text.Json.JsonValueKind.Array)
                                stack.Push(prop.Value);
                        }
                    }
                    else if (node.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        foreach (var item in node.EnumerateArray())
                        {
                            if (item.ValueKind == System.Text.Json.JsonValueKind.Object || item.ValueKind == System.Text.Json.JsonValueKind.Array)
                                stack.Push(item);
                        }
                    }
                }
                usage = default;
                return false;
            }
        }
    }
}
