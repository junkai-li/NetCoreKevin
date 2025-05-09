using Google.Protobuf.WellKnownTypes;
using Kevin_MCP_Server.Tools;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Kevin_MCP_Server
{
    public static class ServiceCollectionExtensions
    {
        public static void AddKevinMCPServer(this IServiceCollection services)
        {
            //AddMcpServer()：添加MCP服务器服务。
            //WithHttpTransport()：指定使用HTTP传输。
            //WithTools<EchoTool>()和WithTools<SampleLlmTool>()：添加特定的工具（EchoTool和SampleLlmTool）到MCP服务器中。
            services.AddMcpServer()
            .WithHttpTransport()// 使用HTTP输入输出作为传输方式
                     .WithStdioServerTransport()// 使用标准输入输出作为传输方式
                     .WithTools<MyTool>();


            //AddOpenTelemetry()：添加OpenTelemetry服务。
            //WithTracing()：配置跟踪，包括所有源（AddSource("*")），ASP.NET Core和HTTP客户端的自动仪器化。
            //WithMetrics()：配置度量，包括所有计量器（AddMeter("*")），ASP.NET Core和HTTP客户端的自动仪器化。
            //WithLogging()：启用日志记录。
            //UseOtlpExporter()：使用OTLP（OpenTelemetry Protocol）导出器将遥测数据发送
            services.AddOpenTelemetry()
                        .WithTracing(b => b.AddSource("*")
                            .AddAspNetCoreInstrumentation()
                            .AddHttpClientInstrumentation())
                        .WithMetrics(b => b.AddMeter("*")
                            .AddAspNetCoreInstrumentation()
                            .AddHttpClientInstrumentation())
                        .WithLogging()
                        .UseOtlpExporter();
        }
    }
}
