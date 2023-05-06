using kevin.Domain.Share.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Web.Actions;

namespace kevin.Permission
{
    public static class ServiceCollectionExtensions
    {

        public static void AddKevinPermission(this IServiceCollection services)
        { 
            AddPermissionServices(services);
        }
        internal static void AddPermissionServices(IServiceCollection services)
        {
            services.AddScoped<IKevinPermissionService, KevinPermissionService>();
        }
    }
}
