// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Actions.PageSize;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Facets.Objects.FromStream;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Facets.Objects.Validation;
using NakedObjects.Architecture.Facets.Properties.Version;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Persist;
using NakedObjects.Resources;
using NakedObjects.Value;
using NakedObjects.Web.Mvc.Helpers;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Web.Mvc.Models;

namespace NakedObjects.Web.Mvc.Controllers {
    public abstract class NakedObjectsController : Controller {
        private readonly INakedObjectsFramework nakedObjectsContext;


        protected NakedObjectsController(INakedObjectsFramework nakedObjectsContext) {
            this.nakedObjectsContext = nakedObjectsContext;
        }

        public IEncryptDecrypt EncryptDecryptService { protected get; set; }

        protected INakedObjectsFramework NakedObjectsContext {
            get { return nakedObjectsContext; }
        }

        protected void SetSession() {
           // NakedObjectsContext.Instance.SetSession(new WindowsSession(User));
        }

        protected void SetControllerName(string name) {
            ControllerContext.RouteData.Values["controller"] = name;
        }

        protected void SetControllerName(object domainObject) {
            string controllerName = NakedObjectsContext.GetObjectTypeName(domainObject);
            SetControllerName(controllerName);
        }

        protected void SetServices() {
            ViewData[IdHelper.NofServices] = NakedObjectsContext.GetServices();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            //NakedObjectsContext.EnsureReady();
            //SetSession();
            //SetServices();
            NakedObjectsContext.ObjectPersistor.StartTransaction();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext) {
            if (filterContext.Exception == null) {
                NakedObjectsContext.ObjectPersistor.EndTransaction();
            }
            else {
                try {
                    NakedObjectsContext.ObjectPersistor.AbortTransaction();
                }
                catch {
                    // fail abort silently 
                }
            }

            SetMessagesAndWarnings();
            SetEncryptDecrypt();
        }

        internal ActionResult AppropriateView(ObjectAndControlData controlData, INakedObject nakedObject, INakedObjectAction action = null, string propertyName = null) {
            if (nakedObject == null) {
                // no object to go to 
                // if action on object go to that object. 
                // if action on collection go to collection 
                // if action on service go to last object 

                nakedObject = controlData.GetNakedObject(NakedObjectsContext);

                if (nakedObject.Specification.IsService) {
                    object lastObject = Session.LastObject(NakedObjectsContext, ObjectCache.ObjectFlag.BreadCrumb);
                    if (lastObject == null) {
                        TempData[IdHelper.NofMessages] = NakedObjectsContext.MessageBroker.Messages;
                        TempData[IdHelper.NofWarnings] = NakedObjectsContext.MessageBroker.Warnings;
                        return RedirectToAction(IdHelper.IndexAction, IdHelper.HomeName);
                    }

                    nakedObject = NakedObjectsContext.GetNakedObject(lastObject);
                }
            }

            if (nakedObject.Specification.IsCollection && !nakedObject.Specification.IsParseable) {
                var collection = nakedObject.GetAsQueryable();
                int collectionSize = collection.Count();
                if (collectionSize == 1) {
                    // remove any paging data - to catch case where custom page has embedded standalone collection as paging data will confuse rendering 
                    ViewData.Remove(IdHelper.PagingData);
                    return View("ObjectView", collection.First());
                }

                nakedObject = Page(nakedObject, collectionSize, controlData, nakedObject.IsNotQueryable());
                action = action ?? ((CollectionMemento)nakedObject.Oid).Action;
                int page, pageSize;
                CurrentlyPaging(controlData, collectionSize, out page, out pageSize);
                var format = ViewData["NofCollectionFormat"] as string;
                return View("StandaloneTable", ActionResultModel.Create(NakedObjectsContext, action, nakedObject, page, pageSize, format));
            }
            // remove any paging data - to catch case where custom page has embedded standalone collection as paging data will confuse rendering   
            ViewData.Remove(IdHelper.PagingData);

            if (controlData.DataDict.Values.Contains("max")) {
                // maximizing an inline object - do not update history
                ViewData.Add("updateHistory", false);
            }

            return propertyName == null ? View(nakedObject.IsNotPersistent() ? "ObjectView" : "ViewNameSetAfterTransaction", nakedObject.Object) :
                                          View(nakedObject.IsNotPersistent() ? "PropertyView" : "ViewNameSetAfterTransaction", new PropertyViewModel(nakedObject.Object, propertyName));
        }


        internal bool ValidateParameters(INakedObject targetNakedObject, INakedObjectAction action, ObjectAndControlData controlData) {

            // check mandatory fields first to emulate WPF UI behaviour where no validation takes place until 
            // all mandatory fields are set. 
            foreach (INakedObjectActionParameter parm in action.Parameters) {
                object result = GetRawParameterValue(parm, controlData, IdHelper.GetParameterInputId(action, parm));
                var stringResult = result as string;

                if (parm.IsMandatory && (result == null || (result is string && string.IsNullOrEmpty(stringResult)))) {
                    ModelState.AddModelError(IdHelper.GetParameterInputId(action, parm), MvcUi.Mandatory);
                }
            }

            //check for individual parameter validity, including parsing of text input
            if (ModelState.IsValid) {
                foreach (INakedObjectActionParameter parm in action.Parameters) {
                    try {
                        INakedObject valueNakedObject = GetParameterValue(parm, IdHelper.GetParameterInputId(action, parm), controlData);

                        ValidateParameter(action, parm, targetNakedObject, valueNakedObject);
                    }
                    catch (InvalidEntryException) {
                        ModelState.AddModelError(IdHelper.GetParameterInputId(action, parm), MvcUi.InvalidEntry);
                    }
                }
            }

            // check for validity of whole set, including any 'co-validation' involving multiple parameters
            if (ModelState.IsValid) {
                IEnumerable<INakedObject> parms = action.Parameters.Select(p => GetParameterValue(p, IdHelper.GetParameterInputId(action, p), controlData));

                IConsent consent = action.IsParameterSetValid(NakedObjectsContext.Session,  targetNakedObject, parms.ToArray(), NakedObjectsContext.ObjectPersistor);
                if (!consent.IsAllowed) {
                    ModelState.AddModelError(string.Empty, consent.Reason);
                }
            }

            return ModelState.IsValid;
        }

