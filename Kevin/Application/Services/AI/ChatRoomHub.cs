using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Web.Global.Exceptions;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// 一对一智能体聊天室 Hub：一条常驻双向连接承载一个会话房间。
    /// <para>
    /// 与纯下行的 <see cref="Kevin.SignalR.MySignalRHub"/> 不同，这里前端通过 <see cref="SendMessage"/>
    /// 直接把问题发进来，后端异步处理后经房间分组流式回推，支持多问并行、不必等待上一条回答。
    /// </para>
    /// </summary>
    public class ChatRoomHub : Hub
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ChatRoomHub(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>房间分组名：连接 query 的 IdentityId（= 会话 chatId）。</summary>
        private string RoomId => Context.GetHttpContext()?.Request.Query["IdentityId"].FirstOrDefault() ?? "";

        /// <summary>
        /// 连接建立：加入以 chatId 命名的房间分组，之后本次会话的所有分片都推给该分组。
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var roomId = RoomId;
            if (!string.IsNullOrEmpty(roomId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            }
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 接收前端问题：只做入参校验并立即返回（ack），真正的处理丢到独立后台 scope 里跑。
        /// <para>
        /// SignalR 对同一连接的调用是串行派发的：若在方法内 <c>await</c> 整个 AddToRoom，
        /// 第二个问题要等第一个完全回答完才会开始处理。立即返回后连接派发循环被释放，多问才真正并行。
        /// </para>
        /// </summary>
        public Task SendMessage(ChatRoomSendRequest req)
        {
            if (req == null || string.IsNullOrEmpty(req.Content))
            {
                return Task.CompletedTask;
            }
            // SignalR 默认 JSON 协议不会把字符串读成 long，而 19 位雪花 Id 用 JS number 会丢精度，
            // 故入参雪花字段一律按字符串传入，服务端在此解析。
            if (!long.TryParse(req.AIChatsId, out var aiChatsId) || aiChatsId <= 0)
            {
                return Task.CompletedTask;
            }
            if (!long.TryParse(req.AskId, out var askId) || askId <= 0)
            {
                // askId 用于把并行多条回答的流式分片路由回对应气泡，必须携带
                return Task.CompletedTask;
            }
            long.TryParse(req.RetryOfId, out var retryOfId);

            // 连接握手请求（含 Query["Authorization"]）在 WebSocket 存活期内仍可读，捕获后交给后台任务桥接用户上下文
            var httpContext = Context.GetHttpContext();
            // fire-and-forget：后台任务内部已全量 try/catch，异常不会冒出成无人接住的 Task
            _ = ProcessAsync(httpContext, req, aiChatsId, askId, retryOfId);
            return Task.CompletedTask;
        }

        /// <summary>后台处理单个提问：独立 DI scope + 独立异步流，保证多问并行且互不串用 DbContext / 用户上下文。</summary>
        private async Task ProcessAsync(HttpContext? httpContext, ChatRoomSendRequest req, long aiChatsId, long askId, long retryOfId)
        {
            var groupName = aiChatsId.ToString();
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var sp = scope.ServiceProvider;
                // 桥接用户上下文：必须在解析应用服务之前完成——BaseService 构造时即读取 accessor。
                // accessor 是基于 AsyncLocal 的单例，本后台任务的异步流内设置，与其他并行提问互不干扰。
                sp.GetRequiredService<IHttpContextAccessor>().HttpContext = httpContext;

                var chatService = sp.GetRequiredService<IAIChatHistorysService>();
                var hubContext = sp.GetRequiredService<IHubContext<ChatRoomHub>>();
                var logger = sp.GetRequiredService<ILogger<ChatRoomHub>>();
                // 与 HTTP Add 返回复用同一套 MVC JsonSerializerOptions（camelCase + long→字符串），
                // 保证 askId、result.id 等雪花 Id 不以会丢精度的数字下发，前端可直接按 askId 匹配气泡。
                var jsonOptions = sp.GetRequiredService<IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>().Value.JsonSerializerOptions;

                var dto = new AIChatHistorysDto
                {
                    AIChatsId = aiChatsId,
                    Id = askId,
                    Content = req.Content,
                    IsSend = true,
                    IsOnlineSearch = req.IsOnlineSearch,
                    FileNames = req.FileNames,
                    ContentFileUrls = req.ContentFileUrls,
                    RetryOfId = retryOfId,
                };

                try
                {
                    // 用 CancellationToken.None：后台跑完并落库，与 HTTP Add 即使客户端离开也会完成一致
                    var result = await chatService.AddToRoom(dto, aiChatsId, CancellationToken.None);
                    await SafeSendAsync(hubContext, logger, groupName, "chatdone",
                        JsonSerializer.Serialize(new { askId = req.AskId, result }, jsonOptions));
                }
                catch (UserFriendlyException ex)
                {
                    // 业务错误（内容为空、权限不足、达上限等）：回传可读提示，不冒泡断连
                    await SafeSendAsync(hubContext, logger, groupName, "chaterror",
                        JsonSerializer.Serialize(new { askId = req.AskId, message = ex.Message }, jsonOptions));
                }
            }
            catch (Exception ex)
            {
                // 后台任务兜底：非业务异常只记录不抛出（此 Task 已被丢弃，不能让未观察异常冒泡）
                using var fallbackScope = _serviceScopeFactory.CreateScope();
                fallbackScope.ServiceProvider.GetService<ILogger<ChatRoomHub>>()
                    ?.LogError(ex, "聊天室后台处理提问失败 askId={AskId} group={Group}", req.AskId, groupName);
            }
        }

        /// <summary>房间推送尽力而为：客户端可能已断开，SendAsync 自身异常只记日志不冒泡。</summary>
        private static async Task SafeSendAsync(IHubContext<ChatRoomHub> hub, ILogger logger, string group, string evt, string data)
        {
            try
            {
                await hub.Clients.Group(group).SendAsync(evt, data);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "聊天室推送失败已忽略 event={Evt} group={Group}", evt, group);
            }
        }
    }

    /// <summary>聊天室发送入参。</summary>
    public class ChatRoomSendRequest
    {
        /// <summary>会话 Id（同时作为房间分组名）；雪花 long，按字符串传入避免丢精度</summary>
        public string AIChatsId { get; set; } = "";
        /// <summary>本次提问记录 Id（前端预生成的雪花 Id，字符串）</summary>
        public string AskId { get; set; } = "";
        /// <summary>提问内容</summary>
        public string Content { get; set; } = "";
        /// <summary>是否联网搜索</summary>
        public bool IsOnlineSearch { get; set; }
        /// <summary>文件名，多个用,隔开</summary>
        public string? FileNames { get; set; }
        /// <summary>文件 url，多个用,隔开</summary>
        public string? ContentFileUrls { get; set; }
        /// <summary>重试来源：上次失败的提问记录 Id（字符串），为空或 0 表示普通发送</summary>
        public string? RetryOfId { get; set; }
    }
}
