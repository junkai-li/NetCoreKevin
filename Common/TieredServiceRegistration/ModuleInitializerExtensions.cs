using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Common.TieredServiceRegistration
{
    public static class ModuleInitializerExtensions
    {
        /// <summary>
        /// 模块初始化方法
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public static IServiceCollection RunModuleInitializers(this IServiceCollection services,
         IEnumerable<Assembly> assemblies)
        {
            foreach (var item in assemblies)
            {
                var types = item.GetTypes();
                var moduleInitializerTypes = types.Where(t => !t.IsAbstract && typeof(IModuleInitializer).IsAssignableFrom(t));
                foreach (var moduleInitializerType in moduleInitializerTypes)
                {
                    var initializer = (IModuleInitializer?)Activator.CreateInstance(moduleInitializerType);
                    if (initializer is null)
                    {
                        throw new ApplicationException($"Cannot create ${moduleInitializerType}");
                    }
                    initializer.Initialize(services);
                }
            }

            return services;
        }
    }
}
