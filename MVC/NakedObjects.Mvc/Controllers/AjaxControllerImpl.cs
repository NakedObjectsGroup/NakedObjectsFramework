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
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;
using NakedObjects.Resources;
using NakedObjects.Surface;
using NakedObjects.Web.Mvc.Helpers;
using NakedObjects.Web.Mvc.Html;

namespace NakedObjects.Web.Mvc.Controllers {
    public class AjaxControllerImpl : NakedObjectsController {
        public AjaxControllerImpl(INakedObjectsFramework nakedObjectsContext, INakedObjectsSurface surface, IOidStrategy oidStrategy) : base(nakedObjectsContext, surface, oidStrategy) {}

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
            INakedObjectAdapter nakedObject = NakedObjectsContext.GetNakedObjectFromId(id);

            if (nakedObject.ResolveState.IsTransient()) {
                // if transient then we cannot validate now - need to wait until save 
                return Jsonp(true);
            }

            IAssociationSpec property = (nakedObject.GetObjectSpec()).Properties.SingleOrDefault(p => p.Id == propertyName);
            string fieldId = GetFieldInputId(nakedObject, property);

            bool isValid = false;

            if (value == null) {
                value = Request.Params[fieldId];
            }

            if (property != null && property is IOneToOneAssociationSpec) {
                ValidateAssociation(nakedObject, property as IOneToOneAssociationSpec, value);
                isValid = ModelState.IsValid;
            }

            if (isValid) {
                return Jsonp(true);
            }

            ModelError error = ModelState[fieldId].Errors.FirstOrDefault();
            return Jsonp(error == null ? "" : error.ErrorMessage);
        }

        private string GetFieldInputId(INakedObjectAdapter nakedObject, IAssociationSpec property) {
            string fieldId;

            var aoid = nakedObject.Oid as IAggregateOid;
            if (aoid != null) {
                IAssociationSpec parent = ((IObjectSpec) aoid.ParentOid.Spec).Properties.SingleOrDefault(p => p.Id == aoid.FieldName);
                fieldId = IdHelper.GetInlineFieldInputId(parent, ScaffoldAdapter.Wrap(nakedObject), property);
            }
            else {
                fieldId = IdHelper.GetFieldInputId(ScaffoldAdapter.Wrap(nakedObject), property);
            }
            return fieldId;
        }

