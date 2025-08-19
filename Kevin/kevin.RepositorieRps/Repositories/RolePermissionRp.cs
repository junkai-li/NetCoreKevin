using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{

    public class RolePermissionRp : Repository<TRolePermission, Guid>, IRolePermissionRp
    {
        public RolePermissionRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
