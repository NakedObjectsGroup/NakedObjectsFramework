using ROSI.Records;

namespace ROSI.Apis;

public static class LinksApi {
    public static Link? GetLinkOfRel(this IEnumerable<Link> linkRepresentations, RelApi.Rels rel) =>
        linkRepresentations.SingleOrDefault(l => l.GetRel().GetRelType() == rel);

    public static bool HasLinkOfRel(this IEnumerable<Link> linkRepresentations, RelApi.Rels rel) =>
        linkRepresentations.GetLinkOfRel(rel) is not null;

    public static bool HasInvokeLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.HasLinkOfRel(RelApi.Rels.invoke);

    public static bool HasPromptLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.HasLinkOfRel(RelApi.Rels.prompt);

    public static Link? GetInvokeLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.GetLinkOfRel(RelApi.Rels.invoke);

    public static Link? GetDetailsLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.GetLinkOfRel(RelApi.Rels.details);

    public static Link? GetModifyLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.GetLinkOfRel(RelApi.Rels.modify);

    public static Link? GetSelfLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.GetLinkOfRel(RelApi.Rels.self);

    public static Link? GetPromptLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.GetLinkOfRel(RelApi.Rels.prompt);

    public static Link? GetPersistLink(this IEnumerable<Link> linkRepresentations) =>
        linkRepresentations.GetLinkOfRel(RelApi.Rels.persist);
}