using Newtonsoft.Json.Linq;

namespace ROSI.Apis;

public static class LinkApi
{
    public static string GetLinkPropertyValue(this JObject linkRepresentation, string pName) => ((JValue)linkRepresentation[pName]).Value.ToString();

    public static JObject GetLinkProperty(this JObject linkRepresentation, string pName) => (JObject)linkRepresentation[pName];

    public static HttpMethod GetMethod(this JObject linkRepresentation) => new(linkRepresentation.GetLinkPropertyValue("method"));

    public static JObject GetArguments(this JObject linkRepresentation) => linkRepresentation.GetLinkProperty("arguments");

    public static Uri GetHref(this JObject linkRepresentation) => new(linkRepresentation.GetLinkPropertyValue("href"));

    public static string GetRel(this JObject linkRepresentation) => linkRepresentation.GetLinkPropertyValue("rel");

    public static JObject GetLinkOfRel(this IEnumerable<JObject> linkRepresentations, RelApi.Rels rel) =>
        linkRepresentations.Single(l => l.GetRel().GetRelType() == rel);

    public static JObject GetInvokeLink(this IEnumerable<JObject> linkRepresentations) =>
        linkRepresentations.GetLinkOfRel(RelApi.Rels.invoke);
}