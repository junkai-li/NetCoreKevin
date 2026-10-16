using kevin.Domain.Entities.AI;

namespace kevin.RepositorieRps.Repositories.AI
{
    public class TAIOpenAICompatibleKeyRp : Repository<TAIOpenAICompatibleKey, long>, ITAIOpenAICompatibleKeyRp
    {
        public TAIOpenAICompatibleKeyRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}