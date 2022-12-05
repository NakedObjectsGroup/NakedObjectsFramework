using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Model;
using Newtonsoft.Json.Linq;

namespace ROSI.Test.Helpers
{
    internal class StubHttpMessageHandler : HttpMessageHandler
    {
        public RestfulObjectsControllerBase Api { get; }

        public StubHttpMessageHandler(RestfulObjectsControllerBase api) {
            Api = api;
        }


        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            var url = request.RequestUri;
            var segments = url.Segments;
            var obj = segments[2].TrimEnd('/');
            var key = segments[3].TrimEnd('/');
            var action = segments[5].TrimEnd('/');

            var am = string.IsNullOrEmpty(url.Query)
                ? ModelBinderUtils.CreateArgumentMap(new JObject(), true)
                : (request.Method == HttpMethod.Get)
                    ? ModelBinderUtils.CreateSimpleArgumentMap(url.Query)
                    : ModelBinderUtils.CreateArgumentMap(JObject.Parse(HttpUtility.UrlDecode(url.Query)), true);

            var ar = Api.GetInvoke(obj, key, action, am);

            var (json, sc, _) = TestHelpers.ReadActionResult(ar, Api.ControllerContext.HttpContext);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = new HttpResponseMessage((HttpStatusCode)sc);
            response.Content = content;

            var task = new Task<HttpResponseMessage>(() => response);
            task.Start();

            return task;
        }
    }
}
