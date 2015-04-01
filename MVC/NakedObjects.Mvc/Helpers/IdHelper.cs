// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Resources;
using NakedObjects.Util;

namespace NakedObjects.Web.Mvc.Html {
    public  class IdHelper : IIdHelper {
        private const string sep = "-";

        private const string inputName = "Input";
        private const string selectName = "Select";
        private const string concurrencyName = "Concurrency";
        private const string autoCompleteName = "AutoComplete";

        private  string InputOrSelect(ITypeSpec spec) {
            return (spec.IsParseable ? inputName : selectName);
        }

        public  string GetDisplayFormatId(string id) {
            return MakeId(id, IdConstants.DisplayFormatFieldId);
        }

        public  string GetCollectionItemId(INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetObjectId(owner) + sep + assoc.Id + sep + "Item";
        }

        public  string GetObjectId(INakedObjectAdapter owner) {
            string postFix = "";

            if (owner.Spec.IsCollection) {
                var elementFacet = owner.Spec.GetFacet<ITypeOfFacet>();
                var elementType = elementFacet.GetValue(owner);

                postFix = sep + elementType.Name;
            }

            return owner.Spec.ShortName + postFix;
        }

        public  string GetFieldId(INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetObjectId(owner) + sep + assoc.Id;
        }

        public  string GetInlineFieldId(IAssociationSpec parent, INakedObjectAdapter owner, IAssociationSpec assoc) {
            return parent.Id + sep + GetObjectId(owner) + sep + assoc.Id;
        }

        public  string GetFieldInputId(INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetFieldId(owner, assoc) + sep + InputOrSelect(assoc.ReturnSpec);
        }

        public  string GetFieldAutoCompleteId(string id, INakedObjectAdapter owner, IAssociationSpec assoc) {
            return assoc.ReturnSpec.IsParseable ? id : id + sep + autoCompleteName;
        }

        public  string GetInlineFieldInputId(IAssociationSpec parent, INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetInlineFieldId(parent, owner, assoc) + sep + InputOrSelect(assoc.ReturnSpec);
        }

        public  string GetConcurrencyFieldInputId(INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetFieldId(owner, assoc) + sep + concurrencyName;
        }

        public  string GetInlineConcurrencyFieldInputId(IAssociationSpec parent, INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetInlineFieldId(parent, owner, assoc) + sep + concurrencyName;
        }

        public  string GetConcurrencyActionInputId(INakedObjectAdapter owner, IActionSpec action, IAssociationSpec assoc) {
            return GetActionId(owner, action) + sep + assoc.Id + sep + concurrencyName;
        }

        public  string GetActionId(INakedObjectAdapter owner, IActionSpec action) {
            return GetObjectId(owner) + sep + action.Id;
        }

        public  string GetActionId(ActionContext targetActionContext, ActionContext actionContext, string propertyName) {
            string contextActionName = actionContext.Action == null ? "" : actionContext.Action.Id + sep;
            string contextNakedObjectId = actionContext.Target == null || actionContext.Target == targetActionContext.Target ? "" : GetObjectId(actionContext.Target) + sep;
            string propertyId = string.IsNullOrEmpty(propertyName) ? "" : NameUtils.CapitalizeName(propertyName) + sep;

            return contextNakedObjectId + contextActionName + propertyId + GetObjectId(targetActionContext.Target) + sep + targetActionContext.Action.Id;
        }

        public  string GetActionDialogId(INakedObjectAdapter owner, IActionSpec action) {
            return GetObjectId(owner) + sep + action.Id + sep + IdConstants.DialogName;
        }

        private  string EnsureEndsWithColon(string id) {
            return id.EndsWith(":") ? id : id + ":";
        }

        public  string GetSubMenuId(INakedObjectAdapter owner, IActionSpec action) {
            return EnsureEndsWithColon(GetObjectId(owner) + sep + action.Id.Split('.').Last());
        }

        public  string GetSubMenuId(INakedObjectAdapter owner, INakedObjectAdapter service) {
            return EnsureEndsWithColon(GetObjectId(owner) + sep + service.Spec.ShortName);
        }

        public  string GetFindMenuId(INakedObjectAdapter nakedObject, IActionSpec action, string propertyName) {
            string contextActionName = action == null ? "" : sep + action.Id;
            return GetObjectId(nakedObject) + contextActionName + sep + NameUtils.CapitalizeName(propertyName) + sep + IdConstants.FindMenuName;
        }

        public  string GetParameterId(IActionSpec action, IActionParameterSpec parameter) {
            return action.OnSpec.ShortName + sep + action.Id + sep + NameUtils.CapitalizeName(parameter.Id);
        }

        public  string GetParameterInputId(IActionSpec action, IActionParameterSpec parameter) {
            return GetParameterId(action, parameter) + sep + InputOrSelect(parameter.Spec);
        }

        public  string GetParameterAutoCompleteId(IActionSpec action, IActionParameterSpec parameter) {
            var id = GetParameterInputId(action, parameter);
            return parameter.Spec.IsParseable ? id : id + sep + autoCompleteName;
        }

        public  string GetCollectionContainerId(INakedObjectAdapter collection) {
            return IdConstants.CollContainerName + sep + collection.Spec.ShortName;
        }

        public  string GetActionContainerId(INakedObjectAdapter nakedObject) {
            return GetObjectId(nakedObject) + sep + IdConstants.ActionsName;
        }

        public  string GetServiceContainerId(INakedObjectAdapter nakedObject) {
            return GetObjectId(nakedObject);
        }

        public  string GetFieldContainerId(INakedObjectAdapter nakedObject) {
            return GetObjectId(nakedObject) + sep + IdConstants. PropertyListName;
        }

        public  string GetParameterContainerId(IActionSpec action) {
            return action.Id + sep + IdConstants.ParamListName;
        }

        public  string GetGenericActionId(INakedObjectAdapter owner, string type) {
            return IdConstants.ActionName + sep + owner.Spec.ShortName + sep + type;
        }

        public  string GetActionLabel(INakedObjectAdapter nakedObject) {
            return MvcUi.Actions;
        }

        public  string GetServiceLabel(INakedObjectAdapter nakedObject) {
            return nakedObject.TitleString();
        }

        public  string GetMandatoryIndicatorClass() {
            return "nof-mandatory-field-indicator";
        }

        public  string GetMandatoryIndicator() {
            return "*";
        }

        public  string MakeId(params string[] ids) {
            return ids.Aggregate(string.Empty, (x, y) => (string.IsNullOrEmpty(x) ? string.Empty : x + sep) + y);
        }

        public  bool KeyPrefixIs(string key, string match) {
            return key.StartsWith(match + sep);
        }
    }
}