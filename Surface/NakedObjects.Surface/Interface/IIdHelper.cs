// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Surface {
    public interface IIdHelper {
        string GetDisplayFormatId(string id);
        string GetCollectionItemId(IObjectFacade owner, INakedObjectAssociationSurface assoc);
        string GetObjectId(IObjectFacade owner);
        string GetFieldId(IObjectFacade owner, INakedObjectAssociationSurface assoc);
        string GetInlineFieldId(INakedObjectAssociationSurface parent, IObjectFacade owner, INakedObjectAssociationSurface assoc);
        string GetFieldInputId(IObjectFacade owner, INakedObjectAssociationSurface assoc);
        string GetFieldAutoCompleteId(string id, IObjectFacade owner, INakedObjectAssociationSurface assoc);
        string GetInlineFieldInputId(INakedObjectAssociationSurface parent, IObjectFacade owner, INakedObjectAssociationSurface assoc);
        string GetConcurrencyFieldInputId(IObjectFacade owner, INakedObjectAssociationSurface assoc);
        string GetInlineConcurrencyFieldInputId(INakedObjectAssociationSurface parent, IObjectFacade owner, INakedObjectAssociationSurface assoc);
        string GetConcurrencyActionInputId(IObjectFacade owner, INakedObjectActionSurface action, INakedObjectAssociationSurface assoc);
        string GetActionId(IObjectFacade owner, INakedObjectActionSurface action);
        string GetActionDialogId(IObjectFacade owner, INakedObjectActionSurface action);
        string GetSubMenuId(IObjectFacade owner, INakedObjectActionSurface action);
        string GetSubMenuId(IObjectFacade owner, IObjectFacade service);
        string GetFindMenuId(IObjectFacade nakedObject, INakedObjectActionSurface action, string propertyName);
        string GetParameterId(INakedObjectActionSurface action, INakedObjectActionParameterSurface parameter);
        string GetParameterInputId(INakedObjectActionSurface action, INakedObjectActionParameterSurface parameter);
        string GetParameterAutoCompleteId(INakedObjectActionSurface action, INakedObjectActionParameterSurface parameter);
        string GetCollectionContainerId(IObjectFacade collection);
        string GetActionContainerId(IObjectFacade nakedObject);
        string GetServiceContainerId(IObjectFacade nakedObject);
        string GetFieldContainerId(IObjectFacade nakedObject);
        string GetParameterContainerId(INakedObjectActionSurface action);
        string GetGenericActionId(IObjectFacade owner, string type);
        string GetActionLabel(IObjectFacade nakedObject);
        string GetServiceLabel(IObjectFacade nakedObject);
        string GetMandatoryIndicatorClass();
        string GetMandatoryIndicator();
        string MakeId(params string[] ids);
        bool KeyPrefixIs(string key, string match);
        string GetActionId(string propertyName, INakedObjectActionSurface actionContextAction, IObjectFacade actionContextTarget, IObjectFacade targetActionContextTarget, INakedObjectActionSurface targetActionContextAction);
        string GetAggregateFieldInputId(IObjectFacade nakedObjectSurface, INakedObjectAssociationSurface propertySurface);
    }
}