using Kevin.Cors.Models;
using Microsoft.Extensions.DependencyInjection;
namespace Kevin.Cors
{
    public static class ServiceCollectionExtensions
    {

        public static void AddKevinCors(this IServiceCollection services, CorsSetting WithOrigins)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            } 
            var orurls = WithOrigins.IPs.Split(",").Where(t => !string.IsNullOrEmpty(t)).Select(t => t).ToList(); 
            services.AddCors(
                options => options.AddDefaultPolicy(builder =>
                {
                    builder.SetIsOriginAllowed(t => true);
                    builder.WithOrigins(orurls.ToArray()).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                })); 
        }
    }
}
