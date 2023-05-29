using App.Domain.Interfaces.Repositorie.v1;
using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace App.RepositorieRps.Repositories.v1
{

    public class RolePermissionRp : Repository<TRolePermission, Guid>, IRolePermissionRp
    {
        public RolePermissionRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
