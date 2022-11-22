using Newtonsoft.Json.Linq;

namespace ROSI.Apis;

public static class LinkApi
{
    public static string GetLinkProperty(this JObject linkRepresentation, string pName) => ((JValue)linkRepresentation[pName]).Value.ToString();

    public static HttpMethod GetMethod(this JObject linkRepresentation) => new(linkRepresentation.GetLinkProperty("method"));

    public static Uri GetHref(this JObject linkRepresentation) => new(linkRepresentation.GetLinkProperty("href"));

    public static string GetRel(this JObject linkRepresentation) => linkRepresentation.GetLinkProperty("rel");

    public static JObject GetLinkOfRel(this IEnumerable<JObject> linkRepresentations, RelApi.Rels rel) =>
        linkRepresentations.Single(l => l.GetRel().GetRelType() == rel);

    public static JObject GetInvokeLink(this IEnumerable<JObject> linkRepresentations) =>
        linkRepresentations.GetLinkOfRel(RelApi.Rels.invoke);
}