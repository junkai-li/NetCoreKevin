using App.RepositorieRps.Repositories.v1;
using kevin.Domain.Interface;
using Kevin.Common.App.Global;
using Kevin.Common.TieredServiceRegistration;
using Microsoft.Extensions.DependencyInjection;
using Service.Repositories.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Web.Base._;
using Web.Global.User;

namespace Repositories
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
            Console.WriteLine("Repositories-ModuleInitializer");
        } 
    }
}
