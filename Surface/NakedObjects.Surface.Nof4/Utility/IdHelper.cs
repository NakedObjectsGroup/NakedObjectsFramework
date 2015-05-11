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
using NakedObjects.Surface.Nof4.Wrapper;
using NakedObjects.Surface.Utility;
using NakedObjects.Util;

namespace NakedObjects.Surface.Nof4.Utility {
    public class IdHelper : IIdHelper {
        private const string Sep = "-";

        private const string InputName = "Input";
        private const string SelectName = "Select";
        private const string ConcurrencyName = "Concurrency";
        private const string AutoCompleteName = "AutoComplete";

        private string InputOrSelect(ITypeSpec spec) {
            return (spec.IsParseable ? InputName : SelectName);
        }

        public string GetDisplayFormatId(string id) {
            return MakeId(id, IdConstants.DisplayFormatFieldId);
        }

        public string GetCollectionItemId(INakedObjectSurface owner, INakedObjectAssociationSurface assocSurface) {
            IAssociationSpec assoc = ((dynamic) assocSurface).WrappedSpec;
            return GetObjectId(owner) + Sep + assoc.Id + Sep + "Item";
        }

        public string GetObjectId(INakedObjectSurface ownerSurface) {
            // todo scaffolding remove later
            INakedObjectAdapter owner = ((dynamic) ownerSurface).WrappedNakedObject;
            string postFix = "";

            if (owner.Spec.IsCollection) {
                var elementFacet = owner.Spec.GetFacet<ITypeOfFacet>();
                var elementType = elementFacet.GetValue(owner);

                postFix = Sep + elementType.Name;
            }

            return owner.Spec.ShortName + postFix;
        }

        public string GetFieldId(INakedObjectSurface owner, INakedObjectAssociationSurface assocSurface) {
            IAssociationSpec assoc = ((dynamic)assocSurface).WrappedSpec;

            return GetObjectId(owner) + Sep + assoc.Id;
        }

        public string GetInlineFieldId(INakedObjectAssociationSurface parentSurface, INakedObjectSurface owner, INakedObjectAssociationSurface assocSurface) {
            IAssociationSpec parent = ((dynamic)parentSurface).WrappedSpec;
            IAssociationSpec assoc = ((dynamic)assocSurface).WrappedSpec;

            return parent.Id + Sep + GetObjectId(owner) + Sep + assoc.Id;
        }

        public string GetFieldInputId(INakedObjectSurface owner, INakedObjectAssociationSurface assocSurface) {
            IAssociationSpec assoc = ((dynamic)assocSurface).WrappedSpec;

            return GetFieldId(owner, assocSurface) + Sep + InputOrSelect(assoc.ReturnSpec);
        }

        public string GetFieldAutoCompleteId(string id, INakedObjectSurface owner, INakedObjectAssociationSurface assocSurface) {
            IAssociationSpec assoc = ((dynamic)assocSurface).WrappedSpec;

            return assoc.ReturnSpec.IsParseable ? id : id + Sep + AutoCompleteName;
        }

        public string GetInlineFieldInputId(INakedObjectAssociationSurface parent, INakedObjectSurface owner, INakedObjectAssociationSurface assocSurface) {
            IAssociationSpec assoc = ((dynamic)assocSurface).WrappedSpec;

            return GetInlineFieldId(parent, owner, assocSurface) + Sep + InputOrSelect(assoc.ReturnSpec);
        }

        public string GetConcurrencyFieldInputId(INakedObjectSurface owner, INakedObjectAssociationSurface assoc) {
            return GetFieldId(owner, assoc) + Sep + ConcurrencyName;
        }

        public string GetInlineConcurrencyFieldInputId(INakedObjectAssociationSurface parent, INakedObjectSurface owner, INakedObjectAssociationSurface assoc) {
            return GetInlineFieldId(parent, owner, assoc) + Sep + ConcurrencyName;
        }

        public string GetConcurrencyActionInputId(INakedObjectSurface owner, INakedObjectActionSurface action, INakedObjectAssociationSurface assocSurface) {
            IAssociationSpec assoc = ((dynamic)assocSurface).WrappedSpec;
            return GetActionId(owner, action) + Sep + assoc.Id + Sep + ConcurrencyName;
        }

        public string GetActionId(INakedObjectSurface owner, INakedObjectActionSurface actionSurface) {
            IActionSpec action = ((dynamic)actionSurface).WrappedSpec;
            return GetObjectId(owner) + Sep + action.Id;
        }

        public string GetActionId(string propertyName, INakedObjectActionSurface actionContextActionSurface, INakedObjectSurface actionContextTargetSurface, INakedObjectSurface targetActionContextTargetSurface, INakedObjectActionSurface targetActionContextActionSurface) {
            IActionSpec actionContextAction = actionContextActionSurface == null ? null : ((dynamic) actionContextActionSurface).WrappedSpec;
            INakedObjectAdapter actionContextTarget = actionContextTargetSurface == null ? null : ((dynamic) actionContextTargetSurface).WrappedNakedObject;
            IActionSpec targetActionContextAction = targetActionContextActionSurface == null ? null : ((dynamic)targetActionContextActionSurface).WrappedSpec;
            INakedObjectAdapter targetActionContextTarget = targetActionContextTargetSurface == null ? null : ((dynamic)targetActionContextTargetSurface).WrappedNakedObject;

            
            string contextActionName = actionContextAction == null ? "" : actionContextAction.Id + Sep;
            string contextNakedObjectId = actionContextTarget == null || actionContextTarget == targetActionContextTarget ? "" : GetObjectId(actionContextTargetSurface) + Sep;
            string propertyId = string.IsNullOrEmpty(propertyName) ? "" : NameUtils.CapitalizeName(propertyName) + Sep;
            return contextNakedObjectId + contextActionName + propertyId + GetObjectId(targetActionContextTargetSurface) + Sep + targetActionContextAction.Id;
        }

