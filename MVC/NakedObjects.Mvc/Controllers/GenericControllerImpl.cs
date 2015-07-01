// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Resources;
using NakedObjects.Web.Mvc.Models;
using Common.Logging;
using NakedObjects.Facade.Utility.Restricted;
using NakedObjects.Web.Mvc.Helpers;

namespace NakedObjects.Web.Mvc.Controllers {
    public abstract class GenericControllerImpl : NakedObjectsController {
        private static readonly ILog Log = LogManager.GetLogger<GenericControllerImpl>();

        #region actions

        protected GenericControllerImpl(IFrameworkFacade facade, IIdHelper idHelper) : base(facade, idHelper) {}

        [HttpGet]
        public virtual ActionResult Details(ObjectAndControlData controlData) {
            Debug.Assert(controlData.SubAction == ObjectAndControlData.SubActionType.Details ||
                         controlData.SubAction == ObjectAndControlData.SubActionType.None);

            var nakedObject = controlData.GetNakedObject(Facade);
            nakedObject = FilterCollection(nakedObject, controlData);
            SetNewCollectionFormats(controlData);
            return AppropriateView(controlData, nakedObject);
        }

        [HttpGet]
        public virtual ActionResult EditObject(ObjectAndControlData controlData) {
            Debug.Assert(controlData.SubAction == ObjectAndControlData.SubActionType.None);
            return View("ObjectEdit", controlData.GetNakedObject(Facade).GetDomainObject());
        }

        [HttpPost]
        public virtual ActionResult Details(ObjectAndControlData controlData, FormCollection form) {
            Decrypt(form);
            controlData.Form = form;
            Debug.Assert(controlData.SubAction == ObjectAndControlData.SubActionType.Redisplay ||
                         controlData.SubAction == ObjectAndControlData.SubActionType.Details ||
                         controlData.SubAction == ObjectAndControlData.SubActionType.Cancel ||
                         controlData.SubAction == ObjectAndControlData.SubActionType.None);
            var nakedObject = FilterCollection(controlData.GetNakedObject(Facade), controlData);
            SetExistingCollectionFormats(form);
            SetNewCollectionFormats(controlData);

            nakedObject.SetIsNotQueryableState(true);

            if (controlData.SubAction == ObjectAndControlData.SubActionType.Cancel && nakedObject.IsTransient && nakedObject.IsUserPersistable) {
                // remove from cache and return to last object 
                Session.RemoveFromCache(Facade, nakedObject, ObjectCache.ObjectFlag.BreadCrumb);
                return AppropriateView(controlData, null);
            }
            string property = DisplaySingleProperty(controlData, controlData.DataDict);
            return AppropriateView(controlData, nakedObject, null, property);
        }

        [HttpPost]
        public virtual ActionResult EditObject(ObjectAndControlData controlData, FormCollection form) {
            Decrypt(form);
            controlData.Form = form;
            var nakedObject = controlData.GetNakedObject(Facade);
            SetExistingCollectionFormats(form);

            if (nakedObject.IsNotPersistent) {
                RefreshTransient(nakedObject, form);
            }

            switch (controlData.SubAction) {
                case (ObjectAndControlData.SubActionType.Action):
                    SetNewCollectionFormats(controlData);
                    return ActionOnNotPersistentObject(controlData);
                case (ObjectAndControlData.SubActionType.None):
                    AddAttemptedValuesNew(nakedObject, controlData);
                    return View("ObjectEdit", nakedObject.GetDomainObject());
                case (ObjectAndControlData.SubActionType.Pager):
                    SetNewCollectionFormats(controlData);
                    return AppropriateView(controlData, nakedObject);
                case (ObjectAndControlData.SubActionType.Redisplay):
                    return Redisplay(controlData);
            }
            Log.ErrorFormat("SubAction handling not implemented in EditObject for {0}", controlData.SubAction.ToString());
            throw new NotImplementedException(controlData.SubAction.ToString());
        }

