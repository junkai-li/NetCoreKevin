using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;

namespace kevin.RepositorieRps.Repositories.AI
{
    public class AIChatMessageStoreRp : Repository<TAIChatMessageStore, long>, IAIChatMessageStoreRp
    {
        public AIChatMessageStoreRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
