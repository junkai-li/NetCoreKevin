using kevin.Domain.Events;
using MediatR;

namespace kevin.Domain.EventHandlers
{
    public class TLogCreatedEventHandlers : INotificationHandler<TLogCreatedEvent>
    {
        public Task Handle(TLogCreatedEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("领域事件:输出控制台日志" + notification.Value.SerializeToJson());
            return Task.CompletedTask;
        }
    }
}
