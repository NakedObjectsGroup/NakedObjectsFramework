// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Common.Logging;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class UriMtHelper {
        public static Func<HttpRequestMessage, string> GetAuthority;
        public static Func<string> GetApplicationPath;
        private static readonly ILog Logger = LogManager.GetLogger<UriMtHelper>();
        private readonly INakedObjectActionSurface action;
        private readonly INakedObjectAssociationSurface assoc;
        private readonly string cachedId; // cache because may not be available at writing time 
        private readonly string cachedType; // cache because may not be available at writing time 
        private readonly INakedObjectSurface nakedObject;
        private readonly INakedObjectActionParameterSurface param;
        private readonly Uri prefix;
        private readonly INakedObjectSpecificationSurface spec;
        private readonly string typeAction;

        // for testing to allow to be stubbed 
        static UriMtHelper() {
            GetAuthority = req => req.RequestUri.Authority;
            GetApplicationPath = () => HttpContext.Current.Request.ApplicationPath;
        }


        public UriMtHelper(HttpRequestMessage req) {
            prefix = new Uri(req.RequestUri.Scheme + "://" + GetAuthority(req));

            string applicationPath = GetApplicationPath();

            if (!string.IsNullOrEmpty(applicationPath)) {
                prefix = new Uri(prefix, applicationPath);
            }

            DebugLogRequest(req);
        }

        public UriMtHelper(HttpRequestMessage req, INakedObjectSurface nakedObject, IOidStrategy oidStrategy) : this(req) {
            this.nakedObject = nakedObject;
            spec = nakedObject.Specification;
            LinkObjectId oid = oidStrategy.GetOid(nakedObject);
            cachedId = oid.InstanceId;
            cachedType = oid.DomainType;
        }

        public UriMtHelper(HttpRequestMessage req, PropertyContextSurface propertyContext, IOidStrategy oidStrategy)
            : this(req) {
            assoc = propertyContext.Property;
            nakedObject = propertyContext.Target;
            spec = nakedObject.Specification;
            LinkObjectId oid = oidStrategy.GetOid(nakedObject);
            cachedId = oid.InstanceId;
            cachedType = oid.DomainType;
        }

        public UriMtHelper(HttpRequestMessage req, PropertyTypeContextSurface propertyContext, IOidStrategy oidStrategy)
            : this(req) {
            assoc = propertyContext.Property;
            spec = propertyContext.OwningSpecification;
            cachedId = "";
            cachedType = spec.DomainTypeName(oidStrategy);
        }

        public UriMtHelper(HttpRequestMessage req, ActionContextSurface actionContext, IOidStrategy oidStrategy)
            : this(req) {
            action = actionContext.Action;
            nakedObject = actionContext.Target;
            spec = nakedObject.Specification;
            LinkObjectId oid = oidStrategy.GetOid(nakedObject);
            cachedId = oid.InstanceId;
            cachedType = oid.DomainType;
        }

        public UriMtHelper(HttpRequestMessage req, ActionTypeContextSurface actionTypeContext, IOidStrategy oidStrategy)
            : this(req) {
            action = actionTypeContext.ActionContext.Action;
            spec = actionTypeContext.OwningSpecification;
            cachedId = "";
            cachedType = spec.DomainTypeName(oidStrategy);
        }

        public UriMtHelper(HttpRequestMessage req, ParameterTypeContextSurface parameterTypeContext, IOidStrategy oidStrategy)
            : this(req) {
            action = parameterTypeContext.Action;
            spec = parameterTypeContext.OwningSpecification;
            param = parameterTypeContext.Parameter;
            cachedId = "";
            cachedType = spec.DomainTypeName(oidStrategy);
        }

        public UriMtHelper(HttpRequestMessage req, ParameterContextSurface parameterContext, IOidStrategy oidStrategy)
            : this(req) {
            action = parameterContext.Action;
            param = parameterContext.Parameter;
            nakedObject = parameterContext.Target;
            spec = nakedObject.Specification;
            LinkObjectId oid = oidStrategy.GetOid(nakedObject);
            cachedId = oid.InstanceId;
            cachedType = oid.DomainType;
        }

        public UriMtHelper(HttpRequestMessage req, INakedObjectSpecificationSurface spec, IOidStrategy oidStrategy)
            : this(req) {
            this.spec = spec;
            cachedId = "";
            cachedType = RestUtils.SpecToPredefinedTypeString(spec, oidStrategy);
        }

        public UriMtHelper(HttpRequestMessage req, INakedObjectAssociationSurface assoc, IOidStrategy oidStrategy)
            : this(req) {
            cachedId = "";
            if (assoc.IsCollection()) {
                cachedType = assoc.IsASet() ? PredefinedType.Set.ToRoString() : PredefinedType.List.ToRoString();
            }
            else {
                cachedType = assoc.Specification.DomainTypeName(oidStrategy);
            }
        }

        public UriMtHelper(HttpRequestMessage req, TypeActionInvokeContext context, IOidStrategy oidStrategy)
            : this(req) {
            typeAction = context.Id;
            cachedId = "";
            cachedType = oidStrategy.GetLinkDomainTypeBySpecification(context.ThisSpecification);
        }

        private static void DebugLogRequest(HttpRequestMessage req) {
            Logger.DebugFormat("AbsolutePath {0}", req.RequestUri.AbsolutePath);
            Logger.DebugFormat("AbsoluteUri {0}", req.RequestUri.AbsoluteUri);
            Logger.DebugFormat("Authority {0}", req.RequestUri.Authority);
            Logger.DebugFormat("DnsSafeHost {0}", req.RequestUri.DnsSafeHost);
            Logger.DebugFormat("Fragment {0}", req.RequestUri.Fragment);
            Logger.DebugFormat("Host {0}", req.RequestUri.Host);

            Logger.DebugFormat("HostNameType {0}", req.RequestUri.HostNameType);
            Logger.DebugFormat("IsAbsoluteUri {0}", req.RequestUri.IsAbsoluteUri);

            Logger.DebugFormat("IsDefaultPort {0}", req.RequestUri.IsDefaultPort);
            Logger.DebugFormat("IsFile {0}", req.RequestUri.IsFile);
            Logger.DebugFormat("IsLoopback {0}", req.RequestUri.IsLoopback);
            Logger.DebugFormat("IsUnc {0}", req.RequestUri.IsUnc);
            Logger.DebugFormat("LocalPath {0}", req.RequestUri.LocalPath);
            Logger.DebugFormat("OriginalString {0}", req.RequestUri.OriginalString);

            Logger.DebugFormat("PathAndQuery {0}", req.RequestUri.PathAndQuery);
            Logger.DebugFormat("Port {0}", req.RequestUri.Port);
            Logger.DebugFormat("Query {0}", req.RequestUri.Query);
            Logger.DebugFormat("Scheme {0}", req.RequestUri.Scheme);
            Logger.DebugFormat("UserEscaped {0}", req.RequestUri.UserEscaped);
            Logger.DebugFormat("UserInfo {0}", req.RequestUri.UserInfo);
        }

        public string[] GetObjectId(string value) {
            var uri = new Uri(value);
            var template = new UriTemplate(SegmentValues.Objects + "/{objectType}/{objectKey}");
            UriTemplateMatch match = template.Match(prefix, uri);

            return match == null ? null : new[] {match.BoundVariables["objectType"], match.BoundVariables["objectKey"]};
        }

        public string GetTypeId(string value) {
            var uri = new Uri(value);
            var template = new UriTemplate(SegmentValues.DomainTypes + "/{typeName}");
            UriTemplateMatch match = template.Match(prefix, uri);

            return match == null ? null : match.BoundVariables["typeName"];
        }

        private static void CheckArgumentNotNull(string argument, string name) {
            if (string.IsNullOrEmpty(argument)) {
                throw new ArgumentException(string.Format("Cannot build URI : {0} is null or empty", name));
            }
        }

        private Uri BuildDomainTypeUri(string type) {
            CheckArgumentNotNull(type, "domain type");

            var template = new UriTemplate(SegmentValues.DomainTypes + "/{type}");
            return template.BindByPosition(prefix, type);
        }

        public Uri GetDomainTypeUri() {
            return BuildDomainTypeUri(cachedType);
        }


        public Uri GetParamTypeUri() {
            CheckArgumentNotNull(cachedType, "object type");
            CheckArgumentNotNull(action.Id, "action id");
            CheckArgumentNotNull(param.Id, "param id");

            var template = new UriTemplate(SegmentValues.DomainTypes + "/{id}/" + SegmentValues.Actions + "/{action}/" + SegmentValues.Params + "/{paramId}");
            return template.BindByPosition(prefix, cachedType, action.Id, param.Id);
        }

        public Uri GetObjectParamUri() {
            CheckArgumentNotNull(cachedType, "object type");
            CheckArgumentNotNull(action.Id, "action id");
            CheckArgumentNotNull(param.Id, "param id");

            var template = new UriTemplate(SegmentValues.Objects + "/{typeId}/{instanceId}/" + SegmentValues.Actions + "/{action}/" + SegmentValues.Params + "/{paramId}");
            return template.BindByPosition(prefix, cachedType, cachedId, action.Id, param.Id);
        }

        public Uri GetServiceParamUri() {
            CheckArgumentNotNull(cachedType, "object type");
            CheckArgumentNotNull(action.Id, "action id");
            CheckArgumentNotNull(param.Id, "param id");

            var template = new UriTemplate(SegmentValues.Services + "/{oid}/" + SegmentValues.Actions + "/{action}/" + SegmentValues.Params + "/{paramId}");
            return template.BindByPosition(prefix, cachedType, action.Id, param.Id);
        }

        public Uri GetParamUri() {
            return spec.IsService() ? GetServiceParamUri() : GetObjectParamUri();
        }

        public Uri GetTypeActionInvokeUri() {
            CheckArgumentNotNull(cachedType, "domain type");
            CheckArgumentNotNull(typeAction, "type action");

            var template = new UriTemplate(SegmentValues.DomainTypes + "/{id}/" + SegmentValues.TypeActions + "/{action}/" + SegmentValues.Invoke);
            return template.BindByPosition(prefix, cachedType, typeAction);
        }

        public Uri GetObjectUri() {
            CheckArgumentNotNull(cachedType, "object type");
            CheckArgumentNotNull(cachedId, "object key");

            var template = new UriTemplate(SegmentValues.Objects + "/{typeId}/{instanceId}");
            return template.BindByPosition(prefix, cachedType, cachedId);
        }

        public Uri GetServiceUri() {
            CheckArgumentNotNull(cachedType, "service type");

            var template = new UriTemplate(SegmentValues.Services + "/{oid}");
            return template.BindByPosition(prefix, cachedType);
        }


        public Uri GetIconUri() {
            var template = new UriTemplate(SegmentValues.Images + "/{image}");
            string name = spec.GetIconName(nakedObject);
            string iconName = name.Contains(".") ? name : name + ".gif";
            CheckArgumentNotNull(iconName, "icon name");
            return template.BindByPosition(prefix, iconName);
        }


        public Uri GetInvokeUri() {
            if (action.IsQueryOnly()) {
                return GetQueryInvokeUri();
            }
            if (action.IsIdempotent()) {
                return GetIdempotentUri();
            }
            return GetNonIdempotentUri();
        }

        private Uri GetServiceInvokeUri(string queryString) {
            CheckArgumentNotNull(cachedType, "service type");
            CheckArgumentNotNull(action.Id, "action id");

            var template = new UriTemplate(SegmentValues.Services + "/{oid}/" + SegmentValues.Actions + "/{action}/" + SegmentValues.Invoke + queryString);
            return template.BindByPosition(prefix, cachedType, action.Id);
        }

        private Uri GetObjectInvokeUri(string queryString) {
            CheckArgumentNotNull(cachedType, "object type");
            CheckArgumentNotNull(cachedId, "object key");
            CheckArgumentNotNull(action.Id, "action id");

            var template = new UriTemplate(SegmentValues.Objects + "/{objectType}/{objectKey}/" + SegmentValues.Actions + "/{action}/" + SegmentValues.Invoke + queryString);
            return template.BindByPosition(prefix, cachedType, cachedId, action.Id);
        }

        private Uri GetInvokeUri(string queryString) {
            return spec.IsService() ? GetServiceInvokeUri(queryString) : GetObjectInvokeUri(queryString);
        }

        private Uri GetNonIdempotentUri() {
            return GetInvokeUri("");
        }

        private Uri GetIdempotentUri() {
            return GetInvokeUri("");
        }

        private Uri GetQueryInvokeUri() {
            if (action.ParameterCount == 0) {
                return GetInvokeUri("");
            }
            return GetInvokeUri(action.Parameters.Aggregate("?", (s, t) => s + t.Id + "={" + t.Id + "}&").TrimEnd('&'));
        }


        public Uri GetHomeUri() {
            return prefix;
        }

        public Uri GetWellKnownUri(string name) {
            CheckArgumentNotNull(name, "well known name");

            var template = new UriTemplate("{fixed}");
            return template.BindByPosition(prefix, name);
        }

        public Uri GetObjectsPersistUri() {
            CheckArgumentNotNull(cachedType, "object type");

            var template = new UriTemplate(SegmentValues.Objects + "/{objectType}");
            return template.BindByPosition(prefix, cachedType);
        }

        public Uri GetRedirectUri(HttpRequestMessage req, string server, string oid) {
            CheckArgumentNotNull(oid, "object oid");

            var redirectPrefix = new Uri("http://" + server);
            var template = new UriTemplate("objects/{oid}");
            return template.BindByPosition(redirectPrefix, oid);
        }

        private Uri GetServiceMemberUri(INakedObjectMemberSurface member, string memberType) {
            CheckArgumentNotNull(cachedType, "service type");
            CheckArgumentNotNull(memberType, "member type");
            CheckArgumentNotNull(member.Id, "member id");


            var template = new UriTemplate(SegmentValues.Services + "/{id}/{memberType}/{memberId}");
            return template.BindByPosition(prefix, cachedType, memberType, member.Id);
        }

        private Uri GetTypeMemberUri(INakedObjectMemberSurface member, string memberType) {
            CheckArgumentNotNull(cachedType, "domain type");
            CheckArgumentNotNull(memberType, "member type");
            CheckArgumentNotNull(member.Id, "member id");


            var template = new UriTemplate(SegmentValues.DomainTypes + "/{objectType}/{memberType}/{memberId}");
            return template.BindByPosition(prefix, cachedType, memberType, member.Id);
        }

        private Uri GetObjectMemberUri(INakedObjectMemberSurface member, string memberType) {
            CheckArgumentNotNull(cachedType, "object type");
            CheckArgumentNotNull(memberType, "member type");
            CheckArgumentNotNull(member.Id, "member id");


            var template = new UriTemplate(SegmentValues.Objects + "/{objectType}/{objectId}/{memberType}/{memberId}");
            return template.BindByPosition(prefix, cachedType, cachedId, memberType, member.Id);
        }

        private Uri GetMemberUri(INakedObjectMemberSurface member, string memberType) {
            return spec.IsService() ? GetServiceMemberUri(member, memberType) : GetObjectMemberUri(member, memberType);
        }

        private Uri ByMemberType(Func<INakedObjectMemberSurface, string, Uri> getUri) {
            if (action != null) {
                return getUri(action, SegmentValues.Actions);
            }

            if (assoc != null && assoc.IsCollection()) {
                return getUri(assoc, SegmentValues.Collections);
            }

            return getUri(assoc, SegmentValues.Properties);
        }

        public Uri GetPromptUri() {
            Uri memberUri = param == null ? ByMemberType(GetMemberUri) : GetParamUri();
            var builder = new UriBuilder(memberUri);
            builder.Path = builder.Path + "/" + SegmentValues.Prompt;
            return builder.Uri;
        }

        public Uri GetCollectionValueUri() {
            Uri memberUri = ByMemberType(GetMemberUri);
            var builder = new UriBuilder(memberUri);
            builder.Path = builder.Path + "/" + SegmentValues.CollectionValue;
            return builder.Uri;
        }


        public Uri GetDetailsUri() {
            return ByMemberType(GetMemberUri);
        }

        public Uri GetTypeDetailsUri() {
            return ByMemberType(GetTypeMemberUri);
        }

        public string GetInvokeMediaType() {
            return RepresentationTypes.ActionResult;
        }

        public string GetActionResultMediaType() {
            return RepresentationTypes.ActionResult;
        }

        public string GetMemberMediaType() {
            if (action != null) {
                return RepresentationTypes.ObjectAction;
            }
            if (assoc != null && assoc.IsCollection()) {
                return RepresentationTypes.ObjectCollection;
            }
            return RepresentationTypes.ObjectProperty;
        }

        public string GetTypeMemberMediaType() {
            if (action != null) {
                return RepresentationTypes.ActionDescription;
            }
            if (assoc != null && assoc.IsCollection()) {
                return RepresentationTypes.CollectionDescription;
            }
            return RepresentationTypes.PropertyDescription;
        }

        public MediaTypeHeaderValue GetAttachmentMediaType() {
            INakedObjectSurface no = assoc.GetNakedObject(nakedObject);
            string mtv = no != null ? no.GetAttachment().MimeType : "";
            return new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(mtv) ? "image/bmp" : mtv);
        }

        public MediaTypeHeaderValue GetIconMediaType() {
            string name = spec.GetIconName(nakedObject);
            string mt = name.Contains(".") ? name.Split('.').Last() : "gif";
            string mtv = string.Format("image/{0}", mt);

            return new MediaTypeHeaderValue(mtv);
        }

        public static MediaTypeHeaderValue GetJsonMediaType(string mt) {
            string profile = string.Format("\"urn:org.restfulobjects:repr-types/{0}\"", mt);

            var mediaType = new MediaTypeHeaderValue("application/json");
            mediaType.Parameters.Add(new NameValueHeaderValue("profile", profile));
            mediaType.Parameters.Add(new NameValueHeaderValue("charset", "utf-8"));

            return mediaType;
        }

        public string GetTypeActionMediaType() {
            return RepresentationTypes.TypeActionResult;
        }

        public Uri GetTypeActionsUri(string actionName) {
            CheckArgumentNotNull(cachedType, "object type");
            CheckArgumentNotNull(actionName, "action name");

            var template = new UriTemplate(SegmentValues.DomainTypes + "/{class}/" + SegmentValues.TypeActions + "/{action}/" + SegmentValues.Invoke);
            return template.BindByPosition(prefix, cachedType, actionName);
        }

        public string GetObjectMediaType() {
            return RepresentationTypes.Object;
        }

        private string GetParameterValue(RestControlFlags flags, string parameterValue) {
            if (flags.SimpleDomainModel) {
                return parameterValue;
            }
            if (flags.FormalDomainModel) {
                return BuildDomainTypeUri(parameterValue).ToString();
            }
            return null;
        }


        private string GetParameterValue(RestControlFlags flags, INakedObjectSpecificationSurface parameterValueSpec) {
            if (flags.SimpleDomainModel) {
                return RestUtils.SpecToTypeAndFormatString(parameterValueSpec, flags.OidStrategy).Item1;
            }
            if (flags.FormalDomainModel) {
                return BuildDomainTypeUri(RestUtils.SpecToPredefinedTypeString(parameterValueSpec, flags.OidStrategy)).ToString();
            }
            return null;
        }

        public void AddListRepresentationParameter(MediaTypeHeaderValue mediaType, RestControlFlags flags) {
            INakedObjectSpecificationSurface specToUse = param == null ? spec : param.Specification;
            string typeName = specToUse == null ? typeof (object).FullName : specToUse.DomainTypeName(flags.OidStrategy);
            string parameterValue = GetParameterValue(flags, typeName);
            mediaType.Parameters.Add(new NameValueHeaderValue(RestControlFlags.ElementTypeReserved, string.Format("\"{0}\"", parameterValue)));
        }

        public void AddObjectRepresentationParameter(MediaTypeHeaderValue mediaType, RestControlFlags flags) {
            string parameterValue = GetParameterValue(flags, spec);
            if (parameterValue != null) {
                mediaType.Parameters.Add(new NameValueHeaderValue(RestControlFlags.DomainTypeReserved, string.Format("\"{0}\"", parameterValue)));
            }
        }

        public void AddObjectCollectionRepresentationParameter(MediaTypeHeaderValue mediaType, RestControlFlags flags) {
            if (assoc != null && assoc.IsCollection()) {
                string parameterValue = GetParameterValue(flags, assoc.ElementSpecification);
                if (parameterValue != null) {
                    mediaType.Parameters.Add(new NameValueHeaderValue(RestControlFlags.ElementTypeReserved, string.Format("\"{0}\"", parameterValue)));
                }
            }
        }

        public void AddActionResultRepresentationParameter(MediaTypeHeaderValue mediaType, RestControlFlags flags) {
            INakedObjectSpecificationSurface resultSpec = action.ReturnType;
            bool isCollection = resultSpec.IsCollection() && !resultSpec.IsParseable();
            INakedObjectSpecificationSurface parameterValueSpec = isCollection ? action.ElementType : resultSpec;
            string parameterValue = GetParameterValue(flags, parameterValueSpec);

            if (parameterValue != null) {
                string parameterType = isCollection ? RestControlFlags.ElementTypeReserved : RestControlFlags.DomainTypeReserved;
                mediaType.Parameters.Add(new NameValueHeaderValue(parameterType, string.Format("\"{0}\"", parameterValue)));
            }
        }

        public string FormatParameter(string resource, string name) {
            return string.Format(";{0}=\"{1}\"", resource, name);
        }

        public string GetRelParameters() {
            return GetRelParametersFor((INakedObjectMemberSurface) action ?? assoc);
        }

        public string GetServiceRelParameter() {
            return FormatParameter(RelParamValues.ServiceId, cachedType);
        }

        public string GetRelParametersFor(INakedObjectMemberSurface nakedObjectMemberSurface) {
            if (nakedObjectMemberSurface is INakedObjectActionSurface) {
                return FormatParameter(RelParamValues.Action, nakedObjectMemberSurface.Id) + (param == null ? "" : FormatParameter(RelParamValues.Param, param.Id));
            }

            if (nakedObjectMemberSurface is INakedObjectAssociationSurface) {
                var associationSurface = (INakedObjectAssociationSurface) nakedObjectMemberSurface;
                return FormatParameter(associationSurface.IsCollection() ? RelParamValues.Collection : RelParamValues.Property, associationSurface.Id);
            }

            throw new ArgumentException("Unexpected type:" + nakedObjectMemberSurface.GetType());
        }

        public string GetRelParametersFor(INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return FormatParameter(RelParamValues.Action, nakedObjectActionParameterSurface.Action.Id) + FormatParameter(RelParamValues.Param, nakedObjectActionParameterSurface.Id);
        }

        public string GetRelParametersFor(string name) {
            return FormatParameter(RelParamValues.TypeAction, name);
        }
    }
}