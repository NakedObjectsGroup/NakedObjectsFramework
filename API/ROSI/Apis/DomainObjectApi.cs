using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;
using Action = ROSI.Records.Action;
using Extensions = ROSI.Records.Extensions;

namespace ROSI.Apis;

// can be object, service or menu
public static class DomainObjectApi {
    private static IEnumerable<JProperty> GetMembersOfType(this DomainObject objectRepresentation, string ofType) =>
        objectRepresentation.GetMembers().Where(t => t.Value["memberType"]?.Value<string>() == ofType);

    private static JProperty GetMemberOfType(this DomainObject objectRepresentation, string ofType, string memberName) => objectRepresentation.GetMembersOfType(ofType).Single(t => t.Name == memberName);

    private static IEnumerable<JProperty> GetMembers(this DomainObject objectRepresentation) {
        var members = objectRepresentation.Wrapped["members"];
        return members.Cast<JProperty>().ToList();
    }

    public static string GetServiceId(this DomainObject objectRepresentation) => objectRepresentation.Wrapped["serviceId"]?.ToString();
    
    public static string GetMenuId(this DomainObject objectRepresentation) => objectRepresentation.Wrapped["menuId"]?.ToString();

    public static string GetInstanceId(this DomainObject objectRepresentation) => objectRepresentation.Wrapped["instanceId"]?.ToString();

    public static string GetDomainType(this DomainObject objectRepresentation) => objectRepresentation.Wrapped["domainType"]?.ToString();

    public static string GetTitle(this DomainObject objectRepresentation) => objectRepresentation.Wrapped["title"].ToString();

    public static IEnumerable<Action> GetActions(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfType("action").Select(p => new Action(p));

    public static IEnumerable<Collection> GetCollections(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfType("collection").Select(p => new Collection(p));

    public static IEnumerable<Property> GetProperties(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfType("property").Select(p => new Property(p));

    public static Property GetProperty(this DomainObject objectRepresentation, string propertyName) => new(objectRepresentation.GetMemberOfType("property", propertyName));

    public static Action GetAction(this DomainObject objectRepresentation, string actionName) => new(objectRepresentation.GetMemberOfType("action", actionName));

    public static Collection GetCollection(this DomainObject objectRepresentation, string collectionName) => new(objectRepresentation.GetMemberOfType("collection", collectionName));

    public static IEnumerable<Link> GetLinks(this DomainObject objectRepresentation) => objectRepresentation.Wrapped.GetLinks();
}

