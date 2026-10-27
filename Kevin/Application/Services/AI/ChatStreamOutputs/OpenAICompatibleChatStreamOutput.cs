using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// OpenAI 协议流式输出适配器：把智能体管线的通道分片翻译成 chat.completion.chunk 帧（结尾 data: [DONE]）。
    /// aimsg → content；aIReasoningContentMsg → reasoning_content（可开关）；toolcalls → delta.tool_calls。
    /// </summary>
    internal sealed class OpenAICompatibleChatStreamOutput : IChatStreamOutput
    {
        private static readonly JsonSerializerOptions ChunkJsonOptions = new()
        {
            // 中文回答按原文下发，不转成 \uXXXX 转义（帧体积与可读性）
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        private readonly HttpResponse _response;
        private readonly string _model;
        private readonly string _completionId;
        private readonly long _created;
        private readonly bool _streamEnabled;
        private readonly bool _thinkingEnabled;
        // 同一个响应体的写操作必须串行：模型分片与思考分片来自并发的回调时不能交错破坏帧格式
        private readonly SemaphoreSlim _writeLock = new(1, 1);
        private bool _firstChunkSent;
        // 本轮透传工具调用（网关不执行，收齐后下发/组装，由客户端执行）
        private readonly List<OpenAICompatibleToolCall> _collectedToolCalls = new();
        // 透传工具调用的 index（delta.tool_calls 按 index 分组的序号）
        private int _nextToolCallIndex;
        // 本轮是否出现过透传工具调用，用于跳过兜底文案
        private bool _toolInvocationSeen;
        // reasoning 兜底：本轮是否发过 content，以及累加的 reasoning 文本
        private bool _contentChunkSent;
        private readonly StringBuilder _accumulatedReasoning = new();

        public OpenAICompatibleChatStreamOutput(HttpResponse response, string model, bool thinkingEnabled, bool streamEnabled)
        {
            _response = response;
            _model = model;
            _thinkingEnabled = thinkingEnabled;
            _streamEnabled = streamEnabled;
            _completionId = "chatcmpl-" + Guid.NewGuid().ToString("N");
            _created = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>流式响应是否已开始写数据（出错时据此判断还能否改写状态码返回 JSON 错误）</summary>
        public bool StreamStarted { get; private set; }

        /// <summary>本轮已收集的透传工具调用（非流式响应组装 message.tool_calls 用；流式下已随 delta 帧下发，此处保留供核对）</summary>
        public IReadOnlyList<OpenAICompatibleToolCall> CollectedToolCalls => _collectedToolCalls;

        public async Task WriteAsync(string channel, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            switch (channel)
            {
                case "aimsg":
                    if (_streamEnabled)
                    {
                        _contentChunkSent = true;
                        await WriteChunkAsync(new OpenAICompatibleDelta { Content = message }, null);
                    }
                    break;
                case "aIReasoningContentMsg":
                    if (_streamEnabled && _thinkingEnabled)
                    {
                        _accumulatedReasoning.Append(message);
                        await WriteChunkAsync(new OpenAICompatibleDelta { ReasoningContent = message }, null);
                    }
                    break;
                case "toolcalls":
                    // 透传工具调用：非流式仅收集，流式逐条下发 delta.tool_calls
                    var toolCall = ParseToolCall(message);
                    if (toolCall == default)
                    {
                        break;
                    }
                    var index = _nextToolCallIndex++;
                    _toolInvocationSeen = true;
                    _collectedToolCalls.Add(toolCall);
                    if (_streamEnabled)
                    {
                        await WriteChunkAsync(new OpenAICompatibleDelta
                        {
                            ToolCalls = new List<OpenAICompatibleDeltaToolCall>
                            {
                                new() { Index = index, Id = toolCall.Id, Type = toolCall.Type, Function = toolCall.Function }
                            }
                        }, null);
                    }
                    break;
                default:
                    // processmsg / aIToolsContentMsg / recommendmsg：智能体内部过程，不进入 OpenAI 协议输出
                    break;
            }
        }

        /// <summary>解析透传工具调用 payload：{"id","name","arguments"}（arguments 为 JSON 字符串）</summary>
        private static OpenAICompatibleToolCall? ParseToolCall(string message)
        {
            try
            {
                using var doc = JsonDocument.Parse(message);
                var root = doc.RootElement;
                if (root.ValueKind != JsonValueKind.Object) return null;
                var id = root.TryGetProperty("id", out var idEl) && idEl.ValueKind == JsonValueKind.String ? idEl.GetString() : null;
                var name = root.TryGetProperty("name", out var nameEl) && nameEl.ValueKind == JsonValueKind.String ? nameEl.GetString() : null;
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name)) return null;
                var arguments = root.TryGetProperty("arguments", out var argsEl) && argsEl.ValueKind == JsonValueKind.String ? argsEl.GetString() : null;
                return new OpenAICompatibleToolCall
                {
                    Id = id!,
                    Type = "function",
                    // OpenAI 协议要求 arguments 为合法 JSON 字符串：缺失时退化为空对象
                    Function = new OpenAICompatibleFunctionCall { Name = name!, Arguments = string.IsNullOrEmpty(arguments) ? "{}" : arguments },
                };
            }
            catch (JsonException)
            {
                // payload 是网关内部回调产物，格式异常说明上游代码有 bug：忽略该条不至于打断整个流
                return null;
            }
        }

        /// <summary>正常收尾：finish 分片 + [DONE]；本轮有透传工具调用时 finish_reason 按 OpenAI 协议用 tool_calls</summary>
        public async Task CompleteAsync()
        {
            if (!_streamEnabled)
            {
                return;
            }
            // 全程没有 content 但有 reasoning 时补一条兜底帧；工具轮不补
            if (!_contentChunkSent && !_toolInvocationSeen && _accumulatedReasoning.Length > 0)
            {
                var fallback = "不太明白你的意思，请再详细描述一下。";
                await WriteChunkAsync(new OpenAICompatibleDelta { Content = fallback }, null);
            }
            await WriteChunkAsync(new OpenAICompatibleDelta(), _collectedToolCalls.Count > 0 ? "tool_calls" : "stop");
            await WriteRawAsync("data: [DONE]\n\n");
        }

        /// <summary>流式处理中途出错：按 OpenAI 流式错误约定下发 error 数据帧后关闭流</summary>
        public async Task WriteErrorAsync(string message)
        {
            if (!_streamEnabled)
            {
                return;
            }
            var payload = JsonSerializer.Serialize(new OpenAICompatibleErrorResponse
            {
                Error = new OpenAICompatibleErrorBody { Message = message, Type = "server_error" }
            }, ChunkJsonOptions);
            await WriteRawAsync($"data: {payload}\n\n");
            await WriteRawAsync("data: [DONE]\n\n");
        }

        /// <summary>下发一条 chunk；首个分片前先补一条带 role 的声明帧（OpenAI 流式约定，部分客户端依赖）</summary>
        private async Task WriteChunkAsync(OpenAICompatibleDelta delta, string? finishReason)
        {
            await _writeLock.WaitAsync();
            try
            {
                if (!_firstChunkSent)
                {
                    _firstChunkSent = true;
                    await WriteRawLockedAsync(BuildChunkFrame(new OpenAICompatibleDelta { Role = "assistant" }, null));
                }
                await WriteRawLockedAsync(BuildChunkFrame(delta, finishReason));
            }
            finally
            {
                _writeLock.Release();
            }
        }

        private string BuildChunkFrame(OpenAICompatibleDelta delta, string? finishReason)
        {
            var chunk = new OpenAICompatibleChunk
            {
                Id = _completionId,
                Created = _created,
                Model = _model,
                Choices = new List<OpenAICompatibleChunkChoice>
                {
                    new() { Index = 0, Delta = delta, FinishReason = finishReason }
                }
            };
            return "data: " + JsonSerializer.Serialize(chunk, ChunkJsonOptions) + "\n\n";
        }

        private async Task WriteRawAsync(string text)
        {
            await _writeLock.WaitAsync();
            try
            {
                await WriteRawLockedAsync(text);
            }
            finally
            {
                _writeLock.Release();
            }
        }

        private async Task WriteRawLockedAsync(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            await _response.Body.WriteAsync(bytes, 0, bytes.Length);
            await _response.Body.FlushAsync();
            StreamStarted = true;
        }
    }
}
