using kevin.AI.AgentFramework.Interfaces.Msg;
using kevin.Application;
using kevin.Application.Services.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Interfaces;
using kevin.Ioc;
using kevin.Ioc.TieredServiceRegistration;
using Kevin.Common.App.Global;
using Kevin.Common.Helper;
using Microsoft.Extensions.DependencyInjection;
using Web.Global.User;

namespace Kevin.Application
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<IAIFileToolService, AIFileToolService>();
            services.AddScoped<IAIMsgService, AIMsgService>();
            // Qdrant 向量记忆服务（可选，未配置 Qdrant/Ollama 时服务内部自动降级到数据库搜索）
            services.AddScoped<IAIQdrantAgentMemoryService, AIQdrantAgentMemoryService>();
            new IocHelper().BatchAddScopeds<IService>(services, t =>
            {
                GlobalServices.AddIService(t);
            });
            services.AddScoped<ICurrentUser, CurrentUser>();
            ConsoleHelper.Print("kevin.Application服务注册完成");
        }
    }
}
