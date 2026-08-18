namespace App.WebApi.Mcp
{
    /// <summary>
    /// http接口转发Mcp 独立网关服务配置（对应 appsettings.json 中的 HttpInterToMcpService 节点）
    /// </summary>
    public class HttpInterToMcpSetting
    {

        /// <summary>
        /// MCP 服务端名称
        /// </summary>
        public string ServerName { get; set; } = "NetCoreKevinMcpServer";

        /// <summary>
        /// 需要转发到目标 REST API 的请求头（逗号分隔），如 Authorization,token
        /// </summary>
        public string ForwardedHeaders { get; set; } = "Authorization";

        /// <summary>
        /// 目标 REST API 的基础地址（转发接口地址）
        /// </summary>
        public string BaseAddress { get; set; } = "";

        /// <summary>
        /// MCP 服务挂载路径名称（实际挂载到 /名称 下）
        /// </summary>
        public string Mcpifier { get; set; } = "NetCoreKevinMcp";

        /// <summary>
        /// Swagger 文档地址或本地文件路径（用于自动生成 MCP 工具）
        /// </summary>
        public string SwaggerJsonUrl { get; set; } = "";
    }
}
