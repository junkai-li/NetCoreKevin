using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using System;
using System.IO;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var nlogPath = Path.Combine(Directory.GetCurrentDirectory(), "Configs\\_\\nlog.config");
            NLog.Logger logger = NLogBuilder.ConfigureNLog(nlogPath).GetCurrentClassLogger();
            try
            {

                Common.EnvironmentHelper.InitTestServer(); 
                logger.Debug("init main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {

                    //启用 Kestrel Https 并绑定证书
                    //webBuilder.UseKestrel(options =>
                    //{
                    //    options.ConfigureHttpsDefaults(options =>
                    //    {
                    //        options.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(Path.Combine(AppContext.BaseDirectory, "xxxx.pfx"), "123456");
                    //    });
                    //});
                    //webBuilder.UseUrls("https://*");


                    webBuilder.UseStartup<Startup>();
                }).UseServiceProviderFactory(new AutofacServiceProviderFactory());
            //替换IOC 使用AutoFac .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
