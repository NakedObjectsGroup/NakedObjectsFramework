using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;
using Action = ROSI.Records.Action;

namespace ROSI.Apis;

public static class ActionApi {
    public static IEnumerable<Link> GetLinks(this Action actionRepresentation) => actionRepresentation.Wrapped.Value["links"].Select(t => new Link(t as JObject));

    public static DomainObject Invoke(this Action actionRepresentation, string token = null) {
        var json = HttpHelpers.Execute(actionRepresentation, token);
        return new DomainObject(JObject.Parse(json));
    }

    public static DomainObject Invoke(this Action actionRepresentation, EntityTagHeaderValue tag, string token = null, params object[] pp) {
        var json = HttpHelpers.Execute(actionRepresentation, tag, token, pp);
        return new DomainObject(JObject.Parse(json));
    }
}