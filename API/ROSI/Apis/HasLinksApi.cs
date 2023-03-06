using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class HasLinksApi {
    public static IEnumerable<Link> GetLinks(this IHasLinks hasLinks) => hasLinks.GetMandatoryProperty(JsonConstants.Links).ToLinks(hasLinks);
}