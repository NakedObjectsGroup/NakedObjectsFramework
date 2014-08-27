// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Web.Mvc;
using Common.Logging;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Util;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Web.Mvc.Models;

namespace NakedObjects.Web.Mvc.Controllers {
    public abstract class GenericControllerImpl : NakedObjectsController {

        private static readonly ILog Log = LogManager.GetLogger<GenericControllerImpl>();

        #region actions

        protected GenericControllerImpl(INakedObjectsFramework nakedObjectsContext) : base(nakedObjectsContext) {}

        [HttpGet]
        public virtual ActionResult Details(ObjectAndControlData controlData) {
            Assert.AssertTrue(controlData.SubAction == ObjectAndControlData.SubActionType.Details ||
                              controlData.SubAction == ObjectAndControlData.SubActionType.None);
            INakedObject nakedObject = FilterCollection(controlData.GetNakedObject(), controlData);
            SetNewCollectionFormats(controlData);
            return AppropriateView(controlData, nakedObject);
        }

        [HttpGet]
        public virtual ActionResult EditObject(ObjectAndControlData controlData) {
            Assert.AssertTrue(controlData.SubAction == ObjectAndControlData.SubActionType.None);
            return View("ObjectEdit", controlData.GetNakedObject().Object);
        }

        [HttpPost]
        public virtual ActionResult Details(ObjectAndControlData controlData, FormCollection form) {
            Decrypt(form);
            controlData.Form = form;
            Assert.AssertTrue(controlData.SubAction == ObjectAndControlData.SubActionType.Redisplay ||
                              controlData.SubAction == ObjectAndControlData.SubActionType.Details ||
                              controlData.SubAction == ObjectAndControlData.SubActionType.Cancel ||
                              controlData.SubAction == ObjectAndControlData.SubActionType.None);
            INakedObject nakedObject = FilterCollection(controlData.GetNakedObject(), controlData);
            SetExistingCollectionFormats(nakedObject, form);
            SetNewCollectionFormats(controlData);
            nakedObject.SetNotQueryable(true);

            if (controlData.SubAction == ObjectAndControlData.SubActionType.Cancel &&
                nakedObject.ResolveState.IsTransient() &&
                nakedObject.Specification.Persistable == Persistable.USER_PERSISTABLE) {
                // remove from cache and return to last object 
                Session.RemoveFromCache(nakedObject, ObjectCache.ObjectFlag.BreadCrumb);
                return AppropriateView(controlData, null);
            }
            string property = DisplaySingleProperty(controlData, controlData.DataDict);
            return AppropriateView(controlData, nakedObject, null, property);
        }

 
        // TODO this is confusingly named - either find a better name or split into two functions
        [HttpPost]
        public virtual ActionResult EditObject(ObjectAndControlData controlData, FormCollection form) {
            Decrypt(form);
            controlData.Form = form;
            INakedObject nakedObject = controlData.GetNakedObject();
            SetExistingCollectionFormats(nakedObject, form);

            if (nakedObject.IsNotPersistent()) {
                RefreshTransient(nakedObject, form);
            }

            switch (controlData.SubAction) {
                case (ObjectAndControlData.SubActionType.Action):
                    SetNewCollectionFormats(controlData);
                    return ActionOnNotPersistentObject(controlData);
                case (ObjectAndControlData.SubActionType.None):
                    AddAttemptedValues(nakedObject, controlData);
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
            RefreshTransient(controlData.GetNakedObject(), form);
            SetExistingCollectionFormats(controlData.GetNakedObject(), form);
            AddAttemptedValues(controlData.GetNakedObject(), controlData);

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
                case (ObjectAndControlData.SubActionType.Action):
                    return ApplyEditAction(controlData);
            }
            Log.ErrorFormat("SubAction handling not implemented in Edit for {0}", controlData.SubAction.ToString());
            throw new NotImplementedException(controlData.SubAction.ToString());
        }

