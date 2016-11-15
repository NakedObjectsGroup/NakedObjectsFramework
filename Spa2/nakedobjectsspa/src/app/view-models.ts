import * as Msg from "./user-messages";
import * as Models from "./models";
import * as Config from "./config";
import { PaneRouteData, CollectionViewState, ViewType, InteractionMode } from "./route-data";
import * as Ro from "./ro-interfaces";
import { ContextService } from "./context.service";
import { ColorService } from "./color.service";
import { UrlManagerService } from "./url-manager.service";
import * as _ from "lodash";
import { ErrorService } from "./error.service";
import { ViewModelFactoryService } from "./view-model-factory.service";
import { ILocalFilter } from "./mask.service";
import { Subject } from 'rxjs/Subject';
import { ISubscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { AbstractControl } from '@angular/forms';
import { ChoiceViewModel } from './view-models/choice-view-model';
import { AttachmentViewModel } from './view-models/attachment-view-model';
import { IDraggableViewModel } from './view-models/idraggable-view-model';
import { IMessageViewModel } from './view-models/imessage-view-model';
import { RecentItemViewModel } from './view-models/recent-item-view-model';
import { LinkViewModel } from './view-models/link-view-model';
import { ItemViewModel } from './view-models/item-view-model';
import * as Idraggableviewmodel from './view-models/idraggable-view-model';
import { MessageViewModel } from './view-models/message-view-model';
import * as Fieldviewmodel from './view-models/field-view-model';
import * as Parameterviewmodel from './view-models/parameter-view-model';

function tooltip(onWhat: { clientValid: () => boolean }, fields: Fieldviewmodel.FieldViewModel[]): string {
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

function removeDuplicateMenus(menus: MenuItemViewModel[]) {
    return _.uniqWith(menus, (m1: MenuItemViewModel, m2: MenuItemViewModel) => {
        if (m1.name && m2.name) {
            return m1.name === m2.name;
        }
        return false;
    });
}

export function createSubmenuItems(avms: ActionViewModel[], menu: MenuItemViewModel, level: number) {
    // if not root menu aggregate all actions with same name
    if (menu.name) {
        const actions = _.filter(avms, a => getMenuForLevel(a.menuPath, level) === menu.name && !getMenuForLevel(a.menuPath, level + 1));
        menu.actions = actions;

        //then collate submenus 

        const submenuActions = _.filter(avms, (a: ActionViewModel) => getMenuForLevel(a.menuPath, level) === menu.name && getMenuForLevel(a.menuPath, level + 1));
        let menus = _
            .chain(submenuActions)
            .map(a => new MenuItemViewModel(getMenuForLevel(a.menuPath, level + 1), null, null))
            .value();

        menus = removeDuplicateMenus(menus);

        menu.menuItems = _.map(menus, m => createSubmenuItems(submenuActions, m, level + 1));
    }
    return menu;
}

export function createMenuItems(avms: ActionViewModel[]) {

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





export class ActionViewModel  {
    actionRep: Models.ActionMember | Models.ActionRepresentation;
    invokableActionRep: Models.IInvokableAction;

    menuPath: string;
    title: string;
    description: string;
    presentationHint: string;

    doInvoke: (right?: boolean) => void;
    execute: (pps: Parameterviewmodel.ParameterViewModel[], right?: boolean) => Promise<Models.ActionResultRepresentation>;
    disabled: () => boolean;
    parameters: () => Parameterviewmodel.ParameterViewModel[];
    makeInvokable: (details: Models.IInvokableAction) => void;
}

export class MenuItemViewModel  {
    constructor(public name: string,
        public actions: ActionViewModel[],
        public menuItems: MenuItemViewModel[]) { }

    toggleCollapsed() {
        this.navCollapsed = !this.navCollapsed;
    }

    navCollapsed = !!this.name;

}

export class DialogViewModel extends MessageViewModel  {
    constructor(private color: ColorService,
        private context: ContextService,
        private viewModelFactory: ViewModelFactoryService,
        private urlManager: UrlManagerService,
        private error: ErrorService) {
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

    actionViewModel: ActionViewModel;
    title: string;
    id: string;
    parameters: Parameterviewmodel.ParameterViewModel[];

    reset(actionViewModel: ActionViewModel, routeData: PaneRouteData) {
        this.actionViewModel = actionViewModel;
        this.onPaneId = routeData.paneId;

        const fields = this.context.getCurrentDialogValues(this.actionMember().actionId(), this.onPaneId);

        const parameters = _.pickBy(actionViewModel.invokableActionRep.parameters(), p => !p.isCollectionContributed()) as _.Dictionary<Models.Parameter>;
        this.parameters = _.map(parameters, p => this.viewModelFactory.parameterViewModel(p, fields[p.id()], this.onPaneId));
        this.listenToParameters();

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

    parameterChanged() {
        this.parameterChangedSource.next(true);
        this.parameterChangedSource.next(false);
    }

    private parameterChangedSource = new Subject<boolean>();

    parameterChanged$ = this.parameterChangedSource.asObservable();

    private validChangedSource = new Subject<boolean>();

    validChanged$ = this.validChangedSource.asObservable();

    private parmSubs: ISubscription[] = [];

    private listenToParameters() {
        _.forEach(this.parameters, p => {
            this.parmSubs.push(p.validChanged$.subscribe(changed => {
                if (changed) {
                    this.validChangedSource.next(true);
                    this.validChangedSource.next(false);
                }
            }))
        });
    }

}

export class PropertyViewModel extends Fieldviewmodel.FieldViewModel implements Idraggableviewmodel.IDraggableViewModel {

    constructor(propertyRep: Models.PropertyMember, color: ColorService, error: ErrorService) {
        super(propertyRep.extensions(), color, error);
        this.draggableType = propertyRep.extensions().returnType();

        this.propertyRep = propertyRep;
        this.entryType = propertyRep.entryType();
        this.isEditable = !propertyRep.disabledReason();
        this.entryType = propertyRep.entryType();
    }


    propertyRep: Models.PropertyMember;
    isEditable: boolean;
    attachment: AttachmentViewModel;
    refType: "null" | "navigable" | "notNavigable";
    isDirty: () => boolean;
    doClick: (right?: boolean) => void;

    // IDraggableViewModel
    draggableType: string;
    draggableTitle = () => this.formattedValue;

    canDropOn: (targetType: string) => Promise<boolean>;
}



export class ListViewModel extends MessageViewModel {

    constructor(private colorService: ColorService,
        private context: ContextService,
        private viewModelFactory: ViewModelFactoryService,
        private urlManager: UrlManagerService,
        private error: ErrorService) {
        super();
    }

    private routeData: PaneRouteData;
    private onPaneId: number;
    private page: number;
    private pageSize: number;
    private numPages: number;
    private state: CollectionViewState;

    allSelected = () => _.every(this.items, item => item.selected);

    id: string;
    listRep: Models.ListRepresentation;
    size: number;
    pluralName: string;
    header: string[];
    items: ItemViewModel[];
    actions: ActionViewModel[];
    menuItems: MenuItemViewModel[];
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
        //this.allSelected = _.every(this.items, item => item.selected);
        const count = this.items.length;
        this.size = count;
        if (count > 0) {
            this.description = () => Msg.pageMessage(this.page, this.numPages, count, totalCount);
        } else {
            this.description = () => Msg.noItemsFound;
        }
    }

    hasTableData = () => this.listRep.hasTableData();

    private collectionContributedActionDecorator(actionViewModel: ActionViewModel) {
        const wrappedInvoke = actionViewModel.execute;
        actionViewModel.execute = (pps: Parameterviewmodel.ParameterViewModel[], right?: boolean) => {
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

    private collectionContributedInvokeDecorator(actionViewModel: ActionViewModel) {

        const showDialog = () =>
            this.context.getInvokableAction(actionViewModel.actionRep as Models.ActionMember).
                then(invokableAction => _.keys(invokableAction.parameters()).length > 1);

        // make sure not null while waiting for promise to assign correct function 
        actionViewModel.doInvoke = () => { };

        const invokeWithDialog = (right?: boolean) => {
            this.context.clearDialogValues(this.onPaneId);
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

    private decorate(actionViewModel: ActionViewModel) {
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
        if (this.disableActions()) return;
        this.urlManager.toggleObjectMenu(this.onPaneId);
    };

    pageNext = () => {
        if (this.pageNextDisabled()) return;
        this.setPage(this.page < this.numPages ? this.page + 1 : this.page, this.state);
    };
    pagePrevious = () => {
        if (this.pagePreviousDisabled()) return;
        this.setPage(this.page > 1 ? this.page - 1 : this.page, this.state);
    };
    pageFirst = () => {
        if (this.pageFirstDisabled()) return;
        this.setPage(1, this.state);
    };
    pageLast = () => {
        if (this.pageLastDisabled()) return;
        this.setPage(this.numPages, this.state);
    };

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

    selectAll = () => {
        const newState = !this.allSelected();

        _.each(this.items, (item) => {
            item.selected = newState;
            //item.selectionChange();
        });

        //_.each(this.items, (item) => {
        //    //item.selected = newState;
        //    item.selectionChange();
        //});
    };

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

export class CollectionViewModel  {

    title: string;
    details: string;
    pluralName: string;
    color: string;
    mayHaveItems: boolean;
    editing: boolean;
    items: ItemViewModel[];
    header: string[];
    onPaneId: number;
    currentState: CollectionViewState;
    //requestedState: CollectionViewState;
    presentationHint: string;
    template: string;
    actions: ActionViewModel[];
    menuItems: MenuItemViewModel[];
    messages: string;
    collectionRep: Models.CollectionMember | Models.CollectionRepresentation;

    doSummary: () => void;
    doTable: () => void;
    doList: () => void;
    hasTableData: () => boolean;

    description = () => this.details.toString();
    refresh: (routeData: PaneRouteData, resetting: boolean) => void;

    disableActions = () => this.editing || !this.actions || this.actions.length === 0;
    allSelected = () => _.every(this.items, item => item.selected);
    selectAll() { }
}

export class MenusViewModel {
    constructor(private viewModelFactory: ViewModelFactoryService) { }

    reset(menusRep: Models.MenusRepresentation, routeData: PaneRouteData) {
        this.menusRep = menusRep;
        this.onPaneId = routeData.paneId;
        this.items = _.map(this.menusRep.value(), link => this.viewModelFactory.linkViewModel(link, this.onPaneId));
        return this;
    }

    menusRep: Models.MenusRepresentation;
    onPaneId: number;
    items: LinkViewModel[];
}

export class MenuViewModel extends MessageViewModel  {
    id: string;
    title: string;
    actions: ActionViewModel[];
    menuItems: MenuItemViewModel[];
    menuRep: Models.MenuRepresentation;
}




export class DomainObjectViewModel extends MessageViewModel implements IDraggableViewModel {

    constructor(private colorService: ColorService,
        private contextService: ContextService,
        private viewModelFactory: ViewModelFactoryService,
        private urlManager: UrlManagerService,
        private error: ErrorService) {
        super();
    }

    private routeData: PaneRouteData;
    private props: _.Dictionary<Models.Value>;
    private instanceId: string;
    private unsaved: boolean;

    // IDraggableViewModel
    value: string;
    reference: string;
    selectedChoice:  ChoiceViewModel;
    color: string;
    draggableType: string;
    draggableTitle = () => this.title;

    domainObject: Models.DomainObjectRepresentation;
    onPaneId: number;

    title: string;
    friendlyName: string;
    presentationHint: string;
    domainType: string;

    isInEdit: boolean;

    actions: ActionViewModel[];
    menuItems: MenuItemViewModel[];
    properties: PropertyViewModel[];
    collections: CollectionViewModel[];

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

    private wrapAction(a: ActionViewModel) {
        const wrappedInvoke = a.execute;
        a.execute = (pps: Parameterviewmodel.ParameterViewModel[], right?: boolean) => {
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

    reset(obj: Models.DomainObjectRepresentation, routeData: PaneRouteData): DomainObjectViewModel {
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
        this.selectedChoice = sav ?  ChoiceViewModel.create(sav, "") : null;

        this.colorService.toColorNumberFromType(this.domainObject.domainType()).
            then(c => this.color = `${Config.objectColor}${c}`).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        this.resetMessage();

        if (routeData.interactionMode === InteractionMode.Form) {
            _.forEach(this.actions, a => this.wrapAction(a));
        }


        return this as DomainObjectViewModel;
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

