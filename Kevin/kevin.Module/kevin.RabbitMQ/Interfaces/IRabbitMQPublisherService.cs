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
        /// <param name="basicAcks">确认机制成功回调</param>
        /// <param name="basicNacks">确认机制失败回调</param>
        /// <param name="isTtl">是否开启TTL机制</param>
        /// <param name="ttl">TTL时间</param>
        /// <param name="dlxExchange">死信交换机</param>
        /// <param name="dlxRoutingKey">死信路由键</param>
        /// <exception cref="UserFriendlyException">确认机制开启，必须传入BasicAcks和BasicNacks</exception>
        /// <exception cref="UserFriendlyException">开启TTL机制消息过期必须传入死信交换机和死信路由</exception>
        void PublishList<T>(string exchange, string routingKey,List<T> messages, string queue = "", string exchangeType = "", bool isTx = false, bool isConfirmSelect = false, EventHandler<BasicAckEventArgs>? basicAcks = default, EventHandler<BasicNackEventArgs>? basicNacks = default, bool isTtl = false, int ttl = 30000, string? dlxExchange = "", string? dlxRoutingKey = "") where T : class;
        Task PublishListAsync<T>(string exchange, string routingKey, List<T> messages, string queue = "", string exchangeType = "", bool isTx = false, bool isConfirmSelect = false, EventHandler<BasicAckEventArgs>? basicAcks = default, EventHandler<BasicNackEventArgs>? basicNacks = default, bool isTtl = false, int ttl = 30000, string? dlxExchange = "", string? dlxRoutingKey = "") where T : class;
 
    }
}
