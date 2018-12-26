using System.Collections.Generic;

namespace GraphQlClient.Relay
{
    public class Connection<TNode> : IConnection<TNode> where TNode : class
    {
        public IEnumerable<Edge<TNode>> Edges { get; set; }
        public PageInfo PageInfo { get; set; }
        IEnumerable<IEdge> IConnection.Edges { get => Edges; set => Edges = (IEnumerable<Edge<TNode>>)value; }
    }
}