using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
namespace Kevin.RepositorieRps.Repositories.AI
{
    /// <summary>
    /// AIAppsBindId�ִ�����ӿ�
    /// </summary>

    public class AIAppsBindIdRp : Repository<TAIAppsBindId, long>, IAIAppsBindIdRp
    {
        public AIAppsBindIdRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
