using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Nodes;

namespace Kevin.Api.Versioning.Swagger
{

    /// <summary>
    /// swagger 集成多版本api自定义设置
    /// </summary>
    public class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            //判断接口遗弃状态，对接口进行标记调整
            operation.Deprecated |= apiDescription.IsDeprecated;

            if (operation.Parameters == null)
            {
                return;
            }

            //为 api-version 参数添加必填验证
            foreach (var parameter in operation.Parameters)
            {
                // IOpenApiParameter 只暴露只读属性，写回需要具体实现类型
                if (parameter is not OpenApiParameter openApiParameter)
                {
                    continue;
                }

                var description = apiDescription.ParameterDescriptions.FirstOrDefault(p => p.Name == parameter.Name);
                if (description == null)
                {
                    continue;
                }

                if (parameter.Description == null)
                {
                    openApiParameter.Description = description.ModelMetadata?.Description;
                }

                if (parameter.Schema is OpenApiSchema schema && schema.Default == null && description.DefaultValue != null)
                {
                    schema.Default = JsonValue.Create(description.DefaultValue.ToString());
                }

                openApiParameter.Required |= description.IsRequired;
            }
        }
    }
}
