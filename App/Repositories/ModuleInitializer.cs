using kevin.Domain.Share.Interfaces;
using kevin.Ioc;
using Kevin.Common.App.Global;
using Kevin.Common.TieredServiceRegistration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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
            Console.WriteLine("仓储服务注入完成");
        } 
    }
}