        [HttpGet]
        public virtual ActionResult Action(ObjectAndControlData controlData) {
            return View("ActionDialog", new FindViewModel {
                ContextObject = controlData.GetNakedObject().Object,
                ContextAction = controlData.GetAction()
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
            INakedObject target = FrameworkHelper.GetNakedObjectFromId(Id);
            INakedObjectAssociation assoc = target.Specification.Properties.Single(a => a.Id == PropertyId);
            var domainObject = assoc.GetNakedObject(target, Core.Context.NakedObjectsContext.ObjectPersistor).GetDomainObject();

            return AsFile(domainObject);
        }

        #endregion

        #region private

        private ActionResult ActionOnNotPersistentObject(ObjectAndControlData controlData) {
            string targetActionId = controlData.DataDict["targetActionId"];
            string targetObjectId = controlData.DataDict["targetObjectId"];

            INakedObject targetNakedObject = FrameworkHelper.GetNakedObjectFromId(targetObjectId);
            INakedObject filteredNakedObject = FilterCollection(targetNakedObject, controlData);
            INakedObjectAction targetAction = FrameworkHelper.GetActions(filteredNakedObject).Single(a => a.Id == targetActionId);

            if (filteredNakedObject.Specification.IsCollection) {
               
                if (!filteredNakedObject.GetAsEnumerable(Core.Context.NakedObjectsContext.ObjectPersistor).Any()) {
                    Core.Context.NakedObjectsContext.MessageBroker.AddWarning("No objects selected");
                    return AppropriateView(controlData, targetNakedObject, targetAction);
                }
                // force any result to not be queryable
                filteredNakedObject.SetNotQueryable(true);
            }

            return ExecuteAction(controlData, filteredNakedObject, targetAction);
        }

        private static INakedObject Execute(INakedObjectAction action, INakedObject target, INakedObject[] parameterSet) {
            var result = action.Execute(target, parameterSet, Core.Context.NakedObjectsContext.ObjectPersistor, Core.Context.NakedObjectsContext.Session);
            if (result != null && result.Oid == null) {
                result.SetATransientOid(new CollectionMemento(Core.Context.NakedObjectsContext.ObjectPersistor, Core.Context.NakedObjectsContext.Reflector, Core.Context.NakedObjectsContext.Session, target, action, parameterSet));
            }
            return result;
        }    

        private ActionResult ExecuteAction(ObjectAndControlData controlData, INakedObject nakedObject, INakedObjectAction action) {
            if (ActionExecutingAsContributed(action, nakedObject) && action.ParameterCount == 1) {
                // contributed action being invoked with a single parm that is the current target
                // no dialog - go straight through 
                var newForm = new FormCollection { { IdHelper.GetParameterInputId(action, action.Parameters.First()), FrameworkHelper.GetObjectId(nakedObject) } };
                
                // horrid kludge 
                var oldForm = controlData.Form;
                controlData.Form = newForm; 
       
                if (ValidateParameters(nakedObject, action, controlData)) {
                    return AppropriateView(controlData, Execute(action, nakedObject, new[] { nakedObject }), action);
                }

                controlData.Form = oldForm;
                AddAttemptedValues(controlData);
            }

            if (!action.Parameters.Any()) {
                return AppropriateView(controlData, Execute(action, nakedObject, new INakedObject[] { }), action);
            }

            SetDefaults(nakedObject, action);
            // do after any parameters set by contributed action so this takes priority
            SetSelectedParameters(action);
            SetPagingValues(controlData, nakedObject);
            var property = DisplaySingleProperty(controlData, controlData.DataDict);
            return View(property == null ? "ActionDialog" : "PropertyEdit", new FindViewModel { ContextObject = nakedObject.Object, ContextAction = action, PropertyName = property });
        }

        private ActionResult InitialAction(ObjectAndControlData controlData) {
            CheckConcurrency(controlData.GetNakedObject(), null, controlData, (z, x, y) => IdHelper.GetConcurrencyActionInputId(x, controlData.GetAction(), y));
            return ExecuteAction(controlData, controlData.GetNakedObject(), controlData.GetAction());
        }

        private ActionResult ApplyAction(ObjectAndControlData controlData) {
            var targetNakedObject = FilterCollection(controlData.GetNakedObject(), controlData);
            var targetAction = controlData.GetAction();

            CheckConcurrency(targetNakedObject, null, controlData, (z, x, y) => IdHelper.GetConcurrencyActionInputId(x, targetAction, y));

            if (targetNakedObject.IsNotPersistent()) {
                RefreshTransient(targetNakedObject, controlData.Form);
            }

            // do after any parameters set by contributed action so this takes priority
            SetSelectedParameters(targetAction);
            if (ValidateParameters(targetNakedObject, targetAction, controlData)) {
                targetNakedObject.SetNotQueryable(targetAction.IsContributedMethod);
                var parms = GetParameterValues(targetAction, controlData);
                return AppropriateView(controlData, Execute(targetAction, targetNakedObject, parms.ToArray()), targetAction);
            }
            var property = DisplaySingleProperty(controlData, controlData.DataDict);
            return View(property == null ? "ActionDialog" : "PropertyEdit", new FindViewModel { ContextObject = targetNakedObject.Object, ContextAction = targetAction, PropertyName = property });
        }

        private ActionResult Find(ObjectAndControlData controlData) {
            
            string spec = controlData.DataDict["spec"];
            string contextObjectId = controlData.DataDict["contextObjectId"];
            string propertyName = controlData.DataDict["propertyName"];
            string contextActionId = controlData.DataDict["contextActionId"];

            var objectSet = Session.CachedObjectsOfType(Core.Context.NakedObjectsContext.Reflector.LoadSpecification(spec)).ToList();

            if (!objectSet.Any()) {
                Log.InfoFormat("No Cached objects of type {0} found", spec);
                Core.Context.NakedObjectsContext.MessageBroker.AddWarning("No objects of appropriate type viewed recently");
            }
            var contextNakedObject = FilterCollection(FrameworkHelper.GetNakedObjectFromId(contextObjectId), controlData);
            var contextAction = string.IsNullOrEmpty(contextActionId) ? null : FrameworkHelper.GetActionFromId(contextActionId);

            if (objectSet.Count == 1) {
                var selectedItem = new Dictionary<string, string> {{propertyName, FrameworkHelper.GetObjectId(objectSet.Single())}};
                return SelectSingleItem(contextNakedObject, contextAction, controlData, selectedItem);
            }

            return View(Request.IsAjaxRequest() ? "PropertyEdit" : "FormWithSelections", new FindViewModel {ActionResult = objectSet, ContextObject = contextNakedObject.Object, ContextAction = contextAction, PropertyName = propertyName});
        }

        private ActionResult SelectSingleItem(INakedObject nakedObject, INakedObjectAction action, ObjectAndControlData controlData, IDictionary<string , string> selectedItem) {
          
            var property = DisplaySingleProperty(controlData, selectedItem);

            if (action == null) {
                SetSelectedReferences(nakedObject, selectedItem);
                return property == null ? View("ObjectEdit", nakedObject.Object) :
                                          View("PropertyEdit", new PropertyViewModel(nakedObject.Object, property));
            }
            SetSelectedParameters(nakedObject, action, selectedItem);
            return View(property == null ? "ActionDialog" : "PropertyEdit", new FindViewModel { ContextObject = nakedObject.Object, ContextAction = action, PropertyName = property });
        }


        private ActionResult ApplyEdit(ObjectAndControlData controlData) {
            string viewName = "ObjectEdit";
            if (ValidateChanges(controlData.GetNakedObject(), controlData)) {
                viewName = ApplyChanges(controlData.GetNakedObject(), controlData) ? "ObjectView" : "ObjectEdit";
            }

            return View(viewName, controlData.GetNakedObject().Object);
        }

        private ActionResult ApplyEditAction(ObjectAndControlData controlData) {
            var ok = ValidateChanges(controlData.GetNakedObject(), controlData) && ApplyChanges(controlData.GetNakedObject(), controlData);
            if (ok) {
                string targetActionId = controlData.DataDict["targetActionId"];          
                INakedObjectAction targetAction = FrameworkHelper.GetActions(controlData.GetNakedObject()).Single(a => a.Id == targetActionId);
                return ExecuteAction(controlData, controlData.GetNakedObject(), targetAction);
            }
            return View("ViewModel", controlData.GetNakedObject().Object);
        }


        private ActionResult Redisplay(ObjectAndControlData controlData) {
            SetNewCollectionFormats(controlData);
            var property = DisplaySingleProperty(controlData, controlData.DataDict);
            var isEdit = bool.Parse(controlData.DataDict["editMode"]);
            return property == null ? View(isEdit ? "ObjectEdit" : "ObjectView", controlData.GetNakedObject().Object) :
                                      View(isEdit ? "PropertyEdit" : "PropertyView", new PropertyViewModel(controlData.GetNakedObject().Object, property));
        }

        private ActionResult Select(ObjectAndControlData controlData) {
            return SelectSingleItem(controlData.GetNakedObject(), null, controlData, controlData.DataDict);
        }

        private ActionResult SelectOnAction(ObjectAndControlData controlData) {
            INakedObjectAction nakedObjectAction = controlData.GetAction();
            INakedObject contextNakedObject = FilterCollection(controlData.GetNakedObject(), controlData);

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

            INakedObject targetNakedObject = FrameworkHelper.GetNakedObjectFromId(targetObjectId);
            INakedObject contextNakedObject = FilterCollection(FrameworkHelper.GetNakedObjectFromId(contextObjectId), controlData);
            INakedObjectAction targetAction = FrameworkHelper.GetActions(targetNakedObject).Single(a => a.Id == targetActionId);
            INakedObjectAction contextAction = string.IsNullOrEmpty(contextActionId) ? null : FrameworkHelper.GetActionFromId(contextActionId);
            INakedObject subEditObject = FrameworkHelper.GetNakedObjectFromId(subEditObjectId);

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

            INakedObject targetNakedObject = FrameworkHelper.GetNakedObjectFromId(targetObjectId);
            INakedObject contextNakedObject = FilterCollection(FrameworkHelper.GetNakedObjectFromId(contextObjectId), controlData);
            INakedObjectAction targetAction = FrameworkHelper.GetActions(targetNakedObject).Single(a => a.Id == targetActionId);
            INakedObjectAction contextAction = string.IsNullOrEmpty(contextActionId) ? null : FrameworkHelper.GetActionFromId(contextActionId);

            SetContextObjectAsParameterValue(targetAction, contextNakedObject);

            if (ValidateParameters(targetNakedObject, targetAction, controlData)) {
                IEnumerable<INakedObject> parms = GetParameterValues(targetAction, controlData);
                INakedObject result = targetAction.Execute(targetNakedObject, parms.ToArray(), Core.Context.NakedObjectsContext.ObjectPersistor, Core.Context.NakedObjectsContext.Session);

                if (result != null) {
                    IEnumerable resultAsEnumerable = !result.Specification.IsCollection ? new List<object> {result.Object} : (IEnumerable) result.Object;

                    if (resultAsEnumerable.Cast<object>().Count() == 1) {
                        var selectedItem = new Dictionary<string, string> {{propertyName, FrameworkHelper.GetObjectId(resultAsEnumerable.Cast<object>().Single())}};
                        return SelectSingleItem(contextNakedObject, contextAction,  controlData, selectedItem);
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

        private static bool ContextParameterIsCollection(INakedObjectAction contextAction, string propertyName) {
            if (contextAction != null) {
                INakedObjectActionParameter parameter = contextAction.Parameters.Single(p => p.Id == propertyName);
                return parameter.Specification.IsCollection;
            }
            return false; 
        }

        private ActionResult ActionAsFind(ObjectAndControlData controlData) {
            string targetActionId = controlData.DataDict["targetActionId"];
            string targetObjectId = controlData.DataDict["targetObjectId"];
            string contextObjectId = controlData.DataDict["contextObjectId"];
            string propertyName = controlData.DataDict["propertyName"];
            string contextActionId = controlData.DataDict["contextActionId"];

            INakedObject targetNakedObject = FrameworkHelper.GetNakedObjectFromId(targetObjectId);
            INakedObject contextNakedObject = FilterCollection(FrameworkHelper.GetNakedObjectFromId(contextObjectId), controlData);
            INakedObjectAction targetAction = FrameworkHelper.GetActions(targetNakedObject).Single(a => a.Id == targetActionId);
            INakedObjectAction contextAction = string.IsNullOrEmpty(contextActionId) ? null : FrameworkHelper.GetActionFromId(contextActionId);

            SetContextObjectAsParameterValue(targetAction, contextNakedObject);
            if (targetAction.ParameterCount == 0) {
                INakedObject result = Execute(targetAction, targetNakedObject, new INakedObject[] {});
                IEnumerable resultAsEnumerable = GetResultAsEnumerable(result, contextAction, propertyName);
                
                if (resultAsEnumerable.Cast<object>().Count() == 1 && result.ResolveState.IsPersistent()) {
                    var selectedItem = new Dictionary<string, string> {{propertyName, FrameworkHelper.GetObjectId(resultAsEnumerable.Cast<object>().Single())}};
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

        private static IEnumerable GetResultAsEnumerable(INakedObject result, INakedObjectAction contextAction, string propertyName) {
            if (result != null) {
                if (result.Specification.IsCollection && !ContextParameterIsCollection(contextAction, propertyName) ) {
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