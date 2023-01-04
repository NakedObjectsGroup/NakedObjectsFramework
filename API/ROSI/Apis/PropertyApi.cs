using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class PropertyApi {
    public static T? GetValue<T>(this IProperty propertyRepresentation) =>
        propertyRepresentation.GetOptionalProperty(JsonConstants.Value) switch {
            JObject => default,
            { } t => t.Value<T>(),
            _ => default
        };

    public static object? GetValue(this IProperty propertyRepresentation) =>
        propertyRepresentation.GetOptionalProperty(JsonConstants.Value) is JValue v ? v.Value : null;

    public static Link? GetLinkValue(this IProperty propertyRepresentation) =>
        propertyRepresentation.GetOptionalProperty(JsonConstants.Value) is JObject jo ? new Link(jo) : null;

    public static bool IsScalarProperty(this IProperty propertyRepresentation) {
        var rt = propertyRepresentation.GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.returnType);
        return rt is "boolean" or "number" or "string" or "integer";
    }

    public static bool HasPromptLink(this IProperty propertyRepresentation) => propertyRepresentation.GetLinks().HasPromptLink();
}