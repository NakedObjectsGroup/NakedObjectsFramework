// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Resources;
using NakedObjects.Util;
using System.Linq;

namespace NakedObjects.Web.Mvc.Html {
    public static class IdHelper {
       

        private const string sep = "-";

        // viewdata keys 
        public const string NofMessages = "Nof-Messages";
        public const string NofWarnings = "Nof-Warnings";
        public const string NofServices = "Services";
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

        private const string inputName = "Input";
        private const string selectName = "Select";
        private const string concurrencyName = "Concurrency";
        private const string autoCompleteName = "AutoComplete";

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
        public const string ClearHistoryAction = "ClearHistory";
        public const string ClearHistoryItemAction = "ClearHistoryItem";
        public const string ClearHistoryOthersAction = "ClearHistoryOthers";


        public const string ClearName = "Clear";

        // Class attributes for HTML buttons, to permit CSS styling
        public const string OkButtonClass = "nof-ok";
        public const string SaveButtonClass = "nof-save";
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

        private static string InputOrSelect(INakedObjectSpecification spec) {
            return (spec.IsParseable ? inputName : selectName);
        }

        private static string UniqueShortName(this INakedObjectSpecification spec) {
            return spec.UniqueShortName(sep);
        }

        public static string GetDisplayFormatId(string id) {
            return MakeId(id, DisplayFormatFieldId);
        }

        public static string GetCollectionItemId(INakedObject owner, INakedObjectAssociation assoc) {
            return GetObjectId(owner) + sep + assoc.Id + sep + "Item";
        }

        public static string GetObjectId(INakedObject owner) {
            return owner.Specification.UniqueShortName();
        }

        public static string GetFieldId(INakedObject owner, INakedObjectAssociation assoc) {
            return GetObjectId(owner) + sep + assoc.Id;
        }

        public static string GetInlineFieldId( INakedObjectAssociation parent, INakedObject owner, INakedObjectAssociation assoc) {
            return  parent.Id + sep + GetObjectId(owner) + sep + assoc.Id;
        }

        public static string GetFieldInputId(INakedObject owner, INakedObjectAssociation assoc) {
            return GetFieldId(owner, assoc) + sep + InputOrSelect(assoc.Specification);
        }

        public static string GetFieldAutoCompleteId(string id, INakedObject owner, INakedObjectAssociation assoc) {
            return assoc.Specification.IsParseable ? id : id + sep + autoCompleteName;
        }

        public static string GetInlineFieldInputId(INakedObjectAssociation parent, INakedObject owner, INakedObjectAssociation assoc) {
            return GetInlineFieldId(parent, owner, assoc) + sep + InputOrSelect(assoc.Specification);
        }

        public static string GetConcurrencyFieldInputId(INakedObject owner, INakedObjectAssociation assoc) {
            return GetFieldId(owner, assoc) + sep + concurrencyName;
        }

        public static string GetInlineConcurrencyFieldInputId(INakedObjectAssociation parent, INakedObject owner, INakedObjectAssociation assoc) {
            return GetInlineFieldId(parent, owner, assoc) + sep + concurrencyName;
        }

        public static string GetConcurrencyActionInputId(INakedObject owner, INakedObjectAction action, INakedObjectAssociation assoc) {
            return GetActionId(owner, action) + sep + assoc.Id + sep + concurrencyName;
        }

        public static string GetActionId(INakedObject owner, INakedObjectAction action) {
            return GetObjectId(owner) + sep + action.Id;
        }

        internal static string GetActionId(ActionContext targetActionContext, ActionContext actionContext, string propertyName) {
            string contextActionName = actionContext.Action == null ? "" : actionContext.Action.Id + sep;
            string contextNakedObjectId = actionContext.Target == null || actionContext.Target == targetActionContext.Target ? "" : GetObjectId(actionContext.Target) + sep;
            string propertyId = string.IsNullOrEmpty(propertyName) ? "" : NameUtils.CapitalizeName(propertyName) + sep;

            return contextNakedObjectId + contextActionName + propertyId + GetObjectId(targetActionContext.Target) + sep + targetActionContext.Action.Id;
        }

        public static string GetActionDialogId(INakedObject owner, INakedObjectAction action) {
            return GetObjectId(owner) + sep + action.Id + sep + DialogName;
        }

        private static string EnsureEndsWithColon(string id) {
            return id.EndsWith(":") ? id : id + ":";
        }

        public static string GetSubMenuId(INakedObject owner, INakedObjectAction action) {
            return EnsureEndsWithColon(GetObjectId(owner) + sep + action.Id.Split('.').Last());
        }

        public static string GetSubMenuId(INakedObject owner, INakedObject service) {
            return EnsureEndsWithColon(GetObjectId(owner) + sep + service.Specification.UniqueShortName());
        }

        public static string GetFindMenuId(INakedObject nakedObject, INakedObjectAction action, string propertyName) {
            string contextActionName = action == null ? "" : sep + action.Id;
            return GetObjectId(nakedObject) + contextActionName + sep + NameUtils.CapitalizeName(propertyName) + sep + FindMenuName;
        }

        public static string GetParameterId(INakedObjectAction action, INakedObjectActionParameter parameter) {
            return action.OnType.UniqueShortName() + sep + action.Id + sep + NameUtils.CapitalizeName(parameter.Id);
        }

        public static string GetParameterInputId(INakedObjectAction action, INakedObjectActionParameter parameter) {
            return GetParameterId(action, parameter) + sep + InputOrSelect(parameter.Specification);
        }

        public static string GetParameterAutoCompleteId(INakedObjectAction action, INakedObjectActionParameter parameter) {
            var id = GetParameterInputId(action, parameter);
            return parameter.Specification.IsParseable ? id : id + sep + autoCompleteName;
        }

        public static string GetCollectionContainerId(INakedObject collection) {
            return CollContainerName + sep + collection.Specification.UniqueShortName();
        }

        public static string GetActionContainerId(INakedObject nakedObject) {
            return GetObjectId(nakedObject) + sep + ActionsName;
        }

        public static string GetServiceContainerId(INakedObject nakedObject) {
           
            return GetObjectId(nakedObject);
        }

        public static string GetFieldContainerId(INakedObject nakedObject) {
            return GetObjectId(nakedObject) + sep + PropertyListName;
        }

        public static string GetParameterContainerId(INakedObjectAction action) {
            return  action.Id + sep + ParamListName;
        }

        public static string GetGenericActionId(INakedObject owner, string type) {
            return ActionName + sep + owner.Specification.UniqueShortName() + sep + type;
        }

        public static string GetActionLabel(INakedObject nakedObject) {
            return MvcUi.Actions;
        }

        public static string GetServiceLabel(INakedObject nakedObject) {
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