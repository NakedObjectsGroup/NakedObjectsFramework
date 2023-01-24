using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ROSI.Apis;
using ROSI.Exceptions;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Helpers;

internal static class HttpHelpers {
    public static async Task<string> Execute(Uri url, InvokeOptions options) => (await ExecuteWithTag(url, options)).Response;

    public static async Task<(string Response, EntityTagHeaderValue? Tag)> ExecuteWithTag(Uri url, InvokeOptions options) {
        var request = CreateMessage(HttpMethod.Get, url.ToString(), options);
        return await SendRequestAndRead(request, options);
    }

    public static async Task<string> GetDetails(IHasLinks hasLinks, InvokeOptions options) {
        var detailsLink = hasLinks.GetLinks().GetDetailsLink() ?? throw new NoSuchPropertyRosiException($"Missing details link in: {hasLinks.GetType()}");
        var (uri, method) = detailsLink.GetUriAndMethod();
        var request = CreateMessage(method, uri.ToString(), options);
        return (await SendRequestAndRead(request, options)).Response;
    }

    public static async Task<string> SetValue(IHasLinks hasLinks, object newValue, InvokeOptions options) {
        var modifyLink = hasLinks.GetLinks().GetModifyLink() ?? throw new NoSuchPropertyRosiException($"Missing modify link in: {hasLinks.GetType()}");
        var (uri, method) = modifyLink.GetUriAndMethod();

        var parameters = new JObject();

        var av = GetActualValue(newValue);
        parameters.Add(new JProperty(JsonConstants.Value, av));

        var parameterString = parameters.ToString(Formatting.None);

        using var content = new StringContent(parameterString, Encoding.UTF8, "application/json");
        var request = CreateMessage(method, uri.ToString(), options, content);

        return (await SendRequestAndRead(request, options)).Response;
    }

    public static async Task<string> Execute(Link invokeLink, InvokeOptions options, params object[] pp) =>
        pp.Any()
            ? UseSimpleArguments(invokeLink, pp)
                ? await ExecuteWithSimpleArguments(invokeLink, options, pp)
                : await ExecuteWithFormalArguments(invokeLink, options, pp)
            : await ExecuteWithNoArguments(invokeLink, options);

    private static bool UseSimpleArguments(Link invokeLink, object[] pp) =>
        invokeLink.GetMethod() == HttpMethod.Get &&
        !pp.OfType<Link>().Any() &&
        !pp.OfType<DomainObject>().Any() &&
        invokeLink.GetRel().GetRelType() != RelApi.Rels.prompt;

    private static async Task<string> ExecuteWithSimpleArguments(Link invokeLink, InvokeOptions options, params object[] pp) {
        var uri = invokeLink.GetHref();
        var properties = invokeLink.GetArgumentsAsJObject()?.Properties() ?? new List<JProperty>();
        var parameters = new Dictionary<string, string?>();

        var argValues = properties.Zip(pp);

        foreach (var (p, v) in argValues) {
            parameters[p.Name] = v.ToString();
        }

        var url = QueryHelpers.AddQueryString(uri.ToString(), parameters);

        return await SendAndRead(HttpMethod.Get, url, options);
    }

    private static async Task<string> ExecuteWithNoArguments(Link invokeLink, InvokeOptions options) {
        var (uri, method) = invokeLink.GetUriAndMethod();

        using var content = method != HttpMethod.Get ? JsonContent.Create("", new MediaTypeHeaderValue("application/json")) : null;
        var request = CreateMessage(method, uri.ToString(), options, content);

        return (await SendRequestAndRead(request, options)).Response;
    }

    private static async Task<string> ExecuteWithFormalArguments(Link invokeLink, InvokeOptions options, object[] pp) {
        var method = invokeLink.GetMethod();
        var uri = invokeLink.GetHref();

        var properties = invokeLink.GetArgumentsAsJObject()?.Properties() ?? new List<JProperty>();
        var parameters = new JObject();

        var argValues = properties.Zip(pp);

        foreach (var (p, v) in argValues) {
            var av = GetActualValue(v);
            parameters.Add(new JProperty(p.Name, new JObject(new JProperty(JsonConstants.Value, av))));
        }

        var parameterString = parameters.ToString(Formatting.None);
        var url = uri.ToString();

        if (method == HttpMethod.Get || method == HttpMethod.Delete) {
            url = $"{url}?{HttpUtility.UrlEncode(parameterString)}";
        }

        using var content = method == HttpMethod.Post || method == HttpMethod.Put ? new StringContent(parameterString, Encoding.UTF8, "application/json") : null;

        return await SendAndRead(method, url, options, content);
    }

    private static async Task<string> SendAndRead(HttpMethod method, string url, InvokeOptions options, StringContent? content = null) {
        var request = CreateMessage(method, url, options, content);
        return (await SendRequestAndRead(request, options)).Response;
    }

    private static Exception MapToException(HttpResponseMessage response) =>
        response.StatusCode switch {
            HttpStatusCode.BadRequest => new HttpInvalidArgumentsRosiException(response.StatusCode, ReadAsString(response), response.Headers.Warning.ToString()),
            HttpStatusCode.Unauthorized => new HttpRosiException(response.StatusCode, response.Headers.Warning.ToString()),
            HttpStatusCode.Forbidden => new HttpRosiException(response.StatusCode, response.Headers.Warning.ToString()),
            HttpStatusCode.NotFound => new HttpRosiException(response.StatusCode, response.Headers.Warning.ToString()),
            HttpStatusCode.UnprocessableEntity => new HttpInvalidArgumentsRosiException(response.StatusCode, ReadAsString(response), response.Headers.Warning.ToString()),
            HttpStatusCode.InternalServerError => new HttpErrorRosiException(response.StatusCode, ReadAsString(response), response.Headers.Warning.ToString()),
            _ => new HttpRosiException(response.StatusCode, response.Headers.Warning.ToString())
        };

    private static async Task<(string Response, EntityTagHeaderValue? Tag)> SendRequestAndRead(HttpRequestMessage request, InvokeOptions options) {
        using var response = await options.HttpClient.SendAsync(request);
        var tag = response.Headers.ETag;

        if (response.IsSuccessStatusCode) {
            return (ReadAsString(response), tag);
        }

        throw MapToException(response);
    }

    private static JObject GetHrefValue(Link l) => new(new JProperty("href", l.GetHref()));

    private static object GetActualValue(object val) => val switch {
        Link l => GetHrefValue(l),
        DomainObject o => GetHrefValue(o.GetLinks().GetSelfLink() ?? throw new NoSuchPropertyRosiException($"Missing self link in: {o.GetType()}")),
        _ => val
    };

    private static string ReadAsString(HttpResponseMessage response) {
        using var sr = new StreamReader(response.Content.ReadAsStream());
        var json = sr.ReadToEnd();
        return json;
    }

    private static (Uri, HttpMethod) GetUriAndMethod(this Link linkRepresentation) => (linkRepresentation.GetHref(), linkRepresentation.GetMethod());

    private static HttpRequestMessage CreateMessage(HttpMethod method, string path, InvokeOptions options, HttpContent? content = null) {
        var request = new HttpRequestMessage(method, path);

        if (options.Token is not null) {
            request.Headers.Add("Authorization", options.Token);
        }

        if (options.Tag is not null) {
            request.Headers.IfMatch.Add(options.Tag);
        }

        if (content is not null) {
            request.Content = content;
        }

        return request;
    }
}