using Azure;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EventBus.Implementations.AzureEventGrid
{
    public class EventGridPublisher
    {
        private readonly EventGridPublisherClient _client;
        private readonly ILogger<EventGridPublisher> _logger;

        public EventGridPublisher(EventGridTopicConfig config, ILogger<EventGridPublisher> logger)
        {
            var credential = new AzureKeyCredential(config.TopicKey);
            _client = new EventGridPublisherClient(new Uri(config.TopicEndpoint), credential);
            _logger = logger;
        }

        public async Task PublishAsync<T>(string eventType, T data)
        {
            try
            {
                var egEvent = new EventGridEvent(
                    subject: typeof(T).Name,
                    eventType: eventType,
                    dataVersion: "1.0",
                    data: data
                );

                await _client.SendEventAsync(egEvent);
                _logger.LogInformation("Event Grid event published successfully. Type: {EventType}", eventType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing to Event Grid. Type: {EventType}", eventType);
                throw;
            }
        }

        public async Task PublishManyAsync<T>(string eventType, IEnumerable<T> events)
        {
            try
            {
                var egEvents = events.Select(e => new EventGridEvent(
                    subject: typeof(T).Name,
                    eventType: eventType,
                    dataVersion: "1.0",
                    data: e
                )).ToList();

                await _client.SendEventsAsync(egEvents);
                _logger.LogInformation("Multiple Event Grid events published successfully. Type: {EventType}, Count: {Count}",
                    eventType, egEvents.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing multiple events to Event Grid. Type: {EventType}", eventType);
                throw;
            }
        }
    }
}