using Newtonsoft.Json.Linq;
using ROSI.Apis;
using ROSI.Records;

namespace ROSI.Helpers;

public static class ApiHelpers {
    public static IEnumerable<Link> ToLinks(this IEnumerable<JToken> tokens) => tokens.Select(t => new Link((JObject)t));

    public static IEnumerable<Link> GetLinks(this JObject jo) => jo["links"].ToLinks();

    public static IEnumerable<Link> GetLinks(this JProperty jp) => jp.Value["links"].ToLinks();

    public static async Task<JObject> GetResourceAsync(Link link) {
        var href = link.GetHref();
        var json = await HttpHelpers.Execute(href, new InvokeOptions());
        return JObject.Parse(json);
    }
}