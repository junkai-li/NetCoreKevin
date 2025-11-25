using kevin.Domain.Entities.AI;
using kevin.Domain.Interface;
using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.Domain.Interfaces.IRepositories
{  
    public class AIAIKmssRp : Repository<TAIKmss, Guid>, IAIKmssRp
    {
        public AIAIKmssRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
