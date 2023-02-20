using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ROSIApi {
    public static async Task<Home> GetHome(Uri uri, InvokeOptions options) {
        var json = (await HttpHelpers.Execute(uri, options)).Response;
        return new Home(JObject.Parse(json));
    }

    public static async Task<DomainObject> GetObject(Uri uri, InvokeOptions options) {
        var (json, tag) = await HttpHelpers.Execute(uri, options);
        return new DomainObject(JObject.Parse(json), tag);
    }

    public static async Task<DomainObject> GetObject(Uri baseUri, string type, string id, InvokeOptions options) {
        var uri = new Uri(baseUri, $"/objects/{type}/{id}");
        return await GetObject(uri, options);
    }

    public static async Task<DomainObject> GetService(Uri baseUri, string type, InvokeOptions options) {
        var uri = new Uri(baseUri, $"/services/{type}");
        return await GetObject(uri, options);
    }
}