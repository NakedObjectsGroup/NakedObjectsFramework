using System.Globalization;
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

    private static TimeSpan ToTimeSpan(this DateTime dt) => new(0, dt.Hour, dt.Minute, dt.Second);

    public static object? Convert(this IProperty fromProperty, Type toType) {
        var fromValue = fromProperty.GetValue();

        return fromValue switch {
            null => null,
            IConvertible c => c.ToType(toType, CultureInfo.InvariantCulture),
            _ => throw new NotSupportedException($"Unrecognized value: {fromValue.GetType()}")
        };
    }

    public static object? ConvertNumberFormat(this IProperty fromProperty) {
        var format = fromProperty.GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.format);
        return format switch {
            "decimal" => fromProperty.Convert(typeof(double)),
            "int" => fromProperty.Convert(typeof(int)),
            _ => throw new NotSupportedException($"Unrecognized format: {format}")
        };
    }

    public static object? ConvertStringFormat(this IProperty fromProperty) {
        var format = fromProperty.GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.format);
        return format switch {
            "string" => fromProperty.GetValue<string>(),
            "date-time" => fromProperty.GetValue<string>() is { } s ? DateTime.Parse(s, CultureInfo.InvariantCulture) : null,
            "date" => fromProperty.GetValue<string>() is { } s ? DateTime.ParseExact(s, "yyyy-M-dd", CultureInfo.InvariantCulture) : null,
            "time" => fromProperty.GetValue<string>() is { } s ? DateTime.ParseExact(s, "HH:mm:ss", CultureInfo.InvariantCulture).ToTimeSpan() : null,
            "utc-millisec" => throw new NotImplementedException("utc-millisec"),
            { } when format.StartsWith("big-integer") => throw new NotImplementedException("big-integer"),
            { } when format.StartsWith("big-decimal") => throw new NotImplementedException("big-decimal"),
            "blob" => throw new NotImplementedException("blob"),
            "clob" => throw new NotImplementedException("clob"),
            _ => throw new NotSupportedException($"Unrecognized format: {format}")
        };
    }

    public static object? ConvertValue(this IProperty fromProperty) {
        var returnType = fromProperty.GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.returnType);
        return returnType switch {
            "boolean" => fromProperty.Convert(typeof(bool)),
            "number" => fromProperty.ConvertNumberFormat(),
            "string" => fromProperty.ConvertStringFormat(),
            _ => throw new NotSupportedException($"Unrecognized type: {returnType}")
        };
    }
}