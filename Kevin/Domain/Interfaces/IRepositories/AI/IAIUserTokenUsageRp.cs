using kevin.Domain.Entities.AI;

namespace kevin.Domain.Interfaces.IRepositories.AI
{
    /// <summary>
    /// 用户Token使用明细仓储接口
    /// </summary>
    public interface IAIUserTokenUsageRp : IRepository<TAIUserTokenUsage, long>
    {
    }
}
