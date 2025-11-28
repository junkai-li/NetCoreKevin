using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{ 
    public class UserInfoRp : Repository<TUserInfo, long>, IUserInfoRp
    {
        public UserInfoRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
