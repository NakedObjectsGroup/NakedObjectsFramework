// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace NakedObjects.Web.Mvc {


    //routes.MapRoute(
    //    "SpecificAction",
    //    "{controller}/{action}/{id}",
    //    new {},
    //    new {
    //            controller = new SpecificActionConstraint(new {
    //                                                              CreateNewExpenseItem = "Claim",
    //                                                              SaveNewExpenseItem = "Claim"
    //                                                          })
    //        }
    //    );

    //routes.MapRoute(
    //    "SpecificAction",
    //    "{controller}/{action}/{id}",
    //    new { },
    //    new {
    //        controller = new SpecificActionConstraint(new {
    //            CreateNewExpenseItem = "Claim",
    //            SaveNewExpenseItem = "Claim",
    //            EditP3 = "Claim",
    //            SelectStage = "Claim"
    //        })
    //    }
    //    );

    public class SpecificActionConstraint : IRouteConstraint {
        private readonly RouteValueDictionary valuesToMatch;

        public SpecificActionConstraint(object values) {
            valuesToMatch = new RouteValueDictionary(values);
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection) {
            if (values.ContainsKey("controller") && values.ContainsKey("action")) {
                var controllerName = (string)values["controller"];
                var actionName = (string)values["action"];

                return valuesToMatch.Any(kvp => controllerName == (string)kvp.Value && actionName == kvp.Key);
            }

            return false;
        }
    }
}
