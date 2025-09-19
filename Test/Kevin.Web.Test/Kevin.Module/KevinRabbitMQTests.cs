using kevin.RabbitMQ;
using RabbitMQ.Client;

namespace Kevin.Unit.Tests.Kevin.Module
{
    public class KevinRabbitMQTests
    {
        [Fact]
        public void SendMsg()
        {
            bool isOk = false;
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
            serv.PublishList<string>("test_exchange", "test_queue", new List<string> { "hello", "world" }, queue: "test_queue", isConfirmSelect: true, BasicAcks: (sender, ea) =>
            {
                isOk = true;
                Console.WriteLine($"message已发送:{sender} {ea.DeliveryTag}");
            }, BasicNacks: (sender, ea) =>
            {
                Console.WriteLine($"message发送失败:{sender} {ea.DeliveryTag}");
            });
            // Assert
            Assert.False(isOk);
        }
    }
}
