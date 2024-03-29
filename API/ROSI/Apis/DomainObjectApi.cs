﻿using System.Globalization;
using Newtonsoft.Json.Linq;
using ROSI.Exceptions;
using ROSI.Helpers;
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

    private static IEnumerable<(JObject obj, string name)> GetMembersOfTypeAsJObjectsAndNames(this DomainObject objectRepresentation, MemberType ofType) =>
        objectRepresentation.GetMembersOfType(ofType).Select(t => ((JObject)t.Value, t.Name));

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

    public static IEnumerable<ActionMember> GetActions(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfTypeAsJObjects(MemberType.Action).Select(p => new ActionMember(p, objectRepresentation.Options));

    public static IEnumerable<CollectionMember> GetCollections(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfTypeAsJObjects(MemberType.Collection).Select(p => new CollectionMember(p, objectRepresentation.Options));

    public static IEnumerable<PropertyMember> GetProperties(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfTypeAsJObjects(MemberType.Property).Select(p => new PropertyMember(p, objectRepresentation.Options));

    public static IEnumerable<(PropertyMember, string)> GetPropertiesAndNames(this DomainObject objectRepresentation) => objectRepresentation.GetMembersOfTypeAsJObjectsAndNames(MemberType.Property).Select(p => (new PropertyMember(p.obj, objectRepresentation.Options), p.name));

    public static PropertyMember? GetProperty(this DomainObject objectRepresentation, string propertyName) => objectRepresentation.GetMemberOfTypeAsJObject(MemberType.Property, propertyName) is { } jo ? new PropertyMember(jo, objectRepresentation.Options) : null;

    public static ActionMember? GetAction(this DomainObject objectRepresentation, string actionName) => objectRepresentation.GetMemberOfTypeAsJObject(MemberType.Action, actionName) is { } jo ? new ActionMember(jo, objectRepresentation.Options) : null;

    public static CollectionMember? GetCollection(this DomainObject objectRepresentation, string collectionName) => objectRepresentation.GetMemberOfTypeAsJObject(MemberType.Collection, collectionName) is { } jo ? new CollectionMember(jo, objectRepresentation.Options) : null;

    public static async Task<DomainObject> Persist(this DomainObject objectRepresentation, params object[] pp) => await objectRepresentation.Persist(objectRepresentation.Options, pp);

    public static async Task<DomainObject> Persist(this DomainObject objectRepresentation, InvokeOptions options, params object[] pp) {
        options = options with { Tag = objectRepresentation.Tag };
        var link = objectRepresentation.GetLinks().GetPersistLink() ?? throw new NoSuchPropertyRosiException("Missing persist link in object");
        var json = (await HttpHelpers.Persist(link, options, pp)).Response;
        return new DomainObject(JObject.Parse(json), objectRepresentation.Options);
    }

    public static async Task ValidatePersist(this DomainObject objectRepresentation, params object[] pp) {
        await ValidatePersist(objectRepresentation, objectRepresentation.Options, pp);
    }

    public static async Task ValidatePersist(this DomainObject objectRepresentation, InvokeOptions options, params object[] pp) {
        options = options with { Tag = objectRepresentation.Tag, ReservedArguments = options.ReservedArguments.Add("x-ro-validate-only", true) };

        var link = objectRepresentation.GetLinks().GetPersistLink() ?? throw new NoSuchPropertyRosiException("Missing persist link in object");
        var json = (await HttpHelpers.Persist(link, options, pp)).Response;
        if (string.IsNullOrEmpty(json)) {
            return;
        }

        throw new UnexpectedResultRosiException($"Expected 'No Content' from validate got: {json[..200]}...");
    }

    public static Dictionary<string, object?> GetPersistMap(this DomainObject objectRepresentation) {
        var args = objectRepresentation.GetLinks().GetPersistLink()?.GetArgumentsMembers() ?? new Dictionary<string, object?>();
        var properties = objectRepresentation.GetProperties().ToDictionary(p => p.GetId()!, p => p.GetValue() ?? p.GetLinkValue());
        var map = new Dictionary<string, object?>();

        foreach (var (k, v) in args) {
            if (properties.ContainsKey(k)) {
                var newValue = properties[k];
                map[k] = newValue;
            }
            else {
                map[k] = v is Dictionary<string, object> d ? d["value"] : null;
            }
        }

        return map;
    }

    public static async Task<DomainObject> PersistWithNamedParams(this DomainObject objectRepresentation, Dictionary<string, object> pp) =>
        await objectRepresentation.Persist(pp.Cast<object>().ToArray());

    public static async Task ValidatePersistWithNamedParams(this DomainObject objectRepresentation, Dictionary<string, object> pp) =>
        await objectRepresentation.ValidatePersist(pp.Cast<object>().ToArray());

    public static async Task<DomainObject> PersistWithNamedParams(this DomainObject objectRepresentation, InvokeOptions options, Dictionary<string, object> pp) =>
        await objectRepresentation.Persist(options, pp.Cast<object>().ToArray());

    public static async Task ValidatePersistWithNamedParams(this DomainObject objectRepresentation, InvokeOptions options, Dictionary<string, object> pp) =>
        await objectRepresentation.ValidatePersist(options, pp.Cast<object>().ToArray());

    public static async Task<DomainObject> Update(this DomainObject objectRepresentation, params object[] pp) => await objectRepresentation.Update(objectRepresentation.Options, pp);

    public static async Task<DomainObject> Update(this DomainObject objectRepresentation, InvokeOptions options, params object[] pp) {
        var link = objectRepresentation.GetLinks().GetUpdateLink() ?? throw new NoSuchPropertyRosiException("Missing update link in object");
        var json = (await HttpHelpers.Execute(link, options, pp)).Response;
        return new DomainObject(JObject.Parse(json), objectRepresentation.Options);
    }

    public static async Task<DomainObject> UpdateWithNamedParams(this DomainObject objectRepresentation, Dictionary<string, object> pp) =>
        await objectRepresentation.Update(pp.Cast<object>().ToArray());

    public static async Task<DomainObject> UpdateWithNamedParams(this DomainObject objectRepresentation, InvokeOptions options, Dictionary<string, object> pp) =>
        await objectRepresentation.Update(options, pp.Cast<object>().ToArray());

    public static async Task<TypeActionResult> IsSubtypeOf(this DomainObject objectRepresentation, object p) => await objectRepresentation.IsSubtypeOf(objectRepresentation.Options, p);

    public static async Task<TypeActionResult> IsSupertypeOf(this DomainObject objectRepresentation, object p) => await objectRepresentation.IsSupertypeOf(objectRepresentation.Options, p);

    public static async Task<TypeActionResult> IsSubtypeOf(this DomainObject objectRepresentation, InvokeOptions options, object p) {
        var link = objectRepresentation.GetLinks().GetIsSubtypeOfLink() ?? throw new NoSuchPropertyRosiException("Missing isSubtypeOf link in object");
        var json = (await HttpHelpers.Execute(link, options, p)).Response;
        return new TypeActionResult(JObject.Parse(json), objectRepresentation.Options);
    }

    public static async Task<TypeActionResult> IsSupertypeOf(this DomainObject objectRepresentation, InvokeOptions options, object p) {
        var link = objectRepresentation.GetLinks().GetIsSupertypeOfLink() ?? throw new NoSuchPropertyRosiException("Missing isSupertypeOf link in object");
        var json = (await HttpHelpers.Execute(link, options, p)).Response;
        return new TypeActionResult(JObject.Parse(json), objectRepresentation.Options);
    }

    public static T GetAsPoco<T>(this DomainObject objectRepresentation) where T : class, new() {
        var scalarProperties = objectRepresentation.GetPropertiesAndNames().Where(p => p.Item1.IsScalarProperty());
        return (T)scalarProperties.Aggregate(new T() as object, CopyProperty); // as object to box
    }

    private static object? Coerce(object? fromValue, Type toType) {
        if (fromValue is not null && toType != fromValue.GetType()) {
            // should log a warning ?
        }

        return fromValue switch {
            null => null,
            IConvertible c => c.ToType(toType, CultureInfo.InvariantCulture),
            TimeSpan t => t,
            _ => throw new NotSupportedException($"Unrecognized value: {fromValue.GetType()}")
        };
    }

    private static object CopyProperty(object toObject, (PropertyMember, string) fromPropertyTuple) {
        var (fromProperty, name) = fromPropertyTuple;
        var toProperty = toObject.GetType().GetProperty(name);
        if (toProperty is not null) {
            var propertyType = toProperty.PropertyType;
            if (propertyType.IsConstructedGenericType) {
                if (propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                    var toPropertyInnerType = propertyType.GenericTypeArguments.First();
                    var convertedFromValue = Coerce(fromProperty.ConvertValue(), toPropertyInnerType);
                    var toValue = convertedFromValue is null ? Activator.CreateInstance(propertyType) : Activator.CreateInstance(propertyType, convertedFromValue);
                    toProperty.SetValue(toObject, toValue);
                }
                else {
                    throw new NotSupportedException($"Unrecognized generic property type: {propertyType}");
                }
            }
            else {
                var convertedFromValue = Coerce(fromProperty.ConvertValue(), propertyType);
                toProperty.SetValue(toObject, convertedFromValue);
            }
        }

        return toObject;
    }
}