using Newtonsoft.Json.Linq;
using ROSI.Records;

namespace ROSI.Apis;

public static class LinkApi {
    public static string GetLinkPropertyValue(this Link linkRepresentation, string pName) => ((JValue)linkRepresentation.GetLinkProperty(pName)).Value.ToString();

    public static JToken GetLinkProperty(this Link linkRepresentation, string pName) => linkRepresentation.Wrapped[pName];

    public static HttpMethod GetMethod(this Link linkRepresentation) => new(linkRepresentation.GetLinkPropertyValue("method"));

    public static JObject GetArguments(this Link linkRepresentation) => linkRepresentation.GetLinkProperty("arguments") as JObject;

    public static Uri GetHref(this Link linkRepresentation) => new(linkRepresentation.GetLinkPropertyValue("href"));

    public static string GetRel(this Link linkRepresentation) => linkRepresentation.GetLinkPropertyValue("rel");

    public static Link GetLinkOfRoRel(this IEnumerable<Link> linkRepresentations, RelApi.Rels rel) =>
        linkRepresentations.Single(l => l.GetRel().GetRelType() == rel);

    public static Link GetLinkOfRel(this IEnumerable<Link> linkRepresentations, string rel) =>
        linkRepresentations.Single(l => l.GetRel() == rel);

    public static Link GetInvokeLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.GetLinkOfRoRel(RelApi.Rels.invoke);

    public static Link GetSelfLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.GetLinkOfRel("self");

}