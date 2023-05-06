using Ax.DataAccess;
using kevin.Domain.Interface;
using kevin.Domain.Kevin;
using Repository.Database;

namespace Service.Repositories.v1
{
    /// <summary>
    /// 仓储接口
    /// </summary>
    public interface IUserRp : IRepository<TUser, Guid>
    {

    }
    public class UserRp : Repository<TUser, Guid>, IUserRp
    {
        public UserRp(dbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
        {
        }
    }
}
