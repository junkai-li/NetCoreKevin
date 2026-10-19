using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;

namespace Kevin.RepositorieRps.Repositories.AI
{
    /// <summary>
    /// 用户月度Token额度仓储实现
    /// </summary>
    public class AIUserTokenQuotaRp : Repository<TAIUserTokenQuota, long>, IAIUserTokenQuotaRp
    {
        public AIUserTokenQuotaRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
