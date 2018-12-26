using Newtonsoft.Json;
using System.Collections.Generic;

namespace GraphQlClient
{
    public class GraphQlResponseMessage : GraphQlResponseMessage<dynamic> { }

    public class GraphQlResponseMessage<TData> : GraphQlResponseMessage<TData, dynamic> { }

    public class GraphQlResponseMessage<TData, TExtensions>
    {
        public TData Data { get; set; }
        public IEnumerable<GraphQlResponseError> Errors { get; set; }
        public TExtensions Extensions { get; set; }

        [JsonIgnore] public dynamic RawData { get; set; }
    }
}
