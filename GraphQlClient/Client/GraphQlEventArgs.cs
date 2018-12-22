namespace GraphQlClient.Client
{
    public class GraphQlEventArgs
    {
        public GraphQlRequestMessage Request { get; set; }
        public GraphQlResponseMessage Response { get; set; }
    }

    public class GraphQlEventArgs<TResponse, TResponseData> : GraphQlEventArgs where TResponse : GraphQlResponseMessage<TResponseData>
    {
        public new TResponse Response
        {
            get => (TResponse)base.Response;
            set => base.Response = value;
        }
    }
}