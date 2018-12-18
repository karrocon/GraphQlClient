using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQlClient.Relay.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GraphQlClient.Relay.Extensions
{
    public static class IGraphQLClientExtensions
    {
        public static async Task<IEnumerable<T>> QueryAllPagesAsync<T>(this IGraphQLClient client, GraphQLRequest request)
        {
            var result = new List<T>();
            var response = await client.SendQueryAsync(request);
            var data = response.GetDataFieldAs<T>("data");
            result.Add(data);

            var page = new PaginableResponse<T>(GraphQueryStringParser.Parse(request.Query), data);
            var queryString = string.Empty;
            while (!string.IsNullOrEmpty(queryString = page.GetNextPageQueryString()))
            {
                response = await client.SendQueryAsync(new GraphQLRequest
                {
                    OperationName = request.OperationName,
                    Query = queryString,
                    Variables = request.Variables
                });
                data = response.GetDataFieldAs<T>("data");
                result.Add(data);
            }

            return result;
        }
    }
}
