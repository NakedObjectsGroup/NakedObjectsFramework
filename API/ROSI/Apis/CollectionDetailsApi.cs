using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class CollectionDetailsApi {
    public static IEnumerable<Link> GetValue(this CollectionDetails collectionRepresentation) => collectionRepresentation.GetMandatoryProperty(JsonConstants.Value).ToLinks();
}