        public void ValidateParameter(INakedObjectAction action, INakedObjectActionParameter parm, INakedObject targetNakedObject, INakedObject valueNakedObject) {
            IConsent consent = parm.IsValid(targetNakedObject, valueNakedObject, NakedObjectsContext.ObjectPersistor, NakedObjectsContext.Session);
            if (!consent.IsAllowed) {
                ModelState.AddModelError(IdHelper.GetParameterInputId(action, parm), consent.Reason);
            }
        }

        private bool CheckForAndAddCollectionMemento(string name, string[] values, ObjectAndControlData controlData) {
            if (values.Count() == 1) {
                INakedObject nakedObject = NakedObjectsContext.GetNakedObjectFromId(values.First());

                if (nakedObject != null && nakedObject.Oid as CollectionMemento != null) {
                    nakedObject = FilterCollection(nakedObject, controlData);
                    AddAttemptedValue(name, nakedObject);
                    return true;
                }
            }
            return false;
        }

        internal void AddAttemptedValues(ObjectAndControlData controlData) {
            INakedObjectAction action = controlData.GetAction(NakedObjectsContext);
            var form = controlData.Form;
            foreach (INakedObjectActionParameter parm in action.Parameters) {
                string name = IdHelper.GetParameterInputId(action, parm);
                ValueProviderResult vp = form.GetValue(name);
                string[] values = vp == null ? new string[] {} : (string[]) vp.RawValue;

                if (parm.IsCollection) {
                    // handle collection mementos 

                    if (parm.IsMultipleChoicesEnabled || !CheckForAndAddCollectionMemento(name, values, controlData)) {
                        var itemSpec = parm.Specification.GetFacet<ITypeOfFacet>().ValueSpec;
                        var itemvalues = values.Select(v => itemSpec.IsParseable ? (object) v : NakedObjectsContext.GetNakedObjectFromId(v).GetDomainObject()).ToList();

                        if (itemvalues.Any()) {
                            AddAttemptedValue(name, NakedObjectsContext.ObjectPersistor.CreateAdapter(itemvalues, null, null));
                        }
                    }
                }
                else {
                    string value = values.Any() ? values.First() : "";

                    if (!string.IsNullOrEmpty(value)) {
                        AddAttemptedValue(name, parm.Specification.IsParseable ? (object) value : FilterCollection(NakedObjectsContext.GetNakedObjectFromId(value), controlData));
                    }
                }
            }
        }

        internal void AddFilesToControlData(ObjectAndControlData controlData) {
            if (Request.Files.Count > 0) {
                foreach (string key in Request.Files.AllKeys) {
                    HttpPostedFileBase file = Request.Files[key];

                    if (file != null && file.ContentLength > 0) {
                        controlData.Files.Add(key, file);
                    }
                }
            }
        }

        internal INakedObject GetParameterValue(INakedObjectActionParameter parm, string name, ObjectAndControlData controlData) {
            object value = GetRawParameterValue(parm, controlData, name);
            var nakedObject = GetParameterValue(parm, value);
            return FilterCollection(nakedObject, controlData);
        }

        internal  INakedObject GetParameterValue(INakedObjectActionParameter parm, object value) {
            if (value == null) {
                return null;
            }
            var fromStreamFacet = parm.Specification.GetFacet<IFromStreamFacet>();
            if (fromStreamFacet != null) {
                var httpPostedFileBase = (HttpPostedFileBase) value;
                return fromStreamFacet.ParseFromStream(httpPostedFileBase.InputStream, httpPostedFileBase.ContentType, httpPostedFileBase.FileName, NakedObjectsContext.ObjectPersistor);
            }
            var stringValue = value as string;
            if (parm.Specification.IsParseable) {
                return parm.Specification.GetFacet<IParseableFacet>().ParseTextEntry(stringValue, NakedObjectsContext.ObjectPersistor);
            }

            var collectionValue = value as IEnumerable;
            if (parm.IsObject || collectionValue == null) {
                return NakedObjectsContext.GetNakedObjectFromId(stringValue);
            }

            return NakedObjectsContext.GetTypedCollection(parm.Specification, collectionValue);
        }

        private static object GetRawParameterValue(INakedObjectActionParameter parm, ObjectAndControlData controlData, string name) {

            var form = controlData.Form;
            ValueProviderResult vp = form.GetValue(name);
            string[] values = vp == null ? null : (string[]) vp.RawValue;

            if (values == null) {
                if (controlData.Files.ContainsKey(name)) {
                    return controlData.Files[name];
                }
                return null; 
            }
            if (parm.IsCollection) {
                return values.All(string.IsNullOrEmpty) ? null : values;
            }
            return values.First();
        }

        internal IEnumerable<INakedObject> GetParameterValues(INakedObjectAction action, ObjectAndControlData controlData) {
            return action.Parameters.Select(parm => GetParameterValue(parm, IdHelper.GetParameterInputId(action, parm), controlData));
        }

