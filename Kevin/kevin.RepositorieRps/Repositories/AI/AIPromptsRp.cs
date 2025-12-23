using kevin.Domain.Entities.AI;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.Domain.Interfaces.IRepositories
{ 
    public class AIPromptsRp : Repository<TAIPrompts, long>, IAIPromptsRp
    {
        public AIPromptsRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
