using System;
using System.IO;
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

    private async Task<HttpResponseMessage> SendAsyncHome() {
        var ar = Api.AsGet().GetHome();
        return await GetResponse(ar);
    }

    private async Task<HttpResponseMessage> SendAsyncUser() {
        var ar = Api.AsGet().GetUser();
        return await GetResponse(ar);
    }

    private async Task<HttpResponseMessage> SendAsyncServices() {
        var ar = Api.AsGet().GetServices();
        return await GetResponse(ar);
    }

    private async Task<HttpResponseMessage> SendAsyncMenus() {
        var ar = Api.AsGet().GetMenus();
        return await GetResponse(ar);
    }

    private async Task<HttpResponseMessage> SendAsyncVersion() {
        var ar = Api.AsGet().GetVersion();
        return await GetResponse(ar);
    }

    private async Task<HttpResponseMessage> SendAsyncService(string serviceId) {
        var ar = Api.AsGet().GetService(serviceId);
        return await GetResponse(ar);
    }

    private async Task<HttpResponseMessage> SendAsyncMenu(string menuId) {
        var ar = Api.AsGet().GetMenu(menuId);
        return await GetResponse(ar);
    }


    private async Task<HttpResponseMessage> GetResponse(ActionResult ar) {
        var (json, sc, _) = await TestHelpers.ReadActionResult(ar, Api.ControllerContext.HttpContext);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = new HttpResponseMessage((HttpStatusCode)sc);
        response.Content = content;

        return response;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        var url = request.RequestUri;
        var segments = url.Segments;

        if (segments.Length == 1) {
            return await SendAsyncHome();
        }

        switch (segments[1]) {
            case "user":
                return await SendAsyncUser();
            case "services":
                return await SendAsyncServices();
            case "services/":
                return await SendAsyncService(segments[2]);
            case "menus":
                return await SendAsyncMenus();
            case "menus/":
                return await SendAsyncMenu(segments[2]);
            case "version":
                return await SendAsyncVersion();
        }

        var obj = segments[2].TrimEnd('/');
        var key = segments[3].TrimEnd('/');
        var action = segments[5].TrimEnd('/');
        var method = request.Method;
        var query = url.Query.TrimStart('?');
        var body = "";

        if (method == HttpMethod.Post || method == HttpMethod.Put) {
            await using var s = await request.Content.ReadAsStreamAsync(cancellationToken);
            using var sr = new StreamReader(s);
            s.Position = 0L;
            body = await sr.ReadToEndAsync();
            s.Position = 0L;
        }

        var am = string.IsNullOrEmpty(query)
            ? ModelBinderUtils.CreateArgumentMap(string.IsNullOrEmpty(body) ? new JObject() : JObject.Parse(body), true)
            : ModelBinderUtils.CreateSimpleArgumentMap(query) ?? ModelBinderUtils.CreateArgumentMap(JObject.Parse(HttpUtility.UrlDecode(query)), false);

        ActionResult ar;

        if (method == HttpMethod.Get) {
            ar = Api.AsGet().GetInvoke(obj, key, action, am);
        }
        else if (method == HttpMethod.Post) {
            ar = Api.AsPost().PostInvoke(obj, key, action, am);
        }
        else if (method == HttpMethod.Put) {
            ar = Api.AsPut().PutInvoke(obj, key, action, am);
        }
        else {
            throw new NotImplementedException();
        }

        return await GetResponse(ar);
    }
}