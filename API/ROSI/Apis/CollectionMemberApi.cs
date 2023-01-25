using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class CollectionMemberApi {
    private static bool HasValue(this CollectionMember collectionRepresentation) => collectionRepresentation.GetOptionalProperty(JsonConstants.Value) is not null;

    public static async Task<CollectionDetails> GetDetails(this CollectionMember collectionRepresentation, InvokeOptions options) {
        var json = await HttpHelpers.GetDetails(collectionRepresentation, options);
        return new CollectionDetails(JObject.Parse(json));
    }

    public static int? GetSize(this CollectionMember collectionRepresentation) => collectionRepresentation.GetOptionalProperty(JsonConstants.Size)?.Value<int>();

    public static async Task<IEnumerable<Link>> GetValue(this CollectionMember collectionRepresentation, InvokeOptions options) {
        if (collectionRepresentation.HasValue()) {
            return collectionRepresentation.GetMandatoryProperty(JsonConstants.Value).ToLinks();
        }

        return (await collectionRepresentation.GetDetails(options)).GetValue();
    }
}