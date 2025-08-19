using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{
    public class RoleRp : Repository<TRole, Guid>, IRoleRp
    {
        public RoleRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
