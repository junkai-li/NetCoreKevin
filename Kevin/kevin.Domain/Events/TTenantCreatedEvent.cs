using kevin.Domain.Entities;
using MediatR;

namespace kevin.Domain.Events
{
    public record TTenantCreatedEvent(TTenant Value) : INotification;
 
}
