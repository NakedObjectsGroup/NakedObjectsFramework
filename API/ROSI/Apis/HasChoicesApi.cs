using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class HasChoicesApi {
    public static IEnumerable<T> GetChoices<T>(this IHasChoices hasChoices) => hasChoices.Wrapped["choices"].Select(c => c.Value<T>());

    public static IEnumerable<Link> GetLinkChoices(this IHasChoices hasChoices) => hasChoices.Wrapped["choices"].ToLinks();
}