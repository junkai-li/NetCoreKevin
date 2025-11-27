using kevin.Application;
using kevin.Domain.Interfaces.IRepositories.Organizational;

namespace kevin.Domain.Interfaces.IServices.Organizational
{ 
    public class DepartmentService : BaseService, IDepartmentService
    {
        public IDepartmentRp departmentRp { get; set; }
        public DepartmentService(IHttpContextAccessor _httpContextAccessor, IDepartmentRp _departmentRp) : base(_httpContextAccessor)
        {
            departmentRp= _departmentRp;
        }
    }
}
