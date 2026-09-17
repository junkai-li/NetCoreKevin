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
        /// Mcp测试连接成功后返回的全部工具名称（JSON字符串数组）
        /// </summary> 
        [Description("Mcp测试连接返回的全部工具名称 Json数组格式")]
        public String? McpTools { get; set; } = "";

        /// <summary>
        /// Mcp勾选启用的工具名称（JSON数组，AI运行时仅加载这些工具）
        /// </summary> 
        [Description("Mcp勾选启用的工具名称 Json数组格式")]
        public String? McpSelectedTools { get; set; } = "";
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
                //所有Mcp都必须测试一次连接并成功返回工具，未测试成功（无工具列表）不允许保存
                var tools = ParseJsonArray(this.McpTools);
                if (tools.Count == 0)
                {
                    throw new Exception("请先点击“测试连接”并成功返回Mcp工具后再保存");
                }
                //必须至少勾选一个工具，AI运行时只会加载勾选的工具
                var selected = ParseJsonArray(this.McpSelectedTools);
                if (selected.Count == 0)
                {
                    throw new Exception("请至少勾选一个Mcp工具");
                }
                //勾选的工具必须都在测试返回的工具列表内，防止伪造或过期数据
                var invalid = selected.Where(t => !tools.Contains(t)).ToList();
                if (invalid.Count > 0)
                {
                    throw new Exception($"勾选的工具不在测试连接返回的工具列表中，请重新测试连接：{string.Join("、", invalid)}");
                }
            }
        }

        /// <summary>
        /// 解析JSON字符串数组，非法或空时返回空列表
        /// </summary>
        private static List<string> ParseJsonArray(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<string>();
            }
            try
            {
                var list = global::System.Text.Json.JsonSerializer.Deserialize<List<string>>(json);
                return list?.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).ToList() ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
