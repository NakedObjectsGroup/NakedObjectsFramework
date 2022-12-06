using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Model;
using Newtonsoft.Json.Linq;

namespace ROSI.Test.Helpers; 

internal class StubHttpMessageHandler : HttpMessageHandler {
    public StubHttpMessageHandler(RestfulObjectsControllerBase api) => Api = api;

    public RestfulObjectsControllerBase Api { get; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        var url = request.RequestUri;
        var segments = url.Segments;
        var obj = segments[2].TrimEnd('/');
        var key = segments[3].TrimEnd('/');
        var action = segments[5].TrimEnd('/');
        var method = request.Method;
        var query = url.Query.TrimStart('?');

        var am = string.IsNullOrEmpty(query)
            ? ModelBinderUtils.CreateArgumentMap(new JObject(), true)
            : ModelBinderUtils.CreateSimpleArgumentMap(query) ?? ModelBinderUtils.CreateArgumentMap(JObject.Parse(HttpUtility.UrlDecode(query)), true);

        ActionResult ar;

        if (method == HttpMethod.Get) {
            ar = Api.GetInvoke(obj, key, action, am);
        }
        else if (method == HttpMethod.Post) {
            ar = Api.PostInvoke(obj, key, action, am);
        }
        else if (method == HttpMethod.Put) {
            ar = Api.PutInvoke(obj, key, action, am);
        }
        else {
            throw new NotImplementedException();
        }

        var (json, sc, _) = TestHelpers.ReadActionResult(ar, Api.ControllerContext.HttpContext);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = new HttpResponseMessage((HttpStatusCode)sc);
        response.Content = content;

        var task = new Task<HttpResponseMessage>(() => response);
        task.Start();

        return task;
    }
}