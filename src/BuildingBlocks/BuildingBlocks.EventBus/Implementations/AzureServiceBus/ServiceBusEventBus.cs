using System.Text.Json;
using Azure.Messaging.ServiceBus;
using BuildingBlocks.EventBus.Abstractions;
using BuildingBlocks.EventBus.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EventBus.Implementations.AzureServiceBus
{
    public class ServiceBusEventBus : IEventBus, IAsyncDisposable
    {
        private readonly ServiceBusConnection _serviceBusConnection;
        private readonly ILogger<ServiceBusEventBus> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly string _topicName;
        private readonly ServiceBusSender _sender;
        private readonly ServiceBusProcessor _processor;
        private readonly string _subscriptionName;
        private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";

        public ServiceBusEventBus(
            ServiceBusConnection serviceBusConnection,
            ILogger<ServiceBusEventBus> logger,
            IServiceScopeFactory serviceScopeFactory,
            IEventBusSubscriptionsManager subsManager,
            string topicName,
            string subscriptionName)
        {
            _serviceBusConnection = serviceBusConnection;
            _logger = logger;
            _subsManager = subsManager;
            _topicName = topicName;
            _subscriptionName = subscriptionName;
            _serviceScopeFactory = serviceScopeFactory;
            _sender = _serviceBusConnection.TopicClient.CreateSender(_topicName);
            _processor = _serviceBusConnection.TopicClient.CreateProcessor(_topicName, _subscriptionName);

            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
            _processor.ProcessMessageAsync += ProcessMessagesAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;
        }

        public async Task PublishAsync(IIntegrationEvent @event)
        {
            var eventName = @event.GetType().Name.Replace(INTEGRATION_EVENT_SUFFIX, "");
            var jsonMessage = JsonSerializer.Serialize(@event);
            var body = new BinaryData(jsonMessage);

            var message = new ServiceBusMessage(body)
            {
                MessageId = @event.Id.ToString(),
                Subject = eventName,
            };

            try
            {
                await _sender.SendMessageAsync(message);
                _logger.LogInformation("Published integration event: {EventId} - ({EventName})", @event.Id, eventName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration event: {EventId} - ({EventName})", @event.Id, eventName);
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

        private void StartBasicConsume()
        {
            if (!_processor.IsProcessing)
            {
                _processor.StartProcessingAsync().GetAwaiter().GetResult();
            }
        }

        private async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
        {
            var eventName = args.Message.Subject;
            var message = args.Message;

            try
            {
                await ProcessEvent(eventName, message);
                await args.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error Processing message: {ExceptionMessage}", ex.Message);
                await args.AbandonMessageAsync(message);
            }
        }

        private async Task ProcessEvent(string eventName, ServiceBusReceivedMessage message)
        {
            if (!_subsManager.HasSubscriptionsForEvent(eventName))
            {
                return;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var subscriptions = _subsManager.GetHandlersForEvent(eventName);

            foreach (var subscription in subscriptions)
            {
                var handler = scope.ServiceProvider.GetService(subscription);
                if (handler == null) continue;

                var eventType = _subsManager.GetEventTypeByName(eventName);
                var integrationEvent = JsonSerializer.Deserialize(message.Body.ToString(), eventType);
                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                await (Task)concreteType.GetMethod("Handle").Invoke(handler, new[] { integrationEvent });
            }
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogError(arg.Exception, "Error Processing message: {ExceptionMessage}", arg.Exception.Message);
            return Task.CompletedTask;
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            // Additional cleanup if needed
        }

        public async ValueTask DisposeAsync()
        {
            if (_processor != null)
            {
                await _processor.DisposeAsync();
            }

            if (_sender != null)
            {
                await _sender.DisposeAsync();
            }

            if (_serviceBusConnection != null)
            {
                await _serviceBusConnection.DisposeAsync();
            }
        }
    }
}