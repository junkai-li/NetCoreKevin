using Kevin.AI.Dto;
using Microsoft.Extensions.DependencyInjection;
namespace Kevin.AI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAIClient(this IServiceCollection services, Action<AISetting> action)
        {
            services.Configure(action);  
            services.AddTransient<IAIClient, AIClient>();
        }
    }
}
