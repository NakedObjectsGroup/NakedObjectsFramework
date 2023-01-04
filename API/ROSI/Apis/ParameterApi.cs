using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ParameterApi {
    public static T? GetDefault<T>(this Parameter parameterRepresentation) =>
        parameterRepresentation.GetOptionalProperty(JsonConstants.Default) switch {
            JObject => default,
            { } t => t.Value<T>(),
            _ => default
        };
    
    public static Link? GetLinkDefault(this Parameter parameterRepresentation) =>
        parameterRepresentation.GetOptionalProperty(JsonConstants.Default) is JObject jo ? new Link(jo) : null;
}