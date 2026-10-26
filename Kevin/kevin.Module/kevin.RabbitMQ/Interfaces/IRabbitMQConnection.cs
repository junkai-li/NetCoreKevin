using RabbitMQ.Client;

namespace kevin.RabbitMQ.Interfaces
{
    public interface IRabbitMQConnection : IDisposable
    {
        IChannel CreateChannel(bool publisherConfirmationsEnabled = false);
    }
}
