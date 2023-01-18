using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ROSIApi {
    public static async Task<Home> GetHome(Uri uri, InvokeOptions options) {
        var json = await HttpHelpers.Execute(uri, options);
        return new Home(JObject.Parse(json));
    }

    public static async Task<DomainObject> GetObject(Uri uri, InvokeOptions options) {
        var json = await HttpHelpers.Execute(uri, options);
        return new DomainObject(JObject.Parse(json));
    }

    public static async Task<(DomainObject, EntityTagHeaderValue? tag)> GetObjectWithTag(Uri uri, InvokeOptions options) {
        var (json, tag) = await HttpHelpers.ExecuteWithTag(uri, options);
        return (new DomainObject(JObject.Parse(json)), tag);
    }

    public static async Task<DomainObject> GetObject(Uri baseUri, string type, string id, InvokeOptions options) {
        var uri = new Uri(baseUri, $"/objects/{type}/{id}");
        return await GetObject(uri, options);
    }
}