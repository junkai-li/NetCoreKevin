using kevin.Domain.Interfaces.IServices.AI;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// 聊天室输出通道适配器：一次问答的所有流式分片都推给“聊天室房间分组”（组名 = 会话 chatId），
    /// 走的是 <see cref="ChatRoomHub"/> 自己的分组——前端聊天室这条常驻连接加入的就是 ChatRoomHub 的房间组，
    /// 不能用 <see cref="SignalRChatStreamOutput"/> 那套（它推给 MySignalRHub 的组，跨 hub 收不到）。
    /// <para>
    /// 由于一个房间里可能同时有多条提问在并行回答，分片必须携带本次提问的 askId，
    /// 前端据此把流式内容路由到对应的回复气泡。这里把 { askId, channel, msg } 序列化为 JSON 字符串下发；
    /// askId 是雪花 long，按字符串下发以避免浏览器 Number 精度丢失。
    /// </para>
    /// </summary>
    internal sealed class RoomChatStreamOutput : IChatStreamOutput
    {
        private static readonly JsonSerializerOptions ChunkJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly IHubContext<ChatRoomHub> _hubContext;
        private readonly string _roomId;
        private readonly string _askId;

        public RoomChatStreamOutput(IHubContext<ChatRoomHub> hubContext, string roomId, string askId)
        {
            _hubContext = hubContext;
            _roomId = roomId;
            _askId = askId;
        }

        public Task WriteAsync(string channel, string message)
            => _hubContext.Clients.Group(_roomId).SendAsync("chatmsg",
                JsonSerializer.Serialize(new ChatRoomChunk(_askId, channel, message), ChunkJsonOptions));

        /// <summary>下发给前端的房间分片：askId 为本次提问记录 Id（字符串），channel 与流式事件名一一对应。</summary>
        private sealed record ChatRoomChunk(string AskId, string Channel, string Msg);
    }
}
