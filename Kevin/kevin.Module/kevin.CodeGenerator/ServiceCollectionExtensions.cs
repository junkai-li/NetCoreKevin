using kevin.CodeGenerator.Dto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace kevin.CodeGenerator
{
    public static class ServiceCollectionExtensions
    {
        public static void AddKevinCodeGenerator(this IServiceCollection services, Action<CodeGeneratorSetting> action)
        {
            services.Configure(action);
            services.TryAddScoped<ICodeGeneratorService, CodeGeneratorService>();
        }
    }
}
