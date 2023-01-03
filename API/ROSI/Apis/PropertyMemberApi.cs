using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class PropertyMemberApi {
    public static async Task<PropertyDetails> GetDetails(this PropertyMember propertyRepresentation, InvokeOptions options) {
        var json = await HttpHelpers.GetDetails(propertyRepresentation, options);
        return new PropertyDetails(JObject.Parse(json));
    }

    private static bool HasChoicesFlag(this PropertyMember propertyRepresentation) => propertyRepresentation.Wrapped["hasChoices"] is not null;

    public static async Task<bool> GetHasChoices(this PropertyMember propertyRepresentation, InvokeOptions options) {
        if (propertyRepresentation.HasChoicesFlag()) {
            return propertyRepresentation.Wrapped["hasChoices"].Value<bool>();
        }

        return (await propertyRepresentation.GetDetails(options)).GetHasChoices();
    }

    public static async Task<IEnumerable<T>> GetChoices<T>(this PropertyMember propertyRepresentation, InvokeOptions options) {
        if (propertyRepresentation.HasChoicesFlag()) {
            return propertyRepresentation.Wrapped["choices"].Select(c => c.Value<T>());
        }

        return (await propertyRepresentation.GetDetails(options)).GetChoices<T>();
    }

    public static async Task<IEnumerable<Link>> GetLinkChoices(this PropertyMember propertyRepresentation, InvokeOptions options) {
        if (propertyRepresentation.HasChoicesFlag()) {
            return propertyRepresentation.Wrapped["choices"].ToLinks();
        }

        return (await propertyRepresentation.GetDetails(options)).GetLinkChoices();
    }

    public static async Task<IEnumerable<T>> GetPrompts<T>(this PropertyMember propertyRepresentation, InvokeOptions options, params object[] pp) {
        if (propertyRepresentation.HasPromptLink()) {
            var prompt = await ApiHelpers.GetPrompt(propertyRepresentation, options, pp);
            return prompt.GetChoices<T>();
        }

        return await (await propertyRepresentation.GetDetails(options)).GetPrompts<T>(options, pp);
    }

    public static async Task<IEnumerable<Link>> GetLinkPrompts(this PropertyMember propertyRepresentation, InvokeOptions options, params object[] pp) {
        if (propertyRepresentation.HasPromptLink()) {
            var prompt = await ApiHelpers.GetPrompt(propertyRepresentation, options, pp);
            return prompt.GetLinkChoices();
        }

        return await (await propertyRepresentation.GetDetails(options)).GetLinkPrompts(options, pp);
    }
}