        internal void SetContextObjectAsParameterValue(INakedObjectAction targetAction, INakedObject contextNakedObject) {
            if (targetAction.Parameters.Any(p => p.Specification.IsOfType(contextNakedObject.Specification))) {
                foreach (INakedObjectActionParameter parm in targetAction.Parameters) {
                    if (parm.Specification.IsOfType(contextNakedObject.Specification)) {
                        string name = IdHelper.GetParameterInputId(targetAction, parm);
                        AddAttemptedValue(name, contextNakedObject);
                    }
                }
            }
        }

        protected string DisplaySingleProperty(ObjectAndControlData controlData, IDictionary<string, string> data) {
            if (Request.IsAjaxRequest()) {
                var nakedObject = controlData.GetNakedObject(NakedObjectsContext);
                if (controlData.SubAction == ObjectAndControlData.SubActionType.Redisplay) {
                    IEnumerable<INakedObjectAssociation> assocs = nakedObject.Specification.Properties.Where(p => p.IsCollection);
                    INakedObjectAssociation item = assocs.SingleOrDefault(a => data.ContainsKey(a.Id));
                    return item == null ? null : item.Id;
                }
                if (controlData.ActionId == null) {
                    IEnumerable<INakedObjectAssociation> assocs = nakedObject.Specification.Properties.Where(p => !p.IsCollection);
                    INakedObjectAssociation item = assocs.SingleOrDefault(a => data.ContainsKey(a.Id));
                    return item == null ? null : item.Id;
                }
                {
                    IEnumerable<INakedObjectActionParameter> parms = controlData.GetAction(NakedObjectsContext).Parameters;
                    INakedObjectActionParameter item = parms.SingleOrDefault(p => data.ContainsKey(p.Id));
                    return item == null ? null : item.Id;
                }
            }

            return null;
        }

        private static string GetName(string nameValue) {
            if (string.IsNullOrEmpty(nameValue)) {
                return string.Empty;
            }
            return nameValue.Remove(nameValue.IndexOf('='));
        }

        private static string GetValue(string nameValue) {
            if (string.IsNullOrEmpty(nameValue)) {
                return string.Empty;
            }
            int indexOfValue = nameValue.IndexOf('=') + 1;
            return indexOfValue == nameValue.Length ? string.Empty : nameValue.Substring(indexOfValue);
        }

        private static bool IsFormat(string id) {
            return id.EndsWith(IdHelper.MinDisplayFormat) ||
                   id.EndsWith(IdHelper.MaxDisplayFormat) ||
                   id.EndsWith(IdHelper.ListDisplayFormat) ||
                   id.EndsWith(IdHelper.SummaryDisplayFormat) ||
                   id.EndsWith(IdHelper.TableDisplayFormat);
        }

        internal void SetNewCollectionFormats(ObjectAndControlData controlData) {
            KeyValuePair<string, string>[] formats = controlData.DataDict.Where(kvp => IsFormat(kvp.Value)).ToArray();
            if (formats.Any()) {
                formats.ForEach(kvp => ViewData[kvp.Key] = kvp.Value);
            }
            else if (!string.IsNullOrWhiteSpace(controlData.Format)) {
                ViewData[IdHelper.CollectionFormat] = controlData.Format;
            }
        }

        internal void SetExistingCollectionFormats(INakedObject nakedObject, FormCollection form) {
            if (form.AllKeys.Any(s => s.EndsWith(IdHelper.DisplayFormatFieldId))) {
                var id = form.AllKeys.Single(s => s.EndsWith(IdHelper.DisplayFormatFieldId));
                var values = form.GetValue(id).AttemptedValue.Split('&').ToDictionary(GetName, GetValue);
                values.Where(kvp => IsFormat(kvp.Value)).ForEach(kvp => ViewData[kvp.Key] = kvp.Value);
            }
        }

        internal void SetDefaults(INakedObject nakedObject, INakedObjectAction action) {
            foreach (INakedObjectActionParameter parm in action.Parameters) {
                INakedObject value = parm.GetDefault(nakedObject, NakedObjectsContext.ObjectPersistor);
                TypeOfDefaultValue typeOfValue = parm.GetDefaultType(nakedObject, NakedObjectsContext.ObjectPersistor);

                bool ignore = value == null || (value.Object is DateTime && ((DateTime) value.Object).Ticks == 0) || typeOfValue == TypeOfDefaultValue.Implicit;
                if (!ignore) {
                    // deliberately not an attempted value so it only gets populated after masking 
                    ViewData[IdHelper.GetParameterInputId(action, parm)] = parm.Specification.IsParseable ? value.Object : value;
                }
            }
        }

        internal void SetSelectedReferences(INakedObject nakedObject, IDictionary<string, string> dict) {
            var refItems = nakedObject.Specification.Properties.Where(p => p.IsObject && !p.Specification.IsParseable).Where(a => dict.ContainsKey(a.Id)).ToList();
            if (refItems.Any()) {
                refItems.ForEach(a => ValidateAssociation(nakedObject, a as IOneToOneAssociation, dict[a.Id]));
                Dictionary<string, INakedObject> items = refItems.ToDictionary(a => IdHelper.GetFieldInputId(nakedObject, a), a => NakedObjectsContext.GetNakedObjectFromId(dict[a.Id]));
                items.ForEach(kvp => ViewData[kvp.Key] = kvp.Value);
            }
        }

        internal void SetSelectedParameters(INakedObjectAction action) {
            var refItems = action.Parameters.Where(p => p.IsObject && !p.Specification.IsParseable).Where(p => ValueProvider.GetValue(p.Id) != null).ToList();
            if (refItems.Any()) {
                Dictionary<string, INakedObject> items = refItems.ToDictionary(p => IdHelper.GetParameterInputId(action, p), p => NakedObjectsContext.GetNakedObjectFromId(ValueProvider.GetValue(p.Id).AttemptedValue));
                items.ForEach(kvp => ViewData[kvp.Key] = kvp.Value);
            }
        }

