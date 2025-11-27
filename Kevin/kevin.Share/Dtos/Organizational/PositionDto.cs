using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kevin.Domain.Share.Enums;
using kevin.Domain.Share.Dtos.Bases;

namespace kevin.Domain.Share.Dtos.Organizational
{
    public class PositionDto : CUD_User_Dto
    {  

        /// <summary>
        /// 岗位名称
        /// </summary> 
        public string? Name { get; set; }

        /// <summary>
        /// 岗位编码
        /// </summary> 
        public string? Code { get; set; }

        /// <summary>
        /// 岗位描述
        /// </summary> 
        public string? Description { get; set; }

        /// <summary>
        /// 上级岗位ID
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 上级岗位ID
        /// </summary>
        public long ParentName { get; set; }

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
