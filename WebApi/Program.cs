

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
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;
using Web.Extension;
using Web.Global;
using Web.Libraries.Swagger;
using Microsoft.Extensions.DependencyInjection;
using kevin.HttpApiClients;
using Kevin.Common.Kevin.Start;

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
                StartWebHostEnvironment.webHostEnvironment = builder.Environment;
                //builder.Logging.UseKevinLog4Net();日志
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

                builder.Services.ConfigServies(builder.Configuration);

                #region Swagger 文档

                //注册Swagger生成器，定义一个和多个Swagger 文档
                builder.Services.AddSwaggerGen(options =>
                {
                    options.OperationFilter<SwaggerOperationFilter>();

                    options.MapType<long>(() => new OpenApiSchema { Type = "string", Format = "long" });

                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml"), true);

                    //其他类库的注释文件
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"kevin.Share.xml"), true);

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
                ////腾讯MiniLive服务注册
                //MiniLive.AppId = "wxf164719d9baf8d83";
                //MiniLive.AppSecret = "****************";//微信小程序密钥 
                //TencentService.Helper.RedisHelper.ConnectionString = builder.Configuration.GetConnectionString("redisConnection");
                //builder.Services.AddSingleton<IMiniLive, MiniLive>();
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

                builder.Services.AddKevinHttpApiClients();
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
                //app.UseKevinConsul(builder.Configuration.GetSection("ConsulSetting").Get<ConsulSetting>(), app.Lifetime);//服务网关 
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
                app.Run();
              
            }
            catch (Exception ex)
            {  
                throw;
            } 
        }
    }
}
