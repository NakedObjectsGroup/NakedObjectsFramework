using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class PropertyMemberApi {
    public static async Task<PropertyDetails> GetDetails(this PropertyMember propertyRepresentation, InvokeOptions? options = null) {
        var json = await HttpHelpers.GetDetails(propertyRepresentation, options ?? propertyRepresentation.Options);
        return new PropertyDetails(JObject.Parse(json), propertyRepresentation.Options);
    }

    private static bool HasChoicesFlag(this PropertyMember propertyRepresentation) => propertyRepresentation.GetOptionalProperty(JsonConstants.HasChoices) is not null;

    public static async Task<bool> GetHasChoices(this PropertyMember propertyRepresentation, InvokeOptions? options = null) {
        if (propertyRepresentation.HasChoicesFlag()) {
            return propertyRepresentation.GetMandatoryProperty(JsonConstants.HasChoices).Value<bool>();
        }

        return (await propertyRepresentation.GetDetails(options)).GetHasChoices();
    }

    public static async Task<IEnumerable<T?>> GetChoices<T>(this PropertyMember propertyRepresentation, InvokeOptions? options = null) {
        if (propertyRepresentation.HasChoicesFlag()) {
            return propertyRepresentation.GetMandatoryProperty(JsonConstants.Choices).Where(c => c is not JObject).Select(c => c.Value<T>());
        }

        return (await propertyRepresentation.GetDetails(options)).GetChoices<T>();
    }

    public static async Task<IEnumerable<Link>> GetLinkChoices(this PropertyMember propertyRepresentation, InvokeOptions? options = null) {
        if (propertyRepresentation.HasChoicesFlag()) {
            return propertyRepresentation.GetMandatoryProperty(JsonConstants.Choices).ToLinks(propertyRepresentation);
        }

        return (await propertyRepresentation.GetDetails(options)).GetLinkChoices();
    }

    public static async Task<IEnumerable<T?>> GetPrompts<T>(this PropertyMember propertyRepresentation, params object[] pp) => await propertyRepresentation.GetPrompts<T>(propertyRepresentation.Options, pp);

    public static async Task<IEnumerable<T?>> GetPrompts<T>(this PropertyMember propertyRepresentation, InvokeOptions options, params object[] pp) {
        if (propertyRepresentation.HasPromptLink()) {
            var prompt = await ApiHelpers.GetPrompt(propertyRepresentation, options, pp);
            return prompt.GetChoices<T>();
        }

        return await (await propertyRepresentation.GetDetails(options)).GetPrompts<T>(options, pp);
    }

    public static async Task<IEnumerable<Link>> GetLinkPrompts(this PropertyMember propertyRepresentation, params object[] pp) => await propertyRepresentation.GetLinkPrompts(propertyRepresentation.Options, pp);

    public static async Task<IEnumerable<Link>> GetLinkPrompts(this PropertyMember propertyRepresentation, InvokeOptions options, params object[] pp) {
        if (propertyRepresentation.HasPromptLink()) {
            var prompt = await ApiHelpers.GetPrompt(propertyRepresentation, options, pp);
            return prompt.GetLinkChoices();
        }

        return await (await propertyRepresentation.GetDetails(options)).GetLinkPrompts(options, pp);
    }
}