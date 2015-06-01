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
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Common.Logging;
using NakedObjects.Facade;
using NakedObjects.Surface;
using NakedObjects.Surface.Context;
using NakedObjects.Surface.Utility;
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
            CacheSettings = new Tuple<int, int, int>(0, 3600, 86400);
            DefaultPageSize = 20;
        }

        protected RestfulObjectsControllerBase(IFrameworkFacade surface) {
            Surface = surface;
            OidStrategy = surface.OidStrategy;
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

        protected IFrameworkFacade Surface { get; set; }
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

            // ReSharper disable RedundantArgumentName
            routes.MapHttpRoute(
                name: "GetInvokeIsTypeOf",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.TypeActions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "GetInvokeIsTypeOf"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidInvokeIsTypeOf",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.TypeActions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "GetInvokeOnService",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "GetInvokeOnService"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "PutInvokeOnService",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "PutInvokeOnService"},
                constraints: new {httpMethod = new HttpMethodConstraint("PUT")}
                );

            routes.MapHttpRoute(
                name: "PostInvokeOnService",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "PostInvokeOnService"},
                constraints: new {httpMethod = new HttpMethodConstraint("POST")}
                );

            routes.MapHttpRoute(
                name: "InvalidInvokeOnService",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "GetInvoke",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "GetInvoke"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "PutInvoke",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "PutInvoke"},
                constraints: new {httpMethod = new HttpMethodConstraint("PUT")}
                );

            routes.MapHttpRoute(
                name: "PostInvoke",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "PostInvoke"},
                constraints: new {httpMethod = new HttpMethodConstraint("POST")}
                );

            routes.MapHttpRoute(
                name: "InvalidInvoke",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "GetActionParameterType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}",
                defaults: new {controller = "RestfulObjects", action = "GetActionParameterType"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidActionParameterType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "GetActionType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "GetActionType"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidActionType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "GetAction",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "GetAction"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidAction",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "GetCollectionType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "GetCollectionType"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidCollectionType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "DeleteCollection",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "DeleteCollection"},
                constraints: new {httpMethod = new HttpMethodConstraint("DELETE")}
                );

            routes.MapHttpRoute(
                name: "PostCollection",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "PostCollection"},
                constraints: new {httpMethod = new HttpMethodConstraint("POST")}
                );

            routes.MapHttpRoute(
                name: "GetCollection",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "GetCollection"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidCollection",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Collections + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "GetPropertyType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "GetPropertyType"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidPropertyType",
                routeTemplate: domainTypes + "/{typeName}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "DeleteProperty",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "DeleteProperty"},
                constraints: new {httpMethod = new HttpMethodConstraint("DELETE")}
                );

            routes.MapHttpRoute(
                name: "PutProperty",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "PutProperty"},
                constraints: new {httpMethod = new HttpMethodConstraint("PUT")}
                );

            routes.MapHttpRoute(
                name: "GetProperty",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "GetProperty"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidProperty",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "PutObject",
                routeTemplate: objects + "/{domainType}/{instanceId}",
                defaults: new {controller = "RestfulObjects", action = "PutObject"},
                constraints: new {httpMethod = new HttpMethodConstraint("PUT")}
                );

            routes.MapHttpRoute(
                name: "GetObject",
                routeTemplate: objects + "/{domainType}/{instanceId}",
                defaults: new {controller = "RestfulObjects", action = "GetObject"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidObject",
                routeTemplate: objects + "/{domainType}/{instanceId}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "GetPropertyPrompt",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "GetPropertyPrompt"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidPropertyPrompt",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "GetParameterPrompt",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "GetParameterPrompt"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidParameterPrompt",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "GetParameterPromptOnService",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "GetParameterPromptOnService"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidParameterPromptOnService",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}/" + SegmentValues.Prompt,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "GetCollectionValue",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Collections + "/{propertyName}/" + SegmentValues.CollectionValue,
                defaults: new {controller = "RestfulObjects", action = "GetCollectionValue"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidCollectionValue",
                routeTemplate: objects + "/{domainType}/{instanceId}/" + SegmentValues.Properties + "/{propertyName}/" + SegmentValues.CollectionValue,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "Persist",
                routeTemplate: objects + "/{domainType}",
                defaults: new {controller = "RestfulObjects", action = "PostPersist"},
                constraints: new {httpMethod = new HttpMethodConstraint("POST")}
                );

            routes.MapHttpRoute(
                name: "InvalidPersist",
                routeTemplate: objects + "/{domainType}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "Image",
                routeTemplate: images + "/{imageId}",
                defaults: new {controller = "RestfulObjects", action = "GetImage"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidImage",
                routeTemplate: images + "/{imageId}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "ServiceAction",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "GetServiceAction"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidServiceAction",
                routeTemplate: services + "/{serviceName}/" + SegmentValues.Actions + "/{actionName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "Service",
                routeTemplate: services + "/{serviceName}",
                defaults: new {controller = "RestfulObjects", action = "GetService"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidService",
                routeTemplate: services + "/{serviceName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "DomainType",
                routeTemplate: domainTypes + "/{typeName}",
                defaults: new {controller = "RestfulObjects", action = "GetDomainType"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidDomainType",
                routeTemplate: domainTypes + "/{typeName}",
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "DomainTypes",
                routeTemplate: domainTypes,
                defaults: new {controller = "RestfulObjects", action = "GetDomainTypes"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidDomainTypes",
                routeTemplate: domainTypes,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "Version",
                routeTemplate: version,
                defaults: new {controller = "RestfulObjects", action = "GetVersion"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidVersion",
                routeTemplate: version,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "Services",
                routeTemplate: services,
                defaults: new {controller = "RestfulObjects", action = "GetServices"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidServices",
                routeTemplate: services,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "User",
                routeTemplate: user,
                defaults: new {controller = "RestfulObjects", action = "GetUser"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidUser",
                routeTemplate: user,
                defaults: new {controller = "RestfulObjects", action = "InvalidMethod"});

            routes.MapHttpRoute(
                name: "Home",
                routeTemplate: home,
                defaults: new {controller = "RestfulObjects", action = "GetHome"},
                constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                );

            routes.MapHttpRoute(
                name: "InvalidHome",
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
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, Surface.GetUser(), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetServices(ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, Surface.GetServices(), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetVersion(ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, GetOptionalCapabilities(), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetDomainTypes(ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, Surface.GetDomainTypes().OrderBy(s => s.DomainTypeName(OidStrategy)).ToArray(), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetDomainType(string typeName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => {
                HandlePredefinedTypes(typeName);
                return new RestSnapshot(OidStrategy, Surface.GetDomainType(typeName), Request, GetFlags(arguments));
            });
        }

        public virtual HttpResponseMessage GetService(string serviceName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => {
                var oid = OidStrategy.Surface.OidTranslator.GetOidTranslation(serviceName);
                return new RestSnapshot(OidStrategy, Surface.GetService(oid), Request, GetFlags(arguments));
            });
        }

        public virtual HttpResponseMessage GetServiceAction(string serviceName, string actionName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => {
                var oid = OidStrategy.Surface.OidTranslator.GetOidTranslation(serviceName);
                return new RestSnapshot(OidStrategy, Surface.GetServiceAction(oid, actionName), Request, GetFlags(arguments));
            });
        }

        public virtual HttpResponseMessage GetImage(string imageId, ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, Surface.GetImage(imageId), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage PostPersist(string domainType, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentsContext, RestControlFlags> args = ProcessPersistArguments(arguments);
                ObjectContextSurface context = Surface.Persist(domainType, args.Item1);
                VerifyNoPersistError(context, args.Item2);
                return SnapshotOrNoContent(new RestSnapshot( OidStrategy, context, Request, args.Item2, HttpStatusCode.Created), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage GetObject(string domainType, string instanceId, ReservedArguments arguments) {

            return InitAndHandleErrors(() => {
                var loid = Surface.OidTranslator.GetOidTranslation(domainType, instanceId);
                return new RestSnapshot(OidStrategy, Surface.GetObject(loid), Request, GetFlags(arguments));
            });
        }

        public virtual HttpResponseMessage GetPropertyPrompt(string domainType, string instanceId, string propertyName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                Tuple<ArgumentsContext, RestControlFlags> args = ProcessArgumentMap(arguments, false);
                // TODO enhance surface to return property with completions 
                var link = Surface.OidTranslator.GetOidTranslation(domainType, instanceId);
                PropertyContextSurface propertyContext = Surface.GetProperty(link, propertyName);
                ListContextSurface completions = Surface.GetPropertyCompletions(link, propertyName, args.Item1);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, propertyContext, completions, Request, args.Item2), false);
            });
        }

        public virtual HttpResponseMessage GetParameterPrompt(string domainType, string instanceId, string actionName, string parmName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                Tuple<ArgumentsContext, RestControlFlags> args = ProcessArgumentMap(arguments, false);
                // TODO enhance surface to return parameter with completions 
                var link = Surface.OidTranslator.GetOidTranslation(domainType, instanceId);
                ActionContextSurface action = Surface.GetObjectAction(link, actionName);
                ParameterContextSurface parm = action.VisibleParameters.Single(p => p.Id == parmName);
                parm.Target = action.Target;
                ListContextSurface completions = Surface.GetParameterCompletions(link, actionName, parmName, args.Item1);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, parm, completions, Request, args.Item2), false);
            });
        }

        public virtual HttpResponseMessage GetParameterPromptOnService(string serviceName, string actionName, string parmName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                Tuple<ArgumentsContext, RestControlFlags> args = ProcessArgumentMap(arguments, false);

                // TODO enhance surface to return parameter with completions 
                var link = OidStrategy.Surface.OidTranslator.GetOidTranslation(serviceName);
                ActionContextSurface action = Surface.GetServiceAction(link, actionName);
                ListContextSurface completions = Surface.GetServiceParameterCompletions(link, actionName, parmName, args.Item1);
                ParameterContextSurface parm = action.VisibleParameters.Single(p => p.Id == parmName);
                parm.Target = action.Target;
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, parm, completions, Request, args.Item2), false);
            });
        }

        public virtual HttpResponseMessage PutObject(string domainType, string instanceId, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentsContext, RestControlFlags> args = ProcessArgumentMap(arguments, true);
                ObjectContextSurface context = Surface.PutObject( Surface.OidTranslator.GetOidTranslation(domainType, instanceId), args.Item1);
                VerifyNoError(context);
                return SnapshotOrNoContent(new RestSnapshot(OidStrategy, context, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage GetProperty(string domainType, string instanceId, string propertyName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => {
                PropertyContextSurface propertyContext = Surface.GetProperty(Surface.OidTranslator.GetOidTranslation(domainType, instanceId), propertyName);

                // found but a collection 
                if (propertyContext.Property.IsCollection) {
                    throw new PropertyResourceNotFoundNOSException(propertyName);
                }

                return new RestSnapshot(OidStrategy , propertyContext, Request, GetFlags(arguments));
            });
        }

        public virtual HttpResponseMessage GetPropertyType(string typeName, string propertyName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy, Surface.GetPropertyType(typeName, propertyName), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetCollection(string domainType, string instanceId, string propertyName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => {
                try {
                    PropertyContextSurface propertyContext = Surface.GetProperty(Surface.OidTranslator.GetOidTranslation(domainType, instanceId), propertyName);


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
                    PropertyContextSurface propertyContext = Surface.GetProperty(Surface.OidTranslator.GetOidTranslation(domainType, instanceId), propertyName);


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
                    return new RestSnapshot( OidStrategy, Surface.GetPropertyType(typeName, propertyName), Request, GetFlags(arguments));
                }
                catch (TypePropertyResourceNotFoundNOSException e) {
                    throw new TypeCollectionResourceNotFoundNOSException(e.ResourceId, e.DomainId);
                }
            });
        }

        public virtual HttpResponseMessage GetAction(string domainType, string instanceId, string actionName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot( OidStrategy, Surface.GetObjectAction(Surface.OidTranslator.GetOidTranslation(domainType, instanceId), actionName), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetActionType(string typeName, string actionName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot( OidStrategy, Surface.GetActionType(typeName, actionName), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage GetActionParameterType(string typeName, string actionName, string parmName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => new RestSnapshot( OidStrategy, Surface.GetActionParameterType(typeName, actionName, parmName), Request, GetFlags(arguments)));
        }

        public virtual HttpResponseMessage PutProperty(string domainType, string instanceId, string propertyName, SingleValueArgument argument) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentContext, RestControlFlags> args = ProcessArgument(argument);
                PropertyContextSurface context = Surface.PutProperty(Surface.OidTranslator.GetOidTranslation(domainType, instanceId), propertyName, args.Item1);
                VerifyNoError(context);
                return SnapshotOrNoContent(new RestSnapshot( OidStrategy, context, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage DeleteProperty(string domainType, string instanceId, string propertyName, ReservedArguments arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentContext, RestControlFlags> args = ProcessDeleteArgument(arguments);
                PropertyContextSurface context = Surface.DeleteProperty(Surface.OidTranslator.GetOidTranslation(domainType, instanceId), propertyName, args.Item1);
                VerifyNoError(context);
                return SnapshotOrNoContent(new RestSnapshot( OidStrategy, context, Request, args.Item2), args.Item2.ValidateOnly);
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
                Tuple<ArgumentsContext, RestControlFlags> args = ProcessArgumentMap(arguments, false, domainType, instanceId, true);
                ActionResultContextSurface context = Surface.ExecuteObjectAction(Surface.OidTranslator.GetOidTranslation(domainType, instanceId), actionName, args.Item1);
                VerifyNoError(context);
                return SnapshotOrNoContent(new RestSnapshot( OidStrategy, context, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage PostInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentsContext, RestControlFlags> args = ProcessArgumentMap(arguments, false, domainType, instanceId);
                ActionResultContextSurface result = Surface.ExecuteObjectAction(Surface.OidTranslator.GetOidTranslation(domainType, instanceId), actionName, args.Item1);
                VerifyNoError(result);
                return SnapshotOrNoContent(new RestSnapshot( OidStrategy, result, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage PutInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                VerifyActionType(domainType, instanceId, actionName, "PUT");
                HandleReadOnlyRequest();
                Tuple<ArgumentsContext, RestControlFlags> args = ProcessArgumentMap(arguments, false, domainType, instanceId);
                ActionResultContextSurface result = Surface.ExecuteObjectAction(Surface.OidTranslator.GetOidTranslation(domainType, instanceId), actionName, args.Item1);
                VerifyNoError(result);
                return SnapshotOrNoContent(new RestSnapshot( OidStrategy, result, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage GetInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                VerifyActionType(serviceName, actionName, "GET");
                Tuple<ArgumentsContext, RestControlFlags> args = ProcessArgumentMap(arguments, false, true);
                ActionResultContextSurface result = Surface.ExecuteServiceAction( OidStrategy.Surface.OidTranslator.GetOidTranslation(serviceName), actionName, args.Item1);
                VerifyNoError(result);
                return SnapshotOrNoContent(new RestSnapshot( OidStrategy, result, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage PutInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                VerifyActionType(serviceName, actionName, "PUT");
                HandleReadOnlyRequest();
                Tuple<ArgumentsContext, RestControlFlags> args = ProcessArgumentMap(arguments, false, true);
                ActionResultContextSurface result = Surface.ExecuteServiceAction(OidStrategy.Surface.OidTranslator.GetOidTranslation(serviceName), actionName, args.Item1);
                VerifyNoError(result);
                return SnapshotOrNoContent(new RestSnapshot( OidStrategy, result, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage PostInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => {
                HandleReadOnlyRequest();
                Tuple<ArgumentsContext, RestControlFlags> args = ProcessArgumentMap(arguments, false, true);
                ActionResultContextSurface result = Surface.ExecuteServiceAction(OidStrategy.Surface.OidTranslator.GetOidTranslation(serviceName), actionName, args.Item1);
                VerifyNoError(result);
                return SnapshotOrNoContent(new RestSnapshot( OidStrategy, result, Request, args.Item2), args.Item2.ValidateOnly);
            });
        }

        public virtual HttpResponseMessage GetInvokeIsTypeOf(string typeName, string actionName, ArgumentMap arguments) {
            return InitAndHandleErrors(() => new RestSnapshot(OidStrategy , GetIsTypeOf(new TypeActionInvokeContext(actionName, typeName), arguments), Request, GetFlags(arguments)));
        }

        #endregion

        #region helpers

        private Tuple<ArgumentContext, RestControlFlags> ToTuple(Tuple<object, RestControlFlags> arguments, string tag) {
            return new Tuple<ArgumentContext, RestControlFlags>(new ArgumentContext {
                Digest = tag,
                Value = arguments.Item1,
                ValidateOnly = arguments.Item2.ValidateOnly
            }, arguments.Item2);
        }


        private RestControlFlags GetFlags(ReservedArguments arguments) {
            if (arguments.IsMalformed) {
                throw new BadRequestNOSException("Malformed arguments"); // todo i18n
            }
            return RestControlFlags.FlagsFromArguments(arguments.ValidateOnly, arguments.DomainModel);
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


        private void VerifyNoError(ActionResultContextSurface result) {
            if (result.ActionContext.VisibleParameters.Any(p => !string.IsNullOrEmpty(p.Reason))) {
                if (result.ActionContext.VisibleParameters.Any(p => p.ErrorCause == Cause.WrongType)) {
                    throw new BadRequestNOSException("Bad Request", result.ActionContext.VisibleParameters.Cast<ContextSurface>().ToList());
                }

                throw new BadArgumentsNOSException("Arguments invalid", result.ActionContext.VisibleParameters.Cast<ContextSurface>().ToList());
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

        private void VerifyNoError(PropertyContextSurface propertyContext) {
            if (!string.IsNullOrEmpty(propertyContext.Reason)) {
                throw new BadArgumentsNOSException("Arguments invalid", propertyContext);
            }
        }

        private void VerifyNoError(ObjectContextSurface objectContext) {
            if (objectContext.VisibleProperties.Any(p => !string.IsNullOrEmpty(p.Reason))) {
                if (objectContext.VisibleProperties.Any(p => p.ErrorCause == Cause.WrongType)) {
                    throw new BadRequestNOSException("Bad Request", objectContext.VisibleProperties.Cast<ContextSurface>().ToList());
                }

                throw new BadArgumentsNOSException("Arguments invalid", objectContext.VisibleProperties.Cast<ContextSurface>().ToList());
            }
            if (!string.IsNullOrEmpty(objectContext.Reason)) {
                if (objectContext.ErrorCause == Cause.WrongType) {
                    throw new BadRequestNOSException("Bad Request", objectContext);
                }

                throw new BadArgumentsNOSException("Arguments invalid", objectContext);
            }
        }

        private void VerifyNoPersistError(ObjectContextSurface objectContext, RestControlFlags flags) {
            if (objectContext.VisibleProperties.Any(p => !string.IsNullOrEmpty(p.Reason))) {
                throw new BadPersistArgumentsException("Arguments invalid", objectContext.VisibleProperties.Cast<ContextSurface>().ToList(), flags);
            }
        }

        private void VerifyActionType(ActionContextSurface context, string method) {
            if (method.ToUpper() == "PUT" && !(context.Action.IsQueryOnly || context.Action.IsIdempotent)) {
                throw new NotAllowedNOSException("action is not idempotent"); // i18n 
            }

            if (method.ToUpper() == "GET" && !context.Action.IsQueryOnly) {
                throw new NotAllowedNOSException("action is not side-effect free"); // i18n 
            }
        }

        private void VerifyActionType(string sName, string actionName, string method) {
            ActionContextSurface context = Surface.GetServiceAction(OidStrategy.Surface.OidTranslator.GetOidTranslation(sName), actionName);
            VerifyActionType(context, method);
        }

        private void VerifyActionType(string domainType, string instanceId, string actionName, string method) {
            ActionContextSurface context = Surface.GetObjectAction(Surface.OidTranslator.GetOidTranslation(domainType, instanceId), actionName);
            VerifyActionType(context, method);
        }

        private HttpResponseMessage InitAndHandleErrors(Func<RestSnapshot> f) {
            bool success = false;
            RestSnapshot ss;
            try {
                Surface.Start();
                ss = f();
                success = true;
            }
            catch (HttpResponseException e) {
                Logger.DebugFormat("HttpResponseException being passed up {0}", e.Message);
                throw;
            }
            catch (NakedObjectsSurfaceException e) {
                return ErrorMsg(e);
            }
            catch (Exception e) {
                Logger.ErrorFormat("Unhandled exception from Surface {0} {1}", e.GetType(), e.Message);
                return ErrorMsg(e);
            }
            finally {
                Surface.End(success);
            }

            try {
                return ConfigureMsg(ss.Populate());
            }
            catch (HttpResponseException e) {
                Logger.DebugFormat("HttpResponseException being passed up {0}", e.Message);
                throw;
            }
            catch (NakedObjectsSurfaceException e) {
                return ErrorMsg(e);
            }
            catch (Exception e) {
                Logger.ErrorFormat("Unhandled exception while configuring message {0} {1}", e.GetType(), e.Message);
                throw;
            }
        }

        private HttpResponseMessage ErrorMsg(Exception e) {
            Logger.InfoFormat("ErrorMsg - NakedObjectsSurfaceException Msg: {0}", e.Message);
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
                object parm = argument.Value.GetValue(Surface, new UriMtHelper(OidStrategy ,Request), OidStrategy);
                return new Tuple<object, RestControlFlags>(parm, GetFlags(argument));
            });
        }

        private Tuple<IDictionary<string, object>, RestControlFlags> ExtractValuesAndFlags(ArgumentMap arguments, bool errorIfNone) {
            return HandleMalformed(() => {
                ValidateArguments(arguments, errorIfNone);
                Dictionary<string, object> dictionary = arguments.Map.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetValue(Surface, new UriMtHelper(OidStrategy, Request), OidStrategy));
                return new Tuple<IDictionary<string, object>, RestControlFlags>(dictionary, GetFlags(arguments));
            });
        }

        private TypeActionInvokeContext GetIsTypeOf(TypeActionInvokeContext context, ArgumentMap arguments) {
            ValidateArguments(arguments);

            if (!arguments.Map.ContainsKey(context.ParameterId)) {
                throw new BadRequestNOSException("Malformed arguments");
            }

            ITypeFacade thisSpecification = Surface.GetDomainType(context.TypeName);
            IValue parameter = arguments.Map[context.ParameterId];
            object value = parameter.GetValue(Surface, new UriMtHelper(OidStrategy ,Request), OidStrategy);
            var otherSpecification = (ITypeFacade) (value is ITypeFacade ? value : Surface.GetDomainType((string) value));
            context.ThisSpecification = thisSpecification;
            context.OtherSpecification = otherSpecification;
            return context;
        }


        private Tuple<ArgumentsContext, RestControlFlags> ProcessPersistArguments(ArgumentMap persistArgumentMap) {
            Tuple<IDictionary<string, object>, RestControlFlags> tuple = ExtractValuesAndFlags(persistArgumentMap, true);


            return new Tuple<ArgumentsContext, RestControlFlags>(new ArgumentsContext {
                Digest = null,
                Values = tuple.Item1,
                ValidateOnly = tuple.Item2.ValidateOnly
            }, tuple.Item2);
        }

        private Tuple<ArgumentsContext, RestControlFlags> ProcessArgumentMap(ArgumentMap arguments, bool errorIfNone, string domainType, string instanceId, bool ignoreConcurrency = false) {
            if (!ignoreConcurrency && domainType != null) {
                ObjectContextSurface contextSurface = Surface.GetObject(Surface.OidTranslator.GetOidTranslation(domainType, instanceId));
                ignoreConcurrency = contextSurface.Specification.IsService || contextSurface.Specification.IsImmutable(contextSurface.Target);
            }

            return ProcessArgumentMap(arguments, errorIfNone, ignoreConcurrency);
        }

        private Tuple<ArgumentsContext, RestControlFlags> ProcessArgumentMap(ArgumentMap arguments, bool errorIfNone, bool ignoreConcurrency = false) {
            Tuple<IDictionary<string, object>, RestControlFlags> valuesAndFlags = ExtractValuesAndFlags(arguments, errorIfNone);
            return new Tuple<ArgumentsContext, RestControlFlags>(new ArgumentsContext {
                Digest = ignoreConcurrency ? null : GetIfMatchTag(),
                Values = valuesAndFlags.Item1,
                ValidateOnly = valuesAndFlags.Item2.ValidateOnly,
                SearchTerm = arguments.SearchTerm
            }, valuesAndFlags.Item2);
        }

        private Tuple<ArgumentContext, RestControlFlags> ProcessArgument(SingleValueArgument argument) {
            return ToTuple(ExtractValueAndFlags(argument), GetIfMatchTag());
        }

        private Tuple<ArgumentContext, RestControlFlags> ProcessDeleteArgument(ReservedArguments arguments) {
            return ToTuple(new Tuple<object, RestControlFlags>(null, GetFlags(arguments)), GetIfMatchTag());
        }


        private static IDictionary<string, string> GetOptionalCapabilities() {
            return new Dictionary<string, string> {
                {"protoPersistentObjects", "yes"},
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