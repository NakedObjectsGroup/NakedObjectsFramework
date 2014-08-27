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
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter.Map;
using NakedObjects.Core.Persist;
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
            INakedObject nakedObject = FrameworkHelper.GetNakedObjectFromId(id);

            if (nakedObject.ResolveState.IsTransient() && !MvcIdentityAdapterHashMap.StoringTransientsInSession) {
                // if transient and not saving transients in session then we cannot validate now - need to wait until save 
                return Jsonp(true);
            }

            INakedObjectAssociation property = nakedObject.Specification.Properties.SingleOrDefault(p => p.Id == propertyName);
            string fieldId = GetFieldInputId(nakedObject, property);

            bool isValid = false;

            if (value == null) {
                value = Request.Params[fieldId];
            }

            if (property != null && property is IOneToOneAssociation) {
                ValidateAssociation(nakedObject, property as IOneToOneAssociation, value);
                isValid = ModelState.IsValid;
            }

            if (isValid) {
                return Jsonp(true);
            }

            ModelError error = ModelState[fieldId].Errors.FirstOrDefault();
            return Jsonp(error == null ? "" : error.ErrorMessage);
        }

        private static string GetFieldInputId(INakedObject nakedObject, INakedObjectAssociation property) {
            string fieldId;

            if (nakedObject.Oid is AggregateOid) {
                var aoid = ((AggregateOid) nakedObject.Oid);
                INakedObjectAssociation parent = aoid.ParentOid.Specification.Properties.Where(p => p.Id == aoid.FieldName).SingleOrDefault();
                fieldId = IdHelper.GetInlineFieldInputId(parent, nakedObject, property);
            }
            else {
                fieldId = IdHelper.GetFieldInputId(nakedObject, property);
            }
            return fieldId;
        }

        public virtual JsonResult ValidateParameter(string id, string value, string actionName, string parameterName) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObjectFromId(id);
            INakedObjectAction action = FrameworkHelper.GetActions(nakedObject).SingleOrDefault(a => a.Id == actionName);
            bool isValid = false;
            string parmId = "";

            if (action != null) {
                INakedObjectActionParameter parameter = action.Parameters.Where(p => p.Id.Equals(parameterName, StringComparison.InvariantCultureIgnoreCase)).Single();
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

        private static INakedObject GetValue(string[] values, INakedObjectSpecification spec) {
            if (!values.Any()) {
                return null;
            }
         
            if (spec.IsParseable) {
                return spec.GetFacet<IParseableFacet>().ParseTextEntry(values.First(), Core.Context.NakedObjectsContext.ObjectPersistor);
            }
            if (spec.IsCollection) {
                return FrameworkHelper.GetTypedCollection(spec, values);
            }

            return FrameworkHelper.GetNakedObjectFromId(values.First());
        }

        private string[] GetRawValues(FormCollection parms, string id) {
            var values = new List<string>();

            for (int i = 0; parms.AllKeys.Contains(id + i); i++) {
                values.Add(parms[id + i]);
            }

            return values.ToArray();
        }


        private IDictionary<string, INakedObject> GetOtherValues(INakedObjectAction action) {
            var results = new Dictionary<string, INakedObject>();
            var parms = new FormCollection(HttpContext.Request.Params);

            Decrypt(parms);

            foreach (INakedObjectActionParameter parm in action.Parameters) {
                string[] values =  GetRawValues(parms, IdHelper.GetParameterInputId(action, parm));
                results[parm.Id.ToLower()] = GetValue(values, parm.Specification);
            }

            return results;
        }

        private IDictionary<string, INakedObject> GetOtherValues(INakedObject nakedObject) {
            var results = new Dictionary<string, INakedObject>();
            var parms = new FormCollection(HttpContext.Request.Params);

            Decrypt(parms);

            foreach (IOneToOneAssociation assoc in nakedObject.Specification.Properties.Where(a => a.IsObject)) {
                string[] values = GetRawValues(parms, GetFieldInputId(nakedObject, assoc));
                results[assoc.Id.ToLower()] = GetValue(values, assoc.Specification);
            }

            return results;
        }


        public virtual JsonResult GetActionChoices(string id, string actionName) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObjectFromId(id);
            INakedObjectAction action = FrameworkHelper.GetActions(nakedObject).SingleOrDefault(a => a.Id == actionName);
            IDictionary<string, string[][]> choices = new Dictionary<string, string[][]>();
            IDictionary<string, INakedObject> otherValues = GetOtherValues(action);

            foreach (INakedObjectActionParameter p in action.Parameters) {
                if (p.IsChoicesEnabled || p.IsMultipleChoicesEnabled) {
                    INakedObject[] nakedObjectChoices = p.GetChoices(nakedObject, otherValues, Core.Context.NakedObjectsContext.ObjectPersistor);
                    string[] content = nakedObjectChoices.Select(c => c.TitleString()).ToArray();
                    string[] value = FrameworkHelper.IsParseableOrCollectionOfParseable(p.Specification) ? content : nakedObjectChoices.Select(FrameworkHelper.GetObjectId).ToArray();

                    choices[IdHelper.GetParameterInputId(action, p)] = new[] {value, content};
                }
            }
            return Jsonp(choices);
        }

     


        public virtual JsonResult GetPropertyChoices(string id) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObjectFromId(id);
            IDictionary<string, string[][]> choices = new Dictionary<string, string[][]>();
            IDictionary<string, INakedObject> otherValues = GetOtherValues(nakedObject);

            foreach (IOneToOneAssociation assoc in nakedObject.Specification.Properties.Where(p => p.IsObject)) {
                if (assoc.IsChoicesEnabled) {
                    INakedObject[] nakedObjectChoices = assoc.GetChoices(nakedObject, otherValues, Core.Context.NakedObjectsContext.ObjectPersistor);
                    string[] content = nakedObjectChoices.Select(c => c.TitleString()).ToArray();
                    string[] value = assoc.Specification.IsParseable ? content : nakedObjectChoices.Select(FrameworkHelper.GetObjectId).ToArray();

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
            return nakedObject.Specification.SingularName;
        }

        private object GetCompletionData(INakedObject nakedObject, INakedObjectSpecification spec) {
            string label = nakedObject.TitleString();
            string value = nakedObject.TitleString();
            string link = spec.IsParseable ? label : FrameworkHelper.GetObjectId(nakedObject);
            string src = GetIconSrc(nakedObject);
            string alt = GetIconAlt(nakedObject);
            return new {label, value, link, src, alt};
        }


        public virtual JsonResult GetPropertyCompletions(string id, string propertyId, string autoCompleteParm) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObjectFromId(id);
            IList<object> completions = new List<object>();
            var assoc = (IOneToOneAssociation) nakedObject.Specification.Properties.Single(p => p.IsObject && p.Id == propertyId);

            if (assoc.IsAutoCompleteEnabled) {
                INakedObject[] nakedObjectCompletions = assoc.GetCompletions(nakedObject, autoCompleteParm, Core.Context.NakedObjectsContext.ObjectPersistor);
                completions = nakedObjectCompletions.Select(no => GetCompletionData(no, assoc.Specification)).ToList();
            }

            return Jsonp(completions);
        }


        public virtual JsonResult GetActionCompletions(string id, string actionName, int parameterIndex, string autoCompleteParm) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObjectFromId(id);
            INakedObjectAction action = FrameworkHelper.GetActions(nakedObject).SingleOrDefault(a => a.Id == actionName);
            IList<object> completions = new List<object>();

            INakedObjectActionParameter p = action.Parameters[parameterIndex];
            if (p.IsAutoCompleteEnabled) {
                INakedObject[] nakedObjectCompletions = p.GetCompletions(nakedObject, autoCompleteParm, Core.Context.NakedObjectsContext.ObjectPersistor);
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