using Newtonsoft.Json.Linq;
using ROSI.Interfaces;
using ROSI.Records;
using Extensions = ROSI.Records.Extensions;

namespace ROSI.Apis;

public static class PropertyApi {
    public static T GetValue<T>(this IProperty propertyRepresentation) => propertyRepresentation.Wrapped["value"].Value<T>();

    public static object GetValue(this IProperty propertyRepresentation) => ((JValue)propertyRepresentation.GetValue<object>()).Value;

    public static Link GetLinkValue(this IProperty propertyRepresentation) => new(propertyRepresentation.Wrapped["value"] as JObject);

    public static bool IsScalarProperty(this IProperty propertyRepresentation) {
        var rt = propertyRepresentation.GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.returnType);
        return rt is "boolean" or "number" or "string" or "integer";
    }
}