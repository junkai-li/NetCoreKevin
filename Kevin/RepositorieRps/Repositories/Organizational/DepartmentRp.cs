using kevin.Domain.Entities.Organizational;

namespace kevin.Domain.Interfaces.IRepositories.Organizational
{
    public class DepartmentRp : Repository<TDepartment, long>, IDepartmentRp
    {
        public DepartmentRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
