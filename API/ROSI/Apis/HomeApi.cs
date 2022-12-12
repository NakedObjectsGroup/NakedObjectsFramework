using ROSI.Helpers;
using ROSI.Records;
using Version = ROSI.Records.Version;

namespace ROSI.Apis;

public static class HomeApi {
    public static IEnumerable<Link> GetLinks(this Home homeRepresentation) => homeRepresentation.Wrapped.GetLinks();
    public static Extensions GetExtensions(this Home homeRepresentation) => homeRepresentation.Wrapped.GetExtensions();

    public static Link GetSelfLink(this Home homeRepresentation) => homeRepresentation.GetLinks().GetLinkOfRel(RelApi.Rels.self);
    public static Link GetUserLink(this Home homeRepresentation) => homeRepresentation.GetLinks().GetLinkOfRel(RelApi.Rels.user);
    public static Link GetServicesLink(this Home homeRepresentation) => homeRepresentation.GetLinks().GetLinkOfRel(RelApi.Rels.services);
    public static Link GetMenusLink(this Home homeRepresentation) => homeRepresentation.GetLinks().GetLinkOfRel(RelApi.Rels.menus);
    public static Link GetVersionLink(this Home homeRepresentation) => homeRepresentation.GetLinks().GetLinkOfRel(RelApi.Rels.version);

    public static async Task<User> GetUserAsync(this Home homeRepresentation) => new(await ApiHelpers.GetResourceAsync(homeRepresentation.GetUserLink()));

    public static async Task<Version> GetVersionAsync(this Home homeRepresentation) => new(await ApiHelpers.GetResourceAsync(homeRepresentation.GetVersionLink()));

    public static async Task<Services> GetServicesAsync(this Home homeRepresentation) => new(await ApiHelpers.GetResourceAsync(homeRepresentation.GetServicesLink()));

    public static async Task<Menus> GetMenusAsync(this Home homeRepresentation) => new(await ApiHelpers.GetResourceAsync(homeRepresentation.GetMenusLink()));
}