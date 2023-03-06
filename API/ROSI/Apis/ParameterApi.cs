using Newtonsoft.Json.Linq;
using ROSI.Exceptions;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ParameterApi {
    public static bool HasDefault(this Parameter parameterRepresentation) =>
        parameterRepresentation.GetOptionalProperty(JsonConstants.Default) is not null;

    public static object? GetDefault(this Parameter parameterRepresentation) =>
        parameterRepresentation.GetOptionalProperty(JsonConstants.Default) switch {
            JObject => default,
            JValue t => t.Value,
            _ => default
        };

    public static T? GetDefault<T>(this Parameter parameterRepresentation) =>
        parameterRepresentation.GetOptionalProperty(JsonConstants.Default) switch {
            JObject => default,
            { } t => t.Value<T>(),
            _ => default
        };

    public static Link? GetLinkDefault(this Parameter parameterRepresentation) =>
        parameterRepresentation.GetOptionalProperty(JsonConstants.Default) is JObject jo ? new Link(jo, parameterRepresentation.Options) : null;

    public static async Task<Prompt> GetPrompts(this Parameter parameterRepresentation, params object[] pp) => await parameterRepresentation.GetPrompts(parameterRepresentation.Options, pp);

    public static async Task<Prompt> GetPrompts(this Parameter parameterRepresentation, InvokeOptions options, params object[] pp) {
        var link = parameterRepresentation.GetLinks().GetPromptLink() ?? throw new NoSuchPropertyRosiException("Missing prompt link in parameter");
        var (json, tag) = await HttpHelpers.Execute(link, options, pp);
        return new Prompt(JObject.Parse(json), parameterRepresentation.Options);
    }
}