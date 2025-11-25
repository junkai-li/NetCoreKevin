using kevin.Domain.Entities;
using Kevin.EntityFrameworkCore._.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.RepositorieRps.Repositories
{

    public class MessageReadRp : Repository<TMessageRead, Guid>, IMessageReadRp
    {
        public MessageReadRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
