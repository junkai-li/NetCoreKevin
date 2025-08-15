using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.EventBus
{
    /// <summary>
    /// 领域事件基础类
    /// </summary>
    public class DEBaseEntity : IDomainEvents
    {
        [NotMapped]
        private List<INotification> domainEvents = new();

        [NotMapped]
        private List<INotification> AdddomainEvents = new();

        [NotMapped]
        private List<INotification> DeleteddomainEvents = new();

        [NotMapped]
        private List<INotification> ModifieddomainEvents = new();
         

        public void AddDomainEvent(INotification eventItem, EventBusEnums eventBusEnums= EventBusEnums.Other)
        {
            switch (eventBusEnums)
            {
                case EventBusEnums.Add:
                    AdddomainEvents.Add(eventItem);
                    break;
                case EventBusEnums.Deleted:
                    DeleteddomainEvents.Add(eventItem);
                    break;
                case EventBusEnums.Modified:
                    ModifieddomainEvents.Add(eventItem);
                    break;
                default:
                    domainEvents.Add(eventItem);
                    break;
            }
        }

        public void  AddDomainEventIfAbsent(INotification eventItem, EventBusEnums eventBusEnums= EventBusEnums.Other)
        {
            switch (eventBusEnums)
            {
                case EventBusEnums.Add:
                    if (!AdddomainEvents.Contains(eventItem))
                    {
                        AdddomainEvents.Add(eventItem);
                    } 
                    break;
                case EventBusEnums.Deleted:
                    if (!DeleteddomainEvents.Contains(eventItem))
                    {
                        DeleteddomainEvents.Add(eventItem);
                    } 
                    break;
                case EventBusEnums.Modified:
                    if (!ModifieddomainEvents.Contains(eventItem))
                    {
                        ModifieddomainEvents.Add(eventItem);
                    } 
                    break;
                default:
                    if (!domainEvents.Contains(eventItem))
                    {
                        domainEvents.Add(eventItem);
                    } 
                    break;
            }
        }

        public void  ClearDomainEvents(EventBusEnums eventBusEnums= EventBusEnums.Other)
        {
            switch (eventBusEnums)
            {
                case EventBusEnums.Add: 
                    AdddomainEvents.Clear();
                    break;
                case EventBusEnums.Deleted: 
                    DeleteddomainEvents.Clear();
                    break;
                case EventBusEnums.Modified: 
                    ModifieddomainEvents.Clear();
                    break;
                default:
                    domainEvents.Clear();
                    break;
            }
        }

        public IEnumerable<INotification> GetDomainEvents(EventBusEnums eventBusEnums= EventBusEnums.Other)
        {
            switch (eventBusEnums)
            {
                case EventBusEnums.Add: 
                    return AdddomainEvents; 
                case EventBusEnums.Deleted: 
                    return DeleteddomainEvents;
                case EventBusEnums.Modified: 
                    return ModifieddomainEvents;
                default:
                    return domainEvents;
            } 
        }
        public IEnumerable<INotification> GetAllDomainEvents()
        {
            domainEvents.AddRange(AdddomainEvents);
            domainEvents.AddRange(DeleteddomainEvents);
            domainEvents.AddRange(ModifieddomainEvents);
            return domainEvents;
        }
    }
}
