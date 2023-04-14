using Ax.DataAccess;
using Repository.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Base;

namespace Service.Repositories.v1
{
    /// <summary>
    /// 仓储接口
    /// </summary>
    public interface IUserRp : IRepository<TUser, Guid>
    {

    }
    public class UserRp : BaseRepository<TUser, Guid>, IUserRp
    {
        public UserRp(dbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
        {
        }
    }
}
