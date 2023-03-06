using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ArgumentApi {
    public static object? GetValue(this Argument argumentRepresentation) =>
        argumentRepresentation.GetOptionalProperty(JsonConstants.Value) is JValue v ? v.Value : null;

    public static Link? GetLinkValue(this Argument argumentRepresentation) =>
        argumentRepresentation.GetOptionalProperty(JsonConstants.Value) is JObject jo ? new Link(jo, argumentRepresentation.Options) : null;

    public static string? GetInvalidReason(this Argument argumentRepresentation) =>
        argumentRepresentation.GetOptionalProperty(JsonConstants.InvalidReason) is JValue v ? v.Value<string>() : null;
}