        [HttpPost]
        public virtual ActionResult Edit(ObjectAndControlData controlData, FormCollection form) {
            Decrypt(form);
            controlData.Form = form;
            AddFilesToControlData(controlData);

            var nakedObject = controlData.GetNakedObject(Facade);
            RefreshTransient(nakedObject, form);
            SetExistingCollectionFormats(form);
            AddAttemptedValuesNew(nakedObject, controlData);

            switch (controlData.SubAction) {
                case (ObjectAndControlData.SubActionType.Find):
                    return Find(controlData);
                case (ObjectAndControlData.SubActionType.Select):
                    return Select(controlData);
                case (ObjectAndControlData.SubActionType.ActionAsFind):
                    return ActionAsFind(controlData);
                case (ObjectAndControlData.SubActionType.InvokeActionAsFind):
                    return InvokeActionAsFind(controlData);
                case (ObjectAndControlData.SubActionType.InvokeActionAsSave):
                    return InvokeActionAsSave(controlData);
                case (ObjectAndControlData.SubActionType.Redisplay):
                    return Redisplay(controlData);
                case (ObjectAndControlData.SubActionType.None):
                    return ApplyEdit(controlData);
                case (ObjectAndControlData.SubActionType.SaveAndClose):
                    return ApplyEditAndClose(controlData);
                case (ObjectAndControlData.SubActionType.Action):
                    return ApplyEditAction(controlData);
            }
            Log.ErrorFormat("SubAction handling not implemented in Edit for {0}", controlData.SubAction.ToString());
            throw new NotImplementedException(controlData.SubAction.ToString());
        }

        [HttpGet]
        public virtual ActionResult Action(ObjectAndControlData controlData) {
            var no = controlData.GetNakedObject(Facade);
            var action = controlData.GetAction(Facade);

            return View("ActionDialog", new FindViewModel {
                ContextObject = no.GetDomainObject(),
                ContextAction = action
            });
        }

        [HttpPost]
        public virtual ActionResult Action(ObjectAndControlData controlData, FormCollection form) {
            Decrypt(form);
            controlData.Form = form;
            AddFilesToControlData(controlData);
            AddAttemptedValues(controlData);

            switch (controlData.SubAction) {
                case (ObjectAndControlData.SubActionType.Find):
                    return Find(controlData);
                case (ObjectAndControlData.SubActionType.Select):
                    return SelectOnAction(controlData);
                case (ObjectAndControlData.SubActionType.ActionAsFind):
                    return ActionAsFind(controlData);
                case (ObjectAndControlData.SubActionType.InvokeActionAsFind):
                    return InvokeActionAsFind(controlData);
                case (ObjectAndControlData.SubActionType.InvokeActionAsSave):
                    return InvokeActionAsSave(controlData);
                case (ObjectAndControlData.SubActionType.Action):
                    return InitialAction(controlData);
                case (ObjectAndControlData.SubActionType.Details):
                    return Details(controlData);
                case (ObjectAndControlData.SubActionType.None):
                    SetNewCollectionFormats(controlData);
                    return ApplyAction(controlData);
            }

            Log.ErrorFormat("SubAction handling not implemented in Action for {0}", controlData.SubAction.ToString());
            throw new NotImplementedException(controlData.SubAction.ToString());
        }

        public virtual FileContentResult GetFile(string Id, string PropertyId) {
            var oid = Facade.OidTranslator.GetOidTranslation(Id);
            var tgt = Facade.GetObject(oid).Target;

            var p = Facade.GetProperty(oid, PropertyId);
            var domainObject = p.Property.GetValue(tgt);

            return AsFile(domainObject);
        }

        #endregion

        #region private

