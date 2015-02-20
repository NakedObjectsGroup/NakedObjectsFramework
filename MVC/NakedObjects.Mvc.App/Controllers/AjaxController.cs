// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using NakedObjects.Web.Mvc.Controllers;

namespace NakedObjects.Mvc.App.Controllers {
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    //[Authorize]
    public class AjaxController : AjaxControllerImpl {
        public AjaxController(INakedObjectsFramework nakedObjectsContext) : base(nakedObjectsContext) {}

        private bool IsKnownKey(string key) {
            return key == "id" || key == "propertyName" || key == "actionName" || key == "parameterName";
        }

        private string GetValue() {
            var keys = Request.QueryString.AllKeys;

            var otherKeys = keys.Where(k => k.EndsWith("-Input"));

            if (otherKeys.Count() == 1) {
                var valueKey = otherKeys.First();
                var values = Request.QueryString.GetValues(valueKey);
                return values != null && values.Count() == 1 ? values.First() : null;
            }
            return null;
        }

        [HttpGet]
        public override JsonResult ValidateProperty(string id, string value, string propertyName) {
            // behaviour from client libraries has changed and value is now in parm with id of field
            // keep existing value check for backward compatibility but if it fail look for par with id ending in -Input
            return base.ValidateProperty(id, value ?? GetValue(), propertyName);
        }

        [HttpGet]
        public override JsonResult ValidateParameter(string id, string value, string actionName, string parameterName) {
            // see ValidateProperty comment
            return base.ValidateParameter(id, value ?? GetValue(), actionName, parameterName);
        }

        [HttpGet]
        public override JsonResult GetActionChoices(string id, string actionName) {
            return base.GetActionChoices(id, actionName);
        }

        [HttpGet]
        public override JsonResult GetPropertyChoices(string id) {
            return base.GetPropertyChoices(id);
        }
    }
}