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
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Facade.Translation;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class UriMtHelper {
        private readonly IOidStrategy oidStrategy;
        public static Func<HttpRequestMessage, string> GetAuthority;
        public static Func<string> GetApplicationPath;
        private static readonly ILog Logger = LogManager.GetLogger<UriMtHelper>();
        private readonly IActionFacade action;
        private readonly IAssociationFacade assoc;
        private readonly string cachedId; // cache because may not be available at writing time 
        private  string cachedType; // cache because may not be available at writing time 
        private readonly IObjectFacade objectFacade;
        private readonly IActionParameterFacade param;
        private readonly Uri prefix;
        private readonly ITypeFacade spec;
        private readonly string typeAction;

        // for testing to allow to be stubbed 
        static UriMtHelper() {
            GetAuthority = req => req.RequestUri.Authority;
            GetApplicationPath = () => HttpContext.Current.Request.ApplicationPath;
        }


        public UriMtHelper(IOidStrategy oidStrategy, HttpRequestMessage req) {
            this.oidStrategy = oidStrategy;
            prefix = new Uri(req.RequestUri.Scheme + "://" + GetAuthority(req));

            string applicationPath = GetApplicationPath();

            if (!string.IsNullOrEmpty(applicationPath)) {
                prefix = new Uri(prefix, applicationPath);
            }

            DebugLogRequest(req);
        }

        public UriMtHelper(IOidStrategy oidStrategy, HttpRequestMessage req, IObjectFacade objectFacade) : this(oidStrategy ,req) {
            this.objectFacade = objectFacade;
            spec = objectFacade.Specification;
            IOidTranslation oid = oidStrategy.FrameworkFacade.OidTranslator.GetOidTranslation(objectFacade);
            cachedId = oid.InstanceId;
            CachedType = oid.DomainType;
        }

        public UriMtHelper(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyContextFacade propertyContext)
            : this(oidStrategy ,req) {
            assoc = propertyContext.Property;
            objectFacade = propertyContext.Target;
            spec = objectFacade.Specification;
            IOidTranslation oid = oidStrategy.FrameworkFacade.OidTranslator.GetOidTranslation(objectFacade);
            cachedId = oid.InstanceId;
            CachedType = oid.DomainType;
        }

        public UriMtHelper(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyTypeContextFacade propertyContext)
            : this(oidStrategy ,req) {
            assoc = propertyContext.Property;
            spec = propertyContext.OwningSpecification;
            cachedId = "";
            CachedType = spec.DomainTypeName(oidStrategy);
        }

        public UriMtHelper(IOidStrategy oidStrategy, HttpRequestMessage req, ActionContextFacade actionContext)
            : this(oidStrategy ,req) {
            action = actionContext.Action;
            objectFacade = actionContext.Target;
            spec = objectFacade.Specification;
            IOidTranslation oid = oidStrategy.FrameworkFacade.OidTranslator.GetOidTranslation(objectFacade);
            cachedId = oid.InstanceId;
            CachedType = oid.DomainType;
        }

        public UriMtHelper(IOidStrategy oidStrategy, HttpRequestMessage req, ActionTypeContextFacade actionTypeContext)
            : this(oidStrategy ,req) {
            action = actionTypeContext.ActionContext.Action;
            spec = actionTypeContext.OwningSpecification;
            cachedId = "";
            CachedType = spec.DomainTypeName(oidStrategy);
        }

        public UriMtHelper(IOidStrategy oidStrategy, HttpRequestMessage req, ParameterTypeContextFacade parameterTypeContext)
            : this(oidStrategy ,req) {
            action = parameterTypeContext.Action;
            spec = parameterTypeContext.OwningSpecification;
            param = parameterTypeContext.Parameter;
            cachedId = "";
            CachedType = spec.DomainTypeName(oidStrategy);
        }

        public UriMtHelper(IOidStrategy oidStrategy, HttpRequestMessage req, ParameterContextFacade parameterContext)
            : this(oidStrategy ,req) {
            action = parameterContext.Action;
            param = parameterContext.Parameter;
            objectFacade = parameterContext.Target;
            spec = objectFacade.Specification;
            IOidTranslation oid = oidStrategy.FrameworkFacade.OidTranslator.GetOidTranslation(objectFacade);
            cachedId = oid.InstanceId;
            CachedType = oid.DomainType;
        }

        public UriMtHelper(IOidStrategy oidStrategy, HttpRequestMessage req, ITypeFacade spec)
            : this(oidStrategy, req) {
            this.spec = spec;
            cachedId = "";
            CachedType = RestUtils.SpecToPredefinedTypeString(spec, oidStrategy);
        }

        public UriMtHelper(IOidStrategy oidStrategy, HttpRequestMessage req, IAssociationFacade assoc)
            : this(oidStrategy ,req) {
            cachedId = "";
            if (assoc.IsCollection) {
                CachedType = assoc.IsASet ? PredefinedType.Set.ToRoString() : PredefinedType.List.ToRoString();
            }
            else {
                CachedType = assoc.Specification.DomainTypeName(oidStrategy);
            }
        }

        public UriMtHelper(IOidStrategy oidStrategy, HttpRequestMessage req, TypeActionInvokeContext context)
            : this(oidStrategy ,req) {
            typeAction = context.Id;
            cachedId = "";
            CachedType = oidStrategy.GetLinkDomainTypeBySpecification(context.ThisSpecification);
        }

        private string CachedType {
            get { return cachedType; }
            set {
                cachedType = value;
            }
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
            return BuildDomainTypeUri(CachedType);
        }


        public Uri GetParamTypeUri() {
            CheckArgumentNotNull(CachedType, "object type");
            CheckArgumentNotNull(action.Id, "action id");
            CheckArgumentNotNull(param.Id, "param id");

            var template = new UriTemplate(SegmentValues.DomainTypes + "/{id}/" + SegmentValues.Actions + "/{action}/" + SegmentValues.Params + "/{paramId}");
            return template.BindByPosition(prefix, CachedType, action.Id, param.Id);
        }

        public Uri GetObjectParamUri() {
            CheckArgumentNotNull(CachedType, "object type");
            CheckArgumentNotNull(action.Id, "action id");
            CheckArgumentNotNull(param.Id, "param id");

            var template = new UriTemplate(SegmentValues.Objects + "/{typeId}/{instanceId}/" + SegmentValues.Actions + "/{action}/" + SegmentValues.Params + "/{paramId}");
            return template.BindByPosition(prefix, CachedType, cachedId, action.Id, param.Id);
        }

        public Uri GetServiceParamUri() {
            CheckArgumentNotNull(CachedType, "object type");
            CheckArgumentNotNull(action.Id, "action id");
            CheckArgumentNotNull(param.Id, "param id");

            var template = new UriTemplate(SegmentValues.Services + "/{oid}/" + SegmentValues.Actions + "/{action}/" + SegmentValues.Params + "/{paramId}");
            return template.BindByPosition(prefix, CachedType, action.Id, param.Id);
        }

        public Uri GetParamUri() {
            return spec.IsService ? GetServiceParamUri() : GetObjectParamUri();
        }

        public Uri GetTypeActionInvokeUri() {
            CheckArgumentNotNull(CachedType, "domain type");
            CheckArgumentNotNull(typeAction, "type action");

            var template = new UriTemplate(SegmentValues.DomainTypes + "/{id}/" + SegmentValues.TypeActions + "/{action}/" + SegmentValues.Invoke);
            return template.BindByPosition(prefix, CachedType, typeAction);
        }

        public Uri GetObjectUri() {
            CheckArgumentNotNull(CachedType, "object type");
            CheckArgumentNotNull(cachedId, "object key");

            var template = new UriTemplate(SegmentValues.Objects + "/{typeId}/{instanceId}");
            return template.BindByPosition(prefix, CachedType, cachedId);
        }

        public Uri GetServiceUri() {
            CheckArgumentNotNull(CachedType, "service type");

            var template = new UriTemplate(SegmentValues.Services + "/{oid}");
            return template.BindByPosition(prefix, CachedType);
        }


        public Uri GetIconUri() {
            var template = new UriTemplate(SegmentValues.Images + "/{image}");
            string name = spec.GetIconName(objectFacade);
            string iconName = name.Contains(".") ? name : name + ".gif";
            CheckArgumentNotNull(iconName, "icon name");
            return template.BindByPosition(prefix, iconName);
        }


        public Uri GetInvokeUri() {
            if (action.IsQueryOnly) {
                return GetQueryInvokeUri();
            }
            if (action.IsIdempotent) {
                return GetIdempotentUri();
            }
            return GetNonIdempotentUri();
        }

        private Uri GetServiceInvokeUri(string queryString) {
            CheckArgumentNotNull(CachedType, "service type");
            CheckArgumentNotNull(action.Id, "action id");

            var template = new UriTemplate(SegmentValues.Services + "/{oid}/" + SegmentValues.Actions + "/{action}/" + SegmentValues.Invoke + queryString);
            return template.BindByPosition(prefix, CachedType, action.Id);
        }

        private Uri GetObjectInvokeUri(string queryString) {
            CheckArgumentNotNull(CachedType, "object type");
            CheckArgumentNotNull(cachedId, "object key");
            CheckArgumentNotNull(action.Id, "action id");

            var template = new UriTemplate(SegmentValues.Objects + "/{objectType}/{objectKey}/" + SegmentValues.Actions + "/{action}/" + SegmentValues.Invoke + queryString);
            return template.BindByPosition(prefix, CachedType, cachedId, action.Id);
        }

        private Uri GetInvokeUri(string queryString) {
            return spec.IsService ? GetServiceInvokeUri(queryString) : GetObjectInvokeUri(queryString);
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
            CheckArgumentNotNull(CachedType, "object type");

            var template = new UriTemplate(SegmentValues.Objects + "/{objectType}");
            return template.BindByPosition(prefix, CachedType);
        }

        public Uri GetRedirectUri(HttpRequestMessage req, string url) {
            CheckArgumentNotNull(url, "url");
     
            return new Uri(url);
        }

        private Uri GetServiceMemberUri(IMemberFacade member, string memberType) {
            CheckArgumentNotNull(CachedType, "service type");
            CheckArgumentNotNull(memberType, "member type");
            CheckArgumentNotNull(member.Id, "member id");


            var template = new UriTemplate(SegmentValues.Services + "/{id}/{memberType}/{memberId}");
            return template.BindByPosition(prefix, CachedType, memberType, member.Id);
        }

        private Uri GetTypeMemberUri(IMemberFacade member, string memberType) {
            CheckArgumentNotNull(CachedType, "domain type");
            CheckArgumentNotNull(memberType, "member type");
            CheckArgumentNotNull(member.Id, "member id");


            var template = new UriTemplate(SegmentValues.DomainTypes + "/{objectType}/{memberType}/{memberId}");
            return template.BindByPosition(prefix, CachedType, memberType, member.Id);
        }

        private Uri GetObjectMemberUri(IMemberFacade member, string memberType) {
            CheckArgumentNotNull(CachedType, "object type");
            CheckArgumentNotNull(memberType, "member type");
            CheckArgumentNotNull(member.Id, "member id");


            var template = new UriTemplate(SegmentValues.Objects + "/{objectType}/{objectId}/{memberType}/{memberId}");
            return template.BindByPosition(prefix, CachedType, cachedId, memberType, member.Id);
        }

        private Uri GetMemberUri(IMemberFacade member, string memberType) {
            return spec.IsService ? GetServiceMemberUri(member, memberType) : GetObjectMemberUri(member, memberType);
        }

        private Uri ByMemberType(Func<IMemberFacade, string, Uri> getUri) {
            if (action != null) {
                return getUri(action, SegmentValues.Actions);
            }

            if (assoc != null && assoc.IsCollection) {
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
            IObjectFacade no = assoc.GetValue(objectFacade);
            string mtv = no == null  || string.IsNullOrWhiteSpace(no.GetAttachment().MimeType) ? ""  : no.GetAttachment().MimeType;
            return new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(mtv) ? "image/bmp" : mtv);
        }

        public MediaTypeHeaderValue GetIconMediaType() {
            string name = spec.GetIconName(objectFacade);
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
            CheckArgumentNotNull(CachedType, "object type");
            CheckArgumentNotNull(actionName, "action name");

            var template = new UriTemplate(SegmentValues.DomainTypes + "/{class}/" + SegmentValues.TypeActions + "/{action}/" + SegmentValues.Invoke);
            return template.BindByPosition(prefix, CachedType, actionName);
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


        private string GetParameterValue(RestControlFlags flags, ITypeFacade parameterValueSpec) {
            if (flags.SimpleDomainModel) {
                return RestUtils.SpecToTypeAndFormatString(parameterValueSpec, oidStrategy).Item1;
            }
            if (flags.FormalDomainModel) {
                return BuildDomainTypeUri(RestUtils.SpecToPredefinedTypeString(parameterValueSpec, oidStrategy)).ToString();
            }
            return null;
        }

        public void AddListRepresentationParameter(MediaTypeHeaderValue mediaType, RestControlFlags flags) {
            ITypeFacade specToUse = param == null ? spec : param.Specification;
            string typeName = specToUse == null ? typeof (object).FullName : specToUse.DomainTypeName(oidStrategy);
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
            if (assoc != null && assoc.IsCollection) {
                string parameterValue = GetParameterValue(flags, assoc.ElementSpecification);
                if (parameterValue != null) {
                    mediaType.Parameters.Add(new NameValueHeaderValue(RestControlFlags.ElementTypeReserved, string.Format("\"{0}\"", parameterValue)));
                }
            }
        }

        public void AddActionResultRepresentationParameter(MediaTypeHeaderValue mediaType, RestControlFlags flags) {
            ITypeFacade resultSpec = action.ReturnType;
            bool isCollection = resultSpec.IsCollection && !resultSpec.IsParseable;
            ITypeFacade parameterValueSpec = isCollection ? action.ElementType : resultSpec;
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
            return GetRelParametersFor((IMemberFacade) action ?? assoc);
        }

        public string GetServiceRelParameter() {
            return FormatParameter(RelParamValues.ServiceId, CachedType);
        }

        public string GetRelParametersFor(IMemberFacade memberFacade) {
            if (memberFacade is IActionFacade) {
                return FormatParameter(RelParamValues.Action, memberFacade.Id) + (param == null ? "" : FormatParameter(RelParamValues.Param, param.Id));
            }

            var associationFacade = memberFacade as IAssociationFacade;
            if (associationFacade != null) {
                return FormatParameter(associationFacade.IsCollection ? RelParamValues.Collection : RelParamValues.Property, associationFacade.Id);
            }

            throw new ArgumentException("Unexpected type:" + memberFacade.GetType());
        }

        public string GetRelParametersFor(IActionParameterFacade actionParameterFacade) {
            return FormatParameter(RelParamValues.Action, actionParameterFacade.Action.Id) + FormatParameter(RelParamValues.Param, actionParameterFacade.Id);
        }

        public string GetRelParametersFor(string name) {
            return FormatParameter(RelParamValues.TypeAction, name);
        }
    }
}