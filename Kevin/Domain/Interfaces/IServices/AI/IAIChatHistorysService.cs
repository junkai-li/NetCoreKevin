using kevin.Domain.Share.Dtos.AI;

namespace kevin.Domain.Interfaces.IServices.AI
{
    public interface IAIChatHistorysService
    {
        Task<dtoPageData<AIChatHistorysDto>> GetPageData(dtoPagePar<string> dtoPage);

        /// <summary>
        /// 获取当前会话的上下文使用量：未压缩历史消息 + 压缩摘要的估算 Token，与模型可用提问预算对比。
        /// <para>估算口径与历史裁剪/压缩判定一致（统一按 1 字符≈0.6 Token），不含系统提示词与工具定义。</para>
        /// </summary>
        /// <param name="aIChatsId">会话 Id</param>
        Task<AIContextUsageDto> GetContextUsage(long aIChatsId);
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

        /// <summary>
        /// 使用调用方自备的输出通道处理一条提问：与 <see cref="Add"/> / <see cref="AddSSE"/> / <see cref="AddToRoom"/>
        /// 共用同一套处理逻辑，仅由调用方决定流式分片的输出方式。
        /// <para>供 Web 层的协议适配器（如 OpenAI 协议兼容接口）复用整条智能体管线，新增输出协议时无需改动本服务。</para>
        /// </summary>
        /// <param name="par">聊天入参</param>
        /// <param name="outputFactory">入参为提问记录 Id（身份标识），返回本次请求使用的流式输出通道</param>
        /// <param name="cancellationToken">请求中止令牌</param>
        Task<AIChatHistorysDto> AddWithStreamOutput(AIChatHistorysDto par, Func<string, IChatStreamOutput> outputFactory, CancellationToken cancellationToken);

        Task<bool> Delete(long id);
    }
}
