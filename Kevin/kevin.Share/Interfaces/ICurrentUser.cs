using kevin.Domain.Share.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web.Global.User
{
    public partial interface ICurrentUser
    {
        /// <summary>
        /// 当前登录用户Id
        /// </summary>
        long UserId { get; }
        /// <summary>
        /// 当前登录用户
        /// </summary>
        string UserName { get; }
        /// <summary>
        /// 当前登录用户租户code
        /// </summary>
        Int32 TenantId { get;   }

        /// <summary>
        /// 超级管理员
        /// </summary>
        bool IsSuperAdmin { get; }

        /// <summary>
        /// 其他信息
        /// </summary>
        dtoUserInfo? UserInfo { get;}

        /// <summary>
        /// 获取请求接口模块登录用户可查看数据访问的所有用户Id集合
        /// </summary>
        Task<List<long>> GetModuleDataPermissionsUserIds();

    }
}
