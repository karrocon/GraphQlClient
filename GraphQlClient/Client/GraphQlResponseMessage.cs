using Newtonsoft.Json;
using System.Collections.Generic;

namespace GraphQlClient.Client
{
    public class GraphQlResponseMessage
    {
        public dynamic Data { get; set; }
    }

    public class GraphQlResponseMessage<T> : GraphQlResponseMessage
    {
        public new T Data { get; set; }
        public IEnumerable<GraphQlResponseError> Errors { get; set; }

        [JsonIgnore] public dynamic RawData { get; set; }
    }
}
