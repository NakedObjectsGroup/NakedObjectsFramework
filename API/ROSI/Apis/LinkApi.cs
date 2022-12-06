using Newtonsoft.Json.Linq;
using ROSI.Records;

namespace ROSI.Apis;

public static class LinkApi {
    public static string GetLinkPropertyValue(this Link linkRepresentation, string pName) => linkRepresentation.GetLinkProperty(pName).ToString();

    private static JToken GetLinkProperty(this Link linkRepresentation, string pName) => linkRepresentation.Wrapped[pName];

    public static HttpMethod GetMethod(this Link linkRepresentation) => new(linkRepresentation.GetLinkPropertyValue("method"));

    public static JObject GetArguments(this Link linkRepresentation) => (JObject)linkRepresentation.GetLinkProperty("arguments");

    public static Uri GetHref(this Link linkRepresentation) => new(linkRepresentation.GetLinkPropertyValue("href"));

    public static string GetRel(this Link linkRepresentation) => linkRepresentation.GetLinkPropertyValue("rel");

    public static Link GetLinkOfRel(this IEnumerable<Link> linkRepresentations, RelApi.Rels rel) =>
        linkRepresentations.Single(l => l.GetRel().GetRelType() == rel);

    public static Link GetInvokeLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.GetLinkOfRel(RelApi.Rels.invoke);

    public static Link GetSelfLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.GetLinkOfRel(RelApi.Rels.self);
}