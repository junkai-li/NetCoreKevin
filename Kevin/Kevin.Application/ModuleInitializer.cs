using kevin.Application;
using kevin.Ioc;
using kevin.Ioc.TieredServiceRegistration;
using kevin.Permission.Interfaces;
using Kevin.Common.App.Global;
using Microsoft.Extensions.DependencyInjection;
using Web.Base._;
using Web.Global.User;

namespace Kevin.Application
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            new IocHelper().BatchAddScopeds<IBaseService>(services, t =>
            {
                GlobalServices.AddIService(t);
            },false);
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IKevinPermissionService, KevinPermissionService>();
            Console.WriteLine("kevin.Application服务注册完成");
        }
    }
}
