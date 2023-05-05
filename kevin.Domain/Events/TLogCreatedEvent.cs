using kevin.Domain.Kevin;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Events
{
    public record TLogCreatedEvent(TLog Value) : INotification;
}
