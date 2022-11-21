using Newtonsoft.Json.Linq;

namespace ROSI;

public static class API {
    public static object? GetObject(Uri uri) => null;

    public static T? GetPropertyValue<T>(this JObject objectRepresentation, string propertyName) {
        var property = objectRepresentation["members"]?[propertyName];

        if (property is null) {
            throw new Exception("No such property");
        }

        return property["value"]!.Value<T>();
    }
}