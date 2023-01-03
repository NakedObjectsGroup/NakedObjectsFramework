using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class CollectionApi {
    public static IEnumerable<Link> GetValue(this ICollection collectionRepresentation) => collectionRepresentation.Wrapped[JsonConstants.Value].ToLinks();
}