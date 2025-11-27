using kevin.Application;
using kevin.Domain.Interfaces.IRepositories.Organizational;

namespace kevin.Domain.Interfaces.IServices.Organizational
{
    public class PositionService : BaseService, IPositionService
    {
        public IPositionRp positionRp { get; set; }
        public PositionService(IHttpContextAccessor _httpContextAccessor, IPositionRp _positionRp) : base(_httpContextAccessor)
        {
            positionRp= _positionRp;
        }
    }
}
