using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
namespace kevin.DistributedLock
{
    public static class ServiceCollectionExtensions
    {

        //public static void AddKevinDistributedLockSqlServer(this IServiceCollection services, string dbConnection)
        //{
        //    //分布式
        //    services.AddSingleton<IDistributedLockProvider>(new SqlDistributedSynchronizationProvider(dbConnection));
        //    //信号锁
        //    services.AddSingleton<IDistributedSemaphoreProvider>(new SqlDistributedSynchronizationProvider(dbConnection));
        //    //读写锁
        //    services.AddSingleton<IDistributedUpgradeableReaderWriterLockProvider>(new SqlDistributedSynchronizationProvider(dbConnection));
        //}

        //public static void AddKevinDistributedLockMySql(this IServiceCollection services, string dbConnection)
        //{
        //    //分布式
        //    services.AddSingleton<IDistributedLockProvider>(new MySqlDistributedSynchronizationProvider(dbConnection));

        //           ////信号锁
        //    //services.AddSingleton<IDistributedSemaphoreProvider>(new MySqlDistributedSemaphoreProvider(dbConnection));
        //    ////读写锁
        //    //services.AddSingleton<IDistributedUpgradeableReaderWriterLockProvider>(new MySqlDistributedUpgradeableReaderWriterLockProvider(dbConnection));
        //}

        public static void AddKevinDistributedLockRedis(this IServiceCollection services, string redisConnection)
        {
            try
            {
                var redisDatabase = ConnectionMultiplexer.Connect(redisConnection).GetDatabase();
                // 配置分布式锁选项：Expiry=2小时，防止任务异常时锁永久占用
                Action<RedisDistributedSynchronizationOptionsBuilder> lockOptions = options => options.Expiry(TimeSpan.FromHours(2));
                //分布式
                services.AddSingleton<IDistributedLockProvider>(new RedisDistributedSynchronizationProvider(redisDatabase, lockOptions));
                //信号锁
                services.AddSingleton<IDistributedSemaphoreProvider>(new RedisDistributedSynchronizationProvider(redisDatabase, lockOptions));
                //读写锁
                services.AddSingleton<IDistributedReaderWriterLockProvider>(new RedisDistributedSynchronizationProvider(redisDatabase, lockOptions));
            }
            catch (Exception ex)
            {
                Console.WriteLine("分布式锁注入失败请检查Redis连接：" + ex.Message);
            }

        }
    }
}
