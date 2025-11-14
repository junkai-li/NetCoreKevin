using kevin.Ioc.TieredServiceRegistration;
using Kevin.Common.Helper;
using Microsoft.Extensions.DependencyInjection;

namespace App.Domain
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            ConsoleHelper.Print("App.Domain领域服务注入完成");
        }
    }
}
