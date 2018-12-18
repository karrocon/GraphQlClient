namespace GraphQlClient
{
    internal class GraphQueryableVariable : GraphQueryableArgumentValue
    {
        public GraphQueryableVariable(string name) : base(name) { }

        public override string ToQueryString()
        {
            return $"${Name}";
        }
    }
}
