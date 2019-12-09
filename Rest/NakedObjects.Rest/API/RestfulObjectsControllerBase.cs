// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Common.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Net.Http.Headers;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.API;
using NakedObjects.Rest.Media;
using NakedObjects.Rest.Model;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Representations;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest {
    public class RestfulObjectsControllerBase : ControllerBase {
        #region static and routes

        private static readonly ILog Logger = LogManager.GetLogger<RestfulObjectsControllerBase>();

        static RestfulObjectsControllerBase() {
            // defaults 
            CacheSettings = new Tuple<int, int, int>(0, 0, 0);
            DefaultPageSize = 20;
            InlineDetailsInActionMemberRepresentations = true;
            InlineDetailsInCollectionMemberRepresentations = true;
            InlineDetailsInPropertyMemberRepresentations = true;
            AllowMutatingActionOnImmutableObject = false;
        }

        protected RestfulObjectsControllerBase(IFrameworkFacade frameworkFacade) {
            FrameworkFacade = frameworkFacade;
            OidStrategy = frameworkFacade.OidStrategy;
        }

        public static bool IsReadOnly { get; set; }

        public static bool InlineDetailsInActionMemberRepresentations { get; set; }

        public static bool InlineDetailsInCollectionMemberRepresentations { get; set; }

        public static bool InlineDetailsInPropertyMemberRepresentations { get; set; }

        // cache settings in seconds, 0 = no cache, "no, short, long")   
        public static Tuple<int, int, int> CacheSettings { get; set; }

        public static int DefaultPageSize {
            get => RestControlFlags.ConfiguredPageSize;
            set => RestControlFlags.ConfiguredPageSize = value;
        }

        public static bool AcceptHeaderStrict {
            get => RestSnapshot.AcceptHeaderStrict;
            set => RestSnapshot.AcceptHeaderStrict = value;
        }

        protected IFrameworkFacade FrameworkFacade { get; set; }
        public IOidStrategy OidStrategy { get; set; }
        public static bool AllowMutatingActionOnImmutableObject { get; set; }

        private static string PrefixRoute(string segment, string prefix) {
            return string.IsNullOrWhiteSpace(prefix) ? segment : prefix + "/" + segment;
        }

        public static void AddRestRoutes(IRouteBuilder routes, string routePrefix = "") {
            //if (!string.IsNullOrWhiteSpace(routePrefix)) {
            //    UriMtHelper.GetApplicationPath = () => {
            //        var appPath = HttpContext.Current.Request.ApplicationPath ?? "";
            //        return appPath + (appPath.EndsWith("/") ? "" : "/") + routePrefix;
            //    };
            //}

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
            routes.MapRoute("GetInvokeIsTypeOf",
                template: domainTypes + "/{typeName}/" + SegmentValues.TypeActions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "GetInvokeTypeActions"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidInvokeIsTypeOf",
                template: domainTypes + "/{typeName}/" + SegmentValues.TypeActions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("GetInvokeOnService",
                template: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "GetInvokeOnService"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("PutInvokeOnService",
                template: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "PutInvokeOnService"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("PUT")}
                );

            routes.MapRoute("PostInvokeOnService",
                template: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "PostInvokeOnService"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("POST")}
                );

            routes.MapRoute("InvalidInvokeOnService",
                template: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("GetInvoke",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "GetInvoke"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("PutInvoke",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "PutInvoke"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("PUT")}
                );

            routes.MapRoute("PostInvoke",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "PostInvoke"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("POST")}
                );

            routes.MapRoute("InvalidInvoke",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("InvalidActionParameterType",
                template: domainTypes + "/{typeName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("InvalidActionType",
                template: domainTypes + "/{typeName}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("GetAction",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "GetAction"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidAction",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("InvalidCollectionType",
                template: domainTypes + "/{typeName}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("DeleteCollection",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "DeleteCollection"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("DELETE")}
                );

            routes.MapRoute("PostCollection",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "PostCollection"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("POST")}
                );

            routes.MapRoute("GetCollection",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "GetCollection"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidCollection",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("InvalidPropertyType",
                template: domainTypes + "/{typeName}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("DeleteProperty",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "DeleteProperty"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("DELETE")}
                );

            routes.MapRoute("PutProperty",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "PutProperty"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("PUT")}
                );

            routes.MapRoute("GetProperty",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "GetProperty"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidProperty",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("PutObject",
                template: objects + "/{domainType}/{instanceId}",
                defaults: new {controller = "RestfulObjects", action = "PutObject"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("PUT")}
                );

            routes.MapRoute("GetObject",
                template: objects + "/{domainType}/{instanceId}",
                defaults: new {controller = "RestfulObjects", action = "GetObject"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidObject",
                template: objects + "/{domainType}/{instanceId}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("PutPersistPropertyPrompt",
                template: objects + "/{domainType}/" + SegmentValues.Properties + "/{propertyName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "PutPersistPropertyPrompt"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("PUT")}
                );

            routes.MapRoute("InvalidPersistPropertyPrompt",
                template: objects + "/{domainType}/" + SegmentValues.Properties + "/{propertyName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("GetPropertyPrompt",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "GetPropertyPrompt"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidPropertyPrompt",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("GetParameterPrompt",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "GetParameterPrompt"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidParameterPrompt",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("GetParameterPromptOnService",
                template: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "GetParameterPromptOnService"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidParameterPromptOnService",
                template: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("GetCollectionValue",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Collections + "/{propertyName}/" + SegmentValues.CollectionValue,
                defaults: new {controller = "RestfulObjects", action = "GetCollectionValue"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidCollectionValue",
                template: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}/" + SegmentValues.CollectionValue,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("Persist",
                template: objects + "/{domainType}",
                defaults: new {controller = "RestfulObjects", action = "PostPersist"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("POST")}
                );

            routes.MapRoute("InvalidPersist",
                template: objects + "/{domainType}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("Image",
                template: images + "/{imageId}",
                defaults: new {controller = "RestfulObjects", action = "GetImage"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidImage",
                template: images + "/{imageId}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("ServiceAction",
                template: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "GetServiceAction"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidServiceAction",
                template: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("Service",
                template: services + "/{serviceName}",
                defaults: new {controller = "RestfulObjects", action = "GetService"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidService",
                template: services + "/{serviceName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("Menu",
                template: menus + "/{menuName}",
                defaults: new {controller = "RestfulObjects", action = "GetMenu"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidMenu",
                template: menus + "/{menuName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("InvalidDomainType",
                template: domainTypes + "/{typeName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("DomainTypes",
                template: domainTypes,
                defaults: new {controller = "RestfulObjects", action = "GetDomainTypes"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidDomainTypes",
                template: domainTypes,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("Version",
                template: version,
                defaults: new {controller = "RestfulObjects", action = "GetVersion"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidVersion",
                template: version,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("Services",
                template: services,
                defaults: new {controller = "RestfulObjects", action = "GetServices"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidServices",
                template: services,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("Menus",
                template: menus,
                defaults: new {controller = "RestfulObjects", action = "GetMenus"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidMenus",
                template: menus,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("User",
                template: user,
                defaults: new {controller = "RestfulObjects", action = "GetUser"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidUser",
                template: user,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapRoute("Home",
                template: home,
                defaults: new {controller = "RestfulObjects", action = "GetHome"},
                constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
                );

            routes.MapRoute("InvalidHome",
                template: home,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});
            // ReSharper restore RedundantArgumentName
        }

        #endregion

        #region api
        [FromQuery(Name = RestControlFlags.ValidateOnlyReserved)]
        public bool ValidateOnly { get; set; }

        [FromQuery(Name = RestControlFlags.DomainTypeReserved)]
        public string DomainType { get; set; }

        [FromQuery(Name = RestControlFlags.ElementTypeReserved)]
        public string ElementType { get; set; }

        [FromQuery(Name = RestControlFlags.DomainModelReserved)]
        public string DomainModel { get; set; }

        [FromQuery(Name = RestControlFlags.FollowLinksReserved)]
        public bool? FollowLinks { get; set; }

        [FromQuery(Name = RestControlFlags.SortByReserved)]
        public bool SortBy { get; set; }

        [FromQuery(Name = RestControlFlags.SearchTermReserved)]
        public string SearchTerm { get; set; }

        [FromQuery(Name = RestControlFlags.PageReserved)]
        public int Page { get; set; }

        [FromQuery(Name = RestControlFlags.PageSizeReserved)]
        public int PageSize { get; set; }

        [FromQuery(Name = RestControlFlags.InlinePropertyDetailsReserved)]
        public bool? InlinePropertyDetails { get; set; }

        [FromQuery(Name = RestControlFlags.InlineCollectionItemsReserved)]
        public bool? InlineCollectionItems { get; set; }

        public virtual ActionResult GetHome() =>
            InitAndHandleErrors(SnapshotFactory.HomeSnapshot(OidStrategy, Request, GetFlags()));

        public virtual ActionResult GetUser() =>
            InitAndHandleErrors(SnapshotFactory.UserSnapshot(OidStrategy, FrameworkFacade.GetUser(), Request, GetFlags()));

        public virtual ActionResult GetServices() =>
            InitAndHandleErrors(SnapshotFactory.ServicesSnapshot(OidStrategy, FrameworkFacade.GetServices(), Request, GetFlags()));

        public virtual ActionResult GetMenus() =>
            InitAndHandleErrors(SnapshotFactory.MenusSnapshot(OidStrategy, FrameworkFacade.GetMainMenus(), Request, GetFlags()));

        public virtual ActionResult GetVersion() =>
            InitAndHandleErrors(SnapshotFactory.VersionSnapshot(OidStrategy, GetOptionalCapabilities(), Request, GetFlags()));

        public virtual ActionResult GetService(string serviceName) =>
            InitAndHandleErrors(SnapshotFactory.ObjectSnapshot(OidStrategy, FrameworkFacade.GetServiceByName(serviceName), Request, GetFlags()));

        public virtual ActionResult GetMenu(string menuName) =>
            InitAndHandleErrors(SnapshotFactory.MenuSnapshot(OidStrategy, FrameworkFacade.GetMenuByName(menuName), Request, GetFlags()));

        public virtual ActionResult GetServiceAction(string serviceName, string actionName) =>
            InitAndHandleErrors(() => new RestSnapshot(OidStrategy, FrameworkFacade.GetServiceActionByName(serviceName, actionName), Request, GetFlags()));

        public virtual ActionResult GetImage(string imageId) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, FrameworkFacade.GetImage(imageId), Request, GetFlags()));
        }

        public virtual ActionResult GetObject(string domainType, string instanceId) {
            return InitAndHandleErrors(() => {
                var loid = FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
                return new RestSnapshot(OidStrategy, FrameworkFacade.GetObject(loid), Request, GetFlags());
            });
        }

        public virtual ActionResult GetPropertyPrompt(string domainType, string instanceId, string propertyName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                var (argsContext, flags) = ProcessArgumentMap(arguments, false, true);
                var link = FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
                var obj = FrameworkFacade.GetObject(link);
                var propertyContext = FrameworkFacade.GetPropertyWithCompletions(obj.Target, propertyName, argsContext);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, propertyContext, Request, flags), false);
            });
        }

        public virtual ActionResult PutPersistPropertyPrompt(string domainType, string propertyName, PromptArgumentMap promptArguments) {
            return InitAndHandleErrors(() => {
                var persistArgs = ProcessPromptArguments(promptArguments);
                var (promptArgs, flags) = ProcessArgumentMap(promptArguments, false, false);
                var obj = FrameworkFacade.GetTransient(domainType, persistArgs);
                PropertyContextFacade propertyContext = FrameworkFacade.GetPropertyWithCompletions(obj.Target, propertyName, promptArgs);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, propertyContext, Request, flags), false);
            });
        }

        public virtual ActionResult GetParameterPrompt(string domainType, string instanceId, string actionName, string parmName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                var (argsContext, flags) = ProcessArgumentMap(arguments, false, true);
                var link = FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
                ActionContextFacade action = FrameworkFacade.GetObjectActionWithCompletions(link, actionName, parmName, argsContext);
                ParameterContextFacade parm = action.VisibleParameters.Single(p => p.Id == parmName);
                parm.Target = action.Target;
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, parm, Request, flags), false);
            });
        }

        public virtual ActionResult GetParameterPromptOnService(string serviceName, string actionName, string parmName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                var (argsContext, flags) = ProcessArgumentMap(arguments, false, true);
                var link = FrameworkFacade.OidTranslator.GetOidTranslation(serviceName);
                ActionContextFacade action = FrameworkFacade.GetServiceActionWithCompletions(link, actionName, parmName, argsContext);
                ParameterContextFacade parm = action.VisibleParameters.Single(p => p.Id == parmName);
                parm.Target = action.Target;
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, parm, Request, flags), false);
            });
        }

        public virtual ActionResult PutObject(string domainType, string instanceId, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                var (argsContext, flags) = ProcessArgumentMap(arguments, true, false);
                ObjectContextFacade context = FrameworkFacade.PutObject(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), argsContext);
                VerifyNoError(context);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, context, Request, flags), flags.ValidateOnly);
            });
        }

        public virtual ActionResult PostPersist(string domainType, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentsContextFacade, RestControlFlags> args = ProcessPersistArguments(arguments);
                ObjectContextFacade context = FrameworkFacade.Persist(domainType, args.Item1);
                VerifyNoPersistError(context, args.Item2);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, context, Request, args.Item2, HttpStatusCode.Created), args.Item2.ValidateOnly);
            });
        }

        public virtual ActionResult GetProperty(string domainType, string instanceId, string propertyName) {
            return InitAndHandleErrors(() => {
                PropertyContextFacade propertyContext = FrameworkFacade.GetProperty(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), propertyName);

                // found but a collection 
                if (propertyContext.Property.IsCollection) {
                    throw new PropertyResourceNotFoundNOSException(propertyName);
                }

                return new RestSnapshot(OidStrategy, propertyContext, Request, GetFlags(), false);
            });
        }

        public virtual ActionResult GetCollection(string domainType, string instanceId, string propertyName) {
            return InitAndHandleErrors(() => {
                try {
                    PropertyContextFacade propertyContext = FrameworkFacade.GetProperty(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), propertyName);

                    if (propertyContext.Property.IsCollection) {
                        return new RestSnapshot(OidStrategy, propertyContext, Request, GetFlags(), false);
                    }
                    // found but not a collection 
                    throw new CollectionResourceNotFoundNOSException(propertyName);
                }
                catch (PropertyResourceNotFoundNOSException e) {
                    throw new CollectionResourceNotFoundNOSException(e.ResourceId);
                }
            });
        }

        public virtual ActionResult GetCollectionValue(string domainType, string instanceId, string propertyName) {
            return InitAndHandleErrors(() => {
                try {
                    PropertyContextFacade propertyContext = FrameworkFacade.GetProperty(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), propertyName);

                    if (propertyContext.Property.IsCollection) {
                        return new RestSnapshot(OidStrategy, propertyContext, Request, GetFlags(), true);
                    }
                    // found but not a collection 
                    throw new CollectionResourceNotFoundNOSException(propertyName);
                }
                catch (PropertyResourceNotFoundNOSException e) {
                    throw new CollectionResourceNotFoundNOSException(e.ResourceId);
                }
            });
        }

        public virtual ActionResult GetAction(string domainType, string instanceId, string actionName) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, FrameworkFacade.GetObjectAction(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), actionName), Request, GetFlags()));
        }

        public virtual ActionResult PutProperty(string domainType, string instanceId, string propertyName, SingleValueArgument argument) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentContextFacade, RestControlFlags> args = ProcessArgument(argument);
                PropertyContextFacade context = FrameworkFacade.PutProperty(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), propertyName, args.Item1);
                VerifyNoError(context);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, context, Request, args.Item2, false), args.Item2.ValidateOnly);
            });
        }

        public virtual ActionResult DeleteProperty(string domainType, string instanceId, string propertyName) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentContextFacade, RestControlFlags> args = ProcessDeleteArgument();
                PropertyContextFacade context = FrameworkFacade.DeleteProperty(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), propertyName, args.Item1);
                VerifyNoError(context);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, context, Request, args.Item2, false), args.Item2.ValidateOnly);
            });
        }

        public virtual ActionResult PostCollection(string domainType, string instanceId, string propertyName, SingleValueArgument argument) {
            return StatusCode((int)HttpStatusCode.Forbidden);
        }

        public virtual ActionResult DeleteCollection(string domainType, string instanceId, string propertyName, SingleValueArgument argument) {
            return StatusCode((int)HttpStatusCode.Forbidden);
        }

        public virtual ActionResult GetInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                var (argsContext, flags) = ProcessArgumentMap(arguments, false,  true);
                ActionResultContextFacade context = FrameworkFacade.ExecuteObjectAction(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), actionName, argsContext);
                VerifyNoError(context);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, context, Request, flags), flags.ValidateOnly);
            });
        }

        public virtual ActionResult PostInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                var (argsContext, flags) = ProcessArgumentMap(arguments, false, false);
                ActionResultContextFacade result = FrameworkFacade.ExecuteObjectAction(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), actionName, argsContext);
                VerifyNoError(result);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, result, Request, flags), flags.ValidateOnly);
            });
        }

        public virtual ActionResult PutInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                var (argsContext, flags) = ProcessArgumentMap(arguments, false, false);
                ActionResultContextFacade result = FrameworkFacade.ExecuteObjectAction(FrameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId), actionName, argsContext);
                VerifyNoError(result);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, result, Request, flags), flags.ValidateOnly);
            });
        }

        public virtual ActionResult GetInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                var (argsContext, flags) = ProcessArgumentMap(arguments, false, true);
                var result = FrameworkFacade.ExecuteServiceAction(FrameworkFacade.OidTranslator.GetOidTranslation(serviceName), actionName, argsContext);
                VerifyNoError(result);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, result, Request, flags), flags.ValidateOnly);
            });
        }

        public virtual ActionResult PutInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                var (argsContext, flags) = ProcessArgumentMap(arguments, false, true);
                ActionResultContextFacade result = FrameworkFacade.ExecuteServiceAction(FrameworkFacade.OidTranslator.GetOidTranslation(serviceName), actionName, argsContext);
                VerifyNoError(result);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, result, Request, flags), flags.ValidateOnly);
            });
        }

        public virtual ActionResult PostInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                var (argsContext, flags) = ProcessArgumentMap(arguments, false, true);
                ActionResultContextFacade result = FrameworkFacade.ExecuteServiceAction(FrameworkFacade.OidTranslator.GetOidTranslation(serviceName), actionName, argsContext);
                VerifyNoError(result);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, result, Request, flags), flags.ValidateOnly);
            });
        }

        public virtual ActionResult GetInvokeTypeActions(string typeName, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                switch (actionName) {
                    case WellKnownIds.IsSubtypeOf:
                    case WellKnownIds.IsSupertypeOf:
                        return GetInvokeIsTypeOf(typeName, actionName, arguments);
                }
                throw new TypeActionResourceNotFoundException(actionName, typeName);
            });
        }

        public virtual ActionResult InvalidMethod() {
            return StatusCode((int) HttpStatusCode.MethodNotAllowed);
        }

        private RestSnapshot GetInvokeIsTypeOf(string typeName, string actionName, ArgumentMap arguments) {
            return new RestSnapshot(OidStrategy, GetIsTypeOf(new TypeActionInvokeContext(actionName, typeName), arguments), Request, GetFlags());
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

        private RestControlFlags GetFlags() {
            return RestControlFlags.FlagsFromArguments(ValidateOnly, 
                Page,
                PageSize, 
                DomainModel,
                InlineDetailsInActionMemberRepresentations,
                InlineDetailsInCollectionMemberRepresentations,
                InlinePropertyDetails.HasValue ? InlinePropertyDetails.Value : InlineDetailsInPropertyMemberRepresentations,
                InlineCollectionItems.HasValue && InlineCollectionItems.Value,
                AllowMutatingActionOnImmutableObject);
        }

        private RestControlFlags GetFlags(ArgumentMap arguments) {
            if (arguments.IsMalformed || arguments.ReservedArguments == null) {
                throw new BadRequestNOSException("Malformed arguments"); // todo i18n
            }

            

            return RestControlFlags.FlagsFromArguments(arguments.ReservedArguments.ValidateOnly,
                arguments.ReservedArguments.Page,
                arguments.ReservedArguments.PageSize,
                arguments.ReservedArguments.DomainModel,
                InlineDetailsInActionMemberRepresentations,
                InlineDetailsInCollectionMemberRepresentations,
                arguments.ReservedArguments.InlinePropertyDetails.HasValue ? arguments.ReservedArguments.InlinePropertyDetails.Value : InlineDetailsInPropertyMemberRepresentations,
                arguments.ReservedArguments.InlineCollectionItems.HasValue && arguments.ReservedArguments.InlineCollectionItems.Value,
                AllowMutatingActionOnImmutableObject);
        }


        private string GetIfMatchTag() {
            var headers = Request.GetTypedHeaders();

            if (headers.IfMatch.Any()) {
                string quotedTag = headers.IfMatch.First().Tag.ToString();
                return quotedTag.Replace("\"", "");
            }

            return null;
        }

        private void VerifyNoError(ActionResultContextFacade result) {

            if (result.ActionContext.VisibleProperties.Any(p => !string.IsNullOrEmpty(p.Reason))) {
                if (result.ActionContext.VisibleProperties.Any(p => p.ErrorCause == Cause.WrongType)) {
                    throw new BadRequestNOSException("Bad Request", result.ActionContext.VisibleProperties.Cast<ContextFacade>().ToList());
                }

                throw new BadArgumentsNOSException("Arguments invalid", result.ActionContext.VisibleProperties.Cast<ContextFacade>().ToList());
            }

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

        private void VerifyNoPersistError(ObjectContextFacade objectContext, RestControlFlags flags) {
            if (objectContext.VisibleProperties.Any(p => !string.IsNullOrEmpty(p.Reason)) || !string.IsNullOrEmpty(objectContext.Reason)) {
                throw new BadPersistArgumentsException("Arguments invalid", objectContext, objectContext.VisibleProperties.Cast<ContextFacade>().ToList(), flags);
            }
        }

        private MethodType GetExpectedMethodType(HttpMethod method) {
            if (method == HttpMethod.Get) {
                return MethodType.QueryOnly;
            }
            if (method == HttpMethod.Put) {
                return MethodType.Idempotent;
            }
            return MethodType.NonIdempotent;
        }


        private void SetCaching(ResponseHeaders m, RestSnapshot ss,  Tuple<int, int, int> cacheSettings)
        {
            int cacheTime = 0;

            switch (ss.Representation.GetCaching())
            {
                case CacheType.Transactional:
                    cacheTime = cacheSettings.Item1;
                    break;
                case CacheType.UserInfo:
                    cacheTime = cacheSettings.Item2;
                    break;
                case CacheType.NonExpiring:
                    cacheTime = cacheSettings.Item3;
                    break;
            }

            if (cacheTime == 0)
            {
                m.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue { NoCache = true };
                //m.Pragma.Add(new NameValueHeaderValue("no-cache"));
                m.Append(HeaderNames.Pragma, "no-cache");
            }
            else
            {
                m.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue { MaxAge = new TimeSpan(0, 0, 0, cacheTime) };
            }

            DateTime now = DateTime.UtcNow;

            m.Date = new DateTimeOffset(now);
            m.Expires = new DateTimeOffset(now).Add(new TimeSpan(0, 0, 0, cacheTime));
        }

        private void SetHeaders(RestSnapshot ss) {
            //HttpResponseMessage msg = Representation.GetAsMessage(formatter, cacheSettings);
            var msg = ControllerContext.HttpContext.Response;
            var responseHeaders = msg.GetTypedHeaders();

            foreach (WarningHeaderValue w in ss.WarningHeaders)
            {
                //responseHeaders.Warning.Add(w);
                responseHeaders.Append(HeaderNames.Warning, w.ToString());
            }

            foreach (string allowHeader in ss.AllowHeaders)
            {
                //responseHeaders.Allow.Add(a);
                responseHeaders.Append(HeaderNames.Allow, allowHeader);
            }

            if (ss.Location != null)
            {
                responseHeaders.Location = ss.Location;
            }

            if (ss.Etag != null)
            {
                responseHeaders.ETag = ss.Etag;
            }

            Microsoft.Net.Http.Headers.MediaTypeHeaderValue ct = ss.Representation.GetContentType();

            if (ct != null)
            {
                //formatter.SupportedMediaTypes.Add(ct);
            }

            //var content = new ObjectContent<Representation>(this, formatter);
            //var msg = new HttpResponseMessage { Content = content };
            responseHeaders.ContentType = ct;

            SetCaching(responseHeaders, ss, CacheSettings);

            //return msg;



            ss.ValidateOutgoingMediaType(ss.Representation is AttachmentRepresentation);
            msg.StatusCode = (int) ss.HttpStatusCode;

            //return msg;
        }

        private void ValidateDomainModel() {
            if (DomainModel != null && DomainModel != RestControlFlags.DomainModelType.Simple.ToString().ToLower() && DomainModel != RestControlFlags.DomainModelType.Formal.ToString().ToLower()) {
                throw new ValidationException((int) HttpStatusCode.BadRequest);
            }
        }

        private void ValidateBinding() {
            if (ModelState.ErrorCount > 0) {
                throw new BadRequestNOSException("Malformed arguments");
            }
        }

        private void Validate() {
            ValidateBinding();
            ValidateDomainModel();
        }

        private ActionResult InitAndHandleErrors(Func<RestSnapshot> f) {
            bool success = false;
            Exception endTransactionError = null;
            RestSnapshot ss;
            try {
                Validate();
                FrameworkFacade.Start();
                ss = f();
                success = true;
            }
            catch (ValidationException validationException) {
                return StatusCode(validationException.StatusCode);
            }
            catch (RedirectionException redirectionException) {
                var responseHeaders = ControllerContext.HttpContext.Response.GetTypedHeaders();
                responseHeaders.Location = redirectionException.RedirectAddress;
                return StatusCode(redirectionException.StatusCode);
            }
            catch (NakedObjectsFacadeException e) {
                return ErrorResult(e);
            }
            catch (Exception e) {
                Logger.ErrorFormat("Unhandled exception from frameworkFacade {0} {1}", e.GetType(), e.Message);
                return ErrorResult(e);
            
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

            if (endTransactionError != null)
            {
                return ErrorResult(endTransactionError);
            }

            try
            {
                //return ConfigureMsg(ss.Populate());
                return RepresentationResult(ss);
            }
            catch (ValidationException validationException)
            {
                return StatusCode(validationException.StatusCode);
            }
            catch (NakedObjectsFacadeException e)
            {
                return ErrorResult(e);
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Unhandled exception while configuring message {0} {1}", e.GetType(), e.Message);
                return ErrorResult(e);
            }
        }


        //private HttpResponseMessage ErrorMsg(Exception e) {
        //    return ConfigureMsg(new RestSnapshot(OidStrategy, e, Request).Populate());
        //}

        private ActionResult ErrorResult(Exception e) {
            var ss = new RestSnapshot(OidStrategy, e, Request);
            return RepresentationResult(ss);
        }

        private ActionResult RepresentationResult(RestSnapshot ss) {
            ss.Populate();
            SetHeaders(ss);
            return new JsonResult(ss.Representation);
        }

        private HttpResponseMessage ConfigureMsg(RestSnapshot snapshot) {
            return snapshot.ConfigureMsg(new JsonNetFormatter(null), CacheSettings);
        }

        private static void HandleReadOnlyRequest() {
            if (IsReadOnly) {
                throw new ValidationException((int) HttpStatusCode.Forbidden);
            }
        }

        private static void ValidateArguments(ArgumentMap arguments, bool errorIfNone = true) {
            if (arguments.IsMalformed) {
                throw new BadRequestNOSException("Malformed arguments"); // todo i18n
            }

            if (!arguments.HasValue && errorIfNone) {
                throw new BadRequestNOSException("Missing arguments"); // todo i18n
            }
        }

        private static void ValidateArguments(SingleValueArgument arguments, bool errorIfNone = true)
        {
            if (arguments.IsMalformed)
            {
                throw new BadRequestNOSException("Malformed arguments"); // todo i18n
            }

            if (!arguments.HasValue && errorIfNone)
            {
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
                return new Tuple<object, RestControlFlags>(parm, GetFlags());
            });
        }

        private IDictionary<string, object> ExtractValues(ArgumentMap arguments, bool errorIfNone) {
            return HandleMalformed(() => {
                ValidateArguments(arguments, errorIfNone);
                return arguments.Map.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetValue(FrameworkFacade, new UriMtHelper(OidStrategy, Request), OidStrategy));
            });
        }

        private (IDictionary<string, object>, RestControlFlags) ExtractValuesAndFlags(ArgumentMap arguments, bool errorIfNone)
        {
            return HandleMalformed(() => {
                ValidateArguments(arguments, errorIfNone);
                var dictionary = arguments.Map.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetValue(FrameworkFacade, new UriMtHelper(OidStrategy, Request), OidStrategy));
                return (dictionary, GetFlags(arguments));
            });
        }

        private (IDictionary<string, object>, RestControlFlags) ExtractValuesAndFlags(PromptArgumentMap arguments, bool errorIfNone) {
            return HandleMalformed(() => {
                ValidateArguments(arguments, errorIfNone);
                var dictionary = arguments.MemberMap.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetValue(FrameworkFacade, new UriMtHelper(OidStrategy, Request), OidStrategy));
                return (dictionary, GetFlags(arguments));
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

        private Tuple<ArgumentsContextFacade, RestControlFlags> ProcessPersistArguments(ArgumentMap persistArgumentMap) {
            var (map, flags) = ExtractValuesAndFlags(persistArgumentMap, true);

            return new Tuple<ArgumentsContextFacade, RestControlFlags>(new ArgumentsContextFacade {
                Digest = GetIfMatchTag(),
                Values = map,
                ValidateOnly = flags.ValidateOnly
            }, flags);
        }

        private ArgumentsContextFacade ProcessPromptArguments(PromptArgumentMap promptArgumentMap) {
            var (dictionary, flags) = ExtractValuesAndFlags(promptArgumentMap, false);

            return new ArgumentsContextFacade {
                Digest = GetIfMatchTag(),
                Values = dictionary,
                ValidateOnly = flags.ValidateOnly
            };
        }

        //private ArgumentsContextFacade ProcessArgumentMap(ArgumentMap arguments, bool errorIfNone, bool ignoreConcurrency) =>
        //    new ArgumentsContextFacade {
        //        Digest = ignoreConcurrency ? null : GetIfMatchTag(),
        //        Values = ExtractValues(arguments, errorIfNone),
        //        ValidateOnly = ValidateOnly,
        //        ExpectedActionType = GetExpectedMethodType(new HttpMethod(Request.Method))
        //    };

        private (ArgumentsContextFacade, RestControlFlags) ProcessArgumentMap(ArgumentMap arguments, bool errorIfNone, bool ignoreConcurrency)
        {
          
            var (map, flags) = arguments.ReservedArguments == null 
                ? (ExtractValues(arguments, errorIfNone), GetFlags()) 
                : ExtractValuesAndFlags(arguments, errorIfNone);

            var facade = new ArgumentsContextFacade
            {
                Digest = ignoreConcurrency ? null : GetIfMatchTag(),
                Values = map,
                ValidateOnly = flags.ValidateOnly,
                ExpectedActionType = GetExpectedMethodType(new HttpMethod(Request.Method))
            };
            return (facade, flags);
        }


        private Tuple<ArgumentContextFacade, RestControlFlags> ProcessArgument(SingleValueArgument argument) {
            return ToTuple(ExtractValueAndFlags(argument), GetIfMatchTag());
        }

        private Tuple<ArgumentContextFacade, RestControlFlags> ProcessDeleteArgument() {
            return ToTuple(new Tuple<object, RestControlFlags>(null, GetFlags()), GetIfMatchTag());
        }

        private static IDictionary<string, string> GetOptionalCapabilities() {
            return new Dictionary<string, string> {
                {"protoPersistentObjects", "yes"},
                {"deleteObjects", "no"},
                {"validateOnly", "yes"},
                {"domainModel", "simple"},
                {"blobsClobs", "attachments"},
                {"inlinedMemberRepresentations", "yes"}
            };
        }

        #endregion
    }
}