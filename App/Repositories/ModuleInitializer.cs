using App.Domain.Interfaces.Repositorie.v1;
using App.RepositorieRps.Repositories.v1;
using Kevin.Common.TieredServiceRegistration;
using Microsoft.Extensions.DependencyInjection;
using Service.Repositories.v1;

namespace App.RepositorieRps
{
    internal class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<IFileRp, FileRp>();
            services.AddScoped<ILogRp, LogRp>();
            services.AddScoped<IPermissionRp, PermissionRp>();
            services.AddScoped<IRolePermissionRp, RolePermissionRp>();
            services.AddScoped<IRoleRp, RoleRp>();
            services.AddScoped<ISignRp, SignRp>();
            services.AddScoped<IUserBindWeixinRp, UserBindWeixinRp>();
            services.AddScoped<IUserRp, UserRp>();
            services.AddScoped<IWeiXinKeyRp, WeiXinKeyRp>();
            services.AddScoped<ITenantRp, TenantRp>();
            Console.WriteLine("Repositories-ModuleInitializer");
        } 
    }
}
