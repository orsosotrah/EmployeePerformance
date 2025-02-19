using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Contracts.Queries
{
    public interface IQueryHandler<in TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
    }

    public abstract record BaseQuery<TResponse> : IQuery<TResponse>
    {
        public Guid Id { get; private init; }
        public DateTime Timestamp { get; private init; }

        protected BaseQuery()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
        }
    }
}
