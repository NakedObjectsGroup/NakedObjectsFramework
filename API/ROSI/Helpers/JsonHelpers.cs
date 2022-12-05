using Newtonsoft.Json.Linq;
using ROSI.Records;
using Action = System.Action;

namespace ROSI.Helpers; 

public static class JsonHelpers {
    public static IEnumerable<Link> GetLinks(this JObject jo) => jo["links"].Select(t => new Link((JObject)t));

    public static IEnumerable<Link> GetLinks(this JProperty jp) => jp.Value["links"].Select(t => new Link((JObject)t));
}