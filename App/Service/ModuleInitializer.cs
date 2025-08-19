using kevin.Ioc;
using kevin.Ioc.TieredServiceRegistration;
using Kevin.Common.App.Global;
using Microsoft.Extensions.DependencyInjection;
using System;
using Web.Base._;

namespace App.Application
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            new IocHelper().BatchAddScopeds<IBaseService>(services, t =>
            {
                GlobalServices.AddIService(t);
            }); 
            Console.WriteLine("App.Application服务注册完成");
        }
         
    }
}
