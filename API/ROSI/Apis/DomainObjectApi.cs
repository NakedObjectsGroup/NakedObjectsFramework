using System.Globalization;
using Newtonsoft.Json.Linq;
using ROSI.Helpers;
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

    private static IEnumerable<JObject> GetMembersOfTypeAsJObjects(this DomainObject objectRepresentation, MemberType ofType) =>
        objectRepresentation.GetMembersOfType(ofType).Select(t => t.Value as JObject);

    private static IEnumerable<JProperty> GetMembersOfType(this DomainObject objectRepresentation, MemberType ofType) => 
        objectRepresentation.GetMembers().Where(t => t.Value[JsonConstants.MemberType]?.Value<string>() == ofType.ToString());

    private static JObject GetMemberOfTypeAsJObject(this DomainObject objectRepresentation, MemberType ofType, string memberName) =>
        objectRepresentation.GetMembersOfType(ofType).Where(t => t.Name == memberName).Select(t => t.Value as JObject).Single();

    private static IEnumerable<JProperty> GetMembers(this DomainObject objectRepresentation) {
        var members = objectRepresentation.Wrapped[JsonConstants.Members];
        return members.Cast<JProperty>().ToList();
    }

    public static string GetServiceId(this DomainObject objectRepresentation) => objectRepresentation.Wrapped[JsonConstants.ServiceId]?.ToString();

    public static string GetMenuId(this DomainObject objectRepresentation) => objectRepresentation.Wrapped[JsonConstants.MenuId]?.ToString();

    public static string GetInstanceId(this DomainObject objectRepresentation) => objectRepresentation.Wrapped[JsonConstants.InstanceId]?.ToString();

    public static string GetDomainType(this DomainObject objectRepresentation) => objectRepresentation.Wrapped[JsonConstants.DomainType]?.ToString();

    public static string GetTitle(this DomainObject objectRepresentation) => objectRepresentation.Wrapped[JsonConstants.Title].ToString();

    public static IEnumerable<ActionMember> GetActions(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfTypeAsJObjects(MemberType.action).Select(p => new ActionMember(p));

    public static IEnumerable<Collection> GetCollections(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfTypeAsJObjects(MemberType.collection).Select(p => new Collection(p));

    public static IEnumerable<PropertyMember> GetProperties(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfTypeAsJObjects(MemberType.property).Select(p => new PropertyMember(p));

    public static PropertyMember GetProperty(this DomainObject objectRepresentation, string propertyName) => new(objectRepresentation.GetMemberOfTypeAsJObject(MemberType.property, propertyName));

    public static ActionMember GetAction(this DomainObject objectRepresentation, string actionName) => new(objectRepresentation.GetMemberOfTypeAsJObject(MemberType.action, actionName));

    public static CollectionMember GetCollection(this DomainObject objectRepresentation, string collectionName) => new(objectRepresentation.GetMemberOfTypeAsJObject(MemberType.collection, collectionName));

    public static T GetAsPoco<T>(this DomainObject objectRepresentation) where T : struct {
        var scalarProperties = objectRepresentation.GetProperties().Where(p => p.IsScalarProperty());
        return (T)scalarProperties.Aggregate(new T() as object, CopyProperty); // as object to box
    }

    private static object CopyProperty(object toObject, IProperty fromProperty) {
        var toProperty = toObject.GetType().GetProperty(fromProperty.GetId());
        if (toProperty is not null && fromProperty.GetValue() is IConvertible fromValue) {
            var convertedFromValue = fromValue.ToType(toProperty.PropertyType, CultureInfo.InvariantCulture);
            toProperty.SetValue(toObject, convertedFromValue);
        }

        return toObject;
    }
}