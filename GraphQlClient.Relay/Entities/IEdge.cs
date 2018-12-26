namespace GraphQlClient.Relay
{
    public interface IEdge
    {
        string Cursor { get; set; }
        object Node { get; set; }
    }
}