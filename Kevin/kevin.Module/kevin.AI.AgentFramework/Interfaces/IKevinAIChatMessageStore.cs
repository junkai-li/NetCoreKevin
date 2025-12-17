using kevin.AI.AgentFramework.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
