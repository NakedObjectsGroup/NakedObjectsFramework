using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ArgumentsApi {
    public static Dictionary<string, Argument> GetArguments(this Arguments argumentsRepresentation) {
        if (argumentsRepresentation.GetDomainType() is null) {
            return argumentsRepresentation.Wrapped.Children().OfType<JProperty>().Where(jp => !jp.Name.StartsWith("x-ro")).ToDictionary(jp => jp.Name, jp => new Argument((JObject)jp.Value, argumentsRepresentation.Options));
        }

        return new Dictionary<string, Argument>();
    }

    public static Dictionary<string, Argument> GetMembers(this Arguments argumentsRepresentation) {
        if (argumentsRepresentation.GetOptionalProperty(JsonConstants.Members) is JObject jo) {
            return jo.Children().OfType<JProperty>().ToDictionary(jp => jp.Name, jp => new Argument((JObject)jp.Value, argumentsRepresentation.Options));
        }

        return new Dictionary<string, Argument>();
    }

    public static string? GetInvalidReason(this Arguments argumentsRepresentation) =>
        argumentsRepresentation.GetOptionalProperty(JsonConstants.XRoInvalidReason) is JValue v ? v.Value<string>() : null;

    public static string? GetDomainType(this Arguments argumentsRepresentation) =>
        argumentsRepresentation.GetOptionalProperty(JsonConstants.DomainType) is JValue v ? v.Value<string>() : null;
}