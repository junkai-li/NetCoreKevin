using Microsoft.Extensions.DependencyInjection;

namespace kevin.CodeGenerator
{
    public static class ServiceCollectionExtensions
    { 
        public static void AddCodeGenerator(this IServiceCollection services)
        { 
            services.AddSingleton<ICodeGeneratorService, CodeGeneratorService>();
        }
    }
}
