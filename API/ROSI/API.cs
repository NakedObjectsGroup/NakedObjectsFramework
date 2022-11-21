using Newtonsoft.Json.Linq;

namespace ROSI;

public static class API {
    public static JObject GetObject(Uri uri) {
        var json = Helpers.Execute(null, uri.ToString());
        return JObject.Parse(json);
    }

    public static T? GetPropertyValue<T>(this JObject objectRepresentation, string propertyName) {
        var property = objectRepresentation["members"]?[propertyName];

        if (property is null) {
            throw new Exception("No such property");
        }

        return property["value"]!.Value<T>();
    }
}