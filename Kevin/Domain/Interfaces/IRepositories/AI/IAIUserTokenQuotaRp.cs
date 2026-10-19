using kevin.Domain.Entities.AI;

namespace kevin.Domain.Interfaces.IRepositories.AI
{
    /// <summary>
    /// 用户月度Token额度仓储接口
    /// </summary>
    public interface IAIUserTokenQuotaRp : IRepository<TAIUserTokenQuota, long>
    {
    }
}
