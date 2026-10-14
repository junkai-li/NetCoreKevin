using kevin.Domain.Interfaces.IServices.AI;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Kevin.Web.Basics.Controllers.AI.OpenAI
{
    /// <summary>
    /// OpenAI 协议流式输出适配器：实现智能体管线的 <see cref="IChatStreamOutput"/> 端口，
    /// 把各通道分片翻译成 OpenAI 流式响应帧（chat.completion.chunk + 结尾 data: [DONE]）写入当前 HTTP 响应。
    /// <para>
    /// 通道映射：aimsg → delta.content；aIReasoningContentMsg → delta.reasoning_content（可开关）；
    /// processmsg / aIToolsContentMsg / recommendmsg 属于智能体内部过程信息，不进入协议输出。
    /// </para>
    /// <para>
    /// 非流式请求时传 streamEnabled=false，实例退化为空实现（完整回复由控制器从落库结果组装）。
    /// </para>
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

        public OpenAICompatibleChatStreamOutput(HttpResponse response, string model, bool thinkingEnabled, bool streamEnabled)
        {
            _response = response;
            _model = model;
            _thinkingEnabled = thinkingEnabled;
            _streamEnabled = streamEnabled;
            _completionId = "chatcmpl-" + Guid.NewGuid().ToString("N");
            _created = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>流式响应是否已开始写数据（控制器据此判断出错时还能否改写状态码返回 JSON 错误）</summary>
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
                        await WriteChunkAsync(new OpenAICompatibleDelta { Content = message }, null);
                    }
                    break;
                case "aIReasoningContentMsg":
                    if (_streamEnabled && _thinkingEnabled)
                    {
                        await WriteChunkAsync(new OpenAICompatibleDelta { ReasoningContent = message }, null);
                    }
                    break;
                case "toolcalls":
                    // 透传工具调用：非流式（streamEnabled=false）下也要收集，控制器据此组装非流式响应；
                    // 流式下逐条下发 delta.tool_calls（网关侧已按 CallId 聚合，单帧携带完整调用，index 分组）
                    var toolCall = ParseToolCall(message);
                    if (toolCall == default)
                    {
                        break;
                    }
                    var index = _collectedToolCalls.Count;
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
