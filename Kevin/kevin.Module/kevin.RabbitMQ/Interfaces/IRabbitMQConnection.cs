using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.RabbitMQ.Interfaces
{
    public interface IRabbitMQConnection : IDisposable
    {
        IModel CreateModel();
    }
}
