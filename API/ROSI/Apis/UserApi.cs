using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class UserApi {
    public static IEnumerable<Link> GetLinks(this User userRepresentation) => userRepresentation.Wrapped.GetLinks();

    public static Extensions GetExtensions(this User userRepresentation) => userRepresentation.Wrapped.GetExtensions();

    public static string GetUserName(this User userRepresentation) => userRepresentation.Wrapped["userName"].ToString();

    public static string GetFriendlyName(this User userRepresentation) => userRepresentation.Wrapped["friendlyName"]?.ToString();

    public static string GetEmail(this User userRepresentation) => userRepresentation.Wrapped["email"]?.ToString();

    public static IEnumerable<string> GetRoles(this User userRepresentation) => userRepresentation.Wrapped["roles"].Select(t => t.ToString());
}