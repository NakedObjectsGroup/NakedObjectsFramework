using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ROSI.Apis;
using ROSI.Records;
using Action = ROSI.Records.Action;

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
        return json ?? "";
    }

    public static string Execute(Uri url, InvokeOptions options, string jsonContent = null) => ExecuteWithTag(url, options, jsonContent).Item1;

    public static (string, EntityTagHeaderValue) ExecuteWithTag(Uri url, InvokeOptions options, string jsonContent = null) {
        using var content = JsonContent.Create("", new MediaTypeHeaderValue("application/json"));
        var request = CreateMessage(HttpMethod.Get, url.ToString(), options, content);

        using var response = Client.SendAsync(request).Result;
        var tag = response.Headers.ETag;

        if (response.IsSuccessStatusCode) {
            return (ReadAsString(response), tag);
        }

        throw new HttpRequestException("request failed", null, response.StatusCode);
    }

    public static string Execute(Action action, InvokeOptions options, string jsonContent = null) {
        var invokeLink = action.GetLinks().GetInvokeLink();
        var uri = invokeLink.GetHref();
        var method = invokeLink.GetMethod();

        using var content = JsonContent.Create("", new MediaTypeHeaderValue("application/json"));
        var request = CreateMessage(method, uri.ToString(), options, content);

        using var response = Client.SendAsync(request).Result;

        if (response.IsSuccessStatusCode) {
            return ReadAsString(response);
        }

        throw new HttpRequestException("request failed", null, response.StatusCode);
    }

    private static JObject GetHrefValue(Link l) => new JObject(new JProperty("href", l.GetHref()));

    private static object GetActualValue(object val) => val switch {
        Link l => GetHrefValue(l),
        DomainObject o => GetHrefValue(o.GetLinks().GetSelfLink()),
        _ => val,
    };

    public static string Execute(Action action, InvokeOptions options, params object[] pp) {
        var invokeLink = action.GetLinks().GetInvokeLink();
        var uri = invokeLink.GetHref();
        var method = invokeLink.GetMethod();
        var arguments = invokeLink.GetArguments();
        var properties = arguments.Properties();
        var parameters = new JObject();

        var argValues = properties.Zip(pp);

        foreach (var (p, v) in argValues) {
            var av = GetActualValue(v);
            parameters.Add(new JProperty(p.Name, new JObject(new JProperty("value", av))));
        }

        using var content = new StringContent(parameters.ToString(Formatting.None), Encoding.UTF8, "application/json");
        var request = CreateMessage(method, uri.ToString(), options, content);

        using var response = Client.SendAsync(request).Result;

        if (response.IsSuccessStatusCode) {
            return ReadAsString(response);
        }

        var error = ReadAsString(response);

        throw new HttpRequestException("request failed", null, response.StatusCode);
    }
}