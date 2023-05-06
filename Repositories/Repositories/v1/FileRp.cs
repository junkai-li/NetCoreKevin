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
    public interface IFileRp : IRepository<TFile, Guid>
    {

    }
    public class FileRp : Repository<TFile, Guid>, IFileRp
    {
        public FileRp(dbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
        {
        }
    }
}
