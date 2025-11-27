using kevin.Domain.Entities.Organizational;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.Domain.Interfaces.IRepositories.Organizational
{  
    public class DepartmentRp : Repository<TDepartment, long>, IDepartmentRp
    {
        public DepartmentRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
