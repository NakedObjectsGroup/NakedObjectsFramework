using Newtonsoft.Json.Linq;
using ROSI.Apis;
using ROSI.Interfaces;
using ROSI.Records;
using Extensions = ROSI.Records.Extensions;

namespace ROSI.Helpers;

internal static class ApiHelpers {
    public static IEnumerable<Link> ToLinks(this IEnumerable<JToken> tokens) => tokens.Select(t => new Link((JObject)t));

    public static IEnumerable<Link> GetLinks(this JObject jo) => jo["links"].ToLinks();

    public static async Task<JObject> GetResourceAsync(Link link, InvokeOptions options) {
        var href = link.GetHref();
        var json = await HttpHelpers.Execute(href, options);
        return JObject.Parse(json);
    }

    public static Extensions GetExtensions(this JObject jo) => new(jo["extensions"] as JObject);

    public static async Task<Prompt> GetPrompt(IHasLinks propertyRepresentation, InvokeOptions options, object[] pp) {
        var json = await HttpHelpers.Execute(propertyRepresentation.GetLinks().GetPromptLink(), options, pp);
        return new Prompt(JObject.Parse(json));
    }
}