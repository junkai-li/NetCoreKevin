using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.RabbitMQ.Interfaces
{
    public interface IRabbitMQConsumer
    {
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
        void StartConsume<T>(
            string exchange,
            string routingKey,
            string queue,
            Func<T, bool> messageHandler,
            string exchangeType = "",
            bool autoAck = false,
            ushort prefetchCount = 1);

        /// <summary>
        /// 停止消费
        /// </summary>
        void StopConsume();

        /// <summary>
        /// 确认消息
        /// </summary>
        /// <param name="deliveryTag">消息标签</param>
        void Ack(ulong deliveryTag);

        /// <summary>
        /// 拒绝消息
        /// </summary>
        /// <param name="deliveryTag">消息标签</param>
        /// <param name="requeue">是否重新入队</param>
        void Nack(ulong deliveryTag, bool requeue = false);
    }
}
