// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Facade {
    public interface IIdHelper {
        string GetDisplayFormatId(string id);
        string GetCollectionItemId(IObjectFacade owner, IAssociationFacade assoc);
        string GetObjectId(IObjectFacade objectFacade);
        string GetFieldId(IObjectFacade owner, IAssociationFacade assoc);
        string GetInlineFieldId(IAssociationFacade parent, IObjectFacade owner, IAssociationFacade assoc);
        string GetFieldInputId(IObjectFacade owner, IAssociationFacade assoc);
        string GetFieldAutoCompleteId(string id, IObjectFacade owner, IAssociationFacade assoc);
        string GetInlineFieldInputId(IAssociationFacade parent, IObjectFacade owner, IAssociationFacade assoc);
        string GetConcurrencyFieldInputId(IObjectFacade owner, IAssociationFacade assoc);
        string GetInlineConcurrencyFieldInputId(IAssociationFacade parent, IObjectFacade owner, IAssociationFacade assoc);
        string GetConcurrencyActionInputId(IObjectFacade owner, IActionFacade action, IAssociationFacade assoc);
        string GetActionId(IObjectFacade owner, IActionFacade action);
        string GetActionDialogId(IObjectFacade owner, IActionFacade action);
        string GetSubMenuId(IObjectFacade owner, IActionFacade action);
        string GetSubMenuId(IObjectFacade owner, IObjectFacade serviceFacade);
        string GetFindMenuId(IObjectFacade objectFacade, IActionFacade action, string propertyName);
        string GetParameterId(IActionFacade action, IActionParameterFacade parameter);
        string GetParameterInputId(IActionFacade action, IActionParameterFacade parameter);
        string GetParameterAutoCompleteId(IActionFacade action, IActionParameterFacade parameter);
        string GetCollectionContainerId(IObjectFacade collectionFacade);
        string GetActionContainerId(IObjectFacade objectFacade);
        string GetServiceContainerId(IObjectFacade objectFacade);
        string GetFieldContainerId(IObjectFacade objectFacade);
        string GetParameterContainerId(IActionFacade action);
        string GetGenericActionId(IObjectFacade objectFacade, string type);
        string GetActionLabel(IObjectFacade objectFacade);
        string GetServiceLabel(IObjectFacade objectFacade);
        string GetMandatoryIndicatorClass();
        string GetMandatoryIndicator();
        string MakeId(params string[] ids);
        bool KeyPrefixIs(string key, string match);
        string GetActionId(string propertyName, IActionFacade actionContextAction, IObjectFacade actionObjectFacade, IObjectFacade targetObjectFacade, IActionFacade targetActionFacade);
        string GetAggregateFieldInputId(IObjectFacade objectFacade, IAssociationFacade propertyFacade);
    }
}