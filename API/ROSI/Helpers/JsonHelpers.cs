using Newtonsoft.Json.Linq;
using ROSI.Records;

namespace ROSI.Helpers;

public static class JsonHelpers {
    public static IEnumerable<Link> ToLinks(this IEnumerable<JToken> tokens) => tokens.Select(t => new Link((JObject)t));

    public static IEnumerable<Link> GetLinks(this JObject jo) => jo["links"].ToLinks();

    public static IEnumerable<Link> GetLinks(this JProperty jp) => jp.Value["links"].ToLinks();
}