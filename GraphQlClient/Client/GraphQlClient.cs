using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("GraphQlClient.Test")]
[assembly: InternalsVisibleTo("GraphQlClient.Relay.Test")]
namespace GraphQlClient
{
    public class GraphQlClient : IDisposable
    {
        #region Properties

        protected internal HttpClient _httpClient;

        #endregion

        #region Events

        protected event EventHandler<IGraphQlEventArgs> OnGraphQlError;

        #endregion

        #region Constructors

        public GraphQlClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        #endregion

        #region Public methods

        public Task<TResult> MutateAsync<TResult>(IMutation<TResult> mutation, uint retries = 0)
        {
            var request = mutation.ToGraphQlRequestMessage();

            return SendAsync<TResult>(request, retries);
        }

        public async Task<T> SendAsync<T>(GraphQlRequestMessage request, uint retries = 0)
        {
            return (await SendAsync<GraphQlResponseMessage<T>, T>(request, retries)).Data;
        }

        public async Task<TResponse> SendAsync<TResponse, TResponseData>(GraphQlRequestMessage request, uint retries = 0) where TResponse : GraphQlResponseMessage<TResponseData>
        {
            var currentAttempt = 1;
            Exception lastException = null;
            do
            {
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    currentAttempt++;
                    lastException = new HttpRequestException($"Request did not return successful status code (StatusCode: {response.StatusCode}, Content: {await response.Content.ReadAsStringAsync()})");
                }
                else
                {
                    var rawResponseString = await response.Content.ReadAsStringAsync();

                    var graphQlResponse = JsonConvert.DeserializeObject<TResponse>(rawResponseString);
                    graphQlResponse.RawData = JsonConvert.DeserializeObject<dynamic>(rawResponseString);

                    if (graphQlResponse.Errors == null)
                    {
                        return graphQlResponse;
                    }

                    OnGraphQlError?.Invoke(this, new GraphQlEventArgs<TResponse, TResponseData>
                    {
                        Request = request,
                        Response = graphQlResponse
                    });

                    lastException = new GraphQlException($"Response returned the following errors: {JsonConvert.SerializeObject(graphQlResponse.Errors)}");
                }
            } while (currentAttempt < retries + 1);

            throw new Exception($"Query failed after {retries + 1} attemp(s) (see inner exception for details)", lastException);
        }

        #endregion
    }
}
