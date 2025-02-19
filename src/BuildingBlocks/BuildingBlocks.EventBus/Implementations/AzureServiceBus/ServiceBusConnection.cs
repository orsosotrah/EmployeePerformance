using Azure.Messaging.ServiceBus;

namespace BuildingBlocks.EventBus.Implementations.AzureServiceBus
{
    public class ServiceBusConnection : IAsyncDisposable
    {
        private readonly string _connectionString;
        private ServiceBusClient? _topicClient;
        private bool _disposed;

        public ServiceBusConnection(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public ServiceBusClient TopicClient
        {
            get
            {
                if (_topicClient.IsNullOrClosed())
                {
                    _topicClient = new ServiceBusClient(_connectionString);
                }

                return _topicClient;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            if (_topicClient != null)
            {
                await _topicClient.DisposeAsync();
            }

            _disposed = true;
        }
    }

    internal static class ServiceBusConnectionExtensions
    {
        public static bool IsNullOrClosed(this ServiceBusClient? client)
        {
            return client == null || client.IsClosed;
        }
    }
}