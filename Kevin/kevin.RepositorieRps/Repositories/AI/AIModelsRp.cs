using kevin.Domain.Entities.AI;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.Domain.Interfaces.IRepositories
{ 
    public class AIModelsRp : Repository<TAIModels, long>, IAIModelsRp
    {
        public AIModelsRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
