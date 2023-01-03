using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class PropertyMemberApi {
    public static async Task<PropertyDetails> GetDetails(this PropertyMember propertyRepresentation, InvokeOptions options) {
        var json = await HttpHelpers.GetDetails(propertyRepresentation, options);
        return new PropertyDetails(JObject.Parse(json));
    }

    public static async Task<IEnumerable<T>> GetPrompts<T>(this PropertyMember propertyRepresentation, InvokeOptions options, params object[] pp) {
        if (propertyRepresentation.HasPromptLink()) {
            var json = await HttpHelpers.Execute(propertyRepresentation.GetLinks().GetPromptLink(), options, pp);
            var prompt = new Prompt(JObject.Parse(json));
            return prompt.GetChoices<T>();
        }

        return await (await propertyRepresentation.GetDetails(options)).GetPrompts<T>(options, pp);
    }

    public static async Task<IEnumerable<Link>> GetLinkPrompts(this PropertyMember propertyRepresentation, InvokeOptions options, params object[] pp) {
        if (propertyRepresentation.HasPromptLink()) {
            var json = await HttpHelpers.Execute(propertyRepresentation.GetLinks().GetPromptLink(), options, pp);
            var prompt = new Prompt(JObject.Parse(json));
            return prompt.GetLinkChoices();
        }
        return await (await propertyRepresentation.GetDetails(options)).GetLinkPrompts(options, pp);
    }
}