using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ROSIApi {
    public static async Task<Home> GetHome(Uri uri, InvokeOptions options = null) {
        var json = await HttpHelpers.Execute(uri, options ?? new InvokeOptions());
        return new Home(JObject.Parse(json));
    }

    public static async Task<DomainObject> GetObject(Uri uri, InvokeOptions options = null) {
        var json = await HttpHelpers.Execute(uri, options ?? new InvokeOptions());
        return new DomainObject(JObject.Parse(json));
    }

    public static async Task<(DomainObject, EntityTagHeaderValue tag)> GetObjectWithTag(Uri uri, InvokeOptions options = null) {
        var (json, tag) = await HttpHelpers.ExecuteWithTag(uri, options ?? new InvokeOptions());
        return (new DomainObject(JObject.Parse(json)), tag);
    }
}