﻿using Medallion.Threading.SqlServer;
using Medallion.Threading;
using Microsoft.Extensions.DependencyInjection;
using Medallion.Threading.MySql;

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

        public static void AddKevinDistributedLockMySql(this IServiceCollection services, string dbConnection)
        {
            //分布式
            services.AddSingleton<IDistributedLockProvider>(new MySqlDistributedSynchronizationProvider(dbConnection)); 
             
            ////信号锁
            //services.AddSingleton<IDistributedSemaphoreProvider>(new MySqlDistributedSemaphoreProvider(dbConnection));
            ////读写锁
            //services.AddSingleton<IDistributedUpgradeableReaderWriterLockProvider>(new MySqlDistributedUpgradeableReaderWriterLockProvider(dbConnection));
        }
    }
}
