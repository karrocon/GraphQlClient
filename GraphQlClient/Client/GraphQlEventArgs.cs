namespace GraphQlClient
{
    public class GraphQlEventArgs : GraphQlEventArgs<GraphQlResponseMessage, dynamic, dynamic> { }

    public class GraphQlEventArgs<TResponse> : GraphQlEventArgs<TResponse, dynamic, dynamic> where TResponse : GraphQlResponseMessage { }

    public class GraphQlEventArgs<TResponse, TResponseData> : GraphQlEventArgs<TResponse, TResponseData, dynamic> where TResponse : GraphQlResponseMessage<TResponseData> { }

    public class GraphQlEventArgs<TResponse, TResponseData, TResponseExtensions> : IGraphQlEventArgs, IGraphQlEventArgs<TResponse> where TResponse : GraphQlResponseMessage<TResponseData, TResponseExtensions>
    {
        public GraphQlRequestMessage Request { get; set; }
        public TResponse Response { get; set; }

        object IGraphQlEventArgs.Response => Response;
    }
}