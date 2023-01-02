using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ROSI.Apis;
using ROSI.Interfaces;
using ROSI.Records;
using Action = ROSI.Records.ActionMember;

namespace ROSI.Helpers;

public static class HttpHelpers {
    public static HttpClient Client { private get; set; } = new();

    public static string GlobalToken { get; set; } = null;

    private static HttpRequestMessage CreateMessage(HttpMethod method, string path, InvokeOptions options, HttpContent content) {
        var request = new HttpRequestMessage(method, path);

        if ((options.Token ?? GlobalToken) is not null) {
            request.Headers.Add("Authorization", options.Token ?? GlobalToken);
        }

        if (options.Tag is not null) {
            request.Headers.IfMatch.Add(options.Tag);
        }

        if (content is not null) {
            request.Content = content;
        }

        return request;
    }

    private static string ReadAsString(HttpResponseMessage response) {
        using var sr = new StreamReader(response.Content.ReadAsStream());
        var json = sr.ReadToEnd();
        return json;
    }

    private static (Uri, HttpMethod) GetUriAndMethod(this Link linkRepresentation) => (linkRepresentation.GetHref(), linkRepresentation.GetMethod());

    public static async Task<string> Execute(Uri url, InvokeOptions options, string jsonContent = null) => (await ExecuteWithTag(url, options, jsonContent)).Item1;

    public static async Task<(string, EntityTagHeaderValue)> ExecuteWithTag(Uri url, InvokeOptions options, string jsonContent = null) {
        using var content = JsonContent.Create("", new MediaTypeHeaderValue("application/json"));
        var request = CreateMessage(HttpMethod.Get, url.ToString(), options, content);

        using var response = await Client.SendAsync(request);
        var tag = response.Headers.ETag;

        if (response.IsSuccessStatusCode) {
            return (ReadAsString(response), tag);
        }

        throw new HttpRequestException("request failed", null, response.StatusCode);
    }

    public static async Task<string> GetDetails(IHasLinks hasLinks, InvokeOptions options) {
        var (uri, method) = hasLinks.GetLinks().GetDetailsLink().GetUriAndMethod();

        using var content = JsonContent.Create("", new MediaTypeHeaderValue("application/json"));
        var request = CreateMessage(method, uri.ToString(), options, content);

        using var response = await Client.SendAsync(request);

        if (response.IsSuccessStatusCode) {
            return ReadAsString(response);
        }

        throw new HttpRequestException("request failed", null, response.StatusCode);
    }

    public static async Task<string> SetValue(IHasLinks hasLinks, object newValue, InvokeOptions options) {
        var modifyLink = hasLinks.GetLinks().GetModifyLink();
        var (uri, method) = modifyLink.GetUriAndMethod();

        var parameters = new JObject();

        var av = GetActualValue(newValue);
        parameters.Add(new JProperty("value", av));

        var parameterString = parameters.ToString(Formatting.None);

        using var content = new StringContent(parameterString, Encoding.UTF8, "application/json");
        var request = CreateMessage(method, uri.ToString(), options, content);

        using var response = await Client.SendAsync(request);

        if (response.IsSuccessStatusCode) {
            return ReadAsString(response);
        }

        throw new HttpRequestException("request failed", null, response.StatusCode);
    }

    public static async Task<string> Execute(IHasLinks action, InvokeOptions options, string jsonContent = null) {
        var (uri, method) = action.GetLinks().GetInvokeLink().GetUriAndMethod();

        using var content = JsonContent.Create("", new MediaTypeHeaderValue("application/json"));
        var request = CreateMessage(method, uri.ToString(), options, content);

        using var response = await Client.SendAsync(request);

        if (response.IsSuccessStatusCode) {
            return ReadAsString(response);
        }

        throw new HttpRequestException("request failed", null, response.StatusCode);
    }

    private static JObject GetHrefValue(Link l) => new(new JProperty("href", l.GetHref()));

    private static object GetActualValue(object val) => val switch {
        Link l => GetHrefValue(l),
        DomainObject o => GetHrefValue(o.GetLinks().GetSelfLink()),
        _ => val
    };

    public static async Task<string> ExecuteWithSimpleArguments(Link invokeLink, InvokeOptions options, params object[] pp) {
        var uri = invokeLink.GetHref();
        var properties = invokeLink.GetArguments().Properties();
        var parameters = new Dictionary<string, string>();

        var argValues = properties.Zip(pp);

        foreach (var (p, v) in argValues) {
            parameters[p.Name] = v.ToString();
        }

        var url = QueryHelpers.AddQueryString(uri.ToString(), parameters);

        return await SendAndRead(options, HttpMethod.Get, url);
    }

    public static async Task<string> Execute(IHasLinks action, InvokeOptions options, params object[] pp) {
        var invokeLink = action.GetLinks().GetInvokeLink();
        var method = invokeLink.GetMethod();

        if (method == HttpMethod.Get && !pp.OfType<Link>().Any() && !pp.OfType<DomainObject>().Any()) {
            return await ExecuteWithSimpleArguments(invokeLink, options, pp);
        }

        return await ExecuteWithFormalArguments(invokeLink, options, pp, method);
    }

    private static async Task<string> ExecuteWithFormalArguments(Link invokeLink, InvokeOptions options, object[] pp, HttpMethod method) {
        var uri = invokeLink.GetHref();

        var properties = invokeLink.GetArguments().Properties();
        var parameters = new JObject();

        var argValues = properties.Zip(pp);

        foreach (var (p, v) in argValues) {
            var av = GetActualValue(v);
            parameters.Add(new JProperty(p.Name, new JObject(new JProperty("value", av))));
        }

        var parameterString = parameters.ToString(Formatting.None);
        var url = uri.ToString();

        if (method == HttpMethod.Get || method == HttpMethod.Delete) {
            url = $"{url}?{HttpUtility.UrlEncode(parameterString)}";
        }

        using var content = method == HttpMethod.Post || method == HttpMethod.Put ? new StringContent(parameterString, Encoding.UTF8, "application/json") : null;

        return await SendAndRead(options, method, url, content);
    }

    private static async Task<string> SendAndRead(InvokeOptions options, HttpMethod method, string url, StringContent content = null) {
        var request = CreateMessage(method, url, options, content);

        using var response = await Client.SendAsync(request);

        if (response.IsSuccessStatusCode) {
            return ReadAsString(response);
        }

        var error = ReadAsString(response);

        throw new HttpRequestException("request failed", null, response.StatusCode);
    }
}