using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{

    public class PermissionRp : Repository<TPermission, long>, IPermissionRp
    {
        public PermissionRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
