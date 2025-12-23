using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{

    public class RolePermissionRp : Repository<TRolePermission, long>, IRolePermissionRp
    {
        public RolePermissionRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
