using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationService
{
    /// <summary>
    /// 系统用户dto
    /// </summary>
    public class userDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否超级管理员
        /// </summary> 
        public   bool IsSuperAdmin { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 创建人;
        /// </summary> 
        public   string CreatedBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary> 
        public virtual DateTime? CreatedTime { get; set; }

        public int TenantId { get; set; }

    }
}
