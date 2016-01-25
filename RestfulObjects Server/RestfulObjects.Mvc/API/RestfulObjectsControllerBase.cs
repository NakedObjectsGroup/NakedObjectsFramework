// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Common.Logging;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using RestfulObjects.Mvc.Media;
using RestfulObjects.Mvc.Model;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Mvc {
    [ServiceContract]
    public class RestfulObjectsControllerBase : ApiController {
        #region static and routes

        private static readonly ILog Logger = LogManager.GetLogger<RestfulObjectsControllerBase>();

        static RestfulObjectsControllerBase() {
            // defaults 
            CacheSettings = new Tuple<int, int, int>(0, 3600, 86400);
            DefaultPageSize = 20;
            ProtoPersistentObjects = true;
        }

        protected RestfulObjectsControllerBase(IFrameworkFacade frameworkFacade) {
            FrameworkFacade = frameworkFacade;
            OidStrategy = frameworkFacade.OidStrategy;
        }

        public static bool IsReadOnly { get; set; }
        public static bool ConcurrencyChecking { get; set; }

        public static RestControlFlags.DomainModelType DomainModel {
            get { return RestControlFlags.ConfiguredDomainModelType; }
            set { RestControlFlags.ConfiguredDomainModelType = value; }
        }

        // cache settings in seconds, 0 = no cache, "no, short, long")   
        public static Tuple<int, int, int> CacheSettings { get; set; }

        public static int DefaultPageSize {
            get { return RestControlFlags.ConfiguredPageSize; }
            set { RestControlFlags.ConfiguredPageSize = value; }
        }

        public static bool AcceptHeaderStrict {
            get { return RestSnapshot.AcceptHeaderStrict; }
            set { RestSnapshot.AcceptHeaderStrict = value; }
        }

        public static bool ProtoPersistentObjects {
            get { return RestControlFlags.ProtoPersistentObjects; }
            set { RestControlFlags.ProtoPersistentObjects = value; }
        }

        protected IFrameworkFacade FrameworkFacade { get; set; }
        public IOidStrategy OidStrategy { get; set; }

        private static string PrefixRoute(string segment, string prefix) {
            return string.IsNullOrWhiteSpace(prefix) ? segment : prefix + "/" + segment;
        }

        public static void AddRestRoutes(RouteCollection routes, string routePrefix = "") {
            if (!string.IsNullOrWhiteSpace(routePrefix)) {
                UriMtHelper.GetApplicationPath = () => {
                    var appPath = HttpContext.Current.Request.ApplicationPath ?? "";
                    return appPath + (appPath.EndsWith("/") ? "" : "/") + routePrefix;
                };
            }

            var domainTypes = PrefixRoute(SegmentValues.DomainTypes, routePrefix);
            var services = PrefixRoute(SegmentValues.Services, routePrefix);
            var objects = PrefixRoute(SegmentValues.Objects, routePrefix);
            var images = PrefixRoute(SegmentValues.Images, routePrefix);
            var user = PrefixRoute(SegmentValues.User, routePrefix);
            var version = PrefixRoute(SegmentValues.Version, routePrefix);
            var home = PrefixRoute(SegmentValues.HomePage, routePrefix);

            // custom extension 
            var menus = PrefixRoute(SegmentValues.Menus, routePrefix);

            // ReSharper disable RedundantArgumentName
            routes.MapHttpRoute("GetInvokeIsTypeOf",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.TypeActions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "GetInvokeTypeActions"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidInvokeIsTypeOf",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.TypeActions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("GetInvokeOnService",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "GetInvokeOnService"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("PutInvokeOnService",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "PutInvokeOnService"},
                constraints: new {httpMethod = new HttpMethodConstraint("PUT")}
                );

            routes.MapHttpRoute("PostInvokeOnService",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "PostInvokeOnService"},
                constraints: new {httpMethod = new HttpMethodConstraint("POST")}
                );

            routes.MapHttpRoute("InvalidInvokeOnService",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("GetInvoke",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "GetInvoke"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("PutInvoke",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "PutInvoke"},
                constraints: new {httpMethod = new HttpMethodConstraint("PUT")}
                );

            routes.MapHttpRoute("PostInvoke",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "PostInvoke"},
                constraints: new {httpMethod = new HttpMethodConstraint("POST")}
                );

            routes.MapHttpRoute("InvalidInvoke",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("GetActionParameterType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}",
                defaults: new {controller = "RestfulObjects", action = "GetActionParameterType"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidActionParameterType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("GetActionType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "GetActionType"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidActionType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("GetAction",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "GetAction"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidAction",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("GetCollectionType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "GetCollectionType"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidCollectionType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("DeleteCollection",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "DeleteCollection"},
                constraints: new {httpMethod = new HttpMethodConstraint("DELETE")}
                );

            routes.MapHttpRoute("PostCollection",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "PostCollection"},
                constraints: new {httpMethod = new HttpMethodConstraint("POST")}
                );

            routes.MapHttpRoute("GetCollection",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "GetCollection"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidCollection",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("GetPropertyType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "GetPropertyType"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidPropertyType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("DeleteProperty",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "DeleteProperty"},
                constraints: new {httpMethod = new HttpMethodConstraint("DELETE")}
                );

            routes.MapHttpRoute("PutProperty",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "PutProperty"},
                constraints: new {httpMethod = new HttpMethodConstraint("PUT")}
                );

            routes.MapHttpRoute("GetProperty",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "GetProperty"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidProperty",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("PutObject",
                routeTemplate: objects + "/{domainType}/{instanceId}",
                defaults: new {controller = "RestfulObjects", action = "PutObject"},
                constraints: new {httpMethod = new HttpMethodConstraint("PUT")}
                );

            routes.MapHttpRoute("GetObject",
                routeTemplate: objects + "/{domainType}/{instanceId}",
                defaults: new {controller = "RestfulObjects", action = "GetObject"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("PostObject",
                routeTemplate: objects + "/{domainType}/{instanceId}",
                defaults: new { controller = "RestfulObjects", action = "PostObject" },
                constraints: new { httpMethod = new HttpMethodConstraint("Post") }
                );

            routes.MapHttpRoute("InvalidObject",
                routeTemplate: objects + "/{domainType}/{instanceId}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("GetPropertyPrompt",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "GetPropertyPrompt"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidPropertyPrompt",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("GetParameterPrompt",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "GetParameterPrompt"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidParameterPrompt",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("GetParameterPromptOnService",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "GetParameterPromptOnService"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidParameterPromptOnService",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("GetCollectionValue",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Collections + "/{propertyName}/" + SegmentValues.CollectionValue,
                defaults: new {controller = "RestfulObjects", action = "GetCollectionValue"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidCollectionValue",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}/" + SegmentValues.CollectionValue,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("Persist",
                routeTemplate: objects + "/{domainType}",
                defaults: new {controller = "RestfulObjects", action = "PostPersist"},
                constraints: new {httpMethod = new HttpMethodConstraint("POST")}
                );

            routes.MapHttpRoute("InvalidPersist",
                routeTemplate: objects + "/{domainType}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("Image",
                routeTemplate: images + "/{imageId}",
                defaults: new {controller = "RestfulObjects", action = "GetImage"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidImage",
                routeTemplate: images + "/{imageId}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("ServiceAction",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "GetServiceAction"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidServiceAction",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("Service",
                routeTemplate: services + "/{serviceName}",
                defaults: new {controller = "RestfulObjects", action = "GetService"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidService",
                routeTemplate: services + "/{serviceName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("Menu",
                routeTemplate: menus + "/{menuName}",
                defaults: new {controller = "RestfulObjects", action = "GetMenu"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidMenu",
                routeTemplate: menus + "/{menuName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("DomainType",
                routeTemplate: domainTypes + "/{typeName}",
                defaults: new {controller = "RestfulObjects", action = "GetDomainType"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidDomainType",
                routeTemplate: domainTypes + "/{typeName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("DomainTypes",
                routeTemplate: domainTypes,
                defaults: new {controller = "RestfulObjects", action = "GetDomainTypes"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidDomainTypes",
                routeTemplate: domainTypes,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("Version",
                routeTemplate: version,
                defaults: new {controller = "RestfulObjects", action = "GetVersion"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidVersion",
                routeTemplate: version,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("Services",
                routeTemplate: services,
                defaults: new {controller = "RestfulObjects", action = "GetServices"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidServices",
                routeTemplate: services,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("Menus",
                routeTemplate: menus,
                defaults: new {controller = "RestfulObjects", action = "GetMenus"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidMenus",
                routeTemplate: menus,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("User",
                routeTemplate: user,
                defaults: new {controller = "RestfulObjects", action = "GetUser"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidUser",
                routeTemplate: user,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute("Home",
                routeTemplate: home,
                defaults: new {controller = "RestfulObjects", action = "GetHome"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute("InvalidHome",
                routeTemplate: home,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});
            // ReSharper restore RedundantArgumentName
        }

        #endregion

        #region api

        public virtual HttpResponseMessage GetHome(ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetUser(ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, FrameworkFacade.GetUser(), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetServices(ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, FrameworkFacade.GetServices(), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetMenus(ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, FrameworkFacade.GetMainMenus(), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetVersion(ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, GetOptionalCapabilities(), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetDomainTypes(ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, FrameworkFacade.GetDomainTypes().OrderBy(s => s.DomainTypeName(OidStrategy)).ToArray(), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetDomainType(string typeName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => {
                HandlePredefinedTypes(typeName);
                return new RestSnapshot(OidStrategy, FrameworkFacade.GetDomainType(typeName), Request, GetFlags(arguments));
            });
        }

        public virtual HttpResponseMessage GetService(string serviceName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => {
                var oid = FrameworkFacade.OidTranslator.GetOidTranslation(serviceName);
                return new RestSnapshot(OidStrategy, FrameworkFacade.GetService(oid), Request, GetFlags(arguments));
            });
        }

        public virtual HttpResponseMessage GetMenu(string menuName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => { return new RestSnapshot(OidStrategy, FrameworkFacade.GetMainMenus().Single(m => m.Id == menuName), Request, GetFlags(arguments)); });
        }

        public virtual HttpResponseMessage GetServiceAction(string serviceName, string actionName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => {
                var oid = FrameworkFacade.OidTranslator.GetOidTranslation(serviceName);
                return new RestSnapshot(OidStrategy, FrameworkFacade.GetServiceAction(oid, actionName), Request, GetFlags(arguments));
            });
        }

        public virtual HttpResponseMessage GetImage(string imageId, ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, FrameworkFacade.GetImage(imageId), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetObject(string domainType, string instanceId, ReservedArguments arguments) {
            return InitAndHandleErrors(() => {
                var loid = FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
                return new RestSnapshot(OidStrategy, GetObject(loid), Request, GetFlags(arguments));
            });
        }

        public virtual HttpResponseMessage GetPropertyPrompt(string domainType, string instanceId, string propertyName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                Tuple<ArgumentsContextFacade, RestControlFlags> args = ProcessArgumentMap(arguments, false);
                // TODO enhance frameworkFacade to return property with completions 
                var link = FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
                var obj = GetObject(link);

                PropertyContextFacade propertyContext = FrameworkFacade.GetProperty(obj.Target, propertyName);
                propertyContext.UniqueIdForTransient = obj.UniqueIdForTransient;
                ListContextFacade completions = FrameworkFacade.GetPropertyCompletions(obj.Target, propertyName, args.Item1);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, propertyContext, completions, Request, args.Item2), false);
            });
        }

        public virtual HttpResponseMessage GetParameterPrompt(string domainType, string instanceId, string actionName, string parmName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                Tuple<ArgumentsContextFacade, RestControlFlags> args = ProcessArgumentMap(arguments, false);
                // TODO enhance frameworkFacade to return parameter with completions 
                var link = FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
                ActionContextFacade action = FrameworkFacade.GetObjectAction(link, actionName);
                ParameterContextFacade parm = action.VisibleParameters.Single(p => p.Id == parmName);
                parm.Target = action.Target;
                ListContextFacade completions = FrameworkFacade.GetParameterCompletions(link, actionName, parmName, args.Item1);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, parm, completions, Request, args.Item2), false);
            });
        }

        public virtual HttpResponseMessage GetParameterPromptOnService(string serviceName, string actionName, string parmName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                Tuple<ArgumentsContextFacade, RestControlFlags> args = ProcessArgumentMap(arguments, false);

                // TODO enhance frameworkFacade to return parameter with completions 
                var link = FrameworkFacade.OidTranslator.GetOidTranslation(serviceName);
                ActionContextFacade action = FrameworkFacade.GetServiceAction(link, actionName);
                ListContextFacade completions = FrameworkFacade.GetServiceParameterCompletions(link, actionName, parmName, args.Item1);
                ParameterContextFacade parm = action.VisibleParameters.Single(p => p.Id == parmName);
                parm.Target = action.Target;
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, parm, completions, Request, args.Item2), false);
            });
        }

        public virtual HttpResponseMessage PutObject(string domainType, string instanceId, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentsContextFacade, RestControlFlags> args = ProcessArgumentMap(arguments, true);
                ObjectContextFacade context = FrameworkFacade.PutObject(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), args.Item1);
                VerifyNoError(context);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, context, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }


        public virtual HttpResponseMessage PostPersist(string domainType, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentsContextFacade, RestControlFlags> args = ProcessPersistArguments(arguments);
                ObjectContextFacade context = FrameworkFacade.Persist(domainType, args.Item1);
                VerifyNoPersistError(context, args.Item2);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, context, Request, args.Item2, HttpStatusCode.Created), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage PostObject(string domainType, string instanceId, ArgumentMap arguments) {
            if (ProtoPersistentObjects) {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.MethodNotAllowed));
            }
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentsContextFacade, RestControlFlags> args = ProcessArgumentMap(arguments, false);

                var loid = FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
                var obj = GetObject(loid);
                ObjectContextFacade context = FrameworkFacade.PersistObject(obj.Target, args.Item1);
                VerifyNoPersistError(context, args.Item2);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, context, Request, args.Item2, HttpStatusCode.Created), args.Item2.ValidateOnly);

            });
        }

        public virtual HttpResponseMessage GetProperty(string domainType, string instanceId, string propertyName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => {
                PropertyContextFacade propertyContext = FrameworkFacade.GetProperty(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), propertyName);

                // found but a collection 
                if (propertyContext.Property.IsCollection) {
                    throw new PropertyResourceNotFoundNOSException(propertyName);
                }

                return new RestSnapshot(OidStrategy, propertyContext, Request, GetFlags(arguments));
            });
        }

        public virtual HttpResponseMessage GetPropertyType(string typeName, string propertyName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, FrameworkFacade.GetPropertyType(typeName, propertyName), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetCollection(string domainType, string instanceId, string propertyName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => {
                try {
                    PropertyContextFacade propertyContext = FrameworkFacade.GetProperty(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), propertyName);

                    if (propertyContext.Property.IsCollection) {
                        return new RestSnapshot(OidStrategy, propertyContext, Request, GetFlags(arguments));
                    }
                    // found but not a collection 
                    throw new CollectionResourceNotFoundNOSException(propertyName);
                }
                catch (PropertyResourceNotFoundNOSException e) {
                    throw new CollectionResourceNotFoundNOSException(e.ResourceId);
                }
            });
        }

        public virtual HttpResponseMessage GetCollectionValue(string domainType, string instanceId, string propertyName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => {
                try {
                    PropertyContextFacade propertyContext = FrameworkFacade.GetProperty(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), propertyName);

                    if (propertyContext.Property.IsCollection) {
                        return new RestSnapshot(OidStrategy, propertyContext, Request, GetFlags(arguments), true);
                    }
                    // found but not a collection 
                    throw new CollectionResourceNotFoundNOSException(propertyName);
                }
                catch (PropertyResourceNotFoundNOSException e) {
                    throw new CollectionResourceNotFoundNOSException(e.ResourceId);
                }
            });
        }

        public virtual HttpResponseMessage GetCollectionType(string typeName, string propertyName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => {
                try {
                    return new RestSnapshot(OidStrategy, FrameworkFacade.GetPropertyType(typeName, propertyName), Request, GetFlags(arguments));
                }
                catch (TypePropertyResourceNotFoundNOSException e) {
                    throw new TypeCollectionResourceNotFoundNOSException(e.ResourceId, e.DomainId);
                }
            });
        }

        public virtual HttpResponseMessage GetAction(string domainType, string instanceId, string actionName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, FrameworkFacade.GetObjectAction(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), actionName), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetActionType(string typeName, string actionName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, FrameworkFacade.GetActionType(typeName, actionName), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetActionParameterType(string typeName, string actionName, string parmName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, FrameworkFacade.GetActionParameterType(typeName, actionName, parmName), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage PutProperty(string domainType, string instanceId, string propertyName, SingleValueArgument argument) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentContextFacade, RestControlFlags> args = ProcessArgument(argument);
                PropertyContextFacade context = FrameworkFacade.PutProperty(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), propertyName, args.Item1);
                VerifyNoError(context);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, context, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage DeleteProperty(string domainType, string instanceId, string propertyName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentContextFacade, RestControlFlags> args = ProcessDeleteArgument(arguments);
                PropertyContextFacade context = FrameworkFacade.DeleteProperty(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), propertyName, args.Item1);
                VerifyNoError(context);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, context, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage PostCollection(string domainType, string instanceId, string propertyName, SingleValueArgument argument) {
            return InitAndHandleErrors(() => { throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)); });
        }

        public virtual HttpResponseMessage DeleteCollection(string domainType, string instanceId, string propertyName, SingleValueArgument argument) {
            return InitAndHandleErrors(() => { throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)); });
        }

        public virtual HttpResponseMessage GetInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                VerifyActionType(domainType, instanceId, actionName, "GET");
                Tuple<ArgumentsContextFacade, RestControlFlags> args = ProcessArgumentMap(arguments, false, domainType, instanceId, true);
                ActionResultContextFacade context = FrameworkFacade.ExecuteObjectAction(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), actionName, args.Item1);
                VerifyNoError(context);
                CacheTransient(context);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, context, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage PostInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentsContextFacade, RestControlFlags> args = ProcessArgumentMap(arguments, false, domainType, instanceId);
                ActionResultContextFacade result = FrameworkFacade.ExecuteObjectAction(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), actionName, args.Item1);
                VerifyNoError(result);
                CacheTransient(result);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, result, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage PutInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                VerifyActionType(domainType, instanceId, actionName, "PUT");
                HandleReadOnlyRequest();
                Tuple<ArgumentsContextFacade, RestControlFlags> args = ProcessArgumentMap(arguments, false, domainType, instanceId);
                ActionResultContextFacade result = FrameworkFacade.ExecuteObjectAction(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), actionName, args.Item1);
                VerifyNoError(result);
                CacheTransient(result);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, result, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage GetInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                VerifyActionType(serviceName, actionName, "GET");
                Tuple<ArgumentsContextFacade, RestControlFlags> args = ProcessArgumentMap(arguments, false, true);
                ActionResultContextFacade result = FrameworkFacade.ExecuteServiceAction(FrameworkFacade.OidTranslator.GetOidTranslation(serviceName), actionName, args.Item1);
                VerifyNoError(result);
                CacheTransient(result);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, result, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage PutInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                VerifyActionType(serviceName, actionName, "PUT");
                HandleReadOnlyRequest();
                Tuple<ArgumentsContextFacade, RestControlFlags> args = ProcessArgumentMap(arguments, false, true);
                ActionResultContextFacade result = FrameworkFacade.ExecuteServiceAction(FrameworkFacade.OidTranslator.GetOidTranslation(serviceName), actionName, args.Item1);
                VerifyNoError(result);
                CacheTransient(result);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, result, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage PostInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentsContextFacade, RestControlFlags> args = ProcessArgumentMap(arguments, false, true);
                ActionResultContextFacade result = FrameworkFacade.ExecuteServiceAction(FrameworkFacade.OidTranslator.GetOidTranslation(serviceName), actionName, args.Item1);
                VerifyNoError(result);
                CacheTransient(result);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, result, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage GetInvokeTypeActions(string typeName, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                switch (actionName) {
                    case WellKnownIds.IsSubtypeOf:
                    case WellKnownIds.IsSupertypeOf:
                        return GetInvokeIsTypeOf(typeName, actionName, arguments);
                    case WellKnownIds.FilterSubtypesFrom:
                    case WellKnownIds.FilterSupertypesFrom:
                        return GetInvokeFilterFrom(typeName, actionName, arguments);
                }
                throw new TypeActionResourceNotFoundException(actionName, typeName);
            });
        }

        private RestSnapshot GetInvokeIsTypeOf(string typeName, string actionName, ArgumentMap arguments) {
            return new RestSnapshot(OidStrategy, GetIsTypeOf(new TypeActionInvokeContext(actionName, typeName), arguments), Request, GetFlags(arguments));
        }

        private RestSnapshot GetInvokeFilterFrom(string typeName, string actionName, ArgumentMap arguments) {
            return new RestSnapshot(OidStrategy, GetFilterFrom(new FilterFromInvokeContext(actionName, typeName), arguments), Request, GetFlags(arguments));
        }

        #endregion

        #region helpers

        private Tuple<ArgumentContextFacade, RestControlFlags> ToTuple(Tuple<object, RestControlFlags> arguments, string tag) {
            return new Tuple<ArgumentContextFacade, RestControlFlags>(new ArgumentContextFacade {
                Digest = tag,
                Value = arguments.Item1,
                ValidateOnly = arguments.Item2.ValidateOnly
            }, arguments.Item2);
        }

        private RestControlFlags GetFlags(ReservedArguments arguments) {
            if (arguments.IsMalformed) {
                throw new BadRequestNOSException("Malformed arguments"); // todo i18n
            }
            return RestControlFlags.FlagsFromArguments(arguments.ValidateOnly, arguments.Page, arguments.PageSize, arguments.DomainModel);
        }

        private string GetIfMatchTag() {
            if (ConcurrencyChecking) {
                if (!Request.Headers.IfMatch.Any()) {
                    throw new PreconditionHeaderMissingNOSException("If-Match header required with last-known value of ETag for the resource in order to modify its state"); // i18n 
                }
                string quotedTag = Request.Headers.IfMatch.First().Tag;
                return quotedTag.Replace("\"", "");
            }

            return null;
        }

        private void HandlePredefinedTypes(string typeName) {
            var regExBigInteger = new Regex(PredefinedType.Big_integer.ToRoString() + @"\(\d+\)");
            var regExBigDecimal = new Regex(PredefinedType.Big_decimal.ToRoString() + @"\(\d+,\d+\)");

            if (regExBigInteger.IsMatch(typeName) || regExBigDecimal.IsMatch(typeName)) {
                throw new NoContentNOSException();
            }

            if (!typeName.StartsWith(PredefinedType.Big_integer.ToRoString()) && !typeName.StartsWith(PredefinedType.Big_decimal.ToRoString())) {
                if (PredefinedTypeExtensions.PredefinedTypeValues().Contains(typeName)) {
                    throw new NoContentNOSException();
                }
            }
        }

        private void VerifyNoError(ActionResultContextFacade result) {
            if (result.ActionContext.VisibleParameters.Any(p => !string.IsNullOrEmpty(p.Reason))) {
                if (result.ActionContext.VisibleParameters.Any(p => p.ErrorCause == Cause.WrongType)) {
                    throw new BadRequestNOSException("Bad Request", result.ActionContext.VisibleParameters.Cast<ContextFacade>().ToList());
                }

                throw new BadArgumentsNOSException("Arguments invalid", result.ActionContext.VisibleParameters.Cast<ContextFacade>().ToList());
            }
            if (!string.IsNullOrEmpty(result.Reason)) {
                if (result.ErrorCause == Cause.WrongType) {
                    throw new BadRequestNOSException("Bad Request", result.ActionContext);
                }

                throw new BadArgumentsNOSException("Arguments invalid", result);
            }
        }

        private RestSnapshot SnapshotOrNoContent(RestSnapshot ss, bool validateOnly) {
            if (!validateOnly) {
                return ss;
            }
            throw new NoContentNOSException();
        }

        private void VerifyNoError(PropertyContextFacade propertyContext) {
            if (!string.IsNullOrEmpty(propertyContext.Reason)) {
                throw new BadArgumentsNOSException("Arguments invalid", propertyContext);
            }
        }

        private void VerifyNoError(ObjectContextFacade objectContext) {
            if (objectContext.VisibleProperties.Any(p => !string.IsNullOrEmpty(p.Reason))) {
                if (objectContext.VisibleProperties.Any(p => p.ErrorCause == Cause.WrongType)) {
                    throw new BadRequestNOSException("Bad Request", objectContext.VisibleProperties.Cast<ContextFacade>().ToList());
                }

                throw new BadArgumentsNOSException("Arguments invalid", objectContext.VisibleProperties.Cast<ContextFacade>().ToList());
            }
            if (!string.IsNullOrEmpty(objectContext.Reason)) {
                if (objectContext.ErrorCause == Cause.WrongType) {
                    throw new BadRequestNOSException("Bad Request", objectContext);
                }

                throw new BadArgumentsNOSException("Arguments invalid", objectContext);
            }
        }

        private void ClearOldest(Dictionary<string, TransientSlot> dict) {
            while (dict.Count >= 2) {
                var ordered = dict.OrderBy(kvp => kvp.Value.TimeAdded);
                dict.Remove(ordered.First().Key);
            }
        }

        private const string  NofTransients = "nof-transients";


        private void CacheTransient(ActionResultContextFacade actionResult) {

            var target = actionResult?.Result?.Target;

            if (!RestControlFlags.ProtoPersistentObjects && target != null && target.IsTransient) {
                var id = Guid.NewGuid();
                actionResult.Result.UniqueIdForTransient = id;
                var session = HttpContext.Current.Session;
                var transientDict = session[NofTransients] as Dictionary<string, TransientSlot> ?? new Dictionary<string, TransientSlot>();
                var index = id.ToString();
                var transient = actionResult.Result.Target.Object;

                if (transientDict.ContainsKey(index)) {
                    transientDict[index].Transient = transient;
                    transientDict[index].TimeAdded = DateTime.UtcNow;
                }
                else {
                    // clear oldest
                    ClearOldest(transientDict);
                    transientDict[index] = new TransientSlot {Transient = transient, TimeAdded = DateTime.UtcNow};
                }
                session[NofTransients] =  transientDict;
            }
        }

        private class TransientSlot {
            public DateTime TimeAdded { get; set; }
            public object Transient { get; set; }

        }

        private object CheckForTransient(NakedObjects.Facade.Translation.IOidTranslation loid, out Guid idAsGuid) {
            var id = loid.InstanceId;

            if (Guid.TryParse(id, out idAsGuid)) {
                var index = idAsGuid.ToString();
                var session = HttpContext.Current.Session;
                if (session != null) {
                    var transientDict = session[NofTransients] as Dictionary<string, TransientSlot>;
                    return transientDict != null && transientDict.ContainsKey(index) ? transientDict[index].Transient : null;
                }
            }

            return null;
        }

        private ObjectContextFacade GetObject(NakedObjects.Facade.Translation.IOidTranslation loid) {
            Guid idAsGuid;
            var transient = CheckForTransient(loid, out idAsGuid);

            if (transient != null) {
                var obj = FrameworkFacade.GetObject(transient);
                var objectContext = FrameworkFacade.GetObject(obj);
                objectContext.UniqueIdForTransient = idAsGuid;
                return objectContext;
            }

            return FrameworkFacade.GetObject(loid);
        }

        private void VerifyNoPersistError(ObjectContextFacade objectContext, RestControlFlags flags) {
            if (objectContext.VisibleProperties.Any(p => !string.IsNullOrEmpty(p.Reason)) || !string.IsNullOrEmpty(objectContext.Reason)) {
                throw new BadPersistArgumentsException("Arguments invalid", objectContext, objectContext.VisibleProperties.Cast<ContextFacade>().ToList(), flags);
            }
        }

        private void VerifyActionType(ActionContextFacade context, string method) {
            if (method.ToUpper() == "PUT" && !(context.Action.IsQueryOnly || context.Action.IsIdempotent)) {
                throw new NotAllowedNOSException("action is not idempotent"); // i18n 
            }

            if (method.ToUpper() == "GET" && !context.Action.IsQueryOnly) {
                throw new NotAllowedNOSException("action is not side-effect free"); // i18n 
            }
        }

        private void VerifyActionType(string sName, string actionName, string method) {
            ActionContextFacade context = FrameworkFacade.GetServiceAction(FrameworkFacade.OidTranslator.GetOidTranslation(sName), actionName);
            VerifyActionType(context, method);
        }

        private void VerifyActionType(string domainType, string instanceId, string actionName, string method) {
            ActionContextFacade context = FrameworkFacade.GetObjectAction(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), actionName);
            VerifyActionType(context, method);
        }

        private HttpResponseMessage InitAndHandleErrors(Func<RestSnapshot> f) {
            bool success = false;
            Exception endTransactionError = null;
            RestSnapshot ss;
            try {
                FrameworkFacade.Start();
                ss = f();
                success = true;
            }
            catch (HttpResponseException e) {
                Logger.DebugFormat("HttpResponseException being passed up {0}", e.Message);
                throw;
            }
            catch (NakedObjectsFacadeException e) {
                return ErrorMsg(e);
            }
            catch (Exception e) {
                Logger.ErrorFormat("Unhandled exception from frameworkFacade {0} {1}", e.GetType(), e.Message);
                return ErrorMsg(e);
            }
            finally {
                try {
                    FrameworkFacade.End(success);
                }
                catch (Exception e) {
                    // can't return from finally 
                    endTransactionError = e;
                }
            }

            if (endTransactionError != null) {
                return ErrorMsg(endTransactionError);
            }

            try {
                return ConfigureMsg(ss.Populate());
            }
            catch (HttpResponseException e) {
                Logger.DebugFormat("HttpResponseException being passed up {0}", e.Message);
                throw;
            }
            catch (NakedObjectsFacadeException e) {
                return ErrorMsg(e);
            }
            catch (Exception e) {
                Logger.ErrorFormat("Unhandled exception while configuring message {0} {1}", e.GetType(), e.Message);
                return ErrorMsg(e);
            }
        }

        private HttpResponseMessage ErrorMsg(Exception e) {
            Logger.InfoFormat("ErrorMsg - NakedObjectsFacadeException Msg: {0}", e.Message);
            return ConfigureMsg(new RestSnapshot(OidStrategy, e, Request).Populate());
        }

        private HttpResponseMessage ConfigureMsg(RestSnapshot snapshot) {
            Logger.DebugFormat("ConfigureMsg for {0}", snapshot.GetType());
            return snapshot.ConfigureMsg(new JsonNetFormatter(null), CacheSettings);
        }

        private static void HandleReadOnlyRequest() {
            if (IsReadOnly) {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
            }
        }

        private static void ValidateArguments(ReservedArguments arguments, bool errorIfNone = true) {
            if (arguments.IsMalformed) {
                throw new BadRequestNOSException("Malformed arguments"); // todo i18n
            }

            if (!string.IsNullOrEmpty(arguments.DomainModel) &&
                arguments.DomainModel != RestControlFlags.DomainModelType.Simple.ToString().ToLower() &&
                arguments.DomainModel != RestControlFlags.DomainModelType.Formal.ToString().ToLower()) {
                throw new BadRequestNOSException("Malformed arguments"); // todo i18n
            }

            if (!arguments.HasValue && errorIfNone) {
                throw new BadRequestNOSException("Missing arguments"); // todo i18n
            }
        }

        private static T HandleMalformed<T>(Func<T> f) {
            try {
                return f();
            }
            catch (BadRequestNOSException) {
                throw;
            }
            catch (ResourceNotFoundNOSException) {
                throw;
            }
            catch (Exception) {
                throw new BadRequestNOSException("Malformed arguments");
            }
        }

        private Tuple<object, RestControlFlags> ExtractValueAndFlags(SingleValueArgument argument) {
            return HandleMalformed(() => {
                ValidateArguments(argument);
                object parm = argument.Value.GetValue(FrameworkFacade, new UriMtHelper(OidStrategy, Request), OidStrategy);
                return new Tuple<object, RestControlFlags>(parm, GetFlags(argument));
            });
        }

        private Tuple<IDictionary<string, object>, RestControlFlags> ExtractValuesAndFlags(ArgumentMap arguments, bool errorIfNone) {
            return HandleMalformed(() => {
                ValidateArguments(arguments, errorIfNone);
                Dictionary<string, object> dictionary = arguments.Map.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetValue(FrameworkFacade, new UriMtHelper(OidStrategy, Request), OidStrategy));
                return new Tuple<IDictionary<string, object>, RestControlFlags>(dictionary, GetFlags(arguments));
            });
        }

        private TypeActionInvokeContext GetIsTypeOf(TypeActionInvokeContext context, ArgumentMap arguments) {
            ValidateArguments(arguments);

            if (!arguments.Map.ContainsKey(context.ParameterId)) {
                throw new BadRequestNOSException("Malformed arguments");
            }

            ITypeFacade thisSpecification = FrameworkFacade.GetDomainType(context.TypeName);
            IValue parameter = arguments.Map[context.ParameterId];
            object value = parameter.GetValue(FrameworkFacade, new UriMtHelper(OidStrategy, Request), OidStrategy);
            var otherSpecification = (ITypeFacade) (value is ITypeFacade ? value : FrameworkFacade.GetDomainType((string) value));
            context.ThisSpecification = thisSpecification;
            context.OtherSpecification = otherSpecification;
            return context;
        }

        private FilterFromInvokeContext GetFilterFrom(FilterFromInvokeContext context, ArgumentMap arguments) {
            ValidateArguments(arguments);

            if (!arguments.Map.ContainsKey(context.ParameterId)) {
                throw new BadRequestNOSException("Malformed arguments");
            }

            ITypeFacade thisSpecification = FrameworkFacade.GetDomainType(context.TypeName);
            IValue parameter = arguments.Map[context.ParameterId];
            var values = ((IEnumerable) parameter.GetValue(FrameworkFacade, new UriMtHelper(OidStrategy, Request), OidStrategy)).Cast<object>();
            var otherSpecifications = values.Select(value => (ITypeFacade) (value is ITypeFacade ? value : FrameworkFacade.GetDomainType((string) value))).ToArray();
            context.ThisSpecification = thisSpecification;
            context.OtherSpecifications = otherSpecifications;
            return context;
        }

        private Tuple<ArgumentsContextFacade, RestControlFlags> ProcessPersistArguments(ArgumentMap persistArgumentMap) {
            Tuple<IDictionary<string, object>, RestControlFlags> tuple = ExtractValuesAndFlags(persistArgumentMap, true);

            return new Tuple<ArgumentsContextFacade, RestControlFlags>(new ArgumentsContextFacade {
                Digest = null,
                Values = tuple.Item1,
                ValidateOnly = tuple.Item2.ValidateOnly
            }, tuple.Item2);
        }

        private Tuple<ArgumentsContextFacade, RestControlFlags> ProcessArgumentMap(ArgumentMap arguments, bool errorIfNone, string domainType, string instanceId, bool ignoreConcurrency = false) {
            if (!ignoreConcurrency && domainType != null) {
                ObjectContextFacade contextFacade = FrameworkFacade.GetObject(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId));
                ignoreConcurrency = contextFacade.Specification.IsService || contextFacade.Specification.IsImmutable(contextFacade.Target);
            }

            return ProcessArgumentMap(arguments, errorIfNone, ignoreConcurrency);
        }

        private Tuple<ArgumentsContextFacade, RestControlFlags> ProcessArgumentMap(ArgumentMap arguments, bool errorIfNone, bool ignoreConcurrency = false) {
            Tuple<IDictionary<string, object>, RestControlFlags> valuesAndFlags = ExtractValuesAndFlags(arguments, errorIfNone);
            return new Tuple<ArgumentsContextFacade, RestControlFlags>(new ArgumentsContextFacade {
                Digest = ignoreConcurrency ? null : GetIfMatchTag(),
                Values = valuesAndFlags.Item1,
                ValidateOnly = valuesAndFlags.Item2.ValidateOnly,
                SearchTerm = arguments.SearchTerm,
                Page = arguments.Page,
                PageSize = arguments.PageSize
            }, valuesAndFlags.Item2);
        }

        private Tuple<ArgumentContextFacade, RestControlFlags> ProcessArgument(SingleValueArgument argument) {
            return ToTuple(ExtractValueAndFlags(argument), GetIfMatchTag());
        }

        private Tuple<ArgumentContextFacade, RestControlFlags> ProcessDeleteArgument(ReservedArguments arguments) {
            return ToTuple(new Tuple<object, RestControlFlags>(null, GetFlags(arguments)), GetIfMatchTag());
        }

        private static IDictionary<string, string> GetOptionalCapabilities() {
            return new Dictionary<string, string> {
                {"protoPersistentObjects", ProtoPersistentObjects ? "yes" : "no"},
                {"deleteObjects", "no"},
                {"validateOnly", "yes"},
                {"domainModel", DomainModel.ToString().ToLower()},
                {"blobsClobs", "attachments"},
                {"inlinedMemberRepresentations", "yes"}
            };
        }

        #endregion

       
    }
}