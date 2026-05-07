using kevin.Domain.Entities.AI;

namespace kevin.Domain.Interfaces.IRepositories
{
    public class AIAppsRp : Repository<TAIApps, long>, IAIAppsRp
    {
        public AIAppsRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
