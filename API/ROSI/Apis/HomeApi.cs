using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class HomeApi {


    public static IEnumerable<Link> GetLinks(this Home homeRepresentation) => homeRepresentation.Wrapped.GetLinks();
}