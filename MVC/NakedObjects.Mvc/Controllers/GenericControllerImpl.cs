// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using NakedObjects.Web.Mvc.Helpers;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Web.Mvc.Models;

namespace NakedObjects.Web.Mvc.Controllers {
    public abstract class GenericControllerImpl : NakedObjectsController {
        private static readonly ILog Log = LogManager.GetLogger<GenericControllerImpl>();

        #region actions

        protected GenericControllerImpl(INakedObjectsFramework nakedObjectsContext, INakedObjectsSurface surface,  IIdHelper idHelper) : base(nakedObjectsContext, surface,  idHelper) { }

        //[HttpGet]
        //public virtual ActionResult Details(ObjectAndControlData controlData) {
        //    Assert.AssertTrue(controlData.SubAction == ObjectAndControlData.SubActionType.Details ||
        //                      controlData.SubAction == ObjectAndControlData.SubActionType.None);
        //    var nakedObject = controlData.GetNakedObject(NakedObjectsContext);
        //    nakedObject = FilterCollection(nakedObject, controlData);
        //    SetNewCollectionFormats(controlData);
        //    return AppropriateView(controlData, nakedObject);
        //}

        [HttpGet]
        public virtual ActionResult Details(ObjectAndControlData controlData) {
            Assert.AssertTrue(controlData.SubAction == ObjectAndControlData.SubActionType.Details ||
                              controlData.SubAction == ObjectAndControlData.SubActionType.None);

            var nakedObject = controlData.GetNakedObject(Surface);
            nakedObject = FilterCollection(nakedObject, controlData);
            SetNewCollectionFormats(controlData);
            return AppropriateView(controlData, nakedObject);
        }


        [HttpGet]
        public virtual ActionResult EditObject(ObjectAndControlData controlData) {
            Assert.AssertTrue(controlData.SubAction == ObjectAndControlData.SubActionType.None);
            return View("ObjectEdit", controlData.GetNakedObject(Surface).Object);
        }

        // temp kludge 
        private void SetNotQueryable(INakedObjectSurface no, bool isNotQueryable) {
            INakedObjectAdapter noa = ((dynamic)no).WrappedNakedObject;
            noa.SetNotQueryable(isNotQueryable);
        }


        [HttpPost]
        public virtual ActionResult Details(ObjectAndControlData controlData, FormCollection form) {
            Decrypt(form);
            controlData.Form = form;
            Assert.AssertTrue(controlData.SubAction == ObjectAndControlData.SubActionType.Redisplay ||
                              controlData.SubAction == ObjectAndControlData.SubActionType.Details ||
                              controlData.SubAction == ObjectAndControlData.SubActionType.Cancel ||
                              controlData.SubAction == ObjectAndControlData.SubActionType.None);
            var nakedObject = FilterCollection(controlData.GetNakedObject(Surface), controlData);
            SetExistingCollectionFormats(form);
            SetNewCollectionFormats(controlData);
            
            // TODO temp hack 
            SetNotQueryable(nakedObject, true);


            if (controlData.SubAction == ObjectAndControlData.SubActionType.Cancel && nakedObject.IsTransient() && nakedObject.IsUserPersistable()) {
                // remove from cache and return to last object 
                Session.RemoveFromCache(Surface, nakedObject, ObjectCache.ObjectFlag.BreadCrumb);
                return AppropriateView(controlData, (INakedObjectSurface)null);
            }
            string property = DisplaySingleProperty(controlData, controlData.DataDict);
            return AppropriateView(controlData, nakedObject, null, property);
        }


        //[HttpPost]
        //public virtual ActionResult Details(ObjectAndControlData controlData, FormCollection form) {
        //    Decrypt(form);
        //    controlData.Form = form;
        //    Assert.AssertTrue(controlData.SubAction == ObjectAndControlData.SubActionType.Redisplay ||
        //                      controlData.SubAction == ObjectAndControlData.SubActionType.Details ||
        //                      controlData.SubAction == ObjectAndControlData.SubActionType.Cancel ||
        //                      controlData.SubAction == ObjectAndControlData.SubActionType.None);
        //    INakedObjectAdapter nakedObject = FilterCollection(controlData.GetNakedObject(NakedObjectsContext), controlData);
        //    SetExistingCollectionFormats(form);
        //    SetNewCollectionFormats(controlData);
        //    nakedObject.SetNotQueryable(true);

