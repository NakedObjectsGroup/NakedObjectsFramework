using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class ListApi {

    public static IEnumerable<Link> GetLinks(this List listRepresentation) => listRepresentation.Wrapped.GetLinks();


    public static IEnumerable<Link> GetValue(this List listRepresentation) => listRepresentation.Wrapped["value"].ToLinks();
}