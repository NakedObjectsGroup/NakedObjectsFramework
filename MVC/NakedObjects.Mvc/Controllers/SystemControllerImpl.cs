// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Web.Mvc;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using NakedObjects.Web.Mvc.Models;

namespace NakedObjects.Web.Mvc.Controllers {
    public abstract class SystemControllerImpl : NakedObjectsController {
        protected SystemControllerImpl(IFrameworkFacade surface, IIdHelper idHelper) : base(surface, idHelper) {}

        public virtual ActionResult ClearHistory(bool clearAll) {
            object lastObject = Session.LastObject(Surface, ObjectCache.ObjectFlag.BreadCrumb);
            Session.ClearCachedObjects(ObjectCache.ObjectFlag.BreadCrumb);
            if (lastObject == null || clearAll) {
                return RedirectToAction(IdConstants.IndexAction, IdConstants.HomeName);
            }
            SetControllerName(lastObject);
            return View(Surface.GetObject(lastObject));
        }

        public virtual ActionResult ClearHistoryItem(string id, string nextId, ObjectAndControlData controlData) {
            Session.RemoveFromCache(id, ObjectCache.ObjectFlag.BreadCrumb);
            return Cancel(nextId, controlData);
        }

        public virtual ActionResult ClearHistoryOthers(string id, ObjectAndControlData controlData) {
            var nakedObject = GetNakedObjectFromId(id);
            Session.RemoveOthersFromCache(Surface, nakedObject.Object, ObjectCache.ObjectFlag.BreadCrumb);
            SetNewCollectionFormats(controlData);
            SetControllerName(nakedObject.Object);
            return AppropriateView(controlData, nakedObject);
        }

        public virtual ActionResult Cancel(string nextId, ObjectAndControlData controlData) {
            var nextNakedObject = string.IsNullOrEmpty(nextId) ? null : GetNakedObjectFromId(nextId);

            if (nextNakedObject == null) {
                return RedirectToAction(IdConstants.IndexAction, IdConstants.HomeName);
            }

            SetNewCollectionFormats(controlData);
            SetControllerName(nextNakedObject.Object);
            return AppropriateView(controlData, nextNakedObject);
        }

        private ActionResult View(IObjectFacade nakedObject) {
            string viewName = nakedObject.IsViewModelEditView ? "ViewModel" : "ObjectView";
            return View(viewName, nakedObject.Object);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext) {
            base.OnActionExecuted(filterContext);
            UpdateViewAndController(filterContext);
        }
    }
}