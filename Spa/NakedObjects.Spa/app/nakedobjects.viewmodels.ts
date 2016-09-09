import * as Msg from "./user-messages.config";
import * as Models from "./models";
import * as Config from "./nakedobjects.config";
import { PaneRouteData, CollectionViewState, ViewType, InteractionMode } from "./nakedobjects.routedata";
import * as Ro from "./nakedobjects.rointerfaces";
import { Context } from "./context.service";
import { Color } from "./color.service";
import { UrlManager } from "./urlmanager.service";
import * as _ from "lodash";
import { Error } from "./error.service";
import { ViewModelFactory } from "./view-model-factory.service";
import { FocusManager } from "./focus-manager.service";
import { ILocalFilter } from "./mask.service";
import { Subject } from 'rxjs/Subject';

export interface IAttachmentViewModel {
    href: string;
    mimeType: string;
    title: string;
    link: Models.Link;
    onPaneId: number;

    downloadFile: () => Promise<Blob>;
    clearCachedFile: () => void;
    displayInline: () => boolean;
    doClick: (right?: boolean) => void;
}

export interface IChoiceViewModel {
    name: string;

    getValue: () => Models.Value;
    equals: (other: IChoiceViewModel) => boolean;
    valuesEqual: (other: IChoiceViewModel) => boolean;
}

export interface IDraggableViewModel {
    value: Ro.scalarValueType | Date;
    reference: string;
    selectedChoice: IChoiceViewModel;
    color: string;
    draggableType: string;

    canDropOn: (targetType: string) => Promise<boolean>;
}

export interface IErrorViewModel {
    originalError: Models.ErrorWrapper;
    title: string;
    message: string;
    stackTrace: string[];
    errorCode: string;
    description: string;
    isConcurrencyError: boolean;
}

export interface ILinkViewModel {
    title: string;
    domainType: string;
    link: Models.Link;

    doClick: (right?: boolean) => void;
}

export interface IItemViewModel extends ILinkViewModel {
    tableRowViewModel: ITableRowViewModel;
    selected: boolean;

    selectionChange: (index: number) => void;
}

export interface IRecentItemViewModel extends ILinkViewModel {
    friendlyName: string;
}

export interface IMessageViewModel {
    clearMessage: () => void;
    resetMessage: () => void;
    setMessage: (msg: string) => void;
    getMessage: () => string;
}

export interface IFieldViewModel extends IMessageViewModel {
    id: string;
    argId: string;
    paneArgId: string;
    onPaneId: number;

    optional: boolean;
    description: string;
    presentationHint: string;
    mask: string;
    title: string;
    returnType: string;
    format: Ro.formatType;
    multipleLines: number;
    password: boolean;
    clientValid: boolean;

    type: "scalar" | "ref";
    reference: string;
    minLength: number;

    color: string;

    isCollectionContributed: boolean;

    currentValue: Models.Value;
    originalValue: Models.Value;

    localFilter: ILocalFilter;
    formattedValue: string;

    choices: IChoiceViewModel[];

    value: Ro.scalarValueType | Date;

    selectedChoice: IChoiceViewModel;
    selectedMultiChoices: IChoiceViewModel[];

    entryType: Models.EntryType;

    validate: (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;

    refresh: (newValue: Models.Value) => void;

    prompt: (searchTerm: string) => Promise<ChoiceViewModel[]>;
    promptArguments: _.Dictionary<Models.Value>;

    conditionalChoices: (args: _.Dictionary<Models.Value>) => Promise<ChoiceViewModel[]>;

    setNewValue: (newValue: IDraggableViewModel) => void;

    drop: (newValue: IDraggableViewModel) => void;

    clear: () => void;

    getValue: () => Models.Value;
}

export interface IParameterViewModel extends IFieldViewModel {
    parameterRep: Models.Parameter;
    dflt: string;
}

export interface IPropertyViewModel extends IFieldViewModel {
    propertyRep: Models.PropertyMember;
    isEditable: boolean;
    attachment: IAttachmentViewModel;
    refType: "null" | "navigable" | "notNavigable";
    isDirty: () => boolean;
    doClick: (right?: boolean) => void;
}

export interface IActionViewModel {
    actionRep: Models.ActionMember | Models.ActionRepresentation;
    invokableActionRep: Models.IInvokableAction;

    menuPath: string;
    title: string;
    description: string;
    presentationHint: string;

    // doInvoke is called from template 
    doInvoke: (right?: boolean) => void;
    execute: (pps: IParameterViewModel[], right?: boolean) => Promise<Models.ActionResultRepresentation>;
    disabled: () => boolean;
    parameters: () => IParameterViewModel[];
    makeInvokable: (details: Models.IInvokableAction) => void;
}

export interface IMenuItemViewModel {
    name: string;
    actions: IActionViewModel[];
    menuItems: IMenuItemViewModel[];
}

export interface IDialogViewModel extends IMessageViewModel {
    actionViewModel: IActionViewModel;
    title: string;
    id: string;
    parameters: IParameterViewModel[];

    reset: (actionViewModel: IActionViewModel, routeData: PaneRouteData) => void;
    refresh: () => void;
    deregister: () => void;
    clientValid: () => boolean;
    tooltip: () => void;
    setParms: () => void;
    doInvoke: (right?: boolean) => void;
    doCloseKeepHistory: () => void;
    doCloseReplaceHistory: () => void;
    clearMessages: () => void;
}

export interface ICollectionPlaceholderViewModel {
    description: () => string;
    reload: () => void;
}

export interface IListViewModel extends IMessageViewModel {
    id: string;
    listRep: Models.ListRepresentation;
    size: number;
    pluralName: string;
    header: string[];
    items: IItemViewModel[];
    actions: IActionViewModel[];
    menuItems: IMenuItemViewModel[];

    description: () => string;
    refresh: (routeData: PaneRouteData) => void;
    reset: (list: Models.ListRepresentation, routeData: PaneRouteData) => void;
    toggleActionMenu: () => void;
    pageNext: () => void;
    pagePrevious: () => void;
    pageFirst: () => void;
    pageLast: () => void;
    doSummary: () => void;
    doList: () => void;
    doTable: () => void;
    reload: () => void;
    selectAll: () => void;
    disableActions: () => void;
    actionsTooltip: () => string;
    actionMember: (id: string) => Models.ActionMember | Models.ActionRepresentation;
}

export interface ICollectionViewModel {
    title: string;
    details: string;
    pluralName: string;
    color: string;
    mayHaveItems: boolean;
    items: IItemViewModel[];
    header: string[];
    onPaneId: number;
    currentState: CollectionViewState;
    presentationHint: string;
    template: string;
    actions: IActionViewModel[];
    menuItems: IMenuItemViewModel[];
    messages: string;
    collectionRep: Models.CollectionMember | Models.CollectionRepresentation;

