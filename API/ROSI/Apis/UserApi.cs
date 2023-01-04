using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class UserApi {
    public static string GetUserName(this User userRepresentation) => userRepresentation.GetMandatoryProperty(JsonConstants.UserName).ToString();

    public static string? GetFriendlyName(this User userRepresentation) => userRepresentation.GetOptionalProperty(JsonConstants.FriendlyName)?.ToString();

    public static string? GetEmail(this User userRepresentation) => userRepresentation.GetOptionalProperty(JsonConstants.Email)?.ToString();

    public static IEnumerable<string> GetRoles(this User userRepresentation) => userRepresentation.GetMandatoryProperty(JsonConstants.Roles).Select(t => t.ToString());
}