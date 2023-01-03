using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;
using System;

namespace ROSI.Apis;

public static class ActionMemberApi {
    public static bool HasInvokeLink(this ActionMember actionRepresentation) => actionRepresentation.GetLinks().HasInvokeLink();

    public static async Task<ActionDetails> GetDetails(this ActionMember actionRepresentation, InvokeOptions options) {
        var json = await HttpHelpers.GetDetails(actionRepresentation, options);
        return new ActionDetails(JObject.Parse(json));
    }

    public static async Task<ActionResult> Invoke(this ActionMember actionRepresentation, InvokeOptions options, params object[] pp) {
        if (actionRepresentation.HasInvokeLink()) {
            var json = await HttpHelpers.Execute(actionRepresentation.GetLinks().GetInvokeLink(), options, pp);
            return new ActionResult(JObject.Parse(json));
        }

        return await (await actionRepresentation.GetDetails(options)).Invoke(options, pp);
    }
}