    doSummary: () => void;
    doTable: () => void;
    doList: () => void;

    description: () => string;
    refresh: (routeData: PaneRouteData, resetting: boolean) => void;
}

export interface IMenusViewModel {
    reset: (menusRep: Models.MenusRepresentation, routeData: PaneRouteData) => IMenusViewModel;
    menusRep: Models.MenusRepresentation;
    onPaneId: number;
    items: ILinkViewModel[];
}

export interface IMenuViewModel extends IMessageViewModel {
    id: string;
    title: string;
    actions: IActionViewModel[];
    menuItems: IMenuItemViewModel[];
    menuRep: Models.MenuRepresentation;
}

export interface ITableRowColumnViewModel {
    type: "ref" | "scalar";
    returnType: string;
    value: Ro.scalarValueType | Date;
    formattedValue: string;
    title: string;
}

export interface ITableRowViewModel {
    title: string;
    hasTitle: boolean;
    properties: ITableRowColumnViewModel[];
}

export interface IApplicationPropertiesViewModel {
    serverVersion: Ro.IVersionRepresentation;
    user: Ro.IUserRepresentation;
    serverUrl: string;
    clientVersion: string;
}

export interface IToolBarViewModel {
    loading: string;
    template: string;
    footerTemplate: string;
    goHome: (right?: boolean) => void;
    goBack: () => void;
    goForward: () => void;
    swapPanes: () => void;
    logOff: () => void;
    singlePane: (right?: boolean) => void;
    recent: (right?: boolean) => void;
    cicero: () => void;
    userName: string;
    applicationProperties: () => void;

    warnings: string[];
    messages: string[];
}

export interface IRecentItemsViewModel {
    onPaneId: number;
    items: IRecentItemViewModel[];
}

export interface IDomainObjectViewModel extends IMessageViewModel {
    domainObject: Models.DomainObjectRepresentation;
    onPaneId: number;

    title: string;
    friendlyName: string;
    presentationHint: string;
    domainType: string;

    isInEdit: boolean;

    actions: IActionViewModel[];
    menuItems: IMenuItemViewModel[];
    properties: IPropertyViewModel[];
    collections: ICollectionViewModel[];

    refresh: (routeData: PaneRouteData) => void;
    reset: (obj: Models.DomainObjectRepresentation, routeData: PaneRouteData) => IDomainObjectViewModel;
    concurrency: () => (event: any, em: Models.ErrorMap) => void; // event: ng.IAngularEvent
    clientValid: () => boolean;
    tooltip: () => string;
    actionsTooltip: () => string;
    toggleActionMenu: () => void;
    setProperties: () => void;
    doEditCancel: () => void;
    clearCachedFiles: () => void;
    doSave: (viewObject: boolean) => void;
    doSaveValidate: () => Promise<boolean>;
    doEdit: () => void;
    doReload: () => void;
    hideEdit: () => boolean;
    disableActions: () => boolean;
}

export interface ICiceroViewModel {
    message: string;
    output: string;
    alert: string;
    input: string;
    parseInput: (input: string) => void;
    previousInput: string;
    chainedCommands: string[];

    selectPreviousInput: () => void;

    clearInput: () => void;

    autoComplete: (input: string) => void;

    outputMessageThenClearIt: () => void;

    renderHome: (routeData: PaneRouteData) => void;
    renderObject: (routeData: PaneRouteData) => void;
    renderList: (routeData: PaneRouteData) => void;
    renderError: () => void;
    viewType: ViewType;
    clipboard: Models.DomainObjectRepresentation;

    executeNextChainedCommandIfAny: () => void;

    popNextCommand: () => string;

    clearInputRenderOutputAndAppendAlertIfAny: (output: string) => void;
}


function tooltip(onWhat: { clientValid: () => boolean }, fields: IFieldViewModel[]): string {
    if (onWhat.clientValid()) {
        return "";
    }

    const missingMandatoryFields = _.filter(fields, p => !p.clientValid && !p.getMessage());

    if (missingMandatoryFields.length > 0) {
        return _.reduce(missingMandatoryFields, (s, t) => s + t.title + "; ", Msg.mandatoryFieldsPrefix);
    }

    const invalidFields = _.filter(fields, p => !p.clientValid);

    if (invalidFields.length > 0) {
        return _.reduce(invalidFields, (s, t) => s + t.title + "; ", Msg.invalidFieldsPrefix);
    }

    return "";
}

function actionsTooltip(onWhat: { disableActions: () => boolean }, actionsOpen: boolean) {
    if (actionsOpen) {
        return Msg.closeActions;
    }
    return onWhat.disableActions() ? Msg.noActions : Msg.openActions;
}

export function toTriStateBoolean(valueToSet: string | boolean | number) {

    // looks stupid but note type checking
    if (valueToSet === true || valueToSet === "true") {
        return true;
    }
    if (valueToSet === false || valueToSet === "false") {
        return false;
    }
    return null;
}

function getMenuForLevel(menupath: string, level: number) {
    let menu = "";

    if (menupath && menupath.length > 0) {
        const menus = menupath.split("_");

        if (menus.length > level) {
            menu = menus[level];
        }
    }

    return menu || "";
}

function removeDuplicateMenus(menus: IMenuItemViewModel[]) {
    return _.uniqWith(menus, (m1: IMenuItemViewModel, m2: IMenuItemViewModel) => {
        if (m1.name && m2.name) {
            return m1.name === m2.name;
        }
        return false;
    });
}

export function createSubmenuItems(avms: IActionViewModel[], menu: IMenuItemViewModel, level: number) {
    // if not root menu aggregate all actions with same name
    if (menu.name) {
        const actions = _.filter(avms, a => getMenuForLevel(a.menuPath, level) === menu.name && !getMenuForLevel(a.menuPath, level + 1));
        menu.actions = actions;

        //then collate submenus 

        const submenuActions = _.filter(avms, (a: IActionViewModel) => getMenuForLevel(a.menuPath, level) === menu.name && getMenuForLevel(a.menuPath, level + 1));
        let menus = _
            .chain(submenuActions)
            .map(a => new MenuItemViewModel(getMenuForLevel(a.menuPath, level + 1), null, null))
            .value();

        menus = removeDuplicateMenus(menus);

        menu.menuItems = _.map(menus, m => createSubmenuItems(submenuActions, m, level + 1));
    }
    return menu;
}

export function createMenuItems(avms: IActionViewModel[]) {

    // first create a top level menu for each action 
    // note at top level we leave 'un-menued' actions
    let menus = _
        .chain(avms)
        .map(a => new MenuItemViewModel(getMenuForLevel(a.menuPath, 0), [a], null))
        .value();

    // remove non unique submenus 
    menus = removeDuplicateMenus(menus);

    // update submenus with all actions under same submenu
    return _.map(menus, m => createSubmenuItems(avms, m, 0));
}

export class AttachmentViewModel implements IAttachmentViewModel {
    href: string;
    mimeType: string;
    title: string;
    link: Models.Link;
    onPaneId: number;

