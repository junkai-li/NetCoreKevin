using kevin.Domain.Entities.AI;
using  kevin.Domain.Interfaces.IRepositories.AI;
namespace Kevin.RepositorieRps.Repositories.AI
{
    /// <summary>
    /// AIChatHistorysBindLogRp 
    /// </summary>

    public class AIChatHistorysBindLogRp : Repository<TAIChatHistorysBindLog, long>, IAIChatHistorysBindLogRp
    {
        public AIChatHistorysBindLogRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
} 
