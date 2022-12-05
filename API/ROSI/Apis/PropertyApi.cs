using Newtonsoft.Json.Linq;
using ROSI.Records;

namespace ROSI.Apis;

public static class PropertyApi {
    public static T GetValue<T>(this Property propertyRepresentation) {
        var valueObject = propertyRepresentation.Wrapped.Value as JObject;

        if (valueObject is null) {
            throw new Exception("No such property value");
        }

        return valueObject["value"]!.Value<T>();
    }
}