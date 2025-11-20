using kevin.Permission.Permission.Action;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace kevin.Permission
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 开启权限认证
        /// </summary>
        /// <param name="services"></param>
        public static void AddKevinAuthorizationPermission(this IServiceCollection services, IConfiguration Configuration)
        {
            //判断配置文件是否开启权限认证
            if (Configuration["IsOpenPermission"].ToBoolean())
            {  
                //权限校验
                services.AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireAssertion(context => IdentityVerification.Authorization(context)).Build();
                });
            } 
        }
    }
}
