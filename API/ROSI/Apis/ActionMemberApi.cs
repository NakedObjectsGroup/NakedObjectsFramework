using Newtonsoft.Json.Linq;
using ROSI.Exceptions;
using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class ActionMemberApi {
    public static bool HasInvokeLink(this ActionMember actionRepresentation) => actionRepresentation.GetLinks().HasInvokeLink();

    public static async Task<ActionDetails> GetDetails(this ActionMember actionRepresentation, IInvokeOptions? options = null) {
        var json = await HttpHelpers.GetDetails(actionRepresentation, options ?? actionRepresentation.Options);
        return new ActionDetails(JObject.Parse(json), actionRepresentation.Options);
    }

    private static bool HasParameters(this ActionMember actionRepresentation) => actionRepresentation.GetOptionalProperty(JsonConstants.Parameters) is not null;

    public static async Task<Parameters> GetParameters(this ActionMember actionRepresentation, IInvokeOptions? options = null) {
        if (actionRepresentation.HasParameters()) {
            return new Parameters(actionRepresentation.GetMandatoryPropertyAsJObject(JsonConstants.Parameters), actionRepresentation.Options);
        }

        return (await actionRepresentation.GetDetails(options)).GetParameters();
    }

    public static async Task<ActionResult> Invoke(this ActionMember actionRepresentation, params object[] pp) => await actionRepresentation.Invoke(actionRepresentation.Options, pp);

    public static async Task<ActionResult> Invoke(this ActionMember actionRepresentation, IInvokeOptions options, params object[] pp) {
        if (actionRepresentation.HasInvokeLink()) {
            var (json, tag) = await HttpHelpers.Execute(actionRepresentation.GetLinks().GetInvokeLink()!, options, pp);
            return new ActionResult(JObject.Parse(json), actionRepresentation.Options, tag);
        }

        return await (await actionRepresentation.GetDetails(options)).Invoke(options, pp);
    }

    public static async Task Validate(this ActionMember actionRepresentation, params object[] pp) {
        await actionRepresentation.Validate(actionRepresentation.Options, pp);
    }

    public static async Task Validate(this ActionMember actionRepresentation, IInvokeOptions options, params object[] pp) {
        options.ReservedArguments["x-ro-validate-only"] = true;

        if (actionRepresentation.HasInvokeLink()) {
            var (json, _) = await HttpHelpers.Execute(actionRepresentation.GetLinks().GetInvokeLink()!, options, pp);
            if (string.IsNullOrEmpty(json)) {
                return;
            }

            throw new UnexpectedResultRosiException($"Expected 'No Content' from validate got: {json[..200]}...");
        }

        await (await actionRepresentation.GetDetails(options)).Validate(options, pp);
    }

    public static async Task<ActionResult> InvokeWithNamedParams(this ActionMember actionRepresentation, Dictionary<string, object> pp) =>
        await actionRepresentation.Invoke(pp.Cast<object>().ToArray());

    public static async Task ValidateWithNamedParams(this ActionMember actionRepresentation, Dictionary<string, object> pp) =>
        await actionRepresentation.Validate(pp.Cast<object>().ToArray());

    public static async Task<ActionResult> InvokeWithNamedParams(this ActionMember actionRepresentation, IInvokeOptions options, Dictionary<string, object> pp) =>
        await actionRepresentation.Invoke(options, pp.Cast<object>().ToArray());

    public static async Task ValidateWithNamedParams(this ActionMember actionRepresentation, IInvokeOptions options, Dictionary<string, object> pp) =>
        await actionRepresentation.Validate(options, pp.Cast<object>().ToArray());
}