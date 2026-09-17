using System.ComponentModel;

namespace kevin.Domain.Share.Dtos.AI
{
    /// <summary>
    /// Mcp测试连接请求参数（仅连接相关字段，不含名称等必填业务字段）
    /// </summary>
    public class McpConnectionTestDto
    {
        /// <summary>
        /// Mcp地址
        /// </summary> 
        [Description("Mcp地址")]
        public String? McpUrl { get; set; } = "";

        /// <summary>
        /// McpType https,sse,stdio
        /// </summary> 
        [Description("McpType McpType https,sse,stdio")]
        public String? McpType { get; set; } = "";

        /// <summary>
        /// McpHeaders 键值对Json格式
        /// </summary> 
        [Description("McpHeaders 键值对Json格式")]
        public String? McpHeaders { get; set; } = "";

        /// <summary>
        /// McpCommand
        /// </summary> 
        [Description("McpCommand")]
        public String? McpCommand { get; set; } = "";

        /// <summary>
        /// McpArguments ,分隔
        /// </summary> 
        [Description("McpArguments ,分隔")]
        public String? McpArguments { get; set; } = "";

        /// <summary>
        /// McpEnvironment 键值对Json格式
        /// </summary> 
        [Description("McpEnvironment 键值对Json格式")]
        public String? McpEnvironment { get; set; } = "";
    }
}