        internal void SetSelectedParameters(INakedObject nakedObject, INakedObjectAction action, IDictionary<string, string> dict) {
            var refItems = action.Parameters.Where(p => p.IsObject && !p.Specification.IsParseable).Where(p => dict.ContainsKey(p.Id)).ToList();
            if (refItems.Any()) {

                refItems.ForEach(p => ValidateParameter(action, p, nakedObject, NakedObjectsContext.GetNakedObjectFromId(dict[p.Id])));

                Dictionary<string, INakedObject> items = refItems.ToDictionary(p => IdHelper.GetParameterInputId(action, p), p => NakedObjectsContext.GetNakedObjectFromId(dict[p.Id]));
                items.ForEach(kvp => ViewData[kvp.Key] = kvp.Value);
            }
        }

        internal  INakedObject GetNakedObjectValue(INakedObjectAssociation assoc, INakedObject targetNakedObject, object value) {
            if (value == null) {
                return null;
            }
            var fromStreamFacet = assoc.Specification.GetFacet<IFromStreamFacet>();
            if (fromStreamFacet != null) {
                var httpPostedFileBase = (HttpPostedFileBase)value;
                return fromStreamFacet.ParseFromStream(httpPostedFileBase.InputStream, httpPostedFileBase.ContentType, httpPostedFileBase.FileName, NakedObjectsContext.ObjectPersistor);
            }
            var stringValue = value as string;
            if (assoc.Specification.IsParseable) {

                return assoc.Specification.GetFacet<IParseableFacet>().ParseTextEntry(stringValue, NakedObjectsContext.ObjectPersistor);
            }
       
            if (assoc.IsObject) {
                return NakedObjectsContext.GetNakedObjectFromId(stringValue);
            }
            // collection 
            return null; 
        }

        internal void CheckConcurrency(INakedObject nakedObject, INakedObjectAssociation parent, ObjectAndControlData controlData, Func<INakedObjectAssociation, INakedObject, INakedObjectAssociation, string> idFunc) {
            var concurrencyFields = nakedObject.Specification.Properties.Where(p => p.ContainsFacet<IConcurrencyCheckFacet>()).ToList();

            if (!nakedObject.ResolveState.IsTransient() && concurrencyFields.Any() ) {
                IEnumerable<Tuple<INakedObjectAssociation, object>> fieldsAndMatchingValues = GetFieldsAndMatchingValues(nakedObject, parent, concurrencyFields, controlData, idFunc);

                foreach (var pair in fieldsAndMatchingValues) {
                    if (pair.Item1.Specification.IsParseable) {
                        INakedObject currentValue = pair.Item1.GetNakedObject(nakedObject, NakedObjectsContext.ObjectPersistor);
                        INakedObject concurrencyValue = pair.Item1.Specification.GetFacet<IParseableFacet>().ParseInvariant(pair.Item2 as string, NakedObjectsContext.ObjectPersistor);

                        if (concurrencyValue != null && currentValue != null) {
                            if (concurrencyValue.TitleString() != currentValue.TitleString()) {
                                throw new ConcurrencyException(nakedObject, null);
                            }
                        }
                        else if (concurrencyValue == null && currentValue == null) {
                            // OK 
                        }
                        else {
                            throw new ConcurrencyException(nakedObject, null);
                        }
                    }
                }
            }
        }

        internal bool ValidateChanges(INakedObject nakedObject, ObjectAndControlData controlData, INakedObjectAssociation parent = null) {
            List<INakedObjectAssociation> usableAndVisibleFields;
            List<Tuple<INakedObjectAssociation, object>> fieldsAndMatchingValues;
            GetUsableAndVisibleFields(nakedObject, controlData, parent, out usableAndVisibleFields, out fieldsAndMatchingValues);

            CheckConcurrency(nakedObject, parent, controlData, GetConcurrencyFieldInputId);

            fieldsAndMatchingValues.ForEach(pair => AddAttemptedValue(GetFieldInputId(parent, nakedObject, pair.Item1), pair.Item2));

            // check mandatory fields first to emulate WPF UI behaviour where no validation takes place until 
            // all mandatory fields are set. 
            foreach (var pair in fieldsAndMatchingValues) {
                var result = pair.Item2;
                var stringResult = result as string;

                if (pair.Item1.IsMandatory && (result == null || (result is string && string.IsNullOrEmpty(stringResult)))) {
                    AddErrorAndAttemptedValue(nakedObject, stringResult, pair.Item1, MvcUi.Mandatory, parent);
                }
            }

            if (ModelState.IsValid) {
                ValidateOrApplyInlineChanges(nakedObject, controlData, usableAndVisibleFields, ValidateChanges);
            }

            if (ModelState.IsValid) {
                foreach (var pair in fieldsAndMatchingValues) {
                    if (!pair.Item1.IsCollection) {
                        ValidateAssociation(nakedObject, (IOneToOneAssociation) pair.Item1, pair.Item2, parent);
                    }
                }
            }

            if (ModelState.IsValid) {
                INakedObjectValidation[] validators = nakedObject.Specification.ValidateMethods();

                foreach (INakedObjectValidation validator in validators) {
                    string[] parmNames = validator.ParameterNames;
                 
                    var matchingparms = parmNames.Select(pn => fieldsAndMatchingValues.Single(t => t.Item1.Id.ToLower() == pn)).ToList();

                    if (matchingparms.Count() == parmNames.Count()) {
                        string result = validator.Execute(nakedObject, matchingparms.Select(t => GetNakedObjectValue(t.Item1, nakedObject, t.Item2)).ToArray());

                        if (!string.IsNullOrEmpty(result)) {                         
                            ModelState.AddModelError(string.Empty, result);
                        }
                    }
                }
            }

            if (ModelState.IsValid) {
                if (nakedObject.Specification.ContainsFacet<IValidateProgrammaticUpdatesFacet>()) {
                    string state = nakedObject.ValidToPersist();
                    if (state != null) {
                        ModelState.AddModelError(string.Empty, state);
                    }
                }
            }

            return ModelState.IsValid;
        }