        public virtual JsonResult ValidateParameter(string id, string value, string actionName, string parameterName) {
            INakedObjectAdapter nakedObject = NakedObjectsContext.GetNakedObjectFromId(id);
            IActionSpec action = NakedObjectsContext.GetActions(nakedObject).SingleOrDefault(a => a.Id == actionName);
            bool isValid = false;
            string parmId = "";

            if (action != null) {
                IActionParameterSpec parameter = action.Parameters.Where(p => p.Id.Equals(parameterName, StringComparison.InvariantCultureIgnoreCase)).Single();
                parmId = IdHelper.GetParameterInputId(action, parameter);

                if (value == null) {
                    value = Request.Params[parmId];
                }
                try {
                    INakedObjectAdapter valueNakedObject = GetParameterValue(parameter, value);
                    ValidateParameter(action, parameter, nakedObject, valueNakedObject);
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

        private INakedObjectAdapter GetValue(string[] values, ISpecification featureSpec, ITypeSpec spec) {
            if (!values.Any()) {
                return null;
            }

            if (spec.IsParseable) {
                return spec.GetFacet<IParseableFacet>().ParseTextEntry(values.First(), NakedObjectsContext.NakedObjectManager);
            }
            if (spec.IsCollection) {
                return NakedObjectsContext.GetTypedCollection(featureSpec, values);
            }

            return NakedObjectsContext.GetNakedObjectFromId(values.First());
        }

        private string[] GetRawValues(FormCollection parms, string id) {
            var values = new List<string>();

            for (int i = 0; parms.AllKeys.Contains(id + i); i++) {
                values.Add(parms[id + i]);
            }

            return values.ToArray();
        }

        private IDictionary<string, INakedObjectAdapter> GetOtherValues(IActionSpec action) {
            var results = new Dictionary<string, INakedObjectAdapter>();
            var parms = new FormCollection(HttpContext.Request.Params);

            Decrypt(parms);

            foreach (IActionParameterSpec parm in action.Parameters) {
                string[] values = GetRawValues(parms, IdHelper.GetParameterInputId(action, parm));
                results[parm.Id.ToLower()] = GetValue(values, parm, parm.Spec);
            }

            return results;
        }

        private IDictionary<string, INakedObjectAdapter> GetOtherValues(INakedObjectAdapter nakedObject) {
            var results = new Dictionary<string, INakedObjectAdapter>();
            var parms = new FormCollection(HttpContext.Request.Params);

            Decrypt(parms);

            foreach (IOneToOneAssociationSpec assoc in (nakedObject.GetObjectSpec()).Properties.OfType<IOneToOneAssociationSpec>()) {
                string[] values = GetRawValues(parms, GetFieldInputId(nakedObject, assoc));
                results[assoc.Id.ToLower()] = GetValue(values, assoc, assoc.ReturnSpec);
            }

            return results;
        }

        public virtual JsonResult GetActionChoices(string id, string actionName) {
            INakedObjectAdapter nakedObject = NakedObjectsContext.GetNakedObjectFromId(id);
            IActionSpec action = NakedObjectsContext.GetActions(nakedObject).SingleOrDefault(a => a.Id == actionName);
            IDictionary<string, string[][]> choices = new Dictionary<string, string[][]>();
            IDictionary<string, INakedObjectAdapter> otherValues = GetOtherValues(action);

            foreach (IActionParameterSpec p in action.Parameters) {
                if (p.IsChoicesEnabled || p.IsMultipleChoicesEnabled) {
                    INakedObjectAdapter[] nakedObjectChoices = p.GetChoices(nakedObject, otherValues);
                    string[] content = nakedObjectChoices.Select(c => c.TitleString()).ToArray();
                    string[] value = NakedObjectsContext.IsParseableOrCollectionOfParseable(p) ? content : nakedObjectChoices.Select(NakedObjectsContext.GetObjectId).ToArray();

                    choices[IdHelper.GetParameterInputId(action, p)] = new[] {value, content};
                }
            }
            return Jsonp(choices);
        }

        public virtual JsonResult GetPropertyChoices(string id) {
            INakedObjectAdapter nakedObject = NakedObjectsContext.GetNakedObjectFromId(id);
            IDictionary<string, string[][]> choices = new Dictionary<string, string[][]>();
            IDictionary<string, INakedObjectAdapter> otherValues = GetOtherValues(nakedObject);

            foreach (IOneToOneAssociationSpec assoc in (nakedObject.GetObjectSpec()).Properties.OfType<IOneToOneAssociationSpec>()) {
                if (assoc.IsChoicesEnabled) {
                    INakedObjectAdapter[] nakedObjectChoices = assoc.GetChoices(nakedObject, otherValues);
                    string[] content = nakedObjectChoices.Select(c => c.TitleString()).ToArray();
                    string[] value = assoc.ReturnSpec.IsParseable ? content : nakedObjectChoices.Select(NakedObjectsContext.GetObjectId).ToArray();

                    choices[GetFieldInputId(nakedObject, assoc)] = new[] {value, content};
                }
            }
            return Jsonp(choices);
        }

        private string GetIconSrc(INakedObjectAdapter nakedObject) {
            var url = new UrlHelper(ControllerContext.RequestContext);
            return url.Content("~/Images/" + FrameworkHelper.IconName(nakedObject));
        }

        private string GetIconAlt(INakedObjectAdapter nakedObject) {
            return nakedObject.Spec.SingularName;
        }

        private object GetCompletionData(INakedObjectAdapter nakedObject, ITypeSpec spec) {
            string label = nakedObject.TitleString();
            string value = nakedObject.TitleString();
            string link = spec.IsParseable ? label : NakedObjectsContext.GetObjectId(nakedObject);
            string src = GetIconSrc(nakedObject);
            string alt = GetIconAlt(nakedObject);
            return new {label, value, link, src, alt};
        }

        public virtual JsonResult GetPropertyCompletions(string id, string propertyId, string autoCompleteParm) {
            INakedObjectAdapter nakedObject = NakedObjectsContext.GetNakedObjectFromId(id);
            IList<object> completions = new List<object>();
            var assoc = (nakedObject.GetObjectSpec()).Properties.OfType<IOneToOneAssociationSpec>().Single(p => p.Id == propertyId);

            if (assoc.IsAutoCompleteEnabled) {
                INakedObjectAdapter[] nakedObjectCompletions = assoc.GetCompletions(nakedObject, autoCompleteParm);
                completions = nakedObjectCompletions.Select(no => GetCompletionData(no, assoc.ReturnSpec)).ToList();
            }

            return Jsonp(completions);
        }

        public virtual JsonResult GetActionCompletions(string id, string actionName, int parameterIndex, string autoCompleteParm) {
            INakedObjectAdapter nakedObject = NakedObjectsContext.GetNakedObjectFromId(id);
            IActionSpec action = NakedObjectsContext.GetActions(nakedObject).SingleOrDefault(a => a.Id == actionName);
            IList<object> completions = new List<object>();

            IActionParameterSpec p = action.Parameters[parameterIndex];
            if (p.IsAutoCompleteEnabled) {
                INakedObjectAdapter[] nakedObjectCompletions = p.GetCompletions(nakedObject, autoCompleteParm);
                completions = nakedObjectCompletions.Select(no => GetCompletionData(no, p.Spec)).ToList();
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