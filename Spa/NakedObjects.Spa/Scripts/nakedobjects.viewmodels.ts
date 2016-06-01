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
    import toTimeString = Models.toTimeString;

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
            const menus = menupath.split("-");

            if (menus.length > level) {
                menu = menus[level];
            }
        }

        return menu || "";
    }

    export function createSubmenuItems(avms: ActionViewModel[], menu: MenuItemViewModel, level: number) {
        // if not root menu aggregate all actions with same name
        if (menu.name) {
            const actions = _.filter(avms, a => getMenuForLevel(a.menuPath, level) === menu.name && 
                                               !getMenuForLevel(a.menuPath, level + 1));
            menu.actions = actions;

            //then collate submenus 

            const submenuActions = _.filter(avms, a => getMenuForLevel(a.menuPath, level) === menu.name &&
                                                       getMenuForLevel(a.menuPath, level + 1));
            let menus = _
                .chain(submenuActions)
                .map(a => ({ name: getMenuForLevel(a.menuPath, level + 1), actions: null, menuItems: null }))
                .value();

            // remove non unique submenus 
            menus = _.uniqWith(menus, (a: { name: string }, b: { name: string }) => {
                if (a.name && b.name) {
                    return a.name === b.name;
                }
                return false;
            });

            menu.menuItems = _.map(menus, m => createSubmenuItems(submenuActions, m, level + 1));
        }
        return menu;
    }

  
    export function createMenuItems(avms: ActionViewModel[]) {

        // first create a top level menu for each action 
        let menus = _
            .chain(avms)
            .map(a => ({ name: getMenuForLevel(a.menuPath, 0), actions: [a], menuItems : null }))
            .value();

        // remove non unique submenus 
        menus = _.uniqWith(menus, (a: { name: string }, b: { name: string }) => {
            if (a.name && b.name) {
                return a.name === b.name;
            }
            return false;
        });

        // update submenus with all actions under same submenu
        return _.map(menus, m => createSubmenuItems(avms, m, 0));
    }

    export class AttachmentViewModel {
        href: string;
        mimeType: string;
        title: string;
        link: Link;
        onPaneId: number;

        private parent: DomainObjectRepresentation;
        private context: IContext;

        static create(attachmentLink: Link, parent: DomainObjectRepresentation, context: IContext, paneId: number) {
            const attachmentViewModel = new AttachmentViewModel();
            attachmentViewModel.link = attachmentLink;
            attachmentViewModel.href = attachmentLink.href();
            attachmentViewModel.mimeType = attachmentLink.type().asString;
            attachmentViewModel.title = attachmentLink.title() || unknownFileTitle;
            attachmentViewModel.parent = parent;
            attachmentViewModel.context = context;
            attachmentViewModel.onPaneId = paneId;
            return attachmentViewModel;
        }

        downloadFile = () => this.context.getFile(this.parent, this.href, this.mimeType);
        clearCachedFile = () => this.context.clearCachedFile(this.href);

        displayInline = () =>
            this.mimeType === "image/jpeg" ||
            this.mimeType === "image/gif" ||
            this.mimeType === "application/octet-stream";

        doClick: (right?: boolean) => void;
    }

    export class ChoiceViewModel {
        id: string;
        name: string;
        value: string;
        search: string;
        isEnum: boolean;
        isReference: boolean;
        wrapped: Value;

        static create(value: Value, id: string, name?: string, searchTerm?: string) {
            const choiceViewModel = new ChoiceViewModel();
            choiceViewModel.wrapped = value;
            choiceViewModel.id = id;
            choiceViewModel.name = name || value.toString();
            choiceViewModel.value = value.isReference() ? value.link().href() : value.toValueString();
            choiceViewModel.search = searchTerm || choiceViewModel.name;

            choiceViewModel.isEnum = !value.isReference() && (choiceViewModel.name !== choiceViewModel.value);
            choiceViewModel.isReference = value.isReference();
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
        previousMessage : string = "";
        message: string = "";
        clearMessage() {
            if (this.message === this.previousMessage) {
                this.resetMessage();
            } else {
                this.previousMessage = this.message;
            }
        }

        resetMessage() {
            this.message = this.previousMessage = "";
        }

        setMessage(msg: string) {
            this.message = msg;
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
        file : Blob;

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

        setColor(color: IColor) {

            if (this.entryType === EntryType.AutoComplete && this.choice && this.type === "ref") {
                const href = this.choice.value;
                if (href) {
                    color.toColorNumberFromHref(href).then((c: number) => this.color = `${linkColor}${c}`);
                    return;
                }
            }
            else if (this.entryType !== EntryType.AutoComplete && this.value) {
                color.toColorNumberFromType(this.returnType).then((c: number) => this.color = `${linkColor}${c}`);
                return;
            }

            this.color = "";
        }


        getValue(): Value {

            if (this.entryType === EntryType.File) {
                return new Value(this.file);
            }

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

                    if (this.format === "time") {
                        // time format
                        return new Value(toTimeString(this.value as Date));
                    }

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

    export class MenuItemViewModel {
        name: string;
        actions: ActionViewModel[];
        menuItems: MenuItemViewModel[];
    }

    export class DialogViewModel extends MessageViewModel {
        constructor(private color: IColor,
            private context: IContext,
            private viewModelFactory: IViewModelFactory,
            private urlManager: IUrlManager,
            private focusManager: IFocusManager,
            private $rootScope: ng.IRootScopeService) {
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
            this.resetMessage();
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
                        this.setMessage(actionResult.warningsOrMessages() || noResultMessage);
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
                    const parent = this.actionMember().parent instanceof DomainObjectRepresentation ? this.actionMember().parent as DomainObjectRepresentation : null;
                    const display = (em: ErrorMap) => this.viewModelFactory.handleErrorResponse(em, this, this.parameters);
                    this.context.handleWrappedError(reject,
                                                    parent,
                                                    () => {
                                                        // this should just be called if concurrency
                                                        this.doClose();
                                                        this.$rootScope.$broadcast(geminiDisplayErrorEvent, new ErrorMap({}, 0, concurrencyError));
                                                    },
                                                    display);
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
            this.resetMessage();
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

        hasTableData = () => {
            const valueLinks = this.listRep.value();
            return valueLinks && _.some(valueLinks, (i: Link) => i.members());
        }

        refresh(routeData: PaneRouteData) {

            this.routeData = routeData;
            if (this.state !== routeData.state) {
                this.state = routeData.state;
                if (this.state === CollectionViewState.Table && !this.hasTableData()) {
                    this.recreate(this.page, this.pageSize)
                        .then(list => {
                            this.listRep = list;
                            this.updateItems(list.value());
                        });
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
                            then(result => this.setMessage(result.shouldExpectResult() ? result.warningsOrMessages() || noResultMessage : "")).
                            catch((reject: ErrorWrapper) => {
                                const display = (em: ErrorMap) => this.setMessage(em.invalidReason() || em.warningMessage);
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
            
            this.state = routeData.state;
            this.updateItems(list.value());

            const actions = this.listRep.actionMembers();
            this.actions = _.map(actions, action => this.viewModelFactory.actionViewModel(action, this, routeData));
            this.menuItems = createMenuItems(this.actions);

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
                    const display = (em: ErrorMap) => this.setMessage(em.invalidReason() || em.warningMessage);
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
        menuItems: MenuItemViewModel[];

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
        menuItems: MenuItemViewModel[];
        messages: string;

        collectionRep: CollectionMember | CollectionRepresentation;
        refresh: (routeData: PaneRouteData, resetting : boolean) => void;
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
        menuItems: MenuItemViewModel[];
        color: string;
    }

    export class MenuViewModel extends MessageViewModel {
        id: string;
        title: string;
        actions: ActionViewModel[];
        menuItems: MenuItemViewModel[];
        color: string;
        menuRep: Models.MenuRepresentation;
    }

    export class TableRowColumnViewModel {
        type: "ref" | "scalar";
        returnType: string;
        value: scalarValueType | Date;
        formattedValue: string;
        title : string;
    }

    export class TableRowViewModel {
        title: string;
        hasTitle: boolean;
        properties: TableRowColumnViewModel[];
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
            _.forEach(this.collections, c => c.refresh(this.routeData, false));

            this.unsaved = routeData.interactionMode === InteractionMode.Transient;

            this.title = this.unsaved ? `Unsaved ${this.domainObject.extensions().friendlyName()}` : this.domainObject.title();

            this.title = this.title + dirtyMarker(this.contextService, this.domainObject.getOid());

            if (routeData.interactionMode === InteractionMode.Form) {
                _.forEach(this.actions, a => this.wrapAction(a));
            }

            // leave message from previous refresh 
            this.clearMessage();

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

            this.menuItems = createMenuItems(this.actions);

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

            this.resetMessage();

            if (routeData.interactionMode === InteractionMode.Form) {
                _.forEach(this.actions, a => this.wrapAction(a));
            }

            return this;
        }

        displayError() {
            return (event: ng.IAngularEvent, em: ErrorMap) => {
                this.routeData = this.urlManager.getRouteData().pane()[this.onPaneId];
                this.contextService.getObject(this.onPaneId, this.domainObject.getOid(), this.routeData.interactionMode)
                    .then(obj => {
                        this.reset(obj, this.routeData);
                        this.viewModelFactory.handleErrorResponse(em, this, this.properties);
                    });
            }
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
        menuItems: MenuItemViewModel[];
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
                    this.resetMessage();
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

        canDropOn = (targetType: string) => this.contextService.isSubTypeOf(this.domainType, targetType);
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
        attachmentTemplate: string;

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
        attachment: AttachmentViewModel;
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