using kevin.Domain.Entities.AI;
using kevin.Domain.Interface;
using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.Domain.Interfaces.IRepositories
{ 
    public class AIChatHistorysRp : Repository<TAIChatHistorys, Guid>, IAIChatHistorysRp
    {
        public AIChatHistorysRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
