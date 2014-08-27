// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Web.Mvc;
using NakedObjects.Architecture.Adapter;

using NakedObjects.Resources;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Web.Mvc.Models;

namespace NakedObjects.Web.Mvc.Controllers {
    public abstract class SystemControllerImpl : NakedObjectsController {
        protected SystemControllerImpl(INakedObjectsFramework nakedObjectsContext) : base(nakedObjectsContext) {}

        public virtual ActionResult ExecuteFixture(string name) {
            try {
                Core.Context.NakedObjectsContext.FixturesInstaller.InstallFixture(Core.Context.NakedObjectsContext.ObjectPersistor, name);
                ViewData[IdHelper.SystemMessages] = new[] {string.Format(MvcUi.InstalledFixtureText, name)};
            }
            catch (Exception e) {
                ViewData[IdHelper.SystemMessages] = new[] {string.Format(MvcUi.FixtureErrorText, name, e.Message)};
            }
            return View("Index");
        }


        public virtual ActionResult ClearHistory(bool clearAll) {
          
            object lastObject = Session.LastObject(ObjectCache.ObjectFlag.BreadCrumb);
            Session.ClearCachedObjects(ObjectCache.ObjectFlag.BreadCrumb);
            if (lastObject == null || clearAll) {
                return RedirectToAction(IdHelper.IndexAction, IdHelper.HomeName);
            }
            SetControllerName(lastObject);
            return View(FrameworkHelper.GetNakedObject(lastObject));
        }

        public virtual ActionResult ClearHistoryItem(string id, string nextId, ObjectAndControlData controlData) { 
            Session.RemoveFromCache(id, ObjectCache.ObjectFlag.BreadCrumb);
            return Cancel(nextId, controlData);
        }

        public virtual ActionResult ClearHistoryOthers(string id, ObjectAndControlData controlData) {
            var nakedObject = FrameworkHelper.GetNakedObjectFromId(id);         
            Session.RemoveOthersFromCache(nakedObject.Object, ObjectCache.ObjectFlag.BreadCrumb);
            SetNewCollectionFormats(controlData);
            SetControllerName(nakedObject.Object);
            return AppropriateView(controlData, nakedObject);
        }

        public virtual ActionResult Cancel(string nextId, ObjectAndControlData controlData) {
            var nextNakedObject = string.IsNullOrEmpty(nextId) ? null : FrameworkHelper.GetNakedObjectFromId(nextId);

            if (nextNakedObject == null) {
                return RedirectToAction(IdHelper.IndexAction, IdHelper.HomeName);
            }

            SetNewCollectionFormats(controlData);
            SetControllerName(nextNakedObject.Object);
            return AppropriateView(controlData, nextNakedObject);
        }

        private ActionResult View(INakedObject nakedObject) {
            string viewName = nakedObject.IsViewModelEditView() ? "ViewModel" : "ObjectView";
            return View(viewName, nakedObject.Object);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext) {
            base.OnActionExecuted(filterContext);
            UpdateViewAndController(filterContext);
        }
       
    }
}