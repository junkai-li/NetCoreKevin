using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Redis.StackExchange;
using Kevin.Hangfire.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using static System.Collections.Specialized.BitVector32;

namespace Kevin.Hangfire
{
    public static class ServiceCollectionExtensions
    {
        public static bool isAdd = false;

        static object lockObject = new object();

        /// <summary>
        /// redis方式注册Hangfire
        /// </summary>
        /// <param name="services"></param>
        /// <param name="hangfireRedisSetting"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void AddKevinHangfireRedis(this IServiceCollection services, Action<HangfireRedisSetting> action)
        {
            lock (lockObject)
            {
                if (isAdd)
                {
                    throw new ArgumentException("Hangfire已通过其他方式注入");
                }
                services.Configure(action);
                var hangfireRedisSetting = new HangfireRedisSetting();
                action.Invoke(hangfireRedisSetting);
                if (hangfireRedisSetting != default)
                { 
                    if (string.IsNullOrWhiteSpace(hangfireRedisSetting.RedisConnectionString))
                    {
                        throw new ArgumentException("Redis连接字符串不能为空", nameof(hangfireRedisSetting.RedisConnectionString));
                    }
                    //注册 HangFire(Redis)
                    services.AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    // 2. 配置Redis存储
                    .UseRedisStorage(hangfireRedisSetting.RedisConnectionString, new RedisStorageOptions
                    {
                        Db = hangfireRedisSetting.Db, // 指定Redis数据库编号
                        Prefix = hangfireRedisSetting.Prefix // 避免Key冲突的命名空间[reference:5]
                    }));
                    // 注册 HangFire 服务
                    services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(3));
                    isAdd = true;
                }
            }
        }

        /// <summary>
        /// 内存方式注册Hangfire
        /// </summary>
        /// <param name="services"></param>
        public static void AddKevinHangfireMemoryStorage(this IServiceCollection services)
        {
            lock (lockObject)
            {
                if (isAdd)
                {
                    throw new ArgumentException("Hangfire已通过其他方式注入");
                }
                //注册 HangFire(Redis)
                services.AddHangfire(options => options.UseMemoryStorage());

                // 注册 HangFire 服务
                services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(3));
                isAdd = true;
            }
        }
    }
}
