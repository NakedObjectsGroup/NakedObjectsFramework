using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class PropertyDetailsApi {
    public static async Task<PropertyDetails> SetValue(this PropertyDetails propertyRepresentation, object newValue, InvokeOptions options) {
        var json = await HttpHelpers.SetValue(propertyRepresentation, newValue, options);
        return new PropertyDetails(JObject.Parse(json));
    }

    public static bool GetHasChoices(this PropertyDetails propertyRepresentation) => propertyRepresentation.Wrapped["hasChoices"].Value<bool>();

    public static IEnumerable<T> GetChoices<T>(this PropertyDetails propertyRepresentation) => propertyRepresentation.Wrapped["choices"].Select(c => c.Value<T>());

    public static IEnumerable<Link> GetLinkChoices(this PropertyDetails propertyRepresentation) => propertyRepresentation.Wrapped["choices"].ToLinks();

    public static async Task<IEnumerable<T>> GetPrompts<T>(this PropertyDetails propertyRepresentation, InvokeOptions options, params object[] pp) {
        var prompt = await ApiHelpers.GetPrompt(propertyRepresentation, options, pp).ConfigureAwait(false);
        return prompt.GetChoices<T>();
    }

    public static async Task<IEnumerable<Link>> GetLinkPrompts(this PropertyDetails propertyRepresentation, InvokeOptions options, params object[] pp) {
        var prompt = await ApiHelpers.GetPrompt(propertyRepresentation, options, pp);
        return prompt.GetLinkChoices();
    }
}