        //    if (controlData.SubAction == ObjectAndControlData.SubActionType.Cancel &&
        //        nakedObject.ResolveState.IsTransient() &&
        //        nakedObject.Spec.Persistable == PersistableType.UserPersistable) {
        //        // remove from cache and return to last object 
        //        Session.RemoveFromCache(NakedObjectsContext, nakedObject, ObjectCache.ObjectFlag.BreadCrumb);
        //        return AppropriateView(controlData, (INakedObjectAdapter)null);
        //    }
        //    string property = DisplaySingleProperty(controlData, controlData.DataDict);
        //    return AppropriateView(controlData, nakedObject, null, property);
        //}

        //// TODO this is confusingly named - either find a better name or split into two functions
        //[HttpPost]
        //public virtual ActionResult EditObject(ObjectAndControlData controlData, FormCollection form) {
        //    Decrypt(form);
        //    controlData.Form = form;
        //    INakedObjectAdapter nakedObject = controlData.GetNakedObject(NakedObjectsContext);
        //    SetExistingCollectionFormats(form);

        //    if (nakedObject.IsNotPersistent()) {
        //        RefreshTransient(nakedObject, form);
        //    }

        //    switch (controlData.SubAction) {
        //        case (ObjectAndControlData.SubActionType.Action):
        //            SetNewCollectionFormats(controlData);
        //            return ActionOnNotPersistentObject(controlData);
        //        case (ObjectAndControlData.SubActionType.None):
        //            AddAttemptedValues(nakedObject, controlData);
        //            return View("ObjectEdit", nakedObject.Object);
        //        case (ObjectAndControlData.SubActionType.Pager):
        //            SetNewCollectionFormats(controlData);
        //            return AppropriateView(controlData, nakedObject);
        //        case (ObjectAndControlData.SubActionType.Redisplay):
        //            return Redisplay(controlData);
        //    }
        //    Log.ErrorFormat("SubAction handling not implemented in EditObject for {0}", controlData.SubAction.ToString());
        //    throw new NotImplementedException(controlData.SubAction.ToString());
        //}

