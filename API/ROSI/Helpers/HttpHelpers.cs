using System.Net;
using System.Net.Http.Headers;
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
    public static async Task<(string Response, EntityTagHeaderValue? Tag)> Execute(Uri url, IInvokeOptions options) => await ExecuteWithTag(url, options);

    public static async Task<(string Response, EntityTagHeaderValue? Tag)> ExecuteWithTag(Uri url, IInvokeOptions options) {
        var request = CreateMessage(HttpMethod.Get, url.ToString(), options);
        return await SendRequestAndRead(request, options);
    }

    public static async Task<string> GetDetails(IHasLinks hasLinks, IInvokeOptions options) {
        var detailsLink = hasLinks.GetLinks().GetDetailsLink() ?? throw new NoSuchPropertyRosiException($"Missing details link in: {hasLinks.GetType()}");
        var (uri, method) = detailsLink.GetUriAndMethod();
        var request = CreateMessage(method, uri.ToString(), options);
        return (await SendRequestAndRead(request, options)).Response;
    }

    public static async Task<string> SetValue(IHasLinks hasLinks, object newValue, IInvokeOptions? options = null) {
        var modifyLink = hasLinks.GetLinks().GetModifyLink() ?? throw new NoSuchPropertyRosiException($"Missing modify link in: {hasLinks.GetType()}");
        var (uri, method) = modifyLink.GetUriAndMethod();

        var parameters = new JObject();

        var av = GetActualValue(newValue);
        parameters.Add(new JProperty(JsonConstants.Value, av));

        var parameterString = parameters.ToString(Formatting.None);

        using var content = new StringContent(parameterString, Encoding.UTF8, "application/json");
        var request = CreateMessage(method, uri.ToString(), options ?? hasLinks.Options, content);

        return (await SendRequestAndRead(request, options ?? hasLinks.Options)).Response;
    }

    public static async Task<(string Response, EntityTagHeaderValue? Tag)> Execute(Link invokeLink, IInvokeOptions options, params object[] pp) =>
        pp.Any()
            ? UseSimpleArguments(invokeLink, pp)
                ? await ExecuteWithSimpleArguments(invokeLink, options, pp)
                : await ExecuteWithFormalArguments(invokeLink, options, pp)
            : await ExecuteWithNoArguments(invokeLink, options);

    public static async Task<(string Response, EntityTagHeaderValue? Tag)> Persist(Link invokeLink, IInvokeOptions options, params object[] pp) =>
        await ExecuteWithFormalArguments(invokeLink, options, pp);

    private static bool UseSimpleArguments(Link invokeLink, object[] pp) =>
        invokeLink.GetMethod() == HttpMethod.Get &&
        !pp.OfType<Link>().Any() &&
        !pp.OfType<DomainObject>().Any() &&
        invokeLink.GetRel().GetRelType() != RelApi.Rels.prompt;

    private static async Task<(string Response, EntityTagHeaderValue? Tag)> ExecuteWithSimpleArguments(Link invokeLink, IInvokeOptions options, params object[] pp) {
        var uri = invokeLink.GetHref();
        var properties = invokeLink.GetArgumentsAsJObject()?.Properties() ?? new List<JProperty>();
        var parameters = GetSimpleParameters(pp, properties);

        var url = QueryHelpers.AddQueryString(uri.ToString(), parameters);

        if (options.ReservedArguments is not null) {
            url = QueryHelpers.AddQueryString(url, options.ReservedArguments.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()));
        }

        return await SendAndRead(HttpMethod.Get, url, options);
    }

    private static Dictionary<string, string?> GetSimpleParameters(object[] pp, IEnumerable<JProperty> properties) {
        if (pp.FirstOrDefault() is KeyValuePair<string, object>) {
            return GetSimpleParameters(pp.Cast<KeyValuePair<string, object>>().ToArray(), properties);
        }

        var parameters = new Dictionary<string, string?>();

        var argValues = properties.Zip(pp);

        foreach (var (p, v) in argValues) {
            parameters[p.Name] = v.ToString();
        }

        return parameters;
    }

    private static Dictionary<string, string?> GetSimpleParameters(KeyValuePair<string, object>[] pp, IEnumerable<JProperty> properties) {
        var argumentNames = properties?.Select(a => a.Name) ?? Array.Empty<string>();
        var parameters = new Dictionary<string, string?>();

        foreach (var (p, v) in pp) {
            if (argumentNames.Any(a => a == p)) {
                parameters[p] = v.ToString();
            }
        }

        return parameters;
    }

    private static async Task<(string Response, EntityTagHeaderValue? Tag)> ExecuteWithNoArguments(Link invokeLink, IInvokeOptions options) {
        var (uri, method) = invokeLink.GetUriAndMethod();

        using var content = method != HttpMethod.Get ? new StringContent("", Encoding.UTF8, "application/json") : null;
        var url = options.ReservedArguments is not null ? QueryHelpers.AddQueryString(uri.ToString(), options.ReservedArguments.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString())) : uri.ToString();

        var request = CreateMessage(method, url, options, content);

        return await SendRequestAndRead(request, options);
    }

    private static async Task<(string Response, EntityTagHeaderValue? Tag)> ExecuteWithFormalArguments(Link invokeLink, IInvokeOptions options, object[] pp) {
        var rel = invokeLink.GetRel().GetRelType();
        var method = invokeLink.GetMethod();
        var uri = invokeLink.GetHref();

        var contentString = rel == RelApi.Rels.persist ? GetMemberString(invokeLink, pp, options) : GetParameterString(invokeLink, pp, options);

        var url = uri.ToString();

        if (method == HttpMethod.Get || method == HttpMethod.Delete) {
            url = $"{url}?{HttpUtility.UrlEncode(contentString)}";
        }

        using var content = method == HttpMethod.Post || method == HttpMethod.Put ? new StringContent(contentString, Encoding.UTF8, "application/json") : null;

        return await SendAndRead(method, url, options, content);
    }

    private static JObject GetParameters(object[] pp, IEnumerable<JProperty> properties) {
        var parameters = new JObject();
        var argValues = properties.Zip(pp);

        foreach (var (p, v) in argValues) {
            parameters.Add(new JProperty(p.Name, new JObject(new JProperty(JsonConstants.Value, GetActualValue(v)))));
        }

        return parameters;
    }

    private static void AddReservedArguments(JObject jo, IInvokeOptions options) {
        if (options.ReservedArguments is not null) {
            foreach (var (key, value) in options.ReservedArguments) {
                jo.Add(new JProperty(key, value));
            }
        }
    }

    private static string GetMemberString(Link invokeLink, KeyValuePair<string, object>[] pp, IInvokeOptions options) {
        var argumentNames = (invokeLink.GetArgumentsAsJObject()?["members"] as JObject)?.Properties().Select(a => a.Name) ?? Array.Empty<string>();
        var parameters = GetParameters(pp, argumentNames);
        return MapParametersToString(options, parameters);
    }

    private static string MapParametersToString(IInvokeOptions options, JObject parameters) {
        var members = new JObject { new JProperty("members", parameters) };
        AddReservedArguments(members, options);
        return members.ToString(Formatting.None);
    }

    private static string GetMemberString(Link invokeLink, object[] pp, IInvokeOptions options) {
        if (pp.FirstOrDefault() is KeyValuePair<string, object>) {
            return GetMemberString(invokeLink, pp.Cast<KeyValuePair<string, object>>().ToArray(), options);
        }

        var properties = (invokeLink.GetArgumentsAsJObject()?["members"] as JObject)?.Properties() ?? Array.Empty<JProperty>();
        var parameters = GetParameters(pp, properties);
        return MapParametersToString(options, parameters);
    }

    private static string GetParameterString(Link invokeLink, object[] pp, IInvokeOptions options) {
        if (pp.FirstOrDefault() is KeyValuePair<string, object>) {
            return GetParameterString(invokeLink, pp.Cast<KeyValuePair<string, object>>().ToArray(), options);
        }

        var properties = invokeLink.GetArgumentsAsJObject()?.Properties() ?? Array.Empty<JProperty>();
        var parameters = GetParameters(pp, properties);
        AddReservedArguments(parameters, options);
        return parameters.ToString(Formatting.None);
    }

    private static string GetParameterString(Link invokeLink, KeyValuePair<string, object>[] pp, IInvokeOptions options) {
        var argumentNames = invokeLink.GetArgumentsAsJObject()?.Properties().Select(a => a.Name) ?? Array.Empty<string>();
        var parameters = GetParameters(pp, argumentNames);
        AddReservedArguments(parameters, options);
        return parameters.ToString(Formatting.None);
    }

    private static JObject GetParameters(KeyValuePair<string, object>[] pp, IEnumerable<string> argumentNames) {
        var parameters = new JObject();

        foreach (var (p, v) in pp) {
            if (argumentNames.Any(a => a == p)) {
                parameters.Add(new JProperty(p, new JObject(new JProperty(JsonConstants.Value, GetActualValue(v)))));
            }
        }

        return parameters;
    }

    private static async Task<(string Response, EntityTagHeaderValue? Tag)> SendAndRead(HttpMethod method, string url, IInvokeOptions options, StringContent? content = null) {
        var request = CreateMessage(method, url, options, content);
        return await SendRequestAndRead(request, options);
    }

    private static Exception MapToException(HttpResponseMessage response, IInvokeOptions options) =>
        response.StatusCode switch {
            HttpStatusCode.BadRequest => new HttpInvalidArgumentsRosiException(response.StatusCode, ReadAsString(response), response.Headers.Warning.ToString(), options),
            HttpStatusCode.Unauthorized => new HttpRosiException(response.StatusCode, response.Headers.Warning.ToString()),
            HttpStatusCode.Forbidden => new HttpRosiException(response.StatusCode, response.Headers.Warning.ToString()),
            HttpStatusCode.NotFound => new HttpRosiException(response.StatusCode, response.Headers.Warning.ToString()),
            HttpStatusCode.MethodNotAllowed => new HttpRosiException(response.StatusCode, response.Headers.Warning.ToString()),
            HttpStatusCode.NotAcceptable => new HttpRosiException(response.StatusCode, response.Headers.Warning.ToString()),
            HttpStatusCode.PreconditionFailed => new HttpRosiException(response.StatusCode, response.Headers.Warning.ToString()),
            HttpStatusCode.UnprocessableEntity => new HttpInvalidArgumentsRosiException(response.StatusCode, ReadAsString(response), response.Headers.Warning.ToString(), options),
            HttpStatusCode.PreconditionRequired => new HttpRosiException(response.StatusCode, response.Headers.Warning.ToString()),
            HttpStatusCode.InternalServerError => new HttpErrorRosiException(response.StatusCode, ReadAsString(response), response.Headers.Warning.ToString(), options),
            _ => new HttpRosiException(response.StatusCode, response.Headers.Warning.ToString())
        };

    private static async Task<(string Response, EntityTagHeaderValue? Tag)> SendRequestAndRead(HttpRequestMessage request, IInvokeOptions options) {
        using var response = await options.HttpClient.SendAsync(request);
        var tag = response.Headers.ETag;

        return response.IsSuccessStatusCode ? (ReadAsString(response), tag) : throw MapToException(response, options);
    }

    private static JObject GetHrefValue(Link l) => new(new JProperty("href", l.GetHref()));

    private static object GetActualValue(object val) => val switch {
        Link l => GetHrefValue(l),
        DomainObject o => GetHrefValue(o.GetLinks().GetSelfLink() ?? throw new NoSuchPropertyRosiException($"Missing self link in: {o.GetType()}")),
        _ => val
    };

    private static string ReadAsString(HttpResponseMessage response) {
        if (response.StatusCode is not HttpStatusCode.NoContent) {
            using var sr = new StreamReader(response.Content.ReadAsStream());
            var json = sr.ReadToEnd();
            return json;
        }

        return "";
    }

    private static (Uri, HttpMethod) GetUriAndMethod(this Link linkRepresentation) => (linkRepresentation.GetHref(), linkRepresentation.GetMethod());

    private static HttpRequestMessage CreateMessage(HttpMethod method, string path, IInvokeOptions options, HttpContent? content = null) {
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