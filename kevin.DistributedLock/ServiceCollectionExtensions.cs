using Medallion.Threading.SqlServer;
using Medallion.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace kevin.DistributedLock
{
    public static class ServiceCollectionExtensions
    {

        public static void AddKevinDistributedLockSqlServer(this IServiceCollection services,string dbConnection)
        {
            //分布式
            services.AddSingleton<IDistributedLockProvider>(new SqlDistributedSynchronizationProvider(dbConnection));
            //信号锁
            services.AddSingleton<IDistributedSemaphoreProvider>(new SqlDistributedSynchronizationProvider(dbConnection));
            //读写锁
            services.AddSingleton<IDistributedUpgradeableReaderWriterLockProvider>(new SqlDistributedSynchronizationProvider(dbConnection));
        }  
    }
}
