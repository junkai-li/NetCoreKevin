using kevin.Domain.Entities.AI;
using  kevin.Domain.Interfaces.IRepositories.AI;
namespace Kevin.RepositorieRps.Repositories.AI
{
    /// <summary>
    /// AIChatMessageStoreCompaction�ִ�����ӿ�
    /// </summary>

    public class AIChatMessageStoreCompactionRp : Repository<TAIChatMessageStoreCompaction, long>, IAIChatMessageStoreCompactionRp
    {
        public AIChatMessageStoreCompactionRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
} 
