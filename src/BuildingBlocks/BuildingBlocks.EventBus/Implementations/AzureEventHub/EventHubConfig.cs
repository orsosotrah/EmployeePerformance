namespace BuildingBlocks.EventBus.Implementations.AzureEventHub
{
    public class EventHubConfig
    {
        public string ConnectionString { get; }
        public string EventHubName { get; }
        public string ConsumerGroup { get; }
        public string BlobStorageConnectionString { get; }
        public string BlobContainerName { get; }

        public EventHubConfig(
            string connectionString,
            string eventHubName,
            string consumerGroup,
            string blobStorageConnectionString,
            string blobContainerName)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            EventHubName = eventHubName ?? throw new ArgumentNullException(nameof(eventHubName));
            ConsumerGroup = consumerGroup ?? throw new ArgumentNullException(nameof(consumerGroup));
            BlobStorageConnectionString = blobStorageConnectionString ?? throw new ArgumentNullException(nameof(blobStorageConnectionString));
            BlobContainerName = blobContainerName ?? throw new ArgumentNullException(nameof(blobContainerName));
        }
    }
}