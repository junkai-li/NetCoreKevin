using MediatR;

namespace kevin.Domain.EventBus
{
    public interface IDomainEvents
    {

       /// <summary>
       /// 所有事件
       /// </summary>
       /// <returns></returns>
        IEnumerable<INotification> GetAllDomainEvents();
        IEnumerable<INotification> GetDomainEvents(EventBusEnums domainEvents = EventBusEnums.Other);
        void AddDomainEvent(INotification eventItem, EventBusEnums domainEvents = EventBusEnums.Other);
        /// <summary>
        /// 如果已经存在这个元素，则跳过，否则增加。以避免对于同样的事件触发多次（比如在一个事务中修改领域模型的多个对象）
        /// </summary>
        /// <param name="eventItem"></param>
        void AddDomainEventIfAbsent(INotification eventItem, EventBusEnums domainEvents = EventBusEnums.Other);
        public void ClearDomainEvents(EventBusEnums domainEvents = EventBusEnums.Other);
    }
}
