using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ServicesApi {
    public static IEnumerable<Link> GetLinks(this Services servicesRepresentation) => servicesRepresentation.Wrapped.GetLinks();

    public static IEnumerable<Link> GetValue(this Services servicesRepresentation) => servicesRepresentation.Wrapped["value"].ToLinks();
}