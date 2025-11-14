using kevin.Domain.Share.Interfaces;
using kevin.Ioc;
using kevin.Ioc.TieredServiceRegistration;
using Kevin.Common.App.Global;
using Kevin.Common.Helper;
using Microsoft.Extensions.DependencyInjection;

namespace kevin.RepositorieRps
{
    internal class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            new IocHelper().BatchAddScopeds<IBaseRepository>(services, t =>
            {
                GlobalServices.AddIRepositry(t); 
            },false);
            ConsoleHelper.Print("kevin.RepositorieRps仓储服务注入完成");
        } 
    }
}
