using kevin.Domain.Entities.AI;
using kevin.Domain.Interface;
using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.Domain.Interfaces.IRepositories
{ 
    public class AIModelsRp : Repository<TAIKmss, Guid>, IAIKmssRp
    {
        public AIModelsRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
