using kevin.Domain.Entities.AI;
using MediatR;

namespace kevin.Domain.Events
{
    /// <summary>
    /// 聊天历史创建事件（回复记录入库时发布，用于按聊天历史记录 Id 记录本轮 Token 使用明细）
    /// </summary>
    public record TAIChatHistorysCreatedEvent(TAIChatHistorys Value) : INotification;
}
