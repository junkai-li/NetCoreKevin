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
            // 无 DI 上下文（后台任务）的兜底路径会直接 new 生成器，把同一份配置同步给兜底快照，
            // 避免这些实例退化成默认节点ID、与容器内单例生成出重复 Id
            var setting = new SnowflakeIdSetting();
            action(setting);
            SnowflakeIdService.SetFallbackSetting(setting);
        }
    }
}
