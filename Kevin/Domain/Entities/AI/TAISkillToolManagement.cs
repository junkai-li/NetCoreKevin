using kevin.Domain.Share.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Entities.AI
{
    /// <summary>
    /// AI智能体技能工具管理
    /// </summary>
    [Table("TAISkillToolManagement")]
    [Description("AI智能体技能工具管理")]
    public class TAISkillToolManagement : CUD_User
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [Description("名称")]
        [StringLength(100)]
        public String Name { get; set; } = "";

        /// <summary>
        /// 方法 Tools 
        /// </summary>
        [Required]
        [Description("方法")]
        [StringLength(100)]
        public String? ClassMethod { get; set; } = "";

        /// <summary>
        ///路径 Skills  
        /// </summary>
        [Description("路径")]
        [StringLength(500)]
        public String? Url { get; set; } = "";

        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(500)]
        [Description("描述")]
        public String? Description { get; set; }

        /// <summary>
        /// 是否系统内置工具（系统内置工具不允许删除和修改）
        /// </summary>
        [Description("是否系统内置工具（系统内置工具不允许删除和修改）")]
        public bool IsSystem { get; set; } = false;

        /// <summary>
        /// 技能工具类型
        /// </summary>
        [Description("技能工具类型")]
        public AISkillToolTypeEnums SkillToolType { get; set; } = AISkillToolTypeEnums.Tool;



    }
}