    private parent: Models.DomainObjectRepresentation;
    private context: Context;

    static create(attachmentLink: Models.Link, parent: Models.DomainObjectRepresentation, context: Context, paneId: number) {
        const attachmentViewModel = new AttachmentViewModel();
        attachmentViewModel.link = attachmentLink;
        attachmentViewModel.href = attachmentLink.href();
        attachmentViewModel.mimeType = attachmentLink.type().asString;
        attachmentViewModel.title = attachmentLink.title() || Msg.unknownFileTitle;
        attachmentViewModel.parent = parent;
        attachmentViewModel.context = context;
        attachmentViewModel.onPaneId = paneId;
        return attachmentViewModel as IAttachmentViewModel;
    }

    downloadFile = () => this.context.getFile(this.parent, this.href, this.mimeType);
    clearCachedFile = () => this.context.clearCachedFile(this.href);

    displayInline = () =>
        this.mimeType === "image/jpeg" ||
        this.mimeType === "image/gif" ||
        this.mimeType === "application/octet-stream";

    doClick: (right?: boolean) => void;
}

export class ChoiceViewModel implements IChoiceViewModel {
    name: string;

    private id: string;
    private search: string;
    private isEnum: boolean;
    private wrapped: Models.Value;

    static create(value: Models.Value, id: string, name?: string, searchTerm?: string) {
        const choiceViewModel = new ChoiceViewModel();
        choiceViewModel.wrapped = value;
        choiceViewModel.id = id;
        choiceViewModel.name = name || value.toString();
        choiceViewModel.search = searchTerm || choiceViewModel.name;

        choiceViewModel.isEnum = !value.isReference() && (choiceViewModel.name !== choiceViewModel.getValue().toValueString());
        return choiceViewModel as IChoiceViewModel;
    }

    getValue() {
        return this.wrapped;
    }

    equals(other: IChoiceViewModel): boolean {
        return other instanceof ChoiceViewModel &&
            this.id === other.id &&
            this.name === other.name &&
            this.wrapped.toValueString() === other.wrapped.toValueString();
    }

    valuesEqual(other: IChoiceViewModel): boolean {

        if (other instanceof ChoiceViewModel) {
            const thisValue = this.isEnum ? this.wrapped.toValueString().trim() : this.search.trim();
            const otherValue = this.isEnum ? other.wrapped.toValueString().trim() : other.search.trim();
            return thisValue === otherValue;
        }
        return false;
    }

    toString() {
        return this.name;
    }
}

export class ErrorViewModel implements IErrorViewModel {
    originalError: Models.ErrorWrapper;
    title: string;
    message: string;
    stackTrace: string[];
    errorCode: string;
    description: string;
    isConcurrencyError: boolean;
}

export class LinkViewModel implements ILinkViewModel, IDraggableViewModel {
    // ILinkViewModel
    title: string;
    domainType: string;
    link: Models.Link;

    doClick: (right?: boolean) => void;

    // IDraggableViewModel 
    color: string;
    value: Ro.scalarValueType;
    reference: string;
    selectedChoice: IChoiceViewModel;
    draggableType: string;
    canDropOn: (targetType: string) => Promise<boolean>;
}

export class ItemViewModel extends LinkViewModel implements IItemViewModel, IDraggableViewModel {
    tableRowViewModel: ITableRowViewModel;
    selected: boolean;
    selectionChange: (index: number) => void;
}

export class RecentItemViewModel extends LinkViewModel implements IRecentItemViewModel, IDraggableViewModel {
    friendlyName: string;
}

abstract class MessageViewModel implements IMessageViewModel {
    private previousMessage = "";
    private message = "";

    clearMessage = () => {
        if (this.message === this.previousMessage) {
            this.resetMessage();
        } else {
            this.previousMessage = this.message;
        }
    }

    resetMessage = () => this.message = this.previousMessage = "";
    setMessage = (msg: string) => this.message = msg;
    getMessage = () => this.message;
}

abstract class ValueViewModel extends MessageViewModel implements IFieldViewModel {

    constructor(ext: Models.Extensions, color: Color, error: Error) {
        super();
        this.optional = ext.optional();
        this.description = ext.description();
        this.presentationHint = ext.presentationHint();
        this.mask = ext.mask();
        this.title = ext.friendlyName();
        this.returnType = ext.returnType();
        this.format = ext.format();
        this.multipleLines = ext.multipleLines() || 1;
        this.password = ext.dataType() === "password";
        this.updateColor = _.partial(this.setColor, color);
        this.error = error;
    }

    id: string;
    argId: string;
    paneArgId: string;
    onPaneId: number;

    optional: boolean;
    description: string;
    presentationHint: string;
    mask: string;
    title: string;
    returnType: string;
    format: Ro.formatType;
    multipleLines: number;
    password: boolean;

    clientValid = true;

    type: "scalar" | "ref";
    reference: string = "";
    minLength: number;

    color: string;

    isCollectionContributed: boolean;

    promptArguments: _.Dictionary<Models.Value>;

    currentValue: Models.Value;
    originalValue: Models.Value;

    localFilter: ILocalFilter;
    formattedValue: string;

    choices: IChoiceViewModel[] = [];

    private currentChoice: IChoiceViewModel;

    private error: Error;

    get selectedChoice(): IChoiceViewModel {
        return this.currentChoice;
    }

    set selectedChoice(newChoice: IChoiceViewModel) {
        // type guard because angular pushes string value here until directive finds 
        // choice
        if (newChoice instanceof ChoiceViewModel || newChoice == null) {
            this.currentChoice = newChoice;
            this.updateColor();
        }
    }

    private currentRawValue: Ro.scalarValueType | Date;

    get value(): Ro.scalarValueType | Date {
        return this.currentRawValue;
    }

    set value(newValue: Ro.scalarValueType | Date) {
        this.currentRawValue = newValue;
        this.updateColor();
    }

    selectedMultiChoices: IChoiceViewModel[];

    private file: Models.Link;

