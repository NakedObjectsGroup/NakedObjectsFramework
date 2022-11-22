using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;
using ROSI.Apis;

namespace ROSI.Helpers;

public static class HttpHelpers {
    private static readonly HttpClient Client = new();

    public static string? GlobalToken { get; set; } = null; 

    private static HttpRequestMessage CreateMessage(HttpMethod method, string path, string? token, HttpContent? content) {
        var request = new HttpRequestMessage(method, path);

        if ((token ?? GlobalToken) is not null) {
            request.Headers.Add("Authorization", (token ?? GlobalToken));
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

    public static string Execute(Uri url, string? token = null, string? jsonContent = null) {
        using var content = JsonContent.Create("", new MediaTypeHeaderValue("application/json"));
        var request = CreateMessage(HttpMethod.Get, url.ToString(), token, content);

        using var response = Client.SendAsync(request).Result;

        if (response.IsSuccessStatusCode) {
            return ReadAsString(response);
        }

        throw new HttpRequestException("request failed", null, response.StatusCode);
    }

    public static string Execute(JProperty action, string? token = null, string? jsonContent = null) {
        var invokeLink = action.GetLinks().GetInvokeLink();
        var uri = invokeLink.GetHref();
        var method = invokeLink.GetMethod();

        using var content = JsonContent.Create("", new MediaTypeHeaderValue("application/json"));
        var request = CreateMessage(method, uri.ToString(), token, content);

        using var response = Client.SendAsync(request).Result;

        if (response.IsSuccessStatusCode) {
            return ReadAsString(response);
        }

        throw new HttpRequestException("request failed", null, response.StatusCode);
    }
}