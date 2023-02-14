using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ActionMemberApi {
    public static bool HasInvokeLink(this ActionMember actionRepresentation) => actionRepresentation.GetLinks().HasInvokeLink();

    public static async Task<ActionDetails> GetDetails(this ActionMember actionRepresentation, InvokeOptions options) {
        var json = await HttpHelpers.GetDetails(actionRepresentation, options);
        return new ActionDetails(JObject.Parse(json));
    }

    private static bool HasParameters(this ActionMember actionRepresentation) => actionRepresentation.GetOptionalProperty(JsonConstants.Parameters) is not null;

    public static async Task<Parameters> GetParameters(this ActionMember actionRepresentation, InvokeOptions options) {
        if (actionRepresentation.HasParameters()) {
            return new Parameters(actionRepresentation.GetMandatoryPropertyAsJObject(JsonConstants.Parameters));
        }

        return (await actionRepresentation.GetDetails(options)).GetParameters();
    }

    public static async Task<ActionResult> Invoke(this ActionMember actionRepresentation, InvokeOptions options, params object[] pp) {
        if (actionRepresentation.HasInvokeLink()) {
            var (json, tag) = await HttpHelpers.Execute(actionRepresentation.GetLinks().GetInvokeLink()!, options, pp);
            return new ActionResult(JObject.Parse(json), tag);
        }

        return await (await actionRepresentation.GetDetails(options)).Invoke(options, pp);
    }
}