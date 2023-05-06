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
    public interface ILogRp : IRepository<TLog, Guid>
    {

    }
    public class LogRp : Repository<TLog, Guid>, ILogRp
    {
        public LogRp(dbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
        {
        }
    }
}
