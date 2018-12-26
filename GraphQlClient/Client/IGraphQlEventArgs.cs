namespace GraphQlClient
{
    public interface IGraphQlEventArgs
    {
        GraphQlRequestMessage Request { get; }
        object Response { get; }
    }

    public interface IGraphQlEventArgs<TResponse>
    {
        GraphQlRequestMessage Request { get; }
        TResponse Response { get; set; }
    }
}