using System.Collections.Generic;

namespace GraphQlClient.Client
{
    public class GraphQlResponseError
    {
		public string Message { get; set; }

        public IEnumerable<GraphQlResponseErrorLocation> Locations { get; set; }
    }
}