using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class MenusApi {
    public static IEnumerable<Link> GetValue(this Menus menusRepresentation) => menusRepresentation.GetMandatoryProperty(JsonConstants.Value).ToLinks(menusRepresentation);

    public static Link? GetMenuLink(this Menus menusRepresentation, string menuId) =>
        menusRepresentation.GetValue().SingleOrDefault(l => l.GetRel().GetMenuId() == menuId);

    public static async Task<DomainObject?> GetMenu(this Menus menusRepresentation, string menuId, InvokeOptions? options = null) =>
        menusRepresentation.GetMenuLink(menuId) is { } l ? new DomainObject(await ApiHelpers.GetResourceAsync(l, options ?? menusRepresentation.Options), menusRepresentation.Options) : null;
}