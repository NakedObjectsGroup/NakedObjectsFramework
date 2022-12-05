using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json.Linq;
using ROSI.Records;

namespace ROSI.Apis;

public static class ActionResultApi {
    public enum ResultType {
        @void,
        scalar,
        list,
        @object
    }


    public static IEnumerable<Link> GetLinks(this ActionResult actionResultRepresentation) => actionResultRepresentation.Wrapped["links"].Select(t => new Link((JObject)t));

    public static ResultType GetResultType(this ActionResult actionResultRepresentation) => Enum.Parse<ResultType>(actionResultRepresentation.Wrapped["resultType"].ToString());

    public static T GetScalarValue<T>(this ActionResult resultRepresentation) => resultRepresentation.Wrapped["result"].Value<T>();

    public static DomainObject GetObjectValue(this ActionResult resultRepresentation) => new DomainObject((JObject)resultRepresentation.Wrapped["result"]);

}