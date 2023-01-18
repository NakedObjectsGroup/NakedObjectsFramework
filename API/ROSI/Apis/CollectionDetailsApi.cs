using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class CollectionDetailsApi {
    public static IEnumerable<Link> GetValue(this CollectionDetails collectionRepresentation) => collectionRepresentation.GetMandatoryProperty(JsonConstants.Value).ToLinks();
}