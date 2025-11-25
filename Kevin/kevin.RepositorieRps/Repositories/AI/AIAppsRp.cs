using kevin.Domain.Entities.AI;
using kevin.Domain.Interface;
using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.Domain.Interfaces.IRepositories
{ 
    public class AIAppsRp : Repository<TAIApps, Guid>, IAIAppsRp
    {
        public AIAppsRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
