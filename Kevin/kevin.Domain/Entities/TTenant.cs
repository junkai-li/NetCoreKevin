using kevin.Domain.Events;
using kevin.Domain.Share.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities
{
    /// <summary>
    /// 租户表
    /// </summary>
    [Table("TTenant")]
    public partial class TTenant: CUD
    {
        public TTenant(Int32 code, string name)
        { 
            this.Id = Guid.NewGuid();
            this.Code= code;
            this.Name= name;
            this.Status = TenantStatusEnums.Active;
            this.CreateTime = DateTime.Now;
            this.IsDelete = false; 
        }

        /// <summary>
        /// 租户编码
        /// </summary>
        public Int32 Code { get; set; }
        /// <summary>
        /// 租户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 租户状态
        /// </summary>
        public TenantStatusEnums Status { get; set; } = TenantStatusEnums.Active;
         
    }
}
