using kevin.Domain.Share.Dtos.Bases;
using kevin.Domain.Share.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kevin.Domain.Share.Dtos.Organizational
{
    public class DepartmentDto : CUD_User_Dto
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
        /// 部门描述
        /// </summary>
        [Description("部门描述")]
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// 上级部门ID
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 上级部门
        /// </summary>
        public long ParentName { get; set; }

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
