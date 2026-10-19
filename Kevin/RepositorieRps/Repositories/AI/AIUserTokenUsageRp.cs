using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;

namespace Kevin.RepositorieRps.Repositories.AI
{
    /// <summary>
    /// 用户Token使用明细仓储实现
    /// </summary>
    public class AIUserTokenUsageRp : Repository<TAIUserTokenUsage, long>, IAIUserTokenUsageRp
    {
        public AIUserTokenUsageRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
