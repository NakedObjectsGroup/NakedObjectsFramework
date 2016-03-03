/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular.Gemini {
    import toDateString = Helpers.toDateString;

    export interface IDraggableViewModel {
        canDropOn:  (targetType: string) => ng.IPromise<boolean>;
        value : number | string | boolean | Date;
        reference: string;
        choice: ChoiceViewModel;
        color: string;
        draggableType : string;
    }

    export function createActionSubmenuMap(avms: ActionViewModel[], menu: { name: string, actions: ActionViewModel[] }) {
        // if not root menu aggregate all actions with same name
        if (menu.name) {
            const actions = _.filter(avms, a => a.menuPath === menu.name);
            menu.actions = actions;
        }
        return menu; 
    }


    export function createActionMenuMap(avms: ActionViewModel[]) {

        // first create a menu for each action 
        let menus = _
            .chain(avms)
            .map(a => ({name : a.menuPath, actions : [a] }))
            .value();

        // remove non unique submenus 
        menus = _.uniqWith(menus, (a, b) => {
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

        static create(href : string, mimeType : string, title : string) {
            const attachmentViewModel = new AttachmentViewModel();
            attachmentViewModel.href = href;
            attachmentViewModel.mimeType = mimeType;
            attachmentViewModel.title = title || "UnknownFile";

            return attachmentViewModel;
        }
    }

    export class ChoiceViewModel {
        id: string; 
        name: string;
        value: string;
        search: string; 
        isEnum: boolean; 
        wrapped : Value;

        static create(value: Value, id : string, name? : string, searchTerm? : string) {
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
        message: string;
        stackTrace: string[];
        code: string;
        description : string;
        isConcurrencyError: boolean;
    } 

    export class LinkViewModel implements IDraggableViewModel{
        title: string;
        color: string;
        doClick: (right?: boolean) => void;

        canDropOn: (targetType: string) => ng.IPromise<boolean>;

        value: number | string | boolean;
        reference: string;
        choice: ChoiceViewModel;
        domainType: string;
        draggableType: string;
        link : Link;
    }

    export class ItemViewModel extends LinkViewModel{
        target: TableRowViewModel;   
        selected : boolean;
        checkboxChange: (index: number) => void;
    }

    export class MessageViewModel {
     
        message: string;
       
        clearMessage() {
            this.message = "";
        }
    }

    export class ValueViewModel extends MessageViewModel {

        isDirty = () => false;

        originalValue : Value; 

        localFilter : ILocalFilter; 
        formattedValue: string;
        value: number | string | boolean | Date;    
        id: string; 
        argId: string; 
        paneArgId: string;
        choices: ChoiceViewModel[];
     
        type: string;
        reference: string;
        choice: ChoiceViewModel; 
        multiChoices: ChoiceViewModel[]; 
        returnType: string;
        title: string;
        format: string;
        arguments: _.Dictionary<Value>; 
        mask: string;
        
        minLength: number;
       
        color: string;
        description: string;
        optional: boolean;
        isCollectionContributed: boolean;
        onPaneId: number;

        multipleLines : number; 

        entryType : EntryType;

        prompt(searchTerm: string): ng.IPromise<ChoiceViewModel[]> {
            return null;
        }

        conditionalChoices(args: _.Dictionary<Value>): ng.IPromise<ChoiceViewModel[]> {
            return null;
        }

        getMemento(): string {
            if (this.entryType === EntryType.Choices) {
                return (this.choice && this.choice.search) ? this.choice.search : this.getValue().toString(); 
            }

            if (this.entryType === EntryType.MultipleChoices) {
                const ss = _.map(this.multiChoices, (c) => {
                    return c.search;
                });

                if (ss.length === 0) {
                    return "";
                }

                return _.reduce(ss, (m: string, s) => {
                    return m + "-" + s;
                });
            } 

            return this.getValue().toString();
        }

        setNewValue(newValue: IDraggableViewModel) {
            this.value = newValue.value;
            this.reference = newValue.reference;
            this.choice = newValue.choice;
            this.color = newValue.color;
        }

        drop : (newValue: IDraggableViewModel) => void;

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
                return new Value(this.choice && this.choice.value ? { href: this.choice.value, title: this.choice.name } : null );
            }

            if (this.type === "scalar") {
                if (this.value == null) {
                    return new Value("");
                }

                if (this.value instanceof Date) {
                
                    if (this.format === "date") {
                        // truncate time;
                        return new Value(toDateString(this.value as Date));
                    }
                    // date-time
                    return new Value((this.value as Date).toISOString());
                }

                return new Value(this.value as number | string | boolean);
            }

            // reference
            return new Value(this.reference ? { href: this.reference, title : this.value.toString() } : null);
        }
    }

    export class ParameterViewModel extends ValueViewModel{
        parameterRep : Parameter;
        dflt: string;     
    } 

    export class ActionViewModel {
        actionRep : ActionMember;
        menuPath : string;
        title: string;
        description: string;
        doInvoke: (right?: boolean) => void;
        executeInvoke: (pps : ParameterViewModel[], right?: boolean)  => ng.IPromise<ActionResultRepresentation>;
        disabled(): boolean { return false; }

        parameters: ParameterViewModel[];
        stopWatchingParms: () => void;
    } 

    export class DialogViewModel extends MessageViewModel {
        constructor(private color: IColor,
            private context: IContext,
            private viewModelFactory: IViewModelFactory,
            private urlManager: IUrlManager,
            private focusManager: IFocusManager) {
            super();
        }

        reset(actionViewModel: ActionViewModel, routeData : PaneRouteData) {
            this.actionMember = actionViewModel.actionRep;
            this.actionViewModel = actionViewModel;
            this.onPaneId = routeData.paneId;
         
            const fields = routeData.dialogFields;
            const parameters = _.filter(actionViewModel.parameters, p => !p.isCollectionContributed);
            this.parameters = _.map(parameters, p => this.viewModelFactory.parameterViewModel(p.parameterRep, fields[p.parameterRep.id()], this.onPaneId));

            this.title = this.actionMember.extensions().friendlyName();
            this.isQueryOnly = this.actionMember.invokeLink().method() === "GET";
            this.message = "";
            return this;
        }


        title: string;
        message: string;
        isQueryOnly: boolean;
        onPaneId: number;

        actionMember : ActionMember;
        actionViewModel: ActionViewModel;

        setParms = () => _.forEach(this.parameters, p => this.urlManager.setFieldValue(this.actionMember.actionId(), p.parameterRep, p.getValue(), false, this.onPaneId));

        private executeInvoke = (right?: boolean) => {

            const pps = this.parameters;
            _.forEach(pps, p => this.urlManager.setFieldValue(this.actionMember.actionId(), p.parameterRep, p.getValue(), false, this.onPaneId));
            return this.actionViewModel.executeInvoke(pps, right);
        }

        doInvoke = (right?: boolean) =>
            this.executeInvoke(right).
                then((result: ActionResultRepresentation) => {
                    if (result.result().isNull() && result.resultType() !== "void") {
                        this.message = "no result found";
                    } else if (result.resultType() === "void" ||  !right) {
                        // leave open if opening on other pane and dialog has result
                        this.doClose();
                    }
                }).
                catch((reject: ErrorWrapper) => {
                    const parent = this.actionMember.parent as DomainObjectRepresentation;
                    const display = (em: ErrorMap) => this.viewModelFactory.handleErrorResponse(em, this, this.parameters);
                    this.context.handleWrappedError(reject, parent, () => { }, display);
                });

        doClose = () => {
            this.urlManager.closeDialog(this.onPaneId);
        };

  
        clearMessages = () => {
            this.message = "";
            _.each(this.actionViewModel.parameters, parm => parm.clearMessage());
        }

        isSame(paneId : number, otherAction : ActionMember ) {
            return this.onPaneId === paneId && this.actionMember.invokeLink().href() === otherAction.invokeLink().href();
        }

        parameters: ParameterViewModel[];
    } 
    
    export class PropertyViewModel extends ValueViewModel implements IDraggableViewModel {

        propertyRep : PropertyMember;
        target: string;
        isEditable: boolean;
        attachment: AttachmentViewModel;
        draggableType: string;
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

        reset(list : ListRepresentation, routeData : PaneRouteData) {
            this.listRep = list;
            this.routeData = routeData; 

            const links = list.value();
            this.state = routeData.state;

            this.id = this.urlManager.getListCacheIndex(routeData.paneId, routeData.page, routeData.pageSize, routeData.state);

            this.onPaneId = routeData.paneId;

            this.pluralName = "Objects";
            this.items = this.viewModelFactory.getItems(links, this.state === CollectionViewState.Table, routeData, this);

            this.page = this.listRep.pagination().page;
            this.pageSize = this.listRep.pagination().pageSize;
            this.numPages = this.listRep.pagination().numPages;
            const totalCount = this.listRep.pagination().totalCount;
            const count = links.length;

            this.size = count;

            this.description = () => `Page ${this.page} of ${this.numPages}; viewing ${count} of ${totalCount} items`;

            const actions = this.listRep.actionMembers();
            this.actions = _.map(actions, action => this.viewModelFactory.actionViewModel(action, this, routeData));
            this.actionsMap = createActionMenuMap(this.actions);

            _.forEach(this.actions, a => {

                const wrappedInvoke = a.executeInvoke;
                a.executeInvoke = (pps: ParameterViewModel[], right?: boolean) => {
                    const selected = _.filter(this.items, i => i.selected);

                    if (selected.length === 0) {
               
                        const em = new ErrorMap({}, 0, "Must select items for collection contributed action");
                        const rp = new ErrorWrapper(ErrorCategory.HttpClientError, HttpStatusCode.UnprocessableEntity, em);

                        return this.$q.reject(rp);
                    }
                    const parms = _.values(a.actionRep.parameters()) as Parameter[];
                    const contribParm = _.find(parms, p => p.isCollectionContributed());
                    const parmValue = new Value(_.map(selected, i => i.link));
                    const collectionParmVm = this.viewModelFactory.parameterViewModel(contribParm, parmValue, this.onPaneId);

                    const allpps = _.clone(pps);
                    allpps.push(collectionParmVm);

                    return wrappedInvoke(allpps, right);
                }

                a.doInvoke = _.keys(a.actionRep.parameters()).length > 1 ?
                    (right?: boolean) => {
                        this.focusManager.focusOverrideOff();
                        this.urlManager.setDialog(a.actionRep.actionId(), this.onPaneId);
                    } :
                    (right?: boolean) => {
                        a.executeInvoke([], right).
                            then((result: ActionResultRepresentation) => {
                                if (result.result().isNull() && result.resultType() !== "void") {
                                    this.message = "no result found";
                                } else {
                                    this.message = "";
                                }
                            }).
                            catch((reject: ErrorWrapper) => {
                                const display = (em: ErrorMap) => this.message = em.invalidReason() || em.warningMessage;
                                this.contextService.handleWrappedError(reject, null, () => {}, display);
                            });
                    };
            });
            return this;
        }

        toggleActionMenu = () => {
            this.focusManager.focusOverrideOff();
            this.urlManager.toggleObjectMenu(this.onPaneId);
        };

        private recreate = (page: number, pageSize: number) => {
            return this.routeData.objectId ?
                this.contextService.getListFromObject(this.routeData.paneId, this.routeData.objectId, this.routeData.actionId, this.routeData.actionParams, page, pageSize) :
                this.contextService.getListFromMenu(this.routeData.paneId, this.routeData.menuId, this.routeData.actionId, this.routeData.actionParams, page, pageSize);
        }

        private pageOrRecreate = (newPage: number, newPageSize, newState?: CollectionViewState) => {
            this.recreate(newPage, newPageSize).
                then((list: ListRepresentation) => {
                    this.routeData.state = newState || this.routeData.state;
                    this.reset(list, this.routeData);
                    this.urlManager.setListPaging(newPage, newPageSize, this.routeData.state, this.onPaneId);
                }).
                catch((reject: ErrorWrapper) => {
                    const display = (em: ErrorMap) => this.message = em.invalidReason() || em.warningMessage;
                    this.contextService.handleWrappedError(reject, null, () => { }, display);
                });
        }


        listRep: ListRepresentation;
        routeData : PaneRouteData;

        //title: string;
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

        id: string;

        private setPage = (newPage: number, newState: CollectionViewState) => {
            this.focusManager.focusOverrideOff();
            this.pageOrRecreate(newPage, this.pageSize, newState);
        }

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

        description(): string { return this.size.toString() }

        template: string;

        disableActions(): boolean {
            return !this.actions || this.actions.length === 0;
        }

        actions: ActionViewModel[];
        actionsMap: { name: string; actions: ActionViewModel[] }[];

        isSame(paneId: number, key: string) {
            return  this.id === key;
        }
       
    } 

    export class CollectionViewModel {

        title: string;
        size: number;
        pluralName: string;
        color: string;
        items: ItemViewModel[];
        header: string[];
        onPaneId: number;

        id: string;

        doSummary(): void { }
        doTable(): void { }
        doList(): void { }

        description(): string { return this.size.toString() }

        template: string;

        actions: ActionViewModel[];
        actionsMap: { name: string; actions: ActionViewModel[] }[];
        messages: string;

        collectionRep: CollectionMember;
    } 

    export class ServicesViewModel {
        title: string; 
        color: string; 
        items: LinkViewModel[];       
    } 

    export class MenusViewModel {
        constructor(private viewModelFactory : IViewModelFactory) {
            
        }

        reset(menusRep : MenusRepresentation, routeData : PaneRouteData) {
            this.menusRep = menusRep;
            this.onPaneId = routeData.paneId;

            this.title = "Menus";
            this.color = "bg-color-darkBlue";
            this.items = _.map(this.menusRep.value(), link => this.viewModelFactory.linkViewModel(link, this.onPaneId));
            return this;
        }

        menusRep: MenusRepresentation;
        onPaneId : number;
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

    export class MenuViewModel extends MessageViewModel{
        title: string;
        actions: ActionViewModel[];
        actionsMap: { name: string; actions: ActionViewModel[] }[];
        color: string;
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
                    private focusManager: IFocusManager) {
            super();
        }

        propertyMap = () => {
            const pps = _.filter(this.properties, property => property.isEditable);
            return _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Value>;
        }

        reset(obj: DomainObjectRepresentation, routeData: PaneRouteData) {
            this.domainObject = obj;
            this.onPaneId = routeData.paneId;
            this.routeData = routeData;
            this.isInEdit = routeData.interactionMode !== InteractionMode.View || this.domainObject.extensions().renderInEdit();
            this.props = routeData.interactionMode !== InteractionMode.View ? routeData.props : {};

            const actions = _.values(this.domainObject.actionMembers()) as ActionMember[];
            this.actions = _.map(actions, action => this.viewModelFactory.actionViewModel(action, this, this.routeData));

            this.actionsMap = createActionMenuMap(this.actions);

            this.properties = _.map(this.domainObject.propertyMembers(), (property, id) => this.viewModelFactory.propertyViewModel(property, id, this.props[id], this.onPaneId, this.propertyMap));
            this.collections = _.map(this.domainObject.collectionMembers(), collection => this.viewModelFactory.collectionViewModel(collection, this.routeData));

            this.unsaved = routeData.interactionMode === InteractionMode.Transient || routeData.interactionMode === InteractionMode.Form;

            this.title = this.unsaved ? `Unsaved ${this.domainObject.extensions().friendlyName()}` : this.domainObject.title();
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
            }

            const sav = selfAsValue();

            this.value = sav ? sav.toString() : "";
            this.reference = sav ? sav.toValueString() : "";
            this.choice = sav ? ChoiceViewModel.create(sav, "") : null;
            this.color = this.colorService.toColorFromType(this.domainObject.domainType());
            this.message = "";
            

            if (routeData.interactionMode === InteractionMode.Form) {
                 _.forEach(this.actions, a => {
                     
                     const wrappedInvoke = a.executeInvoke;
                     a.executeInvoke = (pps: ParameterViewModel[], right?: boolean) => {
                         this.setProperties();
                         const pairs = _.map(this.editProperties(), p => [p.id, p.getValue()]);
                         const prps = (<any>_).fromPairs(pairs) as _.Dictionary<Value>;
                         
                         const parmValueMap = _.mapValues(a.actionRep.parameters(), p => ({ parm: p, value: prps[p.id()] }));
                         const allpps = _.map(parmValueMap, o => this.viewModelFactory.parameterViewModel(o.parm, o.value, this.onPaneId));
                         return wrappedInvoke(allpps, right);
                     }
                 });
             }

            return this;
        }

        routeData: PaneRouteData;
        domainObject: DomainObjectRepresentation;
        onPaneId: number;
        props: _.Dictionary<Value>;

        title: string;
        friendlyName : string;
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
        unsaved : boolean;

        toggleActionMenu = () => {
            this.focusManager.focusOverrideOff();
            this.urlManager.toggleObjectMenu(this.onPaneId);
        };

        private editProperties = () => _.filter(this.properties, p => p.isEditable && p.isDirty());
        public setProperties = () =>
            _.forEach(this.editProperties(), p => this.urlManager.setPropertyValue(this.domainObject, p.propertyRep, p.getValue(), false, this.onPaneId));

        private cancelHandler = () => this.domainObject.extensions().renderInEdit() ?
            () => this.urlManager.popUrlState(this.onPaneId) :
            () => this.urlManager.setInteractionMode(InteractionMode.View, this.onPaneId);

        editComplete = () => {
            this.setProperties();
        };

        doEditCancel = () => {
            this.editComplete();
            this.cancelHandler()();
        };

        private saveHandler = () => this.domainObject.isTransient() ? this.contextService.saveObject : this.contextService.updateObject;

        private handleWrappedError = (reject: ErrorWrapper) => {
            const reset = (updatedObject: DomainObjectRepresentation) => this.reset(updatedObject, this.urlManager.getRouteData().pane()[this.onPaneId]);
            const display = (em: ErrorMap) => this.viewModelFactory.handleErrorResponse(em, this, this.properties);
            this.contextService.handleWrappedError(reject, this.domainObject, reset, display);
        };

     



        doSave = viewObject => {

            this.setProperties();
            const propMap = this.propertyMap();

            this.saveHandler()(this.domainObject, propMap, this.onPaneId, viewObject).
                catch((reject: ErrorWrapper) =>  this.handleWrappedError(reject));
        };


        doEdit = () => {
            this.contextService.reloadObject(this.onPaneId, this.domainObject).
                then((updatedObject: DomainObjectRepresentation) => {
                    this.reset(updatedObject, this.urlManager.getRouteData().pane()[this.onPaneId]);
                    this.urlManager.pushUrlState(this.onPaneId);
                    this.urlManager.setInteractionMode(InteractionMode.Edit, this.onPaneId);
                }).
                catch((reject: ErrorWrapper) => this.handleWrappedError(reject));
        }

        doReload = () =>
            this.contextService.reloadObject(this.onPaneId, this.domainObject).
                then((updatedObject: DomainObjectRepresentation) => {
                    this.reset(updatedObject, this.urlManager.getRouteData().pane()[this.onPaneId]);
                }).
                catch((reject: ErrorWrapper) => this.handleWrappedError(reject));


        hideEdit = () => this.domainObject.extensions().renderInEdit() || _.every(this.properties, p => !p.isEditable);

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
        singlePane: (right?: boolean) => void;
        cicero: () => void;

        warnings: string[];
        messages: string[];
    }

    export class CiceroViewModel {
        message: string;
        output: string;
        alert: string = ""; //Alert is appended before the output
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
    }
}