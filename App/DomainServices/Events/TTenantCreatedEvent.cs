using kevin.Domain.Entities;
using MediatR;

namespace App.Domain.Events
{
    public record TTenantCreatedEvent(TTenant Value) : INotification;
 
}