        private void ValidateOrApplyInlineChanges(INakedObject nakedObject, ObjectAndControlData controlData, IEnumerable<INakedObjectAssociation> assocs, Func<INakedObject, ObjectAndControlData, INakedObjectAssociation, bool> validateOrApply) {

            var form = controlData.Form;
            // inline or one or more keys in form starts with the property id which indicates we have nested values for the subobject 
            foreach (INakedObjectAssociation assoc in assocs.Where(a => a.IsInline || form.AllKeys.Any(k => k.KeyPrefixIs(a.Id)))) {
                INakedObject inlineNakedObject = assoc.GetNakedObject(nakedObject, NakedObjectsContext.ObjectPersistor);
                if (inlineNakedObject != null) {
                    validateOrApply(inlineNakedObject, controlData, assoc);
                }
            }
        }

        private  void GetUsableAndVisibleFields(INakedObject nakedObject, ObjectAndControlData controlData, INakedObjectAssociation parent, out List<INakedObjectAssociation> usableAndVisibleFields, out List<Tuple<INakedObjectAssociation, object>> fieldsAndMatchingValues) {
            usableAndVisibleFields = nakedObject.Specification.Properties.Where(p => IsUsable(p, nakedObject) && IsVisible(p, nakedObject)).ToList();
            fieldsAndMatchingValues = GetFieldsAndMatchingValues(nakedObject, parent, usableAndVisibleFields, controlData, GetFieldInputId).ToList();
        }

        internal bool ApplyChanges(INakedObject nakedObject, ObjectAndControlData controlData, INakedObjectAssociation parent = null) {
            List<INakedObjectAssociation> usableAndVisibleFields;
            List<Tuple<INakedObjectAssociation, object>> fieldsAndMatchingValues;
            GetUsableAndVisibleFields(nakedObject, controlData, parent, out usableAndVisibleFields, out fieldsAndMatchingValues);

            foreach (var pair in fieldsAndMatchingValues) {
                INakedObject value = GetNakedObjectValue(pair.Item1, nakedObject, pair.Item2);
                if (!pair.Item1.IsCollection) {
                    SetAssociation(nakedObject, (IOneToOneAssociation)pair.Item1, value, pair.Item2);
                }
            }

            ValidateOrApplyInlineChanges(nakedObject, controlData, nakedObject.Specification.Properties, ApplyChanges);

            if (nakedObject.ResolveState.IsTransient()) {
                CanPersist(nakedObject, usableAndVisibleFields);
                if (ModelState.IsValid) {
                    if (nakedObject.Specification.Persistable == Persistable.USER_PERSISTABLE) {
                        NakedObjectsContext.ObjectPersistor.MakePersistent(nakedObject);
                    }
                    else {
                        NakedObjectsContext.ObjectPersistor.ObjectChanged(nakedObject);
                    }
                }
            }

            return ModelState.IsValid;
        }


        private static IEnumerable<Tuple<INakedObjectAssociation, object>> GetFieldsAndMatchingValues(INakedObject nakedObject,
                                                                                                      INakedObjectAssociation parent,
                                                                                                      IEnumerable<INakedObjectAssociation> associations,                                                                                                      
                                                                                                      ObjectAndControlData controlData, 
                                                                                                      Func<INakedObjectAssociation, INakedObject, INakedObjectAssociation, string> idFunc) {
                       
            foreach (INakedObjectAssociation assoc in associations.Where(a => !a.IsInline)) {
                string name = idFunc(parent, nakedObject, assoc);
                object newValue = GetValueFromForm(controlData, name);
                yield return new Tuple<INakedObjectAssociation, object>(assoc, newValue);
            }
        }

        private static object GetValueFromForm(ObjectAndControlData controlData,  string name) {
            var form = controlData.Form;

            if (form.GetValue(name) != null) {
                return ((string[]) form.GetValue(name).RawValue).First();
            }

            return controlData.Files.ContainsKey(name) ? controlData.Files[name] : null;
        }

        internal void AddErrorAndAttemptedValue(INakedObject nakedObject, string newValue, INakedObjectAssociation assoc, string errorText, INakedObjectAssociation parent = null) {
            string key = GetFieldInputId(parent, nakedObject, assoc);
            ModelState.AddModelError(key, errorText);
            AddAttemptedValue(key, assoc.Specification.IsParseable ? (object)newValue : NakedObjectsContext.GetNakedObjectFromId(newValue));
        }

        internal void AddAttemptedValues(INakedObject nakedObject, ObjectAndControlData controlData, INakedObjectAssociation parent = null) {
          
            foreach (INakedObjectAssociation assoc in nakedObject.Specification.Properties.Where(p => (IsUsable(p, nakedObject) && IsVisible(p, nakedObject)) || IsConcurrency(p))) {
                string name = GetFieldInputId(parent, nakedObject, assoc);
                string value = GetValueFromForm(controlData, name) as string;
                if (value != null) {
                    AddAttemptedValue(name, value);
                }
            }

            foreach (INakedObjectAssociation assoc in nakedObject.Specification.Properties.Where(IsConcurrency)) {             
                string name = GetConcurrencyFieldInputId(parent, nakedObject, assoc);
                string value = GetValueFromForm(controlData, name) as string; 
                if (value != null) {
                    AddAttemptedValue(name, value);
                }
            }

            foreach (INakedObjectAssociation assoc in nakedObject.Specification.Properties.Where(p => p.IsInline)) {
                var inlineNakedObject = assoc.GetNakedObject(nakedObject, NakedObjectsContext.ObjectPersistor);
                AddAttemptedValues(inlineNakedObject, controlData, assoc);
            }

        }

