using kevin.RabbitMQ.Interfaces;
using Kevin.log4Net;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace kevin.RabbitMQ
{
    public class RabbitMQConsumerService : IRabbitMQConsumer, IDisposable
    {
        private readonly IRabbitMQConnection _connection;
        private readonly IChannel _channel;
        private string? _consumerTag;
        private bool _isDisposed;

        public RabbitMQConsumerService(IRabbitMQConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _channel = _connection.CreateChannel();
        }
        /// <summary>
        /// 开始消费消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="exchange">交换机名称</param>
        /// <param name="routingKey">路由键</param>
        /// <param name="queue">队列名称</param>
        /// <param name="exchangeType">交换机类型</param>
        /// <param name="autoAck">是否自动确认</param>
        /// <param name="prefetchCount">预获取数量</param>
        /// <param name="messageHandler">消息处理函数</param>
        public void StartConsume<T>(string exchange, string routingKey, string queue, Func<T, bool> messageHandler, string exchangeType = "", bool autoAck = false, ushort prefetchCount = 1)
        {
            // 设置QoS
            _channel.BasicQosAsync(0, prefetchCount, false).GetAwaiter().GetResult();

            // 声明交换机（如果提供了交换机类型）
            if (!string.IsNullOrEmpty(exchangeType))
            {
                _channel.ExchangeDeclareAsync(exchange, exchangeType, true).GetAwaiter().GetResult();
            }

            // 声明队列
            _channel.QueueDeclareAsync(queue, true, false, false, null).GetAwaiter().GetResult();

            // 绑定队列到交换机
            if (!string.IsNullOrEmpty(exchange))
            {
                _channel.QueueBindAsync(queue, exchange, routingKey).GetAwaiter().GetResult();
            }

            // 创建消费者
            var consumer = new AsyncEventingBasicConsumer(_channel);

            // 注册接收消息事件
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(body)) ?? throw new ArgumentNullException(nameof(T));
                    bool result = true;
                    if (messageHandler != null)
                    {
                        result = messageHandler(data);
                    }

                    if (result && !autoAck)
                    {
                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    else if (!result && !autoAck)
                    {
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper<RabbitMQConsumerService>.logger.Error($"消息处理异常: {ex.Message}");
                    if (!autoAck)
                    {
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                    }
                }
            };

            // 开始消费
            _consumerTag = _channel.BasicConsumeAsync(queue, autoAck, string.Empty, false, false, null, consumer).GetAwaiter().GetResult();
        }

        public void StopConsume()
        {
            if (!string.IsNullOrEmpty(_consumerTag))
            {
                _channel.BasicCancelAsync(_consumerTag).GetAwaiter().GetResult();
                _consumerTag = null;
            }
        }

        public void Ack(ulong deliveryTag)
        {
            _channel.BasicAckAsync(deliveryTag, false).GetAwaiter().GetResult();
        }

        public void Nack(ulong deliveryTag, bool requeue = false)
        {
            _channel.BasicNackAsync(deliveryTag, false, requeue).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    StopConsume();
                    _channel?.CloseAsync().GetAwaiter().GetResult();
                    _channel?.Dispose();
                    _connection?.Dispose();
                }
                _isDisposed = true;
            }
        }
    }
}
