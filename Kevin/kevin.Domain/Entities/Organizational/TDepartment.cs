using kevin.Domain.Share.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.Organizational
{
    /// <summary>
    /// 部门表
    /// </summary>
    [Table("TPosition")]
    [Description("部门表")]
    public class TDepartment : CUD_User
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        [Description("部门名称")]
        [MaxLength(100)]
        public string? Name { get; set; }

        /// <summary>
        /// 部门编码
        /// </summary>
        [Description("部门编码")]
        [MaxLength(100)]
        public string? Code { get; set; }


        /// <summary>
        /// 岗位描述
        /// </summary>
        [Description("岗位描述")]
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// 上级部门ID
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 部门负责人ID
        /// </summary>
        public long ManagerUserId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 状态（1.启用/禁用-1）
        /// </summary>
        public OrganizationalStatus Status { get; set; }
    }
}
