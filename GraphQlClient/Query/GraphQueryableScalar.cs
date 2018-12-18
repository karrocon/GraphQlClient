namespace GraphQlClient
{
    internal class GraphQueryableScalar : GraphQueryableField
    {
        public GraphQueryableScalar(string name) : base(name) { }

        public override string ToQueryString()
        {
            return Name;
        }
    }
}
