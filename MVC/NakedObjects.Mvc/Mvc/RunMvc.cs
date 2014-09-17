// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Web.Mvc;
using System.Web.Routing;

namespace NakedObjects.Web.Mvc {
    public class NakedObjectsRouteConfig {

        public static void RegisterGenericRoutes(RouteCollection routes) {
            routes.MapRoute(
                "NakedObjectsAjax",
                "Ajax/{action}",
                new {controller = "Ajax", action = ""}
                );

            routes.MapRoute(
                "NakedObjectsSystem",
                "Home/{Action}",
                new {controller = "Home", action = "Index"}
                );

            routes.MapRoute(
                "NakedObjectsGetFile",
                "{Object}/GetFile/{file}",
                new {controller = "Generic", action = "GetFile"}
                );

            routes.MapRoute(
                "NakedObjectsDialog",
                "{Object}/Dialog",
                new {controller = "Generic", action = "Dialog"}
                );

            routes.MapRoute(
                "NakedObjectsDetails",
                "{Object}/Details",
                new {controller = "Generic", action = "Details"}
                );

            routes.MapRoute(
                "NakedObjectsEditObject",
                "{Object}/EditObject",
                new {controller = "Generic", action = "EditObject"}
                );

            routes.MapRoute(
                "NakedObjectsEdit",
                "{Object}/Edit",
                new {controller = "Generic", action = "Edit"}
                );

            routes.MapRoute(
                "NakedObjectsAction",
                "{Object}/Action/{ActionId}",
                new {controller = "Generic", action = "Action"}
                );

            routes.MapRoute(
                "NakedObjectsDefault",
                "{controller}/{Action}",
                new {controller = "Home", action = "Index"}
                );
        }

    }
}