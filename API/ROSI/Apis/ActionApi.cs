using Newtonsoft.Json.Linq;
using ROSI.Helpers;

namespace ROSI.Apis;

public static class ActionApi {
    public static IEnumerable<JObject> GetLinks(this JProperty actionRepresentation) => actionRepresentation.Value["links"]!.ToList().Cast<JObject>();

    public static JObject Invoke(this JProperty actionRepresentation) {
        var json = HttpHelpers.Execute(actionRepresentation);
        return JObject.Parse(json);
    }
}