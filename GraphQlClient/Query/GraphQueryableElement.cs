namespace GraphQlClient
{
    internal abstract class GraphQueryableElement : IGraphQueryable
    {
        public string Name { get; set; }

        protected GraphQueryableElement(string name)
        {
            Name = name;
        }

        public abstract string ToQueryString();
    }
}
