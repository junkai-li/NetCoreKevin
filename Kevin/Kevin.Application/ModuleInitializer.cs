using kevin.Application;
using kevin.Domain.Share;
using kevin.Domain.Share.Interfaces;
using kevin.Ioc;
using kevin.Ioc.TieredServiceRegistration;
using kevin.Permission.Interfaces;
using Kevin.Common.App.Global;
using Kevin.Common.Helper;
using Microsoft.Extensions.DependencyInjection;
using Web.Global.User;

namespace Kevin.Application
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            new IocHelper().BatchAddScopeds<IService>(services, t =>
            {
                GlobalServices.AddIService(t);
            });
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IKevinPermissionService, KevinPermissionService>();
            ConsoleHelper.Print("kevin.Application服务注册完成");
        }
    }
}
