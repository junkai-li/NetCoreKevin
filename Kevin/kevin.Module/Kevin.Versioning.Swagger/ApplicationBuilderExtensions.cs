using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Api.Versioning
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseKevinUseSwagger(this IApplicationBuilder app)
        { 
            //启用中间件服务生成Swagger作为JSON端点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON端点
            app.UseSwaggerUI(options =>
            {
                var apiVersionDescriptionProvider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
                if (apiVersionDescriptionProvider!=default)
                {
                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }

                } 
                options.RoutePrefix = "swagger";
            });
        }
    }
}
