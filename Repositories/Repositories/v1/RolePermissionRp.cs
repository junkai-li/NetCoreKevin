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
    public interface IRolePermissionRp : IRepository<TRolePermission, Guid>
    {

    }
    public class RolePermissionRp : Repository<TRolePermission, Guid>, IRolePermissionRp
    {
        public RolePermissionRp(dbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
        {
        }
    }
}
