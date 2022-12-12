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

    public static ResultType GetResultType(this ActionResult actionResultRepresentation) => Enum.Parse<ResultType>(actionResultRepresentation.Wrapped["resultType"].ToString());

    public static T GetScalarValue<T>(this ActionResult resultRepresentation) => resultRepresentation.Wrapped["result"].Value<T>();

    public static DomainObject GetObject(this ActionResult resultRepresentation) => new((JObject)resultRepresentation.Wrapped["result"]);

    public static List GetList(this ActionResult resultRepresentation) => new((JObject)resultRepresentation.Wrapped["result"]);
}