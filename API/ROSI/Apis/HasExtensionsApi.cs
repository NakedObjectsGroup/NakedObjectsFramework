using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class HasExtensionsApi {
    public static Extensions GetExtensions(this IHasExtensions hasExtensions) => hasExtensions.Wrapped.GetExtensions();
}