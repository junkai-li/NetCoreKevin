using RabbitMQ.Client;

namespace kevin.RabbitMQ.Interfaces
{
    public interface IRabbitMQConnection : IDisposable
    {
        IModel CreateModel();
    }
}
