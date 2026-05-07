using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace kevin.Domain.EventBus
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 把rootAssembly及直接或者间接引用的程序集（排除系统程序集）中的MediatR 相关类进行注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static void AddKevinMediatRDomainEventBus(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(assemblies.ToArray());
            });
        }
    }
}
