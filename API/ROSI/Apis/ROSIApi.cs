using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class ROSIApi {
    public static async Task<Home> GetHome(Uri uri, InvokeOptions options) {
        var json = (await HttpHelpers.Execute(uri, options)).Response;
        return new Home(JObject.Parse(json), options);
    }

    public static async Task<DomainObject> GetObject(Uri uri, InvokeOptions options) {
        var (json, tag) = await HttpHelpers.Execute(uri, options);
        return new DomainObject(JObject.Parse(json), options, tag);
    }

    public static async Task<DomainObject> GetObject(Uri baseUri, string type, string id, InvokeOptions options) {
        var uri = new Uri(baseUri, $"/objects/{type}/{id}");
        return await GetObject(uri, options);
    }

    public static async Task<DomainObject> GetService(Uri baseUri, string name, InvokeOptions options) {
        var uri = new Uri(baseUri, $"/services/{name}");
        return await GetObject(uri, options);
    }

    public static async Task<DomainObject> GetMenu(Uri baseUri, string name, InvokeOptions options) {
        var uri = new Uri(baseUri, $"/menus/{name}");
        return await GetObject(uri, options);
    }
}