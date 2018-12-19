using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQlClient.Relay.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQlClient.Relay.Extensions
{
    public static class IGraphQLClientExtensions
    {
        public static async Task<IEnumerable<T>> QueryAllPagesAsync<T>(this IGraphQLClient client, GraphQLRequest request)
        {
            var result = new List<T>();
            var response = await client.SendQueryAsync(request);

            if (response.Errors != null)
            {
                throw new Exception($"GraphQL exception: Query returned the following errors:\n {string.Join("\n", response.Errors.Select(e => e.Message))}");
            }

            var data = response.Data.ToObject<T>();
            result.Add(data);

            var page = new PaginableResponse<T>(GraphQueryStringParser.Parse(request.Query));
            var queryString = string.Empty;
            while (!string.IsNullOrEmpty(queryString = page.GetNextPageQueryString(data)))
            {
                response = await client.SendQueryAsync(new GraphQLRequest
                {
                    OperationName = request.OperationName,
                    Query = queryString,
                    Variables = request.Variables
                });
                data = response.Data.ToObject<T>();
                result.Add(data);
            }

            return result;
        }
    }
}