        internal  bool IsUsable(INakedObjectAssociation assoc, INakedObject nakedObject) {
            return assoc.IsUsable(NakedObjectsContext.Session, nakedObject, NakedObjectsContext.ObjectPersistor).IsAllowed;
        }

        internal  bool IsVisible(INakedObjectAssociation assoc, INakedObject nakedObject) {
            return assoc.IsVisible(NakedObjectsContext.Session, nakedObject, NakedObjectsContext.ObjectPersistor);
        }

        internal  bool IsConcurrency(INakedObjectAssociation assoc) {
            return assoc.ContainsFacet<IConcurrencyCheckFacet>();
        }

        internal  void RefreshTransient(INakedObject nakedObject, FormCollection form, INakedObjectAssociation parent = null) {
            if (nakedObject.Oid.IsTransient) { // use oid to catch transient aggregates 
                foreach (INakedObjectAssociation assoc in nakedObject.Specification.Properties.Where(p => !p.IsReadOnly)) {
                    // TODO change this to use GetValueFromForm 
                    foreach (string item in form.AllKeys) {
                        string name =  GetFieldInputId(parent, nakedObject, assoc);
                        if (name == item) {
                            object newValue = ((string[]) form.GetValue(item).RawValue).First();

                            if (assoc.Specification.IsParseable) {
                                try {
                                 
                                    var oneToOneAssoc = ((IOneToOneAssociation) assoc);
                                    INakedObject value = assoc.Specification.GetFacet<IParseableFacet>().ParseTextEntry((string)newValue, NakedObjectsContext.ObjectPersistor);
                                    oneToOneAssoc.SetAssociation(nakedObject, value, NakedObjectsContext.ObjectPersistor);
                                }
                                catch (InvalidEntryException) {
                                    ModelState.AddModelError(name, MvcUi.InvalidEntry);
                                }
                            }
                            else if (assoc.IsObject) {
                                INakedObject value = NakedObjectsContext.GetNakedObjectFromId((string) newValue);
                                var oneToOneAssoc = ((IOneToOneAssociation) assoc);
                                oneToOneAssoc.SetAssociation(nakedObject, value, NakedObjectsContext.ObjectPersistor);
                            }
                        }
                    }
                }

                foreach (IOneToManyAssociation assoc in nakedObject.Specification.Properties.Where(p => p.IsCollection)) {
                    string name = IdHelper.GetCollectionItemId(nakedObject, assoc);
                    ValueProviderResult items = form.GetValue(name);

                    if (items != null && assoc.Count(nakedObject, NakedObjectsContext.ObjectPersistor) == 0) {
                        var itemIds = (string[]) items.RawValue;
                        var values = itemIds.Select(NakedObjectsContext.GetNakedObjectFromId).ToArray();
                        var collection = assoc.GetNakedObject(nakedObject, NakedObjectsContext.ObjectPersistor);
                        collection.Specification.GetFacet<ICollectionFacet>().Init(collection, values);
                    }
                }

                foreach (INakedObjectAssociation assoc in nakedObject.Specification.Properties.Where(p => p.IsInline)) {
                    var inlineNakedObject = assoc.GetNakedObject(nakedObject, NakedObjectsContext.ObjectPersistor);
                    RefreshTransient(inlineNakedObject, form, assoc);
                }

            }
        }

        private static string GetFieldInputId(INakedObjectAssociation parent, INakedObject nakedObject, INakedObjectAssociation assoc) {
            return parent == null ? IdHelper.GetFieldInputId(nakedObject, assoc) : IdHelper.GetInlineFieldInputId(parent, nakedObject, assoc);
        }

        private static string GetConcurrencyFieldInputId(INakedObjectAssociation parent, INakedObject nakedObject, INakedObjectAssociation assoc) {
            return parent == null ? IdHelper.GetConcurrencyFieldInputId(nakedObject, assoc) : IdHelper.GetInlineConcurrencyFieldInputId(parent, nakedObject, assoc);
        }

       

        private bool CanPersist(INakedObject nakedObject, IEnumerable<INakedObjectAssociation> usableAndVisibleFields) {
            foreach (INakedObjectAssociation assoc in usableAndVisibleFields) {
                INakedObject value = assoc.GetNakedObject(nakedObject, NakedObjectsContext.ObjectPersistor);

                if (value != null && value.Specification.IsObject) {
                    if (!IsObjectCompleteAndSaved(value)) {
                        return false;
                    }
                }
            }

            return true;
        }


        private bool IsObjectCompleteAndSaved(INakedObject fieldTarget) {
            if (fieldTarget.Specification.IsCollection) {         
                if (fieldTarget.GetAsEnumerable(NakedObjectsContext.ObjectPersistor).Any(no => !IsReferenceValidToPersist(no))) {
                    ModelState.AddModelError("", MvcUi.CollectionIncomplete);
                    return false;
                }
            }
            else {
                if (!IsReferenceValidToPersist(fieldTarget)) {
                    ModelState.AddModelError("", MvcUi.ObjectIncomplete);
                    return false;
                }
            }
         
            return true;
        }

