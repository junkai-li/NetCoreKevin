using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiService.Global;
using WebApiService.Libraries.Swagger;

namespace WebApiService.Expand
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection ConfigServies(this IServiceCollection service)
        { 

            service.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
            //注册雪花ID算法示例
            service.AddSingleton(new Common.SnowflakeHelper(0, 0));
            GlobalServices.ServiceProvider=service.BuildServiceProvider();
            return service;
        }
    }
}
