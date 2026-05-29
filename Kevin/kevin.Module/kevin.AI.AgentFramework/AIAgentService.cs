using HttpMataki.NET.Auto;
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
        /// <param name="fileUrl"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<(AIAgent, string)> CreateOpenAIAgentAndSendMSG(AISetting aISetting, ChatClientAgentOptions chatClientAgentOptions, string msg, List<string>? fileUrl = default, CancellationToken cancellationToken = default)
        {
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
            var resultText = string.Empty;
            ChatMessage message = new(ChatRole.User, [new TextContent(msg)]);
            if (fileUrl != default && fileUrl.Count > 0)
            {
                message= new(ChatRole.User, [new TextContent(msg),..fileUrl.Select(t=> new UriContent(t)).ToList()]);
            }
            if (aISetting.IsStreame)
            {
                if (aISetting.StreameCallback != default)
                {
                    await foreach (var update in aiAgent.RunStreamingAsync(message, cancellationToken: cancellationToken))
                    {
                        foreach (var content in update.Contents)
                        {
                            switch (content)
                            {
                                case FunctionCallContent funcCall:
                                    // 1. 模型决定调用工具 
                                    if (aISetting.ToolStreameCallback != default)
                                    {
                                        aISetting.ToolStreameCallback.Invoke($"\n [工具调用] 名称：{funcCall.Name}，调用ID：{funcCall.CallId}，参数：{JsonConvert.SerializeObject(funcCall.Arguments).ToString()}");
                                    }
                                    break;

                                case FunctionResultContent funcResult:
                                    // 2. 工具执行完毕返回结果 
                                    if (aISetting.ToolStreameCallback != default)
                                    {
                                        aISetting.ToolStreameCallback.Invoke($"\n [工具返回] 调用ID：{funcResult.CallId}，结果：{funcResult.Result}");
                                    }
                                    break;

                                case TextContent textContent:
                                    // 3. 普通文本输出 
                                    break;
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
                                    aISetting.ReasoningStreameCallback.Invoke($"{await GetReasoningTextAsync(update)}");
                                }

                            }
                        }

                    }

                }
            }
            else
            {
                reslut = await aiAgent.RunAsync(message, cancellationToken: cancellationToken);
                resultText = reslut.Text;
            }
            if (aISetting.IsHttpLog)
            {
                HttpClientAutoInterceptor.StopInterception();
            }
            return (aiAgent, resultText);
        }

        /// <summary>
        ///获取模型思考过程文本（适用于流式输出时从原始响应中提取reasoning字段）
        /// </summary>
        /// <param name="update">流式更新对象</param>
        /// <returns>思考过程文本</returns>
        private async Task<string> GetReasoningTextAsync(AgentResponseUpdate update)
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
    }
}
