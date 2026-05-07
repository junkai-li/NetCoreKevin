using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;

namespace kevin.RepositorieRps.Repositories.AI
{
    public class AIChatsRp : Repository<TAIChats, long>, IAIChatsRp
    {
        public AIChatsRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
