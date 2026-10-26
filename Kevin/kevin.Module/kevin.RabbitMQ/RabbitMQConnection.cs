using kevin.RabbitMQ.Interfaces;
using RabbitMQ.Client;

namespace kevin.RabbitMQ
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnection _connection;
        private bool _isDisposed;

        public RabbitMQConnection(ConnectionFactory factory)
        {
            ArgumentNullException.ThrowIfNull(factory);
            // RabbitMQ.Client 7.x 仅提供异步建连，构造阶段同步等待完成
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        }

        public IChannel CreateChannel(bool publisherConfirmationsEnabled = false)
        {
            EnsureNotDisposed();
            var options = new CreateChannelOptions(publisherConfirmationsEnabled, publisherConfirmationsEnabled);
            return _connection.CreateChannelAsync(options).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            _connection.Dispose();

            _isDisposed = true;
        }

        ~RabbitMQConnection()
        {
            Dispose(false);
        }

        private void EnsureNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(RabbitMQConnection));
            }
        }
    }
}
