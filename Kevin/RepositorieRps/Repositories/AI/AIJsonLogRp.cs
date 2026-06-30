using kevin.Domain.Entities.AI;
using  kevin.Domain.Interfaces.IRepositories.AI;
namespace Kevin.RepositorieRps.Repositories.AI
{
    /// <summary>
    /// AIJsonLog�ִ�����ӿ�
    /// </summary>

    public class AIJsonLogRp : Repository<TAIJsonLog, long>, IAIJsonLogRp
    {
        public AIJsonLogRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
} 
