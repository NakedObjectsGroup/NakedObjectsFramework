using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class PropertyMemberApi {
    public static async Task<PropertyDetails> GetDetails(this PropertyMember propertyRepresentation, InvokeOptions options ) {
        var json = await HttpHelpers.GetDetails(propertyRepresentation, options);
        return new PropertyDetails(JObject.Parse(json));
    }
}