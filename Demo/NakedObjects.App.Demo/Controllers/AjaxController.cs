// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Web.Mvc;
using System.Web.UI;
using NakedObjects.Web.Mvc.Controllers;

namespace NakedObjects.App.Demo.Controllers {

    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    //[Authorize]
    public class AjaxController : AjaxControllerImpl {
        [HttpGet]
        public override JsonResult ValidateProperty(string id, string value, string propertyName) {
            return base.ValidateProperty(id, value, propertyName);
        }

        [HttpGet]
        public override JsonResult ValidateParameter(string id, string value, string actionName, string parameterName) {
            return base.ValidateParameter(id, value, actionName, parameterName);
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