    entryType: Models.EntryType;

    validate: (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;

    refresh: (newValue: Models.Value) => void;

    prompt: (searchTerm: string) => Promise<ChoiceViewModel[]>;

    conditionalChoices: (args: _.Dictionary<Models.Value>) => Promise<ChoiceViewModel[]>;

    setNewValue(newValue: IDraggableViewModel) {
        this.selectedChoice = newValue.selectedChoice;
        this.value = newValue.value;
        this.reference = newValue.reference;
    }

    drop: (newValue: IDraggableViewModel) => void;

    clear() {
        this.selectedChoice = null;
        this.value = null;
        this.reference = "";
    }

    private updateColor: () => void;

    private setColor(color: Color) {

        if (this.entryType === Models.EntryType.AutoComplete && this.selectedChoice && this.type === "ref") {
            const href = this.selectedChoice.getValue().href();
            if (href) {
                color.toColorNumberFromHref(href).
                    then(c => this.color = `${Config.linkColor}${c}`).
                    catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
                return;
            }
        }
        else if (this.entryType !== Models.EntryType.AutoComplete && this.value) {
            color.toColorNumberFromType(this.returnType).
                then(c => this.color = `${Config.linkColor}${c}`).
                catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
            return;
        }

        this.color = "";
    }

    getValue(): Models.Value {

        if (this.entryType === Models.EntryType.File) {
            return new Models.Value(this.file);
        }

        if (this.entryType !== Models.EntryType.FreeForm || this.isCollectionContributed) {

            if (this.entryType === Models.EntryType.MultipleChoices || this.entryType === Models.EntryType.MultipleConditionalChoices || this.isCollectionContributed) {
                const selections = this.selectedMultiChoices || [];
                if (this.type === "scalar") {
                    const selValues = _.map(selections, cvm => cvm.getValue().scalar());
                    return new Models.Value(selValues);
                }
                const selRefs = _.map(selections, cvm => ({ href: cvm.getValue().href(), title: cvm.name })); // reference 
                return new Models.Value(selRefs);
            }

            const choiceValue = this.selectedChoice ? this.selectedChoice.getValue() : null;
            if (this.type === "scalar") {
                return new Models.Value(choiceValue && choiceValue.scalar() != null ? choiceValue.scalar() : "");
            }

            // reference 
            return new Models.Value(choiceValue && choiceValue.isReference() ? { href: choiceValue.href(), title: this.selectedChoice.name } : null);
        }

        if (this.type === "scalar") {
            if (this.value == null) {
                return new Models.Value("");
            }

            if (this.value instanceof Date) {

                if (this.format === "time") {
                    // time format
                    return new Models.Value(Models.toTimeString(this.value as Date));
                }

                if (this.format === "date") {
                    // truncate time;
                    return new Models.Value(Models.toDateString(this.value as Date));
                }
                // date-time
                return new Models.Value((this.value as Date).toISOString());
            }

            return new Models.Value(this.value as Ro.scalarValueType);
        }

        // reference
        return new Models.Value(this.reference ? { href: this.reference, title: this.value.toString() } : null);
    }
}

export class ParameterViewModel extends ValueViewModel {

    constructor(parmRep: Models.Parameter, paneId: number, color: Color, error: Error) {
        super(parmRep.extensions(), color, error);
        this.parameterRep = parmRep;
        this.onPaneId = paneId;
        this.type = parmRep.isScalar() ? "scalar" : "ref";
        this.dflt = parmRep.default().toString();
        this.id = parmRep.id();
        this.argId = `${this.id.toLowerCase()}`;
        this.paneArgId = `${this.argId}${this.onPaneId}`;
        this.isCollectionContributed = parmRep.isCollectionContributed();
        this.entryType = parmRep.entryType();
        this.value = null;
    }

    parameterRep: Models.Parameter;
    dflt: string;
}

export class ActionViewModel implements IActionViewModel {
    actionRep: Models.ActionMember | Models.ActionRepresentation;
    invokableActionRep: Models.IInvokableAction;

    menuPath: string;
    title: string;
    description: string;
    presentationHint: string;

    doInvoke: (right?: boolean) => void;
    execute: (pps: IParameterViewModel[], right?: boolean) => Promise<Models.ActionResultRepresentation>;
    disabled: () => boolean;
    parameters: () => IParameterViewModel[];
    makeInvokable: (details: Models.IInvokableAction) => void;
}

export class MenuItemViewModel implements IMenuItemViewModel {
    constructor(public name: string,
        public actions: IActionViewModel[],
        public menuItems: IMenuItemViewModel[]) { }

    toggleCollapsed() {
        this.navCollapsed = !this.navCollapsed;
    }

    navCollapsed = !!this.name;

}

export class DialogViewModel extends MessageViewModel implements IDialogViewModel {
    constructor(private color: Color,
        private context: Context,
        private viewModelFactory: ViewModelFactory,
        private urlManager: UrlManager,
        private focusManager: FocusManager,
        private error: Error) {
        super();
    }

    private onPaneId: number;
    private isQueryOnly: boolean;

    private actionMember = () => this.actionViewModel.actionRep;

    private execute = (right?: boolean) => {

        const pps = this.parameters;
        this.context.updateValues();
        return this.actionViewModel.execute(pps, right);
    };

    actionViewModel: IActionViewModel;
    title: string;
    id: string;
    parameters: IParameterViewModel[];

    reset(actionViewModel: IActionViewModel, routeData: PaneRouteData) {
        this.actionViewModel = actionViewModel;
        this.onPaneId = routeData.paneId;

        const fields = this.context.getCurrentDialogValues(this.actionMember().actionId(), this.onPaneId);

        const parameters = _.pickBy(actionViewModel.invokableActionRep.parameters(), p => !p.isCollectionContributed()) as _.Dictionary<Models.Parameter>;
        this.parameters = _.map(parameters, p => this.viewModelFactory.parameterViewModel(p, fields[p.id()], this.onPaneId));

        this.title = this.actionMember().extensions().friendlyName();
        this.isQueryOnly = actionViewModel.invokableActionRep.invokeLink().method() === "GET";
        this.resetMessage();
        this.id = actionViewModel.actionRep.actionId();
    }

    refresh() {
        const fields = this.context.getCurrentDialogValues(this.actionMember().actionId(), this.onPaneId);
        _.forEach(this.parameters, p => p.refresh(fields[p.id]));
    }

    deregister: () => void;

    clientValid = () => _.every(this.parameters, p => p.clientValid);

    tooltip = () => tooltip(this, this.parameters);

