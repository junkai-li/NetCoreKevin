using kevin.Domain.Interfaces.IServices.AI;
using Kevin.SignalR.Service;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// SignalR 输出通道适配器：沿用改造前的行为，按提问记录 Id（身份标识）把分片私发给前端。
    /// </summary>
    internal sealed class SignalRChatStreamOutput : IChatStreamOutput
    {
        private readonly ISignalRMsgService _signalRMsgService;
        private readonly string _identityId;

        public SignalRChatStreamOutput(ISignalRMsgService signalRMsgService, string identityId)
        {
            _signalRMsgService = signalRMsgService;
            _identityId = identityId;
        }

        public Task WriteAsync(string channel, string message)
            => _signalRMsgService.SendIdentityIdMsg(channel, _identityId, message);
    }
}
