using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;
using System;

namespace ROSI.Apis;

public static class ActionDetailsApi {
    public static Parameters GetParameters(this ActionDetails actionRepresentation) => new(actionRepresentation.GetMandatoryPropertyAsJObject(JsonConstants.Parameters));

    public static async Task<ActionResult> Invoke(this ActionDetails actionRepresentation, InvokeOptions options, params object[] pp) {
        var json = await HttpHelpers.Execute(actionRepresentation.GetLinks().GetInvokeLink(), options, pp);
        return new ActionResult(JObject.Parse(json));
    }
}