        private ActionResult ActionOnNotPersistentObject(ObjectAndControlData controlData) {
            string targetActionId = controlData.DataDict["targetActionId"];
            string targetObjectId = controlData.DataDict["targetObjectId"];

            var targetNakedObject = GetNakedObjectFromId(targetObjectId);
            if (targetNakedObject.Specification.IsCollection) {
                var filteredNakedObject = FilterCollection(targetNakedObject, controlData);

                var elementSpec = targetNakedObject.ElementSpecification;
                Trace.Assert(elementSpec != null);
                var targetAction = elementSpec.GetCollectionContributedActions().Single(a => a.Id == targetActionId);

                if (!filteredNakedObject.ToEnumerable().Any()) {
                    Facade.MessageBroker.AddWarning("No objects selected");
                    return AppropriateView(controlData, targetNakedObject, null);
                }

                // force any result to not be queryable
                filteredNakedObject.SetIsNotQueryableState(true);

                return ExecuteAction(controlData, filteredNakedObject, targetAction);
            }
            else {
                var oid = Facade.OidTranslator.GetOidTranslation(targetNakedObject);
                var targetAction = Facade.GetObjectAction(oid, targetActionId).Action;

                //var targetAction = NakedObjectsContext.GetActions(targetNakedObject).Single(a => a.Id == targetActionId);
                return ExecuteAction(controlData, targetNakedObject, targetAction);
            }
        }

        private IObjectFacade GetResult(ActionResultContextFacade context) {

            if (context.HasResult && context.Result != null) {
                var result = context.Result;

                if (result.RedirectedUrl != null) {
                    throw new RedirectException(result.RedirectedUrl);
                }

                return result.Target;
            }
            return null;
        }

        private ActionResult ExecuteAction(ObjectAndControlData controlData, IObjectFacade nakedObject, IActionFacade action) {
            if (ActionExecutingAsContributed(action, nakedObject) && action.ParameterCount == 1) {
                // contributed action being invoked with a single parm that is the current target
                // no dialog - go straight through 

                var ac = new ArgumentsContextFacade {Values = new Dictionary<string, object>(), ValidateOnly = false};

                if (nakedObject.Specification.IsCollection && !nakedObject.Specification.IsParseable) {
                    var oids = nakedObject.ToEnumerable().Select(no => Facade.OidTranslator.GetOidTranslation(no)).ToArray();
                    var spec = nakedObject.ElementSpecification;

                    var ar = Facade.ExecuteListAction(oids, spec, action.Id, ac);
                    return AppropriateView(controlData, GetResult(ar), action);
                }
                else {
                    var oid = Facade.OidTranslator.GetOidTranslation(nakedObject);
                    var ar = Facade.ExecuteObjectAction(oid, action.Id, ac);

                    return AppropriateView(controlData, GetResult(ar), action);
                }
            }

            if (!action.Parameters.Any()) {
                var ac = new ArgumentsContextFacade {Values = new Dictionary<string, object>(), ValidateOnly = false};
                var oid = Facade.OidTranslator.GetOidTranslation(nakedObject);
                var result = Facade.ExecuteObjectAction(oid, action.Id, ac);

                return AppropriateView(controlData, GetResult(result), action);
            }

            SetDefaults(nakedObject, action);
            // do after any parameters set by contributed action so this takes priority
            SetSelectedParameters(action);
            SetPagingValues(controlData, nakedObject);
            var property = DisplaySingleProperty(controlData, controlData.DataDict);

            return View(property == null ? "ActionDialog" : "PropertyEdit", new FindViewModel {ContextObject = nakedObject.GetDomainObject(), ContextAction = action, PropertyName = property});
        }

        private ActionResult InitialAction(ObjectAndControlData controlData) {
            var nakedObject = controlData.GetNakedObject(Facade);
            var nakedObjectAction = controlData.GetAction(Facade);
            CheckConcurrency(nakedObject, null, controlData, (z, x, y) => IdHelper.GetConcurrencyActionInputId(x, nakedObjectAction, y));
            return ExecuteAction(controlData, nakedObject, nakedObjectAction);
        }

        private bool HasError(ActionResultContextFacade ar) {
            return !string.IsNullOrEmpty(ar.ActionContext.Reason) || ar.ActionContext.VisibleParameters.Any(p => !string.IsNullOrEmpty(p.Reason));
        }

