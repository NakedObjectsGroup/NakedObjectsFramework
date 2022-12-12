using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class PropertyDetailsApi {
    public static async Task<PropertyDetails> SetValue(this PropertyDetails propertyRepresentation, object newValue, InvokeOptions options = null) {
        var json = await HttpHelpers.SetValue(propertyRepresentation, newValue, options ?? new InvokeOptions());
        return new PropertyDetails(JObject.Parse(json));
    }
}