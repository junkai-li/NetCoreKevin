using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{

    public class UserRp : Repository<TUser, long>, IUserRp
    {
        public UserRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
