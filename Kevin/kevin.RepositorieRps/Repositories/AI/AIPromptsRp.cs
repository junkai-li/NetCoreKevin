using kevin.Domain.Entities.AI;
using kevin.Domain.Interface;
using kevin.Domain.Kevin;
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
