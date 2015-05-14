// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Resources;
using NakedObjects.Surface;
using NakedObjects.Surface.Context;
using NakedObjects.Surface.Utility;
using NakedObjects.Web.Mvc.Helpers;
using NakedObjects.Web.Mvc.Html;

namespace NakedObjects.Web.Mvc.Controllers {
    public class AjaxControllerImpl : NakedObjectsController {
        public AjaxControllerImpl(INakedObjectsSurface surface, IIdHelper idHelper) : base(surface, idHelper) { }

        protected internal JsonpResult Jsonp(object data) {
            return Jsonp(data, null /* contentType */);
        }

        protected internal JsonpResult Jsonp(object data, string contentType) {
            return Jsonp(data, contentType, null);
        }

        protected internal virtual JsonpResult Jsonp(object data, string contentType, Encoding contentEncoding) {
            return new JsonpResult {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public virtual JsonResult ValidateProperty(string id, string value, string propertyName) {
            var nakedObject = GetNakedObjectFromId(id);

            if (nakedObject.IsTransient()) {
                // if transient then we cannot validate now - need to wait until save 
                return Jsonp(true);
            }

            var property = nakedObject.Specification.Properties.SingleOrDefault(p => p.Id == propertyName);
            string fieldId = IdHelper.GetAggregateFieldInputId(nakedObject, property);

            bool isValid = false;

            if (value == null) {
                value = Request.Params[fieldId];
            }

           
            if (property != null && (!property.IsCollection() || property.Specification.IsParseable())) {
                var pvalue = GetValue(new[] { value }, property, property.Specification);
                ValidateAssociation(nakedObject, property, pvalue);
                isValid = ModelState.IsValid;
            }

            if (isValid) {
                return Jsonp(true);
            }

            ModelError error = ModelState[fieldId].Errors.FirstOrDefault();
            return Jsonp(error == null ? "" : error.ErrorMessage);
        }

        //private string GetFieldInputId(INakedObjectAdapter nakedObject, IAssociationSpec property) {
        //    string fieldId;

        //    var aoid = nakedObject.Oid as IAggregateOid;
        //    if (aoid != null) {
        //        IAssociationSpec parent = ((IObjectSpec) aoid.ParentOid.Spec).Properties.SingleOrDefault(p => p.Id == aoid.FieldName);
        //        fieldId = IdHelper.GetInlineFieldInputId(ScaffoldAssoc.Wrap(parent), ScaffoldAdapter.Wrap(nakedObject), ScaffoldAssoc.Wrap(property));
        //    }
        //    else {
        //        fieldId = IdHelper.GetFieldInputId(ScaffoldAdapter.Wrap(nakedObject), ScaffoldAssoc.Wrap(property));
        //    }
        //    return fieldId;
        //}

        public virtual JsonResult ValidateParameter(string id, string value, string actionName, string parameterName) {
            var nakedObject = GetNakedObjectFromId(id);
            var action = nakedObject.Specification.GetActionLeafNodes().SingleOrDefault(a => a.Id == actionName);
            bool isValid = false;
            string parmId = "";

            if (action != null) {
                var parameter = action.Parameters.Single(p => p.Id.Equals(parameterName, StringComparison.InvariantCultureIgnoreCase));
                parmId = IdHelper.GetParameterInputId(action, parameter);

                if (value == null) {
                    value = Request.Params[parmId];
                }
                try {
                    var parameterValue = GetParameterValue(parameter, value);
                    ValidateParameter(action, parameter, nakedObject, parameterValue);
                }
                catch (InvalidEntryException) {
                    ModelState.AddModelError(parmId, MvcUi.InvalidEntry);
                }

                isValid = ModelState.IsValid;
            }

            if (isValid) {
                return Jsonp(true);
            }

            ModelError error = ModelState[parmId].Errors.FirstOrDefault();
            return Jsonp(error == null ? "" : error.ErrorMessage);
        }

        // todo consolidate these
        private object GetValue(string[] values, INakedObjectActionParameterSurface featureSpec, INakedObjectSpecificationSurface spec) {
            if (!values.Any()) {
                return null;
            }

            if (spec.IsParseable()) {
                var v = values.First();
                return string.IsNullOrEmpty(v) ? null : v;
            }
            if (spec.IsCollection()) {
                return Surface.GetTypedCollection(featureSpec, values);
            }

            return GetNakedObjectFromId(values.First()).Object;
        }

        private object GetValue(string[] values, INakedObjectAssociationSurface featureSpec, INakedObjectSpecificationSurface spec) {
            if (!values.Any()) {
                return null;
            }

            if (spec.IsParseable()) {
                return values.First();
            }
            if (spec.IsCollection()) {
                return Surface.GetTypedCollection(featureSpec, values);
            }

            return GetNakedObjectFromId(values.First()).GetDomainObject<object>();
        }


        private string[] GetRawValues(FormCollection parms, string id) {
            var values = new List<string>();

            for (int i = 0; parms.AllKeys.Contains(id + i); i++) {
                values.Add(parms[id + i]);
            }

            return values.ToArray();
        }


        private IDictionary<string, object> GetOtherValues(INakedObjectActionSurface action) {
            var results = new Dictionary<string, object>();
            var parms = new FormCollection(HttpContext.Request.Params);

            Decrypt(parms);

            foreach (var parm in action.Parameters) {
                string[] values = GetRawValues(parms, IdHelper.GetParameterInputId(action, parm));
                results[parm.Id.ToLower()] = GetValue(values, parm, parm.Specification);
            }

            return results;
        }

        private IDictionary<string, object> GetOtherValues(INakedObjectSurface nakedObject) {
            var results = new Dictionary<string, object>();
            var parms = new FormCollection(HttpContext.Request.Params);

            Decrypt(parms);

            foreach (var assoc in nakedObject.Specification.Properties.Where(p => !p.IsCollection())) {
                string[] values = GetRawValues(parms, IdHelper.GetAggregateFieldInputId(nakedObject, assoc));
                results[assoc.Id.ToLower()] = GetValue(values, assoc, assoc.Specification);
            }

            return results;
        }

        public static bool IsParseableOrCollectionOfParseable( INakedObjectsSurface surface, INakedObjectActionParameterSurface parmSpec) {
            var spec = parmSpec.Specification;
            return spec.IsParseable() || (spec.IsCollection() && parmSpec.ElementType.IsParseable());
        }


        //public virtual JsonResult GetActionChoices(string id, string actionName) {
        //    INakedObjectAdapter nakedObject = NakedObjectsContext.GetNakedObjectFromId(id);
        //    IActionSpec action = NakedObjectsContext.GetActions(nakedObject).SingleOrDefault(a => a.Id == actionName);
        //    IDictionary<string, string[][]> choices = new Dictionary<string, string[][]>();
        //    IDictionary<string, INakedObjectAdapter> otherValues = GetOtherValues(action);

        //    foreach (IActionParameterSpec p in action.Parameters) {
        //        if (p.IsChoicesEnabled || p.IsMultipleChoicesEnabled) {
        //            INakedObjectAdapter[] nakedObjectChoices = p.GetChoices(nakedObject, otherValues);
        //            string[] content = nakedObjectChoices.Select(c => c.TitleString()).ToArray();
        //            string[] value = NakedObjectsContext.IsParseableOrCollectionOfParseable(p) ? content : nakedObjectChoices.Select(NakedObjectsContext.GetObjectId).ToArray();

        //            choices[IdHelper.GetParameterInputId(action, p)] = new[] { value, content };
        //        }
        //    }
        //    return Jsonp(choices);
        //}


        public virtual JsonResult GetActionChoices(string id, string actionName) {
            var nakedObject = GetNakedObjectFromId(id);
            var action = nakedObject.Specification.GetActionLeafNodes().Single(a => a.Id == actionName);
            IDictionary<string, string[][]> choices = new Dictionary<string, string[][]>();
            var otherValues = GetOtherValues(action);

            foreach (var p in action.Parameters) {
                if (p.IsChoicesEnabled != Choices.NotEnabled) {
                    var nakedObjectChoices = p.GetChoices(nakedObject, otherValues);
                    string[] content = nakedObjectChoices.Select(c => c.TitleString()).ToArray();
                    string[] value = IsParseableOrCollectionOfParseable(Surface, p) ? content : nakedObjectChoices.Select(o => Surface.OidStrategy.GetOid(o).ToString()).ToArray();

                    choices[IdHelper.GetParameterInputId(action, p)] = new[] { value, content };
                }
            }
            return Jsonp(choices);
        }

        public virtual JsonResult GetPropertyChoices(string id) {
            var nakedObject = GetNakedObjectFromId(id);
            IDictionary<string, string[][]> choices = new Dictionary<string, string[][]>();
            var otherValues = GetOtherValues(nakedObject);

            foreach (var assoc in nakedObject.Specification.Properties) {
                if (assoc.IsChoicesEnabled != Choices.NotEnabled) {
                    var nakedObjectChoices = assoc.GetChoices(nakedObject, otherValues);
                    string[] content = nakedObjectChoices.Select(c => c.TitleString()).ToArray();
                    string[] value = assoc.Specification.IsParseable() ? content : nakedObjectChoices.Select(o => Surface.OidStrategy.GetOid(o).ToString()).ToArray();

                    choices[IdHelper.GetAggregateFieldInputId(nakedObject, assoc)] = new[] {value, content};
                }
            }
            return Jsonp(choices);
        }

        public static string IconName(INakedObjectSurface nakedObject) {
            string name = nakedObject.Specification.GetIconName(nakedObject);
            return name.Contains(".") ? name : name + ".png";
        }

        private string GetIconSrc(INakedObjectSurface nakedObject) {
            var url = new UrlHelper(ControllerContext.RequestContext);
            return url.Content("~/Images/" + IconName(nakedObject));
        }

        private string GetIconAlt(INakedObjectSurface nakedObject) {
            return nakedObject.Specification.SingularName();
        }

        private object GetCompletionData(INakedObjectSurface nakedObject, INakedObjectSpecificationSurface spec) {
            string label = nakedObject.TitleString();
            string value = nakedObject.TitleString();
            string link = spec.IsParseable() ? label : Surface.OidStrategy.GetOid(nakedObject).ToString();
            string src = GetIconSrc(nakedObject);
            string alt = GetIconAlt(nakedObject);
            return new {label, value, link, src, alt};
        }

        public virtual JsonResult GetPropertyCompletions(string id, string propertyId, string autoCompleteParm) {
            var nakedObject = GetNakedObjectFromId(id);
            IList<object> completions = new List<object>();
            var assoc = nakedObject.Specification.Properties.SingleOrDefault(p => p.Id == propertyId && p.IsAutoCompleteEnabled);

            if (assoc != null) {
                var nakedObjectCompletions = assoc.GetCompletions(nakedObject, autoCompleteParm);
                completions = nakedObjectCompletions.Select(no => GetCompletionData(no, assoc.Specification)).ToList();
            }

            return Jsonp(completions);
        }

        public virtual JsonResult GetActionCompletions(string id, string actionName, int parameterIndex, string autoCompleteParm) {
            var nakedObject = GetNakedObjectFromId(id);
            var action = nakedObject.Specification.GetActionLeafNodes().Single(a => a.Id == actionName);
            IList<object> completions = new List<object>();

            var p = action.Parameters[parameterIndex];
            if (p.IsAutoCompleteEnabled) {
                var nakedObjectCompletions = p.GetCompletions(nakedObject, autoCompleteParm);
                completions = nakedObjectCompletions.Select(no => GetCompletionData(no, p.Specification)).ToList();
            }

            return Jsonp(completions);
        }

        public class JsonpResult : JsonResult {
            public override void ExecuteResult(ControllerContext context) {
                if (context == null) {
                    throw new ArgumentNullException("context");
                }

                HttpResponseBase response = context.HttpContext.Response;

                response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;
                if (ContentEncoding != null) {
                    response.ContentEncoding = ContentEncoding;
                }
                if (Data != null) {
                    string callback = context.HttpContext.Request.Params["callback"];
                    string serializedData = new JavaScriptSerializer().Serialize(Data);
                    response.Write(string.IsNullOrEmpty(callback) ? serializedData : callback + "(" + serializedData + ")");
                }
            }
        }
    }
}