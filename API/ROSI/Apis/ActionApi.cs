using Newtonsoft.Json.Linq;
using ROSI.Helpers;

namespace ROSI.Apis;

public static class ActionApi {
    public static IEnumerable<JObject> GetLinks(this JProperty actionRepresentation) => actionRepresentation.Value["links"]!.ToList().Cast<JObject>();

    public static JObject Invoke(this JProperty actionRepresentation, string? token = null) {
        var json = HttpHelpers.Execute(actionRepresentation, token);
        return JObject.Parse(json);
    }
}