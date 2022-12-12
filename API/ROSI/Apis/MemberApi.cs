using ROSI.Interfaces;

namespace ROSI.Apis;

public static class MemberApi {
    public static string GetMemberType(this IMember memberRepresentation) => memberRepresentation.Wrapped["memberType"]!.ToString();
    public static string GetId(this IMember memberRepresentation) => memberRepresentation.Wrapped["id"]!.ToString();

    public static string? GetDisabledReason(this IMember memberRepresentation) => memberRepresentation.Wrapped["disabledReason"]?.ToString();
}