        private  bool IsReferenceValidToPersist(INakedObject target1) {
            if (target1.ResolveState.IsTransient() ||
                (target1.Oid is AggregateOid && ((AggregateOid)target1.Oid).ParentOid.IsTransient)) {
                string validToPersist = target1.ValidToPersist();
                if (validToPersist != null) {
                    ModelState.AddModelError("", validToPersist);
                    return false;
                }
            }
            return true;
        }


        internal void SetAssociation(INakedObject nakedObject, IOneToOneAssociation oneToOneAssoc, INakedObject valueNakedObject, object attemptedValue) {
            IConsent consent = oneToOneAssoc.IsAssociationValid(nakedObject, valueNakedObject, NakedObjectsContext.Session);
            string key = IdHelper.GetFieldInputId(nakedObject, oneToOneAssoc);
            if (consent.IsAllowed) {
                oneToOneAssoc.SetAssociation(nakedObject, valueNakedObject, NakedObjectsContext.ObjectPersistor);
            }
            else {
                ModelState.AddModelError(key, consent.Reason);
            }
            AddAttemptedValue(key, attemptedValue);
        }

        internal void ValidateAssociation(INakedObject nakedObject, IOneToOneAssociation oneToOneAssoc, object attemptedValue, INakedObjectAssociation parent = null) {
            string key = GetFieldInputId(parent, nakedObject, oneToOneAssoc);
            try {
                INakedObject valueNakedObject = GetNakedObjectValue(oneToOneAssoc, nakedObject, attemptedValue);

                IConsent consent = oneToOneAssoc.IsAssociationValid(nakedObject, valueNakedObject, NakedObjectsContext.Session);               
                if (!consent.IsAllowed) {
                    ModelState.AddModelError(key, consent.Reason);
                }
            }
            catch (InvalidEntryException) {
                ModelState.AddModelError(key, MvcUi.InvalidEntry);
            }
            catch (ArgumentException) {
                // Always expect newValue to be non-null for a parseable field as it should always be included 
                // in the form so this is an unexpected result for a parseable field 
                ModelState.AddModelError(key, MvcUi.InvalidEntry);
            }
            finally {
                AddAttemptedValue(key, attemptedValue);
            }
        }

        private void AddAttemptedValue(string key, object value) {
            ModelState.SetModelValue(key, new ValueProviderResult(value, value == null ? string.Empty : value.ToString(), null));
        }

        internal static bool ActionExecutingAsContributed(INakedObjectAction action, INakedObject targetNakedObject) {
            return action.IsContributedMethod && action.OnType != targetNakedObject.Specification;
        }

        internal void SetMessagesAndWarnings() {
            string[] messages = NakedObjectsContext.MessageBroker.Messages;
            string[] warnings = NakedObjectsContext.MessageBroker.Warnings;

            var existingMessages = TempData[IdHelper.NofMessages];
            var existingWarnings = TempData[IdHelper.NofWarnings];

            if (existingMessages is string[] && ((string[])existingMessages).Length > 0) {
                messages = ((string[])existingMessages).Union(messages).ToArray();
            }

            if (existingWarnings is string[] && ((string[])existingWarnings).Length > 0) {
                warnings = ((string[])existingWarnings).Union(warnings).ToArray();
            }

            ViewData.Add(IdHelper.NofMessages, messages);
            ViewData.Add(IdHelper.NofWarnings, warnings);
        }

        internal void SetEncryptDecrypt() {
            ViewData.Add(IdHelper.NofEncryptDecrypt, EncryptDecryptService);
        }

        internal void SetPagingValues(ObjectAndControlData controlData, INakedObject nakedObject) {
            if (nakedObject.Specification.IsCollection) {
                int sink1, sink2;
                CurrentlyPaging(controlData, nakedObject.GetAsEnumerable(NakedObjectsContext.ObjectPersistor).Count(), out sink1, out sink2);
            }
        }


        internal bool CurrentlyPaging(ObjectAndControlData controlData, int collectionSize, out int page, out int pageSize) {
            pageSize = GetPageSize(controlData);
            page = 1;

            if (pageSize > 0) {
                page = GetPage(controlData);

                var pagingData = new Dictionary<string, int> {
                                                                 {IdHelper.PagingCurrentPage, page},
                                                                 {IdHelper.PagingPageSize, pageSize},
                                                                 {IdHelper.PagingTotal, collectionSize}
                                                             };
                ViewData[IdHelper.PagingData] = pagingData;
                return true;
            }

            return false;
        }

        private static int GetPage(ObjectAndControlData controlData) {
            if (controlData.DataDict.ContainsKey(IdHelper.PageKey)) {
                return int.Parse(controlData.DataDict[IdHelper.PageKey]);
            }
            return !string.IsNullOrEmpty(controlData.Page) ? int.Parse(controlData.Page) : 1;
        }


        internal  int GetPageSize(ObjectAndControlData controlData) {
            if (controlData.DataDict.ContainsKey(IdHelper.PageSizeKey)) {
                return int.Parse(controlData.DataDict[IdHelper.PageSizeKey]);
            }
            if (!string.IsNullOrEmpty(controlData.PageSize)) {
                return int.Parse(controlData.PageSize);
            }
            INakedObjectAction action = controlData.GetAction(NakedObjectsContext);
            return action != null ? action.GetFacet<IPageSizeFacet>().Value : 0;
        }


        internal INakedObject Page(INakedObject nakedObject, int collectionSize, ObjectAndControlData controlData, bool forceEnumerable) {
            int page, pageSize;
            var collectionfacet = nakedObject.GetCollectionFacetFromSpec();
            if (CurrentlyPaging(controlData, collectionSize, out page, out pageSize) && !nakedObject.IsPaged()) {
                return DoPaging(nakedObject, collectionfacet, page, pageSize, forceEnumerable);
            }

            // one page of full collection 
            return DoPaging(nakedObject, collectionfacet, 1, collectionSize, forceEnumerable);
        }

