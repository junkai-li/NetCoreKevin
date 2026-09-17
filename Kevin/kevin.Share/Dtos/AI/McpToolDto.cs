using System.ComponentModel;

namespace kevin.Domain.Share.Dtos.AI
{
    /// <summary>
    /// Mcp测试连接返回的单个工具信息
    /// </summary>
    public class McpToolDto
    {
        /// <summary>
        /// 工具名称
        /// </summary>
        [Description("工具名称")]
        public string Name { get; set; } = "";

        /// <summary>
        /// 工具描述
        /// </summary>
        [Description("工具描述")]
        public string? Description { get; set; }
    }
}
