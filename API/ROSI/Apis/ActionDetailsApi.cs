using Newtonsoft.Json.Linq;
using ROSI.Exceptions;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ActionDetailsApi {
    public static Parameters GetParameters(this ActionDetails actionRepresentation) => new(actionRepresentation.GetMandatoryPropertyAsJObject(JsonConstants.Parameters));

    public static async Task<ActionResult> Invoke(this ActionDetails actionRepresentation, InvokeOptions options, params object[] pp) {
        var link = actionRepresentation.GetLinks().GetInvokeLink() ?? throw new NoSuchPropertyRosiException("Missing invoke link in action details");
        var json = await HttpHelpers.Execute(link, options, pp);
        return new ActionResult(JObject.Parse(json));
    }
}