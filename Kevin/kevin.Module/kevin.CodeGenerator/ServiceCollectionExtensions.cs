using kevin.CodeGenerator.Dto;
using Microsoft.Extensions.DependencyInjection;

namespace kevin.CodeGenerator
{
    public static class ServiceCollectionExtensions
    { 
        public static void AddKevinCodeGenerator(this IServiceCollection services, Action<CodeGeneratorSetting> action)
        { 
            services.Configure(action);
            services.AddSingleton<ICodeGeneratorService, CodeGeneratorService>(); 
        }
    }
}
