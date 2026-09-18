using kevin.AI.AgentFramework;
using kevin.AI.AgentFramework.ImageGeneration;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Safety;
using kevin.AI.AgentFramework.Interfaces.Tools;
using kevin.AI.AgentFramework.Modality;
using kevin.AI.AgentFramework.Safety;
using kevin.AI.AgentFramework.ScriptRunners;
using kevin.AI.AgentFramework.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
namespace Kevin.AI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAIAgentClient(this IServiceCollection services)
        {
            services.TryAddScoped<IAIShareInfoService, AIShareInfoService>();
            services.TryAddScoped<ICommonToolsService, CommonToolsService>();
            services.TryAddScoped<IPythonToolsService, PythonToolsService>();
            services.TryAddScoped<IShellToolsService, ShellToolsService>();
            services.TryAddScoped<IAgentHttpClientToolsService, AgentHttpClientToolsService>();
            services.TryAddScoped<IAIAgentService, AIAgentService>();
            services.TryAddScoped<IAuthorizedToolsService, AuthorizedToolsService>();
            services.TryAddScoped<IWebSearchEngine, WebSearchEngine>();
            services.TryAddScoped<IPySubprocessScriptRunner, PySubprocessScriptRunner>();
            services.TryAddScoped<ISkillSafetyService, SkillSafetyService>();
            services.TryAddScoped<IAIInputOutputSafetyService, AIInputOutputSafetyService>();

            // 多模态内容构造器：把远程文件 URL 转成 AIContent 或纯文本片段
            // 单例注册：无 Scoped 依赖，只读静态配置，避免每次请求都构造
            services.TryAddSingleton<IModalityContentBuilder, ModalityContentBuilder>();

            // 文生图：客户端负责 HTTP 调用，工厂负责按智能体配置封装为 AIFunction
            services.TryAddScoped<IImageGenerationClient, OpenAICompatImageGenerationClient>();
            services.TryAddScoped<IImageGenToolFactory, ImageGenToolFactory>();
        }
    }
}
