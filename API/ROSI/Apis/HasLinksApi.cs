using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class HasLinksApi {
    public static IEnumerable<Link> GetLinks(this IHasLinks hasLinks) => hasLinks.Wrapped.GetLinks();

    public static Link GetSelfLink(this IHasLinks hasLinks) => hasLinks.GetLinks().GetLinkOfRel(RelApi.Rels.self);
}