        private ActionResult ApplyAction(ObjectAndControlData controlData) {
            var targetNakedObject = FilterCollection(controlData.GetNakedObject(Facade), controlData);
            var targetAction = controlData.GetAction(Facade);

            CheckConcurrency(targetNakedObject, null, controlData, (z, x, y) => IdHelper.GetConcurrencyActionInputId(x, targetAction, y));

            if (targetNakedObject.IsNotPersistent) {
                RefreshTransient(targetNakedObject, controlData.Form);
            }

            // do after any parameters set by contributed action so this takes priority
            SetSelectedParameters(targetAction);

            var ac = GetParameterValues(targetAction, controlData);
            ActionResultContextFacade ar;

            if (targetNakedObject.Specification.IsCollection && !targetNakedObject.Specification.IsParseable) {
                var oids = targetNakedObject.ToEnumerable().Select(no => Facade.OidTranslator.GetOidTranslation(no)).ToArray();
                var spec = targetNakedObject.ElementSpecification;

                ar = Facade.ExecuteListAction(oids, spec, targetAction.Id, ac);
            }
            else {
                var oid = Facade.OidTranslator.GetOidTranslation(targetNakedObject);
                ar = Facade.ExecuteObjectAction(oid, targetAction.Id, ac);
            }

            if (!HasError(ar)) {
                targetNakedObject.SetIsNotQueryableState(targetAction.IsContributed);
                return AppropriateView(controlData, GetResult(ar), targetAction);
            }

            foreach (var parm in ar.ActionContext.VisibleParameters) {
                if (!string.IsNullOrEmpty(parm.Reason)) {
                    ModelState.AddModelError(IdHelper.GetParameterInputId(targetAction, parm.Parameter), parm.Reason);
                }
            }

            if (!(string.IsNullOrEmpty(ar.ActionContext.Reason))) {
                ModelState.AddModelError("", ar.ActionContext.Reason);
            }

            var property = DisplaySingleProperty(controlData, controlData.DataDict);
            return View(property == null ? "ActionDialog" : "PropertyEdit", new FindViewModel {ContextObject = targetNakedObject.GetDomainObject(), ContextAction = targetAction, PropertyName = property});
        }

        private ActionResult Find(ObjectAndControlData controlData) {
            string spec = controlData.DataDict["spec"];
            string contextObjectId = controlData.DataDict["contextObjectId"];
            string propertyName = controlData.DataDict["propertyName"];
            string contextActionId = controlData.DataDict["contextActionId"];

            var objectSet = Session.CachedObjectsOfType(Facade, Facade.GetDomainType(spec)).ToList();

            if (!objectSet.Any()) {
                Log.InfoFormat("No Cached objects of type {0} found", spec);
                Facade.MessageBroker.AddWarning("No objects of appropriate type viewed recently");
            }
            var contextNakedObject = FilterCollection(GetNakedObjectFromId(contextObjectId), controlData);
            var contextAction = string.IsNullOrEmpty(contextActionId) ? null : contextNakedObject.Specification.GetActionLeafNodes().Single(a => a.Id == contextActionId);

            if (objectSet.Count == 1) {
                var selectedItem = new Dictionary<string, string> {{propertyName, GetObjectId(objectSet.Single())}};
                return SelectSingleItem(contextNakedObject, contextAction, controlData, selectedItem);
            }

            return View(Request.IsAjaxRequest() ? "PropertyEdit" : "FormWithSelections", new FindViewModel {ActionResult = objectSet, ContextObject = contextNakedObject.GetDomainObject(), ContextAction = contextAction, PropertyName = propertyName});
        }

        private ActionResult SelectSingleItem(IObjectFacade nakedObject, IActionFacade action, ObjectAndControlData controlData, IDictionary<string, string> selectedItem) {
            var property = DisplaySingleProperty(controlData, selectedItem);

            if (action == null) {
                SetSelectedReferences(nakedObject, selectedItem);
                return property == null ? View("ObjectEdit", nakedObject.GetDomainObject()) : View("PropertyEdit", new PropertyViewModel(nakedObject.GetDomainObject(), property));
            }
            SetSelectedParameters(nakedObject, action, selectedItem);

            return View(property == null ? "ActionDialog" : "PropertyEdit", new FindViewModel {ContextObject = nakedObject.GetDomainObject(), ContextAction = action, PropertyName = property});
        }

