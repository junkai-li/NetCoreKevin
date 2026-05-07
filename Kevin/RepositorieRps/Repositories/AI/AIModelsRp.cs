using kevin.Domain.Entities.AI;

namespace kevin.Domain.Interfaces.IRepositories
{
    public class AIModelsRp : Repository<TAIModels, long>, IAIModelsRp
    {
        public AIModelsRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
