using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Contracts.Queries
{
    public interface IQuery<TResponse>
    {
        Guid Id { get; }
        DateTime Timestamp { get; }
    }
}
