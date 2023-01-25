using Newtonsoft.Json.Linq;
using ROSI.Records;

namespace ROSI.Apis;

public static class ParametersApi {
    public static IDictionary<string, Parameter> Parameters(this Parameters parametersRepresentation) =>
        parametersRepresentation.Wrapped.Children().OfType<JProperty>().ToDictionary(p => p.Name, p => new Parameter((JObject)p.Value));
}