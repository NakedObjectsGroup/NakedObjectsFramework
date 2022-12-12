using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;
using Action = ROSI.Records.Action;
using Extensions = ROSI.Records.Extensions;

namespace ROSI.Apis;

public static class ExtensionsApi {
    public static IDictionary<string, object> GetExtensions(this Extensions extensionsRepresentation) =>
        extensionsRepresentation.Wrapped.Children().Cast<JProperty>().ToDictionary(p => p.Name, p => ((JValue)p.Value).Value);

    public static T GetExtension<T>(this Extensions extensionsRepresentation, string key) => (T)extensionsRepresentation.GetExtensions()[key];

}