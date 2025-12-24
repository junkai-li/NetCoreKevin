using kevin.Domain.Entities;
using MediatR;

namespace kevin.Domain.Events
{
    public record TLogCreatedEvent(TLog Value) : INotification;
}
