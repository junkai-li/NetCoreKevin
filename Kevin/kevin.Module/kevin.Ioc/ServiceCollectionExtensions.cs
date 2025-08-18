using kevin.Ioc.TieredServiceRegistration;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace kevin.Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static void ReplaceIocControllerActivator(this IServiceCollection services)
        {
            //前提需要把控制器作为服务注册，然后使用它内置的ioc来替换原来的控制器的创建器 
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, IocServiceBaseControllerActivator>());
            services.RunModuleInitializers(ReflectionScheduler.GetAllReferencedAssemblies());//初始化模块
        }
    }
}
