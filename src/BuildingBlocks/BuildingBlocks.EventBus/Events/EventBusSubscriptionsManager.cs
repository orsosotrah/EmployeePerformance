using BuildingBlocks.EventBus.Abstractions;

namespace BuildingBlocks.EventBus.Events
{
    public class EventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventTypes;

        public event EventHandler<string>? OnEventRemoved;

        public EventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<Type>>();
            _eventTypes = new List<Type>();
        }

        public bool IsEmpty => !_handlers.Keys.Any();

        public void AddSubscription<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();

            AddSubscription(typeof(TH), eventName);

            if (!_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }
        }

        public void RemoveSubscription<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var handlerToRemove = typeof(TH);
            var eventName = GetEventKey<T>();
            RemoveHandler(eventName, handlerToRemove);
        }

        public bool HasSubscriptionsForEvent<T>() where T : IIntegrationEvent
        {
            var eventName = GetEventKey<T>();
            return HasSubscriptionsForEvent(eventName);
        }

        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

        public Type GetEventTypeByName(string eventName) => _eventTypes.Single(t => t.Name == eventName);

        public void Clear() => _handlers.Clear();

        public IEnumerable<Type> GetHandlersForEvent<T>() where T : IIntegrationEvent
        {
            var eventName = GetEventKey<T>();
            return GetHandlersForEvent(eventName);
        }

        public IEnumerable<Type> GetHandlersForEvent(string eventName) => _handlers[eventName];

        private void AddSubscription(Type handlerType, string eventName)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<Type>());
            }

            if (_handlers[eventName].Any(s => s == handlerType))
            {
                throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            _handlers[eventName].Add(handlerType);
        }

        private void RemoveHandler(string eventName, Type handlerType)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                return;
            }

            _handlers[eventName].Remove(handlerType);
            if (!_handlers[eventName].Any())
            {
                _handlers.Remove(eventName);
                var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                if (eventType != null)
                {
                    _eventTypes.Remove(eventType);
                }
                RaiseOnEventRemoved(eventName);
            }
        }

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);
        }

        private string GetEventKey<T>()
        {
            return typeof(T).Name;
        }
    }
}