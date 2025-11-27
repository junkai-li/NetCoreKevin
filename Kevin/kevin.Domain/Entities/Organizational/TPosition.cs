using kevin.Domain.Kevin;
using kevin.Domain.Share.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.Organizational
{
    /// <summary>
    /// 岗位表
    /// </summary>
    [Table("TPosition")]
    [Description("岗位表")]
    public partial class TPosition : CUD_User
    {
        /// <summary>
        /// 岗位名称
        /// </summary>
        [Description("岗位名称")]
        [MaxLength(100)]
        public string? Name { get; set; }

        /// <summary>
        /// 岗位编码
        /// </summary>
        [Description("岗位编码")]
        [MaxLength(100)]
        public string? Code { get; set; }

        /// <summary>
        /// 岗位描述
        /// </summary>
        [Description("岗位描述")]
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// 上级岗位ID
        /// </summary>
        public long? ParentId { get; set; } 
        public virtual TPosition? Parent { get; set; }

        /// <summary>
        /// 状态（1.启用/禁用-1）
        /// </summary>
        public OrganizationalStatus Status { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

    }
}
