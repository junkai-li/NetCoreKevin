using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.RabbitMQ.Interfaces
{
    public interface IRabbitMQPublisherService
    {
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
        void PublishList<T>(string exchange, string routingKey,List<T> messages, string queue = "", string exchangeType = "", bool isTx = false, bool isConfirmSelect = false, EventHandler<BasicAckEventArgs>? BasicAcks = default, EventHandler<BasicNackEventArgs>? BasicNacks = default);
    }
}
