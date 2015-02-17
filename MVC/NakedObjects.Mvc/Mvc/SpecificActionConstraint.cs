// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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
                var controllerName = (string) values["controller"];
                var actionName = (string) values["action"];

                return valuesToMatch.Any(kvp => controllerName == (string) kvp.Value && actionName == kvp.Key);
            }

            return false;
        }
    }
}