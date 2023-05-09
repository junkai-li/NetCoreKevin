using App.Domain.Interfaces.Repositorie.v1;
using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace Service.Repositories.v1
{

    public class UserRp : Repository<TUser, Guid>, IUserRp
    {
        public UserRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
