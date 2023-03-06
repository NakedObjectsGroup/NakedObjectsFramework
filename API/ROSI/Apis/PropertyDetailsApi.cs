using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class PropertyDetailsApi {
    public static async Task<PropertyDetails> SetValue(this PropertyDetails propertyRepresentation, object newValue, InvokeOptions? options = null) {
        var json = await HttpHelpers.SetValue(propertyRepresentation, newValue, options);
        return new PropertyDetails(JObject.Parse(json), propertyRepresentation.Options);
    }

    public static bool GetHasChoices(this PropertyDetails propertyRepresentation) => propertyRepresentation.GetMandatoryProperty(JsonConstants.HasChoices).Value<bool>();

    public static async Task<IEnumerable<T?>> GetPrompts<T>(this PropertyDetails propertyRepresentation, params object[] pp) => await propertyRepresentation.GetPrompts<T>(propertyRepresentation.Options, pp);

    public static async Task<IEnumerable<T?>> GetPrompts<T>(this PropertyDetails propertyRepresentation, InvokeOptions options, params object[] pp) {
        var prompt = await ApiHelpers.GetPrompt(propertyRepresentation, options, pp);
        return prompt.GetChoices<T>();
    }

    public static async Task<IEnumerable<Link>> GetLinkPrompts(this PropertyDetails propertyRepresentation, params object[] pp) => await propertyRepresentation.GetLinkPrompts(propertyRepresentation.Options, pp);

    public static async Task<IEnumerable<Link>> GetLinkPrompts(this PropertyDetails propertyRepresentation, InvokeOptions options, params object[] pp) {
        var prompt = await ApiHelpers.GetPrompt(propertyRepresentation, options, pp);
        return prompt.GetLinkChoices();
    }
}