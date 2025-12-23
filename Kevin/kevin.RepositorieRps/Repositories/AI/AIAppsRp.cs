using kevin.Domain.Entities.AI;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.Domain.Interfaces.IRepositories
{ 
    public class AIAppsRp : Repository<TAIApps, long>, IAIAppsRp
    {
        public AIAppsRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
