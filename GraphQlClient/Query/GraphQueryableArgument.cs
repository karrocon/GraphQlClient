using GraphQlClient.Extensions;

namespace GraphQlClient
{
    internal class GraphQueryableArgument : GraphQueryableElement
    {
        public GraphQueryableArgumentValue Value { get; }

        public GraphQueryableArgument(string name, GraphQueryableArgumentValue value) : base(name)
        {
            Value = value;
        }

        public override string ToQueryString()
        {
            return $"{Name.ToLowerInvariantFirst()}:{Value.ToQueryString()}";
        }
    }
}
