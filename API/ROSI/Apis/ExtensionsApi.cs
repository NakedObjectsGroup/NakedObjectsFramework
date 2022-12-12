using Newtonsoft.Json.Linq;
using Extensions = ROSI.Records.Extensions;

namespace ROSI.Apis;

public static class ExtensionsApi {
    public static IDictionary<string, object> Extensions(this Extensions extensionsRepresentation) =>
        extensionsRepresentation.Wrapped.Children().Cast<JProperty>().ToDictionary(p => p.Name, p => ((JValue)p.Value).Value);

    public static T GetExtension<T>(this Extensions extensionsRepresentation, string key) => (T)extensionsRepresentation.Extensions()[key];
}