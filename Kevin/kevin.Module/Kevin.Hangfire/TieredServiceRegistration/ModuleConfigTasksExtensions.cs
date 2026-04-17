using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Hangfire.TieredServiceRegistration
{
    public static class ModuleConfigTasksExtensions
    {
        /// <summary>
        /// 任务模块初始化方法
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public static void RunModuleConfigTasks(this IApplicationBuilder app, IRecurringJobManager recurringJobManager,
         IEnumerable<Assembly> assemblies)
        {
            foreach (var item in assemblies)
            {
                var types = item.GetTypes();
                var moduleInitializerTypes = types.Where(t => !t.IsAbstract && typeof(IModuleConfigTasks).IsAssignableFrom(t));
                foreach (var moduleInitializerType in moduleInitializerTypes)
                {
                    var initializer = (IModuleConfigTasks?)Activator.CreateInstance(moduleInitializerType);
                    if (initializer is null)
                    {
                        throw new ApplicationException($"Cannot create ${moduleInitializerType}");
                    }
                    initializer.ConfigTasks(recurringJobManager);
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("NetCoreKevin:Kevin.Hangfire自动任务模块注入完成");
        }
    }
}
