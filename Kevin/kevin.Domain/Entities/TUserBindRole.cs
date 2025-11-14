using kevin.Domain.Kevin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities
{
    [Table("TUserBindRole")]
    [Description("用户绑定角色表")]
    public class TUserBindRole : CUD_User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Description("用户ID")]
        public Guid UserId { get; set; }
        public virtual TUser? User { get; set; } 
        /// <summary>
        /// 角色信息
        /// </summary>
        [Description("角色信息")]
        public Guid RoleId { get; set; }
        public virtual TRole? Role { get; set; }

    }
}
