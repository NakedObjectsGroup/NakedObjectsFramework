using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;

namespace ROSI.Apis;

public static class HomeApi {
    public static IEnumerable<Link> GetLinks(this Home homeRepresentation) => homeRepresentation.Wrapped.GetLinks();

    public static Link GetUserLink(this Home homeRepresentation) => homeRepresentation.Wrapped.GetLinks().GetLinkOfRel(RelApi.Rels.user);

    public static async Task<User> GetUser(this Home homeRepresentation) {
        var href = homeRepresentation.GetUserLink().GetHref();
        var json = await HttpHelpers.Execute(href, new InvokeOptions());
        return new User(JObject.Parse(json));
    }
}