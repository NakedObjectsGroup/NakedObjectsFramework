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

    public static class IdConstants {

        // viewdata keys 
        public const string NofMessages = "Nof-Messages";
        public const string NofWarnings = "Nof-Warnings";
        public const string NofServices = "Services";
        public const string NofMainMenus = "NofMainMenus";
        public const string NoFramework = "NakedObjectsFramework";
        public const string SystemMessages = "Messages";
        public const string PagingData = "Nof-PagingData";
        public const string NofEncryptDecrypt = "Nof-EncryptDecrypt";
        public const string CollectionFormat = "NofCollectionFormat";

        // sub keys 
        public const string PagingCurrentPage = "Nof-PagingCurrentPage";
        public const string PagingTotal = "Nof-PagingTotal";
        public const string PagingPageSize = "Nof-PagingPageSize";

        public const string ObjectEditName = "nof-objectedit";
        public const string ObjectViewName = "nof-objectview";
        public const string ViewModelName = "nof-viewmodel";
        public const string ServicesContainerName = "nof-servicelist";
        public const string FieldContainerName = "nof-propertylist";
        public const string FieldName = "nof-property";
        public const string Label = "nof-label";
        public const string MenuContainerName = "nof-menu";
        public const string ActionContainerName = "nof-actions";
        public const string ActionName = "nof-action";
        public const string ActionNameFile = "nof-action-file";
        public const string ParamContainerName = "nof-parameterlist";
        public const string ParamName = "nof-parameter";
        public const string CollContainerName = "nof-objecttable";
        public const string ActionListName = "nof-menuitems";
        public const string SubMenuName = "nof-submenu";
        public const string SystemMenuName = "nof-system";
        public const string SubMenuItemsName = "nof-submenuitems";
        public const string CollectionTableName = "nof-collection-table";
        public const string CollectionListName = "nof-collection-list";
        public const string CollectionSummaryName = "nof-collection-summary";
        public const string ActionDialogName = "nof-actiondialog";
        public const string EditName = "nof-edit";
        public const string DialogNameClass = "nof-dialog";
        public const string DialogNameFileClass = "nof-dialog-file";
        public const string HistoryContainerName = "nof-history";
        public const string TabbedHistoryContainerName = "nof-tabbed-history";
        public const string TransientName = "nof-transient";
        public const string FindDialogName = "nof-finddialog";
        public const string StandaloneTableName = "nof-standalonetable";

        public const string NameName = "Name";
        public const string FindMenuName = "Find";
        public const string DialogName = "Dialog";
        public const string PropertyListName = "PropertyList";
        public const string ActionsName = "Actions";
        public const string ParamListName = "ParameterList";

        public const string CollItemName = "Row";
        public const string CollCellName = "Cell";
        public const string CollHeader = "Header";

        public const string ValueName = "nof-value";
        public const string ObjectName = "nof-object";
        public const string ActionNameLabel = "nof-actionname";
        public const string MenuNameLabel = "nof-menuname";

        public const string TableDisplayFormat = "table";
        public const string ListDisplayFormat = "list";
        public const string SummaryDisplayFormat = "summary";
        public const string MultilineDisplayFormat = "multiline";

        public const string Checkbox = "checkbox";
        public const string CheckboxAll = "checkboxAll";

        public const string MinDisplayFormat = "min";
        public const string MaxDisplayFormat = "max";

        public const string ActionAction = "Action";
        public const string EditObjectAction = "EditObject";
        public const string EditAction = "Edit";
        public const string ViewAction = "Details";
        public const string RemoveAction = "Remove";
        public const string GetFileAction = "GetFile";
        public const string IndexAction = "Index";

        public const string FindAction = "Finder";
        public const string ActionFindAction = "ActionAsFinder";
        public const string InvokeFindAction = "InvokeActionAsFinder";
        public const string InvokeSaveAction = "InvokeActionAsSave";
        public const string SelectAction = "Selector";
        public const string RedisplayAction = "Redisplay";
        public const string PageAction = "Pager";
        public const string ActionInvokeAction = "InvokeAction";
        public const string CancelAction = "Cancel";

        public const string DisplayFormatFieldId = "displayFormats";


        public const string ExecuteFixtureAction = "ExecuteFixture";
        public const string SwitchPerspectiveAction = "SwitchPerspective";
        public const string OptionsAction = "Options";
        public const string AboutNakedObjectsAction = "AboutNakedObjects";
        public const string ChangeLicenceAction = "ChangeLicence";
        public const string LogOutAction = "LogOut";
        public const string DebugAction = "Debug";
        public const string HomeName = "Home";
        public const string AddAction = "Add";
        public const string SaveAction = "Save";
        public const string SaveAndCloseAction = "SaveAndClose";
        public const string ClearHistoryAction = "ClearHistory";
        public const string ClearHistoryItemAction = "ClearHistoryItem";
        public const string ClearHistoryOthersAction = "ClearHistoryOthers";

        public const string ClearName = "Clear";

        // Class attributes for HTML buttons, to permit CSS styling
        public const string OkButtonClass = "nof-ok";
        public const string SaveButtonClass = "nof-save";
        public const string SaveCloseButtonClass = "nof-saveclose";
        public const string EditButtonClass = "nof-edit";
        public const string SummaryButtonClass = "nof-summary";
        public const string ListButtonClass = "nof-list";
        public const string TableButtonClass = "nof-table";
        public const string SelectButtonClass = "nof-select";
        public const string RemoveButtonClass = "nof-remove";
        public const string CancelButtonClass = "nof-cancel";
        public const string ClearButtonClass = "nof-clear";
        public const string ClearItemButtonClass = "nof-clear-item";
        public const string ClearOthersButtonClass = "nof-clear-others";
        public const string CheckboxClass = "nof-checkbox";

        public const string MinButtonClass = "nof-minimize";
        public const string MaxButtonClass = "nof-maximize";

        public const string QueryOnlyClass = "nof-queryonly";
        public const string IdempotentClass = "nof-idempotent";

        // paging
        public const string PagingClass = "nof-paging";
        public const string CollectionFormatClass = "nof-collection-formats";
        public const string PageNumberClass = "nof-page-number";
        public const string TotalCountClass = "nof-total-count";

        public const string PageKey = "page";
        public const string PageSizeKey = "pageSize";
        public const string FormatKey = "format";

        public const string ActiveClass = "active";
       

    }



    public static class IdHelper {
        private const string sep = "-";

        private const string inputName = "Input";
        private const string selectName = "Select";
        private const string concurrencyName = "Concurrency";
        private const string autoCompleteName = "AutoComplete";

        private static string InputOrSelect(ITypeSpec spec) {
            return (spec.IsParseable ? inputName : selectName);
        }

        public static string GetDisplayFormatId(string id) {
            return MakeId(id, IdConstants.DisplayFormatFieldId);
        }

        public static string GetCollectionItemId(INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetObjectId(owner) + sep + assoc.Id + sep + "Item";
        }

        public static string GetObjectId(INakedObjectAdapter owner) {
            string postFix = "";

            if (owner.Spec.IsCollection) {
                var elementFacet = owner.Spec.GetFacet<ITypeOfFacet>();
                var elementType = elementFacet.GetValue(owner);

                postFix = sep + elementType.Name;
            }

            return owner.Spec.ShortName + postFix;
        }

        public static string GetFieldId(INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetObjectId(owner) + sep + assoc.Id;
        }

        public static string GetInlineFieldId(IAssociationSpec parent, INakedObjectAdapter owner, IAssociationSpec assoc) {
            return parent.Id + sep + GetObjectId(owner) + sep + assoc.Id;
        }

        public static string GetFieldInputId(INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetFieldId(owner, assoc) + sep + InputOrSelect(assoc.ReturnSpec);
        }

        public static string GetFieldAutoCompleteId(string id, INakedObjectAdapter owner, IAssociationSpec assoc) {
            return assoc.ReturnSpec.IsParseable ? id : id + sep + autoCompleteName;
        }

        public static string GetInlineFieldInputId(IAssociationSpec parent, INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetInlineFieldId(parent, owner, assoc) + sep + InputOrSelect(assoc.ReturnSpec);
        }

        public static string GetConcurrencyFieldInputId(INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetFieldId(owner, assoc) + sep + concurrencyName;
        }

        public static string GetInlineConcurrencyFieldInputId(IAssociationSpec parent, INakedObjectAdapter owner, IAssociationSpec assoc) {
            return GetInlineFieldId(parent, owner, assoc) + sep + concurrencyName;
        }

        public static string GetConcurrencyActionInputId(INakedObjectAdapter owner, IActionSpec action, IAssociationSpec assoc) {
            return GetActionId(owner, action) + sep + assoc.Id + sep + concurrencyName;
        }

        public static string GetActionId(INakedObjectAdapter owner, IActionSpec action) {
            return GetObjectId(owner) + sep + action.Id;
        }

        internal static string GetActionId(ActionContext targetActionContext, ActionContext actionContext, string propertyName) {
            string contextActionName = actionContext.Action == null ? "" : actionContext.Action.Id + sep;
            string contextNakedObjectId = actionContext.Target == null || actionContext.Target == targetActionContext.Target ? "" : GetObjectId(actionContext.Target) + sep;
            string propertyId = string.IsNullOrEmpty(propertyName) ? "" : NameUtils.CapitalizeName(propertyName) + sep;

            return contextNakedObjectId + contextActionName + propertyId + GetObjectId(targetActionContext.Target) + sep + targetActionContext.Action.Id;
        }

        public static string GetActionDialogId(INakedObjectAdapter owner, IActionSpec action) {
            return GetObjectId(owner) + sep + action.Id + sep + IdConstants.DialogName;
        }

        private static string EnsureEndsWithColon(string id) {
            return id.EndsWith(":") ? id : id + ":";
        }

        public static string GetSubMenuId(INakedObjectAdapter owner, IActionSpec action) {
            return EnsureEndsWithColon(GetObjectId(owner) + sep + action.Id.Split('.').Last());
        }

        public static string GetSubMenuId(INakedObjectAdapter owner, INakedObjectAdapter service) {
            return EnsureEndsWithColon(GetObjectId(owner) + sep + service.Spec.ShortName);
        }

        public static string GetFindMenuId(INakedObjectAdapter nakedObject, IActionSpec action, string propertyName) {
            string contextActionName = action == null ? "" : sep + action.Id;
            return GetObjectId(nakedObject) + contextActionName + sep + NameUtils.CapitalizeName(propertyName) + sep + IdConstants.FindMenuName;
        }

        public static string GetParameterId(IActionSpec action, IActionParameterSpec parameter) {
            return action.OnSpec.ShortName + sep + action.Id + sep + NameUtils.CapitalizeName(parameter.Id);
        }

        public static string GetParameterInputId(IActionSpec action, IActionParameterSpec parameter) {
            return GetParameterId(action, parameter) + sep + InputOrSelect(parameter.Spec);
        }

        public static string GetParameterAutoCompleteId(IActionSpec action, IActionParameterSpec parameter) {
            var id = GetParameterInputId(action, parameter);
            return parameter.Spec.IsParseable ? id : id + sep + autoCompleteName;
        }

        public static string GetCollectionContainerId(INakedObjectAdapter collection) {
            return IdConstants.CollContainerName + sep + collection.Spec.ShortName;
        }

        public static string GetActionContainerId(INakedObjectAdapter nakedObject) {
            return GetObjectId(nakedObject) + sep + IdConstants.ActionsName;
        }

        public static string GetServiceContainerId(INakedObjectAdapter nakedObject) {
            return GetObjectId(nakedObject);
        }

        public static string GetFieldContainerId(INakedObjectAdapter nakedObject) {
            return GetObjectId(nakedObject) + sep + IdConstants. PropertyListName;
        }

        public static string GetParameterContainerId(IActionSpec action) {
            return action.Id + sep + IdConstants.ParamListName;
        }

        public static string GetGenericActionId(INakedObjectAdapter owner, string type) {
            return IdConstants.ActionName + sep + owner.Spec.ShortName + sep + type;
        }

        public static string GetActionLabel(INakedObjectAdapter nakedObject) {
            return MvcUi.Actions;
        }

        public static string GetServiceLabel(INakedObjectAdapter nakedObject) {
            return nakedObject.TitleString();
        }

        public static string GetMandatoryIndicatorClass() {
            return "nof-mandatory-field-indicator";
        }

        public static string GetMandatoryIndicator() {
            return "*";
        }

        public static string MakeId(params string[] ids) {
            return ids.Aggregate(string.Empty, (x, y) => (string.IsNullOrEmpty(x) ? string.Empty : x + sep) + y);
        }

        public static bool KeyPrefixIs(this string key, string match) {
            return key.StartsWith(match + sep);
        }
    }
}