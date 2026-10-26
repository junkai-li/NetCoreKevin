using kevin.RabbitMQ.Interfaces;
using Kevin.log4Net;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Web.Global.Exceptions;
namespace kevin.RabbitMQ
{
    public class RabbitMQPublisherService : IRabbitMQPublisherService, IDisposable
    {
        private readonly IRabbitMQConnection _connection;
        private readonly IChannel _channel;
        private bool _isDisposed;
        public RabbitMQPublisherService(IRabbitMQConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            //创建开启发布确认的通道
            _channel = _connection.CreateChannel(publisherConfirmationsEnabled: true);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exchange">交换机</param>
        /// <param name="routingKey">路由</param>
        /// <param name="messages">消息</param>
        /// <param name="queue">队列</param>
        /// <param name="exchangeType">交换机类型模式 ExchangeType</param>
        /// <param name="isTx">是否开启事务</param>
        /// <param name="isConfirmSelect">是否确认机制</param>
        /// <param name="basicAcks">确认机制成功回调</param>
        /// <param name="basicNacks">确认机制失败回调</param>
        /// <param name="isTtl">是否开启TTL机制</param>
        /// <param name="ttl">TTL时间</param>
        /// <param name="dlxExchange">死信交换机</param>
        /// <param name="dlxRoutingKey">死信路由键</param>
        /// <exception cref="UserFriendlyException">确认机制开启，必须传入BasicAcks和BasicNacks</exception>
        /// <exception cref="UserFriendlyException">开启TTL机制消息过期必须传入死信交换机和死信路由</exception>
        public void PublishList<T>(string exchange, string routingKey, List<T> messages,
                                string queue = "", string exchangeType = "", bool isTx = false, bool isConfirmSelect = false,
                                AsyncEventHandler<BasicAckEventArgs>? basicAcks = default, AsyncEventHandler<BasicNackEventArgs>? basicNacks = default,
                                bool isTtl = false, int ttl = 30000, string? dlxExchange = "", string? dlxRoutingKey = ""
            ) where T : class
        {
            var args = new Dictionary<string, object?>();

            #region 校验

            if (isConfirmSelect)
            {
                if (basicAcks == default || basicNacks == default)
                {
                    throw new UserFriendlyException("确认机制开启，必须传入BasicAcks和BasicNacks");
                }
            }
            if (isTtl)
            {
                if (string.IsNullOrEmpty(dlxExchange) || string.IsNullOrEmpty(dlxRoutingKey))
                {
                    throw new UserFriendlyException("开启TTL机制消息过期必须传入死信交换机和死信路由");
                }
                args = new Dictionary<string, object?>
                {
                    { "x-dead-letter-exchange", dlxExchange }, //死信交换机});
                    { "x-dead-letter-routing-key", dlxRoutingKey } //死信路由键
                };
            }

            #endregion


            try
            {
                //声明队列
                if (!string.IsNullOrEmpty(queue))
                {
                    _channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false, arguments: args).GetAwaiter().GetResult();
                    _channel.QueueBindAsync(queue, exchange, routingKey).GetAwaiter().GetResult();
                }

                //默认声明交换机
                _channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct).GetAwaiter().GetResult();

                //声明交换机类型
                if (!string.IsNullOrEmpty(exchangeType))
                {
                    _channel.ExchangeDeclareAsync(exchange, exchangeType).GetAwaiter().GetResult();
                }
                //声明消息持久化
                var properties = new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json"
                };
                if (isTtl)
                {
                    properties.Expiration = ttl.ToString();
                }
                //开启确认机制
                if (isConfirmSelect)
                {
                    _channel.BasicAcksAsync += basicAcks!;
                    _channel.BasicNacksAsync += basicNacks!;
                }
                //开启事务
                if (isTx) { _channel.TxSelectAsync().GetAwaiter().GetResult(); }

                foreach (var message in messages)
                {
                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                    _channel.BasicPublishAsync(exchange, routingKey, false, properties, body).GetAwaiter().GetResult();
                }

                //提交事务
                if (isTx) { _channel.TxCommitAsync().GetAwaiter().GetResult(); }

            }
            catch (Exception ex)
            {
                LogHelper<RabbitMQPublisherService>.logger.Error($"消息发布失败:An error occurred: {ex.Message}");
                //回滚事务
                if (isTx) { _channel.TxRollbackAsync().GetAwaiter().GetResult(); }
                throw;
            }

        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exchange">交换机</param>
        /// <param name="routingKey">路由</param>
        /// <param name="messages">消息</param>
        /// <param name="queue">队列</param>
        /// <param name="exchangeType">交换机类型模式 ExchangeType</param>
        /// <param name="isTx">是否开启事务</param>
        /// <param name="isConfirmSelect">是否确认机制</param>
        /// <param name="basicAcks">确认机制成功回调</param>
        /// <param name="basicNacks">确认机制失败回调</param>
        /// <param name="isTtl">是否开启TTL机制</param>
        /// <param name="ttl">TTL时间</param>
        /// <param name="dlxExchange">死信交换机</param>
        /// <param name="dlxRoutingKey">死信路由键</param>
        /// <exception cref="UserFriendlyException">确认机制开启，必须传入BasicAcks和BasicNacks</exception>
        /// <exception cref="UserFriendlyException">开启TTL机制消息过期必须传入死信交换机和死信路由</exception>
        public Task PublishListAsync<T>(string exchange, string routingKey, List<T> messages, string queue = "", string exchangeType = "", bool isTx = false, bool isConfirmSelect = false, AsyncEventHandler<BasicAckEventArgs>? basicAcks = null, AsyncEventHandler<BasicNackEventArgs>? basicNacks = null, bool isTtl = false, int ttl = 30000, string? dlxExchange = "", string? dlxRoutingKey = "") where T : class
        {
            return Task.Run(() => PublishList(exchange, routingKey, messages, queue, exchangeType, isTx, isConfirmSelect, basicAcks, basicNacks, isTtl, ttl, dlxExchange, dlxRoutingKey));
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
                    _channel?.CloseAsync().GetAwaiter().GetResult();
                    _channel?.Dispose();
                    _connection?.Dispose();
                }
                _isDisposed = true;
            }
        }
    }
}
