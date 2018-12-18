namespace GraphQlClient.Relay.Entities
{
    public interface IEdge<TNode> : IEdge
    {
        new TNode Node { get; set; }
    }
}