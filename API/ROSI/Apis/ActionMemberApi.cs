using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ActionMemberApi {
    public static async Task<ActionDetails> GetDetails(this ActionMember actionRepresentation, InvokeOptions? options = null) {
        var json = await HttpHelpers.GetDetails(actionRepresentation, options ?? new InvokeOptions());
        return new ActionDetails(JObject.Parse(json));
    }

    public static async Task<ActionResult> Invoke(this ActionMember actionRepresentation, InvokeOptions? options = null) {
        if (actionRepresentation.HasInvokeLink()) {
            var json = await HttpHelpers.Execute(actionRepresentation, options ?? new InvokeOptions());
            return new ActionResult(JObject.Parse(json));
        }

        return await (await actionRepresentation.GetDetails(options)).Invoke(options);
    }

    public static async Task<ActionResult> Invoke(this ActionMember actionRepresentation, InvokeOptions options, params object[] pp) {
        if (actionRepresentation.HasInvokeLink()) {
            var json = await HttpHelpers.Execute(actionRepresentation, options, pp);
            return new ActionResult(JObject.Parse(json));
        }

        return await (await actionRepresentation.GetDetails(options)).Invoke(options, pp);
    }

    public static async Task<ActionResult> Invoke(this ActionMember actionRepresentation, params object[] pp) => await actionRepresentation.Invoke(new InvokeOptions(), pp);
}