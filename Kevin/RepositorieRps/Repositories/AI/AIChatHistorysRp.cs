using kevin.Domain.Entities.AI;

namespace kevin.Domain.Interfaces.IRepositories
{
    public class AIChatHistorysRp : Repository<TAIChatHistorys, long>, IAIChatHistorysRp
    {
        public AIChatHistorysRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
