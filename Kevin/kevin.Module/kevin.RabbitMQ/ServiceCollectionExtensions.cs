using kevin.RabbitMQ;
using kevin.RabbitMQ.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace kevin.RabbitMQ
{
    public static class ServiceCollectionExtensions
    {
        public static void AddKevinRabbitMQ(this IServiceCollection services, Action<RabbitMQOptions> action)
        {
            services.Configure(action);
            // 注册RabbitMQ连接工厂
            services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
                var factory = new ConnectionFactory()
                {
                    HostName = options.HostName,
                    Port = options.Port,
                    UserName = options.UserName,
                    Password = options.Password,
                    VirtualHost = options.VirtualHost,
                    HandshakeContinuationTimeout = TimeSpan.FromSeconds(30), // 握手超时
                    DispatchConsumersAsync = true, // 启用异步消费者
                    AutomaticRecoveryEnabled = true, // 自动重连
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10) // 重连间隔
                };
                return new RabbitMQConnection(factory);
            });
            //MQ生产者
            services.AddSingleton<IRabbitMQPublisherService, RabbitMQPublisherService>();
            //MQ消费者
           // services.AddSingleton<IRabbitMQConsumerService, RabbitMQConsumerService>();

        }
    }
}
