// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Surface.Utility {
    public interface IIdHelper {
        string GetDisplayFormatId(string id);
        string GetCollectionItemId(INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetObjectId(INakedObjectSurface owner);
        string GetFieldId(INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetInlineFieldId(INakedObjectAssociationSurface parent, INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetFieldInputId(INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetFieldAutoCompleteId(string id, INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetInlineFieldInputId(INakedObjectAssociationSurface parent, INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetConcurrencyFieldInputId(INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetInlineConcurrencyFieldInputId(INakedObjectAssociationSurface parent, INakedObjectSurface owner, INakedObjectAssociationSurface assoc);
        string GetConcurrencyActionInputId(INakedObjectSurface owner, INakedObjectActionSurface action, INakedObjectAssociationSurface assoc);
        string GetActionId(INakedObjectSurface owner, INakedObjectActionSurface action);
        string GetActionDialogId(INakedObjectSurface owner, INakedObjectActionSurface action);
        string GetSubMenuId(INakedObjectSurface owner, INakedObjectActionSurface action);
        string GetSubMenuId(INakedObjectSurface owner, INakedObjectSurface service);
        string GetFindMenuId(INakedObjectSurface nakedObject, INakedObjectActionSurface action, string propertyName);
        string GetParameterId(INakedObjectActionSurface action, INakedObjectActionParameterSurface parameter);
        string GetParameterInputId(INakedObjectActionSurface action, INakedObjectActionParameterSurface parameter);
        string GetParameterAutoCompleteId(INakedObjectActionSurface action, INakedObjectActionParameterSurface parameter);
        string GetCollectionContainerId(INakedObjectSurface collection);
        string GetActionContainerId(INakedObjectSurface nakedObject);
        string GetServiceContainerId(INakedObjectSurface nakedObject);
        string GetFieldContainerId(INakedObjectSurface nakedObject);
        string GetParameterContainerId(INakedObjectActionSurface action);
        string GetGenericActionId(INakedObjectSurface owner, string type);
        string GetActionLabel(INakedObjectSurface nakedObject);
        string GetServiceLabel(INakedObjectSurface nakedObject);
        string GetMandatoryIndicatorClass();
        string GetMandatoryIndicator();
        string MakeId(params string[] ids);
        bool KeyPrefixIs(string key, string match);
        string GetActionId(string propertyName, INakedObjectActionSurface actionContextAction, INakedObjectSurface actionContextTarget, INakedObjectSurface targetActionContextTarget, INakedObjectActionSurface targetActionContextAction);
        string GetAggregateFieldInputId(INakedObjectSurface nakedObjectSurface, INakedObjectAssociationSurface propertySurface);
    }
}