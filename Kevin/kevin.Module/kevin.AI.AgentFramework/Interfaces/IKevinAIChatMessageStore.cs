using kevin.AI.AgentFramework.Dto;

namespace kevin.AI.AgentFramework.Interfaces
{
    /// <summary>
    /// AI 消息存储接口
    /// </summary>
    public interface IKevinAIChatMessageStore
    {
        Task AddMessagesAsync(List<ChatHistoryItemDto> chatHistoryItems, CancellationToken cancellationToken);

        /// <summary>
        /// 读取线程的未压缩消息历史（按 Token 预算裁剪、最近 N 轮保底由 KevinChatMessageStore 负责）
        /// </summary>
        /// <param name="maxUserTurns">保留参数兼容调用方，查询不再按轮硬截断</param>
        Task<List<ChatHistoryItemDto>> GetMessagesAsync(string threadId, CancellationToken cancellationToken, int maxUserTurns = 0);
    }
}
