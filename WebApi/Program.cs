

#region 全局引用

global using System;
global using Web.Global.Exceptions;
#endregion

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using TencentService;
using TencentService._;
using Web.Extension;
using Web.Filters;
using Web.Global;
using Web.Libraries.Swagger;
using Web.Subscribes;
using log4net;
using DnsClient;
using Microsoft.Extensions.Logging;
using Kevin.Common.Helper;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        { 
            try
            {

                Common.EnvironmentHelper.InitTestServer();
                var builder = WebApplication.CreateBuilder(args); 
                builder.Logging.AddLog4Net("Configs/_/log4.config"); 
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

                #region Cap

                builder.Services.AddSingleton<DemoSubscribe>();
                builder.Services.AddCap(options =>
                {

                    //使用 Redis 传输消息
                    options.UseRedis(builder.Configuration.GetConnectionString("redisConnection"));

                    //var rabbitMQSetting = Configuration.GetSection("RabbitMQSetting").Get<RabbitMQSetting>();

                    ////使用 RabbitMQ 传输消息
                    //options.UseRabbitMQ(options =>
                    //{
                    //    options.HostName = rabbitMQSetting.HostName;
                    //    options.UserName = rabbitMQSetting.UserName;
                    //    options.Password = rabbitMQSetting.PassWord;
                    //    options.VirtualHost = rabbitMQSetting.VirtualHost;
                    //    options.Port = rabbitMQSetting.Port;
                    //    options.ConnectionFactoryOptions = options =>
                    //    {
                    //        options.Ssl = new RabbitMQ.Client.SslOption { Enabled = rabbitMQSetting.Ssl.Enabled, ServerName = rabbitMQSetting.Ssl.ServerName };
                    //    };
                    //});


                    //使用 ef 搭配 db 存储执行情况
                    options.UseEntityFramework<Repository.Database.dbContext>();
                    //使用Dashboard，这是一个Cap的可视化管理界面；默认地址:http://localhost:端口/cap
                    options.UseDashboard();
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

                    options.DefaultGroupName = "default";   //默认组名称
                    options.GroupNamePrefix = null; //全局组名称前缀
                    options.TopicNamePrefix = null; //Topic 统一前缀
                    options.Version = "v1";
                    options.FailedRetryInterval = 60;   //失败时重试间隔
                    options.ConsumerThreadCount = 1;    //消费者线程并行处理消息的线程数，当这个值大于1时，将不能保证消息执行的顺序
                    options.FailedRetryCount = 10;  //失败时重试的最大次数
                    options.FailedThresholdCallback = null; //重试阈值的失败回调
                    options.SucceedMessageExpiredAfter = 24 * 3600; //成功消息的过期时间（秒）
                }).AddSubscribeFilter<CapSubscribeFilter>();
                #endregion  

                builder.Services.ConfigServies(builder.Configuration);

                #region Swagger 文档

                //注册Swagger生成器，定义一个和多个Swagger 文档
                builder.Services.AddSwaggerGen(options =>
                {
                    options.OperationFilter<SwaggerOperationFilter>();

                    options.MapType<long>(() => new OpenApiSchema { Type = "string", Format = "long" });

                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml"), true);

                    //其他类库的注释文件
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"Models.xml"), true);

                    //开启 Swagger JWT 鉴权模块
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        BearerFormat = "JWT",
                        Scheme = "Bearer"
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                        new string[] { }
                    }
                });
                });
                #endregion

                #region 腾讯MiniLive
                //腾讯MiniLive服务注册
                MiniLive.AppId = "wxf164719d9baf8d83";
                MiniLive.AppSecret = "****************";//微信小程序密钥 
                TencentService.Helper.RedisHelper.ConnectionString = builder.Configuration.GetConnectionString("redisConnection");
                builder.Services.AddSingleton<IMiniLive, MiniLive>(); 
                #endregion

                #region IDSERVER授权服务器
                //IDSERVER 使用授权服务器 用于单点登录
                builder.Services.AddAuthentication(o =>
                {
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer("Bearer", o =>
                {
                    o.Audience = builder.Configuration["JwtOptions:Audience"];
                    o.Authority = builder.Configuration["JwtOptions:Authority"];
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters.RequireExpirationTime = true;
                    o.TokenValidationParameters.ValidateAudience = false;
                });
                #endregion


                #region HttpClients
                builder.Services.AddHttpClient("", options =>
                {
                    options.DefaultRequestVersion = new Version("2.0");
                    options.DefaultRequestHeaders.Add("Accept", "*/*");
                    options.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36");
                    options.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
                }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AllowAutoRedirect = false
                });


                builder.Services.AddHttpClient("SkipSsl", options =>
                {
                    options.DefaultRequestVersion = new Version("2.0");
                    options.DefaultRequestHeaders.Add("Accept", "*/*");
                    options.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36");
                    options.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
                }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                });
                #endregion


                builder.Services.AddControllers(options =>
                {
                    options.OutputFormatters.RemoveType<StringOutputFormatter>();
                }); 

                var app = builder.Build();
                //开启倒带模式运行多次读取HttpContext.Body中的内容 
                app.Use(async (context, next) =>
                {
                    GlobalServices.Set(context.Request.HttpContext.RequestServices);
                    context.Request.EnableBuffering();
                    await next.Invoke();
                });
               
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage(); 
                }
                else
                {
                    ////注册全局异常处理机制
                    app.UseExceptionHandler(builder => builder.Run(async context => await Web.Actions.GlobalError.ErrorEvent(context)));
                }
                
                //kevin初始化
                app.UseKevin();

                //注册HostingEnvironment
                Web.Libraries.Start.StartHostingEnvironment.Add(app.Environment);
                //启用中间件服务对swagger-ui，指定Swagger JSON端点
                app.UseSwaggerUI(options =>
                {
                    var apiVersionDescriptionProvider = app.Services.GetService<IApiVersionDescriptionProvider>();
                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }

                    options.RoutePrefix = "swagger";
                });

                LogHelper.logger.Debug("init main");
                app.Run();
            }
            catch (Exception ex)
            {
                LogHelper.logger?.Error("Stopped program because of exception",ex);
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    
            }
        } 
    }
}
