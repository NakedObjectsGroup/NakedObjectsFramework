using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using Extensions = ROSI.Records.Extensions;

namespace ROSI.Apis;

public static class ExtensionsApi {

    public enum ExtensionKeys {
        domainType,
        friendlyName,
        pluralName, 
        description, 
        isService,
        memberOrder, 
        returnType,
        optional, 
        format,
        maxlength, 
        pattern,
        hasParams,
        elementType,
        maxLength,

        x_ro_nof_interactionMode,
        x_ro_nof_finderAction,
        x_ro_nof_tableViewTitle,
        x_ro_nof_tableViewColumns,
        x_ro_nof_menuPath,
        x_ro_nof_choices
    }

    public static IDictionary<string, object> RawExtensions(this Extensions extensionsRepresentation) =>
        extensionsRepresentation.Wrapped.Children().Cast<JProperty>().ToDictionary(p => p.Name, p => ((JValue)p.Value).Value);

    private static object? ExtensionObject(this JProperty prop) =>
        prop.Value switch {
            JValue jv => jv.Value,
            JObject jo => jo.Children().Cast<JProperty>().ToDictionary(c => c.Name, c => ((JValue)c.Value).Value),
            JArray ja => ja.Select(t => t),
            _ => null
        };

    public static IDictionary<ExtensionKeys, object> Extensions(this Extensions extensionsRepresentation) =>
        extensionsRepresentation.Wrapped.Children().Cast<JProperty>().ToDictionary(p => Enum.Parse<ExtensionKeys>(p.Name.Replace('-', '_')), p => p.ExtensionObject());

    public static T GetExtension<T>(this Extensions extensionsRepresentation, string key) => (T)extensionsRepresentation.RawExtensions()[key];

    public static T GetExtension<T>(this Extensions extensionsRepresentation, ExtensionKeys key) => (T)extensionsRepresentation.Extensions()[key];
}