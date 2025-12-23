using kevin.Domain.Entities.AI;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.Domain.Interfaces.IRepositories
{ 
    public class AIKmsDetailsRp : Repository<TAIKmsDetails, long>, IAIKmsDetailsRp
    {
        public AIKmsDetailsRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
