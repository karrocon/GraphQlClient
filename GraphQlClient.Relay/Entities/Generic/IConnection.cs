using System.Collections.Generic;

namespace GraphQlClient.Relay.Entities
{
    public interface IConnection<TNode> : IConnection where TNode : class
    {
        new IEnumerable<IEdge<TNode>> Edges { get; set; }
    }
}