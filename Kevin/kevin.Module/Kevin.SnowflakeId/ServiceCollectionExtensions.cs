using Kevin.SnowflakeId.Models;
using Kevin.SnowflakeId.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Kevin.SnowflakeId
{
    public static class ServiceCollectionExtensions
    {
        public static void AddKevinSnowflakeId(this IServiceCollection services, Action<SnowflakeIdSetting> action)
        {
            services.Configure(action);
            services.AddSingleton<ISnowflakeIdService, SnowflakeIdService>();
        }
    }
}
