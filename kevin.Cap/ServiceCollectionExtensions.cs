
using DotNetCore.CAP;
using kevin.Cap.Filter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace kevin.Cap
{
    public static class ServiceCollectionExtensions
    {

        public static void AddKevinCap(this IServiceCollection services, string redisConnection)
        {
            services.AddSingleton<DemoSubscribe>();
            services.AddCap(options =>
            {

                //使用 Redis 传输消息
                options.UseRedis(redisConnection); 
                //var rabbitMQSetting = Configuration.GetSection("RabbitMQSetting").Get<RabbitMQSetting>();

                ////使用 RabbitMQ 传输消息
                //options.UseRabbitMQ(options =>
                //{
                //    options.HostName = rabbitMQSetting.HostName;
                //    options.UserName = rabbitMQSetting.UserName;
                //    options.Password = rabbitMQSetting.PassWord;
                //    options.VirtualHost = rabbitMQSetting.VirtualHost;
                //    options.Port = rabbitMQSetting.Port;
                //    options.ConnectionFactoryOptions = options =>
                //    {
                //        options.Ssl = new RabbitMQ.Client.SslOption { Enabled = rabbitMQSetting.Ssl.Enabled, ServerName = rabbitMQSetting.Ssl.ServerName };
                //    };
                //});


                //使用 ef 搭配 db 存储执行情况
                //options.UseEntityFramework<Repository.Database.dbContext>();
                //使用Dashboard，这是一个Cap的可视化管理界面；默认地址:http://localhost:端口/cap
                options.UseDashboard();
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

                options.DefaultGroupName = "default";   //默认组名称
                options.GroupNamePrefix = null; //全局组名称前缀
                options.TopicNamePrefix = null; //Topic 统一前缀
                options.Version = "v1";
                options.FailedRetryInterval = 60;   //失败时重试间隔
                options.ConsumerThreadCount = 1;    //消费者线程并行处理消息的线程数，当这个值大于1时，将不能保证消息执行的顺序
                options.FailedRetryCount = 10;  //失败时重试的最大次数
                options.FailedThresholdCallback = null; //重试阈值的失败回调
                options.SucceedMessageExpiredAfter = 24 * 3600; //成功消息的过期时间（秒）
            }).AddSubscribeFilter<CapSubscribeFilter>();
        }
        public static void AddKevinRedisCap(this IServiceCollection services, string redisConnection,string dbConnection)
        {
            services.AddSingleton<DemoSubscribe>();
            services.AddCap(options =>
            { 
                //使用 Redis 传输消息
                options.UseRedis(redisConnection); 
                //使用Dashboard，这是一个Cap的可视化管理界面；默认地址:http://localhost:端口/cap
                options.UseDashboard();
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                //使用 Mysql 存储执行情况
                options.UseMySql(dbConnection);
               // options.UseInMemoryStorage()
                options.DefaultGroupName = "default";   //默认组名称
                options.GroupNamePrefix = null; //全局组名称前缀
                options.TopicNamePrefix = null; //Topic 统一前缀
                options.Version = "v1";
                options.FailedRetryInterval = 60;   //失败时重试间隔
                options.ConsumerThreadCount = 1;    //消费者线程并行处理消息的线程数，当这个值大于1时，将不能保证消息执行的顺序
                options.FailedRetryCount = 10;  //失败时重试的最大次数
                options.FailedThresholdCallback = null; //重试阈值的失败回调
                options.SucceedMessageExpiredAfter = 24 * 3600; //成功消息的过期时间（秒）
            }).AddSubscribeFilter<CapSubscribeFilter>();
        }
    }
}
