using Asp.Versioning;
using Kevin.Api.Versioning.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;
namespace Kevin.Api.Versioning
{
    public static class ServiceCollectionExtensions
    { 
        /// <summary>
        /// 添加版本控制
        /// </summary>
        /// <param name="services"></param>
        /// <param name="xmls">xml文件名称列表</param>
        public static void AddKevinApiVersioning(this IServiceCollection services,List<string> xmls)
        {
            services.AddEndpointsApiExplorer();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>(); 
            //注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerOperationFilter>();

                options.MapType<long>(() => new OpenApiSchema { Type = "string", Format = "long" });
                //其他类库的注释文件
                foreach (var item in xmls)
                {
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, item), true);
                }   
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
                // 使用解决冲突的策略
                options.ResolveConflictingActions(apiDescriptions =>
                {
                    return apiDescriptions.First();
                });
            });
           
            var apiVersioningBuilder = services.AddApiVersioning(options =>
            {
                //通过Header向客户端通报支持的版本
                options.ReportApiVersions = true;

                //允许不加版本标记直接调用接口
                options.AssumeDefaultVersionWhenUnspecified = true;

                //接口默认版本
                //options.DefaultApiVersion = new ApiVersion(1, 0);

                //如果未加版本标记默认以当前最高版本进行处理
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
                // 结合多种版本控制方式
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("version"),
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-API-Version"),
                    new MediaTypeApiVersionReader("version")
                    );   
            });
            apiVersioningBuilder.AddApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });
            //注册统一模型验证
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {

                    //获取验证失败的模型字段 
                    var errors = actionContext.ModelState.Where(e => e.Value?.Errors.Count > 0).Select(e => e.Value?.Errors.First().ErrorMessage).ToList();

                    var dataStr = string.Join(" | ", errors);

                    //设置返回内容
                    var result = new
                    {
                        errMsg = dataStr
                    };

                    return new BadRequestObjectResult(result);
                };
            }); 

        }
    }
}
