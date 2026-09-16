

#region 全局引用

global using System;
#endregion

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Hosting;
using Web.Extension;
using Microsoft.Extensions.DependencyInjection;
using kevin.HttpApiClients;
using Kevin.Common.App.Global;
using Kevin.Common.App.IO;
using Kevin.log4Net;
using System.Threading;
namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                //设置环境变量-----如果需要手动切换环境 只需要修改这里即可 
                Kevin.Common.Helper.EnvironmentConfigHelper.SetEnvironment(Kevin.Common.Helper.EnvironmentConfigHelper.GetEnvironment());
                //线程池最小线程数默认等于逻辑核心数，而线程池每秒只补充约 2 个线程：
                //容器里只有 8 核时，一旦有同步等 Redis 的调用占住工作线程，积压会让后续同步调用整体超时
                //（StackExchange.Redis 自检里的 WORKER Busy 远大于 Min 就是这个信号）。
                //这里先把下限抬起来做兜底，热路径上的 sync-over-async 仍应持续改为异步。
                ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);
                if (minWorkerThreads < 200)
                {
                    ThreadPool.SetMinThreads(200, Math.Max(minCompletionPortThreads, 200));
                }
                var builder = WebApplication.CreateBuilder(args);
                builder.Logging.UseKevinLog4Net();//日志
                #region Kestrel Https并绑定证书
                //启用 Kestrel Https 并绑定证书
                //builder.WebHost.UseKestrel(options =>
                //{
                //    options.ConfigureHttpsDefaults(options =>
                //    {
                //        options.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(Path.Combine(AppContext.BaseDirectory, "xxxx.pfx"), "123456");
                //    });
                //});
                //builder.WebHost.UseUrls("https://*");
                #endregion

                //builder.Services.AddKevinRedisCap(builder.Configuration.GetConnectionString("redisConnection"), builder.Configuration.GetConnectionString("dbConnection")); cap
                Path._hostingEnvironment = builder.Environment;
                builder.Services.ConfigureServices(builder.Configuration);


                builder.Services.AddKevinHttpApiClients();
                builder.Services.AddControllers(options =>
                {
                    options.OutputFormatters.RemoveType<StringOutputFormatter>();
                });
                var app = builder.Build();



                //app.MapMcp(); //MCP服务映射MCP端点
                //开启倒带模式运行多次读取HttpContext.Body中的内容 
                app.Use(async (context, next) =>
                {
                    GlobalServices.Set(context.Request.HttpContext.RequestServices);
                    context.Request.EnableBuffering();
                    await next.Invoke();
                });
                //app.Urls.Add("http://*:9901"); // 监听所有网络接口的9901端口
                if (app.Environment.IsDevelopment())
                {

                    app.UseDeveloperExceptionPage();
                    ////注册全局异常处理机制
                    // app.UseExceptionHandler(builder => builder.Run(async context => await GlobalError.ErrorEvent(context)));
                }
                else
                {
                    ////注册全局异常处理机制
                    app.UseExceptionHandler(builder => builder.Run(async context => await GlobalError.ErrorEvent(context)));
                }

                //堆的硬限制设置为2G字节（AI应用需要更多内存，从1G调整为2G）
                AppContext.SetData("GCHeapHardLimit", (ulong)2048 * 1_024 * 1_024);

                //kevin初始化
                app.UseKevin(builder.Configuration);
                //app.UseKevinConsul(builder.Configuration.GetSection("ConsulSetting").Get<ConsulSetting>(), app.Lifetime);//服务网关  
                app.Run();
            }
            catch (Exception ex)
            {
                Kevin.log4Net.LogHelper<Program>.logger.Error(ex.Message, ex);
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
