using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class CollectionApi {
    public static IEnumerable<Link> GetValue(this Collection collectionRepresentation) => collectionRepresentation.Wrapped["value"].ToLinks();
}