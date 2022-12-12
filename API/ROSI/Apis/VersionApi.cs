using ROSI.Helpers;
using ROSI.Records;
using Version = ROSI.Records.Version;

namespace ROSI.Apis;

public static class VersionApi {
    public static string GetSpecVersion(this Version versionRepresentation) => versionRepresentation.Wrapped["specVersion"].ToString();

    public static string GetImplVersion(this Version versionRepresentation) => versionRepresentation.Wrapped["implVersion"].ToString();
}