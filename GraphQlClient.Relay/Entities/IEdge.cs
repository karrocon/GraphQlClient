namespace GraphQlClient.Relay.Entities
{
    public interface IEdge
    {
        string Cursor { get; set; }
        object Node { get; set; }
    }
}