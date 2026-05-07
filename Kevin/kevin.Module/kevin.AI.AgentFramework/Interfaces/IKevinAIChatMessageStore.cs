using kevin.AI.AgentFramework.Dto;

namespace kevin.AI.AgentFramework.Interfaces
{
    /// <summary>
    /// AI 消息存储接口
    /// </summary>
    public interface IKevinAIChatMessageStore
    {
        Task AddMessagesAsync(List<ChatHistoryItemDto> chatHistoryItems, CancellationToken cancellationToken);

        Task<List<ChatHistoryItemDto>> GetMessagesAsync(string threadId, CancellationToken cancellationToken);
    }
}
