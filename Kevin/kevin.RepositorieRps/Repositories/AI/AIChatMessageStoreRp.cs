using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using Kevin.EntityFrameworkCore._.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.RepositorieRps.Repositories.AI
{ 
    public class AIChatMessageStoreRp : Repository<TAIChatMessageStore, long>, IAIChatMessageStoreRp
    {
        public AIChatMessageStoreRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
