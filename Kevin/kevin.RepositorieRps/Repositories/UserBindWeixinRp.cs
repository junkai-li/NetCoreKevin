using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{

    public class UserBindWeixinRp : Repository<TUserBindWeixin, long>, IUserBindWeixinRp
    {
        public UserBindWeixinRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