        [HttpPost]
        public virtual ActionResult EditObject(ObjectAndControlData controlData, FormCollection form) {
            Decrypt(form);
            controlData.Form = form;
            var nakedObject = controlData.GetNakedObject(Surface);
            SetExistingCollectionFormats(form);

            if (nakedObject.IsNotPersistent()) {
                RefreshTransient(nakedObject, form);
            }

            switch (controlData.SubAction) {
                case (ObjectAndControlData.SubActionType.Action):
                    SetNewCollectionFormats(controlData);
                    return ActionOnNotPersistentObject(controlData);
                case (ObjectAndControlData.SubActionType.None):
                    AddAttemptedValuesNew(nakedObject, controlData);
                    return View("ObjectEdit", nakedObject.Object);
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

            var nakedObject = controlData.GetNakedObject(Surface);
            RefreshTransient(nakedObject, form);
            SetExistingCollectionFormats( form);
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

        //[HttpPost]
        //public virtual ActionResult Edit(ObjectAndControlData controlData, FormCollection form) {
        //    Decrypt(form);
        //    controlData.Form = form;
        //    AddFilesToControlData(controlData);

        //    var nakedObject = controlData.GetNakedObject(NakedObjectsContext);
        //    RefreshTransient(nakedObject, form);
        //    SetExistingCollectionFormats(form);
        //    AddAttemptedValues(nakedObject, controlData);

        //    switch (controlData.SubAction) {
        //        case (ObjectAndControlData.SubActionType.Find):
        //            return Find(controlData);
        //        case (ObjectAndControlData.SubActionType.Select):
        //            return Select(controlData);
        //        case (ObjectAndControlData.SubActionType.ActionAsFind):
        //            return ActionAsFind(controlData);
        //        case (ObjectAndControlData.SubActionType.InvokeActionAsFind):
        //            return InvokeActionAsFind(controlData);
        //        case (ObjectAndControlData.SubActionType.InvokeActionAsSave):
        //            return InvokeActionAsSave(controlData);
        //        case (ObjectAndControlData.SubActionType.Redisplay):
        //            return Redisplay(controlData);
        //        case (ObjectAndControlData.SubActionType.None):
        //            return ApplyEdit(controlData);
        //        case (ObjectAndControlData.SubActionType.SaveAndClose):
        //            return ApplyEditAndClose(controlData);
        //        case (ObjectAndControlData.SubActionType.Action):
        //            return ApplyEditAction(controlData);
        //    }
        //    Log.ErrorFormat("SubAction handling not implemented in Edit for {0}", controlData.SubAction.ToString());
        //    throw new NotImplementedException(controlData.SubAction.ToString());
        //}

        // Not clear that this is ever called
        //[HttpGet]
        //public virtual ActionResult Action(ObjectAndControlData controlData) {
        //    return View("ActionDialog", new FindViewModel {
        //        ContextObject = controlData.GetNakedObject(NakedObjectsContext).Object,
        //        ContextAction = controlData.GetAction(NakedObjectsContext)
        //    });
        //}

        [HttpGet]
        public virtual ActionResult Action(ObjectAndControlData controlData) {
            var no = controlData.GetNakedObject(Surface);
            var action = controlData.GetAction(Surface);

            return View("ActionDialog", new FindViewModel {
                ContextObject = no.Object,
                ContextAction = ((dynamic)action).WrappedSpec // todo fix hack 
            });
        }

        [HttpPost]
        public virtual ActionResult Action(ObjectAndControlData controlData, FormCollection form) {
            Decrypt(form);
            controlData.Form = form;
            AddFilesToControlData(controlData);
            AddAttemptedValuesNew(controlData);

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

        //[HttpGet]
        //public virtual ActionResult Action(ObjectAndControlData controlData) {
        //    return View("ActionDialog", new FindViewModel {
        //        ContextObject = controlData.GetNakedObject(NakedObjectsContext).Object,
        //        ContextAction = controlData.GetAction(NakedObjectsContext)
        //    });
        //}

        //[HttpPost]
        //public virtual ActionResult Action(ObjectAndControlData controlData, FormCollection form) {
        //    Decrypt(form);
        //    controlData.Form = form;
        //    AddFilesToControlData(controlData);
        //    AddAttemptedValues(controlData);

        //    switch (controlData.SubAction) {
        //        case (ObjectAndControlData.SubActionType.Find):
        //            return Find(controlData);
        //        case (ObjectAndControlData.SubActionType.Select):
        //            return SelectOnAction(controlData);
        //        case (ObjectAndControlData.SubActionType.ActionAsFind):
        //            return ActionAsFind(controlData);
        //        case (ObjectAndControlData.SubActionType.InvokeActionAsFind):
        //            return InvokeActionAsFind(controlData);
        //        case (ObjectAndControlData.SubActionType.InvokeActionAsSave):
        //            return InvokeActionAsSave(controlData);
        //        case (ObjectAndControlData.SubActionType.Action):
        //            return InitialAction(controlData);
        //        case (ObjectAndControlData.SubActionType.Details):
        //            return Details(controlData);
        //        case (ObjectAndControlData.SubActionType.None):
        //            SetNewCollectionFormats(controlData);
        //            return ApplyAction(controlData);
        //    }

        //    Log.ErrorFormat("SubAction handling not implemented in Action for {0}", controlData.SubAction.ToString());
        //    throw new NotImplementedException(controlData.SubAction.ToString());
        //}

        public virtual FileContentResult GetFile(string Id, string PropertyId) {
            //INakedObjectAdapter target = NakedObjectsContext.GetNakedObjectFromId(Id);
            //IAssociationSpec assoc = target.GetObjectSpec().Properties.Single(a => a.Id == PropertyId);
            //var domainObject = assoc.GetNakedObject(target).GetDomainObject();

            var oid = Surface.OidStrategy.GetOid(Id, "");
            var tgt = Surface.GetObject(oid).Target;

            var p = Surface.GetProperty(oid, PropertyId);
            var domainObject = p.Property.GetNakedObject(tgt).Object;



            return AsFile(domainObject);
        }

        #endregion

        #region private

      


        private ActionResult ActionOnNotPersistentObject(ObjectAndControlData controlData) {
            string targetActionId = controlData.DataDict["targetActionId"];
            string targetObjectId = controlData.DataDict["targetObjectId"];

            var targetNakedObject = GetNakedObjectFromId(targetObjectId);
            if (targetNakedObject.Specification.IsCollection()) {
                var filteredNakedObject = FilterCollection(targetNakedObject, controlData);
                //var metamodel = NakedObjectsContext.MetamodelManager.Metamodel;
                //IObjectSpecImmutable elementSpecImmut =
                //    filteredNakedObject.Spec.GetFacet<ITypeOfFacet>().GetValueSpec(filteredNakedObject, metamodel);

                var elementSpec = targetNakedObject.ElementSpecification;
                Trace.Assert(elementSpec != null);
                var targetAction = elementSpec.GetCollectionContributedActions().Single(a => a.Id == targetActionId);

                if (!filteredNakedObject.ToEnumerable().Any()) {
                    NakedObjectsContext.MessageBroker.AddWarning("No objects selected");
                    return AppropriateView(controlData, targetNakedObject, targetAction);
                }

                // force any result to not be queryable
                //filteredNakedObject.SetNotQueryable(true);
                // TODO temp hack 
                SetNotQueryable(filteredNakedObject, true);

                return ExecuteAction(controlData, filteredNakedObject, targetAction);
            }
            else {
                var oid = Surface.OidStrategy.GetOid(targetNakedObject);
                var targetAction = Surface.GetObjectAction(oid, targetActionId).Action;

                //var targetAction = NakedObjectsContext.GetActions(targetNakedObject).Single(a => a.Id == targetActionId);
                return ExecuteAction(controlData, targetNakedObject, targetAction);
            }
        }


        //private ActionResult ActionOnNotPersistentObject(ObjectAndControlData controlData) {
        //    string targetActionId = controlData.DataDict["targetActionId"];
        //    string targetObjectId = controlData.DataDict["targetObjectId"];

        //    INakedObjectAdapter targetNakedObject = NakedObjectsContext.GetNakedObjectFromId(targetObjectId);
        //    if (targetNakedObject.Spec.IsCollection) {
        //        INakedObjectAdapter filteredNakedObject = FilterCollection(targetNakedObject, controlData);
        //        var metamodel = NakedObjectsContext.MetamodelManager.Metamodel;
        //        IObjectSpecImmutable elementSpecImmut =
        //            filteredNakedObject.Spec.GetFacet<ITypeOfFacet>().GetValueSpec(filteredNakedObject, metamodel);

        //        var elementSpec = NakedObjectsContext.MetamodelManager.GetSpecification(elementSpecImmut) as IObjectSpec;
        //        Trace.Assert(elementSpec != null);
        //        var targetAction = elementSpec.GetCollectionContributedActions().Single(a => a.Id == targetActionId);

        //        if (!filteredNakedObject.GetAsEnumerable(NakedObjectsContext.NakedObjectManager).Any()) {
        //            NakedObjectsContext.MessageBroker.AddWarning("No objects selected");
        //            return AppropriateView(controlData, targetNakedObject, targetAction);
        //        }
        //        // force any result to not be queryable
        //        filteredNakedObject.SetNotQueryable(true);
        //        return ExecuteAction(controlData, filteredNakedObject, targetAction);
        //    }
        //    else {
        //        var targetAction = NakedObjectsContext.GetActions(targetNakedObject).Single(a => a.Id == targetActionId);
        //        return ExecuteAction(controlData, targetNakedObject, targetAction);
        //    }
        //}

        private INakedObjectAdapter Execute(IActionSpec action, INakedObjectAdapter target, INakedObjectAdapter[] parameterSet) {
            return action.Execute(target, parameterSet);
        }

        private INakedObjectSurface GetResult(ActionResultContextSurface context) {
            if (context.HasResult) {
                return context.Result.Target;
            }
            return null;
        }

        private ActionResult ExecuteAction(ObjectAndControlData controlData, INakedObjectSurface nakedObject, INakedObjectActionSurface action) {
            if (ActionExecutingAsContributed(action, nakedObject) && action.ParameterCount == 1) {
                // contributed action being invoked with a single parm that is the current target
                //// no dialog - go straight through 
                //var newForm = new FormCollection { { IdHelper.GetParameterInputId(ScaffoldAction.Wrap(action), ScaffoldParm.Wrap(action.Parameters.First())), NakedObjectsContext.GetObjectId(nakedObject) } };

                //// horrid kludge 
                //var oldForm = controlData.Form;
                //controlData.Form = newForm;

                //if (ValidateParameters(nakedObject, action, controlData)) {
                var ac = new ArgumentsContext() {Values = new Dictionary<string, object>(), ValidateOnly = false};
                var oid = Surface.OidStrategy.GetOid(nakedObject);
                var result = Surface.ExecuteObjectAction(oid, action.Id, ac);
                return AppropriateView(controlData, GetResult(result), action);
                //}

                //controlData.Form = oldForm;
                //AddAttemptedValues(controlData);
            }

            if (!action.Parameters.Any()) {
                var ac = new ArgumentsContext() { Values = new Dictionary<string, object>(), ValidateOnly = false };
                var oid = Surface.OidStrategy.GetOid(nakedObject);
                var result = Surface.ExecuteObjectAction(oid, action.Id, ac);

                return AppropriateView(controlData, GetResult(result), action);
            }

            SetDefaults(nakedObject, action);
            // do after any parameters set by contributed action so this takes priority
            SetSelectedParameters(action);
            SetPagingValues(controlData, nakedObject);
            var property = DisplaySingleProperty(controlData, controlData.DataDict);

            // TODO temp hack
            IActionSpec oldAction = ((dynamic) action).WrappedSpec;
            return View(property == null ? "ActionDialog" : "PropertyEdit", new FindViewModel { ContextObject = nakedObject.Object, ContextAction = oldAction, PropertyName = property });
        }


        private ActionResult ExecuteAction(ObjectAndControlData controlData, INakedObjectAdapter nakedObject, IActionSpec action) {
            if (ActionExecutingAsContributed(action, nakedObject) && action.ParameterCount == 1) {
                // contributed action being invoked with a single parm that is the current target
                // no dialog - go straight through 
                var newForm = new FormCollection { { IdHelper.GetParameterInputId(ScaffoldAction.Wrap(action), ScaffoldParm.Wrap( action.Parameters.First())), NakedObjectsContext.GetObjectId(nakedObject) } };

                // horrid kludge 
                var oldForm = controlData.Form;
                controlData.Form = newForm;

                if (ValidateParameters(nakedObject, action, controlData)) {
                    return AppropriateView(controlData, Execute(action, nakedObject, new[] {nakedObject}), action);
                }

                controlData.Form = oldForm;
                AddAttemptedValues(controlData);
            }

            if (!action.Parameters.Any()) {
                return AppropriateView(controlData, Execute(action, nakedObject, new INakedObjectAdapter[] {}), action);
            }

            SetDefaults(nakedObject, action);
            // do after any parameters set by contributed action so this takes priority
            SetSelectedParameters(action);
            SetPagingValues(controlData, nakedObject);
            var property = DisplaySingleProperty(controlData, controlData.DataDict);
            return View(property == null ? "ActionDialog" : "PropertyEdit", new FindViewModel {ContextObject = nakedObject.Object, ContextAction = action, PropertyName = property});
        }

        private ActionResult InitialAction(ObjectAndControlData controlData) {
            var nakedObject = controlData.GetNakedObject(Surface);
            var nakedObjectAction = controlData.GetAction(Surface);
            CheckConcurrency(nakedObject, null, controlData, (z, x, y) => IdHelper.GetConcurrencyActionInputId(x, nakedObjectAction, y));
            return ExecuteAction(controlData, nakedObject, nakedObjectAction);
        }

        //private ActionResult ApplyAction(ObjectAndControlData controlData) {
        //    var targetNakedObject = FilterCollection(controlData.GetNakedObject(NakedObjectsContext), controlData);
        //    var targetAction = controlData.GetAction(NakedObjectsContext);

        //    CheckConcurrency(targetNakedObject, null, controlData, (z, x, y) => IdHelper.GetConcurrencyActionInputId(ScaffoldAdapter.Wrap(x), ScaffoldAction.Wrap(targetAction), ScaffoldAssoc.Wrap(y)));

        //    if (targetNakedObject.IsNotPersistent()) {
        //        RefreshTransient(targetNakedObject, controlData.Form);
        //    }

        //    // do after any parameters set by contributed action so this takes priority
        //    SetSelectedParameters(targetAction);
        //    if (ValidateParameters(targetNakedObject, targetAction, controlData)) {
        //        targetNakedObject.SetNotQueryable(targetAction.IsContributedMethod);
        //        var parms = GetParameterValues(targetAction, controlData);
        //        return AppropriateView(controlData, Execute(targetAction, targetNakedObject, parms.ToArray()), targetAction);
        //    }
        //    var property = DisplaySingleProperty(controlData, controlData.DataDict);
        //    return View(property == null ? "ActionDialog" : "PropertyEdit", new FindViewModel {ContextObject = targetNakedObject.Object, ContextAction = targetAction, PropertyName = property});
        //}

        private bool HasError(ActionResultContextSurface ar) {

            return !string.IsNullOrEmpty(ar.ActionContext.Reason) || ar.ActionContext.VisibleParameters.Any(p => !string.IsNullOrEmpty(p.Reason));
        }

        private ActionResult ApplyAction(ObjectAndControlData controlData) {
            var targetNakedObject = FilterCollection(controlData.GetNakedObject(Surface), controlData);
            var targetAction = controlData.GetAction(Surface);

            CheckConcurrency(targetNakedObject, null, controlData, (z, x, y) => IdHelper.GetConcurrencyActionInputId(x, targetAction, y));

            if (targetNakedObject.IsNotPersistent()) {
                RefreshTransient(targetNakedObject, controlData.Form);
            }

            // do after any parameters set by contributed action so this takes priority
            SetSelectedParameters(targetAction);

            var ac = GetParameterValues(targetAction, controlData);
            var oid = Surface.OidStrategy.GetOid(targetNakedObject);
            var ar = Surface.ExecuteObjectAction(oid, targetAction.Id, ac);

            if (!HasError(ar)) {

                SetNotQueryable(targetNakedObject, targetAction.IsContributed()); // kludge

                return AppropriateView(controlData, GetResult(ar), targetAction);
            }

            foreach (var parm in ar.ActionContext.VisibleParameters) {
                if (!string.IsNullOrEmpty(parm.Reason)) {
                    ModelState.AddModelError(IdHelper.GetParameterInputId(targetAction, parm.Parameter), parm.Reason);
                }
            }

            if ( !(string.IsNullOrEmpty(ar.ActionContext.Reason)))
            {
                ModelState.AddModelError("", ar.ActionContext.Reason);
            }
        


            var property = DisplaySingleProperty(controlData, controlData.DataDict);
            // TODO temp hack
            IActionSpec oldAction = ((dynamic)targetAction).WrappedSpec;
            return View(property == null ? "ActionDialog" : "PropertyEdit", new FindViewModel { ContextObject = targetNakedObject.Object, ContextAction = oldAction, PropertyName = property });
        }



        private ActionResult Find(ObjectAndControlData controlData) {
            string spec = controlData.DataDict["spec"];
            string contextObjectId = controlData.DataDict["contextObjectId"];
            string propertyName = controlData.DataDict["propertyName"];
            string contextActionId = controlData.DataDict["contextActionId"];

            var objectSet = Session.CachedObjectsOfType(NakedObjectsContext, NakedObjectsContext.MetamodelManager.GetSpecification(spec)).ToList();

            if (!objectSet.Any()) {
                Log.InfoFormat("No Cached objects of type {0} found", spec);
                NakedObjectsContext.MessageBroker.AddWarning("No objects of appropriate type viewed recently");
            }
            var contextNakedObject = FilterCollection(NakedObjectsContext.GetNakedObjectFromId(contextObjectId), controlData);
            var contextAction = string.IsNullOrEmpty(contextActionId) ? null : NakedObjectsContext.GetActionFromId(contextActionId);

            if (objectSet.Count == 1) {
                var selectedItem = new Dictionary<string, string> {{propertyName, NakedObjectsContext.GetObjectId(objectSet.Single())}};
                return SelectSingleItem(contextNakedObject, contextAction, controlData, selectedItem);
            }

            return View(Request.IsAjaxRequest() ? "PropertyEdit" : "FormWithSelections", new FindViewModel {ActionResult = objectSet, ContextObject = contextNakedObject.Object, ContextAction = contextAction, PropertyName = propertyName});
        }

        private ActionResult SelectSingleItem(INakedObjectAdapter nakedObject, IActionSpec action, ObjectAndControlData controlData, IDictionary<string, string> selectedItem) {
            var property = DisplaySingleProperty(controlData, selectedItem);

            if (action == null) {
                SetSelectedReferences(nakedObject, selectedItem);
                return property == null ? View("ObjectEdit", nakedObject.Object) :
                    View("PropertyEdit", new PropertyViewModel(nakedObject.Object, property));
            }
            SetSelectedParameters(nakedObject, action, selectedItem);
            return View(property == null ? "ActionDialog" : "PropertyEdit", new FindViewModel {ContextObject = nakedObject.Object, ContextAction = action, PropertyName = property});
        }

        private ActionResult ApplyEdit(ObjectAndControlData controlData) {
            string viewName = "ObjectEdit";
            var nakedObject = controlData.GetNakedObject(NakedObjectsContext);
            if (ValidateChanges(nakedObject, controlData)) {
                viewName = ApplyChanges(nakedObject, controlData) ? "ObjectView" : "ObjectEdit";
            }

            return View(viewName, nakedObject.Object);
        }

        private ActionResult ApplyEditAndClose(ObjectAndControlData controlData) {
            var nakedObject = controlData.GetNakedObject(NakedObjectsContext);
            if (ValidateChanges(nakedObject, controlData)) {
                if (ApplyChanges(nakedObject, controlData)) {
                    // last object or home
                    object lastObject = Session.LastObject(NakedObjectsContext, ObjectCache.ObjectFlag.BreadCrumb);
                    if (lastObject == null) {
                        return RedirectHome();
                    }

                    nakedObject = NakedObjectsContext.GetNakedObject(lastObject);
                    return AppropriateView(controlData, nakedObject);
                }
            }
            return View("ObjectEdit", nakedObject.Object);
        }

        private ActionResult ApplyEditAction(ObjectAndControlData controlData) {
            var nakedObject = controlData.GetNakedObject(NakedObjectsContext);
            var ok = ValidateChanges(nakedObject, controlData) && ApplyChanges(nakedObject, controlData);
            if (ok) {
                string targetActionId = controlData.DataDict["targetActionId"];
                IActionSpec targetAction = NakedObjectsContext.GetActions(nakedObject).Single(a => a.Id == targetActionId);
                return ExecuteAction(controlData, nakedObject, targetAction);
            }
            return View("ViewModel", nakedObject.Object);
        }

        private ActionResult Redisplay(ObjectAndControlData controlData) {
            SetNewCollectionFormats(controlData);
            var property = DisplaySingleProperty(controlData, controlData.DataDict);
            var isEdit = bool.Parse(controlData.DataDict["editMode"]);
            var nakedObject = controlData.GetNakedObject(NakedObjectsContext);
            return property == null ? View(isEdit ? "ObjectEdit" : "ObjectView", nakedObject.Object) :
                View(isEdit ? "PropertyEdit" : "PropertyView", new PropertyViewModel(nakedObject.Object, property));
        }

        private ActionResult Select(ObjectAndControlData controlData) {
            return SelectSingleItem(controlData.GetNakedObject(NakedObjectsContext), null, controlData, controlData.DataDict);
        }

        private ActionResult SelectOnAction(ObjectAndControlData controlData) {
            IActionSpec nakedObjectAction = controlData.GetAction(NakedObjectsContext);
            INakedObjectAdapter contextNakedObject = FilterCollection(controlData.GetNakedObject(NakedObjectsContext), controlData);

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

            INakedObjectAdapter targetNakedObject = NakedObjectsContext.GetNakedObjectFromId(targetObjectId);
            INakedObjectAdapter contextNakedObject = FilterCollection(NakedObjectsContext.GetNakedObjectFromId(contextObjectId), controlData);
            IActionSpec targetAction = NakedObjectsContext.GetActions(targetNakedObject).Single(a => a.Id == targetActionId);
            IActionSpec contextAction = string.IsNullOrEmpty(contextActionId) ? null : NakedObjectsContext.GetActionFromId(contextActionId);
            INakedObjectAdapter subEditObject = NakedObjectsContext.GetNakedObjectFromId(subEditObjectId);

            if (ValidateChanges(subEditObject, controlData)) {
                ApplyChanges(subEditObject, controlData);
            }

            // tempting to try to associate the new object at once - however it is still transient until the end of the 
            // transaction and so association may not work (possible persistent to transient). By doing this we split into two transactions 
            // and so all OK. 

            IEnumerable resultAsEnumerable = new List<object> {subEditObject.Object};
            return View(Request.IsAjaxRequest() ? "PropertyEdit" : "FormWithSelections", new FindViewModel {
                ActionResult = resultAsEnumerable,
                TargetObject = targetNakedObject.Object,
                ContextObject = contextNakedObject.Object,
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

            INakedObjectAdapter targetNakedObject = NakedObjectsContext.GetNakedObjectFromId(targetObjectId);
            INakedObjectAdapter contextNakedObject = FilterCollection(NakedObjectsContext.GetNakedObjectFromId(contextObjectId), controlData);
            IActionSpec targetAction = NakedObjectsContext.GetActions(targetNakedObject).Single(a => a.Id == targetActionId);
            IActionSpec contextAction = string.IsNullOrEmpty(contextActionId) ? null : NakedObjectsContext.GetActionFromId(contextActionId);

            SetContextObjectAsParameterValue(targetAction, contextNakedObject);

            if (ValidateParameters(targetNakedObject, targetAction, controlData)) {
                IEnumerable<INakedObjectAdapter> parms = GetParameterValues(targetAction, controlData);
                INakedObjectAdapter result = targetAction.Execute(targetNakedObject, parms.ToArray());

                if (result != null) {
                    IEnumerable resultAsEnumerable = !result.Spec.IsCollection ? new List<object> {result.Object} : (IEnumerable) result.Object;

                    if (resultAsEnumerable.Cast<object>().Count() == 1) {
                        var selectedItem = new Dictionary<string, string> {{propertyName, NakedObjectsContext.GetObjectId(resultAsEnumerable.Cast<object>().Single())}};
                        return SelectSingleItem(contextNakedObject, contextAction, controlData, selectedItem);
                    }
                    string view = Request.IsAjaxRequest() ? "PropertyEdit" : "FormWithSelections";
                    return View(view, new FindViewModel {
                        ActionResult = resultAsEnumerable,
                        TargetObject = targetNakedObject.Object,
                        ContextObject = contextNakedObject.Object,
                        TargetAction = targetAction,
                        ContextAction = contextAction,
                        PropertyName = propertyName
                    });
                }
            }
            return View(Request.IsAjaxRequest() ? "PropertyEdit" : "FormWithFinderDialog", new FindViewModel {
                TargetObject = targetNakedObject.Object,
                ContextObject = contextNakedObject.Object,
                TargetAction = targetAction,
                ContextAction = contextAction,
                PropertyName = propertyName
            });
        }

        private static bool ContextParameterIsCollection(IActionSpec contextAction, string propertyName) {
            if (contextAction != null) {
                IActionParameterSpec parameter = contextAction.Parameters.Single(p => p.Id == propertyName);
                return parameter.Spec.IsCollection;
            }
            return false;
        }

        private ActionResult ActionAsFind(ObjectAndControlData controlData) {
            string targetActionId = controlData.DataDict["targetActionId"];
            string targetObjectId = controlData.DataDict["targetObjectId"];
            string contextObjectId = controlData.DataDict["contextObjectId"];
            string propertyName = controlData.DataDict["propertyName"];
            string contextActionId = controlData.DataDict["contextActionId"];

            INakedObjectAdapter targetNakedObject = NakedObjectsContext.GetNakedObjectFromId(targetObjectId);
            INakedObjectAdapter contextNakedObject = FilterCollection(NakedObjectsContext.GetNakedObjectFromId(contextObjectId), controlData);
            IActionSpec targetAction = NakedObjectsContext.GetActions(targetNakedObject).Single(a => a.Id == targetActionId);
            IActionSpec contextAction = string.IsNullOrEmpty(contextActionId) ? null : NakedObjectsContext.GetActionFromId(contextActionId);

            SetContextObjectAsParameterValue(targetAction, contextNakedObject);
            if (targetAction.ParameterCount == 0) {
                INakedObjectAdapter result = Execute(targetAction, targetNakedObject, new INakedObjectAdapter[] {});
                IEnumerable resultAsEnumerable = GetResultAsEnumerable(result, contextAction, propertyName);

                if (resultAsEnumerable.Cast<object>().Count() == 1 && result.ResolveState.IsPersistent()) {
                    var selectedItem = new Dictionary<string, string> {{propertyName, NakedObjectsContext.GetObjectId(resultAsEnumerable.Cast<object>().Single())}};
                    return SelectSingleItem(contextNakedObject, contextAction, controlData, selectedItem);
                }

                string view = Request.IsAjaxRequest() ? "PropertyEdit" : "FormWithSelections";
                return View(view, new FindViewModel {
                    ActionResult = resultAsEnumerable,
                    TargetObject = targetNakedObject.Object,
                    ContextObject = contextNakedObject.Object,
                    TargetAction = targetAction,
                    ContextAction = contextAction,
                    PropertyName = propertyName
                });
            }

            SetDefaults(targetNakedObject, targetAction);
            return View(Request.IsAjaxRequest() ? "PropertyEdit" : "FormWithFinderDialog", new FindViewModel {
                TargetObject = targetNakedObject.Object,
                ContextObject = contextNakedObject.Object,
                TargetAction = targetAction,
                ContextAction = contextAction,
                PropertyName = propertyName
            });
        }

        private static IEnumerable GetResultAsEnumerable(INakedObjectAdapter result, IActionSpec contextAction, string propertyName) {
            if (result != null) {
                if (result.Spec.IsCollection && !ContextParameterIsCollection(contextAction, propertyName)) {
                    return (IEnumerable) result.Object;
                }
                return new List<object> {result.Object};
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

            if (filterContext.Exception is DataUpdateException) {
                filterContext.Result = View("DataUpdateError", filterContext.Exception);
                filterContext.ExceptionHandled = true;
            }
            else if (filterContext.Exception is ConcurrencyException) {
                filterContext.Result = View("ConcurrencyError", filterContext.Exception);
                filterContext.ExceptionHandled = true;
            }
            else if (filterContext.Exception is ObjectNotFoundException) {
                filterContext.Result = View("DestroyedError");
                filterContext.ExceptionHandled = true;
            }
            else if (filterContext.Exception is NakedObjectDomainException) {
                filterContext.Result = View("DomainError", filterContext.Exception);
                filterContext.ExceptionHandled = true;
            }

            base.OnException(filterContext);
        }

        #endregion
    }
}