    setParms = () => _.forEach(this.parameters, p => this.context.setFieldValue(this.actionMember().actionId(), p.parameterRep.id(), p.getValue(), this.onPaneId));


    doInvoke = (right?: boolean) =>
        this.execute(right).
            then((actionResult: Models.ActionResultRepresentation) => {
                if (actionResult.shouldExpectResult()) {
                    this.setMessage(actionResult.warningsOrMessages() || Msg.noResultMessage);
                } else if (actionResult.resultType() === "void") {
                    // dialog staying on same page so treat as cancel 
                    // for url replacing purposes
                    this.doCloseReplaceHistory();
                }
                else if (!this.isQueryOnly) {
                    // not query only - always close
                    //this.doCloseReplaceHistory();
                }
                else if (!right) {
                    // query only going to new page close dialog and keep history
                    //this.doCloseKeepHistory();
                }
                // else query only going to other tab leave dialog open
            }).
            catch((reject: Models.ErrorWrapper) => {
                const display = (em: Models.ErrorMap) => this.viewModelFactory.handleErrorResponse(em, this, this.parameters);
                this.error.handleErrorAndDisplayMessages(reject, display);
            });

    doCloseKeepHistory = () => {
        //this.deregister();
        this.urlManager.closeDialogKeepHistory(this.onPaneId);
    }

    doCloseReplaceHistory = () => {
        //this.deregister();
        this.urlManager.closeDialogReplaceHistory(this.onPaneId);
    }

    clearMessages = () => {
        this.resetMessage();
        _.each(this.actionViewModel.parameters, parm => parm.clearMessage());
    };

}

export class PropertyViewModel extends ValueViewModel implements IPropertyViewModel, IDraggableViewModel {

    constructor(propertyRep: Models.PropertyMember, color: Color, error: Error) {
        super(propertyRep.extensions(), color, error);
        this.draggableType = propertyRep.extensions().returnType();

        this.propertyRep = propertyRep;
        this.entryType = propertyRep.entryType();
        this.isEditable = !propertyRep.disabledReason();
        this.entryType = propertyRep.entryType();
    }


    propertyRep: Models.PropertyMember;
    isEditable: boolean;
    attachment: IAttachmentViewModel;
    refType: "null" | "navigable" | "notNavigable";
    isDirty: () => boolean;
    doClick: (right?: boolean) => void;

    // IDraggableViewModel
    draggableType: string;
    canDropOn: (targetType: string) => Promise<boolean>;
}

export class CollectionPlaceholderViewModel implements ICollectionPlaceholderViewModel {
    description: () => string;
    reload: () => void;
}

export class ListViewModel extends MessageViewModel implements IListViewModel {

    constructor(private colorService: Color,
                private context: Context,
                private viewModelFactory: ViewModelFactory,
                private urlManager: UrlManager,
                private focusManager: FocusManager,
                private error: Error) {
        super();
    }

    private routeData: PaneRouteData;
    private onPaneId: number;
    private page: number;
    private pageSize: number;
    private numPages: number;
    private state: CollectionViewState;
    private allSelected: boolean;

    id: string;
    listRep: Models.ListRepresentation;
    size: number;
    pluralName: string;
    header: string[];
    items: IItemViewModel[];
    actions: IActionViewModel[];
    menuItems: IMenuItemViewModel[];
    description: () => string;

    private recreate = (page: number, pageSize: number) => {
        return this.routeData.objectId ?
            this.context.getListFromObject(this.routeData.paneId, this.routeData, page, pageSize) :
            this.context.getListFromMenu(this.routeData.paneId, this.routeData, page, pageSize);
    };

    private pageOrRecreate = (newPage: number, newPageSize: number, newState?: CollectionViewState) => {
        this.recreate(newPage, newPageSize).
            then((list: Models.ListRepresentation) => {
                this.urlManager.setListPaging(newPage, newPageSize, newState || this.routeData.state, this.onPaneId);
                this.routeData = this.urlManager.getRouteData().pane()[this.onPaneId];
                this.reset(list, this.routeData);
            }).
            catch((reject: Models.ErrorWrapper) => {
                const display = (em: Models.ErrorMap) => this.setMessage(em.invalidReason() || em.warningMessage);
                this.error.handleErrorAndDisplayMessages(reject, display);
            });
    };

    private setPage = (newPage: number, newState: CollectionViewState) => {
        this.context.updateValues();
        this.focusManager.focusOverrideOff();
        this.pageOrRecreate(newPage, this.pageSize, newState);
    };

    private earlierDisabled = () => this.page === 1 || this.numPages === 1;

    private laterDisabled = () => this.page === this.numPages || this.numPages === 1;

    private pageFirstDisabled = this.earlierDisabled;

    private pageLastDisabled = this.laterDisabled;

    private pageNextDisabled = this.laterDisabled;

    private pagePreviousDisabled = this.earlierDisabled;

    private updateItems(value: Models.Link[]) {
        this.items = this.viewModelFactory.getItems(value,
            this.state === CollectionViewState.Table,
            this.routeData,
            this);

        const totalCount = this.listRep.pagination().totalCount;
        this.allSelected = _.every(this.items, item => item.selected);
        const count = this.items.length;
        this.size = count;
        if (count > 0) {
            this.description = () => Msg.pageMessage(this.page, this.numPages, count, totalCount);
        } else {
            this.description = () => Msg.noItemsFound;
        }
    }

    hasTableData = () =>  this.listRep.hasTableData();
    
    private collectionContributedActionDecorator(actionViewModel: IActionViewModel) {
        const wrappedInvoke = actionViewModel.execute;
        actionViewModel.execute = (pps: IParameterViewModel[], right?: boolean) => {
            const selected = _.filter(this.items, i => i.selected);

            if (selected.length === 0) {

                const em = new Models.ErrorMap({}, 0, Msg.noItemsSelected);
                const rp = new Models.ErrorWrapper(Models.ErrorCategory.HttpClientError, Models.HttpStatusCode.UnprocessableEntity, em);

                return <any>Promise.reject(rp);
            }

            const getParms = (action: Models.IInvokableAction) => {

                const parms = _.values(action.parameters()) as Models.Parameter[];
                const contribParm = _.find(parms, p => p.isCollectionContributed());
                const parmValue = new Models.Value(_.map(selected, i => i.link));
                const collectionParmVm = this.viewModelFactory.parameterViewModel(contribParm, parmValue, this.onPaneId);

                const allpps = _.clone(pps);
                allpps.push(collectionParmVm);
                return allpps;
            }

            if (actionViewModel.invokableActionRep) {
                return wrappedInvoke(getParms(actionViewModel.invokableActionRep), right);
            }

            return this.context.getActionDetails(actionViewModel.actionRep as Models.ActionMember)
                .then((details: Models.ActionRepresentation) => wrappedInvoke(getParms(details), right));
        }
    }

