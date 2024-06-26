﻿using System.Net;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Model;
using Newtonsoft.Json.Linq;

namespace NakedFramework.RATL.Helpers;

public class StubHttpMessageHandler : HttpMessageHandler {
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

    private async Task<HttpResponseMessage> SendAsyncService(HttpRequestMessage request, string[] segments) {
        var serviceId = WebUtility.UrlDecode(segments[2].TrimEnd('/'));

        if (segments.Length > 3) {
            switch (segments[3]) {
                case "actions/":
                    if (segments.Length > 5) {
                        return await SendAsyncAction(request, serviceId, "", segments[4].TrimEnd('/'), false);
                    }

                    return await SendAsyncActionDetails(request, serviceId, "", segments[4].TrimEnd('/'));
            }
        }
        else {
            var ar = Api.AsGet().GetService(serviceId);
            return await GetResponse(ar);
        }

        throw new NotImplementedException();
    }

    private async Task<HttpResponseMessage> SendAsyncMenu(HttpRequestMessage request, string[] segments) {
        var menuId = segments[2].TrimEnd('/');

        if (segments.Length > 3) {
            switch (segments[3]) {
                case "actions/":
                    if (segments.Length > 5) {
                        return await SendAsyncAction(request, menuId, "", segments[4].TrimEnd('/'), true);
                    }

                    return await SendAsyncActionDetails(request, menuId, "", segments[4].TrimEnd('/'));
            }
        }
        else {
            var ar = Api.AsGet().GetMenu(menuId);
            return await GetResponse(ar);
        }

        throw new NotImplementedException();
    }

    private async Task<HttpResponseMessage> SendAsyncProperty(HttpRequestMessage request, string obj, string key, string propertyId) {
        if (propertyId.EndsWith('/')) {
            var query = request.RequestUri.Query.TrimStart('?');

            var am = ModelBinderUtils.CreateArgumentMap(JObject.Parse(HttpUtility.UrlDecode(query)), true);
            var ar = Api.AsGet().GetPropertyPrompt(obj, key, propertyId.TrimEnd('/'), am);
            return await GetResponse(ar);
        }

        if (request.Method == HttpMethod.Get) {
            var ar = Api.AsGet().GetProperty(obj, key, propertyId);
            return await GetResponse(ar);
        }

        if (request.Method == HttpMethod.Put) {
            var body = await ReadBody(request);
            var arg = ModelBinderUtils.CreateSingleValueArgument(JObject.Parse(body), false);

            var ar = Api.AsPut().PutProperty(obj, key, propertyId, arg);
            return await GetResponse(ar);
        }

        throw new NotImplementedException();
    }

    private async Task<HttpResponseMessage> SendAsyncCollection(HttpRequestMessage request, string obj, string key, string propertyId) {
        if (request.Method == HttpMethod.Get) {
            var ar = Api.AsGet().GetCollection(obj, key, propertyId);
            return await GetResponse(ar);
        }
        //if (request.Method == HttpMethod.Put) {
        //    var body = await ReadBody(request);
        //    var arg = ModelBinderUtils.CreateSingleValueArgument(JObject.Parse(body), false);

        //    var ar = Api.AsPut().PutCollection(obj, key, propertyId, arg);
        //    return await GetResponse(ar);
        //}

        throw new NotImplementedException();
    }

    private async Task<HttpResponseMessage> SendAsyncActionDetails(HttpRequestMessage request, string obj, string key, string actionId) {
        if (request.Method == HttpMethod.Get) {
            var ar1 = string.IsNullOrEmpty(key) ? Api.AsGet().GetServiceAction(obj, actionId) : Api.AsGet().GetAction(obj, key, actionId);
            return await GetResponse(ar1);
        }
        //if (request.Method == HttpMethod.Put) {
        //    var body = await ReadBody(request);
        //    var arg = ModelBinderUtils.CreateSingleValueArgument(JObject.Parse(body), false);

        //    var ar = Api.AsPut().PutCollection(obj, key, propertyId, arg);
        //    return await GetResponse(ar);
        //}

        throw new NotImplementedException();
    }

    private async Task<HttpResponseMessage> SendAsyncPersist(HttpRequestMessage request, string obj) {
        var body = await ReadBody(request);

        var am = ModelBinderUtils.CreatePersistArgMap(string.IsNullOrEmpty(body) ? new JObject() : JObject.Parse(body), true);

        var ar = Api.AsPost().PostPersist(obj, am);

        return await GetResponse(ar);
    }

    private async Task<HttpResponseMessage> SendAsyncParameterPrompt(HttpRequestMessage request, string obj, string key, string action, string param) {
        var method = request.Method;
        var query = request.RequestUri.Query.TrimStart('?');

        var am = ModelBinderUtils.CreateSimpleArgumentMap(query) ?? ModelBinderUtils.CreateArgumentMap(JObject.Parse(HttpUtility.UrlDecode(query)), true);

        var ar = Api.AsGet().GetParameterPrompt(obj, key, action, param, am);

        return await GetResponse(ar);
    }

