using kevin.Domain.Share.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Share.Dtos.System
{
    public class dtoTenant
    {
        // <summary>
        /// 主键标识ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 租户编码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 租户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 租户状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>S
        public DateTime CreateTime { get; set; }
    }
}
