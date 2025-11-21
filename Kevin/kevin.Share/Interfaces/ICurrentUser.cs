using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Global.User
{
    public interface ICurrentUser
    {
        /// <summary>
        /// 当前登录用户Id
        /// </summary>
        Guid UserId { get; }
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

    }
}
