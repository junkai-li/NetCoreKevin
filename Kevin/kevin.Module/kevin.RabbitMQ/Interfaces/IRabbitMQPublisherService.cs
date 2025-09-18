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
    
        void PublishList<T>(string exchange, string routingKey,List<T> messages, string queue = "", string exchangeType = "", bool isTx = false, bool isConfirmSelect = false, EventHandler<BasicAckEventArgs>? BasicAcks = default, EventHandler<BasicNackEventArgs>? BasicNacks = default);
    }
}
