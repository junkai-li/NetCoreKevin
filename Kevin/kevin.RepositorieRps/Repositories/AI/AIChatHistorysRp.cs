using kevin.Domain.Entities.AI;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.Domain.Interfaces.IRepositories
{ 
    public class AIChatHistorysRp : Repository<TAIChatHistorys, long>, IAIChatHistorysRp
    {
        public AIChatHistorysRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
