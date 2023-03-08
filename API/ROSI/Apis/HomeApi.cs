using ROSI.Exceptions;
using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;
using Version = ROSI.Records.Version;

namespace ROSI.Apis;

public static class HomeApi {
    public static Link GetSelfLink(this Home homeRepresentation) => homeRepresentation.GetLinks().GetSelfLink() ?? throw new NoSuchPropertyRosiException("Missing self link on Home");
    public static Link GetUserLink(this Home homeRepresentation) => homeRepresentation.GetLinks().GetLinkOfRel(RelApi.Rels.user) ?? throw new NoSuchPropertyRosiException("Missing user link on Home");
    public static Link? GetServicesLink(this Home homeRepresentation) => homeRepresentation.GetLinks().GetLinkOfRel(RelApi.Rels.services);
    public static Link? GetMenusLink(this Home homeRepresentation) => homeRepresentation.GetLinks().GetLinkOfRel(RelApi.Rels.menus);
    public static Link GetVersionLink(this Home homeRepresentation) => homeRepresentation.GetLinks().GetLinkOfRel(RelApi.Rels.version) ?? throw new NoSuchPropertyRosiException("Missing version link on Home");

    public static async Task<User> GetUser(this Home homeRepresentation, InvokeOptions? options = null) => new(await ApiHelpers.GetResourceAsync(homeRepresentation.GetUserLink(), options ?? homeRepresentation.Options), homeRepresentation.Options);

    public static async Task<Version> GetVersion(this Home homeRepresentation, InvokeOptions? options = null) => new(await ApiHelpers.GetResourceAsync(homeRepresentation.GetVersionLink(), options ?? homeRepresentation.Options), homeRepresentation.Options);

    public static async Task<Services?> GetServices(this Home homeRepresentation, InvokeOptions? options = null) => homeRepresentation.GetServicesLink() is { } l ? new Services(await ApiHelpers.GetResourceAsync(l, options ?? homeRepresentation.Options), homeRepresentation.Options) : null;

    public static async Task<Menus?> GetMenus(this Home homeRepresentation, InvokeOptions? options = null) => homeRepresentation.GetMenusLink() is { } l ? new Menus(await ApiHelpers.GetResourceAsync(l, options ?? homeRepresentation.Options), homeRepresentation.Options) : null;
}