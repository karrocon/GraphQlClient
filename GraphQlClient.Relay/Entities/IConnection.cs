using System.Collections.Generic;

namespace GraphQlClient.Relay
{
    public interface IConnection
    {
        IEnumerable<IEdge> Edges { get; set; }
        PageInfo PageInfo { get; set; }
    }
}