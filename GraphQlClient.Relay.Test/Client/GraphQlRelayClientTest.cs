using GraphQlClient.Relay.Samples;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GraphQlClient.Relay.Test.Client
{
    public class GraphQlRelayClientTest
    {
        GraphQlRelayClient _client;

        HttpClient _httpClient;

        public GraphQlRelayClientTest()
        {
            _httpClient = Substitute.For<HttpClient>();
        }

        [Fact(Skip = "Skipped for now because it requires some extra job to mock httpclient")]
        public async Task SendAsyncShouldRetrieveAllPagesWhenThereIsOnlyOneConnectionInTheQuery()
        {
            var firstQueryRoot = new QueryRoot
            {
                Shops = new Connection<Shop>
                {
                    Edges = new[]
                    {
                        new Edge<Shop>
                        {
                            Cursor = "Cursor1",
                            Node = new Shop
                            {
                                Name = "Shop1"
                            }
                        }
                    },
                    PageInfo = new PageInfo
                    {
                        HasNextPage = true,
                        HasPreviousPage = false
                    }
                }
            };
            var secondQueryRoot = new QueryRoot
            {
                Shops = new Connection<Shop>
                {
                    Edges = new[]
                    {
                        new Edge<Shop>
                        {
                            Cursor = "Cursor2",
                            Node = new Shop
                            {
                                Name = "Shop2"
                            }
                        }
                    },
                    PageInfo = new PageInfo
                    {
                        HasNextPage = false,
                        HasPreviousPage = true
                    }
                }
            };

            _httpClient.SendAsync(Arg.Any<HttpRequestMessage>()).Returns(call =>
            {
                switch (call.ReceivedCalls().Count())
                {
                    case 1:
                        return Task.FromResult(new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(new
                            {
                                data = firstQueryRoot
                            })),
                            StatusCode = HttpStatusCode.OK
                        });
                    case 2:
                        return Task.FromResult(new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(new
                            {
                                data = secondQueryRoot
                            })),
                            StatusCode = HttpStatusCode.OK
                        });
                    default:
                        throw new Exception("This should not happen");
                }
            });

            _client = new GraphQlRelayClient
            {
                _httpClient = _httpClient
            };

            var result = await _client.QueryAllPagesAsync<QueryRoot>(new GraphQlRequestMessage
            {
                Query = "{ shops(first:1) { edges { cursor, node { name } }, pageInfo { hasNextPage, hasPreviousPage } } }"
            });

            Assert.Equal(2, result.Count());
            Assert.Equal(firstQueryRoot.Shops.Edges.First().Node.Name, result.ElementAt(0).Shops.Edges.First().Node.Name);
            Assert.Equal(secondQueryRoot.Shops.Edges.First().Node.Name, result.ElementAt(1).Shops.Edges.First().Node.Name);
        }
    }
}
