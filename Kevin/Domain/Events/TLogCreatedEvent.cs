using MediatR;

namespace kevin.Domain.Events
{
    public record TLogCreatedEvent(TLog Value) : INotification;
}
