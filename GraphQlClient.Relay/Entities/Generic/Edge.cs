namespace GraphQlClient.Relay.Entities
{
    public class Edge<TNode> : IEdge<TNode> where TNode : class
    {
        public string Cursor { get; set; }
        public TNode Node { get; set; }
        object IEdge.Node { get => Node; set => Node = (TNode)value; }
    }
}