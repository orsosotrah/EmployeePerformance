namespace BuildingBlocks.EventBus.Events
{
    public record IntegrationEvent : IIntegrationEvent
    {
        public Guid Id { get; private init; }
        public DateTime CreationDate { get; private init; }
        public string EventType => GetType().Name;

        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public IntegrationEvent(Guid id, DateTime createDate)
        {
            Id = id;
            CreationDate = createDate;
        }
    }
}