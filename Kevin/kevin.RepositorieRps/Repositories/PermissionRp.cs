using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{

    public class PermissionRp : Repository<TPermission, Guid>, IPermissionRp
    {
        public PermissionRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
