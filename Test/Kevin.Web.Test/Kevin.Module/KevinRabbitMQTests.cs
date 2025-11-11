using kevin.RabbitMQ;
using RabbitMQ.Client;

namespace Kevin.Unit.Tests.Kevin.Module
{
    public class KevinRabbitMQTests
    {
        [Fact]
        public void SendMsg()
        {
            bool isPublishOk = false;
            bool isConsumer = false;
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "admin",
                Password = "admin",
                VirtualHost = "/app_webapi_test",
                HandshakeContinuationTimeout = TimeSpan.FromSeconds(30), // 握手超时 
                AutomaticRecoveryEnabled = true, // 自动重连
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10) // 重连间隔
            }; 
            using var rabbitMQConnection = new RabbitMQConnection(factory);
            var serv = new RabbitMQPublisherService(rabbitMQConnection);
            serv.PublishList<string>("test_exchange", "test_routingkey", new List<string> { "hello", "world" },exchangeType: ExchangeType.Direct, queue: "test_queue", isConfirmSelect: true, basicAcks: (sender, ea) =>
            {
                isPublishOk = true;
                Console.WriteLine($"message已发送:{sender} {ea.DeliveryTag}");
            }, basicNacks: (sender, ea) =>
            {
                Console.WriteLine($"message发送失败:{sender} {ea.DeliveryTag}");
            });
            // Assert
            Assert.False(isPublishOk);

            // 创建消费者
            isConsumer = false;
            // 创建消费端服务
            using var rabbitMQConnection2 = new RabbitMQConnection(factory);
            using var consumerService = new RabbitMQConsumerService(rabbitMQConnection2);

            // 定义消息处理函数
            Func<string, bool> messageHandler = (message) =>
            {
                Console.WriteLine($"收到消息: {message}");
                // 处理消息
                // 返回true表示处理成功，false表示处理失败
                isConsumer= true;
                return true;
            }; 
            // 开始消费
            consumerService.StartConsume(
                exchange: "test_exchange",
                routingKey: "test_routingkey",
                queue: "test_queue",
                exchangeType: ExchangeType.Direct,
                autoAck: false,
                prefetchCount: 1,
                messageHandler: messageHandler);  
            Assert.False(isConsumer);
        }
    }
}
