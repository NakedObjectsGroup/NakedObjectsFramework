// Copyright � Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Web.Mvc;
using NakedObjects.Web.Mvc.Controllers;
using NakedObjects.Web.Mvc.Models;

namespace NakedObjects.Mvc.App.Controllers {

    //[Authorize] 
    public class GenericController : GenericControllerImpl {

        
        public GenericController(INakedObjectsFramework framework) : base(framework) {
            
        }

        #region actions

        [HttpGet]
        public override ActionResult Details(ObjectAndControlData controlData) {
            return base.Details(controlData);
        }

        [HttpGet]
        public override ActionResult EditObject(ObjectAndControlData controlData) {
            return base.EditObject(controlData);
        }

        [HttpGet]
        public override ActionResult Action(ObjectAndControlData controlData) {
            return base.Action(controlData);
        }

        [HttpPost]
        public override ActionResult Details(ObjectAndControlData controlData, FormCollection form) {
            return base.Details(controlData, form);
        }

        [HttpPost]
        public override  ActionResult EditObject(ObjectAndControlData controlData, FormCollection form) {
            return base.EditObject(controlData, form);
        }

        [HttpPost]
        public override ActionResult Edit(ObjectAndControlData controlData, FormCollection form) {
            return base.Edit(controlData, form);
        }

        [HttpPost]
        public override ActionResult Action(ObjectAndControlData controlData, FormCollection form) {
            return base.Action(controlData, form);
        }

        public override FileContentResult GetFile(string Id, string PropertyId) {
            return base.GetFile(Id, PropertyId);
        }

        #endregion

    }
}