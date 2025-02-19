namespace BuildingBlocks.EventBus.Events
{
    public interface IIntegrationEvent
    {
        Guid Id { get; }
        DateTime CreationDate { get; }
        string EventType { get; }
    }
}