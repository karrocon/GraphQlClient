using System.Collections.Generic;

namespace GraphQlClient
{
    public class GraphQlResponseError
    {
		public string Message { get; set; }

        public IEnumerable<GraphQlResponseErrorLocation> Locations { get; set; }
    }
}