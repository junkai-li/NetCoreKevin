using kevin.Cache.Service;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.SqlServer;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;

namespace kevin.Cache
{
    public static class ServiceCollectionExtensions
    {

        public static void AddKevinMemoryCache(this IServiceCollection service)
        {
            //注册缓存服务 内存模式
            service.AddDistributedMemoryCache();
            AddCacheServices(service);
        }

        public static void AddKevinSqlServerCache(this IServiceCollection service, Action<SqlServerCacheOptions> setupAction)
        {
            //注册缓存服务 SqlServer模式
            //service.AddDistributedSqlServerCache(options =>
            //{
            //    options.ConnectionString = Configuration.GetConnectionString("dbConnection");
            //    options.SchemaName = "dbo";
            //    options.TableName = "t_cache";
            //});
            service.AddDistributedSqlServerCache(setupAction);
            AddCacheServices(service);
        }

        public static void AddKevineRedisCache(this IServiceCollection service, Action<RedisCacheOptions> setupAction)
        {
            //注册缓存服务 Redis模式
            //service.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = Configuration.GetConnectionString("redisConnection");
            //    options.InstanceName = "cache";
            //});
            service.AddStackExchangeRedisCache(setupAction);
            AddCacheServices(service);
        }

        internal static void AddCacheServices(IServiceCollection services)
        {
            services.Add(ServiceDescriptor.Singleton<ICacheService, CacheService>());
        }
    }
}
