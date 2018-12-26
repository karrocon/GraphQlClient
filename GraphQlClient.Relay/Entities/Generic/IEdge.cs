namespace GraphQlClient.Relay
{
    public interface IEdge<TNode> : IEdge
    {
        new TNode Node { get; set; }
    }
}