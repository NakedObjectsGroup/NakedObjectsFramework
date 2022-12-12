using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ServicesApi {
    public static IEnumerable<Link> GetLinks(this Services servicesRepresentation) => servicesRepresentation.Wrapped.GetLinks();

    public static Extensions GetExtensions(this Services servicesRepresentation) => servicesRepresentation.Wrapped.GetExtensions();

    public static IEnumerable<Link> GetValue(this Services servicesRepresentation) => servicesRepresentation.Wrapped["value"].ToLinks();

    public static Link GetServiceLink(this Services servicesRepresentation, string serviceId) =>
        servicesRepresentation.GetValue().SingleOrDefault(l => l.GetRel().GetServiceId() == serviceId);

    public static async Task<DomainObject> GetService(this Services servicesRepresentation, string serviceId) =>
        new(await ApiHelpers.GetResourceAsync(servicesRepresentation.GetServiceLink(serviceId)));
}