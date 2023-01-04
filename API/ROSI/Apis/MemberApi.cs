using ROSI.Helpers;
using ROSI.Interfaces;

namespace ROSI.Apis;

public static class MemberApi {
    public static string GetMemberType(this IMember memberRepresentation) => memberRepresentation.GetMandatoryProperty(JsonConstants.MemberType).ToString();
    public static string? GetId(this IMember memberRepresentation) => memberRepresentation.GetOptionalProperty(JsonConstants.Id)?.ToString();
    public static string? GetDisabledReason(this IMember memberRepresentation) => memberRepresentation.GetOptionalProperty(JsonConstants.DisabledReason)?.ToString();
}