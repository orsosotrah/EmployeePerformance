using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using BuildingBlocks.EventBus.Abstractions;
using BuildingBlocks.EventBus.Events;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BuildingBlocks.EventBus.Implementations.AzureEventHub
{
    public class EventHubEventBus : IEventBus, IAsyncDisposable
    {
        private readonly EventHubProducerClient _producerClient;
        private readonly EventHubProcessor _processor;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly ILogger<EventHubProcessor> _logger;
        private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";

        public EventHubEventBus(
            EventHubConfig config,
            IEventBusSubscriptionsManager subsManager,
            ILogger<EventHubProcessor> logger)
        {
            _subsManager = subsManager;
            _logger = logger;
            _producerClient = new EventHubProducerClient(config.ConnectionString, config.EventHubName);

            _processor = new EventHubProcessor(
                config,
                logger,
                ProcessEventDataAsync);
        }

        public async Task PublishAsync(IIntegrationEvent @event)
        {
            var eventName = @event.GetType().Name.Replace(INTEGRATION_EVENT_SUFFIX, "");
            var jsonMessage = JsonSerializer.Serialize(@event);
            var eventData = new EventData(jsonMessage)
            {
                MessageId = @event.Id.ToString()
            };
            eventData.Properties.Add("EventType", eventName);

            try
            {
                using var eventBatch = await _producerClient.CreateBatchAsync();
                if (!eventBatch.TryAdd(eventData))
                {
                    throw new Exception($"Event is too large to fit in the batch: {eventName}");
                }

                await _producerClient.SendAsync(eventBatch);
                _logger.LogInformation("Published integration event to Event Hub: {EventId} - ({EventName})",
                    @event.Id, eventName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration event to Event Hub: {EventId} - ({EventName})",
                    @event.Id, eventName);
                throw;
            }
        }

        public void Subscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name.Replace(INTEGRATION_EVENT_SUFFIX, "");

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);

            _subsManager.AddSubscription<T, TH>();
            StartBasicConsume();
        }

        public void Unsubscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name.Replace(INTEGRATION_EVENT_SUFFIX, "");

            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _subsManager.RemoveSubscription<T, TH>();
        }

        private async Task ProcessEventDataAsync(EventData eventData)
        {
            var eventType = eventData.Properties["EventType"].ToString();
            if (!_subsManager.HasSubscriptionsForEvent(eventType))
            {
                return;
            }

            var messageData = eventData.Body.ToString();
            var integrationEvent = JsonSerializer.Deserialize(
                messageData,
                _subsManager.GetEventTypeByName(eventType));

            var handlers = _subsManager.GetHandlersForEvent(eventType);
            foreach (var handler in handlers)
            {
                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(_subsManager.GetEventTypeByName(eventType));
                await (Task)concreteType.GetMethod("Handle").Invoke(handler, new[] { integrationEvent });
            }
        }

        private void StartBasicConsume()
        {
            _processor.StartProcessingAsync().GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            await _processor.StopProcessingAsync();
            await _producerClient.DisposeAsync();
        }
    }
}