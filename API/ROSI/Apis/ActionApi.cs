using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;
using Action = ROSI.Records.Action;

namespace ROSI.Apis;

public static class ActionApi {
    public static IEnumerable<Link> GetLinks(this Action actionRepresentation) => actionRepresentation.Wrapped.GetLinks();

    public static ActionResult Invoke(this Action actionRepresentation, InvokeOptions options = null) {
        var json = HttpHelpers.Execute(actionRepresentation, options ?? new InvokeOptions());
        return new ActionResult(JObject.Parse(json));
    }

    public static ActionResult Invoke(this Action actionRepresentation, InvokeOptions options, params object[] pp) {
        var json = HttpHelpers.Execute(actionRepresentation, options, pp);
        return new ActionResult(JObject.Parse(json));
    }

    public static ActionResult Invoke(this Action actionRepresentation, params object[] pp) {
        var json = HttpHelpers.Execute(actionRepresentation, new InvokeOptions(), pp);
        return new ActionResult(JObject.Parse(json));
    }
}