using Kevin.Common.TieredServiceRegistration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Web.Base._;
using Web.Global;

namespace Service
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            #region App业务服务
            Type typeOf_IService = typeof(IBaseService);
            Assembly ser = Assembly.GetExecutingAssembly();
            var sers = ser.GetTypes().Where(a => a.IsClass && !a.IsInterface && !a.IsAbstract && typeOf_IService.IsAssignableFrom(a));

            foreach (var serviceType in sers)
            {

                var implementedInterfaces = serviceType.GetInterfaces().Where(a => a != typeof(IDisposable) && a != typeOf_IService);
                foreach (Type implementedInterface in implementedInterfaces)
                {
                    services.AddScoped(implementedInterface, sp => sp.GetServiceOrCreateInstance(serviceType));
                }
                GlobalServices.AddIService(serviceType);
                if (!serviceType.IsGenericType)
                {
                    services.AddScoped(serviceType, serviceType);
                }
            }
            //Assembly[] Assemblys = AppDomain.CurrentDomain.GetAssemblies();
            //foreach (var item in Assemblys)
            //{
            //    var serviceTypes = item.GetTypes().Where(a => a.IsClass && !a.IsInterface && !a.IsAbstract && typeOf_IService.IsAssignableFrom(a));
            //    foreach (var serviceType in serviceTypes)
            //    {

            //        var implementedInterfaces = serviceType.GetInterfaces().Where(a => a != typeof(IDisposable) && a != typeOf_IService);
            //        foreach (Type implementedInterface in implementedInterfaces)
            //        {
            //            services.AddScoped(implementedInterface, sp => sp.GetServiceOrCreateInstance(serviceType));
            //        }
            //        GlobalServices.AddIService(serviceType);
            //        if (!serviceType.IsGenericType)
            //        {
            //            services.AddScoped(serviceType, serviceType);
            //        }
            //    }
            //}
            #endregion 

            Console.WriteLine("App服务注册完成");
        }
    }
}
