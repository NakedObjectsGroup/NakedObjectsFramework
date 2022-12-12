using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class MenusApi {
    public static IEnumerable<Link> GetLinks(this Menus menusRepresentation) => menusRepresentation.Wrapped.GetLinks();

    public static Extensions GetExtensions(this Menus menusRepresentation) => menusRepresentation.Wrapped.GetExtensions();

    public static IEnumerable<Link> GetValue(this Menus menusRepresentation) => menusRepresentation.Wrapped["value"].ToLinks();

    public static Link GetMenuLink(this Menus menusRepresentation, string menuId) =>
        menusRepresentation.GetValue().SingleOrDefault(l => l.GetRel().GetMenuId() == menuId);

    public static async Task<DomainObject> GetMenu(this Menus menusRepresentation, string menuId) =>
        new(await ApiHelpers.GetResourceAsync(menusRepresentation.GetMenuLink(menuId)));
}