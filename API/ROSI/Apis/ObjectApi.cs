using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Records;
using Action = ROSI.Records.Action;

namespace ROSI.Apis;

public static class ObjectApi {
    private static IEnumerable<JProperty> GetMembersOfType(this DomainObject objectRepresentation, string ofType) =>
        objectRepresentation.GetMembers().Where(t => t.Value["memberType"]?.Value<string>() == ofType);

    private static JProperty GetMemberOfType(this DomainObject objectRepresentation, string ofType, string memberName) => objectRepresentation.GetMembersOfType(ofType).Single(t => t.Name == memberName);

    public static DomainObject GetObject(Uri uri, string token = null) {
        var json = HttpHelpers.Execute(uri, token);
        return new DomainObject(JObject.Parse(json));
    }

    public static (DomainObject, EntityTagHeaderValue tag) GetObjectWithTag(Uri uri, string token = null) {
        var (json, tag) = HttpHelpers.ExecuteWithTag(uri, token);
        return (new DomainObject(JObject.Parse(json)), tag);
    }

    private static IEnumerable<JProperty> GetMembers(this DomainObject objectRepresentation) {
        var members = objectRepresentation.Wrapped["members"];

        if (members is null) {
            throw new Exception("No members");
        }

        return members.Cast<JProperty>().ToList();
    }

    public static IEnumerable<Action> GetActions(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfType("action").Select(p => new Action(p));

    public static IEnumerable<Collection> GetCollections(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfType("collection").Select(p => new Collection(p));

    public static IEnumerable<Property> GetProperties(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfType("property").Select(p => new Property(p));

    public static Property GetProperty(this DomainObject objectRepresentation, string propertyName) => new(objectRepresentation.GetMemberOfType("property", propertyName));

    public static Action GetAction(this DomainObject objectRepresentation, string actionName) => new(objectRepresentation.GetMemberOfType("action", actionName));

    public static Collection GetCollection(this DomainObject objectRepresentation, string collectionName) => new(objectRepresentation.GetMemberOfType("collection", collectionName));

    public static IEnumerable<Link> GetLinks(this DomainObject objectRepresentation) => objectRepresentation.Wrapped["links"].Select(t => new Link(t as JObject));
}