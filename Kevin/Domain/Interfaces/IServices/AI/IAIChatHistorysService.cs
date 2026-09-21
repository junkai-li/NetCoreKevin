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

        Task<bool> Delete(long id);
    }
}
