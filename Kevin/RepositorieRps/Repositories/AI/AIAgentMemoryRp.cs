using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
namespace Kevin.RepositorieRps.Repositories.AI
{
    /// <summary>
    /// 智能体记忆仓储实现
    /// </summary>
    public class AIAgentMemoryRp : Repository<TAIAgentMemory, long>, IAIAgentMemoryRp
    {
        public AIAgentMemoryRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
} 
