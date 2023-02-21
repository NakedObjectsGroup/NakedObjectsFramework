using Newtonsoft.Json.Linq;
using ROSI.Apis;
using ROSI.Exceptions;
using ROSI.Interfaces;
using ROSI.Records;
using System.Globalization;

namespace ROSI.Helpers;

public static class ApiHelpers {
    public static IEnumerable<Link> ToLinks(this IEnumerable<JToken> tokens) => tokens.OfType<JObject>().Select(jo => new Link(jo));

    public static async Task<JObject> GetResourceAsync(Link link, InvokeOptions options) {
        var href = link.GetHref();
        var json = (await HttpHelpers.Execute(href, options)).Response;
        return JObject.Parse(json);
    }

    public static async Task<Prompt> GetPrompt(IHasLinks propertyRepresentation, InvokeOptions options, object[] pp) {
        var promptLink = propertyRepresentation.GetLinks().GetPromptLink() ?? throw new NoSuchPropertyRosiException($"Missing prompt link in: {propertyRepresentation.GetType()}");
        var json = (await HttpHelpers.Execute(promptLink, options, pp)).Response;
        return new Prompt(JObject.Parse(json));
    }

    public static JToken GetMandatoryProperty(this IWrapped wrapped, string key) =>
        wrapped.Wrapped[key] ?? throw new NoSuchPropertyRosiException(wrapped, key);

    public static JObject GetMandatoryPropertyAsJObject(this IWrapped wrapped, string key) =>
        wrapped.GetMandatoryProperty(key) is JObject jo ? jo : throw new UnexpectedTypeRosiException(wrapped, key, typeof(JObject));

    public static JToken? GetOptionalProperty(this IWrapped wrapped, string key) => wrapped.Wrapped[key];

    public static JObject? GetOptionalPropertyAsJObject(this IWrapped wrapped, string key) =>
        wrapped.GetOptionalProperty(key) switch {
            JObject jo => jo,
            null => null,
            _ => throw new UnexpectedTypeRosiException(wrapped, key, typeof(JObject))
        };

    public static object? MapToObject(this JProperty prop) =>
        prop.Value switch {
            JValue jv => jv.Value,
            JObject jo => jo.Children().Cast<JProperty>().ToDictionary(c => c.Name, c => c.MapToObject()),
            JArray ja => ja.Children().Cast<JValue>().Select(t => t.Value).ToList(),
            _ => null
        };

    public static Type NumberTypeToMatch(this IHasExtensions owner) {
        var format = owner.GetExtensions().GetExtension<string>("format");
        return format switch {
            "decimal" => typeof(double),
            "int" => typeof(int),
            _ => throw new NotSupportedException($"Unrecognized format: {format}")
        };
    }

    public static Type StringTypeToMatch(this IHasExtensions owner) {
        var format = owner.GetExtensions().GetExtension<string>("format");
        return format switch {
            "string" => typeof(string),
            "date-time" => typeof(DateTime),
            "date" => typeof(DateTime),
            "time" => typeof(TimeSpan),
            "utc-millisec" => throw new NotImplementedException("utc-millisec"),
            { } when format.StartsWith("big-integer") => throw new NotImplementedException("big-integer"),
            { } when format.StartsWith("big-decimal") => throw new NotImplementedException("big-decimal"),
            "blob" => throw new NotImplementedException("blob"),
            "clob" => throw new NotImplementedException("clob"),
            _ => throw new NotSupportedException($"Unrecognized format: {format}")
        };
    }

    public static Type TypeToMatch(this IHasExtensions owner) {
        var returnType = owner.GetExtensions().GetExtension<string>("returnType");
        return returnType switch {
            "boolean" => typeof(bool),
            "number" => owner.NumberTypeToMatch(),
            "string" => owner.StringTypeToMatch(),
            _ => typeof(Link)
        };
    }
}