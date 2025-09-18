using kevin.RabbitMQ.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.RabbitMQ
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private bool _isDisposed;

        public RabbitMQConnection(ConnectionFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory)); 
            _connection = factory.CreateConnection();
        }

        public IModel CreateModel()
        {
            EnsureNotDisposed();
            return _connection.CreateModel();
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
