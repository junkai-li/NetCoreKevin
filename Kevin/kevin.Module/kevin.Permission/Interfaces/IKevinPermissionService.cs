using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Permission.Interfaces
{
    public interface IKevinPermissionService
    {
        /// <summary>
        /// 验证权限
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        bool IsAccess(string permissionId, IHttpContextAccessor httpContext);

        /// <summary>
        /// 获取用户所拥有的权限列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Dictionary<string, bool> GetUserPermissions(string userId);
    }
}
