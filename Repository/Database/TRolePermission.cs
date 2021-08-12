using Repository.Database.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Database
{
    public class TRolePermission: CD_User
    {
        /// <summary>
        /// 角色编号;
        /// </summary>
        public virtual Guid RoleId { get; set; } 
        /// <summary>
        /// 角色编号;
        /// </summary>
        public virtual TRole Role { get; set; }

        /// <summary>
        /// 权限编号;
        /// </summary>
        public virtual string PermissionId { get; set; }

        /// <summary>
        /// 权限编号;
        /// </summary>
        public virtual TPermission Permission { get; set; }
    }
}
