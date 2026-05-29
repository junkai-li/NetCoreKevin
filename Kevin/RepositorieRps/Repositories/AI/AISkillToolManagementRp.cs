using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
namespace Kevin.RepositorieRps.Repositories.AI
{
    /// <summary>
    /// AISkillToolManagement�ִ�����ӿ�
    /// </summary>

    public class AISkillToolManagementRp : Repository<TAISkillToolManagement, long>, IAISkillToolManagementRp
    {
        public AISkillToolManagementRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
