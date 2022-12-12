using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class CollectionMemberApi {
    public static async Task<CollectionDetails> GetDetails(this CollectionMember collectionRepresentation, InvokeOptions options = null) {
        var json = await HttpHelpers.GetDetails(collectionRepresentation, options ?? new InvokeOptions());
        return new CollectionDetails(JObject.Parse(json));
    }

    public static int GetSize(this CollectionMember collectionRepresentation) => collectionRepresentation.Wrapped["size"].Value<int>();
}