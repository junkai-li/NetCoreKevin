using kevin.Domain.Share.Interfaces;
using kevin.Ioc;
using Kevin.Common.App.Global;
using Kevin.Common.TieredServiceRegistration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
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
            Console.WriteLine("AppBaseService服务注册完成");
        }
         
    }
}
