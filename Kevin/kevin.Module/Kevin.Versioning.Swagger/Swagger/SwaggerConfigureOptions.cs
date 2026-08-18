using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Kevin.Api.Versioning.Swagger
{
    /// <summary>
    /// 配置swagger生成选项。
    /// </summary>
    public class SwaggerConfigureOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider provider;

        // OpenAPI 规范要求 components/schemas 的 key 只能包含字母、数字、.、-、_
        // 泛型 DTO（如 dtoPageData[xxx]）的方括号/逗号会被替换为下划线，否则 Mcpifier 等严格校验的解析器会报错
        private static readonly Regex InvalidSchemaIdChars = new("[^a-zA-Z0-9.\\-_]", RegexOptions.Compiled);

        public SwaggerConfigureOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                Console.WriteLine(description.GroupName);
                var modelPrefix = Assembly.GetEntryAssembly()?.GetName().Name + ".Models.";
                var versionPrefix = description.GroupName + ".";
                options.SchemaGeneratorOptions = new SchemaGeneratorOptions { SchemaIdSelector = type => GetSchemaId(type, modelPrefix, versionPrefix) };

            }
        }

        /// <summary>
        /// 生成符合 OpenAPI 命名规范（^[a-zA-Z0-9.\-_]+$）的 SchemaId
        /// </summary>
        static string GetSchemaId(Type type, string modelPrefix, string versionPrefix)
        {
            var typeName = type.ToString();
            var modelsIndex = typeName.IndexOf("Models.");
            if (modelsIndex >= 0)
            {
                typeName = typeName[(modelsIndex + 7)..];
            }
            typeName = typeName.Replace(modelPrefix, "").Replace(versionPrefix, "").Replace("`1", "").Replace("+", ".");
            // 泛型的 [ ] , 及空格等非法字符统一替换为下划线，保证不同泛型实参的 SchemaId 仍唯一
            return InvalidSchemaIdChars.Replace(typeName, "_");
        }

        static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = Assembly.GetEntryAssembly()?.GetName().Name,
                Version = "v" + description.ApiVersion.ToString(),
                Description = "API文档 v" + description.ApiVersion.ToString(),
                //Contact = new OpenApiContact() { Name = "", Email = "" }
            };

            if (description.IsDeprecated)
            {
                info.Description += "此 Api " + info.Version + " 版本已弃用，请尽快升级新版";
            }

            return info;
        }
    }
}
