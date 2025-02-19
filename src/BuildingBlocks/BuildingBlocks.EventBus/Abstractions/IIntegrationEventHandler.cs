using BuildingBlocks.EventBus.Events;

namespace BuildingBlocks.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent>
        where TIntegrationEvent : IIntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    // Base interface tanpa generic parameter 
    // (berguna untuk service registration)
    public interface IIntegrationEventHandler
    {
    }
}