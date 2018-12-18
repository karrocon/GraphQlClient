using System;

namespace GraphQlClient
{
    public static class GraphQueryStringBuilder
    {
        public static string Build<TRootObject>(Action<IGraphQueryableObject<TRootObject>> buildFcn)
        {
            var configuration = new GraphQueryableObject<TRootObject>(string.Empty);

            buildFcn(configuration);

            return configuration.ToQueryString();
        }
    }
}