        private void ApplyInlineEdit(IObjectFacade nakedObject, ObjectAndControlData controlData, IEnumerable<IAssociationFacade> assocs) {
            var form = controlData.Form;
            // inline or one or more keys in form starts with the property id which indicates we have nested values for the subobject 
            foreach (var assoc in assocs.Where(a => a.IsInline || form.AllKeys.Any(k => IdHelper.KeyPrefixIs(k, a.Id)))) {
                var inlineNakedObject = assoc.GetValue(nakedObject);
                if (inlineNakedObject != null) {
                    ApplyEdit(inlineNakedObject, controlData, assoc);
                }
            }
        }

      
        private bool ApplyEdit(IObjectFacade nakedObject, ObjectAndControlData controlData, IAssociationFacade parent = null) {
            var oid = Facade.OidTranslator.GetOidTranslation(nakedObject);

            var usableAndVisibleFields = nakedObject.Specification.Properties.Where(p => p.IsVisible(nakedObject) && p.IsUsable(nakedObject).IsAllowed);
            var fieldsAndMatchingValues = GetFieldsAndMatchingValues(nakedObject, parent, usableAndVisibleFields, controlData, GetFieldInputId).ToList();

            CheckConcurrency(nakedObject, null, controlData, GetConcurrencyFieldInputId);

            fieldsAndMatchingValues.ForEach(pair => AddAttemptedValue(GetFieldInputId(parent, nakedObject, pair.Item1), pair.Item2));

            var ac = new ArgumentsContextFacade {
                Values = fieldsAndMatchingValues.ToDictionary(f => f.Item1.Id, f => GetObjectValue(f.Item1, nakedObject, f.Item2)),
                ValidateOnly = false
            };

            // check mandatory fields first to emulate WPF UI behaviour where no validation takes place until 
            // all mandatory fields are set. 
            foreach (var pair in fieldsAndMatchingValues) {
                var result = pair.Item2;
                var stringResult = result as string;

                if (pair.Item1.IsMandatory && (result == null || (result is string && string.IsNullOrEmpty(stringResult)))) {
                    AddErrorAndAttemptedValue(nakedObject, stringResult, pair.Item1, MvcUi.Mandatory);
                }
            }

            if (!ModelState.IsValid) {
                return false;
            }

            ApplyInlineEdit(nakedObject, controlData, usableAndVisibleFields);

            if (!ModelState.IsValid) {
                return false;
            }

            var res = Facade.PutObject(oid, ac);

            if (HasError(res)) {
                foreach (var parm in res.VisibleProperties) {
                    if (!string.IsNullOrEmpty(parm.Reason)) {
                        ModelState.AddModelError(GetFieldInputId(parent, nakedObject, parm.Property), parm.Reason);
                    }
                }

                if (!(string.IsNullOrEmpty(res.Reason))) {
                    ModelState.AddModelError("", res.Reason);
                }

                return false;
            }

            return true;
        }

        private ActionResult ApplyEdit(ObjectAndControlData controlData) {
            var nakedObject = controlData.GetNakedObject(Facade);
            var viewName = ApplyEdit(nakedObject, controlData) ? "ObjectView" : "ObjectEdit";
            return View(viewName, nakedObject.GetDomainObject());
        }

        private ActionResult ApplyEditAndClose(ObjectAndControlData controlData) {
            var nakedObject = controlData.GetNakedObject(Facade);
            if (ApplyEdit(nakedObject, controlData)) {
                // last object or home
                object lastObject = Session.LastObject(Facade, ObjectCache.ObjectFlag.BreadCrumb);
                if (lastObject == null) {
                    return RedirectHome();
                }

                nakedObject = Facade.GetObject(lastObject);
                return AppropriateView(controlData, nakedObject);
            }
            return View("ObjectEdit", nakedObject.GetDomainObject());
        }

