using kevin.Domain.Share.Dtos.AI;

namespace kevin.Domain.Interfaces.IServices.AI
{
    public interface IAIChatHistorysService
    {
        Task<dtoPageData<AIChatHistorysDto>> GetPageData(dtoPagePar<string> dtoPage);
        Task<AIChatHistorysDto> Add(AIChatHistorysDto par, CancellationToken cancellationToken);

        /// <summary>
        /// 新增AI对话聊天记录（SSE 流式输出）：与 <see cref="Add"/> 共用同一套处理逻辑，
        /// 仅把处理过程中的流式分片改为通过 SSE（text/event-stream）写回当前 HTTP 响应，而不是走 SignalR 推送。
        /// </summary>
        /// <param name="par">聊天入参</param>
        /// <param name="cancellationToken">请求中止令牌</param>
        Task AddSSE(AIChatHistorysDto par, CancellationToken cancellationToken);

        /// <summary>
        /// 聊天室发送：与 <see cref="Add"/> 共用同一套处理逻辑，仅把流式分片改为推给“房间分组”
        /// （组名 = <paramref name="roomId"/>，即会话 chatId），分片携带本次提问的 askId 供前端并行路由。
        /// </summary>
        /// <param name="par">聊天入参</param>
        /// <param name="roomId">房间分组名（会话 chatId）</param>
        /// <param name="cancellationToken">连接中止令牌</param>
        Task<AIChatHistorysDto> AddToRoom(AIChatHistorysDto par, long roomId, CancellationToken cancellationToken);

        Task<bool> Delete(long id);
    }
}
