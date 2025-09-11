using kevin.Ioc.Extension;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace kevin.Ioc
{
    /// <summary>
    /// IOC帮助类
    /// </summary>
    public class IocHelper
    {
        private static IEnumerable<Type> GetTypes(Type type)
        {
            Assembly ser = Assembly.GetExecutingAssembly();
            return ser.GetTypes().Where(a => a.IsClass && !a.IsInterface && !a.IsAbstract && type.IsAssignableFrom(a));
        }


        /// <summary>
        /// 批量根据传入T进行AddScoped的生命周期注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="action"></param>
        public void BatchAddScopeds<T>(IServiceCollection services, Action<Type> action,bool isAllAssemblys = true)
        {
            Type typeOf_IService = typeof(T);
            var sers = GetTypes(typeOf_IService);
            foreach (var serviceType in sers)
            {
                var implementedInterfaces = serviceType.GetInterfaces().Where(a => a != typeof(IDisposable) && a != typeOf_IService);
                foreach (Type implementedInterface in implementedInterfaces)
                {
                    services.AddScoped(implementedInterface, sp => sp.GetServiceOrCreateInstance(serviceType)!);
                }
                action.Invoke(serviceType);
                if (!serviceType.IsGenericType)
                {
                    services.AddScoped(serviceType, serviceType);
                }

            }
            if (isAllAssemblys)
            {
                Assembly[] Assemblys = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var item in Assemblys)
                {
                    try
                    {
                        var serviceTypes = item?.GetTypes()?.Where(a => a.IsClass && !a.IsInterface && !a.IsAbstract && typeOf_IService.IsAssignableFrom(a));
                        if (serviceTypes!=default)
                        {
                            foreach (var serviceType in serviceTypes)
                            {

                                var implementedInterfaces = serviceType.GetInterfaces().Where(a => a != typeof(IDisposable) && a != typeOf_IService);
                                foreach (Type implementedInterface in implementedInterfaces)
                                {
                                    services.AddScoped(implementedInterface, sp => sp.GetServiceOrCreateInstance(serviceType)!);
                                }
                                action.Invoke(serviceType);
                                if (!serviceType.IsGenericType)
                                {
                                    services.AddScoped(serviceType, serviceType);
                                }
                            }
                        }
                      
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }

                }
            }
 
        }
        /// <summary>
        /// 批量根据传入T进行AddTransient的生命周期注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="action"></param>
        public void BatchAddTransients<T>(IServiceCollection services, Action<Type> action, bool isAllAssemblys = true)
        {
            Type typeOf_IService = typeof(T);
            var sers = GetTypes(typeOf_IService);
            foreach (var serviceType in sers)
            {

                var implementedInterfaces = serviceType.GetInterfaces().Where(a => a != typeof(IDisposable) && a != typeOf_IService);
                foreach (Type implementedInterface in implementedInterfaces)
                {
                    services.AddTransient(implementedInterface, sp => sp.GetServiceOrCreateInstance(serviceType)!);
                }
                action.Invoke(serviceType);
                if (!serviceType.IsGenericType)
                {
                    services.AddTransient(serviceType, serviceType);
                }

            }
            if (isAllAssemblys)
            {
                Assembly[] Assemblys = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var item in Assemblys)
                {
                    try
                    {
                        var serviceTypes = item?.GetTypes()?.Where(a => a.IsClass && !a.IsInterface && !a.IsAbstract && typeOf_IService.IsAssignableFrom(a));
                        if (serviceTypes!=default)
                        {
                            foreach (var serviceType in serviceTypes)
                            {

                                var implementedInterfaces = serviceType.GetInterfaces().Where(a => a != typeof(IDisposable) && a != typeOf_IService);
                                foreach (Type implementedInterface in implementedInterfaces)
                                {
                                    services.AddTransient(implementedInterface, sp => sp.GetServiceOrCreateInstance(serviceType));
                                }
                                action.Invoke(serviceType);
                                if (!serviceType.IsGenericType)
                                {
                                    services.AddTransient(serviceType, serviceType);
                                }
                            }
                        }
                    
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }

                }
            }
          
        }
        /// <summary>
        /// 批量根据传入T进行AddSingleton的生命周期注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="action"></param>
        public void BatchAddSingletons<T>(IServiceCollection services, Action<Type> action, bool isAllAssemblys = true)
        {
            Type typeOf_IService = typeof(T);
            var sers = GetTypes(typeOf_IService);
            foreach (var serviceType in sers)
            {

                var implementedInterfaces = serviceType.GetInterfaces().Where(a => a != typeof(IDisposable) && a != typeOf_IService);
                foreach (Type implementedInterface in implementedInterfaces)
                {
                    services.AddSingleton(implementedInterface, sp => sp.GetServiceOrCreateInstance(serviceType));
                }
                action.Invoke(serviceType);
                if (!serviceType.IsGenericType)
                {
                    services.AddSingleton(serviceType, serviceType);
                }

            }
            if (isAllAssemblys)
            {
                Assembly[] Assemblys = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var item in Assemblys)
                {
                    try
                    {
                        var serviceTypes = item?.GetTypes()?.Where(a => a.IsClass && !a.IsInterface && !a.IsAbstract && typeOf_IService.IsAssignableFrom(a));
                        if (serviceTypes!=default)
                        {
                            foreach (var serviceType in serviceTypes)
                            {

                                var implementedInterfaces = serviceType.GetInterfaces().Where(a => a != typeof(IDisposable) && a != typeOf_IService);
                                foreach (Type implementedInterface in implementedInterfaces)
                                {
                                    services.AddSingleton(implementedInterface, sp => sp.GetServiceOrCreateInstance(serviceType));
                                }
                                action.Invoke(serviceType);
                                if (!serviceType.IsGenericType)
                                {
                                    services.AddSingleton(serviceType, serviceType);
                                }
                            }
                        }
                      
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }

                }
            }
        }
    }
}
