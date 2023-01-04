using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ActionResultApi {
    public enum ResultType {
        Void,
        Scalar,
        List,
        Object
    }

    private static JObject? Result(this ActionResult resultRepresentation) => resultRepresentation.GetOptionalPropertyAsJObject(JsonConstants.Result);

    public static ResultType GetResultType(this ActionResult actionResultRepresentation) => Enum.Parse<ResultType>(actionResultRepresentation.GetMandatoryProperty(JsonConstants.ResultType).ToString(), true);

    public static T? GetScalarValue<T>(this ActionResult resultRepresentation) => resultRepresentation.GetOptionalProperty(JsonConstants.Result) is { } o ? o.Value<T>() : default;

    public static DomainObject? GetObject(this ActionResult resultRepresentation) => resultRepresentation.Result() is { } jo ? new DomainObject(jo) : null;

    public static List? GetList(this ActionResult resultRepresentation) => resultRepresentation.Result() is { } jo ? new List(jo) : null;
}