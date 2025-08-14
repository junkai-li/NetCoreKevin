using kevin.Domain.Share.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities
{
    public class TTenant
    {
        // <summary>
        /// 主键标识ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 租户编码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 租户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 租户名称
        /// </summary>
        public TenantStatusEnums Status { get; set; } = TenantStatusEnums.Active;
        /// <summary>
        /// 创建时间
        /// </summary>S
        public DateTime CreateTime { get; set; }  

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }



        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeleteTime { get; set; }
    }
}
