using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;
using Action = ROSI.Records.Action;

namespace ROSI.Apis;

public static class ActionApi {

    public static async Task<Action> GetDetails(this Action actionRepresentation, InvokeOptions? options = null) {
        var json = await HttpHelpers.GetDetails(actionRepresentation, options ?? new InvokeOptions());
        return new Action(JObject.Parse(json));
    }

    public static async Task<ActionResult> Invoke(this IAction actionRepresentation, InvokeOptions? options = null) {
        var json = await HttpHelpers.Execute(actionRepresentation, options ?? new InvokeOptions());
        return new ActionResult(JObject.Parse(json));
    }

    public static async Task<ActionResult> Invoke(this IAction actionRepresentation, InvokeOptions options, params object[] pp) {
        var json = await HttpHelpers.Execute(actionRepresentation, options, pp);
        return new ActionResult(JObject.Parse(json));
    }

    public static async Task<ActionResult> Invoke(this IAction actionRepresentation, params object[] pp) => await actionRepresentation.Invoke(new InvokeOptions(), pp);
}