using App.Domain.Interfaces.Repositorie.v1;
using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace App.RepositorieRps.Repositories.v1
{

    public class PermissionRp : Repository<TPermission, Guid>, IPermissionRp
    {
        public PermissionRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
