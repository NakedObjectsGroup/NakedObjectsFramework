// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NakedObjects.Resources;
using NakedObjects.Surface;
using NakedObjects.Surface.Context;
using NakedObjects.Surface.Utility;
using NakedObjects.Surface.Utility.Restricted;
using NakedObjects.Value;
using NakedObjects.Web.Mvc.Helpers;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Web.Mvc.Models;

namespace NakedObjects.Web.Mvc.Controllers {
    public abstract class NakedObjectsController : Controller {
        private readonly IIdHelper idHelper;
        private readonly IOidStrategy oidStrategy;
        private readonly INakedObjectsSurface surface;

        protected NakedObjectsController(INakedObjectsSurface surface,
                                         IIdHelper idHelper) {
            this.surface = surface;
            oidStrategy = surface.OidStrategy;
            this.idHelper = idHelper;
        }

        public IEncryptDecrypt EncryptDecryptService { protected get; set; }

        protected INakedObjectsSurface Surface {
            get { return surface; }
        }

        protected IOidStrategy OidStrategy {
            get { return oidStrategy; }
        }

        protected IIdHelper IdHelper {
            get { return idHelper; }
        }

        protected void SetControllerName(string name) {
            ControllerContext.RouteData.Values[IdConstants.Controller] = name;
        }

        protected void SetControllerName(object domainObject) {
            string controllerName = Surface.GetObjectTypeName(domainObject);
            SetControllerName(controllerName);
        }

        protected void SetServices() {
            ViewData[IdConstants.NofServices] = Surface.GetServices().List.Select(no => no.Object);
        }

        protected void SetSurface() {
            ViewData[IdConstants.NoSurface] = Surface;
            ViewData[IdConstants.IdHelper] = IdHelper;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            SetServices();
            SetSurface();
            Surface.Start();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext) {
            if (filterContext.Exception == null) {
                Surface.End(true);
            }
            else {
                try {
                    Surface.End(false);
                }
                catch {
                    // fail abort silently 
                }
            }

            SetMessagesAndWarnings();
            SetEncryptDecrypt();
        }

        internal ActionResult RedirectHome() {
            TempData[IdConstants.NofMessages] = Surface.MessageBroker.Messages;
            TempData[IdConstants.NofWarnings] = Surface.MessageBroker.Warnings;
            return RedirectToAction(IdConstants.IndexAction, IdConstants.HomeName);
        }

        internal ActionResult AppropriateView(ObjectAndControlData controlData, INakedObjectSurface nakedObject, INakedObjectActionSurface action = null, string propertyName = null) {
            if (nakedObject == null) {
                // no object to go to 
                // if action on object go to that object. 
                // if action on collection go to collection 
                // if action on service go to last object 

                nakedObject = controlData.GetNakedObject(Surface);

                if (nakedObject.Specification.IsService()) {
                    object lastObject = Session.LastObject(Surface, ObjectCache.ObjectFlag.BreadCrumb);
                    if (lastObject == null) {
                        return RedirectHome();
                    }

                    var oid = Surface.OidStrategy.GetOid(lastObject);
                    nakedObject = Surface.GetObject(oid).Target;
                }
            }

            if (nakedObject.Specification.IsCollection() && !nakedObject.Specification.IsParseable()) {
                int collectionSize = nakedObject.Count();
                if (collectionSize == 1) {
                    // remove any paging data - to catch case where custom page has embedded standalone collection as paging data will confuse rendering 
                    ViewData.Remove(IdConstants.PagingData);
                    // is this safe TODO !!
                    return View("ObjectView", nakedObject.ToEnumerable().First().Object);
                }

                nakedObject = Page(nakedObject, collectionSize, controlData);
                // todo is there a better way to do this ?
                action = action ?? nakedObject.MementoAction();
                int page, pageSize;
                CurrentlyPaging(controlData, collectionSize, out page, out pageSize);
                var format = ViewData["NofCollectionFormat"] as string;
                return View("StandaloneTable", ActionResultModel.Create(Surface, action, nakedObject, page, pageSize, format));
            }
            // remove any paging data - to catch case where custom page has embedded standalone collection as paging data will confuse rendering   
            ViewData.Remove(IdConstants.PagingData);

            if (controlData.DataDict.Values.Contains("max")) {
                // maximizing an inline object - do not update history
                ViewData.Add("updateHistory", false);
            }

            return propertyName == null ? View(nakedObject.IsNotPersistent() ? "ObjectView" : "ViewNameSetAfterTransaction", nakedObject.Object) :
                View(nakedObject.IsNotPersistent() ? "PropertyView" : "ViewNameSetAfterTransaction", new PropertyViewModel(nakedObject.Object, propertyName));
        }

