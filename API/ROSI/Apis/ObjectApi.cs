using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using ROSI.Helpers;

namespace ROSI.Apis;


public static class ObjectApi {
    public static JObject GetObject(Uri uri, string token = null) {
        var json = HttpHelpers.Execute(uri, token);
        return JObject.Parse(json);
    }

    public static (JObject, EntityTagHeaderValue tag) GetObjectWithTag(Uri uri, string token = null) {
        var (json, tag) = HttpHelpers.ExecuteWithTag(uri, token);
        return (JObject.Parse(json), tag);
    }

    public static JObject InvokeAction(this JObject objectRepresentation, string actionName) {
        var action = objectRepresentation.GetAction(actionName);
        var json = HttpHelpers.Execute(action);
        return JObject.Parse(json);
    }

    public static T GetPropertyValue<T>(this JObject objectRepresentation, string propertyName) {
        var valueObject = objectRepresentation.GetProperty(propertyName).Value as JObject;

        if (valueObject is null) {
            throw new Exception("No such property value");
        }

        return valueObject["value"]!.Value<T>();
    }

    public static IEnumerable<JProperty> GetMembers(this JObject objectRepresentation) {
        var members = objectRepresentation["members"];

        if (members is null) {
            throw new Exception("No members");
        }

        return members.Cast<JProperty>().ToList();
    }

    private static IEnumerable<JProperty> GetMembersOfType(this JObject objectRepresentation, string ofType) =>
        objectRepresentation.GetMembers().Where(t => t.Value["memberType"]?.Value<string>() == ofType);

    public static IEnumerable<JProperty> GetActions(this JObject objectRepresentation) => objectRepresentation.GetMembersOfType("action");

    public static IEnumerable<JProperty> GetCollections(this JObject objectRepresentation) => objectRepresentation.GetMembersOfType("collection");

    public static IEnumerable<JProperty> GetProperties(this JObject objectRepresentation) => objectRepresentation.GetMembersOfType("property");

    private static JProperty GetMemberOfType(this JObject objectRepresentation, string ofType, string memberName) => objectRepresentation.GetMembersOfType(ofType).Single(t => t.Name == memberName);

    public static JProperty GetProperty(this JObject objectRepresentation, string propertyName) => objectRepresentation.GetMemberOfType("property", propertyName);

    public static JProperty GetAction(this JObject objectRepresentation, string actionName) => objectRepresentation.GetMemberOfType("action", actionName);

    public static JProperty GetCollection(this JObject objectRepresentation, string collectionName) => objectRepresentation.GetMemberOfType("collection", collectionName);
}