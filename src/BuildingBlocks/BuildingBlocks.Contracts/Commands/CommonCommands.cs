namespace BuildingBlocks.Contracts.Commands
{
    public abstract record BaseCommand : ICommand
    {
        public Guid Id { get; private init; }
        public DateTime Timestamp { get; private init; }

        protected BaseCommand()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
        }
    }

    public abstract record BaseCommand<TResponse> : ICommand<TResponse>
    {
        public Guid Id { get; private init; }
        public DateTime Timestamp { get; private init; }

        protected BaseCommand()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
        }
    }

    // Helper untuk void commands
    public record struct Unit
    {
        public static Unit Value => default;
    }
}