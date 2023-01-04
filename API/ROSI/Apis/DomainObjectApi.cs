using System.Globalization;
using Newtonsoft.Json.Linq;
using ROSI.Helpers;
using ROSI.Interfaces;
using ROSI.Records;

namespace ROSI.Apis;

// can be object, service or menu
public static class DomainObjectApi {
    public enum MemberType {
        Action,
        Collection,
        Property
    }

    private static IEnumerable<JObject> GetMembersOfTypeAsJObjects(this DomainObject objectRepresentation, MemberType ofType) =>
        objectRepresentation.GetMembersOfType(ofType).Select(t => t.Value).Cast<JObject>();

    private static IEnumerable<JProperty> GetMembersOfType(this DomainObject objectRepresentation, MemberType ofType) =>
        objectRepresentation.GetMembers().Where(t => t.Value[JsonConstants.MemberType]?.Value<string>() == ofType.ToString().ToLower());

    private static JObject? GetMemberOfTypeAsJObject(this DomainObject objectRepresentation, MemberType ofType, string memberName) =>
        objectRepresentation.GetMembersOfType(ofType).Where(t => t.Name == memberName).Select(t => t.Value).Cast<JObject>().SingleOrDefault();

    private static IEnumerable<JProperty> GetMembers(this DomainObject objectRepresentation) =>
        objectRepresentation.GetMandatoryProperty(JsonConstants.Members).Cast<JProperty>();

    public static string? GetServiceId(this DomainObject objectRepresentation) => objectRepresentation.GetOptionalProperty(JsonConstants.ServiceId)?.ToString();

    public static string? GetMenuId(this DomainObject objectRepresentation) => objectRepresentation.GetOptionalProperty(JsonConstants.MenuId)?.ToString();

    public static string? GetInstanceId(this DomainObject objectRepresentation) => objectRepresentation.GetOptionalProperty(JsonConstants.InstanceId)?.ToString();

    public static string? GetDomainType(this DomainObject objectRepresentation) => objectRepresentation.GetOptionalProperty(JsonConstants.DomainType)?.ToString();

    public static string GetTitle(this DomainObject objectRepresentation) => objectRepresentation.GetMandatoryProperty(JsonConstants.Title).ToString();

    public static IEnumerable<ActionMember> GetActions(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfTypeAsJObjects(MemberType.Action).Select(p => new ActionMember(p));

    public static IEnumerable<Collection> GetCollections(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfTypeAsJObjects(MemberType.Collection).Select(p => new Collection(p));

    public static IEnumerable<PropertyMember> GetProperties(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfTypeAsJObjects(MemberType.Property).Select(p => new PropertyMember(p));

    public static PropertyMember? GetProperty(this DomainObject objectRepresentation, string propertyName) => objectRepresentation.GetMemberOfTypeAsJObject(MemberType.Property, propertyName) is { } jo ? new PropertyMember(jo) : null;

    public static ActionMember? GetAction(this DomainObject objectRepresentation, string actionName) => objectRepresentation.GetMemberOfTypeAsJObject(MemberType.Action, actionName) is { } jo ? new ActionMember(jo) : null;

    public static CollectionMember? GetCollection(this DomainObject objectRepresentation, string collectionName) => objectRepresentation.GetMemberOfTypeAsJObject(MemberType.Collection, collectionName) is { } jo ? new CollectionMember(jo) : null;

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