        private  INakedObject DoPaging(INakedObject nakedObject, ICollectionFacet collectionfacet, int page, int pageSize, bool forceEnumerable) {
            INakedObject newNakedObject = collectionfacet.Page(page, pageSize, nakedObject, NakedObjectsContext.ObjectPersistor, forceEnumerable);
            object[] objects = newNakedObject.GetAsEnumerable(NakedObjectsContext.ObjectPersistor).Select(no => no.Object).ToArray();
            newNakedObject.SetATransientOid(new CollectionMemento(NakedObjectsContext.ObjectPersistor, NakedObjectsContext.Reflector, NakedObjectsContext.Session, nakedObject.Oid as CollectionMemento, objects) { IsPaged = true });
            return newNakedObject;
        }

        internal INakedObject FilterCollection(INakedObject nakedObject, ObjectAndControlData controlData) {
            var form = controlData.Form;
            if (form != null && nakedObject != null && nakedObject.Specification.IsCollection && nakedObject.Oid is CollectionMemento) {
                nakedObject = Page(nakedObject, nakedObject.GetAsQueryable().Count(), controlData, false);
                var map = nakedObject.GetAsEnumerable(NakedObjectsContext.ObjectPersistor).ToDictionary(NakedObjectsContext.GetObjectId, y => y.Object);
                var selected = map.Where(kvp => form.Keys.Cast<string>().Contains(kvp.Key) && form[kvp.Key].Contains("true")).Select(kvp => kvp.Value).ToArray();
                return CloneAndPopulateCollection(nakedObject, selected, false);
            }

            return nakedObject;
        }

        private  INakedObject CloneAndPopulateCollection(INakedObject nakedObject, object[] selected, bool forceEnumerable) {
            IList result = CollectionUtils.CloneCollectionAndPopulate(nakedObject.Object, selected);
            INakedObject adapter = NakedObjectsContext.ObjectPersistor.CreateAdapter(nakedObject.Specification.IsQueryable && !forceEnumerable ? (IEnumerable)result.AsQueryable() : result, null, null);
            adapter.SetATransientOid(new CollectionMemento(NakedObjectsContext.ObjectPersistor, NakedObjectsContext.Reflector, NakedObjectsContext.Session, nakedObject.Oid as CollectionMemento, selected));
            return adapter;
        }

        protected void Decrypt(FormCollection form) {
            if (EncryptDecryptService != null) {
                EncryptDecryptService.Decrypt(Session, form);
            }
        }

        internal void UpdateViewAndController(ActionExecutedContext filterContext) {
            if (filterContext.Result is ViewResultBase) {
                var viewResult = ((ViewResultBase) filterContext.Result);
                object model = viewResult.ViewData.Model;

                if (model is FindViewModel) {
                    SetControllerName(((FindViewModel) model).ContextObject);
                }
                else if (model is ActionResultModel) {
                    INakedObject nakedObject = NakedObjectsContext.GetNakedObject(((ActionResultModel) model).Result);
                    SetControllerName(nakedObject.Specification.GetFacet<ITypeOfFacet>().ValueSpec.ShortName);
                }
                else if (model != null) {
                    INakedObject nakedObject = model is PropertyViewModel ? NakedObjectsContext.GetNakedObject(((PropertyViewModel) model).ContextObject) : NakedObjectsContext.GetNakedObject(model);

                    if (nakedObject.Specification.IsCollection) {
                        SetControllerName(nakedObject.Specification.GetFacet<ITypeOfFacet>().ValueSpec.ShortName);
                    }
                    else {
                        SetControllerName(nakedObject.Object);
                    }

                    if (viewResult.ViewName == "ViewNameSetAfterTransaction") {
                        if (nakedObject.ResolveState.IsTransient()) {
                            viewResult.ViewName = model is PropertyViewModel ? "PropertyEdit" : "ObjectEdit";
                        }
                        else if (nakedObject.ResolveState.IsDestroyed()) {
                            viewResult.ViewName = "DestroyedError";
                        }
                        else if (nakedObject.IsViewModelEditView()) {
                            viewResult.ViewName = "ViewModel";
                        }
                        else if (nakedObject.Specification.IsFile(NakedObjectsContext)) {
                            filterContext.Result = AsFile(nakedObject.Object);
                        }
                        else if (nakedObject.Specification.IsParseable) {
                            viewResult.ViewName = "ScalarView";
                        }
                        else {
                            viewResult.ViewName = model is PropertyViewModel ? "PropertyView" : "ObjectView";
                        }
                    }
                }
            }
        }

        protected FileContentResult AsFile(object domainObject) {
            if (domainObject is FileAttachment) {
                var fileAttachment = domainObject as FileAttachment;
                bool addHeader = !string.IsNullOrWhiteSpace(fileAttachment.DispositionType);

                if (addHeader) {
                    string dispositionValue = string.Format("{0}; filename={1}", fileAttachment.DispositionType, fileAttachment.Name);
                    Response.AddHeader("Content-Disposition", dispositionValue);
                }

                Stream stream = fileAttachment.GetResourceAsStream();
                using (var br = new BinaryReader(stream)) {
                    byte[] bytes = br.ReadBytes((int) stream.Length);
                    var mimeType = fileAttachment.MimeType ?? "image/bmp";

                    // need to use different File overloads or will end up with two content-disposition headers 
                    return addHeader ? File(bytes, mimeType) : File(bytes, mimeType, fileAttachment.Name);
                }
            }
            var byteArray = domainObject as byte[];
            return File(byteArray, "application/octet-stream");
        }
    }
}