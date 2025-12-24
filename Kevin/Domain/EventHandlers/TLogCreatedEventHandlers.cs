using kevin.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
