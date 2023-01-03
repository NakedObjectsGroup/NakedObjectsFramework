using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class PromptApi {
    public static string GetId(this Prompt promptRepresentation) => promptRepresentation.Wrapped["id"]!.ToString();

    public static IEnumerable<T> GetChoices<T>(this Prompt promptRepresentation) => promptRepresentation.Wrapped["choices"].Select(c => c.Value<T>());

    public static IEnumerable<Link> GetLinkChoices(this Prompt promptRepresentation) => promptRepresentation.Wrapped["choices"].ToLinks();
}