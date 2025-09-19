using kevin.RabbitMQ.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Web.Global.Exceptions;
namespace kevin.RabbitMQ
{
    public class RabbitMQPublisherService : IRabbitMQPublisherService
    {
        private readonly IRabbitMQConnection _connection;

        public RabbitMQPublisherService(IRabbitMQConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
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
        /// <param name="BasicAcks">确认机制成功回调</param>
        /// <param name="BasicNacks">确认机制失败回调</param>
        /// <exception cref="UserFriendlyException">确认机制开启，必须传入BasicAcks和BasicNacks</exception>
        public void PublishList<T>(string exchange, string routingKey,List<T> messages, string queue = "", string exchangeType = "", bool isTx = false, bool isConfirmSelect = false, EventHandler<BasicAckEventArgs>? BasicAcks = default, EventHandler<BasicNackEventArgs>? BasicNacks = default)
        {

            #region 校验
          
            if (isConfirmSelect)
            {
                if (BasicAcks == default || BasicNacks == default)
                {
                    throw new UserFriendlyException("确认机制开启，必须传入BasicAcks和BasicNacks");
                }
            }

            #endregion
            //创建通道
            using var channel = _connection.CreateModel();

            try
            {
                //声明队列
                if (!string.IsNullOrEmpty(queue))
                {
                    channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
                    channel.QueueBind(queue, exchange, routingKey);
                }

                //默认声明交换机
                channel.ExchangeDeclare(exchange, ExchangeType.Direct);

                //声明交换机类型
                if (!string.IsNullOrEmpty(exchangeType))
                {
                    channel.ExchangeDeclare(exchange, exchangeType);
                }
                //声明消息持久化
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.ContentType = "application/json";
         
                //开启确认机制
                if (isConfirmSelect)
                {
                    channel.ConfirmSelect();
                    channel.BasicAcks += BasicAcks;
                    channel.BasicNacks += BasicNacks;
                }
                //开启事务
                if (isTx) { channel.TxSelect(); }
              
                foreach (var message in messages)
                {
                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                    channel.BasicPublish(exchange, routingKey, false, properties, body);
                }  

                //提交事务
                if (isTx) { channel.TxCommit(); }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"消息发布失败:An error occurred: {ex.Message}");
                //回滚事务
                if (isTx) { channel.TxRollback(); }
                throw; 
            }
        
        }
 
    }
}
