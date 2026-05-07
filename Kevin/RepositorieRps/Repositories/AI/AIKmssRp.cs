using kevin.Domain.Entities.AI;

namespace kevin.Domain.Interfaces.IRepositories
{
    public class AIAIKmssRp : Repository<TAIKmss, long>, IAIKmssRp
    {
        public AIAIKmssRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
