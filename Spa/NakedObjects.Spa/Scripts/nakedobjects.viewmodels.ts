/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.userMessages.config.ts" />


module NakedObjects {

    import Value = Models.Value;
    import Link = Models.Link;
    import EntryType = Models.EntryType;
    import Parameter = Models.Parameter;
    import ActionMember = Models.ActionMember;
    import ActionResultRepresentation = Models.ActionResultRepresentation;
    import ErrorWrapper = Models.ErrorWrapper;
    import DomainObjectRepresentation = Models.DomainObjectRepresentation;
    import ErrorMap = Models.ErrorMap;
    import PropertyMember = Models.PropertyMember;
    import ListRepresentation = Models.ListRepresentation;
    import ErrorCategory = Models.ErrorCategory;
    import HttpStatusCode = Models.HttpStatusCode;
    import CollectionMember = Models.CollectionMember;
    import MenusRepresentation = Models.MenusRepresentation;
    import DateString = Models.toDateString;
    import ActionRepresentation = Models.ActionRepresentation;
    import IInvokableAction = Models.IInvokableAction;
    import CollectionRepresentation = Models.CollectionRepresentation;
    import scalarValueType = RoInterfaces.scalarValueType;
    import dirtyMarker = Models.dirtyMarker;


    export interface IDraggableViewModel {
        canDropOn: (targetType: string) => ng.IPromise<boolean>;
        value: scalarValueType | Date;
        reference: string;
        choice: ChoiceViewModel;
        color: string;
        draggableType: string;
    }

    function tooltip(onWhat: { clientValid: () => boolean }, fields: ValueViewModel[]): string {
        if (onWhat.clientValid()) {
            return "";
        }

        const missingMandatoryFields = _.filter(fields, p => !p.clientValid && !p.message);

        if (missingMandatoryFields.length > 0) {
            return _.reduce(missingMandatoryFields, (s, t) => s + t.title + "; ", mandatoryFieldsPrefix);
        }

        const invalidFields = _.filter(fields, p => !p.clientValid);

        if (invalidFields.length > 0) {
            return _.reduce(invalidFields, (s, t) => s + t.title + "; ", invalidFieldsPrefix);
        }

        return "";
    }

    function actionsTooltip(onWhat: { disableActions: () => boolean }, actionsOpen: boolean) {
        if (actionsOpen) {
            return closeActions;
        }
        return onWhat.disableActions() ? noActions : openActions;
    }

