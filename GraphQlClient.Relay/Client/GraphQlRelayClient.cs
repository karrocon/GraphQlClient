using GraphQlClient.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("GraphQlClient.Relay.Test")]
namespace GraphQlClient.Relay.Client
{
    public class GraphQlRelayClient : GraphQlClient.Client.GraphQlClient
    {
        protected event EventHandler<GraphQlEventArgs> OnBeforeQueryPage;

        public async Task<IEnumerable<T>> QueryAllPagesAsync<T>(GraphQlRequestMessage request, uint retries = 0)
        {
            return (await QueryAllPagesAsync<GraphQlResponseMessage<T>, T>(request, retries)).Select(x => x.Data);
        }

        public async Task<IEnumerable<TResponse>> QueryAllPagesAsync<TResponse, TResponseData>(GraphQlRequestMessage request, uint retries = 0) where TResponse : GraphQlResponseMessage<TResponseData>
        {
            var result = new List<TResponse>();
            var response = await SendAsync<TResponse, TResponseData>(request, retries);

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

                response = await SendAsync<TResponse, TResponseData>(new GraphQlRequestMessage
                {
                    OperationName = request.OperationName,
                    Query = queryString,
                    Variables = request.Variables
                }, retries);

                result.Add(response);
            }

            return result;
        }
    }
}
