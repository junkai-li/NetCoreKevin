using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationService
{
    /// <summary>
    /// 客户端Dto
    /// </summary>
    public class uMClientUserDto
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary> 
        public string Name { get; set; }



        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 性别
        /// </summary> 
        public bool? Sex { get; set; }

        /// <summary>
        /// 创建时间;
        /// </summary>
        public virtual DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 更新时间;
        /// </summary>
        public virtual DateTime? UpdatedTime { get; set; }

        public string TenantId { get; set; }
    }
}
