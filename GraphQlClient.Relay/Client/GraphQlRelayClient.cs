using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("GraphQlClient.Relay.Test")]
namespace GraphQlClient.Relay
{
    public class GraphQlRelayClient : GraphQlClient
    {
        protected event EventHandler<IGraphQlEventArgs> OnBeforeQueryPage;

        public async Task<IEnumerable<T>> QueryAllPagesAsync<T>(GraphQlRequestMessage request)
        {
            return (await QueryAllPagesAsync<GraphQlResponseMessage<T>, T>(request)).Select(x => x.Data);
        }

        public async Task<IEnumerable<TResponse>> QueryAllPagesAsync<TResponse, TResponseData>(GraphQlRequestMessage request) where TResponse : GraphQlResponseMessage<TResponseData>
        {
            var result = new List<TResponse>();
            var response = await SendAsync<TResponse, TResponseData>(request);

            result.Add(response);

            var page = new PaginableResponse<TResponseData>(GraphQueryStringParser.Parse(request.Query));
            var queryString = string.Empty;
            while (!string.IsNullOrEmpty(queryString = page.GetNextPageQueryString(response.Data)))
            {
                OnBeforeQueryPage?.Invoke(this, new GraphQlEventArgs<TResponse, TResponseData>
                {
                    Request = request,
                    Response = response
                });

                var pageRequest = new GraphQlRequestMessage
                {
                    OperationName = request.OperationName,
                    Query = queryString,
                    Variables = request.Variables,
                    RequestUri = request.RequestUri
                };

                foreach (var header in request.Headers)
                {
                    pageRequest.Headers.Add(header.Key, header.Value);
                }

                response = await SendAsync<TResponse, TResponseData>(pageRequest);

                result.Add(response);
            }

            return result;
        }
    }
}
