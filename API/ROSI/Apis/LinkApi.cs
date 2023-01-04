using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class LinkApi {
    internal static JObject? GetArgumentsAsJObject(this Link linkRepresentation) => linkRepresentation.GetOptionalPropertyAsJObject(JsonConstants.Arguments);

    public static Uri GetHref(this Link linkRepresentation) => new(linkRepresentation.GetMandatoryProperty(JsonConstants.Href).ToString());

    public static string GetRel(this Link linkRepresentation) => linkRepresentation.GetMandatoryProperty(JsonConstants.Rel).ToString();

    public static string GetType(this Link linkRepresentation) => linkRepresentation.GetMandatoryProperty(JsonConstants.Type).ToString();

    public static HttpMethod GetMethod(this Link linkRepresentation) => new(linkRepresentation.GetMandatoryProperty(JsonConstants.Method).ToString());

    public static string? GetTitle(this Link linkRepresentation) => linkRepresentation.GetOptionalProperty(JsonConstants.Title)?.ToString();

    public static IDictionary<string, object?>? GetArguments(this Link linkRepresentation) => linkRepresentation.GetArgumentsAsJObject()?.Children().Cast<JProperty>().ToDictionary(p => p.Name, p => p.MapToObject());
}