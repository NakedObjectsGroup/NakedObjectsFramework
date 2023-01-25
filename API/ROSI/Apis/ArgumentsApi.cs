using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ArgumentsApi {
    public static Dictionary<string, Argument> GetArguments(this Arguments argumentsRepresentation) =>
        argumentsRepresentation.Wrapped.Children().OfType<JProperty>().Where(jp => !jp.Name.StartsWith("x-ro")).ToDictionary(jp => jp.Name, jp => new Argument((JObject)jp.Value));

    public static string? GetInvalidReason(this Arguments argumentsRepresentation) =>
        argumentsRepresentation.GetOptionalProperty(JsonConstants.XRoInvalidReason) is JValue v ? v.Value<string>() : null;
}