using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{

    public class UserBindWeixinRp : Repository<TUserBindWeixin, Guid>, IUserBindWeixinRp
    {
        public UserBindWeixinRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
