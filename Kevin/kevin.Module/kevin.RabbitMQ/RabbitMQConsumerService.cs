using kevin.RabbitMQ.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.RabbitMQ
{
    public class RabbitMQConsumerService : IRabbitMQConsumer, IDisposable
    {
        private readonly IRabbitMQConnection _connection;
        private readonly IModel _channel;
        private string? _consumerTag;
        private bool _isDisposed;

        public RabbitMQConsumerService(IRabbitMQConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _channel = _connection.CreateModel();
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
            _channel.BasicQos(0, prefetchCount, false);

            // 声明交换机（如果提供了交换机类型）
            if (!string.IsNullOrEmpty(exchangeType))
            {
                _channel.ExchangeDeclare(exchange, exchangeType, true);
            }

            // 声明队列
            _channel.QueueDeclare(queue, true, false, false, null);

            // 绑定队列到交换机
            if (!string.IsNullOrEmpty(exchange))
            {
                _channel.QueueBind(queue, exchange, routingKey);
            }

            // 创建消费者
            var consumer = new EventingBasicConsumer(_channel);

            // 注册接收消息事件
            consumer.Received += (model, ea) =>
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
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    else if (!result && !autoAck)
                    {
                        _channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                }
                catch (Exception ex)
                {
                    // 记录日志
                    Console.WriteLine($"消息处理异常: {ex.Message}");

                    if (!autoAck)
                    {
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                }
            };

            // 开始消费
            _consumerTag = _channel.BasicConsume(queue, autoAck, consumer);
        }

        public void StopConsume()
        {
            if (!string.IsNullOrEmpty(_consumerTag))
            {
                _channel.BasicCancel(_consumerTag);
                _consumerTag = null;
            }
        }

        public void Ack(ulong deliveryTag)
        {
            _channel.BasicAck(deliveryTag, false);
        }

        public void Nack(ulong deliveryTag, bool requeue = false)
        {
            _channel.BasicNack(deliveryTag, false, requeue);
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
                    _channel?.Close();
                    _channel?.Dispose();
                    _connection?.Dispose();
                }
                _isDisposed = true;
            }
        } 
    }
}
