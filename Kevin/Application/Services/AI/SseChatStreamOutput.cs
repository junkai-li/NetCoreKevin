using kevin.Domain.Interfaces.IServices.AI;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// SSE 输出通道适配器：把分片按 Server-Sent Events 帧格式写入当前 HTTP 响应并即时 flush，
    /// 事件名用 <c>event:</c>、内容用 <c>data:</c>（多行内容拆成多条 data 行）。
    /// </summary>
    internal sealed class SseChatStreamOutput : IChatStreamOutput
    {
        private static readonly UTF8Encoding Utf8NoBom = new(false);
        private readonly HttpResponse _response;
        // 同一个响应体的写操作必须串行，避免模型分片与工具/思考回调并发写交错破坏帧格式
        private readonly SemaphoreSlim _writeLock = new(1, 1);

        public SseChatStreamOutput(HttpResponse response)
        {
            _response = response;
        }

        public async Task WriteAsync(string channel, string message)
        {
            // SSE 的 event/data 字段不能含裸换行：统一换行符后按行拆成多条 data:
            var payload = message ?? string.Empty;
            var sb = new StringBuilder();
            sb.Append("event: ").Append(channel).Append('\n');
            foreach (var line in payload.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
            {
                sb.Append("data: ").Append(line).Append('\n');
            }
            sb.Append('\n');
            var bytes = Utf8NoBom.GetBytes(sb.ToString());

            await _writeLock.WaitAsync();
            try
            {
                await _response.Body.WriteAsync(bytes, 0, bytes.Length);
                await _response.Body.FlushAsync();
            }
            finally
            {
                _writeLock.Release();
            }
        }
    }
}
