// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Web.Mvc;
using System.Web.Routing;
using NakedObjects.Web.Mvc;

namespace AnotherTestofNOF7 {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            RegisterNakedObjectsRoutes(routes);

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                );
        }

        private static void RegisterNakedObjectsRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{*favicon}", new {favicon = @"(.*/)?favicon.ico(/.*)?"});
            routes.IgnoreRoute("{*nakedobjects}", new {nakedobjects = @"(.*/)?nakedobjects.ico(/.*)?"});
            routes.IgnoreRoute("Content/{*wildcard}");
            routes.IgnoreRoute("Scripts/{*wildcard}");
            routes.IgnoreRoute("Images/{*wildcard}");
            routes.IgnoreRoute("fonts/{*wildcard}");

            routes.RouteExistingFiles = true; //This is to stop Attachments (where link name includes file extension)
            //from being intercepted by web server. (This also necessitates the additional IgnoreRoutes above).

            RestfulObjectsConfig.RegisterRestfulObjectsRoutes(routes); // must be rest first 
            NakedObjectsRouteConfig.RegisterGenericRoutes(routes);
        }
    }
}