        private ActionResult ApplyEditAction(ObjectAndControlData controlData) {
            var nakedObject = controlData.GetNakedObject(Facade);
            var ok = ApplyEdit(nakedObject, controlData);
            if (ok) {
                string targetActionId = controlData.DataDict["targetActionId"];
                var oid = Facade.OidTranslator.GetOidTranslation(nakedObject);
                var targetAction = Facade.GetObjectAction(oid, targetActionId).Action;
                return ExecuteAction(controlData, nakedObject, targetAction);
            }
            return View("ViewModel", nakedObject.GetDomainObject());
        }

        private ActionResult Redisplay(ObjectAndControlData controlData) {
            SetNewCollectionFormats(controlData);
            var property = DisplaySingleProperty(controlData, controlData.DataDict);
            var isEdit = bool.Parse(controlData.DataDict["editMode"]);
            var nakedObject = controlData.GetNakedObject(Facade);
            return property == null ? View(isEdit ? "ObjectEdit" : "ObjectView", nakedObject.GetDomainObject()) :
                View(isEdit ? "PropertyEdit" : "PropertyView", new PropertyViewModel(nakedObject.GetDomainObject(), property));
        }

        private ActionResult Select(ObjectAndControlData controlData) {
            return SelectSingleItem(controlData.GetNakedObject(Facade), null, controlData, controlData.DataDict);
        }

        private ActionResult SelectOnAction(ObjectAndControlData controlData) {
            var nakedObjectAction = controlData.GetAction(Facade);
            var contextNakedObject = FilterCollection(controlData.GetNakedObject(Facade), controlData);

            return SelectSingleItem(contextNakedObject, nakedObjectAction, controlData, controlData.DataDict);
        }

        private ActionResult InvokeActionAsSave(ObjectAndControlData controlData) {
            var form = controlData.Form;
            string targetActionId = controlData.DataDict["targetActionId"];
            string targetObjectId = controlData.DataDict["targetObjectId"];
            string contextObjectId = controlData.DataDict["contextObjectId"];
            string propertyName = controlData.DataDict["propertyName"];
            string contextActionId = controlData.DataDict["contextActionId"];
            string subEditObjectId = controlData.DataDict["subEditObjectId"];

            var targetNakedObject = GetNakedObjectFromId(targetObjectId);
            var contextNakedObject = FilterCollection(GetNakedObjectFromId(contextObjectId), controlData);
            var targetAction = targetNakedObject.Specification.GetActionLeafNodes().Single(a => a.Id == targetActionId);
            var contextAction = string.IsNullOrEmpty(contextActionId) ? null : contextNakedObject.Specification.GetActionLeafNodes().Single(a => a.Id == contextActionId);
            var subEditObject = GetNakedObjectFromId(subEditObjectId);

            var oid = Facade.OidTranslator.GetOidTranslation(subEditObject);
            var ac = ConvertForSave(subEditObject, controlData);

            var result = Facade.PutObject(oid, ac);

            foreach (var p in result.VisibleProperties) {
                string key = GetFieldInputId(null, subEditObject, p.Property);
                if (!string.IsNullOrEmpty(p.Reason)) {
                    // kludge to keep ui the same 
                    string reason = p.Reason == MvcUi.Mandatory ? MvcUi.Mandatory : MvcUi.InvalidEntry;
                    ModelState.AddModelError(key, reason);
                }
                AddAttemptedValue(key, p.Property.Specification.IsParseable ? p.ProposedValue : p.ProposedObjectFacade.GetDomainObject<object>());
            }

            if (!string.IsNullOrEmpty(result.Reason)) {
                ModelState.AddModelError(string.Empty, result.Reason);
            }

            // tempting to try to associate the new object at once - however it is still transient until the end of the 
            // transaction and so association may not work (possible persistent to transient). By doing this we split into two transactions 
            // and so all OK. 

            IEnumerable resultAsEnumerable = new List<object> {result.Target.GetDomainObject()};
            return View(Request.IsAjaxRequest() ? "PropertyEdit" : "FormWithSelections", new FindViewModel {
                ActionResult = resultAsEnumerable,
                TargetObject = targetNakedObject.GetDomainObject(),
                ContextObject = contextNakedObject.GetDomainObject(),
                TargetAction = targetAction,
                ContextAction = contextAction,
                PropertyName = propertyName
            });
        }

