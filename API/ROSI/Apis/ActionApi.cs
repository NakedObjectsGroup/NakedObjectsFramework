using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;
using Action = ROSI.Records.Action;

namespace ROSI.Apis;

public static class ActionApi {
    public static IEnumerable<Link> GetLinks(this Action actionRepresentation) => actionRepresentation.Wrapped.Value["links"].Select(t => new Link(t as JObject));

    public static DomainObject Invoke(this Action actionRepresentation, InvokeOptions options = null) {
        var json = HttpHelpers.Execute(actionRepresentation, options ?? new InvokeOptions());
        return new DomainObject(JObject.Parse(json));
    }

    public static DomainObject Invoke(this Action actionRepresentation, InvokeOptions options, params object[] pp) {
        var json = HttpHelpers.Execute(actionRepresentation, options, pp);
        return new DomainObject(JObject.Parse(json));
    }

    public static DomainObject Invoke(this Action actionRepresentation, params object[] pp) {
        var json = HttpHelpers.Execute(actionRepresentation, new InvokeOptions(), pp);
        return new DomainObject(JObject.Parse(json));
    }
}