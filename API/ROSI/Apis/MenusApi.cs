using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class MenusApi {
    public static IEnumerable<Link> GetValue(this Menus menusRepresentation) => menusRepresentation.Wrapped[JsonConstants.Value].ToLinks();

    public static Link GetMenuLink(this Menus menusRepresentation, string menuId) =>
        menusRepresentation.GetValue().SingleOrDefault(l => l.GetRel().GetMenuId() == menuId);

    public static async Task<DomainObject> GetMenu(this Menus menusRepresentation, string menuId, InvokeOptions options) =>
        new(await ApiHelpers.GetResourceAsync(menusRepresentation.GetMenuLink(menuId), options));
}