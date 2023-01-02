using System.Globalization;
using System.Reflection;
using Newtonsoft.Json.Linq;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

// can be object, service or menu
public static class DomainObjectApi {
    public enum MemberType {
        action,
        collection,
        property
    }

    private static IEnumerable<JObject> GetMembersOfType(this DomainObject objectRepresentation, MemberType ofType) =>
        objectRepresentation.GetMembers().Where(t => t.Value["memberType"]?.Value<string>() == ofType.ToString()).Select(t => t.Value as JObject);

    private static JObject GetMemberOfType(this DomainObject objectRepresentation, MemberType ofType, string memberName) =>
        objectRepresentation.GetMembers().Where(t => t.Value["memberType"]?.Value<string>() == ofType.ToString() && t.Name == memberName).Select(t => t.Value as JObject).Single();

    private static IEnumerable<JProperty> GetMembers(this DomainObject objectRepresentation) {
        var members = objectRepresentation.Wrapped["members"];
        return members.Cast<JProperty>().ToList();
    }

    public static string GetServiceId(this DomainObject objectRepresentation) => objectRepresentation.Wrapped["serviceId"]?.ToString();

    public static string GetMenuId(this DomainObject objectRepresentation) => objectRepresentation.Wrapped["menuId"]?.ToString();

    public static string GetInstanceId(this DomainObject objectRepresentation) => objectRepresentation.Wrapped["instanceId"]?.ToString();

    public static string GetDomainType(this DomainObject objectRepresentation) => objectRepresentation.Wrapped["domainType"]?.ToString();

    public static string GetTitle(this DomainObject objectRepresentation) => objectRepresentation.Wrapped["title"].ToString();

    public static IEnumerable<ActionMember> GetActions(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfType(MemberType.action).Select(p => new ActionMember(p));

    public static IEnumerable<Collection> GetCollections(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfType(MemberType.collection).Select(p => new Collection(p));

    public static IEnumerable<PropertyMember> GetProperties(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfType(MemberType.property).Select(p => new PropertyMember(p));

    public static PropertyMember GetProperty(this DomainObject objectRepresentation, string propertyName) => new(objectRepresentation.GetMemberOfType(MemberType.property, propertyName));

    public static ActionMember GetAction(this DomainObject objectRepresentation, string actionName) => new(objectRepresentation.GetMemberOfType(MemberType.action, actionName));

    public static CollectionMember GetCollection(this DomainObject objectRepresentation, string collectionName) => new(objectRepresentation.GetMemberOfType(MemberType.collection, collectionName));

    public static T GetAsPoco<T>(this DomainObject objectRepresentation) where T : struct {
        var scalarProperties = objectRepresentation.GetProperties().Where(p => p.IsScalarProperty());
        return scalarProperties.Aggregate(new T(), CopyProperty);
    }

    private static T CopyProperty<T>(T toObject, IProperty fromProperty) {
        var boxed = toObject as object;
        var toProperty = typeof(T).GetProperty(fromProperty.GetId());
        if (toProperty is not null && fromProperty.GetValue() is IConvertible fromValue) {
            var convertedFromValue = fromValue.ToType(toProperty.PropertyType, CultureInfo.InvariantCulture);
            toProperty.SetValue(boxed, convertedFromValue);
        }

        return (T)boxed;
    }
}