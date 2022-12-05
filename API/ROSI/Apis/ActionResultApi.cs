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


    public static IEnumerable<Link> GetLinks(this ActionResult actionResultRepresentation) => actionResultRepresentation.Wrapped["links"].Select(t => new Link(t as JObject));

    public static ResultType GetResultType(this ActionResult actionResultRepresentation) => Enum.Parse<ResultType>(actionResultRepresentation.Wrapped["resultType"].ToString());

    public static JObject GetResultInner(this ActionResult actionResultRepresentation) => actionResultRepresentation.Wrapped["result"] as JObject;

    public static object GetResult(this ActionResult actionResultRepresentation) =>
        actionResultRepresentation.GetResultType() switch {
            ResultType.@void => null,
            ResultType.scalar => null,
            ResultType.list => null,
            ResultType.@object => null,
        };

}