using System.Collections.Generic;

namespace GraphQlClient.Relay
{
    public interface IConnection<TNode> : IConnection where TNode : class
    {
        new IEnumerable<Edge<TNode>> Edges { get; set; }
    }
}