using kevin.Domain.Entities.AI;

namespace kevin.Domain.Interfaces.IRepositories
{
    public class AIKmsDetailsRp : Repository<TAIKmsDetails, long>, IAIKmsDetailsRp
    {
        public AIKmsDetailsRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
