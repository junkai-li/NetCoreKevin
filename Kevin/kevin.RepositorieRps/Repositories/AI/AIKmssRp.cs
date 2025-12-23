using kevin.Domain.Entities.AI;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.Domain.Interfaces.IRepositories
{  
    public class AIAIKmssRp : Repository<TAIKmss, long>, IAIKmssRp
    {
        public AIAIKmssRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
