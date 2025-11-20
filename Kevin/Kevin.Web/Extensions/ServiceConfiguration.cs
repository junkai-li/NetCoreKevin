using kevin.Cache;
using kevin.DistributedLock;
using kevin.Domain.EventBus;
using kevin.FileStorage;
using kevin.Ioc;
using kevin.Ioc.TieredServiceRegistration;
using kevin.Permission;
using kevin.RabbitMQ;
using Kevin.Api.Versioning;
using Kevin.Common.App.Global;
using Kevin.Common.App.Start;
using Kevin.Common.Helper;
using Kevin.Cors;
using Kevin.Cors.Models;
using Kevin.Email;
using Kevin.SignalR;
using Kevin.SignalR.Models;
using Kevin.SMS;
using Kevin.Web.Filters;
using Kevin.Web.Filters.TransactionScope;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.Database;
using System;
using System.Linq;
using Web.Filters;
namespace Web.Extension
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection ConfigServies(this IServiceCollection services, IConfiguration Configuration)
        { 
            ConsoleHelper.PrintFrameworkName("欢迎使用NetCoreKevin框架");
            ConsoleHelper.PrintWithTypewriterEffect("正在初始化......");
            #region json配置
            //json动态响应压缩https://docs.microsoft.com/zh-cn/aspnet/core/performance/response-compression?view=aspnetcore-5.0
            services.AddResponseCompression();
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;
            });
            //请求配置
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
                options.Limits.MaxRequestBodySize = int.MaxValue;//请求流大小
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddHsts(options =>
            {
                options.MaxAge = TimeSpan.FromDays(365);
            });
            #endregion

            #region 权限校验 
            //权限校验
            services.AddKevinAuthorizationPermission(Configuration); 
            #endregion

            #region 注册常用 

            //注册全局过滤器
            services.AddMvc(config =>
            {
                config.Filters.Add(new ResultFilter());
                //添加过滤器
                config.Filters.Add(typeof(HttpLogFilter));

                config.Filters.Add(typeof(TransactionScopeFilter));
            });
            //注册配置文件信息
            StartConfiguration.Add(Configuration);

            //注册HttpContext
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //注册雪花ID算法示例
            services.AddSingleton(new Common.SnowflakeHelper(0, 0));
            #endregion

            services.AddControllers();

            services.AddControllers().AddJsonOptions(option =>
            {
                option.JsonSerializerOptions.Converters.Add(new Common.Json.DateTimeConverter());
                option.JsonSerializerOptions.Converters.Add(new Common.Json.DateTimeNullConverter());
                option.JsonSerializerOptions.Converters.Add(new Common.Json.LongConverter());
            });

            services.AddControllers().AddControllersAsServices(); //控制器当做实例创建 
            services.AddKevinApiVersioning(new System.Collections.Generic.List<string> {
           "kevin.Domain.Share.xml","App.WebApi.xml"
            }); //api版本

            #region 缓存服务模式
            //注册缓存服务 内存模式
            services.AddKevinMemoryCache();

            #endregion

            #region 分布式锁服务注册 
            services.AddKevinDistributedLockMySql(Configuration.GetConnectionString("dbConnection"));
            #endregion

            //把控制器作为服务注册，然后使用它内置的ioc来替换原来的控制器的创建器
            services.AddControllersWithViews().AddControllersAsServices();
            services.ReplaceIocControllerActivator();
            //App服务注册
            RegisterAppServices(services, Configuration);
            services.AddKevinCors(Configuration.GetSection("CorsSetting").Get<CorsSetting>()); 

            #region 注入SignalRRedis
            services.AddKevinSignalRRedis(options =>
            {
                var newoptions = Configuration.GetRequiredSection("SignalrRdisSetting").Get<SignalrRdisSetting>();
                options.url = newoptions.url;
                options.password = newoptions.password;
                options.port = newoptions.port;
                options.defaultDatabase = newoptions.defaultDatabase;
                options.configurationChannel = newoptions.configurationChannel;
                options.hostname = newoptions.hostname;
                options.cacheMySignalRKeyName = newoptions.cacheMySignalRKeyName;
            });
            #endregion

            services.AddKevinMediatRDomainEventBus(ReflectionScheduler.GetAllReferencedAssemblies());//初始化

            #region 注册短信服务  

            services.AddAliCloudSMS(options =>
            {
                var settings = Configuration.GetRequiredSection("AliCloudSMS").Get<Kevin.SMS.AliCloud.Models.SMSSetting>()!;
                options.AccessKeyId = settings.AccessKeyId;
                options.AccessKeySecret = settings.AccessKeySecret;
            }); 

            #endregion

            #region 注册文件服务 
            services.AddAliCloudStorage(options =>
            {
                var settings = Configuration.GetRequiredSection("AliCloudFileStorage").Get<kevin.FileStorage.AliCloud.Models.FileStorageSetting>()!;
                options.Endpoint = settings.Endpoint;
                options.AccessKeyId = settings.AccessKeyId;
                options.AccessKeySecret = settings.AccessKeySecret;
                options.BucketName = settings.BucketName;
            }); 
            #endregion

            #region MCP服务注册
            //services.AddKevinMCPServer(options =>
            //{
            //    var settings = Configuration.GetRequiredSection("MCPSseClient").Get<MCPSseClientSetting>()!;
            //    options.Name = settings.Name;
            //    options.Url = settings.Url;
            //    options.UseStreamableHttp = false;
            //    options.AdditionalHeaders = default;
            //    options.ConnectionTimeout = default;
            //});  
            #endregion

            #region MQ服务注入

            services.AddKevinRabbitMQ(options =>
            {
                var settings = Configuration.GetRequiredSection("RabbitMQ").Get<RabbitMQOptions>()!;
                options.HostName = settings.HostName;
                options.Port = settings.Port;
                options.UserName = settings.UserName;
                options.Password = settings.Password;
                options.VirtualHost = settings.VirtualHost;
            });

            #endregion


            #region 邮件服务

            services.AddEmailService(options =>
            {
                var settings = Configuration.GetRequiredSection("EmailSetting").Get<EmailSetting>()!;
                options.SmtpServer = settings.SmtpServer;
                options.AccountName = settings.AccountName;
                options.AccountPassword = settings.AccountPassword;
                options.Port = settings.Port;
            });

            #endregion

            return services;
        }

        /// <summary>
        /// 使用UseKevin
        /// UseKevin
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseKevin(this IApplicationBuilder app)
        {
            /////json压缩
            app.UseResponseCompression();
            app.UseHsts();
            //注册跨域信息
            app.UseKevinCors();
            //强制重定向到Https
            app.UseHttpsRedirection();

            //静态文件中间件 (UseStaticFiles) 返回静态文件，并简化进一步请求处理。
            //app.UseStaticFiles();

            app.UseKevinUseSwagger();

            app.UseRouting();

            //注册用户认证机制,必须放在 UseCors UseRouting 之后
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseKevinRedisSignalR(options =>
            {
                var newoptions = StartConfiguration.configuration.GetRequiredSection("SignalrRdisSetting").Get<SignalrRdisSetting>();
                options.url = newoptions.url;
                options.password = newoptions.password;
                options.port = newoptions.port;
                options.defaultDatabase = newoptions.defaultDatabase;
                options.configurationChannel = newoptions.configurationChannel;
                options.hostname = newoptions.hostname;
                options.cacheMySignalRKeyName = newoptions.cacheMySignalRKeyName;
            });

            GlobalServices.Set(app.ApplicationServices);
            return app;
        }

        /// <summary>
        /// App服务注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static void RegisterAppServices(this IServiceCollection services, IConfiguration Configuration)
        {
            //AddSingleton→AddTransient→AddScoped 
            //AddSingleton的生命周期：

            //项目启动 - 项目关闭   相当于静态类 只会有一个

            //AddScoped的生命周期：

            //请求开始 - 请求结束  在这次请求中获取的对象都是同一个

            //  AddTransient的生命周期：

            //请求获取 -（GC回收 - 主动释放） 每一次获取的对象都不是同一个
            //为各数据库注入连接字符串
            Repository.Database.KevinDbContext.ConnectionString = Configuration.GetConnectionString("dbConnection");
            Repository.Database.KevinDbContext.DBDefaultHasIndexFields = Configuration.GetRequiredSection("DBDefaultHasIndexFields").Get<string>().Split(",").ToList();
            services.AddDbContextPool<Repository.Database.KevinDbContext>(options => { }, 100);
            services.AddScoped<KevinDbContext, KevinDbContext>(); 
        } 
   
    }
}
