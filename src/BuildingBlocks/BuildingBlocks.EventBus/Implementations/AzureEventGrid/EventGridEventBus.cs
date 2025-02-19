using BuildingBlocks.EventBus.Abstractions;
using BuildingBlocks.EventBus.Events;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EventBus.Implementations.AzureEventGrid
{
    public class EventGridEventBus : IEventBus
    {
        private readonly EventGridPublisher _publisher;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly ILogger<EventGridEventBus> _logger;
        private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";

        public EventGridEventBus(
            EventGridPublisher publisher,
            IEventBusSubscriptionsManager subsManager,
            ILogger<EventGridEventBus> logger)
        {
            _publisher = publisher;
            _subsManager = subsManager;
            _logger = logger;
        }

        public async Task PublishAsync(IIntegrationEvent @event)
        {
            var eventName = @event.GetType().Name.Replace(INTEGRATION_EVENT_SUFFIX, "");

            try
            {
                await _publisher.PublishAsync(eventName, @event);
                _logger.LogInformation("Published integration event to Event Grid: {EventId} - ({EventName})",
                    @event.Id, eventName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration event to Event Grid: {EventId} - ({EventName})",
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
        }

        public void Unsubscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name.Replace(INTEGRATION_EVENT_SUFFIX, "");

            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _subsManager.RemoveSubscription<T, TH>();
        }
    }
}