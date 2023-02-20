using Newtonsoft.Json.Linq;
using ROSI.Exceptions;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ActionDetailsApi {
    public static Parameters GetParameters(this ActionDetails actionRepresentation) => new(actionRepresentation.GetMandatoryPropertyAsJObject(JsonConstants.Parameters));

    public static async Task<ActionResult> Invoke(this ActionDetails actionRepresentation, InvokeOptions options, params object[] pp) {
        var link = actionRepresentation.GetLinks().GetInvokeLink() ?? throw new NoSuchPropertyRosiException("Missing invoke link in action details");
        var (json, tag) = await HttpHelpers.Execute(link, options, pp);
        return new ActionResult(JObject.Parse(json), tag);
    }

    public static async Task<ActionResult> InvokeWithNamedParams(this ActionDetails actionRepresentation, InvokeOptions options, Dictionary<string, object> pp) =>
        await actionRepresentation.Invoke(options, pp.Cast<object>().ToArray());

    public static async Task Validate(this ActionDetails actionRepresentation, InvokeOptions options, params object[] pp) {
        options.ReservedArguments ??= new Dictionary<string, object>();
        options.ReservedArguments["x-ro-validate-only"] = true;

        var link = actionRepresentation.GetLinks().GetInvokeLink() ?? throw new NoSuchPropertyRosiException("Missing invoke link in action details");
        var (json, _) = await HttpHelpers.Execute(link, options, pp);
        if (string.IsNullOrEmpty(json)) {
            return;
        }

        throw new UnexpectedResultRosiException($"Expected 'No Content' from validate got: {json[..200]}...");
    }

    public static async Task ValidateWithNamedParams(this ActionDetails actionRepresentation, InvokeOptions options, Dictionary<string, object> pp) =>
        await actionRepresentation.Validate(options, pp.Cast<object>().ToArray());
}