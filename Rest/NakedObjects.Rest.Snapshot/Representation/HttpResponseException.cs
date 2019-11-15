using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NakedObjects.Rest.Snapshot.Representations
{
    public class HttpResponseException : Exception
    {
        public HttpResponseMessage HttpResponseMessage { get; private set; }

        public HttpResponseException(HttpResponseMessage httpResponseMessage) {
            HttpResponseMessage = httpResponseMessage;
        }
    }
}
