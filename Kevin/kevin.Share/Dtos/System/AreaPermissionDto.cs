using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kevin.Permission.Permission
{
    public class AreaPermissionDto
    {
        public string area { get; set; }
        public string areaName { get; set; }

        public List<ModulePermissionDto> modules { get; set; }
    }

    public class ModulePermissionDto
    {
        public string ModuleName { get; set; }
        public string Module { get; set; }

        public List<ActionPermissionDto> actions { get; set; }
    }
    public class ActionPermissionDto
    {
        public string Id { get; set; }
        public string ActionName { get; set; }
        public string Action { get; set; }
        public string FullName { get; set; }
        /// <summary>
        /// Method;
        /// </summary> 
        public string HttpMethod { get; set; }

        /// <summary>
        /// 手动添加;
        /// </summary>
        public bool? IsManual { get; set; }

        /// <summary>
        /// 是否有权限？
        /// </summary>
        public bool IsPermission { get; set; }
        /// <summary>
        /// 序号;
        /// </summary>
        public int? Seq { get; set; }
        /// <summary>
        /// 图标;
        /// </summary> 
        public string Icon { get; set; }
        /// <summary>
        /// 创建人;
        /// </summary> 
        public string CreatedBy { get; set; }
        /// <summary>
        /// 创建时间;
        /// </summary>
        public DateTime? CreatedTime { get; set; }
        /// <summary>
        /// 编辑人;
        /// </summary> 
        public string UpdatedBy { get; set; }
        /// <summary>
        /// 更新时间;
        /// </summary>
        public DateTime? UpdatedTime { get; set; }
    }
}
