using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class TypeActionResultApi {
    public static bool GetValue(this TypeActionResult typeActionResultRepresentation) =>
        typeActionResultRepresentation.GetMandatoryProperty(JsonConstants.Value).Value<bool>();

    public static string GetId(this TypeActionResult typeActionResultRepresentation) =>
        typeActionResultRepresentation.GetMandatoryProperty(JsonConstants.Id).Value<string>() ?? "";
}