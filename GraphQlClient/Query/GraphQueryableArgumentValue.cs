namespace GraphQlClient
{
    internal abstract class GraphQueryableArgumentValue : GraphQueryableElement, IGraphQueryable
    {
        protected GraphQueryableArgumentValue(string name) : base(name) { }
    }
}
