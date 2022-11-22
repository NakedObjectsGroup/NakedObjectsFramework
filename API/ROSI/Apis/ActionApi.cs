using Newtonsoft.Json.Linq;

namespace ROSI.Apis;

public static class ActionApi
{

    public static IEnumerable<JObject> GetLinks(this JProperty actionRepresentation) => actionRepresentation.Value["links"]!.ToList().Cast<JObject>();


}