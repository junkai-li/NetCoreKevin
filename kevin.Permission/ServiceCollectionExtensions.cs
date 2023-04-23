using kevin.Permission.Permission.Action;
using kevin.Permission.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Web.Actions;

namespace kevin.Permission
{
    public static class ServiceCollectionExtensions
    {

        public static void AddKevinPermission(this IServiceCollection services)
        {
            //权限校验
            //services.AddAuthorization(options =>
            //{
            //    options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireAssertion(context => IdentityVerification.Authorization(context)).Build();
            //});
            AddPermissionServices(services);
        }
        internal static void AddPermissionServices(IServiceCollection services)
        {
            services.AddScoped<IKevinPermissionService, KevinPermissionService>();
        }
    }
}
