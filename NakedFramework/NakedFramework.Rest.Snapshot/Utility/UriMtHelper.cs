﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Interface;
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Snapshot.Constants;

namespace NakedFramework.Rest.Snapshot.Utility;

public class UriMtHelper {
    private static readonly Func<HttpRequest, string> GetAuthority;
    public static Func<HttpRequest, string> GetApplicationPath;
    private readonly IActionFacade action;
    private readonly IAssociationFacade assoc;
    private readonly string cachedId; // cache because may not be available at writing time 
    private readonly IObjectFacade objectFacade;
    private readonly IOidStrategy oidStrategy;
    private readonly IActionParameterFacade param;
    private readonly Uri prefix;
    private readonly ITypeFacade spec;
    private readonly string typeAction;

    // for testing to allow to be stubbed 
    static UriMtHelper() {
        GetAuthority = req => req.Host.ToUriComponent();
        GetApplicationPath = req => req.PathBase;
    }

    public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req) {
        this.oidStrategy = oidStrategy;
        prefix = new Uri(req.Scheme + "://" + GetAuthority(req));

        var applicationPath = GetApplicationPath(req);

        if (!string.IsNullOrEmpty(applicationPath)) {
            prefix = new Uri(prefix, applicationPath);
        }
    }

    public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req, IMenuFacade menuFacade) : this(oidStrategy, req) => CachedType = menuFacade.Id;

    public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req, IObjectFacade objectFacade) : this(oidStrategy, req) {
        this.objectFacade = objectFacade;
        spec = objectFacade.Specification;
        if (objectFacade.Specification.IsParseable) {
            throw new ArgumentException($"Cannot build URI  for parseable specification : {objectFacade.Specification.FullName}");
        }

        var oid = oidStrategy.OidTranslator.GetOidTranslation(objectFacade);
        cachedId = oid.InstanceId;
        CachedType = oid.DomainType;
    }

    public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req, IObjectFacade objectFacade, string instanceId) : this(oidStrategy, req) {
        this.objectFacade = objectFacade;
        spec = objectFacade.Specification;
        if (objectFacade.Specification.IsParseable) {
            throw new ArgumentException($"Cannot build URI  for parseable specification : {objectFacade.Specification.FullName}");
        }

        var oid = oidStrategy.OidTranslator.GetOidTranslation(objectFacade);
        cachedId = instanceId;
        CachedType = oid.DomainType;
    }

    public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req, PropertyContextFacade propertyContext)
        : this(oidStrategy, req) {
        assoc = propertyContext.Property;
        objectFacade = propertyContext.Target;
        spec = objectFacade.Specification;
        if (objectFacade.Specification.IsParseable) {
            throw new ArgumentException($"Cannot build URI  for parseable specification : {objectFacade.Specification.FullName}");
        }

        var oid = oidStrategy.OidTranslator.GetOidTranslation(objectFacade);
        cachedId = propertyContext.Target.IsTransient ? "" : oid.InstanceId;
        CachedType = oid.DomainType;
    }

    public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req, PropertyContextFacade propertyContext, string instanceId)
        : this(oidStrategy, req) {
        assoc = propertyContext.Property;
        objectFacade = propertyContext.Target;
        spec = objectFacade.Specification;
        if (objectFacade.Specification.IsParseable) {
            throw new ArgumentException($"Cannot build URI  for parseable specification : {objectFacade.Specification.FullName}");
        }

        var oid = oidStrategy.OidTranslator.GetOidTranslation(objectFacade);
        cachedId = instanceId;
        CachedType = oid.DomainType;
    }

    public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req, ActionContextFacade actionContext)
        : this(oidStrategy, req) {
        action = actionContext.Action;

        if (actionContext.Target is null) {
            spec = actionContext.Action.OnType;
            cachedId = "";
            CachedType = actionContext.MenuId ?? actionContext.Action.OnType.FullName;
        }
        else {
            objectFacade = actionContext.Target;
            spec = objectFacade.Specification;
            if (spec.IsParseable) {
                throw new ArgumentException($"Cannot build URI  for parseable specification : {objectFacade.Specification.FullName}");
            }

            var oid = oidStrategy.OidTranslator.GetOidTranslation(objectFacade);
            cachedId = oid.InstanceId;
            CachedType = oid.DomainType;
        }
    }

    public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req, ParameterContextFacade parameterContext)
        : this(oidStrategy, req) {
        action = parameterContext.Action;
        param = parameterContext.Parameter;

        if (parameterContext.Target is null) {
            spec = parameterContext.Action.OnType;
            cachedId = "";
            CachedType = parameterContext.MenuId;
        }
        else {
            objectFacade = parameterContext.Target;
            spec = objectFacade.Specification;
            if (objectFacade.Specification.IsParseable) {
                throw new ArgumentException($"Cannot build URI  for parseable specification : {objectFacade.Specification.FullName}");
            }

            var oid = oidStrategy.OidTranslator.GetOidTranslation(objectFacade);
            cachedId = oid.InstanceId;
            CachedType = oid.DomainType;
        }
    }

    public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req, ITypeFacade spec)
        : this(oidStrategy, req) {
        this.spec = spec;
        cachedId = "";
        CachedType = RestUtils.SpecToPredefinedTypeString(spec, oidStrategy);
    }

    public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req, IAssociationFacade assoc, IObjectFacade objectFacade)
        : this(oidStrategy, req) {
        var oid = oidStrategy.OidTranslator.GetOidTranslation(objectFacade);
        cachedId = oid.InstanceId;
        CachedType = oid.DomainType;
        this.assoc = assoc;
        spec = objectFacade.Specification;
    }

    public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req, TypeActionInvokeContext context)
        : this(oidStrategy, req) {
        typeAction = context.Id;
        cachedId = "";
        CachedType = oidStrategy.GetLinkDomainTypeBySpecification(context.ThisSpecification);
    }

    private string CachedType { get; }

    public static (string type, string key)? GetObjectId(string value) {
        var uri = new Uri(value);
        var path = uri.AbsolutePath;
        var pattern = $"/{SegmentValues.Objects}/([^/]+)/([^/]+)";

        var matches = Regex.Match(path, pattern);

        if (matches != Match.Empty && matches.Groups.Count == 3) {
            var objectType = matches.Groups[1].Value;
            var objectKey = matches.Groups[2].Value;

            return (objectType, objectKey);
        }

        return null;
    }

    public static string GetTypeId(string value) {
        var uri = new Uri(value);
        var path = uri.AbsolutePath;
        var pattern = $"/{SegmentValues.DomainTypes}/([^/]+)";

        var matches = Regex.Match(path, pattern);

        if (matches != Match.Empty && matches.Groups.Count == 2) {
            var typeId = matches.Groups[1].Value;
            return typeId;
        }

        return null;
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private static void CheckArgumentNotNull(string argument, string name, string allArgs = "") {
        if (string.IsNullOrEmpty(argument)) {
            throw new ArgumentException($"Cannot build URI : {name} is null or empty - other arguments: {allArgs}");
        }
    }

    private static void CheckArgumentNotNull(string argument, string name, params string[] allArguments) {
        CheckArgumentNotNull(argument, name, string.Join(", ", allArguments));
    }

    private static void CheckArgumentsNotNull(params (string argument, string name)[] arguments) {
        foreach (var (argument, name) in arguments) {
            CheckArgumentNotNull(argument, name, arguments.Select(t => t.argument).ToArray());
        }
    }

    private Uri BuildDomainTypeUri(string type) {
        CheckArgumentNotNull(type, "domain type");
        return new Uri($"{prefix}{SegmentValues.DomainTypes}/{type}");
    }

    public Uri GetDomainTypeUri() => BuildDomainTypeUri(CachedType);

    private Uri GetObjectParamUri() {
        CheckArgumentsNotNull((CachedType, "object type"), (action.Id, "action id"), (param.Id, "param id"));
        return new Uri($"{prefix}{SegmentValues.Objects}/{CachedType}/{cachedId}/{SegmentValues.Actions}/{action.Id}/{SegmentValues.Params}/{param.Id}");
    }

    private Uri GetServiceParamUri() {
        CheckArgumentsNotNull((CachedType, "object type"), (action.Id, "action id"), (param.Id, "param id"));
        return new Uri($"{prefix}{SegmentValues.Services}/{CachedType}/{SegmentValues.Actions}/{action.Id}/{SegmentValues.Params}/{param.Id}");
    }

    private Uri GetMenuParamUri() {
        CheckArgumentsNotNull((CachedType, "object type"), (action.Id, "action id"), (param.Id, "param id"));
        return new Uri($"{prefix}{SegmentValues.Menus}/{CachedType}/{SegmentValues.Actions}/{action.Id}/{SegmentValues.Params}/{param.Id}");
    }

    private Uri GetParamUri() =>
        spec.IsService
            ? GetServiceParamUri()
            : spec.IsStatic
                ? GetMenuParamUri()
                : GetObjectParamUri();

    public Uri GetTypeActionInvokeUri() {
        CheckArgumentsNotNull((CachedType, "domain type"), (typeAction, "type action"));
        return new Uri($"{prefix}{SegmentValues.DomainTypes}/{CachedType}/{SegmentValues.TypeActions}/{typeAction}/{SegmentValues.Invoke}");
    }

    public Uri GetObjectUri() {
        CheckArgumentsNotNull((CachedType, "object type"), (cachedId, "object key"));
        return new Uri($"{prefix}{SegmentValues.Objects}/{CachedType}/{cachedId}");
    }

    public Uri GetServiceUri() {
        CheckArgumentNotNull(CachedType, "service type");
        return new Uri($"{prefix}{SegmentValues.Services}/{CachedType}");
    }

    public Uri GetInvokeUri() =>
        spec.IsService || action?.IsQueryContributedAction == true
            ? GetServiceInvokeUri()
            : spec.IsStatic || action?.IsStaticObjectMenu == true
                ? GetMenuInvokeUri()
                : GetObjectInvokeUri();

    private Uri GetMenuInvokeUri() {
        CheckArgumentsNotNull((CachedType, "service type"), (action.Id, "action id"));
        return new Uri($"{prefix}{SegmentValues.Menus}/{CachedType}/{SegmentValues.Actions}/{action.Id}/{SegmentValues.Invoke}");
    }

    private Uri GetServiceInvokeUri() {
        CheckArgumentsNotNull((CachedType, "service type"), (action.Id, "action id"));
        return new Uri($"{prefix}{SegmentValues.Services}/{CachedType}/{SegmentValues.Actions}/{action.Id}/{SegmentValues.Invoke}");
    }

    private Uri GetObjectInvokeUri() {
        CheckArgumentsNotNull((CachedType, "object type"), (cachedId, "object key"), (action.Id, "action id"));
        return new Uri($"{prefix}{SegmentValues.Objects}/{CachedType}/{cachedId}/{SegmentValues.Actions}/{action.Id}/{SegmentValues.Invoke}");
    }

    public Uri GetHomeUri() => prefix;

    public Uri GetWellKnownUri(string name) {
        CheckArgumentNotNull(name, "well known name");
        return new Uri($"{prefix}{name}");
    }

    public Uri GetObjectsPersistUri() {
        CheckArgumentNotNull(CachedType, "object type");
        return new Uri($"{prefix}{SegmentValues.Objects}/{CachedType}");
    }

    public static Uri GetRedirectUri(HttpRequest req, string server, string oid) {
        CheckArgumentNotNull(oid, "object oid");
        var redirectPrefix = new Uri("http://" + server);
        return new Uri($"{redirectPrefix}{SegmentValues.Objects}/{oid}");
    }

    private Uri GetMenuMemberUri(IMemberFacade member, string memberType) {
        CheckArgumentsNotNull((CachedType, "menu type"), (memberType, "member type"), (member.Id, "member id"));
        return new Uri($"{prefix}{SegmentValues.Menus}/{CachedType}/{memberType}/{member.Id}");
    }

    private Uri GetServiceMemberUri(IMemberFacade member, string memberType) {
        CheckArgumentsNotNull((CachedType, "service type"), (memberType, "member type"), (member.Id, "member id"));
        return new Uri($"{prefix}{SegmentValues.Services}/{CachedType}/{memberType}/{member.Id}");
    }

    private Uri GetPersistentObjectMemberUri(IMemberFacade member, string memberType) {
        CheckArgumentsNotNull((CachedType, "object type"), (memberType, "member type"), (member.Id, "member id"));
        return new Uri($"{prefix}{SegmentValues.Objects}/{CachedType}/{cachedId}/{memberType}/{member.Id}");
    }

    private Uri GetTransientObjectMemberUri(IMemberFacade member, string memberType) {
        CheckArgumentsNotNull((CachedType, "object type"), (memberType, "member type"));
        return new Uri($"{prefix}{SegmentValues.Objects}/{CachedType}/{memberType}/{member.Id}");
    }

    private Uri GetObjectMemberUri(IMemberFacade member, string memberType) => string.IsNullOrEmpty(cachedId) ? GetTransientObjectMemberUri(member, memberType) : GetPersistentObjectMemberUri(member, memberType);

    private Uri GetMemberUri(IMemberFacade member, string memberType) =>
        spec.IsService || member is IActionFacade { IsQueryContributedAction: true }
            ? GetServiceMemberUri(member, memberType)
            : spec.IsStatic || member is IActionFacade { IsStaticObjectMenu: true }
                ? GetMenuMemberUri(member, memberType)
                : GetObjectMemberUri(member, memberType);

    private Uri ByMemberType(Func<IMemberFacade, string, Uri> getUri) {
        if (action is not null) {
            return getUri(action, SegmentValues.Actions);
        }

        if (assoc is { IsCollection: true }) {
            return getUri(assoc, SegmentValues.Collections);
        }

        return getUri(assoc, SegmentValues.Properties);
    }

    private Uri GetMemberUri() => param == null ? ByMemberType(GetMemberUri) : GetParamUri();

    public Uri GetPromptUri() {
        var memberUri = GetMemberUri();
        var builder = new UriBuilder(memberUri);
        builder.Path = builder.Path + "/" + SegmentValues.Prompt;
        return builder.Uri;
    }

    public Uri GetCollectionValueUri() {
        var memberUri = ByMemberType(GetMemberUri);
        var builder = new UriBuilder(memberUri);
        builder.Path = builder.Path + "/" + SegmentValues.CollectionValue;
        return builder.Uri;
    }

    public Uri GetDetailsUri() => ByMemberType(GetMemberUri);

    public static string GetInvokeMediaType() => RepresentationTypes.ActionResult;

    public static string GetActionResultMediaType() => RepresentationTypes.ActionResult;

    public string GetMemberMediaType() {
        if (action is not null) {
            return RepresentationTypes.ObjectAction;
        }

        if (assoc is { IsCollection: true }) {
            return RepresentationTypes.ObjectCollection;
        }

        return RepresentationTypes.ObjectProperty;
    }

    public MediaTypeHeaderValue GetAttachmentMediaType() {
        var no = assoc.GetValue(objectFacade);
        var attachment = no?.GetAttachment();
        var mtv = string.IsNullOrWhiteSpace(attachment?.MimeType) ? "" : attachment.MimeType;
        return new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(mtv) ? attachment.DefaultMimeType() : mtv);
    }

    public static MediaTypeHeaderValue GetJsonMediaType(string mt) {
        var profile = $"\"urn:org.restfulobjects:repr-types/{mt}\"";

        var mediaType = new MediaTypeHeaderValue("application/json");
        mediaType.Parameters.Add(new NameValueHeaderValue("profile", profile));
        mediaType.Parameters.Add(new NameValueHeaderValue("charset", "utf-8"));

        return mediaType;
    }

    public static string GetTypeActionMediaType() => RepresentationTypes.TypeActionResult;

    public Uri GetTypeActionsUri(string actionName) {
        CheckArgumentsNotNull((CachedType, "object type"), (actionName, "action name"));
        return new Uri($"{prefix}{SegmentValues.DomainTypes}/{CachedType}/{SegmentValues.TypeActions}/{actionName}/{SegmentValues.Invoke}");
    }

    public static string GetObjectMediaType() => RepresentationTypes.Object;

    public static string GetMenuMediaType() => RepresentationTypes.Menu;

    private static string GetParameterValue(RestControlFlags flags, string parameterValue) => parameterValue;

    private string GetParameterValue(RestControlFlags flags, ITypeFacade parameterValueSpec) => RestUtils.SpecToPredefinedTypeString(parameterValueSpec, oidStrategy);

    public void AddListRepresentationParameter(MediaTypeHeaderValue mediaType, RestControlFlags flags) {
        var specToUse = param == null ? spec : param.Specification;
        var typeName = specToUse == null ? typeof(object).FullName : specToUse.DomainTypeName(oidStrategy);
        var parameterValue = GetParameterValue(flags, typeName);
        mediaType.Parameters.Add(new NameValueHeaderValue(RestControlFlags.ElementTypeReserved, $"\"{parameterValue}\""));
    }

    public void AddObjectRepresentationParameter(MediaTypeHeaderValue mediaType, RestControlFlags flags) {
        var parameterValue = GetParameterValue(flags, spec);
        if (parameterValue != null) {
            mediaType.Parameters.Add(new NameValueHeaderValue(RestControlFlags.DomainTypeReserved, $"\"{parameterValue}\""));
        }
    }

    public void AddObjectCollectionRepresentationParameter(MediaTypeHeaderValue mediaType, RestControlFlags flags) {
        if (assoc is { IsCollection: true }) {
            var parameterValue = GetParameterValue(flags, assoc.ElementSpecification);
            if (parameterValue is not null) {
                mediaType.Parameters.Add(new NameValueHeaderValue(RestControlFlags.ElementTypeReserved, $"\"{parameterValue}\""));
            }
        }
    }

    public void AddActionResultRepresentationParameter(MediaTypeHeaderValue mediaType, RestControlFlags flags) {
        var resultSpec = action.ReturnType;
        var isCollection = resultSpec.IsCollection && !resultSpec.IsParseable;
        var parameterValueSpec = isCollection ? action.ElementType : resultSpec;
        var parameterValue = GetParameterValue(flags, parameterValueSpec);

        if (parameterValue is not null) {
            var parameterType = isCollection ? RestControlFlags.ElementTypeReserved : RestControlFlags.DomainTypeReserved;
            mediaType.Parameters.Add(new NameValueHeaderValue(parameterType, $"\"{parameterValue}\""));
        }
    }

    private static string FormatParameter(string resource, string name) => $";{resource}=\"{name}\"";

    public string GetRelParameters() => GetRelParametersFor((IMemberFacade)action ?? assoc);

    public string GetServiceRelParameter() => FormatParameter(RelParamValues.ServiceId, CachedType);

    public string GetRelParametersFor(IMemberFacade memberFacade) =>
        memberFacade switch {
            IActionFacade => FormatParameter(RelParamValues.Action, memberFacade.Id) + (param == null ? "" : FormatParameter(RelParamValues.Param, param.Id)),
            IAssociationFacade associationFacade => FormatParameter(associationFacade.IsCollection ? RelParamValues.Collection : RelParamValues.Property, associationFacade.Id),
            _ => throw new ArgumentException("Unexpected type:" + memberFacade.GetType())
        };

    public static string GetRelParametersFor(string actionId, string parmId) => FormatParameter(RelParamValues.Action, actionId) + FormatParameter(RelParamValues.Param, parmId);

    public string GetRelParametersFor(IActionParameterFacade actionParameterFacade) => GetRelParametersFor(actionParameterFacade.Action.Id, actionParameterFacade.Id);

    public static string GetRelParametersFor(string name) => FormatParameter(RelParamValues.TypeAction, name);

    public Uri GetMenuUri() {
        CheckArgumentNotNull(CachedType, "service type");
        return new Uri($"{prefix}{SegmentValues.Menus}/{CachedType}");
    }

    public object GetMenuRelParameter() => FormatParameter(RelParamValues.MenuId, CachedType);
}