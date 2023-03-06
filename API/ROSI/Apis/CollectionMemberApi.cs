using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class CollectionMemberApi {
    private static bool HasValue(this CollectionMember collectionRepresentation) => collectionRepresentation.GetOptionalProperty(JsonConstants.Value) is not null;

    public static async Task<CollectionDetails> GetDetails(this CollectionMember collectionRepresentation, IInvokeOptions? options = null) {
        var json = await HttpHelpers.GetDetails(collectionRepresentation, options ?? collectionRepresentation.Options);
        return new CollectionDetails(JObject.Parse(json), collectionRepresentation.Options);
    }

    public static int? GetSize(this CollectionMember collectionRepresentation) => collectionRepresentation.GetOptionalProperty(JsonConstants.Size)?.Value<int>();

    public static async Task<IEnumerable<Link>> GetValue(this CollectionMember collectionRepresentation, IInvokeOptions? options = null) {
        if (collectionRepresentation.HasValue()) {
            return collectionRepresentation.GetMandatoryProperty(JsonConstants.Value).ToLinks(collectionRepresentation);
        }

        return (await collectionRepresentation.GetDetails(options)).GetValue();
    }
}