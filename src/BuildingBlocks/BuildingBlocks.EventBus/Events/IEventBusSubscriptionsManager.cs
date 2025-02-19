using BuildingBlocks.EventBus.Abstractions;

namespace BuildingBlocks.EventBus.Events
{
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }

        event EventHandler<string> OnEventRemoved;

        void AddSubscription<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        bool HasSubscriptionsForEvent<T>() where T : IIntegrationEvent;
        bool HasSubscriptionsForEvent(string eventName);

        Type GetEventTypeByName(string eventName);
        void Clear();

        IEnumerable<Type> GetHandlersForEvent<T>() where T : IIntegrationEvent;
        IEnumerable<Type> GetHandlersForEvent(string eventName);
    }
}