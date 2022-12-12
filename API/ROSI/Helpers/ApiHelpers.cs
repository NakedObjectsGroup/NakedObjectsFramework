using Newtonsoft.Json.Linq;
using ROSI.Apis;
using ROSI.Records;
using Extensions = ROSI.Records.Extensions;

namespace ROSI.Helpers;

public static class ApiHelpers {
    public static IEnumerable<Link> ToLinks(this IEnumerable<JToken> tokens) => tokens.Select(t => new Link((JObject)t));

    public static IEnumerable<Link> GetLinks(this JObject jo) => jo["links"].ToLinks();

    public static async Task<JObject> GetResourceAsync(Link link) {
        var href = link.GetHref();
        var json = await HttpHelpers.Execute(href, new InvokeOptions());
        return JObject.Parse(json);
    }

    public static Extensions GetExtensions(this JObject jo) => new(jo["extensions"] as JObject);
}