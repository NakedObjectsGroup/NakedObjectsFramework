// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter.Map;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Resolve;
using NakedObjects.Resources;
using NakedObjects.Web.Mvc.Html;

namespace NakedObjects.Web.Mvc.Controllers {
    public class AjaxControllerImpl : NakedObjectsController {
        public AjaxControllerImpl(INakedObjectsFramework nakedObjectsContext) : base(nakedObjectsContext) {}

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
            INakedObject nakedObject = NakedObjectsContext.GetNakedObjectFromId(id);

            if (nakedObject.ResolveState.IsTransient() && !MvcIdentityAdapterHashMap.StoringTransientsInSession) {
                // if transient and not saving transients in session then we cannot validate now - need to wait until save 
                return Jsonp(true);
            }

            IAssociationSpec property = nakedObject.Spec.Properties.SingleOrDefault(p => p.Id == propertyName);
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

        private static string GetFieldInputId(INakedObject nakedObject, IAssociationSpec property) {
            string fieldId;

            if (nakedObject.Oid is AggregateOid) {
                var aoid = ((AggregateOid) nakedObject.Oid);
                IAssociationSpec parent = aoid.ParentOid.Spec.Properties.Where(p => p.Id == aoid.FieldName).SingleOrDefault();
                fieldId = IdHelper.GetInlineFieldInputId(parent, nakedObject, property);
            }
            else {
                fieldId = IdHelper.GetFieldInputId(nakedObject, property);
            }
            return fieldId;
        }

        public virtual JsonResult ValidateParameter(string id, string value, string actionName, string parameterName) {
            INakedObject nakedObject = NakedObjectsContext.GetNakedObjectFromId(id);
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
                    INakedObject valueNakedObject = GetParameterValue(parameter, value);
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

        private  INakedObject GetValue(string[] values, IFeatureSpec featureSpec) {
            var spec = featureSpec.Spec;
            if (!values.Any()) {
                return null;
            }
         
            if (spec.IsParseable) {
                return spec.GetFacet<IParseableFacet>().ParseTextEntry(values.First(), NakedObjectsContext.Manager);
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


        private IDictionary<string, INakedObject> GetOtherValues(IActionSpec action) {
            var results = new Dictionary<string, INakedObject>();
            var parms = new FormCollection(HttpContext.Request.Params);

            Decrypt(parms);

            foreach (IActionParameterSpec parm in action.Parameters) {
                string[] values =  GetRawValues(parms, IdHelper.GetParameterInputId(action, parm));
                results[parm.Id.ToLower()] = GetValue(values, parm);
            }

            return results;
        }

        private IDictionary<string, INakedObject> GetOtherValues(INakedObject nakedObject) {
            var results = new Dictionary<string, INakedObject>();
            var parms = new FormCollection(HttpContext.Request.Params);

            Decrypt(parms);

            foreach (IOneToOneAssociationSpec assoc in nakedObject.Spec.Properties.Where(a => a.IsObject)) {
                string[] values = GetRawValues(parms, GetFieldInputId(nakedObject, assoc));
                results[assoc.Id.ToLower()] = GetValue(values, assoc);
            }

            return results;
        }


        public virtual JsonResult GetActionChoices(string id, string actionName) {
            INakedObject nakedObject = NakedObjectsContext.GetNakedObjectFromId(id);
            IActionSpec action = NakedObjectsContext.GetActions(nakedObject).SingleOrDefault(a => a.Id == actionName);
            IDictionary<string, string[][]> choices = new Dictionary<string, string[][]>();
            IDictionary<string, INakedObject> otherValues = GetOtherValues(action);

            foreach (IActionParameterSpec p in action.Parameters) {
                if (p.IsChoicesEnabled || p.IsMultipleChoicesEnabled) {
                    INakedObject[] nakedObjectChoices = p.GetChoices(nakedObject, otherValues);
                    string[] content = nakedObjectChoices.Select(c => c.TitleString()).ToArray();
                    string[] value = NakedObjectsContext.IsParseableOrCollectionOfParseable(p) ? content : nakedObjectChoices.Select(NakedObjectsContext.GetObjectId).ToArray();

                    choices[IdHelper.GetParameterInputId(action, p)] = new[] {value, content};
                }
            }
            return Jsonp(choices);
        }

     


        public virtual JsonResult GetPropertyChoices(string id) {
            INakedObject nakedObject = NakedObjectsContext.GetNakedObjectFromId(id);
            IDictionary<string, string[][]> choices = new Dictionary<string, string[][]>();
            IDictionary<string, INakedObject> otherValues = GetOtherValues(nakedObject);

            foreach (IOneToOneAssociationSpec assoc in nakedObject.Spec.Properties.Where(p => p.IsObject)) {
                if (assoc.IsChoicesEnabled) {
                    INakedObject[] nakedObjectChoices = assoc.GetChoices(nakedObject, otherValues);
                    string[] content = nakedObjectChoices.Select(c => c.TitleString()).ToArray();
                    string[] value = assoc.Spec.IsParseable ? content : nakedObjectChoices.Select(NakedObjectsContext.GetObjectId).ToArray();

                    choices[GetFieldInputId(nakedObject, assoc)] = new[] {value, content};
                }
            }
            return Jsonp(choices);
        }

        private string GetIconSrc(INakedObject nakedObject) {
            var url = new UrlHelper(ControllerContext.RequestContext);
            return url.Content("~/Images/" + FrameworkHelper.IconName(nakedObject));
        }

        private string GetIconAlt(INakedObject nakedObject) {
            return nakedObject.Spec.SingularName;
        }

        private object GetCompletionData(INakedObject nakedObject, IObjectSpec spec) {
            string label = nakedObject.TitleString();
            string value = nakedObject.TitleString();
            string link = spec.IsParseable ? label : NakedObjectsContext.GetObjectId(nakedObject);
            string src = GetIconSrc(nakedObject);
            string alt = GetIconAlt(nakedObject);
            return new {label, value, link, src, alt};
        }


        public virtual JsonResult GetPropertyCompletions(string id, string propertyId, string autoCompleteParm) {
            INakedObject nakedObject = NakedObjectsContext.GetNakedObjectFromId(id);
            IList<object> completions = new List<object>();
            var assoc = (IOneToOneAssociationSpec) nakedObject.Spec.Properties.Single(p => p.IsObject && p.Id == propertyId);

            if (assoc.IsAutoCompleteEnabled) {
                INakedObject[] nakedObjectCompletions = assoc.GetCompletions(nakedObject, autoCompleteParm);
                completions = nakedObjectCompletions.Select(no => GetCompletionData(no, assoc.Spec)).ToList();
            }

            return Jsonp(completions);
        }


        public virtual JsonResult GetActionCompletions(string id, string actionName, int parameterIndex, string autoCompleteParm) {
            INakedObject nakedObject = NakedObjectsContext.GetNakedObjectFromId(id);
            IActionSpec action = NakedObjectsContext.GetActions(nakedObject).SingleOrDefault(a => a.Id == actionName);
            IList<object> completions = new List<object>();

            IActionParameterSpec p = action.Parameters[parameterIndex];
            if (p.IsAutoCompleteEnabled) {
                INakedObject[] nakedObjectCompletions = p.GetCompletions(nakedObject, autoCompleteParm);
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