    private collectionContributedInvokeDecorator(actionViewModel: IActionViewModel) {

        const showDialog = () =>
            this.context.getInvokableAction(actionViewModel.actionRep as Models.ActionMember).
                then(invokableAction => _.keys(invokableAction.parameters()).length > 1);

        // make sure not null while waiting for promise to assign correct function 
        actionViewModel.doInvoke = () => { };

        const invokeWithDialog = (right?: boolean) => {
            this.context.clearDialogValues(this.onPaneId);
            this.focusManager.focusOverrideOff();
            this.urlManager.setDialog(actionViewModel.actionRep.actionId(), this.onPaneId);
        };

        const invokeWithoutDialog = (right?: boolean) =>
            actionViewModel.execute([], right).
                then(result => this.setMessage(result.shouldExpectResult() ? result.warningsOrMessages() || Msg.noResultMessage : "")).
                catch((reject: Models.ErrorWrapper) => {
                    const display = (em: Models.ErrorMap) => this.setMessage(em.invalidReason() || em.warningMessage);
                    this.error.handleErrorAndDisplayMessages(reject, display);
                });

        showDialog().
            then(show => actionViewModel.doInvoke = show ? invokeWithDialog : invokeWithoutDialog).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
    }

    private decorate(actionViewModel: IActionViewModel) {
        this.collectionContributedActionDecorator(actionViewModel);
        this.collectionContributedInvokeDecorator(actionViewModel);
    }

    refresh(routeData: PaneRouteData) {

        this.routeData = routeData;
        if (this.state !== routeData.state) {
            if (routeData.state === CollectionViewState.Table && !this.hasTableData()) {
                this.recreate(this.page, this.pageSize).
                    then(list => {
                        this.state = list.hasTableData() ? CollectionViewState.Table : CollectionViewState.List;
                        this.listRep = list;
                        this.updateItems(list.value());
                    }).
                    catch((reject: Models.ErrorWrapper) => {
                        this.error.handleError(reject);
                    });
            } else {
                this.updateItems(this.listRep.value());
            }
        }
    }

    reset(list: Models.ListRepresentation, routeData: PaneRouteData) {
        this.listRep = list;
        this.routeData = routeData;

        this.id = this.urlManager.getListCacheIndex(routeData.paneId, routeData.page, routeData.pageSize);

        this.onPaneId = routeData.paneId;

        this.pluralName = "Objects";
        this.page = this.listRep.pagination().page;
        this.pageSize = this.listRep.pagination().pageSize;
        this.numPages = this.listRep.pagination().numPages;

        this.state = this.listRep.hasTableData() ? CollectionViewState.Table : CollectionViewState.List;
        this.updateItems(list.value());

        const actions = this.listRep.actionMembers();
        this.actions = _.map(actions, action => this.viewModelFactory.actionViewModel(action, this, routeData));
        this.menuItems = createMenuItems(this.actions);

        _.forEach(this.actions, a => this.decorate(a));
    }

    toggleActionMenu = () => {
        this.focusManager.focusOverrideOff();
        this.urlManager.toggleObjectMenu(this.onPaneId);
    };

    pageNext = () => this.setPage(this.page < this.numPages ? this.page + 1 : this.page, this.state);
    pagePrevious = () => this.setPage(this.page > 1 ? this.page - 1 : this.page, this.state);
    pageFirst = () => this.setPage(1, this.state);
    pageLast = () => this.setPage(this.numPages, this.state);

    doSummary = () => {
        this.context.updateValues();
        this.urlManager.setListState(CollectionViewState.Summary, this.onPaneId);
    };

    doList = () => {
        this.context.updateValues();
        this.urlManager.setListState(CollectionViewState.List, this.onPaneId);
    };

    doTable = () => {
        this.context.updateValues();
        this.urlManager.setListState(CollectionViewState.Table, this.onPaneId);
    };

    reload = () => {
        this.context.clearCachedList(this.onPaneId, this.routeData.page, this.routeData.pageSize);
        this.setPage(this.page, this.state);
    };

    selectAll = () => _.each(this.items, (item, i) => {
        item.selected = this.allSelected;
        item.selectionChange(i);
    });

    disableActions = () => !this.actions || this.actions.length === 0 || !this.items || this.items.length === 0;

    actionsTooltip = () => actionsTooltip(this, !!this.routeData.actionsOpen);

    actionMember = (id: string) => {
        const actionViewModel = _.find(this.actions, a => a.actionRep.actionId() === id);
        return actionViewModel.actionRep;
    }

    showActions() {
        return !!this.urlManager.getRouteData().pane()[this.onPaneId].actionsOpen;
    }
}

export class CollectionViewModel implements ICollectionViewModel {

    title: string;
    details: string;
    pluralName: string;
    color: string;
    mayHaveItems: boolean;
    items: IItemViewModel[];
    header: string[];
    onPaneId: number;
    currentState: CollectionViewState;
    requestedState: CollectionViewState;
    presentationHint: string;
    template: string;
    actions: IActionViewModel[];
    menuItems: IMenuItemViewModel[];
    messages: string;
    collectionRep: Models.CollectionMember | Models.CollectionRepresentation;

    doSummary: () => void;
    doTable: () => void;
    doList: () => void;
    hasTableData: () => boolean;

    description = () => this.details.toString();
    refresh: (routeData: PaneRouteData, resetting: boolean) => void;
}

export class MenusViewModel implements IMenusViewModel {
    constructor(private viewModelFactory: ViewModelFactory) { }

    reset(menusRep: Models.MenusRepresentation, routeData: PaneRouteData) {
        this.menusRep = menusRep;
        this.onPaneId = routeData.paneId;
        this.items = _.map(this.menusRep.value(), link => this.viewModelFactory.linkViewModel(link, this.onPaneId));
        return this;
    }

