using Newtonsoft.Json.Linq;
using ROSI.Apis;
using ROSI.Exceptions;
using ROSI.Interfaces;
using ROSI.Records;
using Extensions = ROSI.Records.Extensions;

namespace ROSI.Helpers;

internal static class ApiHelpers {
    public static IEnumerable<Link> ToLinks(this IEnumerable<JToken> tokens) => tokens.OfType<JObject>().Select(jo => new Link(jo));

    public static async Task<JObject> GetResourceAsync(Link link, InvokeOptions options) {
        var href = link.GetHref();
        var json = await HttpHelpers.Execute(href, options);
        return JObject.Parse(json);
    }

    public static async Task<Prompt> GetPrompt(IHasLinks propertyRepresentation, InvokeOptions options, object[] pp) {
        var promptLink = propertyRepresentation.GetLinks().GetPromptLink() ?? throw new NoSuchPropertyRosiException($"Missing prompt link in: {propertyRepresentation.GetType()}");
        var json = await HttpHelpers.Execute(promptLink, options, pp);
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
}