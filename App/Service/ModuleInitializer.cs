using kevin.Domain.Share.Interfaces;
using kevin.Ioc;
using kevin.Ioc.TieredServiceRegistration;
using Kevin.Common.App.Global;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace App.Application
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            new IocHelper().BatchAddScopeds<IService>(services, t =>
            {
                GlobalServices.AddIService(t);
            }); 
            Console.WriteLine("App.Application服务注册完成");
        }
         
    }
}
