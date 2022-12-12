using ROSI.Helpers;
using ROSI.Interfaces;
using Extensions = ROSI.Records.Extensions;

namespace ROSI.Apis;


public static class HasExtensionsApi {

    public static Extensions GetExtensions(this IHasExtensions hasExtensions) => hasExtensions.Wrapped.GetExtensions();
}