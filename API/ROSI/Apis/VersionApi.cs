using ROSI.Helpers;
using Version = ROSI.Records.Version;

namespace ROSI.Apis;

public static class VersionApi {
    public static string GetSpecVersion(this Version versionRepresentation) => versionRepresentation.GetMandatoryProperty(JsonConstants.SpecVersion).ToString();

    public static string? GetImplVersion(this Version versionRepresentation) => versionRepresentation.GetOptionalProperty(JsonConstants.ImplVersion)?.ToString();
}