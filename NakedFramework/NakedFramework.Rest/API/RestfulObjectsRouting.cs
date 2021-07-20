// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.API {
    public static class RestfulObjectsRouting {
        private static string PrefixRoute(string segment, string prefix) => string.IsNullOrWhiteSpace(prefix) ? segment : EnsureTrailingSlash(prefix) + segment;
        private static string EnsureTrailingSlash(string path) => path.EndsWith("/") ? path : path + "/";

        public static void AddRestRoutes(IRouteBuilder routes, string routePrefix = "") {
            if (!string.IsNullOrWhiteSpace(routePrefix)) {
                UriMtHelper.GetApplicationPath = req => {
                    var appPath = req.PathBase.ToString();
                    return EnsureTrailingSlash(appPath) + EnsureTrailingSlash(routePrefix);
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

            routes.MapRoute("GetInvokeOnMenu",
                            template: menus + "/{menuName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                            defaults: new {controller = "RestfulObjects", action = "GetInvokeOnMenu"},
                            constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
            );

            routes.MapRoute("PutInvokeOnMenu",
                            template: menus + "/{menuName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                            defaults: new {controller = "RestfulObjects", action = "PutInvokeOnMenu"},
                            constraints: new {httpMethod = new HttpMethodRouteConstraint("PUT")}
            );

            routes.MapRoute("PostInvokeOnMenu",
                            template: menus + "/{menuName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
                            defaults: new {controller = "RestfulObjects", action = "PostInvokeOnMenu"},
                            constraints: new {httpMethod = new HttpMethodRouteConstraint("POST")}
            );

            routes.MapRoute("InvalidInvokeOnMenu",
                            template: menus + "/{menuName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Invoke,
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

            routes.MapRoute("GetParameterPromptOnMenu",
                            template: menus + "/{menuName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}/" + SegmentValues.Prompt,
                            defaults: new {controller = "RestfulObjects", action = "GetParameterPromptOnMenu"},
                            constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
            );

            routes.MapRoute("InvalidParameterPromptOnMenu",
                            template: menus + "/{menuName}/" + SegmentValues.Actions + "/{actionName}/" + SegmentValues.Params + "/{parmName}/" + SegmentValues.Prompt,
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

            routes.MapRoute("MenuAction",
                            template: menus + "/{menuName}/" + SegmentValues.Actions + "/{actionName}",
                            defaults: new {controller = "RestfulObjects", action = "GetMenuAction"},
                            constraints: new {httpMethod = new HttpMethodRouteConstraint("GET")}
            );

            routes.MapRoute("InvalidMenuAction",
                            template: menus + "/{menuName}/" + SegmentValues.Actions + "/{actionName}",
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
    }
}