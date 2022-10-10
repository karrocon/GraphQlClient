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

        public Task<TResult> MutateAsync<TResult>(IMutation<TResult> mutation)
        {
            var request = mutation.ToGraphQlRequestMessage();

            return SendAsync<TResult>(request);
        }

        public async Task<T> SendAsync<T>(GraphQlRequestMessage request)
        {
            return (await SendAsync<GraphQlResponseMessage<T>, T>(request)).Data;
        }

        public async Task<TResponse> SendAsync<TResponse, TResponseData>(GraphQlRequestMessage request) where TResponse : GraphQlResponseMessage<TResponseData>
        {
            var currentAttempt = 0;
            HttpResponseMessage response = null;
            Exception lastException = null;
            do
            {
                response = await _httpClient.SendAsync(new GraphQlRequestMessage(request));
                var rawResponseContent = await response.Content.ReadAsStringAsync();

                TResponse parsedResponse = null;
                if (response.IsSuccessStatusCode && (parsedResponse = JsonConvert.DeserializeObject<TResponse>(rawResponseContent)).IsValid())
                {
                    return parsedResponse;
                }
                else
                {
                    OnGraphQlError?.Invoke(this, new GraphQlEventArgs<TResponse, TResponseData>
                    {
                        Request = request,
                        Response = parsedResponse
                    });

                    currentAttempt++;
                    lastException = new HttpRequestException($"Request did not return successful status code (" +
                        $"StatusCode: {response.StatusCode}, " +
                        $"Content: {rawResponseContent}, " +
                        $"Headers: {response.Headers}, " +
                        $"OperationName: {request.OperationName}, " +
                        $"Query: {request.Query}), " +
                        $"Variables: {request.Variables}");
                }
            } while (ShouldRetry(request, response));

            throw new Exception($"Query failed after {currentAttempt} attempt(s) (see inner exception for details)", lastException);
        }

        #endregion

        #region Protected methods

        protected virtual bool ShouldRetry(GraphQlRequestMessage request, HttpResponseMessage response)
        {
            return false;
        }

        #endregion
    }
}
