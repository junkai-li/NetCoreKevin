using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
namespace Kevin.RepositorieRps.Repositories.AI
{
    /// <summary>
    /// AI 自动任务配置仓储实现
    /// </summary>
    public class AITaskConfigRp : Repository<TAITaskConfig, long>, IAITaskConfigRp
    {
        public AITaskConfigRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
