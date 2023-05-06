using Ax.DataAccess;
using kevin.Domain.Interface;
using kevin.Domain.Kevin;
using Repository.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.RepositorieRps.Repositories.v1
{ 
    /// <summary>
    /// 仓储接口
    /// </summary>
    public interface IWeiXinKeyRp : IRepository<TWeiXinKey, Guid>
    {

    }
    public class WeiXinKeyRp : Repository<TWeiXinKey, Guid>, IWeiXinKeyRp
    {
        public WeiXinKeyRp(dbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
        {
        }
    }
}
