using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class UserApi {
    public static IEnumerable<Link> GetLinks(this User userRepresentation) => userRepresentation.Wrapped.GetLinks();

    public static string GetUserName(this User userRepresentation) => userRepresentation.Wrapped["userName"].ToString();

    public static string GetFriendlyNameName(this User userRepresentation) => userRepresentation.Wrapped["friendlyName"].ToString();
}