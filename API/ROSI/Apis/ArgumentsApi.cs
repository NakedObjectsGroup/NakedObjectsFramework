using Newtonsoft.Json.Linq;
using ROSI.Records;

namespace ROSI.Apis;

public static class ArgumentsApi {
    public static Dictionary<string, Argument> GetArguments(this Arguments errorRepresentation) =>
        errorRepresentation.Wrapped.Children().OfType<JProperty>().ToDictionary(jp => jp.Name, jp => new Argument((JObject)jp.Value));
}