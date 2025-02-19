using BuildingBlocks.EventBus.Events;

namespace BuildingBlocks.EventBus.Implementations.AzureServiceBus
{
    public class ServiceBusSubscriptionManager : EventBusSubscriptionsManager
    {
        public string TopicName { get; }
        public string SubscriptionName { get; }

        public ServiceBusSubscriptionManager(string topicName, string subscriptionName)
            : base()
        {
            TopicName = topicName;
            SubscriptionName = subscriptionName;
        }
    }
}