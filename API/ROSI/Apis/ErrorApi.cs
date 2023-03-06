using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ErrorApi {
    public static string GetMessage(this Error errorRepresentation) => errorRepresentation.GetMandatoryProperty(JsonConstants.Message).ToString();

    public static IEnumerable<string>? GetStackTrace(this Error errorRepresentation) => errorRepresentation.GetOptionalProperty(JsonConstants.StackTrace)?.Select(t => t.ToString());

    public static Error? GetCausedBy(this Error errorRepresentation) => errorRepresentation.GetOptionalProperty(JsonConstants.CausedBy) is JObject jo ? new Error(jo, errorRepresentation.Options) : null;
}