using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class PropertyApi {
    public static T GetValue<T>(this IProperty propertyRepresentation) => propertyRepresentation.Wrapped[JsonConstants.Value].Value<T>();

    public static object GetValue(this IProperty propertyRepresentation) => ((JValue)propertyRepresentation.GetValue<object>()).Value;

    public static Link GetLinkValue(this IProperty propertyRepresentation) => new(propertyRepresentation.Wrapped[JsonConstants.Value] as JObject);

    public static bool IsScalarProperty(this IProperty propertyRepresentation) {
        var rt = propertyRepresentation.GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.returnType);
        return rt is "boolean" or "number" or "string" or "integer";
    }

    public static bool HasPromptLink(this IProperty propertyRepresentation) => propertyRepresentation.GetLinks().HasPromptLink();
}