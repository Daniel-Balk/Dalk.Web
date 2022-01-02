using System;
using System.Runtime.Serialization;

namespace Dalk.Web.HttpServer
{
    [Serializable]
    internal class ResponseAlreadySentException : Exception
    {
        public ResponseAlreadySentException()
        {
        }

        public ResponseAlreadySentException(string message) : base(message)
        {
        }

        public ResponseAlreadySentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ResponseAlreadySentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}