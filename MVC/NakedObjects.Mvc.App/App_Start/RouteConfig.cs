using System.Web.Mvc;
using System.Web.Routing;
using MvcTestApp;
using NakedObjects.Web.Mvc;

namespace NakedObjects.Mvc.App
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes) {
            RegisterNakedObjectsRoutes(routes); 

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
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
