

#region ȫ������

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
using Web.Libraries.Swagger;
using Microsoft.Extensions.DependencyInjection;
using kevin.HttpApiClients;
using Kevin.Common.App.Global;
using Kevin.Common.App.Start;

namespace AppVTApi
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
                //builder.Logging.UseKevinLog4Net();��־
                #region Kestrel Https����֤��
                //���� Kestrel Https ����֤��
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

                #region Swagger �ĵ�

                //ע��Swagger������������һ���Ͷ��Swagger �ĵ�
                builder.Services.AddSwaggerGen(options =>
                {
                    options.OperationFilter<SwaggerOperationFilter>();

                    options.MapType<long>(() => new OpenApiSchema { Type = "string", Format = "long" });

                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml"), true);

                    //��������ע���ļ�
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"kevin.Domain.Share.xml"), true);

                    //���� Swagger JWT ��Ȩģ��
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        Description = "���¿�����������ͷ����Ҫ���Jwt��ȨToken��Bearer Token",
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

                #region ��ѶMiniLive
                ////��ѶMiniLive����ע��
                //MiniLive.AppId = "wxf164719d9baf8d83";
                //MiniLive.AppSecret = "****************";//΢��С������Կ 
                //TencentService.Helper.RedisHelper.ConnectionString = builder.Configuration.GetConnectionString("redisConnection");
                //builder.Services.AddSingleton<IMiniLive, MiniLive>();
                #endregion

                #region IDSERVER��Ȩ������
                //IDSERVER ʹ����Ȩ������ ���ڵ����¼
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
                app.MapMcp(); //MCP����ӳ��MCP�˵�
                //��������ģʽ���ж�ζ�ȡHttpContext.Body�е����� 
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
                    ////ע��ȫ���쳣�������
                    app.UseExceptionHandler(builder => builder.Run(async context => await GlobalError.ErrorEvent(context)));
                }

                //kevin��ʼ��
                app.UseKevin();
                //app.UseKevinConsul(builder.Configuration.GetSection("ConsulSetting").Get<ConsulSetting>(), app.Lifetime);//�������� 
                //�����м�������swagger-ui��ָ��Swagger JSON�˵�
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
