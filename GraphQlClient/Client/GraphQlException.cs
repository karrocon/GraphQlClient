using System;
using System.Runtime.Serialization;

namespace GraphQlClient.Client
{
    [Serializable]
    internal class GraphQlException : Exception
    {
        public GraphQlException()
        {
        }

        public GraphQlException(string message) : base(message)
        {
        }

        public GraphQlException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GraphQlException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}