    private async Task<HttpResponseMessage> SendAsyncAction(HttpRequestMessage request, string obj, string key, string action, bool isMenu) {
        var method = request.Method;
        var query = request.RequestUri.Query.TrimStart('?');
        var body = "";

        if (method == HttpMethod.Post || method == HttpMethod.Put) {
            body = await ReadBody(request);
        }

        var am = string.IsNullOrEmpty(query)
            ? ModelBinderUtils.CreateArgumentMap(string.IsNullOrEmpty(body) ? new JObject() : JObject.Parse(body), true)
            : ModelBinderUtils.CreateSimpleArgumentMap(query) ?? ModelBinderUtils.CreateArgumentMap(JObject.Parse(HttpUtility.UrlDecode(query)), true);

        ActionResult ar;

        if (method == HttpMethod.Get) {
            ar = string.IsNullOrEmpty(key)
                ? isMenu
                    ? Api.AsGet().GetInvokeOnMenu(obj, action, am)
                    : Api.AsGet().GetInvokeOnService(obj, action, am)
                : Api.AsGet().GetInvoke(obj, key, action, am);
        }
        else if (method == HttpMethod.Post) {
            ar = string.IsNullOrEmpty(key)
                ? isMenu
                    ? Api.AsPost().PostInvokeOnMenu(obj, action, am)
                    : Api.AsPost().PostInvokeOnService(obj, action, am)
                : Api.AsPost().PostInvoke(obj, key, action, am);
        }
        else if (method == HttpMethod.Put) {
            ar = string.IsNullOrEmpty(key)
                ? isMenu
                    ? Api.AsPut().PutInvokeOnMenu(obj, action, am)
                    : Api.AsPut().PutInvokeOnService(obj, action, am)
                : Api.AsPut().PutInvoke(obj, key, action, am);
        }
        else {
            throw new NotImplementedException();
        }

        return await GetResponse(ar);
    }

    private async Task<HttpResponseMessage> SendAsyncDomainTypes(HttpRequestMessage request, string obj, string action) {
        var query = request.RequestUri.Query.TrimStart('?');
        var am = ModelBinderUtils.CreateSimpleArgumentMap(query) ?? ModelBinderUtils.CreateArgumentMap(JObject.Parse(HttpUtility.UrlDecode(query)), false);
        var ar = Api.AsGet().GetInvokeTypeActions(obj, action, am);

        return await GetResponse(ar);
    }

    private async Task<HttpResponseMessage> SendAsyncObject(HttpRequestMessage request, string[] segments) {
        var obj = segments[2].TrimEnd('/');

        if (segments.Length == 3) {
            return await SendAsyncPersist(request, obj);
        }

        var key = segments[3].TrimEnd('/');
        if (segments.Length > 4) {
            switch (segments[4]) {
                case "properties/":
                    return await SendAsyncProperty(request, obj, key, segments[5]);
                case "collections/":
                    return await SendAsyncCollection(request, obj, key, segments[5]);
                case "actions/":
                    if (segments.Length == 9) {
                        return await SendAsyncParameterPrompt(request, obj, key, segments[5].TrimEnd('/'), segments[7].TrimEnd('/'));
                    }

                    if (segments.Length > 6) {
                        return await SendAsyncAction(request, obj, key, segments[5].TrimEnd('/'), false);
                    }

                    return await SendAsyncActionDetails(request, obj, key, segments[5].TrimEnd('/'));
            }
        }
        else {
            var method = request.Method;

            if (method == HttpMethod.Get) {
                var ar = Api.AsGet().GetObject(obj, key);
                return await GetResponse(ar);
            }

            if (method == HttpMethod.Put) {
                var body = await ReadBody(request);
                var args = ModelBinderUtils.CreateArgumentMap(JObject.Parse(body), false);
                var ar = Api.AsPut().PutObject(obj, key, args);
                return await GetResponse(ar);
            }
        }

        throw new NotImplementedException();
    }

    private async Task<HttpResponseMessage> GetResponse(ActionResult ar) {
        var (json, sc, _) = await TestHelpers.ReadActionResult(ar, Api.ControllerContext.HttpContext);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var headers = Api.ControllerContext.HttpContext.Response.Headers;

        var response = new HttpResponseMessage((HttpStatusCode)sc);
        response.Content = content;

        if (headers.FirstOrDefault(kvp => kvp.Key == "Warning") is ({ } key, var value)) {
            response.Headers.Add(key, value.ToString());
        }

        if (headers.FirstOrDefault(kvp => kvp.Key == "ETag") is ({ } key1, var value1)) {
            response.Headers.Add(key1, value1.ToString());
        }

        return response;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        var headers = Api.ControllerContext.HttpContext.Request.Headers;

        if (request.Headers.FirstOrDefault(kvp => kvp.Key == "If-Match") is ({ } key, var value)) {
            headers.Add(key, value.First());
        }

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
                return await SendAsyncService(request, segments);
            case "menus":
                return await SendAsyncMenus();
            case "menus/":
                return await SendAsyncMenu(request, segments);
            case "version":
                return await SendAsyncVersion();
            case "objects/":
                return await SendAsyncObject(request, segments);
            case "domain-types/":
                return await SendAsyncDomainTypes(request, segments[2].TrimEnd('/'), segments[4].TrimEnd('/'));
        }

        throw new NotImplementedException();
    }

    private static async Task<string> ReadBody(HttpRequestMessage request) {
        await using var s = await request.Content.ReadAsStreamAsync(new CancellationToken());
        using var sr = new StreamReader(s);
        s.Position = 0L;
        var body = await sr.ReadToEndAsync();
        s.Position = 0L;
        return body;
    }
}