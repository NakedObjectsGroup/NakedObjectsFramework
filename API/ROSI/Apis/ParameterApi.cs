using Newtonsoft.Json.Linq;
using ROSI.Records;

namespace ROSI.Apis;

public static class ParameterApi {
    public static T GetDefault<T>(this Parameter parameterRepresentation) => parameterRepresentation.Wrapped["default"].Value<T>();

    public static Link GetLinkDefault(this Parameter parameterRepresentation) => new(parameterRepresentation.Wrapped["default"] as JObject);
}