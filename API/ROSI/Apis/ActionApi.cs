using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;
using Action = ROSI.Records.Action;

namespace ROSI.Apis;

public static class ActionApi {
    public static string GetMemberType(this Action actionRepresentation) => actionRepresentation.Wrapped["memberType"].ToString();
    public static string GetId(this Action actionRepresentation) => actionRepresentation.Wrapped["id"].ToString(); 
    
    public static IEnumerable<Link> GetLinks(this Action actionRepresentation) => actionRepresentation.Wrapped.GetLinks();


    public static async Task<ActionResult> Invoke(this Action actionRepresentation, InvokeOptions options = null) {
        var json = await HttpHelpers.Execute(actionRepresentation, options ?? new InvokeOptions());
        return new ActionResult(JObject.Parse(json));
    }

    public static async Task<ActionResult> Invoke(this Action actionRepresentation, InvokeOptions options, params object[] pp) {
        var json = await HttpHelpers.Execute(actionRepresentation, options, pp);
        return new ActionResult(JObject.Parse(json));
    }

    public static async Task<ActionResult> Invoke(this Action actionRepresentation, params object[] pp) => await actionRepresentation.Invoke(new InvokeOptions(), pp);
}