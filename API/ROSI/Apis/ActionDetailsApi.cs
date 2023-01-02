using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ActionDetailsApi {

    public static Parameters GetParameters(this ActionDetails actionRepresentation) => new(actionRepresentation.Wrapped["parameters"] as JObject);

    public static async Task<ActionResult> Invoke(this ActionDetails actionRepresentation, InvokeOptions? options = null) {
        var json = await HttpHelpers.Execute(actionRepresentation, options ?? new InvokeOptions());
        return new ActionResult(JObject.Parse(json));
    }

    public static async Task<ActionResult> Invoke(this ActionDetails actionRepresentation, InvokeOptions options, params object[] pp) {
        var json = await HttpHelpers.Execute(actionRepresentation, options, pp);
        return new ActionResult(JObject.Parse(json));
    }

    public static async Task<ActionResult> Invoke(this ActionDetails actionRepresentation, params object[] pp) => await actionRepresentation.Invoke(new InvokeOptions(), pp);
}