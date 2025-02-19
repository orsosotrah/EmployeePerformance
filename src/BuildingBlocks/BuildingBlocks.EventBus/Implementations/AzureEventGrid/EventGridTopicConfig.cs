namespace BuildingBlocks.EventBus.Implementations.AzureEventGrid
{
    public class EventGridTopicConfig
    {
        public string TopicEndpoint { get; }
        public string TopicKey { get; }
        public string? TopicName { get; }

        public EventGridTopicConfig(string topicEndpoint, string topicKey, string? topicName = null)
        {
            TopicEndpoint = topicEndpoint ?? throw new ArgumentNullException(nameof(topicEndpoint));
            TopicKey = topicKey ?? throw new ArgumentNullException(nameof(topicKey));
            TopicName = topicName;
        }
    }
}