    menusRep: Models.MenusRepresentation;
    onPaneId: number;
    items: ILinkViewModel[];
}

export class MenuViewModel extends MessageViewModel implements IMenuViewModel {
    id: string;
    title: string;
    actions: IActionViewModel[];
    menuItems: IMenuItemViewModel[];
    menuRep: Models.MenuRepresentation;
}

export class TableRowColumnViewModel implements ITableRowColumnViewModel {
    type: "ref" | "scalar";
    returnType: string;
    value: Ro.scalarValueType | Date;
    formattedValue: string;
    title: string;
}

export class TableRowViewModel implements ITableRowViewModel {
    title: string;
    hasTitle: boolean;
    properties: ITableRowColumnViewModel[];
}

export class ApplicationPropertiesViewModel implements IApplicationPropertiesViewModel {
    serverVersion: Ro.IVersionRepresentation;
    user: Ro.IUserRepresentation;
    serverUrl: string;
    clientVersion: string;
}

export class ToolBarViewModel implements IToolBarViewModel {
    loading: string;
    template: string;
    footerTemplate: string;
    goHome: (right?: boolean) => void;
    goBack: () => void;
    goForward: () => void;
    swapPanes: () => void;
    logOff: () => void;
    singlePane: (right?: boolean) => void;
    recent: (right?: boolean) => void;
    cicero: () => void;
    userName: string;
    applicationProperties: () => void;

    warnings: string[];
    messages: string[];
}

export class RecentItemsViewModel implements IRecentItemsViewModel {
    onPaneId: number;
    items: IRecentItemViewModel[];
}

export class DomainObjectViewModel extends MessageViewModel implements IDomainObjectViewModel, IDraggableViewModel {

    constructor(private colorService: Color,
        private contextService: Context,
        private viewModelFactory: ViewModelFactory,
        private urlManager: UrlManager,
        private focusManager: FocusManager,
        private error: Error) {
        super();
    }

    private routeData: PaneRouteData;
    private props: _.Dictionary<Models.Value>;
    private instanceId: string;
    private unsaved: boolean;

    // IDraggableViewModel
    value: string;
    reference: string;
    selectedChoice: IChoiceViewModel;
    color: string;
    draggableType: string;

    domainObject: Models.DomainObjectRepresentation;
    onPaneId: number;

    title: string;
    friendlyName: string;
    presentationHint: string;
    domainType: string;

    isInEdit: boolean;

    actions: IActionViewModel[];
    menuItems: IMenuItemViewModel[];
    properties: IPropertyViewModel[];
    collections: ICollectionViewModel[];

    private editProperties = () => _.filter(this.properties, p => p.isEditable && p.isDirty());

    private isFormOrTransient = () => this.domainObject.extensions().interactionMode() === "form" || this.domainObject.extensions().interactionMode() === "transient";

    private cancelHandler = () => this.isFormOrTransient() ?
        () => this.urlManager.popUrlState(this.onPaneId) :
        () => this.urlManager.setInteractionMode(InteractionMode.View, this.onPaneId);

    private saveHandler = () => this.domainObject.isTransient() ? this.contextService.saveObject : this.contextService.updateObject;

    private validateHandler = () => this.domainObject.isTransient() ? this.contextService.validateSaveObject : this.contextService.validateUpdateObject;

    private handleWrappedError = (reject: Models.ErrorWrapper) => {
        const display = (em: Models.ErrorMap) => this.viewModelFactory.handleErrorResponse(em, this, this.properties);
        this.error.handleErrorAndDisplayMessages(reject, display);
    };

    private propertyMap = () => {
        const pps = _.filter(this.properties, property => property.isEditable);
        return _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Models.Value>;
    };

    private wrapAction(a: IActionViewModel) {
        const wrappedInvoke = a.execute;
        a.execute = (pps: IParameterViewModel[], right?: boolean) => {
            this.setProperties();
            const pairs = _.map(this.editProperties(), p => [p.id, p.getValue()]);
            const prps = (<any>_.fromPairs)(pairs) as _.Dictionary<Models.Value>;

            const parmValueMap = _.mapValues(a.invokableActionRep.parameters(), p => ({ parm: p, value: prps[p.id()] }));
            const allpps = _.map(parmValueMap, o => this.viewModelFactory.parameterViewModel(o.parm, o.value, this.onPaneId));
            return wrappedInvoke(allpps, right).
                catch((reject: Models.ErrorWrapper) => {
                    this.handleWrappedError(reject);
                    return Promise.reject(reject);
                });
        };
    }

    private editComplete = () => {
        this.contextService.updateValues();
        this.contextService.clearObjectUpdater(this.onPaneId);
    };


    // must be careful with this - OK for changes on client but after server updates should use  reset
    // because parameters may have appeared or disappeared etc and refesh just updates existing views. 
    // So OK for view state changes but not eg for a parameter that disappears after saving

    refresh(routeData: PaneRouteData) {

        this.routeData = routeData;
        const iMode = this.domainObject.extensions().interactionMode();
        this.isInEdit = routeData.interactionMode !== InteractionMode.View || iMode === "form" || iMode === "transient";
        this.props = routeData.interactionMode !== InteractionMode.View ? this.contextService.getCurrentObjectValues(this.domainObject.id(), routeData.paneId) : {};

        _.forEach(this.properties, p => p.refresh(this.props[p.id]));
        _.forEach(this.collections, c => c.refresh(this.routeData, false));

        this.unsaved = routeData.interactionMode === InteractionMode.Transient;

        this.title = this.unsaved ? `Unsaved ${this.domainObject.extensions().friendlyName()}` : this.domainObject.title();

        this.title = this.title + Models.dirtyMarker(this.contextService, this.domainObject.getOid());

        if (routeData.interactionMode === InteractionMode.Form) {
            _.forEach(this.actions, a => this.wrapAction(a));
        }

        // leave message from previous refresh 
        this.clearMessage();
    }

    reset(obj: Models.DomainObjectRepresentation, routeData: PaneRouteData): IDomainObjectViewModel {
        this.domainObject = obj;
        this.onPaneId = routeData.paneId;
        this.routeData = routeData;
        const iMode = this.domainObject.extensions().interactionMode();
        this.isInEdit = routeData.interactionMode !== InteractionMode.View || iMode === "form" || iMode === "transient";
        this.props = routeData.interactionMode !== InteractionMode.View ? this.contextService.getCurrentObjectValues(this.domainObject.id(), routeData.paneId) : {};

        const actions = _.values(this.domainObject.actionMembers()) as Models.ActionMember[];
        this.actions = _.map(actions, action => this.viewModelFactory.actionViewModel(action, this, this.routeData));

        this.menuItems = createMenuItems(this.actions);

        this.properties = _.map(this.domainObject.propertyMembers(), (property, id) => this.viewModelFactory.propertyViewModel(property, id, this.props[id], this.onPaneId, this.propertyMap));
        this.collections = _.map(this.domainObject.collectionMembers(), collection => this.viewModelFactory.collectionViewModel(collection, this.routeData));

        this.unsaved = routeData.interactionMode === InteractionMode.Transient;

        this.title = this.unsaved ? `Unsaved ${this.domainObject.extensions().friendlyName()}` : this.domainObject.title();

        this.title = this.title + Models.dirtyMarker(this.contextService, obj.getOid());

        this.friendlyName = this.domainObject.extensions().friendlyName();
        this.presentationHint = this.domainObject.extensions().presentationHint();
        this.domainType = this.domainObject.domainType();
        this.instanceId = this.domainObject.instanceId();
        this.draggableType = this.domainObject.domainType();

        const selfAsValue = () => {
            const link = this.domainObject.selfLink();
            if (link) {
                // not transient - can't drag transients so no need to set up IDraggable members on transients
                link.setTitle(this.title);
                return new Models.Value(link);
            }
            return null;
        };
        const sav = selfAsValue();

        this.value = sav ? sav.toString() : "";
        this.reference = sav ? sav.toValueString() : "";
        this.selectedChoice = sav ? ChoiceViewModel.create(sav, "") : null;

        this.colorService.toColorNumberFromType(this.domainObject.domainType()).
            then(c => this.color = `${Config.objectColor}${c}`).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        this.resetMessage();

        if (routeData.interactionMode === InteractionMode.Form) {
            _.forEach(this.actions, a => this.wrapAction(a));
        }


        return this as IDomainObjectViewModel;
    }

