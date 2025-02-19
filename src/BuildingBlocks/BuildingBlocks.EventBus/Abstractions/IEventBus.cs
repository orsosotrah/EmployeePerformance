using BuildingBlocks.EventBus.Events;

namespace BuildingBlocks.EventBus.Abstractions
{
    public interface IEventBus
    {
        Task PublishAsync(IIntegrationEvent @event);

        void Subscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
}