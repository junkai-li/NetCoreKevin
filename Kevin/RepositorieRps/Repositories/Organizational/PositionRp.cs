using kevin.Domain.Entities.Organizational;

namespace kevin.Domain.Interfaces.IRepositories.Organizational
{
    public class PositionRp : Repository<TPosition, long>, IPositionRp
    {
        public PositionRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
