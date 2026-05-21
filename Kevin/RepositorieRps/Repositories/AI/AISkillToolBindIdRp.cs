using kevin.Domain.Entities.AI;
using  kevin.Domain.Interfaces.IRepositories.AI;
namespace Kevin.RepositorieRps.Repositories.AI
{
    /// <summary>
    /// AISkillToolBindId�ִ�����ӿ�
    /// </summary>

    public class AISkillToolBindIdRp : Repository<TAISkillToolBindId, long>, IAISkillToolBindIdRp
    {
        public AISkillToolBindIdRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
} 
