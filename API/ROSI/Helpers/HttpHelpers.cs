using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ROSI.Helpers;

public static class HttpHelpers
{
    private static readonly HttpClient Client = new();

    private static HttpRequestMessage CreateMessage(HttpMethod method, string path, HttpContent? content = null)
    {
        var request = new HttpRequestMessage(method, path);

        if (content is not null)
        {
            request.Content = content;
        }

        return request;
    }

    private static string ReadAsString(HttpResponseMessage response)
    {
        using var sr = new StreamReader(response.Content.ReadAsStream());
        var json = sr.ReadToEnd();
        return json ?? "";
    }

    public static string Execute(string? jsonContent, string url)
    {
        using var content = JsonContent.Create("", new MediaTypeHeaderValue("application/json"));
        var request = CreateMessage(HttpMethod.Get, url, content);

        using var response = Client.SendAsync(request).Result;

        if (response.IsSuccessStatusCode)
        {
            return ReadAsString(response);
        }

        throw new HttpRequestException("request failed", null, response.StatusCode);
    }
}