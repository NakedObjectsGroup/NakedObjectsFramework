using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class HasValueApi {
    public static IEnumerable<Link> GetValue(this IHasValue hasValue) => hasValue.GetMandatoryProperty(JsonConstants.Value).ToLinks(hasValue);
}