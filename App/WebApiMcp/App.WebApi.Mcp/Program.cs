
using Summerdawn.Mcpifier.DependencyInjection;

namespace App.WebApi.Mcp
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 从 appsettings.json 读取 HttpInterToMcpService 配置
            var setting = builder.Configuration.GetRequiredSection("HttpInterToMcpService").Get<HttpInterToMcpSetting>()
                ?? throw new InvalidOperationException("缺少 HttpInterToMcpService 配置节点");

            // Add services to the container.
            //builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            builder.Services
                       .AddMcpifier(options =>
                       {
                           // 设置目标 REST API 的基础地址
                           options.Rest.BaseAddress = setting.BaseAddress; 
                           // 设置需要转发的请求头，如 Authorization
                           options.Rest.ForwardedHeaders = setting.ForwardedHeaders.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                                                             .ToDictionary(h => h, _ => true);
                           // 设置 MCP 服务器信息
                           options.ServerInfo = new() { Name = setting.ServerName };
                       })
                       .AddAspNetCore()
                       .AddToolsFromSwagger(setting.SwaggerJsonUrl); // 传入 Swagger 文档地址或本地文件路径

            var app = builder.Build();
            // [诊断用] 打印到达 /mcp 的请求 Authorization 头，确认客户端是否真的带了头
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/" + setting.Mcpifier))
                {
                    var auth = context.Request.Headers.Authorization.ToString();
                    Console.WriteLine($"[诊断] {context.Request.Method} {context.Request.Path} | Authorization: " +
                        (string.IsNullOrEmpty(auth) ? "<未携带>" : auth[..Math.Min(auth.Length, 40)] + "..."));
                }
                await next();
            });
            // Configure the HTTP request pipeline.
            app.MapMcpifier("/" + setting.Mcpifier);   // MCP 服务器挂在 /配置名称 路径下

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.Run();
        }
    }
}