    export function createActionSubmenuMap(avms: ActionViewModel[], menu: { name: string, actions: ActionViewModel[] }) {
        // if not root menu aggregate all actions with same name
        if (menu.name) {
            const actions = _.filter(avms, a => a.menuPath === menu.name);
            menu.actions = actions;
        }
        return menu;
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

    export function createActionMenuMap(avms: ActionViewModel[]) {

        // first create a menu for each action 
        let menus = _
            .chain(avms)
            .map(a => ({ name: a.menuPath, actions: [a] }))
            .value();

        // remove non unique submenus 
        menus = _.uniqWith(menus, (a: { name: string }, b: { name: string }) => {
            if (a.name && b.name) {
                return a.name === b.name;
            }
            return false;
        });

        // update submenus with all actions under same submenu
        return _.map(menus, m => createActionSubmenuMap(avms, m));
    }

    export class AttachmentViewModel {
        href: string;
        mimeType: string;
        title: string;

        static create(href: string, mimeType: string, title: string) {
            const attachmentViewModel = new AttachmentViewModel();
            attachmentViewModel.href = href;
            attachmentViewModel.mimeType = mimeType;
            attachmentViewModel.title = title || unknownFileTitle;

            return attachmentViewModel;
        }

        downloadFile: () => ng.IPromise<Blob>;
        clearCachedFile: () => void;
    }

    export class ChoiceViewModel {
        id: string;
        name: string;
        value: string;
        search: string;
        isEnum: boolean;
        wrapped: Value;

        static create(value: Value, id: string, name?: string, searchTerm?: string) {
            const choiceViewModel = new ChoiceViewModel();
            choiceViewModel.wrapped = value;
            choiceViewModel.id = id;
            choiceViewModel.name = name || value.toString();
            choiceViewModel.value = value.isReference() ? value.link().href() : value.toValueString();
            choiceViewModel.search = searchTerm || choiceViewModel.name;

            choiceViewModel.isEnum = !value.isReference() && (choiceViewModel.name !== choiceViewModel.value);
            return choiceViewModel;
        }

        equals(other: ChoiceViewModel) {
            return this.id === other.id &&
                this.name === other.name &&
                this.value === other.value;
        }

        match(other: ChoiceViewModel) {
            const thisValue = this.isEnum ? this.value.trim() : this.search.trim();
            const otherValue = this.isEnum ? other.value.trim() : other.search.trim();
            return thisValue === otherValue;
        }
    }

    export class ErrorViewModel {
        title: string;
        message: string;
        stackTrace: string[];
        code: string;
        description: string;
        isConcurrencyError: boolean;
    }

    export class LinkViewModel implements IDraggableViewModel {
        title: string;
        color: string;
        doClick: (right?: boolean) => void;

        canDropOn: (targetType: string) => ng.IPromise<boolean>;

        value: scalarValueType;
        reference: string;
        choice: ChoiceViewModel;
        domainType: string;
        draggableType: string;
        link: Link;
    }

    export class ItemViewModel extends LinkViewModel {
        target: TableRowViewModel;
        selected: boolean;
        checkboxChange: (index: number) => void;
    }

    export class RecentItemViewModel extends ItemViewModel {
        friendlyName: string;
    }

    export class MessageViewModel {
        message: string;
        clearMessage() {
            this.message = "";
        }
    }

    export class ValueViewModel extends MessageViewModel {

        validate: (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;
        clientValid: boolean;

        isDirty = () => false;

        currentValue: Value;
        originalValue: Value;

        localFilter: ILocalFilter;
        formattedValue: string;
        value: scalarValueType | Date;
        id: string;
        argId: string;
        paneArgId: string;
        choices: ChoiceViewModel[];

        type: "scalar" | "ref";
        reference: string;
        choice: ChoiceViewModel;
        multiChoices: ChoiceViewModel[];
        returnType: string;
        title: string;
        format: formatType;
        arguments: _.Dictionary<Value>;
        mask: string;
        password: boolean;

        minLength: number;

        color: string;
        description: string;
        optional: boolean;
        isCollectionContributed: boolean;
        onPaneId: number;

        multipleLines: number;

        entryType: EntryType;

        refresh: (newValue: Value) => void;

        prompt(searchTerm: string): ng.IPromise<ChoiceViewModel[]> {
            return null;
        }

        conditionalChoices(args: _.Dictionary<Value>): ng.IPromise<ChoiceViewModel[]> {
            return null;
        }

        setNewValue(newValue: IDraggableViewModel) {
            this.value = newValue.value;
            this.reference = newValue.reference;
            this.choice = newValue.choice;
            this.color = newValue.color;
        }

        drop: (newValue: IDraggableViewModel) => void;

        clear() {
            this.value = null;
            this.reference = "";
            this.choice = null;
            this.color = "";
        }

        getValue(): Value {

            if (this.entryType !== EntryType.FreeForm || this.isCollectionContributed) {

                if (this.entryType === EntryType.MultipleChoices || this.entryType === EntryType.MultipleConditionalChoices || this.isCollectionContributed) {
                    const selections = this.multiChoices || [];
                    if (this.type === "scalar") {
                        const selValues = _.map(selections, cvm => cvm.value);
                        return new Value(selValues);
                    }
                    const selRefs = _.map(selections, cvm => ({ href: cvm.value, title: cvm.name })); // reference 
                    return new Value(selRefs);
                }


                if (this.type === "scalar") {
                    return new Value(this.choice && this.choice.value != null ? this.choice.value : "");
                }

                // reference 
                return new Value(this.choice && this.choice.value ? { href: this.choice.value, title: this.choice.name } : null);
            }

            if (this.type === "scalar") {
                if (this.value == null) {
                    return new Value("");
                }

                if (this.value instanceof Date) {

                    if (this.format === "date") {
                        // truncate time;
                        return new Value(DateString(this.value as Date));
                    }
                    // date-time
                    return new Value((this.value as Date).toISOString());
                }

                return new Value(this.value as scalarValueType);
            }

            // reference
            return new Value(this.reference ? { href: this.reference, title: this.value.toString() } : null);
        }
    }

    export class ParameterViewModel extends ValueViewModel {
        parameterRep: Parameter;
        dflt: string;
    }

    export class ActionViewModel {
        actionRep: ActionMember | ActionRepresentation;
        invokableActionRep: IInvokableAction;

        menuPath: string;
        title: string;
        description: string;

        // todo - confusing name better 
        doInvoke: (right?: boolean) => void;
        executeInvoke: (pps: ParameterViewModel[], right?: boolean) => ng.IPromise<ActionResultRepresentation>;

        disabled(): boolean { return false; }

        parameters: () => ParameterViewModel[];
        stopWatchingParms: () => void;

        makeInvokable: (details: IInvokableAction) => void;
    }

    export class DialogViewModel extends MessageViewModel {
        constructor(private color: IColor,
            private context: IContext,
            private viewModelFactory: IViewModelFactory,
            private urlManager: IUrlManager,
            private focusManager: IFocusManager) {
            super();
        }

        reset(actionViewModel: ActionViewModel, routeData: PaneRouteData) {
            this.actionViewModel = actionViewModel;
            this.onPaneId = routeData.paneId;

            const fields = routeData.dialogFields;

            const parameters = _.pickBy(actionViewModel.invokableActionRep.parameters(), p => !p.isCollectionContributed()) as _.Dictionary<Parameter>;
            this.parameters = _.map(parameters, p => this.viewModelFactory.parameterViewModel(p, fields[p.id()], this.onPaneId));

            this.title = this.actionMember().extensions().friendlyName();
            this.isQueryOnly = actionViewModel.invokableActionRep.invokeLink().method() === "GET";
            this.message = "";
            this.id = actionViewModel.actionRep.actionId();
            return this;
        }

        private actionMember = () => this.actionViewModel.actionRep;
        title: string;
        message: string;
        isQueryOnly: boolean;
        onPaneId: number;
        id: string;

        deregister: () => void;

        actionViewModel: ActionViewModel;

        clientValid = () => _.every(this.parameters, p => p.clientValid);

        tooltip = () => tooltip(this, this.parameters);

        setParms = () =>
            _.forEach(this.parameters, p => this.urlManager.setFieldValue(this.actionMember().actionId(), p.parameterRep, p.getValue(), this.onPaneId));

        private executeInvoke = (right?: boolean) => {

            const pps = this.parameters;
            _.forEach(pps, p => this.urlManager.setFieldValue(this.actionMember().actionId(), p.parameterRep, p.getValue(), this.onPaneId));
            return this.actionViewModel.executeInvoke(pps, right);
        };

        doInvoke = (right?: boolean) =>
            this.executeInvoke(right).
                then((actionResult: ActionResultRepresentation) => {
                    if (actionResult.shouldExpectResult()) {
                        this.message = actionResult.warningsOrMessages() || noResultMessage;
                    } else if (actionResult.resultType() === "void") {
                        // dialog staying on same page so treat as cancel 
                        // for url replacing purposes
                        this.doCancel();
                    }
                    else if (!right) {
                        // going to new page close dialog (and do not replace url)
                        this.doClose();
                    }
                    // else leave open if opening on other pane and dialog has result

                }).
                catch((reject: ErrorWrapper) => {
                    const parent = this.actionMember().parent as DomainObjectRepresentation;
                    const display = (em: ErrorMap) => this.viewModelFactory.handleErrorResponse(em, this, this.parameters);
                    this.context.handleWrappedError(reject, parent, () => { }, display);
                });

        doClose = () => {
            this.deregister();
            this.urlManager.closeDialog(this.onPaneId);
        }

        doCancel = () => {
            this.deregister();
            this.urlManager.cancelDialog(this.onPaneId);
        }

        clearMessages = () => {
            this.message = "";
            _.each(this.actionViewModel.parameters, parm => parm.clearMessage());
        };

        parameters: ParameterViewModel[];
    }

    export class PropertyViewModel extends ValueViewModel implements IDraggableViewModel {

        propertyRep: PropertyMember;
        target: string;
        isEditable: boolean;
        attachment: AttachmentViewModel;
        draggableType: string;
        refType: "null" | "navigable" | "notNavigable";

        doClick(right?: boolean): void { }

        canDropOn: (targetType: string) => ng.IPromise<boolean>;
    }

    export class CollectionPlaceholderViewModel {
        description: () => string;
        reload: () => void;
    }

    export class ListViewModel extends MessageViewModel {

        constructor(private colorService: IColor,
            private contextService: IContext,
            private viewModelFactory: IViewModelFactory,
            private urlManager: IUrlManager,
            private focusManager: IFocusManager,
            private $q: ng.IQService) {
            super();
        }

        updateItems(value: Link[]) {
            this.items = this.viewModelFactory.getItems(value,
                                                        this.state === CollectionViewState.Table,
                                                        this.routeData,
                                                        this);

            const totalCount = this.listRep.pagination().totalCount;
            this.allSelected = _.every(this.items, item => item.selected);
            const count = this.items.length;
            this.size = count;
            this.description = () => pageMessage(this.page, this.numPages, count, totalCount);
        }

        refresh(routeData: PaneRouteData) {

            this.routeData = routeData;
            if (!this.state || this.state !== routeData.state) {
                this.state = routeData.state;
                if (this.state === CollectionViewState.Table) {
                    this.recreate(this.page, this.pageSize).then(list => this.updateItems(list.value()));
                } else {
                    this.updateItems(this.listRep.value());
                }
            }
        }

        collectionContributedActionDecorator(actionViewModel : ActionViewModel) {
            const wrappedInvoke = actionViewModel.executeInvoke;
            actionViewModel.executeInvoke = (pps: ParameterViewModel[], right?: boolean) => {
                const selected = _.filter(this.items, i => i.selected);

                if (selected.length === 0) {

                    const em = new ErrorMap({}, 0, noItemsSelected);
                    const rp = new ErrorWrapper(ErrorCategory.HttpClientError, HttpStatusCode.UnprocessableEntity, em);

                    return this.$q.reject(rp);
                }

                const getParms = (action: IInvokableAction) => {

                    const parms = _.values(action.parameters()) as Parameter[];
                    const contribParm = _.find(parms, p => p.isCollectionContributed());
                    const parmValue = new Value(_.map(selected, i => i.link));
                    const collectionParmVm = this.viewModelFactory
                        .parameterViewModel(contribParm, parmValue, this.onPaneId);

                    const allpps = _.clone(pps);
                    allpps.push(collectionParmVm);
                    return allpps;
                }

                if (actionViewModel.invokableActionRep) {
                    return wrappedInvoke(getParms(actionViewModel.invokableActionRep), right);
                }

                return this.contextService.getActionDetails(actionViewModel.actionRep as ActionMember)
                    .then((details: ActionRepresentation) => wrappedInvoke(getParms(details), right));
            }
        }

        collectionContributedInvokeDecorator(actionViewModel: ActionViewModel) {
            const showDialog = () => this.contextService.getInvokableAction(actionViewModel.actionRep as ActionMember).
                then((ia: IInvokableAction) => _.keys(ia.parameters()).length > 1);

            actionViewModel.doInvoke = () => { };
            showDialog().
                then((show: boolean) => actionViewModel.doInvoke = show ?
                    (right?: boolean) => {
                        this.focusManager.focusOverrideOff();
                        this.urlManager.setDialog(actionViewModel.actionRep.actionId(), this.onPaneId);
                    } :
                    (right?: boolean) => {
                        actionViewModel.executeInvoke([], right).
                            then(result => this.message = result.shouldExpectResult() ? result.warningsOrMessages() || noResultMessage : "").
                            catch((reject: ErrorWrapper) => {
                                const display = (em: ErrorMap) => this.message = em.invalidReason() || em.warningMessage;
                                this.contextService.handleWrappedError(reject, null, () => { }, display);
                            });
                    });
        }

        decorate(actionViewModel: ActionViewModel) {
            this.collectionContributedActionDecorator(actionViewModel);
            this.collectionContributedInvokeDecorator(actionViewModel);
        }


        reset(list: ListRepresentation, routeData: PaneRouteData) {
            this.listRep = list;
            this.routeData = routeData;

            this.id = this.urlManager.getListCacheIndex(routeData.paneId, routeData.page, routeData.pageSize);

            this.onPaneId = routeData.paneId;

            this.pluralName = "Objects";
            this.page = this.listRep.pagination().page;
            this.pageSize = this.listRep.pagination().pageSize;
            this.numPages = this.listRep.pagination().numPages;
            //clear state so we always refresh items
            this.state = null;

            this.refresh(routeData);

            const actions = this.listRep.actionMembers();
            this.actions = _.map(actions, action => this.viewModelFactory.actionViewModel(action, this, routeData));
            this.actionsMap = createActionMenuMap(this.actions);

            _.forEach(this.actions, a => this.decorate(a));
               
            return this;
        }

        toggleActionMenu = () => {
            this.focusManager.focusOverrideOff();
            this.urlManager.toggleObjectMenu(this.onPaneId);
        };

        private recreate = (page: number, pageSize: number) => {
            return this.routeData.objectId ?
                this.contextService.getListFromObject(this.routeData.paneId, this.routeData, page, pageSize) :
                this.contextService.getListFromMenu(this.routeData.paneId, this.routeData, page, pageSize);
        };

        private pageOrRecreate = (newPage: number, newPageSize: number, newState?: CollectionViewState) => {
            this.recreate(newPage, newPageSize).
                then((list: ListRepresentation) => {
                    this.routeData.state = newState || this.routeData.state;
                    this.routeData.page = newPage;
                    this.routeData.pageSize = newPageSize;
                    this.reset(list, this.routeData);
                    this.urlManager.setListPaging(newPage, newPageSize, this.routeData.state, this.onPaneId);
                }).
                catch((reject: ErrorWrapper) => {
                    const display = (em: ErrorMap) => this.message = em.invalidReason() || em.warningMessage;
                    this.contextService.handleWrappedError(reject, null, () => { }, display);
                });
        };

        listRep: ListRepresentation;
        routeData: PaneRouteData;

        size: number;
        pluralName: string;
        color: string;
        items: ItemViewModel[];
        header: string[];
        onPaneId: number;
        page: number;
        pageSize: number;
        numPages: number;
        state: CollectionViewState;

        allSelected: boolean;

        id: string;

        private setPage = (newPage: number, newState: CollectionViewState) => {
            this.focusManager.focusOverrideOff();
            this.pageOrRecreate(newPage, this.pageSize, newState);
        };
        pageNext = () => this.setPage(this.page < this.numPages ? this.page + 1 : this.page, this.state);
        pagePrevious = () => this.setPage(this.page > 1 ? this.page - 1 : this.page, this.state);
        pageFirst = () => this.setPage(1, this.state);
        pageLast = () => this.setPage(this.numPages, this.state);

        private earlierDisabled = () => this.page === 1 || this.numPages === 1;
        private laterDisabled = () => this.page === this.numPages || this.numPages === 1;

        private pageFirstDisabled = this.earlierDisabled;
        private pageLastDisabled = this.laterDisabled;
        private pageNextDisabled = this.laterDisabled;
        private pagePreviousDisabled = this.earlierDisabled;

        doSummary = () => this.urlManager.setListState(CollectionViewState.Summary, this.onPaneId);
        doList = () => this.urlManager.setListState(CollectionViewState.List, this.onPaneId);
        doTable = () => this.urlManager.setListState(CollectionViewState.Table, this.onPaneId);

        reload = () => {
            this.contextService.clearCachedList(this.onPaneId, this.routeData.page, this.routeData.pageSize);
            this.setPage(this.page, this.state);
        };

        selectAll = () => _.each(this.items, (item, i) => {
            item.selected = this.allSelected;
            item.checkboxChange(i);
        });

        description(): string { return this.size ? this.size.toString() : "" }

        template: string;

        disableActions(): boolean {
            return !this.actions || this.actions.length === 0 || !this.items || this.items.length === 0;
        }

        actionsTooltip = () => actionsTooltip(this, !!this.routeData.actionsOpen);

        actions: ActionViewModel[];
        actionsMap: { name: string; actions: ActionViewModel[] }[];

        actionMember = (id: string) => {
            const actionViewModel = _.find(this.actions, a => a.actionRep.actionId() === id);
            return actionViewModel.actionRep;
        }
    }

    export class CollectionViewModel {

        title: string;
        size: string;
        pluralName: string;
        color: string;
        items: ItemViewModel[];
        header: string[];
        onPaneId: number;
        currentState: CollectionViewState;

        id: string;

        doSummary(): void { }
        doTable(): void { }
        doList(): void { }

        description(): string { return this.size.toString() }

        template: string;

        actions: ActionViewModel[];
        actionsMap: { name: string; actions: ActionViewModel[] }[];
        messages: string;

        collectionRep: CollectionMember | CollectionRepresentation;
        refresh: (routeData: PaneRouteData) => void;
    }

    export class ServicesViewModel {
        title: string;
        color: string;
        items: LinkViewModel[];
    }

    export class MenusViewModel {
        constructor(private viewModelFactory: IViewModelFactory) {

        }

        reset(menusRep: MenusRepresentation, routeData: PaneRouteData) {
            this.menusRep = menusRep;
            this.onPaneId = routeData.paneId;

            this.title = "Menus";
            this.color = "bg-color-darkBlue";
            this.items = _.map(this.menusRep.value(), link => this.viewModelFactory.linkViewModel(link, this.onPaneId));
            return this;
        }

        menusRep: MenusRepresentation;
        onPaneId: number;
        title: string;
        color: string;
        items: LinkViewModel[];
    }

    export class ServiceViewModel extends MessageViewModel {
        title: string;
        serviceId: string;
        actions: ActionViewModel[];
        actionsMap: { name: string; actions: ActionViewModel[] }[];
        color: string;
    }

    export class MenuViewModel extends MessageViewModel {
        id: string;
        title: string;
        actions: ActionViewModel[];
        actionsMap: { name: string; actions: ActionViewModel[] }[];
        color: string;
        menuRep: Models.MenuRepresentation;
    }

    export class TableRowViewModel {
        title: string;
        hasTitle: boolean;
        properties: PropertyViewModel[];
    }

    export class DomainObjectViewModel extends MessageViewModel implements IDraggableViewModel {

        constructor(private colorService: IColor,
            private contextService: IContext,
            private viewModelFactory: IViewModelFactory,
            private urlManager: IUrlManager,
            private focusManager: IFocusManager,
            private $q: ng.IQService) {
            super();
        }

        propertyMap = () => {
            const pps = _.filter(this.properties, property => property.isEditable);
            return _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Value>;
        };

        wrapAction(a: ActionViewModel) {
            const wrappedInvoke = a.executeInvoke;
            a.executeInvoke = (pps: ParameterViewModel[], right?: boolean) => {
                this.setProperties();
                const pairs = _.map(this.editProperties(), p => [p.id, p.getValue()]);
                const prps = (<any>_).fromPairs(pairs) as _.Dictionary<Value>;

                const parmValueMap = _.mapValues(a.invokableActionRep.parameters(), p => ({ parm: p, value: prps[p.id()] }));
                const allpps = _.map(parmValueMap, o => this.viewModelFactory.parameterViewModel(o.parm, o.value, this.onPaneId));
                return wrappedInvoke(allpps, right).
                    catch((reject: ErrorWrapper) => {
                        this.handleWrappedError(reject);
                        return this.$q.reject(reject);
                    });
            };
        }

        // must be careful with this - OK for changes on client but after server updates should use  reset
        // because parameters may have appeared or disappeared etc and refesh just updates existing views. 
        // So OK for view state changes but not eg for a parameter that disappears after saving

        refresh(routeData: PaneRouteData) {

            this.routeData = routeData;
            const iMode = this.domainObject.extensions().interactionMode();
            this.isInEdit = routeData.interactionMode !== InteractionMode.View || iMode === "form" || iMode === "transient";
            this.props = routeData.interactionMode !== InteractionMode.View ? routeData.props : {};

            _.forEach(this.properties, p => p.refresh(this.props[p.id]));
            _.forEach(this.collections, c => c.refresh(this.routeData));

            this.unsaved = routeData.interactionMode === InteractionMode.Transient;

            this.title = this.unsaved ? `Unsaved ${this.domainObject.extensions().friendlyName()}` : this.domainObject.title();

            this.title = this.title + dirtyMarker(this.contextService, this.domainObject.getOid());

            if (routeData.interactionMode === InteractionMode.Form) {
                _.forEach(this.actions, a => this.wrapAction(a));
            }

            return this;
        }

        reset(obj: DomainObjectRepresentation, routeData: PaneRouteData) {
            this.domainObject = obj;
            this.onPaneId = routeData.paneId;
            this.routeData = routeData;
            const iMode = this.domainObject.extensions().interactionMode();
            this.isInEdit = routeData.interactionMode !== InteractionMode.View || iMode === "form" || iMode === "transient";
            this.props = routeData.interactionMode !== InteractionMode.View ? routeData.props : {};

            const actions = _.values(this.domainObject.actionMembers()) as ActionMember[];
            this.actions = _.map(actions, action => this.viewModelFactory.actionViewModel(action, this, this.routeData));

            this.actionsMap = createActionMenuMap(this.actions);

            this.properties = _.map(this.domainObject.propertyMembers(), (property, id) => this.viewModelFactory.propertyViewModel(property, id, this.props[id], this.onPaneId, this.propertyMap));
            this.collections = _.map(this.domainObject.collectionMembers(), collection => this.viewModelFactory.collectionViewModel(collection, this.routeData));

            this.unsaved = routeData.interactionMode === InteractionMode.Transient;

            this.title = this.unsaved ? `Unsaved ${this.domainObject.extensions().friendlyName()}` : this.domainObject.title();

            this.title = this.title + dirtyMarker(this.contextService, obj.getOid());

            this.friendlyName = this.domainObject.extensions().friendlyName();
            this.domainType = this.domainObject.domainType();
            this.instanceId = this.domainObject.instanceId();
            this.draggableType = this.domainObject.domainType();

            const selfAsValue = () => {
                const link = this.domainObject.selfLink();
                if (link) {
                    // not transient - can't drag transients so no need to set up IDraggable members on transients
                    link.setTitle(this.title);
                    return new Value(link);
                }
                return null;
            };
            const sav = selfAsValue();

            this.value = sav ? sav.toString() : "";
            this.reference = sav ? sav.toValueString() : "";
            this.choice = sav ? ChoiceViewModel.create(sav, "") : null;

            this.colorService.toColorNumberFromType(this.domainObject.domainType()).then((c: number) => {
                this.color = `${objectColor}${c}`;
            });

            this.message = "";

            if (routeData.interactionMode === InteractionMode.Form) {
                _.forEach(this.actions, a => this.wrapAction(a));
            }

            return this;
        }

        routeData: PaneRouteData;
        domainObject: DomainObjectRepresentation;
        onPaneId: number;
        props: _.Dictionary<Value>;

        title: string;
        friendlyName: string;
        domainType: string;
        instanceId: string;
        draggableType: string;
        isInEdit: boolean;
        value: string;
        reference: string;
        choice: ChoiceViewModel;
        color: string;
        actions: ActionViewModel[];
        actionsMap: { name: string; actions: ActionViewModel[] }[];
        properties: PropertyViewModel[];
        collections: CollectionViewModel[];
        unsaved: boolean;

        clientValid = () => _.every(this.properties, p => p.clientValid);

        tooltip = () => tooltip(this, this.properties);

        actionsTooltip = () => actionsTooltip(this, !!this.routeData.actionsOpen);

        toggleActionMenu = () => {
            this.focusManager.focusOverrideOff();
            this.urlManager.toggleObjectMenu(this.onPaneId);
        };

        private editProperties = () => _.filter(this.properties, p => p.isEditable && p.isDirty());
        setProperties = () =>
            _.forEach(this.editProperties(), p => this.urlManager.setPropertyValue(this.domainObject, p.propertyRep, p.getValue(), this.onPaneId));

        private cancelHandler = () => this.domainObject.extensions().interactionMode() === "form" || this.domainObject.extensions().interactionMode() === "transient" ?
            () => this.urlManager.popUrlState(this.onPaneId) :
            () => this.urlManager.setInteractionMode(InteractionMode.View, this.onPaneId);

        editComplete = () => {
            this.setProperties();
        };

        doEditCancel = () => {
            this.editComplete();
            this.cancelHandler()();
        };

        clearCachedFiles = () => {
            _.forEach(this.properties, p => p.attachment ? p.attachment.clearCachedFile() : null);
        }

        private saveHandler = () => this.domainObject.isTransient() ? this.contextService.saveObject : this.contextService.updateObject;

        private validateHandler = () => this.domainObject.isTransient() ? this.contextService.validateSaveObject : this.contextService.validateUpdateObject;

        private handleWrappedError = (reject: ErrorWrapper) => {
            const reset = (updatedObject: DomainObjectRepresentation) => this.reset(updatedObject, this.urlManager.getRouteData().pane()[this.onPaneId]);
            const display = (em: ErrorMap) => this.viewModelFactory.handleErrorResponse(em, this, this.properties);
            this.contextService.handleWrappedError(reject, this.domainObject, reset, display);
        };

        doSave = (viewObject: boolean) => {
            this.clearCachedFiles();
            this.setProperties();
            const propMap = this.propertyMap();
            this.saveHandler()(this.domainObject, propMap, this.onPaneId, viewObject).
                then(obj => this.reset(obj, this.urlManager.getRouteData().pane()[this.onPaneId])).
                catch((reject: ErrorWrapper) => this.handleWrappedError(reject));
        };

        doSaveValidate = () => {
            const propMap = this.propertyMap();

            return this.validateHandler()(this.domainObject, propMap).
                then(() => {
                    this.message = "";
                    return true;
                }).
                catch((reject: ErrorWrapper) => {
                    this.handleWrappedError(reject);
                    return this.$q.reject(false);
                });
        };

        doEdit = () => {
            this.clearCachedFiles();
            this.contextService.getObjectForEdit(this.onPaneId, this.domainObject).
                then((updatedObject: DomainObjectRepresentation) => {
                    this.reset(updatedObject, this.urlManager.getRouteData().pane()[this.onPaneId]);
                    this.urlManager.pushUrlState(this.onPaneId);
                    this.urlManager.setInteractionMode(InteractionMode.Edit, this.onPaneId);
                }).
                catch((reject: ErrorWrapper) => this.handleWrappedError(reject));
        };

        doReload = () => {
            this.clearCachedFiles();
            this.contextService.reloadObject(this.onPaneId, this.domainObject)
                .then((updatedObject: DomainObjectRepresentation) => {
                    this.reset(updatedObject, this.urlManager.getRouteData().pane()[this.onPaneId]);
                })
                .catch((reject: ErrorWrapper) => this.handleWrappedError(reject));
        }


        hideEdit = () => this.domainObject.extensions().interactionMode() === "form" ||
            this.domainObject.extensions().interactionMode() === "transient" ||
            _.every(this.properties, p => !p.isEditable);

        disableActions(): boolean {
            return !this.actions || this.actions.length === 0;
        }

        canDropOn = (targetType: string) => this.contextService.isSubTypeOf(targetType, this.domainType);
    }

    export class ToolBarViewModel {
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

        warnings: string[];
        messages: string[];
    }

    export class RecentItemsViewModel {
        onPaneId: number;
        items: RecentItemViewModel[];
    }

    export interface INakedObjectsScope extends ng.IScope {
        backgroundColor: string;
        title: string;

        homeTemplate: string;
        actionsTemplate: string;
        dialogTemplate: string;
        errorTemplate: string;
        listTemplate: string;
        recentTemplate: string;
        objectTemplate: string;
        collectionsTemplate: string;

        menus: MenusViewModel;
        object: DomainObjectViewModel;
        menu: MenuViewModel;
        dialog: DialogViewModel;
        error: ErrorViewModel;
        recent: RecentItemsViewModel;
        collection: ListViewModel;
        collectionPlaceholder: CollectionPlaceholderViewModel;
        toolBar: ToolBarViewModel;
        cicero: CiceroViewModel;
    }

    export class CiceroViewModel {
        message: string;
        output: string;
        alert = ""; //Alert is appended before the output
        input: string;
        parseInput: (input: string) => void;
        previousInput: string;
        chainedCommands: string[];

        selectPreviousInput(): void {
            this.input = this.previousInput;
        }

        clearInput(): void {
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
        clipboard: DomainObjectRepresentation;

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
}