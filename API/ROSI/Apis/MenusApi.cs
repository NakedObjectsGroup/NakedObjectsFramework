using ROSI.Helpers;
using ROSI.Records;
using Version = ROSI.Records.Version;

namespace ROSI.Apis;

public static class MenusApi {
    public static IEnumerable<Link> GetLinks(this Menus menusRepresentation) => menusRepresentation.Wrapped.GetLinks();

    public static IEnumerable<Link> GetValue(this Menus menusRepresentation) => menusRepresentation.Wrapped["value"].ToLinks();
}