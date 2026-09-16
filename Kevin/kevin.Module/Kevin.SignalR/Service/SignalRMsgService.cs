using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Web.Global.User;

namespace Kevin.SignalR.Service
{
    /// <summary>
    /// 推送目标改用 SignalR 分组：组名即业务侧的 identityId。
    /// <para>
    /// 分组归属由 SignalR 与其 Redis 背板维护（连接结束即自动退出组），
    /// 因此这里不再自己往 Redis 里维护一份“身份→连接”映射表：
    /// 既没有“每次推送都整份读回全租户映射表”的读放大，
    /// 也没有“读整表→改→写整表”在并发连接时互相覆盖的问题，
    /// 更不会留下 Pod 重启/页面强关后清不掉的死连接。
    /// </para>
    /// </summary>
    public class SignalRMsgService : ISignalRMsgService
    {
        private readonly IHubContext<MySignalRHub> _messageHub;

        public ICurrentUser _currentUser { get; set; }

        private readonly ILogger<SignalRMsgService> _logger;

        public SignalRMsgService(IHubContext<MySignalRHub> messageHub, ICurrentUser currentUser, ILogger<SignalRMsgService> logger)
        {
            _messageHub = messageHub;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task SendPublicMsg(string method, string msg)
        {
            await SendSafe("All", () => _messageHub.Clients.All.SendAsync(method, msg));
        }

        public async Task SendConnIdMsg(string method, string connId, string msg)
        {
            if (string.IsNullOrEmpty(connId))
            {
                return;
            }
            await SendSafe($"Client({connId})", () => _messageHub.Clients.Client(connId).SendAsync(method, msg));
        }

        public async Task SendConnIdsMsg(string method, List<string> connIds, string msg)
        {
            if (connIds.Count <= 0)
            {
                return;
            }
            await SendSafe($"Clients({connIds.Count})", () => _messageHub.Clients.Clients(connIds).SendAsync(method, msg));
        }

        public async Task SendIdentityIdMsg(string method, string identityId, string msg)
        {
            if (string.IsNullOrEmpty(identityId))
            {
                return;
            }
            await SendSafe($"Group({identityId})", () => _messageHub.Clients.Group(identityId).SendAsync(method, msg));
        }

        public async Task SendIdentityIdsMsg(string method, List<string> identityIds, string msg)
        {
            var groups = identityIds.Where(t => !string.IsNullOrEmpty(t)).Distinct().ToList();
            if (groups.Count <= 0)
            {
                return;
            }
            await SendSafe($"Groups({groups.Count})", () => _messageHub.Clients.Groups(groups).SendAsync(method, msg));
        }

        /// <summary>
        /// 推送统一出口：SignalR/背板抖动只记日志不抛异常。
        /// <para>
        /// 推送是尽力而为的旁路：前端断开、背板不可用只应丢一次展示，
        /// 不该把已在正常输出的 AI 回答当成失败（更不能让异常冒出 async void 去弄死进程）。
        /// </para>
        /// </summary>
        private async Task SendSafe(string target, Func<Task> send)
        {
            try
            {
                await send();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SignalR 推送失败已忽略，目标：{Target}", target);
            }
        }
    }
}