        public string GetActionDialogId(INakedObjectSurface owner, INakedObjectActionSurface actionSurface) {
            IActionSpec action = ((dynamic)actionSurface).WrappedSpec;

            return GetObjectId(owner) + Sep + action.Id + Sep + IdConstants.DialogName;
        }

        private string EnsureEndsWithColon(string id) {
            return id.EndsWith(":") ? id : id + ":";
        }

        public string GetSubMenuId(INakedObjectSurface owner, INakedObjectActionSurface actionSurface) {
            IActionSpec action = ((dynamic)actionSurface).WrappedSpec;

            return EnsureEndsWithColon(GetObjectId(owner) + Sep + action.Id.Split('.').Last());
        }

        public string GetSubMenuId(INakedObjectSurface owner, INakedObjectSurface serviceSurface) {
            INakedObjectAdapter service = ((dynamic)serviceSurface).WrappedNakedObject;
            return EnsureEndsWithColon(GetObjectId(owner) + Sep + service.Spec.ShortName);
        }

        public string GetFindMenuId(INakedObjectSurface nakedObject, INakedObjectActionSurface actionSurface, string propertyName) {
            IActionSpec action =  actionSurface == null ? null : ((dynamic)actionSurface).WrappedSpec;
            string contextActionName = action == null ? "" : Sep + action.Id;
            return GetObjectId(nakedObject) + contextActionName + Sep + NameUtils.CapitalizeName(propertyName) + Sep + IdConstants.FindMenuName;
        }

        public string GetParameterId(INakedObjectActionSurface actionSurface, INakedObjectActionParameterSurface parameterSurface) {
            IActionParameterSpec parameter = ((dynamic)parameterSurface).WrappedSpec;
            IActionSpec action = ((dynamic)actionSurface).WrappedSpec;
            return action.OnSpec.ShortName + Sep + action.Id + Sep + NameUtils.CapitalizeName(parameter.Id);
        }

        public string GetParameterInputId(INakedObjectActionSurface action, INakedObjectActionParameterSurface parameterSurface) {
            IActionParameterSpec parameter = ((dynamic)parameterSurface).WrappedSpec;
            return GetParameterId(action, parameterSurface) + Sep + InputOrSelect(parameter.Spec);
        }

        public string GetParameterAutoCompleteId(INakedObjectActionSurface action, INakedObjectActionParameterSurface parameterSurface) {
            IActionParameterSpec parameter = ((dynamic)parameterSurface).WrappedSpec;
            var id = GetParameterInputId(action, parameterSurface);
            return parameter.Spec.IsParseable ? id : id + Sep + AutoCompleteName;
        }

        public string GetCollectionContainerId(INakedObjectSurface collectionSurface) {
            INakedObjectAdapter collection = ((dynamic)collectionSurface).WrappedNakedObject;

            return IdConstants.CollContainerName + Sep + collection.Spec.ShortName;
        }

        public string GetActionContainerId(INakedObjectSurface nakedObject) {
            return GetObjectId(nakedObject) + Sep + IdConstants.ActionsName;
        }

        public string GetServiceContainerId(INakedObjectSurface nakedObject) {
            return GetObjectId(nakedObject);
        }

        public string GetFieldContainerId(INakedObjectSurface nakedObject) {
            return GetObjectId(nakedObject) + Sep + IdConstants.PropertyListName;
        }

        public string GetParameterContainerId(INakedObjectActionSurface actionSurface) {
            IActionSpec action = ((dynamic)actionSurface).WrappedSpec;

            return action.Id + Sep + IdConstants.ParamListName;
        }

        public string GetGenericActionId(INakedObjectSurface ownerSurface, string type) {
            INakedObjectAdapter owner = ((dynamic)ownerSurface).WrappedNakedObject;

            return IdConstants.ActionName + Sep + owner.Spec.ShortName + Sep + type;
        }

        public string GetActionLabel(INakedObjectSurface nakedObject) {
            return "Actions";
        }

        public string GetServiceLabel(INakedObjectSurface nakedObjectSurface) {
            INakedObjectAdapter nakedObject = ((dynamic)nakedObjectSurface).WrappedNakedObject;

            return nakedObject.TitleString();
        }

        public string GetMandatoryIndicatorClass() {
            return "nof-mandatory-field-indicator";
        }

        public string GetMandatoryIndicator() {
            return "*";
        }

        public string MakeId(params string[] ids) {
            return ids.Aggregate(string.Empty, (x, y) => (string.IsNullOrEmpty(x) ? string.Empty : x + Sep) + y);
        }

        public bool KeyPrefixIs(string key, string match) {
            return key.StartsWith(match + Sep);
        }

        public string GetAggregateFieldInputId(INakedObjectSurface nakedObjectSurface, INakedObjectAssociationSurface propertySurface) {
            string fieldId;
            INakedObjectAdapter nakedObject = ((dynamic)nakedObjectSurface).WrappedNakedObject;

            var aoid = nakedObject.Oid as IAggregateOid;
            if (aoid != null) {
                IAssociationSpec parent = ((IObjectSpec)aoid.ParentOid.Spec).Properties.SingleOrDefault(p => p.Id == aoid.FieldName);
                fieldId = GetInlineFieldInputId(new NakedObjectAssociationWrapper(parent, null, null), nakedObjectSurface, propertySurface);
            }
            else {
                fieldId = GetFieldInputId(nakedObjectSurface, propertySurface);
            }
            return fieldId;
        }

    }
}