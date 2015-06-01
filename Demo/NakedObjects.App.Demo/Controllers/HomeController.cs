// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Web.Mvc;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using NakedObjects.Web.Mvc.Controllers;
using NakedObjects.Web.Mvc.Models;

namespace NakedObjects.Mvc.App.Controllers {

    //[Authorize]
    public class HomeController : SystemControllerImpl {
        public HomeController( IFrameworkFacade surface, IIdHelper idHelper) : base( surface, idHelper) {}

        public ActionResult Index() {
            return View();
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        [HttpPost]
        public override ActionResult ClearHistory(bool clearAll) {
            return base.ClearHistory(clearAll);
        }

        [HttpPost]
        public override ActionResult ClearHistoryItem(string id, string nextId, ObjectAndControlData controlData) {
            return base.ClearHistoryItem(id, nextId, controlData);
        }

        [HttpPost]
        public override ActionResult Cancel(string nextId, ObjectAndControlData controlData) {
            return base.Cancel(nextId, controlData);
        }

        [HttpPost]
        public override ActionResult ClearHistoryOthers(string id, ObjectAndControlData controlData) {
            return base.ClearHistoryOthers(id, controlData);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (Request.Browser.Type.ToUpper() == "IE6" || Request.Browser.Type.ToUpper() == "IE7") {
                filterContext.Result = View("BrowserError");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}