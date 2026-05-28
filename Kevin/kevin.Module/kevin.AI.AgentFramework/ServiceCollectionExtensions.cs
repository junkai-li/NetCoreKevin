using kevin.AI.AgentFramework;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tools;
using kevin.AI.AgentFramework.Tools;
using Kevin.AI.Dto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
namespace Kevin.AI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAIAgentClient(this IServiceCollection services)
        {
            services.TryAddScoped<ICommonToolsService, CommonToolsService>();
            services.TryAddScoped<IPythonToolsService, PythonToolsService>();
            services.TryAddScoped<IShellToolsService, ShellToolsService>(); 
            services.TryAddScoped<IAgentHttpClientToolsService, AgentHttpClientToolsService>();
            services.TryAddScoped<IAIAgentService, AIAgentService>();  
        }
    }
}
