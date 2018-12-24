using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace GraphQlClient.Client
{
    public class GraphQlRequestMessage : HttpRequestMessage
    {
        #region Properties

        public new HttpContent Content
        {
            get => base.Content;
            private set => base.Content = value;
        }

        private string _operationName;
        public string OperationName
        {
            get => _operationName;
            set
            {
                _operationName = value;
                SetContentFromProperties();
            }
        }

        private string _query;
        public string Query
        {
            get => _query;
            set
            {
                _query = value;
                SetContentFromProperties();
            }
        }

        private dynamic _variables;
        public dynamic Variables
        {
            get => _variables;
            set
            {
                _variables = value;
                SetContentFromProperties();
            }
        }

        #endregion

        #region Constructors

        public GraphQlRequestMessage() : this(HttpMethod.Post, (Uri)null) { }

        public GraphQlRequestMessage(HttpMethod method, string requestUri) : this(method, new Uri(requestUri)) { }

        public GraphQlRequestMessage(HttpMethod method, Uri requestUri) : base(method, requestUri) { }

        #endregion

        #region Private helpers

        private void SetContentFromProperties()
        {
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                operationName = _operationName,
                query = _query,
                variables = _variables
            }), Encoding.UTF8, "application/json");
        }

        #endregion
    }
}