    concurrency() {
        return (event: any, em: Models.ErrorMap) => {
            this.routeData = this.urlManager.getRouteData().pane()[this.onPaneId];
            this.contextService.getObject(this.onPaneId, this.domainObject.getOid(), this.routeData.interactionMode).
                then(obj => {
                    // cleared cached values so all values are from reloaded representation 
                    this.contextService.clearObjectValues(this.onPaneId);
                    return this.contextService.reloadObject(this.onPaneId, obj);
                }).
                then(reloadedObj => {
                    if (this.routeData.dialogId) {
                        this.urlManager.closeDialogReplaceHistory(this.onPaneId);
                    }
                    this.reset(reloadedObj, this.routeData);
                    this.viewModelFactory.handleErrorResponse(em, this, this.properties);
                }).
                catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
        }
    }

    clientValid = () => _.every(this.properties, p => p.clientValid);

    tooltip = () => tooltip(this, this.properties);

    actionsTooltip = () => actionsTooltip(this, !!this.routeData.actionsOpen);

    toggleActionMenu = () => {
        this.focusManager.focusOverrideOff();
        this.contextService.updateValues();
        this.urlManager.toggleObjectMenu(this.onPaneId);
    };

    setProperties = () => _.forEach(this.editProperties(),
        p => this.contextService.setPropertyValue(this.domainObject, p.propertyRep, p.getValue(), this.onPaneId));

    doEditCancel = () => {
        this.editComplete();
        this.contextService.clearObjectValues(this.onPaneId);
        this.cancelHandler()();
    };

    clearCachedFiles = () => _.forEach(this.properties, p => p.attachment ? p.attachment.clearCachedFile() : null);

    doSave = (viewObject: boolean) => {
        this.clearCachedFiles();
        this.contextService.updateValues();
        const propMap = this.propertyMap();
        this.contextService.clearObjectUpdater(this.onPaneId);
        this.saveHandler()(this.domainObject, propMap, this.onPaneId, viewObject).
            then(obj => this.reset(obj, this.urlManager.getRouteData().pane()[this.onPaneId])).
            catch((reject: Models.ErrorWrapper) => this.handleWrappedError(reject));
    };

    doSaveValidate = () => {
        const propMap = this.propertyMap();

        return this.validateHandler()(this.domainObject, propMap).
            then(() => {
                this.resetMessage();
                return true;
            }).
            catch((reject: Models.ErrorWrapper) => {
                this.handleWrappedError(reject);
                return Promise.reject(false);
            });
    };

    doEdit = () => {
        this.contextService.updateValues(); // for other panes
        this.clearCachedFiles();
        this.contextService.clearObjectValues(this.onPaneId);
        this.contextService.getObjectForEdit(this.onPaneId, this.domainObject).
            then(updatedObject => {
                this.reset(updatedObject, this.urlManager.getRouteData().pane()[this.onPaneId]);
                this.urlManager.pushUrlState(this.onPaneId);
                this.urlManager.setInteractionMode(InteractionMode.Edit, this.onPaneId);
            }).
            catch((reject: Models.ErrorWrapper) => this.handleWrappedError(reject));
    };

    doReload = () => {
        this.contextService.updateValues();
        this.clearCachedFiles();
        this.contextService.reloadObject(this.onPaneId, this.domainObject)
            .then(updatedObject => this.reset(updatedObject, this.urlManager.getRouteData().pane()[this.onPaneId]))
            .catch((reject: Models.ErrorWrapper) => this.handleWrappedError(reject));
    }

    hideEdit = () => this.isFormOrTransient() || _.every(this.properties, p => !p.isEditable);

    disableActions = () => !this.actions || this.actions.length === 0;

    canDropOn = (targetType: string) => this.contextService.isSubTypeOf(this.domainType, targetType);



    showActions() {
        return !!this.urlManager.getRouteData().pane()[this.onPaneId].actionsOpen;
    }

    propertyChanged() {
        this.propertyChangedSource.next(true);
        this.propertyChangedSource.next(false);
    }

    private propertyChangedSource = new Subject<boolean>();

    propertyChanged$ = this.propertyChangedSource.asObservable();


}



export class CiceroViewModel implements ICiceroViewModel {
    message: string;
    output: string;
    alert = ""; //Alert is appended before the output
    input: string;
    parseInput: (input: string) => void;
    previousInput: string;
    chainedCommands: string[];

    selectPreviousInput = () => {
        this.input = this.previousInput;
    }

    clearInput = () => {
        this.input = null;
    }

    autoComplete: (input: string) => void;

    outputMessageThenClearIt() {
        this.output = this.message;
        this.message = null;
    }

    renderHome: (routeData: PaneRouteData) => void;
    renderObject: (routeData: PaneRouteData) => void;
    renderList: (routeData: PaneRouteData) => void;
    renderError: () => void;
    viewType: ViewType;
    clipboard: Models.DomainObjectRepresentation;

    executeNextChainedCommandIfAny: () => void;

    popNextCommand(): string {
        if (this.chainedCommands) {
            const next = this.chainedCommands[0];
            this.chainedCommands.splice(0, 1);
            return next;

        }
        return null;
    }

    clearInputRenderOutputAndAppendAlertIfAny(output: string): void {
        this.clearInput();
        this.output = output;
        if (this.alert) {
            this.output += this.alert;
            this.alert = "";
        }
    }
}
