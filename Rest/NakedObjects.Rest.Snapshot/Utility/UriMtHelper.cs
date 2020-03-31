// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Common.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Snapshot.Constants;

namespace NakedObjects.Rest.Snapshot.Utility {
    public class UriMtHelper {
        public static Func<HttpRequest, string> GetAuthority;
        public static Func<HttpRequest, string> GetApplicationPath;
        private static readonly ILog Logger = LogManager.GetLogger<UriMtHelper>();
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

            var oid = oidStrategy.FrameworkFacade.OidTranslator.GetOidTranslation(objectFacade);
            cachedId = oid.InstanceId;
            CachedType = oid.DomainType;
        }

        public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req, IObjectFacade objectFacade, string instanceId) : this(oidStrategy, req) {
            this.objectFacade = objectFacade;
            spec = objectFacade.Specification;
            if (objectFacade.Specification.IsParseable) {
                throw new ArgumentException($"Cannot build URI  for parseable specification : {objectFacade.Specification.FullName}");
            }

            var oid = oidStrategy.FrameworkFacade.OidTranslator.GetOidTranslation(objectFacade);
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

            var oid = oidStrategy.FrameworkFacade.OidTranslator.GetOidTranslation(objectFacade);
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

            var oid = oidStrategy.FrameworkFacade.OidTranslator.GetOidTranslation(objectFacade);
            cachedId = instanceId;
            CachedType = oid.DomainType;
        }

        public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req, ActionContextFacade actionContext)
            : this(oidStrategy, req) {
            action = actionContext.Action;
            objectFacade = actionContext.Target;
            spec = objectFacade.Specification;
            if (objectFacade.Specification.IsParseable) {
                throw new ArgumentException($"Cannot build URI  for parseable specification : {objectFacade.Specification.FullName}");
            }

            var oid = oidStrategy.FrameworkFacade.OidTranslator.GetOidTranslation(objectFacade);
            cachedId = oid.InstanceId;
            CachedType = oid.DomainType;
        }

        public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req, ParameterContextFacade parameterContext)
            : this(oidStrategy, req) {
            action = parameterContext.Action;
            param = parameterContext.Parameter;
            objectFacade = parameterContext.Target;
            spec = objectFacade.Specification;
            if (objectFacade.Specification.IsParseable) {
                throw new ArgumentException($"Cannot build URI  for parseable specification : {objectFacade.Specification.FullName}");
            }

            var oid = oidStrategy.FrameworkFacade.OidTranslator.GetOidTranslation(objectFacade);
            cachedId = oid.InstanceId;
            CachedType = oid.DomainType;
        }

        public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req, ITypeFacade spec)
            : this(oidStrategy, req) {
            this.spec = spec;
            cachedId = "";
            CachedType = RestUtils.SpecToPredefinedTypeString(spec, oidStrategy);
        }

        public UriMtHelper(IOidStrategy oidStrategy, HttpRequest req, IAssociationFacade assoc, IObjectFacade objectFacade)
            : this(oidStrategy, req) {
            var oid = oidStrategy.FrameworkFacade.OidTranslator.GetOidTranslation(objectFacade);
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

        public string[] GetObjectId(string value) {
            // todo this needs testing 

            var uri = new Uri(value);
            var path = uri.AbsolutePath;
            var pattern = $"/{SegmentValues.Objects}/([^/]+)/([^/]+)";

            var matches = Regex.Match(path, pattern);

            if (matches != Match.Empty && matches.Groups.Count == 3) {
                var objectType = matches.Groups[1].Value;
                var objectKey = matches.Groups[2].Value;

                return new[] {objectType, objectKey};
            }

            return null;

            //var template = new UriTemplate(SegmentValues.Objects + "/{objectType}/{objectKey}");
            //UriTemplateMatch match = template.Match(prefix, uri);

            //return match == null ? null : new[] {match.BoundVariables["objectType"], match.BoundVariables["objectKey"]};
        }

        public string GetTypeId(string value) {
            // todo this needs testing 

            var uri = new Uri(value);
            //var template = new UriTemplate(SegmentValues.DomainTypes + "/{typeName}");
            //UriTemplateMatch match = template.Match(prefix, uri);

            //return match == null ? null : match.BoundVariables["typeName"];

            var path = uri.AbsolutePath;
            var pattern = $"/{SegmentValues.DomainTypes}/([^/]+)";

            var matches = Regex.Match(path, pattern);

            if (matches != Match.Empty && matches.Groups.Count == 2) {
                var typeId = matches.Groups[1].Value;
                return typeId;
            }

            return null;
        }

        private static void CheckArgumentNotNull(string argument, string name) {
            if (string.IsNullOrEmpty(argument)) {
                throw new ArgumentException($"Cannot build URI : {name} is null or empty");
            }
        }

        private Uri BuildDomainTypeUri(string type) {
            CheckArgumentNotNull(type, "domain type");

            //var template = new UriTemplate(SegmentValues.DomainTypes + "/{type}");
            //return template.BindByPosition(prefix, type);

            return new Uri($"{prefix}{SegmentValues.DomainTypes}/{type}");
        }

        public Uri GetDomainTypeUri() => BuildDomainTypeUri(CachedType);

        public Uri GetParamTypeUri() {
            CheckArgumentNotNull(CachedType, "object type");
            CheckArgumentNotNull(action.Id, "action id");
            CheckArgumentNotNull(param.Id, "param id");

            //var template = new UriTemplate(SegmentValues.DomainTypes + "/{id}/" + SegmentValues.Actions + "/{action}/" + SegmentValues.Params + "/{paramId}");
            //return template.BindByPosition(prefix, CachedType, action.Id, param.Id);

            return new Uri($"{prefix}{SegmentValues.DomainTypes}/{CachedType}/{SegmentValues.Actions}/{action.Id}/{SegmentValues.Params}/{param.Id}");
        }

        public Uri GetObjectParamUri() {
            CheckArgumentNotNull(CachedType, "object type");
            CheckArgumentNotNull(action.Id, "action id");
            CheckArgumentNotNull(param.Id, "param id");

            //var template = new UriTemplate(SegmentValues.Objects + "/{typeId}/{instanceId}/" + SegmentValues.Actions + "/{action}/" + SegmentValues.Params + "/{paramId}");
            //return template.BindByPosition(prefix, CachedType, cachedId, action.Id, param.Id);

            return new Uri($"{prefix}{SegmentValues.Objects}/{CachedType}/{cachedId}/{SegmentValues.Actions}/{action.Id}/{SegmentValues.Params}/{param.Id}");
        }

        public Uri GetServiceParamUri() {
            CheckArgumentNotNull(CachedType, "object type");
            CheckArgumentNotNull(action.Id, "action id");
            CheckArgumentNotNull(param.Id, "param id");

            //var template = new UriTemplate(SegmentValues.Services + "/{oid}/" + SegmentValues.Actions + "/{action}/" + SegmentValues.Params + "/{paramId}");
            //return template.BindByPosition(prefix, CachedType, action.Id, param.Id);

            return new Uri($"{prefix}{SegmentValues.Services}/{CachedType}/{SegmentValues.Actions}/{action.Id}/{SegmentValues.Params}/{param.Id}");
        }

        public Uri GetParamUri() => spec.IsService ? GetServiceParamUri() : GetObjectParamUri();

        public Uri GetTypeActionInvokeUri() {
            CheckArgumentNotNull(CachedType, "domain type");
            CheckArgumentNotNull(typeAction, "type action");

            //var template = new UriTemplate(SegmentValues.DomainTypes + "/{id}/" + SegmentValues.TypeActions + "/{action}/" + SegmentValues.Invoke);
            //return template.BindByPosition(prefix, CachedType, typeAction);

            return new Uri($"{prefix}{SegmentValues.DomainTypes}/{CachedType}/{SegmentValues.TypeActions}/{typeAction}/{SegmentValues.Invoke}");
        }

        public Uri GetObjectUri() {
            CheckArgumentNotNull(CachedType, "object type");
            CheckArgumentNotNull(cachedId, "object key");

            //var template = new UriTemplate(SegmentValues.Objects + "/{typeId}/{instanceId}");
            //return template.BindByPosition(prefix, CachedType, cachedId);

            return new Uri($"{prefix}{SegmentValues.Objects}/{CachedType}/{cachedId}");
        }

        public Uri GetServiceUri() {
            CheckArgumentNotNull(CachedType, "service type");

            //var template = new UriTemplate(SegmentValues.Services + "/{oid}");
            //return template.BindByPosition(prefix, CachedType);

            return new Uri($"{prefix}{SegmentValues.Services}/{CachedType}");
        }

        public Uri GetIconUri() {
            //var template = new UriTemplate(SegmentValues.Images + "/{image}");
            var name = spec.GetIconName(objectFacade);
            var iconName = name.Contains(".") ? name : name + ".gif";
            CheckArgumentNotNull(iconName, "icon name");
            //return template.BindByPosition(prefix, iconName);

            return new Uri($"{prefix}{SegmentValues.Images}/{iconName}");
        }

        //if (action.IsQueryOnly) {
        //    return GetQueryInvokeUri();
        //}
        //if (action.IsIdempotent) {
        //    return GetIdempotentUri();
        //}
        //return GetNonIdempotentUri();
        public Uri GetInvokeUri() => spec.IsService ? GetServiceInvokeUri() : GetObjectInvokeUri();

        private Uri GetServiceInvokeUri() {
            CheckArgumentNotNull(CachedType, "service type");
            CheckArgumentNotNull(action.Id, "action id");

            //var template = new UriTemplate(SegmentValues.Services + "/{oid}/" + SegmentValues.Actions + "/{action}/" + SegmentValues.Invoke + queryString);
            //return template.BindByPosition(prefix, CachedType, action.Id);

            return new Uri($"{prefix}{SegmentValues.Services}/{CachedType}/{SegmentValues.Actions}/{action.Id}/{SegmentValues.Invoke}");
        }

        private Uri GetObjectInvokeUri() {
            CheckArgumentNotNull(CachedType, "object type");
            CheckArgumentNotNull(cachedId, "object key");
            CheckArgumentNotNull(action.Id, "action id");

            //var template = new UriTemplate(SegmentValues.Objects + "/{objectType}/{objectKey}/" + SegmentValues.Actions + "/{action}/" + SegmentValues.Invoke + queryString);
            //return template.BindByPosition(prefix, CachedType, cachedId, action.Id);

            return new Uri($"{prefix}{SegmentValues.Objects}/{CachedType}/{cachedId}/{SegmentValues.Actions}/{action.Id}/{SegmentValues.Invoke}");
        }

        //private Uri GetInvokeUri() {
        //    return spec.IsService ? GetServiceInvokeUri() : GetObjectInvokeUri();
        //}

        //private Uri GetNonIdempotentUri() {
        //    return GetInvokeUri();
        //}

        //private Uri GetIdempotentUri() {
        //    return GetInvokeUri();
        //}

        //private Uri GetQueryInvokeUri() {
        //    return GetInvokeUri();
        //}

        public Uri GetHomeUri() => prefix;

        public Uri GetWellKnownUri(string name) {
            CheckArgumentNotNull(name, "well known name");

            //var template = new UriTemplate("{fixed}");
            //return template.BindByPosition(prefix, name);
            return new Uri($"{prefix}{name}");
        }

        public Uri GetObjectsPersistUri() {
            CheckArgumentNotNull(CachedType, "object type");

            //var template = new UriTemplate(SegmentValues.Objects + "/{objectType}");
            //return template.BindByPosition(prefix, CachedType);

            return new Uri($"{prefix}{SegmentValues.Objects}/{CachedType}");
        }

        public Uri GetRedirectUri(HttpRequest req, string server, string oid) {
            CheckArgumentNotNull(oid, "object oid");
            var redirectPrefix = new Uri("http://" + server);
            //var template = new UriTemplate("objects/{oid}");
            //return template.BindByPosition(redirectPrefix, oid);

            return new Uri($"{redirectPrefix}{SegmentValues.Objects}/{oid}");
        }

        private Uri GetServiceMemberUri(IMemberFacade member, string memberType) {
            CheckArgumentNotNull(CachedType, "service type");
            CheckArgumentNotNull(memberType, "member type");
            CheckArgumentNotNull(member.Id, "member id");

            //var template = new UriTemplate(SegmentValues.Services + "/{id}/{memberType}/{memberId}");
            //return template.BindByPosition(prefix, CachedType, memberType, member.Id);

            return new Uri($"{prefix}{SegmentValues.Services}/{CachedType}/{memberType}/{member.Id}");
        }

        private Uri GetTypeMemberUri(IMemberFacade member, string memberType) {
            CheckArgumentNotNull(CachedType, "domain type");
            CheckArgumentNotNull(memberType, "member type");
            CheckArgumentNotNull(member.Id, "member id");

            //var template = new UriTemplate(SegmentValues.DomainTypes + "/{objectType}/{memberType}/{memberId}");
            //return template.BindByPosition(prefix, CachedType, memberType, member.Id);

            return new Uri($"{prefix}{SegmentValues.DomainTypes}/{CachedType}/{memberType}/{member.Id}");
        }

        private Uri GetPersistentObjectMemberUri(IMemberFacade member, string memberType) {
            CheckArgumentNotNull(CachedType, "object type");
            CheckArgumentNotNull(memberType, "member type");
            CheckArgumentNotNull(member.Id, "member id");

            //var template = new UriTemplate(SegmentValues.Objects + "/{objectType}/{objectId}/{memberType}/{memberId}");
            //return template.BindByPosition(prefix, CachedType, cachedId, memberType, member.Id);

            return new Uri($"{prefix}{SegmentValues.Objects}/{CachedType}/{cachedId}/{memberType}/{member.Id}");
        }

        private Uri GetTransientObjectMemberUri(IMemberFacade member, string memberType) {
            CheckArgumentNotNull(CachedType, "object type");
            CheckArgumentNotNull(memberType, "member type");

            //var template = new UriTemplate(SegmentValues.Objects + "/{objectType}/{memberType}/{memberId}");
            //return template.BindByPosition(prefix, CachedType, memberType, member.Id);

            return new Uri($"{prefix}{SegmentValues.Objects}/{CachedType}/{memberType}/{member.Id}");
        }

        private Uri GetObjectMemberUri(IMemberFacade member, string memberType) => string.IsNullOrEmpty(cachedId) ? GetTransientObjectMemberUri(member, memberType) : GetPersistentObjectMemberUri(member, memberType);

        private Uri GetMemberUri(IMemberFacade member, string memberType) => spec.IsService ? GetServiceMemberUri(member, memberType) : GetObjectMemberUri(member, memberType);

        private Uri ByMemberType(Func<IMemberFacade, string, Uri> getUri) {
            if (action != null) {
                return getUri(action, SegmentValues.Actions);
            }

            if (assoc != null && assoc.IsCollection) {
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

        public Uri GetTypeDetailsUri() => ByMemberType(GetTypeMemberUri);

        public string GetInvokeMediaType() => RepresentationTypes.ActionResult;

        public string GetActionResultMediaType() => RepresentationTypes.ActionResult;

        public string GetMemberMediaType() {
            if (action != null) {
                return RepresentationTypes.ObjectAction;
            }

            if (assoc != null && assoc.IsCollection) {
                return RepresentationTypes.ObjectCollection;
            }

            return RepresentationTypes.ObjectProperty;
        }

        public string GetTypeMemberMediaType() {
            if (action != null) {
                return RepresentationTypes.ActionDescription;
            }

            if (assoc != null && assoc.IsCollection) {
                return RepresentationTypes.CollectionDescription;
            }

            return RepresentationTypes.PropertyDescription;
        }

        public MediaTypeHeaderValue GetAttachmentMediaType() {
            var no = assoc.GetValue(objectFacade);
            var attachment = no?.GetAttachment();
            var mtv = string.IsNullOrWhiteSpace(attachment?.MimeType) ? "" : attachment.MimeType;
            return new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(mtv) ? attachment.DefaultMimeType() : mtv);
        }

        public MediaTypeHeaderValue GetIconMediaType() {
            var name = spec.GetIconName(objectFacade);
            var mt = name.Contains(".") ? name.Split('.').Last() : "gif";
            var mtv = $"image/{mt}";

            return new MediaTypeHeaderValue(mtv);
        }

        public static MediaTypeHeaderValue GetJsonMediaType(string mt) {
            var profile = $"\"urn:org.restfulobjects:repr-types/{mt}\"";

            var mediaType = new MediaTypeHeaderValue("application/json");
            mediaType.Parameters.Add(new NameValueHeaderValue("profile", profile));
            mediaType.Parameters.Add(new NameValueHeaderValue("charset", "utf-8"));

            return mediaType;
        }

        public string GetTypeActionMediaType() => RepresentationTypes.TypeActionResult;

        public Uri GetTypeActionsUri(string actionName) {
            CheckArgumentNotNull(CachedType, "object type");
            CheckArgumentNotNull(actionName, "action name");

            //var template = new UriTemplate(SegmentValues.DomainTypes + "/{class}/" + SegmentValues.TypeActions + "/{action}/" + SegmentValues.Invoke);
            //return template.BindByPosition(prefix, CachedType, actionName);

            return new Uri($"{prefix}{SegmentValues.DomainTypes}/{CachedType}/{SegmentValues.TypeActions}/{actionName}/{SegmentValues.Invoke}");
        }

        public string GetObjectMediaType() => RepresentationTypes.Object;

        public string GetMenuMediaType() => RepresentationTypes.Menu;

        private string GetParameterValue(RestControlFlags flags, string parameterValue) => parameterValue;

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
            if (assoc != null && assoc.IsCollection) {
                var parameterValue = GetParameterValue(flags, assoc.ElementSpecification);
                if (parameterValue != null) {
                    mediaType.Parameters.Add(new NameValueHeaderValue(RestControlFlags.ElementTypeReserved, $"\"{parameterValue}\""));
                }
            }
        }

        public void AddActionResultRepresentationParameter(MediaTypeHeaderValue mediaType, RestControlFlags flags) {
            var resultSpec = action.ReturnType;
            var isCollection = resultSpec.IsCollection && !resultSpec.IsParseable;
            var parameterValueSpec = isCollection ? action.ElementType : resultSpec;
            var parameterValue = GetParameterValue(flags, parameterValueSpec);

            if (parameterValue != null) {
                var parameterType = isCollection ? RestControlFlags.ElementTypeReserved : RestControlFlags.DomainTypeReserved;
                mediaType.Parameters.Add(new NameValueHeaderValue(parameterType, $"\"{parameterValue}\""));
            }
        }

        public string FormatParameter(string resource, string name) => $";{resource}=\"{name}\"";

        public string GetRelParameters() => GetRelParametersFor((IMemberFacade) action ?? assoc);

        public string GetServiceRelParameter() => FormatParameter(RelParamValues.ServiceId, CachedType);

        public string GetRelParametersFor(IMemberFacade memberFacade) {
            if (memberFacade is IActionFacade) {
                return FormatParameter(RelParamValues.Action, memberFacade.Id) + (param == null ? "" : FormatParameter(RelParamValues.Param, param.Id));
            }

            if (memberFacade is IAssociationFacade associationFacade) {
                return FormatParameter(associationFacade.IsCollection ? RelParamValues.Collection : RelParamValues.Property, associationFacade.Id);
            }

            throw new ArgumentException("Unexpected type:" + memberFacade.GetType());
        }

        public string GetRelParametersFor(string actionId, string parmId) => FormatParameter(RelParamValues.Action, actionId) + FormatParameter(RelParamValues.Param, parmId);

        public string GetRelParametersFor(IActionParameterFacade actionParameterFacade) => GetRelParametersFor(actionParameterFacade.Action.Id, actionParameterFacade.Id);

        public string GetRelParametersFor(string name) => FormatParameter(RelParamValues.TypeAction, name);

        public Uri GetMenuUri() {
            CheckArgumentNotNull(CachedType, "service type");

            //var template = new UriTemplate(SegmentValues.Menus + "/{oid}");
            //return template.BindByPosition(prefix, CachedType);

            return new Uri($"{prefix}{SegmentValues.Menus}/{CachedType}");
        }

        public object GetMenuRelParameter() => FormatParameter(RelParamValues.MenuId, CachedType);
    }
}