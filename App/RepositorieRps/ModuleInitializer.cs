using kevin.Domain.Share.Interfaces;
using kevin.Ioc;
using kevin.Ioc.TieredServiceRegistration;
using Kevin.Common.App.Global;
using Kevin.Common.Helper;
using Microsoft.Extensions.DependencyInjection;

namespace App.RepositorieRps
{
    internal class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            new IocHelper().BatchAddScopeds<IBaseRepository>(services, t =>
            {
                GlobalServices.AddIRepositry(t); 
            });
            ConsoleHelper.Print("App.RepositorieRps仓储服务注入完成", ConsoleHelper.Style.Gradient); 
        } 
    }
}
