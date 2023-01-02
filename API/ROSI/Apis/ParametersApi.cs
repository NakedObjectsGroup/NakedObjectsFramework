using Newtonsoft.Json.Linq;
using ROSI.Records;

namespace ROSI.Apis; 

public static class ParametersApi {
    public static IDictionary<string, Parameter> Parameters(this Parameters extensionsRepresentation) =>
        extensionsRepresentation.Wrapped.Children().Cast<JProperty>().ToDictionary(p => p.Name, p =>  new Parameter(p.Value as JObject));
}