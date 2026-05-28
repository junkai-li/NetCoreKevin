using kevin.AI.AgentFramework;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Tools;
using kevin.AI.AgentFramework.Tools;
using Kevin.AI.Dto;
using Microsoft.Extensions.DependencyInjection;
namespace Kevin.AI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAIAgentClient(this IServiceCollection services)
        {
            services.AddTransient<ICommonToolsService, CommonToolsService>();
            services.AddTransient<IPythonToolsService, PythonToolsService>();
            services.AddTransient<IShellToolsService, ShellToolsService>(); 
            services.AddTransient<IAgentHttpClientToolsService, AgentHttpClientToolsService>();
            services.AddTransient<IAIAgentService, AIAgentService>();  
        }
    }
}
