using kevin.Domain.Entities.AI;

namespace kevin.Domain.Interfaces.IRepositories
{
    public class AIPromptsRp : Repository<TAIPrompts, long>, IAIPromptsRp
    {
        public AIPromptsRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
