using kevin.Domain.Interfaces.IServices.AI;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Kevin.Web.Basics.AI
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

        public async Task WriteAsync(string channel, string message)
        {
            if (!_streamEnabled || string.IsNullOrEmpty(message))
            {
                return;
            }
            switch (channel)
            {
                case "aimsg":
                    await WriteChunkAsync(new OpenAICompatibleDelta { Content = message }, null);
                    break;
                case "aIReasoningContentMsg":
                    if (_thinkingEnabled)
                    {
                        await WriteChunkAsync(new OpenAICompatibleDelta { ReasoningContent = message }, null);
                    }
                    break;
                default:
                    // processmsg / aIToolsContentMsg / recommendmsg：智能体内部过程，不进入 OpenAI 协议输出
                    break;
            }
        }

        /// <summary>正常收尾：finish 分片 + [DONE]</summary>
        public async Task CompleteAsync()
        {
            if (!_streamEnabled)
            {
                return;
            }
            await WriteChunkAsync(new OpenAICompatibleDelta(), "stop");
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
