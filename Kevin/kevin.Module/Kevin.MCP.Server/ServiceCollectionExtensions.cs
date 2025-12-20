using Kevin.AI.MCP.Server.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Kevin.AI.MCP.Server
{
    public static class ServiceCollectionExtensions
    {
        public static void AddKevinMCPServer(this IServiceCollection services, Action<MCPSseClientSetting>? action = default)
        {
            //AddMcpServer()：添加MCP服务器服务。
            //WithHttpTransport()：指定使用HTTP传输。
            //WithTools<EchoTool>()和WithTools<SampleLlmTool>()：添加特定的工具（EchoTool和SampleLlmTool）到MCP服务器中。
            services.AddMcpServer()
            .WithHttpTransport()// 使用HTTP输入输出作为传输方式
                     .WithStdioServerTransport()// 使用标准输入输出作为传输方式
                     .WithToolsFromAssembly(); // 从程序集中扫描添加 tools
                                               //.WithTools<MyTool>();

            //注入客户端AddTransient()：将IMySSEToolClient接口的实现类MySSEToolClient注册为瞬时（Transient）生命周期服务。
            if (action != default)
            {
                services.Configure(action);
            } 
           // services.AddTransient<IMySseToolClient, MySseToolClient>();
            //AddOpenTelemetry()：添加OpenTelemetry服务。
            //WithTracing()：配置跟踪，包括所有源（AddSource("*")），ASP.NET Core和HTTP客户端的自动仪器化。
            //WithMetrics()：配置度量，包括所有计量器（AddMeter("*")），ASP.NET Core和HTTP客户端的自动仪器化。
            //WithLogging()：启用日志记录。
            //UseOtlpExporter()：使用OTLP（OpenTelemetry Protocol）导出器将遥测数据发送
            //services.AddOpenTelemetry()
            //            .WithTracing(b => b.AddSource("*")
            //                .AddAspNetCoreInstrumentation()
            //                .AddHttpClientInstrumentation())
            //            .WithMetrics(b => b.AddMeter("*")
            //                .AddAspNetCoreInstrumentation()
            //                .AddHttpClientInstrumentation())
            //            .WithLogging()
            //            .UseOtlpExporter();
        }
    }
}
