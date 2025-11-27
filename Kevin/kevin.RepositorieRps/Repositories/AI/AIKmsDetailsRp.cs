using kevin.Domain.Entities.AI;
using kevin.Domain.Interface;
using kevin.Domain.Kevin;
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
