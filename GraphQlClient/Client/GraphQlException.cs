using System;

namespace GraphQlClient
{
    [Serializable]
    internal class GraphQlException : Exception
    {
        public GraphQlException() { }

        public GraphQlException(string message) : base(message) { }

        public GraphQlException(string message, Exception innerException) : base(message, innerException) { }
    }
}