        private ActionResult InvokeActionAsFind(ObjectAndControlData controlData) {
            string targetActionId = controlData.DataDict["targetActionId"];
            string targetObjectId = controlData.DataDict["targetObjectId"];
            string contextObjectId = controlData.DataDict["contextObjectId"];
            string propertyName = controlData.DataDict["propertyName"];
            string contextActionId = controlData.DataDict["contextActionId"];

            var targetNakedObject = GetNakedObjectFromId(targetObjectId);
            var contextNakedObject = FilterCollection(GetNakedObjectFromId(contextObjectId), controlData);
            var targetAction = targetNakedObject.Specification.GetActionLeafNodes().Single(a => a.Id == targetActionId);
            var contextAction = string.IsNullOrEmpty(contextActionId) ? null : contextNakedObject.Specification.GetActionLeafNodes().Single(a => a.Id == contextActionId);

            SetContextObjectAsParameterValue(targetAction, contextNakedObject);

            var oid = Facade.OidTranslator.GetOidTranslation(targetNakedObject);
            var parms = GetParameterValues(targetAction, controlData);
            var context = Facade.ExecuteObjectAction(oid, targetActionId, parms);

            var result = GetResult(context);

            if (result != null) {
                IEnumerable resultAsEnumerable = !result.Specification.IsCollection ? new List<object> {result.GetDomainObject()} : result.GetDomainObject<IEnumerable>();

                if (resultAsEnumerable.Cast<object>().Count() == 1) {
                    var selectedItem = new Dictionary<string, string> {{propertyName, GetObjectId(resultAsEnumerable.Cast<object>().Single())}};
                    return SelectSingleItem(contextNakedObject, contextAction, controlData, selectedItem);
                }
                string view = Request.IsAjaxRequest() ? "PropertyEdit" : "FormWithSelections";

                return View(view, new FindViewModel {
                    ActionResult = resultAsEnumerable,
                    TargetObject = targetNakedObject.GetDomainObject(),
                    ContextObject = contextNakedObject.GetDomainObject(),
                    TargetAction = (targetAction),
                    ContextAction = (contextAction),
                    PropertyName = propertyName
                });
            }

            return View(Request.IsAjaxRequest() ? "PropertyEdit" : "FormWithFinderDialog", new FindViewModel {
                TargetObject = targetNakedObject.GetDomainObject(),
                ContextObject = contextNakedObject.GetDomainObject(),
                TargetAction = (targetAction),
                ContextAction = (contextAction),
                PropertyName = propertyName
            });
        }

        private static bool ContextParameterIsCollection(IActionFacade contextAction, string propertyName) {
            if (contextAction != null) {
                var parameter = contextAction.Parameters.Single(p => p.Id == propertyName);
                return parameter.Specification.IsCollection;
            }
            return false;
        }

        private string GetObjectId(object domainObject) {
            var no = Facade.GetObject(domainObject);
            return Facade.OidTranslator.GetOidTranslation(no).ToString();
        }

