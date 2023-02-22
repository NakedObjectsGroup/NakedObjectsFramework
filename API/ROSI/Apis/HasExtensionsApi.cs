using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

public static class HasExtensionsApi {
    public static Extensions GetExtensions(this IHasExtensions hasExtensions) => new(hasExtensions.GetMandatoryPropertyAsJObject(JsonConstants.Extensions));

    public static object? SafeGetExtension(this IHasExtensions hasExtensions, ExtensionsApi.ExtensionKeys key) {
        var extensions = hasExtensions.GetExtensions().Extensions();
        return extensions.ContainsKey(key) ? extensions[key] : null;
    }
}