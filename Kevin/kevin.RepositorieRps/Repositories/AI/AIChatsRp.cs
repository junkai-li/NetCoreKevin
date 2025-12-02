using kevin.Domain.Entities.AI;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories.AI
{ 
    public class AIChatsRp : Repository<TAIChats, long>, IAIChatsRp
    {
        public AIChatsRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
