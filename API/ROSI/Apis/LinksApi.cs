using ROSI.Records;

namespace ROSI.Apis;

public static class LinksApi {
    public static Link GetLinkOfRel(this IEnumerable<Link> linkRepresentations, RelApi.Rels rel) =>
        linkRepresentations.Single(l => l.GetRel().GetRelType() == rel);

    public static Link GetInvokeLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.GetLinkOfRel(RelApi.Rels.invoke);

    public static Link GetDetailsLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.GetLinkOfRel(RelApi.Rels.details);

    public static Link GetModifyLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.GetLinkOfRel(RelApi.Rels.modify);

    public static Link GetSelfLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.GetLinkOfRel(RelApi.Rels.self);
}