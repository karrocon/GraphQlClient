using Newtonsoft.Json;

namespace GraphQlClient
{
    internal class GraphQueryableLiteral<T> : GraphQueryableArgumentValue
    {
        public T Value { get; }

        public GraphQueryableLiteral(T value) : base(string.Empty)
        {
            Value = value;
        }

        public override string ToQueryString()
        {
            return JsonConvert.SerializeObject(Value);
        }
    }
}
