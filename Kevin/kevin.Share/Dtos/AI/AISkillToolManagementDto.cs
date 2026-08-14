using kevin.Domain.Share.Dtos.Bases;
using kevin.Domain.Share.Dtos.System;
using kevin.Domain.Share.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kevin.Domain.Share.Dtos.AI
{
    public class AISkillToolManagementDto : CUD_User_Dto
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
        [Description("方法")]
        [StringLength(100)]
        public String? ClassMethod { get; set; } = "";
        /// <summary>
        /// Mcp地址
        /// </summary> 
        [Description("Mcp地址")]
        public String? McpUrl { get; set; } = "";

        /// <summary>
        /// McpType https,sse,stdio
        /// </summary> 
        [Description("McpType McpType https,sse,stdio")]
        [StringLength(100)]
        public String? McpType { get; set; } = "";

        /// <summary>
        /// McpHeaders
        /// </summary> 
        [Description("McpHeaders 键值对Json格式")]
        public String? McpHeaders { get; set; } = "";
        /// <summary>
        /// McpCommand
        /// </summary> 
        [Description("McpCommand")]
        public String? McpCommand { get; set; } = "";
        /// <summary>
        /// McpArguments
        /// </summary> 
        [Description("McpArguments ,分隔")]
        public String? McpArguments { get; set; } = "";

        /// <summary>
        /// McpEnvironment
        /// </summary> 
        [Description("McpEnvironment 键值对Json格式")]
        public String? McpEnvironment { get; set; } = "";
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
        /// 是否启用
        /// </summary>
        [Description("是否启用")]
        public InActiveStatusEnums ActiveStatus { get; set; } = InActiveStatusEnums.Active;

        /// <summary>
        /// 技能工具类型
        /// </summary>
        [Description("技能工具类型")]
        public AISkillToolTypeEnums SkillToolType { get; set; } = AISkillToolTypeEnums.Tool;

        /// <summary>
        /// skill工具关联的文件信息
        /// </summary>
        public FileDto? SkillFile { get; set; }

        /// <summary>
        /// 检验
        /// </summary>
        public void Check()
        { 
            if (this.SkillToolType == AISkillToolTypeEnums.Mcp)
            {
                if (string.IsNullOrEmpty(this.McpUrl) && this.McpType != "stdio")
                {
                    throw new Exception("McpUrl不能为空");
                }
                if (string.IsNullOrEmpty(this.McpType))
                {
                    throw new Exception("McpType不能为空");
                }
                if (this.McpType == "stdio" && string.IsNullOrEmpty(this.McpCommand))
                {
                    throw new Exception("McpCommand不能为空");
                }
            }
        }
    }
}