        private ActionResult ActionAsFind(ObjectAndControlData controlData) {
            string targetActionId = controlData.DataDict["targetActionId"];
            string targetObjectId = controlData.DataDict["targetObjectId"];
            string contextObjectId = controlData.DataDict["contextObjectId"];
            string propertyName = controlData.DataDict["propertyName"];
            string contextActionId = controlData.DataDict["contextActionId"];

            var targetNakedObject = GetNakedObjectFromId(targetObjectId);
            var contextNakedObject = FilterCollection(GetNakedObjectFromId(contextObjectId), controlData);
            var targetAction = targetNakedObject.Specification.GetActionLeafNodes().Single(a => a.Id == targetActionId);
            var contextAction = string.IsNullOrEmpty(contextActionId) ? null : contextNakedObject.Specification.GetActionLeafNodes().Single(a => a.Id == contextActionId);

            SetContextObjectAsParameterValue(targetAction, contextNakedObject);
            if (targetAction.ParameterCount == 0) {
                var oid = Facade.OidTranslator.GetOidTranslation(targetNakedObject);

                var context = Facade.ExecuteObjectAction(oid, targetAction.Id, new ArgumentsContextFacade {
                    Values = new Dictionary<string, object>(),
                    ValidateOnly = false
                });

                var result = GetResult(context);

                IEnumerable resultAsEnumerable = GetResultAsEnumerable(result, contextAction, propertyName);

                if (resultAsEnumerable.Cast<object>().Count() == 1 && !result.IsTransient) {
                    var selectedItem = new Dictionary<string, string> {{propertyName, GetObjectId(resultAsEnumerable.Cast<object>().Single())}};
                    return SelectSingleItem(contextNakedObject, contextAction, controlData, selectedItem);
                }

                string view = Request.IsAjaxRequest() ? "PropertyEdit" : "FormWithSelections";
                return View(view, new FindViewModel {
                    ActionResult = resultAsEnumerable,
                    TargetObject = targetNakedObject.GetDomainObject(),
                    ContextObject = contextNakedObject.GetDomainObject(),
                    TargetAction = (targetAction),
                    ContextAction = (contextAction),
                    PropertyName = propertyName
                });
            }

            SetDefaults(targetNakedObject, targetAction);
            return View(Request.IsAjaxRequest() ? "PropertyEdit" : "FormWithFinderDialog", new FindViewModel {
                TargetObject = targetNakedObject.GetDomainObject(),
                ContextObject = contextNakedObject.GetDomainObject(),
                TargetAction = (targetAction),
                ContextAction = (contextAction),
                PropertyName = propertyName
            });
        }

        private static IEnumerable GetResultAsEnumerable(IObjectFacade result, IActionFacade contextAction, string propertyName) {
            if (result != null) {
                if (result.Specification.IsCollection && !ContextParameterIsCollection(contextAction, propertyName)) {
                    return result.GetDomainObject<IEnumerable>();
                }
                return new List<object> {result.GetDomainObject()};
            }
            return new List<object>();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext) {
            base.OnActionExecuted(filterContext); // end the transaction 
            UpdateViewAndController(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext) {
            if (filterContext.Exception != null) {
                Exception e = filterContext.Exception;
                Log.ErrorFormat("GenericControllerImpl:OnException handling Type: {0} Message : {1} Trace : {2}", e.GetType(), e.Message, e.StackTrace);
            }
            else {
                // assume this will never happen. 
                Log.Error("GenericControllerImpl:OnException handling exception but exception is null");
            }

            if (filterContext.Exception is DataUpdateNOSException) {
                filterContext.Result = View("DataUpdateError", filterContext.Exception);
                filterContext.ExceptionHandled = true;
            }
            else if (filterContext.Exception is PreconditionFailedNOSException) {
                filterContext.Result = View("ConcurrencyError", (PreconditionFailedNOSException) filterContext.Exception);
                filterContext.ExceptionHandled = true;
            }
            else if (filterContext.Exception is ObjectResourceNotFoundNOSException) {
                filterContext.Result = View("DestroyedError");
                filterContext.ExceptionHandled = true;
            }
            else if (filterContext.Exception is NakedObjectsFacadeException) {
                filterContext.Result = View("DomainError", filterContext.Exception);
                filterContext.ExceptionHandled = true;
            }
            else if (filterContext.Exception is RedirectException) {
                var e = (RedirectException) filterContext.Exception;
                filterContext.Result = Redirect(e.Url);
                filterContext.ExceptionHandled = true;
            }

            base.OnException(filterContext);
        }

        #endregion
    }
}