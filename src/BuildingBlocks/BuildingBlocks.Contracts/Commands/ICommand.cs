using MediatR;

namespace BuildingBlocks.Contracts.Commands
{
    public interface ICommand<out TResponse>
    {
        Guid Id { get; }
        DateTime Timestamp { get; }
    }

    public interface ICommand : ICommand<Unit>
    {
    }
}