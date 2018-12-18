using System.Collections.Generic;

namespace GraphQlClient.Relay.Entities
{
    public class Connection<TNode> : IConnection<TNode> where TNode : class
    {
        public IEnumerable<IEdge<TNode>> Edges { get; set; }
        public PageInfo PageInfo { get; set; }
        IEnumerable<IEdge> IConnection.Edges { get => Edges; set => Edges = (IEnumerable<IEdge<TNode>>)value; }
    }
}