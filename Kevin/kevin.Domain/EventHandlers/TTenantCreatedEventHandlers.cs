using kevin.Domain.Events;
using kevin.Domain.Interfaces.IServices;
using MediatR;

namespace kevin.Domain.EventHandlers
{
    public class TTenantCreatedEventHandlers : INotificationHandler<TTenantCreatedEvent>
    {
        ITenantService _TenantService;
        public TTenantCreatedEventHandlers(ITenantService tenantService)
        {
            _TenantService = tenantService;
        }
        public Task Handle(TTenantCreatedEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("租户创建领域事件:输出控制台日志" + notification.Value.SerializeToJson());
            _TenantService.InitializedData(new kevin.Domain.Share.Dtos.System.dtoTenant
            {
                Id = notification.Value.Id.ToString(),
                Code = notification.Value.Code,
                Name = notification.Value.Name,
                CreateTime = notification.Value.CreateTime
            });
            return Task.CompletedTask;
        }
    }
}
