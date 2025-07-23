using kevin.Cache;
using kevin.DistributedLock;
using kevin.Domain.EventBus;
using kevin.Ioc;
using kevin.Permission;
using Kevin.AI.MCP.Server;
using Kevin.AI.MCP.Server.Models;
using Kevin.Api.Versioning;
using Kevin.Common.App.Global;
using Kevin.Common.App.Start;
using Kevin.Common.TieredServiceRegistration;
using Kevin.Cors;
using Kevin.Web.Filters.TransactionScope;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.Database;
using System;
using Web.Filters;
using Web.Global.User;
namespace Web.Extension
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection ConfigServies(this IServiceCollection services, IConfiguration Configuration)
        {
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
            //services.AddAuthorization(options =>
            //{
            //    options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireAssertion(context => IdentityVerification.Authorization(context)).Build();
            //});
            //services.AddKevinPermission();
            #endregion

            #region 注册常用 

            //注册全局过滤器
            services.AddMvc(config =>
            {
                config.Filters.Add(new ResultFilter());
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
            services.AddKevinPermission();
            //services.AddKevinCors(Configuration.GetSection("CorsSetting").Get<CorsSetting>());
            //services.AddKevinSignalR(Configuration);
            services.RunModuleInitializers(ReflectionScheduler.GetAllReferencedAssemblies());//初始化
            services.AddKevinMediatRDomainEventBus(ReflectionScheduler.GetAllReferencedAssemblies());//初始化

            #region 注册短信服务

            //services.AddTencentCloudSMS(options =>
            //{
            //    var settings = Configuration.GetRequiredSection("TencentCloudSMS").Get<Kevin.SMS.TencentCloud.Models.SMSSetting>()!;
            //    options.AppId = settings.AppId;
            //    options.SecretId = settings.SecretId;
            //    options.SecretKey = settings.SecretKey;
            //});


            //services.AddAliCloudSMS(options =>
            //{
            //    var settings = Configuration.GetRequiredSection("AliCloudSMS").Get<Kevin.SMS.AliCloud.Models.SMSSetting>()!;
            //    options.AccessKeyId = settings.AccessKeyId;
            //    options.AccessKeySecret = settings.AccessKeySecret;
            //});

            #endregion

            #region 注册文件服务


            //services.AddTencentCloudStorage(options =>
            //{
            //    var settings = Configuration.GetRequiredSection("TencentCloudFileStorage").Get<kevin.FileStorage.TencentCloud.Models.FileStorageSetting>()!;
            //    options.AppId = settings.AppId;
            //    options.Region = settings.Region;
            //    options.SecretId = settings.SecretId;
            //    options.SecretKey = settings.SecretKey;
            //    options.BucketName = settings.BucketName;
            //});


            //services.AddAliCloudStorage(options =>
            //{
            //    var settings =Configuration.GetRequiredSection("AliCloudFileStorage").Get<kevin.FileStorage.AliCloud.Models.FileStorageSetting>()!;
            //    options.Endpoint = settings.Endpoint;
            //    options.AccessKeyId = settings.AccessKeyId;
            //    options.AccessKeySecret = settings.AccessKeySecret;
            //    options.BucketName = settings.BucketName;
            //});

            #endregion
            //services.AddKevinMCPServer(options =>
            //{
            //    var settings = Configuration.GetRequiredSection("MCPSseClient").Get<MCPSseClientSetting>()!;
            //    options.Name = settings.Name;
            //    options.Url = settings.Url;
            //    options.UseStreamableHttp = false;
            //    options.AdditionalHeaders = default;
            //    options.ConnectionTimeout = default;
            //}); //mcp服务注册
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
            //app.UseKevinSignalR(StartConfiguration.configuration.GetSection("SignalrSetting").Get<SignalrSetting>());

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
            #region 基本服务
            //为各数据库注入连接字符串
            Repository.Database.dbContext.ConnectionString = Configuration.GetConnectionString("dbConnection");
            services.AddDbContextPool<Repository.Database.dbContext>(options => { }, 100);
            services.AddScoped<dbContext, dbContext>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            //注入事务对象
            services.AddScoped<TransactionScopeFilter>();
            #endregion

        }
    }
}