        public void ValidateParameter(INakedObjectActionSurface action, INakedObjectActionParameterSurface parm, INakedObjectSurface targetNakedObject, object value) {
            var isValid = parm.IsValid(targetNakedObject, value);

            if (!isValid.IsAllowed) {
                ModelState.AddModelError(IdHelper.GetParameterInputId(action, parm), isValid.Reason);
            }
        }

        private bool CheckForAndAddCollectionMementoNew(string name, string[] values, ObjectAndControlData controlData) {
            if (values.Count() == 1) {
                var oid = Surface.OidStrategy.GetOid(values.First(), "");
                var nakedObject = Surface.GetObject(oid).Target;

                if (nakedObject != null && nakedObject.IsCollectionMemento()) {
                    nakedObject = FilterCollection(nakedObject, controlData);
                    AddAttemptedValue(name, nakedObject);
                    return true;
                }
            }
            return false;
        }

        internal void AddAttemptedValues(ObjectAndControlData controlData) {
            var action = controlData.GetAction(Surface);
            var form = controlData.Form;
            foreach (var parm in action.Parameters) {
                string name = IdHelper.GetParameterInputId(action, parm);
                ValueProviderResult vp = form.GetValue(name);
                string[] values = vp == null ? new string[] {} : (string[]) vp.RawValue;

                if (parm.Specification.IsCollection() && !parm.Specification.IsParseable()) {
                    // handle collection mementos 

                    if (parm.IsChoicesEnabled == Choices.Multiple || !CheckForAndAddCollectionMementoNew(name, values, controlData)) {
                        var itemSpec = parm.ElementType;
                        var itemvalues = values.Select(v => itemSpec.IsParseable() ? (object) v : GetNakedObjectFromId(v).Object).ToList();

                        if (itemvalues.Any()) {
                            var no = Surface.GetObject(itemvalues);

                            AddAttemptedValue(name, no);
                        }
                    }
                }
                else {
                    string value = values.Any() ? values.First() : "";

                    if (!string.IsNullOrEmpty(value)) {
                        AddAttemptedValue(name, parm.Specification.IsParseable() ? (object) value : FilterCollection(GetNakedObjectFromId(value), controlData));
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

        internal object GetParameterValue(INakedObjectActionParameterSurface parm, string name, ObjectAndControlData controlData) {
            object value = GetRawParameterValue(parm, controlData, name);
            return GetParameterValue(parm, value);
            // todo make this work
            //return FilterCollection(nakedObject, controlData);
        }

        internal object GetParameterValue(INakedObjectActionParameterSurface parm, object value) {
            if (value == null) {
                return null;
            }

            if (parm.Specification.IsStream()) {
                // todo not sure about this couple surface to http?
                // create stream wrapper ? 
                //    return fromStreamFacet.ParseFromStream(httpPostedFileBase.InputStream, httpPostedFileBase.ContentType, httpPostedFileBase.FileName, NakedObjectsContext.NakedObjectManager);
                return (HttpPostedFileBase) value;
            }

            var stringValue = value as string;
            if (parm.Specification.IsParseable()) {
                return string.IsNullOrEmpty(stringValue) ? null : stringValue;
            }

            var collectionValue = value as IEnumerable;
            if (!parm.Specification.IsCollection() || collectionValue == null) {
                return GetNakedObjectFromId(stringValue).Object;
            }

            return Surface.GetTypedCollection(parm, collectionValue);
        }

        private static object GetRawParameterValue(INakedObjectActionParameterSurface parm, ObjectAndControlData controlData, string name) {
            var form = controlData.Form;
            ValueProviderResult vp = form.GetValue(name);
            string[] values = vp == null ? null : (string[]) vp.RawValue;

            if (values == null) {
                if (controlData.Files.ContainsKey(name)) {
                    return controlData.Files[name];
                }
                return null;
            }
            if (parm.Specification.IsCollection() && !parm.Specification.IsParseable()) {
                return values.All(string.IsNullOrEmpty) ? null : values;
            }
            return values.First();
        }

        internal ArgumentsContext GetParameterValues(INakedObjectActionSurface action, ObjectAndControlData controlData) {
            var values = action.Parameters.Select(parm => new {Id = IdHelper.GetParameterInputId(action, parm), Parm = parm}).ToDictionary(a => a.Parm.Id, a => GetParameterValue(a.Parm, a.Id, controlData));

            return new ArgumentsContext() {Values = values, ValidateOnly = false};
        }

        internal void SetContextObjectAsParameterValue(INakedObjectActionSurface targetAction, INakedObjectSurface contextNakedObject) {
            if (targetAction.Parameters.Any(p => p.Specification.IsOfType(contextNakedObject.Specification))) {
                foreach (var parm in targetAction.Parameters) {
                    if (parm.Specification.IsOfType(contextNakedObject.Specification)) {
                        string name = IdHelper.GetParameterInputId(targetAction, parm);
                        AddAttemptedValue(name, contextNakedObject);
                    }
                }
            }
        }

        protected string DisplaySingleProperty(ObjectAndControlData controlData, IDictionary<string, string> data) {
            if (Request.IsAjaxRequest()) {
                var nakedObject = controlData.GetNakedObject(Surface);
                if (controlData.SubAction == ObjectAndControlData.SubActionType.Redisplay) {
                    var assocs = nakedObject.Specification.Properties.Where(p => p.IsCollection() && !p.Specification.IsParseable());
                    var item = assocs.SingleOrDefault(a => data.ContainsKey(a.Id));
                    return item == null ? null : item.Id;
                }
                if (controlData.ActionId == null) {
                    var assocs = nakedObject.Specification.Properties.Where(p => !p.IsCollection() || p.Specification.IsParseable());
                    var item = assocs.SingleOrDefault(a => data.ContainsKey(a.Id));
                    return item == null ? null : item.Id;
                }
                {
                    var parms = controlData.GetAction(Surface).Parameters;
                    var item = parms.SingleOrDefault(p => data.ContainsKey(p.Id));
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
            return id.EndsWith(IdConstants.MinDisplayFormat) ||
                   id.EndsWith(IdConstants.MaxDisplayFormat) ||
                   id.EndsWith(IdConstants.ListDisplayFormat) ||
                   id.EndsWith(IdConstants.SummaryDisplayFormat) ||
                   id.EndsWith(IdConstants.TableDisplayFormat);
        }

        internal void SetNewCollectionFormats(ObjectAndControlData controlData) {
            KeyValuePair<string, string>[] formats = controlData.DataDict.Where(kvp => IsFormat(kvp.Value)).ToArray();
            if (formats.Any()) {
                formats.ForEach(kvp => ViewData[kvp.Key] = kvp.Value);
            }
            else if (!string.IsNullOrWhiteSpace(controlData.Format)) {
                ViewData[IdConstants.CollectionFormat] = controlData.Format;
            }
        }

        internal void SetExistingCollectionFormats(FormCollection form) {
            if (form.AllKeys.Any(s => s.EndsWith(IdConstants.DisplayFormatFieldId))) {
                var id = form.AllKeys.Single(s => s.EndsWith(IdConstants.DisplayFormatFieldId));
                var values = form.GetValue(id).AttemptedValue.Split('&').ToDictionary(GetName, GetValue);
                values.Where(kvp => IsFormat(kvp.Value)).ForEach(kvp => ViewData[kvp.Key] = kvp.Value);
            }
        }

        internal void SetDefaults(INakedObjectSurface nakedObject, INakedObjectActionSurface action) {
            foreach (var parm in action.Parameters) {
                var value = parm.GetDefault(nakedObject);

                var isExplicit = parm.DefaultTypeIsExplicit(nakedObject);

                bool ignore = value == null || (value.Object is DateTime && ((DateTime) value.Object).Ticks == 0) || !isExplicit;
                if (!ignore) {
                    // deliberately not an attempted value so it only gets populated after masking 
                    ViewData[IdHelper.GetParameterInputId(action, parm)] = parm.Specification.IsParseable() ? value.Object : value;
                }
            }
        }

        protected INakedObjectSurface GetNakedObject(object domainObject) {
            if (domainObject == null) {
                return null;
            }

            return Surface.GetObject(domainObject);
        }

        protected INakedObjectSurface GetNakedObjectFromId(string id) {
            if (string.IsNullOrEmpty(id)) {
                return null;
            }

            var oid = Surface.OidStrategy.GetOid(id, "");
            return Surface.GetObject(oid).Target;
        }

        internal void SetSelectedReferences(INakedObjectSurface nakedObject, IDictionary<string, string> dict) {
            var refItems = (nakedObject.Specification.Properties.Where(p => !p.IsCollection() && !p.Specification.IsParseable())).Where(a => dict.ContainsKey(a.Id)).ToList();
            if (refItems.Any()) {
                refItems.ForEach(a => ValidateAssociation(nakedObject, a, dict[a.Id]));
                Dictionary<string, INakedObjectSurface> items = refItems.ToDictionary(a => IdHelper.GetFieldInputId(nakedObject, a), a => GetNakedObjectFromId(dict[a.Id]));
                items.ForEach(kvp => ViewData[kvp.Key] = kvp.Value);
            }
        }

        internal void SetSelectedParameters(INakedObjectActionSurface action) {
            var refItems = action.Parameters.Where(p => !p.Specification.IsCollection() && !p.Specification.IsParseable()).Where(p => ValueProvider.GetValue(p.Id) != null).ToList();
            if (refItems.Any()) {
                Dictionary<string, INakedObjectSurface> items = refItems.ToDictionary(p => IdHelper.GetParameterInputId(action, p), p => GetNakedObjectFromId(ValueProvider.GetValue(p.Id).AttemptedValue));
                items.ForEach(kvp => ViewData[kvp.Key] = kvp.Value);
            }
        }

        internal void SetSelectedParameters(INakedObjectSurface nakedObject, INakedObjectActionSurface action, IDictionary<string, string> dict) {
            var refItems = action.Parameters.Where(p => !p.Specification.IsCollection() && !p.Specification.IsParseable()).Where(p => dict.ContainsKey(p.Id)).ToList();
            if (refItems.Any()) {
                refItems.ForEach(p => ValidateParameter(action, p, nakedObject, GetNakedObjectFromId(dict[p.Id]).GetDomainObject<object>()));
                Dictionary<string, INakedObjectSurface> items = refItems.ToDictionary(p => IdHelper.GetParameterInputId(action, p), p => GetNakedObjectFromId(dict[p.Id]));
                items.ForEach(kvp => ViewData[kvp.Key] = kvp.Value);
            }
        }

        internal object GetObjectValue(INakedObjectAssociationSurface assoc, INakedObjectSurface targetNakedObject, object value) {
            if (value == null) {
                return null;
            }
            // todo
            if (assoc.Specification.IsStream()) {
                // todo not sure about this couple surface to http?
                // create stream wrapper ? 
                //    return fromStreamFacet.ParseFromStream(httpPostedFileBase.InputStream, httpPostedFileBase.ContentType, httpPostedFileBase.FileName, NakedObjectsContext.NakedObjectManager);
                return (HttpPostedFileBase) value;
            }

            if (assoc.Specification.IsParseable()) {
                return value;
            }

            if (!assoc.IsCollection()) {
                return Surface.OidStrategy.GetDomainObjectByOid(Surface.OidStrategy.GetOid(value.ToString(), ""));
            }
            // collection 
            return null;
        }

        internal void CheckConcurrency(INakedObjectSurface nakedObject, INakedObjectAssociationSurface parent, ObjectAndControlData controlData, Func<INakedObjectAssociationSurface, INakedObjectSurface, INakedObjectAssociationSurface, string> idFunc) {
            var objectSpec = nakedObject.Specification;
            var concurrencyFields = objectSpec == null ? new List<INakedObjectAssociationSurface>() : objectSpec.Properties.Where(p => p.IsConcurrency()).ToList();

            if (!nakedObject.IsTransient() && concurrencyFields.Any()) {
                IEnumerable<Tuple<INakedObjectAssociationSurface, object>> fieldsAndMatchingValues = GetFieldsAndMatchingValues(nakedObject, parent, concurrencyFields, controlData, idFunc);

                foreach (var pair in fieldsAndMatchingValues) {
                    if (pair.Item1.Specification.IsParseable()) {
                        var currentValue = pair.Item1.GetNakedObject(nakedObject);

                        // todo revisit this 
                        //var concurrencyValue = pair.Item1.ReturnSpec.GetFacet<IParseableFacet>().ParseInvariant(pair.Item2 as string, NakedObjectsContext.NakedObjectManager);

                        var concurrencyValue = pair.Item2 as string;

                        if (concurrencyValue != null && currentValue != null) {
                            if (concurrencyValue != currentValue.TitleString()) {
                                throw new PreconditionFailedNOSException(nakedObject);
                            }
                        }
                        else if (concurrencyValue == null && currentValue == null) {
                            // OK 
                        }
                        else {
                            throw new PreconditionFailedNOSException(nakedObject);
                        }
                    }
                }
            }
        }

        protected void GetUsableAndVisibleFields(INakedObjectSurface nakedObject, ObjectAndControlData controlData, INakedObjectAssociationSurface parent, out List<INakedObjectAssociationSurface> usableAndVisibleFields, out List<Tuple<INakedObjectAssociationSurface, object>> fieldsAndMatchingValues) {
            usableAndVisibleFields = nakedObject.Specification.Properties.Where(p => p.IsUsable(nakedObject).IsAllowed && p.IsVisible(nakedObject)).ToList();
            fieldsAndMatchingValues = GetFieldsAndMatchingValues(nakedObject, parent, usableAndVisibleFields, controlData, GetFieldInputId).ToList();
        }

        protected static IEnumerable<Tuple<INakedObjectAssociationSurface, object>> GetFieldsAndMatchingValues(INakedObjectSurface nakedObject,
                                                                                                               INakedObjectAssociationSurface parent,
                                                                                                               IEnumerable<INakedObjectAssociationSurface> associations,
                                                                                                               ObjectAndControlData controlData,
                                                                                                               Func<INakedObjectAssociationSurface, INakedObjectSurface, INakedObjectAssociationSurface, string> idFunc) {
            foreach (var assoc in associations.Where(a => !a.IsInline())) {
                string name = idFunc(parent, nakedObject, assoc);
                object newValue = GetValueFromForm(controlData, name);
                yield return new Tuple<INakedObjectAssociationSurface, object>(assoc, newValue);
            }
        }

        private static object GetValueFromForm(ObjectAndControlData controlData, string name) {
            var form = controlData.Form;

            if (form.GetValue(name) != null) {
                return ((string[]) form.GetValue(name).RawValue).First();
            }

            return controlData.Files.ContainsKey(name) ? controlData.Files[name] : null;
        }

        internal void AddErrorAndAttemptedValue(INakedObjectSurface nakedObject, string newValue, INakedObjectAssociationSurface assoc, string errorText, INakedObjectAssociationSurface parent = null) {
            string key = GetFieldInputId(parent, nakedObject, assoc);
            ModelState.AddModelError(key, errorText);
            AddAttemptedValue(key, assoc.Specification.IsParseable() ? (object) newValue : GetNakedObjectFromId(newValue));
        }

        internal void AddAttemptedValuesNew(INakedObjectSurface nakedObject, ObjectAndControlData controlData, INakedObjectAssociationSurface parent = null) {
            foreach (var assoc in nakedObject.Specification.Properties.Where(p => p.IsUsable(nakedObject).IsAllowed && p.IsVisible(nakedObject) || p.IsConcurrency())) {
                string name = GetFieldInputId(parent, nakedObject, assoc);
                string value = GetValueFromForm(controlData, name) as string;
                if (value != null) {
                    AddAttemptedValue(name, value);
                }
            }

            foreach (var assoc in nakedObject.Specification.Properties.Where(p => p.IsConcurrency())) {
                string name = GetConcurrencyFieldInputId(parent, nakedObject, assoc);
                string value = GetValueFromForm(controlData, name) as string;
                if (value != null) {
                    AddAttemptedValue(name, value);
                }
            }

            foreach (var assoc in (nakedObject.Specification.Properties.Where(p => p.IsInline()))) {
                var inlineNakedObject = assoc.GetNakedObject(nakedObject);
                AddAttemptedValuesNew(inlineNakedObject, controlData, assoc);
            }
        }

        internal ArgumentsContext ConvertForSave(INakedObjectSurface nakedObject, ObjectAndControlData controlData, bool validateOnly = false) {
            List<INakedObjectAssociationSurface> usableAndVisibleFields;
            List<Tuple<INakedObjectAssociationSurface, object>> fieldsAndMatchingValues;
            GetUsableAndVisibleFields(nakedObject, controlData, null, out usableAndVisibleFields, out fieldsAndMatchingValues);

            var ac = new ArgumentsContext {
                ValidateOnly = validateOnly,
                Values = fieldsAndMatchingValues.ToDictionary(t => t.Item1.Id, f => GetObjectValue(f.Item1, nakedObject, f.Item2))
            };

            return ac;
        }

        internal ArgumentsContext Convert(FormCollection form, bool validateOnly = false) {
            var ac = new ArgumentsContext {
                ValidateOnly = validateOnly,
                Values = form.AllKeys.ToDictionary(k => k, k => form.GetValue(k).RawValue)
            };

            return ac;
        }

        internal void RefreshTransient(INakedObjectSurface nakedObject, FormCollection form, INakedObjectAssociationSurface parent = null) {
            if (nakedObject.IsTransient()) {
                var ac = Convert(form);
                Surface.RefreshObject(nakedObject, ac);
            }
        }

        protected string GetFieldInputId(INakedObjectAssociationSurface parent, INakedObjectSurface nakedObject, INakedObjectAssociationSurface assoc) {
            return parent == null ? IdHelper.GetFieldInputId(nakedObject, assoc) :
                IdHelper.GetInlineFieldInputId(parent, nakedObject, assoc);
        }

        protected string GetConcurrencyFieldInputId(INakedObjectAssociationSurface parent, INakedObjectSurface nakedObject, INakedObjectAssociationSurface assoc) {
            return parent == null ? IdHelper.GetConcurrencyFieldInputId(nakedObject, assoc) :
                IdHelper.GetInlineConcurrencyFieldInputId(parent, nakedObject, assoc);
        }

        internal void ValidateAssociation(INakedObjectSurface nakedObject, INakedObjectAssociationSurface oneToOneAssoc, object attemptedValue, INakedObjectAssociationSurface parent = null) {
            string key = GetFieldInputId(parent, nakedObject, oneToOneAssoc);
            try {
                var oid = Surface.OidStrategy.GetOid(nakedObject);

                var ac = new ArgumentContext {
                    Value = attemptedValue,
                    ValidateOnly = true
                };

                var pcs = Surface.PutProperty(oid, oneToOneAssoc.Id, ac);

                if (!string.IsNullOrEmpty(pcs.Reason)) {
                    ModelState.AddModelError(key, pcs.Reason);
                }
            }
                //catch (InvalidEntryException) {

            catch (NakedObjectsSurfaceException) {
                // todo find correct NOS exception
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

        protected void AddAttemptedValue(string key, object value) {
            ModelState.SetModelValue(key, new ValueProviderResult(value, value == null ? string.Empty : value.ToString(), null));
        }

        internal static bool ActionExecutingAsContributed(INakedObjectActionSurface action, INakedObjectSurface targetNakedObject) {
            return action.IsContributed() && !action.OnType.Equals(targetNakedObject.Specification);
        }

        internal void SetMessagesAndWarnings() {
            string[] messages = Surface.MessageBroker.Messages;
            string[] warnings = Surface.MessageBroker.Warnings;

            var existingMessages = TempData[IdConstants.NofMessages];
            var existingWarnings = TempData[IdConstants.NofWarnings];

            if (existingMessages is string[] && ((string[]) existingMessages).Length > 0) {
                messages = ((string[]) existingMessages).Union(messages).ToArray();
            }

            if (existingWarnings is string[] && ((string[]) existingWarnings).Length > 0) {
                warnings = ((string[]) existingWarnings).Union(warnings).ToArray();
            }

            ViewData.Add(IdConstants.NofMessages, messages);
            ViewData.Add(IdConstants.NofWarnings, warnings);
        }

        internal void SetEncryptDecrypt() {
            ViewData.Add(IdConstants.NofEncryptDecrypt, EncryptDecryptService);
        }

        internal void SetPagingValues(ObjectAndControlData controlData, INakedObjectSurface nakedObject) {
            if (nakedObject.Specification.IsCollection()) {
                int sink1, sink2;
                CurrentlyPaging(controlData, nakedObject.Count(), out sink1, out sink2);
            }
        }

        internal bool CurrentlyPaging(ObjectAndControlData controlData, int collectionSize, out int page, out int pageSize) {
            pageSize = GetPageSize(controlData);
            page = 1;

            if (pageSize > 0) {
                page = GetPage(controlData);

                var pagingData = new Dictionary<string, int> {
                    {IdConstants.PagingCurrentPage, page},
                    {IdConstants.PagingPageSize, pageSize},
                    {IdConstants.PagingTotal, collectionSize}
                };
                ViewData[IdConstants.PagingData] = pagingData;
                return true;
            }

            return false;
        }

        private static int GetPage(ObjectAndControlData controlData) {
            if (controlData.DataDict.ContainsKey(IdConstants.PageKey)) {
                return int.Parse(controlData.DataDict[IdConstants.PageKey]);
            }
            return !string.IsNullOrEmpty(controlData.Page) ? int.Parse(controlData.Page) : 1;
        }

        internal int GetPageSize(ObjectAndControlData controlData) {
            if (controlData.DataDict.ContainsKey(IdConstants.PageSizeKey)) {
                return int.Parse(controlData.DataDict[IdConstants.PageSizeKey]);
            }
            if (!string.IsNullOrEmpty(controlData.PageSize)) {
                return int.Parse(controlData.PageSize);
            }

            var action = controlData.GetAction(Surface);
            return action != null ? action.PageSize() : 0;
        }

        internal INakedObjectSurface Page(INakedObjectSurface nakedObject, int collectionSize, ObjectAndControlData controlData) {
            int page, pageSize;

            if (CurrentlyPaging(controlData, collectionSize, out page, out pageSize) && !nakedObject.IsPaged()) {
                return nakedObject.Page(page, pageSize);
            }

            // one page of full collection 
            return nakedObject.Page(1, collectionSize);
        }

        internal INakedObjectSurface FilterCollection(INakedObjectSurface nakedObject, ObjectAndControlData controlData) {
            var form = controlData.Form;
            if (form != null && nakedObject != null && nakedObject.Specification.IsCollection() /*&& nakedObject.Oid is ICollectionMemento  todo */) {
                nakedObject = Page(nakedObject, nakedObject.Count(), controlData);
                var map = nakedObject.ToEnumerable().ToDictionary(Surface.OidStrategy.GetObjectId, y => y.Object);
                var selected = map.Where(kvp => form.Keys.Cast<string>().Contains(kvp.Key) && form[kvp.Key].Contains("true")).Select(kvp => kvp.Value).ToArray();
                return nakedObject.Select(selected, false);
            }

            return nakedObject;
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
                    var nakedObject = GetNakedObject(((ActionResultModel) model).Result);
                    SetControllerName(nakedObject.Specification.FullName().Split('.').Last());
                }
                else if (model != null) {
                    var nakedObject = model is PropertyViewModel ? GetNakedObject(((PropertyViewModel) model).ContextObject) : GetNakedObject(model);

                    if (nakedObject.Specification.IsCollection() && !nakedObject.Specification.IsParseable()) {
                        //2nd clause is to avoid rendering a string as a collection
                        SetControllerName(nakedObject.Specification.FullName().Split('.').Last());
                    }
                    else {
                        SetControllerName(nakedObject.Object);
                    }

                    if (viewResult.ViewName == "ViewNameSetAfterTransaction") {
                        // todo sort
                        if (nakedObject.IsTransient()) {
                            viewResult.ViewName = model is PropertyViewModel ? "PropertyEdit" : "ObjectEdit";
                        }
                        else if (nakedObject.IsDestroyed()) {
                            viewResult.ViewName = "DestroyedError";
                        }
                        else if (nakedObject.IsViewModelEditView()) {
                            viewResult.ViewName = "ViewModel";
                        }
                        else if (nakedObject.Specification.IsFile()) {
                            filterContext.Result = AsFile(nakedObject.Object);
                        }
                        else if (nakedObject.Specification.IsParseable()) {
                            viewResult.ViewName = "ScalarView";
                        }
                        else {
                            viewResult.ViewName = model is PropertyViewModel ? "PropertyView" : "ObjectView";
                        }
                    }
                }
            }
        }

        protected bool HasError(ObjectContextSurface ar) {
            return !string.IsNullOrEmpty(ar.Reason) || ar.VisibleProperties.Any(p => !string.IsNullOrEmpty(p.Reason));
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