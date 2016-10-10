/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.userMessages.config.ts" />


namespace NakedObjects {

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
    import IVersionRepresentation = RoInterfaces.IVersionRepresentation;
    import IUserRepresentation = RoInterfaces.IUserRepresentation;
    import Extensions = Models.Extensions;

    function tooltip(onWhat: { clientValid: () => boolean }, fields: IFieldViewModel[]): string {
        if (onWhat.clientValid()) {
            return "";
        }

        const missingMandatoryFields = _.filter(fields, p => !p.clientValid && !p.getMessage());

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

            const submenuActions = _.filter(avms, (a : IActionViewModel) => getMenuForLevel(a.menuPath, level) === menu.name && getMenuForLevel(a.menuPath, level + 1));
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
        private wrapped: Value;

        static create(value: Value, id: string, name?: string, searchTerm?: string) {
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

        equals(other: IChoiceViewModel) : boolean {
            return other instanceof ChoiceViewModel &&
                this.id === other.id &&
                this.name === other.name &&
                this.wrapped.toValueString() === other.wrapped.toValueString();
        }

        valuesEqual(other: IChoiceViewModel) : boolean {
           
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

    export class ErrorViewModel implements IErrorViewModel{
        originalError : ErrorWrapper;
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
        link: Link;

        doClick: (right?: boolean) => void;

        // IDraggableViewModel 
        color: string;
        value: scalarValueType;
        reference: string;
        selectedChoice: IChoiceViewModel;
        draggableType: string;
        canDropOn: (targetType: string) => ng.IPromise<boolean>;
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

        constructor(ext: Extensions, color : IColor, error : IError) {
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
        format: formatType;
        multipleLines: number;
        password: boolean;

        clientValid = true;
      
        type: "scalar" | "ref";
        reference: string = "";
        minLength: number;

        color: string;
        
        isCollectionContributed: boolean;
          
        promptArguments: _.Dictionary<Value>;
         
        currentValue: Value;
        originalValue: Value;

        localFilter: ILocalFilter;
        formattedValue: string;    
      
        choices: IChoiceViewModel[] = [];  

        private currentChoice: IChoiceViewModel;
        private currentMultipleChoices: IChoiceViewModel[];

        private error : IError;

        get selectedChoice(): IChoiceViewModel {
            return this.currentChoice;
        }

        set selectedChoice(newChoice: IChoiceViewModel) {
            // type guard because angular pushes string value here until directive finds 
            // choice
            if (newChoice instanceof ChoiceViewModel || newChoice == null) {
                this.currentChoice = newChoice;
                this.update();
            }
        }

        private currentRawValue: scalarValueType | Date;

        get value(): scalarValueType | Date {
            return this.currentRawValue;
        }

        set value(newValue: scalarValueType | Date) {
            this.currentRawValue = newValue;
            this.update();
        }

        get selectedMultiChoices(): IChoiceViewModel[] {
            return this.currentMultipleChoices;
        }

        set selectedMultiChoices(choices : IChoiceViewModel[]) {
            this.currentMultipleChoices = choices; 
            this.update();
        }

        private file: Link;     

        entryType: EntryType;

        validate: (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;

        refresh: (newValue: Value) => void;

        prompt : (searchTerm: string)=> ng.IPromise<ChoiceViewModel[]>;

        conditionalChoices : (args: _.Dictionary<Value>) => ng.IPromise<ChoiceViewModel[]>;

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

        protected update() {
            this.updateColor();
        }

        private setColor(color: IColor) {

            if (this.entryType === EntryType.AutoComplete && this.selectedChoice && this.type === "ref") {
                const href = this.selectedChoice.getValue().href();
                if (href) {
                    color.toColorNumberFromHref(href).
                        then(c => this.color = `${linkColor}${c}`).
                        catch((reject: ErrorWrapper) => this.error.handleError(reject));
                    return;
                }
            }
            else if (this.entryType !== EntryType.AutoComplete && this.value) {
                color.toColorNumberFromType(this.returnType).
                    then(c => this.color = `${linkColor}${c}`).
                    catch((reject: ErrorWrapper) => this.error.handleError(reject));
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
                    const selections = this.selectedMultiChoices || [];
                    if (this.type === "scalar") {
                        const selValues = _.map(selections, cvm => cvm.getValue().scalar());
                        return new Value(selValues);
                    }
                    const selRefs = _.map(selections, cvm => ({ href: cvm.getValue().href(), title: cvm.name })); // reference 
                    return new Value(selRefs);
                }

                const choiceValue = this.selectedChoice ? this.selectedChoice.getValue() : null;
                if (this.type === "scalar") {                
                    return new Value(choiceValue && choiceValue.scalar() != null ? choiceValue.scalar() : "");
                }

                // reference 
                return new Value(choiceValue && choiceValue.isReference() ? { href: choiceValue.href(), title: this.selectedChoice.name } : null);
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

        constructor(private parmRep: Parameter, paneId : number, color : IColor, error : IError) {
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
        }

        parameterRep: Parameter;
        dflt: string;

        setAsRow(i: number) {
            this.paneArgId = `${this.argId}${i}`;
        }

        protected update() {
            super.update();

            switch (this.entryType) {
                case (EntryType.FreeForm):
                    if (this.type === "scalar") {
                        if (this.localFilter) {
                            this.formattedValue = this.value ? this.localFilter.filter(this.value) : "";
                        } else {
                            this.formattedValue = this.value ? this.value.toString() : "";
                        }
                        break;
                    }
                    // fall through 
                case (EntryType.AutoComplete):
                case (EntryType.Choices):
                case (EntryType.ConditionalChoices):
                    this.formattedValue = this.selectedChoice ? this.selectedChoice.toString() : "";
                    break;
                case (EntryType.MultipleChoices):
                case (EntryType.MultipleConditionalChoices):
                    const count = !this.selectedMultiChoices ? 0 : this.selectedMultiChoices.length;
                    this.formattedValue = `${count} selected`;
                    break;
                default:
                    this.formattedValue = this.value ? this.value.toString() : "";
            }
        }
    }

    export class ActionViewModel implements IActionViewModel {
        actionRep: ActionMember | ActionRepresentation;
        invokableActionRep: IInvokableAction;

        menuPath: string;
        title: string;
        description: string;
        presentationHint: string;
        gotoResult = true;

        doInvoke: (right?: boolean) => void;
        execute: (pps: IParameterViewModel[], right?: boolean) => ng.IPromise<ActionResultRepresentation>;
        disabled : () => boolean;
        parameters: () => IParameterViewModel[];
        makeInvokable: (details: IInvokableAction) => void;
    }

    export class MenuItemViewModel implements IMenuItemViewModel {
        constructor(public name: string,
                    public actions: IActionViewModel[],
                    public menuItems: IMenuItemViewModel[]) { }
    }

    export class DialogViewModel extends MessageViewModel implements IDialogViewModel {
        constructor(private color: IColor,
            private context: IContext,
            private viewModelFactory: IViewModelFactory,
            private urlManager: IUrlManager,
            private focusManager: IFocusManager,
            private error: IError,
            private $rootScope: ng.IRootScopeService) {
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
        parameters: IParameterViewModel[] = [];
        submitted = false;

        reset(actionViewModel: IActionViewModel, paneId : number) {
            this.actionViewModel = actionViewModel;
            this.onPaneId = paneId;

            const fields = this.context.getCurrentDialogValues(this.actionMember().actionId(), this.onPaneId);

            const parameters = _.pickBy(actionViewModel.invokableActionRep.parameters(), p => !p.isCollectionContributed()) as _.Dictionary<Parameter>;
            this.parameters = _.map(parameters, p => this.viewModelFactory.parameterViewModel(p, fields[p.id()], this.onPaneId));

            this.title = this.actionMember().extensions().friendlyName();
            this.isQueryOnly = actionViewModel.invokableActionRep.invokeLink().method() === "GET";
            this.resetMessage();
            this.id = actionViewModel.actionRep.actionId();
            this.submitted = false;
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
                then((actionResult: ActionResultRepresentation) => {
                   
                    if (actionResult.shouldExpectResult()) {
                        this.setMessage(actionResult.warningsOrMessages() || noResultMessage);
                    } else if (actionResult.resultType() === "void") {
                        // dialog staying on same page so treat as cancel 
                        // for url replacing purposes
                        this.doCloseReplaceHistory();
                    }
                    else if (!this.isQueryOnly) {
                        // not query only - always close
                        this.doCloseReplaceHistory();
                    }
                    else if (!right) {
                        // query only going to new page close dialog and keep history
                        this.doCloseKeepHistory();
                    }
                    // else query only going to other tab leave dialog open
                    this.doCompleteButLeaveOpen();
                }).
                catch((reject: ErrorWrapper) => {
                    const display = (em: ErrorMap) => this.viewModelFactory.handleErrorResponse(em, this, this.parameters);
                    this.error.handleErrorAndDisplayMessages(reject, display);
                });

        doCloseKeepHistory = () => {
            this.deregister();
            this.urlManager.closeDialogKeepHistory(this.onPaneId);
        }

        doCloseReplaceHistory = () => {
            this.deregister();
            this.urlManager.closeDialogReplaceHistory(this.onPaneId);
        }

        doCompleteButLeaveOpen = () => {       
        }

        clearMessages = () => {
            this.resetMessage();
            _.each(this.actionViewModel.parameters, parm => parm.clearMessage());
        };

        matchingCollectionId: string;
    }

    export class MultiLineDialogViewModel implements IMultiLineDialogViewModel {

        private createRow(i : number) {
            const dialogViewModel = new DialogViewModel(this.color, this.context, this.viewModelFactory, this.urlManager, this.focusManager, this.error, this.$rootScope);
            const actionViewModel = this.viewModelFactory.actionViewModel(this.action, dialogViewModel, this.routeData);
            actionViewModel.gotoResult = false;

            dialogViewModel.reset(actionViewModel, 1);

            dialogViewModel.doCloseKeepHistory = () => {
                dialogViewModel.submitted = true;
            };

            dialogViewModel.doCloseReplaceHistory = () => {
                dialogViewModel.submitted = true;
            };

            dialogViewModel.doCompleteButLeaveOpen = () => {
                dialogViewModel.submitted = true;
            };


            dialogViewModel.parameters.forEach(p => p.setAsRow(i));


            return dialogViewModel;
        }

        objectFriendlyName: string; 
        objectTitle : string; 

        title: string = "";
        action: ActionMember | ActionRepresentation;
        routeData: PaneRouteData;

        constructor(private color: IColor,
            private context: IContext,
            private viewModelFactory: IViewModelFactory,
            private urlManager: IUrlManager,
            private focusManager: IFocusManager,
            private error: IError,
            private $rootScope: ng.IRootScopeService) {

        }

        reset(routeData: PaneRouteData, action: ActionMember | ActionRepresentation) {

            this.action = action;
            this.routeData = routeData;
            this.action.parent.etagDigest = "*";

            const initialCount = action.extensions().multipleLines() || 1;

            this.dialogs = _.map(_.range(initialCount), (i) => this.createRow(i));
            this.title = this.dialogs[0].title;
            return this;
        }

        dialogs: IDialogViewModel[] = [];

        header() {
            return this.dialogs.length === 0 ? [] : _.map(this.dialogs[0].parameters, p => p.title);
        }

        clientValid() {
            return _.every(this.dialogs, d => d.clientValid());
        }

        tooltip() {
            const tooltips =   _.map(this.dialogs, (d, i) =>  `row: ${i} ${d.tooltip() || "OK"}`);
            return _.reduce(tooltips, (s, t) => `${s}\n${t}`);
        }

        invokeAndAdd(index: number) {
            this.dialogs[index].doInvoke();
            this.add(index);     
            this.focusManager.focusOn(FocusTarget.MultiLineDialogRow, 1, 1); 
        }

        add(index : number) {
            if (index === this.dialogs.length - 1) {
                // if this is last dialog always add another
                this.dialogs.push(this.createRow(this.dialogs.length));
            }
            else if (_.takeRight(this.dialogs)[0].submitted) {
                // if the last dialog is submitted add another 
                this.dialogs.push(this.createRow(this.dialogs.length));
            }
        }

        clear(index : number) {
            _.pullAt(this.dialogs, [index]);
        }

        submitAll() {
            if (this.clientValid()) {
                _.each(this.dialogs, d => {
                    if (!d.submitted) {
                        d.doInvoke();
                    }
                });
            }
        }

        submittedCount() {
            return (_.filter(this.dialogs, d => d.submitted)).length;
        }

        close() {
            this.urlManager.popUrlState();
        }
    }

    export class PropertyViewModel extends ValueViewModel implements IPropertyViewModel,  IDraggableViewModel {

        constructor(propertyRep: PropertyMember, color : IColor, error : IError) {
            super(propertyRep.extensions(), color, error);
            this.draggableType = propertyRep.extensions().returnType();

            this.propertyRep = propertyRep;
            this.entryType = propertyRep.entryType();
            this.isEditable = !propertyRep.disabledReason();
            this.entryType = propertyRep.entryType();
        }


        propertyRep: PropertyMember;
        isEditable: boolean;
        attachment: IAttachmentViewModel;
        refType: "null" | "navigable" | "notNavigable";
        isDirty: () => boolean;
        doClick: (right?: boolean) => void;

        // IDraggableViewModel
        draggableType: string;
        canDropOn: (targetType: string) => ng.IPromise<boolean>;  
   }

    export class CollectionPlaceholderViewModel implements ICollectionPlaceholderViewModel {
        description: () => string;
        reload: () => void;
    }

    abstract class ContributedActionParentViewModel extends MessageViewModel {

        constructor(protected context: IContext,
            protected viewModelFactory: IViewModelFactory,
            protected urlManager: IUrlManager,
            protected focusManager: IFocusManager,
            protected error: IError,
            protected $q: ng.IQService) {
            super();
        }

        onPaneId : number;
        allSelected: boolean;
        items: IItemViewModel[];

        private isLocallyContributed(action: IInvokableAction) {
            return _.some(action.parameters(), p => p.isCollectionContributed()); 
        }

        protected collectionContributedActionDecorator(actionViewModel: IActionViewModel) {
            const wrappedInvoke = actionViewModel.execute;
            actionViewModel.execute = (pps: IParameterViewModel[], right?: boolean) => {

                const selected = _.filter(this.items, i => i.selected);
             

                const rejectAsNeedSelection = (action: IInvokableAction) => {
                    if (this.isLocallyContributed(action)) {

                        if (selected.length === 0) {

                            const em = new ErrorMap({}, 0, noItemsSelected);
                            const rp = new ErrorWrapper(ErrorCategory.HttpClientError, HttpStatusCode.UnprocessableEntity, em);

                            return rp;
                        }
                    }
                    return null;
                }


                const getParms = (action: IInvokableAction) => {

                    const parms = _.values(action.parameters()) as Parameter[];
                    const contribParm = _.find(parms, p => p.isCollectionContributed());

                    if (contribParm) {
                        const parmValue = new Value(_.map(selected, i => i.link));
                        const collectionParmVm = this.viewModelFactory.parameterViewModel(contribParm, parmValue, this.onPaneId);

                        const allpps = _.clone(pps);
                        allpps.push(collectionParmVm);
                        return allpps;
                    }
                    return pps;
                }

                if (actionViewModel.invokableActionRep) {
                    const rp = rejectAsNeedSelection(actionViewModel.invokableActionRep);
                    return rp ? this.$q.reject(rp) : wrappedInvoke(getParms(actionViewModel.invokableActionRep), right).
                        then(result => {
                            // clear selected items on void actions 
                            this.clearSelected(result);
                            return result;
                        });
                }

                return this.context.getActionDetails(actionViewModel.actionRep as ActionMember).
                    then((details: ActionRepresentation) => {
                        const rp = rejectAsNeedSelection(details);
                        if (rp) {
                            return this.$q.reject(rp);
                        }
                        return wrappedInvoke(getParms(details), right);
                    }).
                    then(result => {
                        // clear selected items on void actions 
                        this.clearSelected(result);
                        return result;
                    });
            }
        }

        protected collectionContributedInvokeDecorator(actionViewModel: IActionViewModel) {

            const showDialog = () =>
                this.context.getInvokableAction(actionViewModel.actionRep as ActionMember).
                    then(invokableAction => {
                    const keyCount = _.keys(invokableAction.parameters()).length;
                    return keyCount > 1 || keyCount === 1 && !_.toArray(invokableAction.parameters())[0].isCollectionContributed();
                });

            // make sure not null while waiting for promise to assign correct function 
            actionViewModel.doInvoke = () => { };

            const invokeWithDialog = (right?: boolean) => {
                this.context.clearDialogValues(this.onPaneId);
                this.focusManager.focusOverrideOff();
                this.urlManager.setDialogOrMultiLineDialog(actionViewModel.actionRep, this.onPaneId);
            };

            const invokeWithoutDialog = (right?: boolean) =>
                actionViewModel.execute([], right).
                    then(result => {
                        this.setMessage(result.shouldExpectResult() ? result.warningsOrMessages() || noResultMessage : "");
                        // clear selected items on void actions 
                        this.clearSelected(result);
                    }).
                    catch((reject: ErrorWrapper) => {
                        const display = (em: ErrorMap) => this.setMessage(em.invalidReason() || em.warningMessage);
                        this.error.handleErrorAndDisplayMessages(reject, display);
                    });

            showDialog().
                then(show => actionViewModel.doInvoke = show ? invokeWithDialog : invokeWithoutDialog).
                catch((reject: ErrorWrapper) => this.error.handleError(reject));
        }

        protected decorate(actionViewModel: IActionViewModel) {
            this.collectionContributedActionDecorator(actionViewModel);
            this.collectionContributedInvokeDecorator(actionViewModel);
        }

        protected clearSelected(result: ActionResultRepresentation) {
            if (result.resultType() === "void") {
                this.allSelected = false;
                this.selectAll();
            }
        }

        selectAll = () => _.each(this.items, (item, i) => {
            item.selected = this.allSelected;
            item.selectionChange(i);
        });
    }


    export class ListViewModel extends ContributedActionParentViewModel implements IListViewModel {

        constructor(private colorService: IColor,
             context: IContext,
             viewModelFactory: IViewModelFactory,
             urlManager: IUrlManager,
             focusManager: IFocusManager,
             error: IError,
             $q: ng.IQService) {
            super(context, viewModelFactory, urlManager, focusManager, error, $q);
        }

        private routeData: PaneRouteData;
        
        private page: number;
        private pageSize: number;
        private numPages: number;
        private state: CollectionViewState;  

        id: string;
        listRep: ListRepresentation;
        size: number;
        pluralName: string;    
        header: string[];        
       
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
                then((list: ListRepresentation) => {
                    this.urlManager.setListPaging(newPage, newPageSize, newState || this.routeData.state, this.onPaneId);
                    this.routeData = this.urlManager.getRouteData().pane()[this.onPaneId];
                    this.reset(list, this.routeData);
                }).
                catch((reject: ErrorWrapper) => {
                    const display = (em: ErrorMap) => this.setMessage(em.invalidReason() || em.warningMessage);
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

        private updateItems(value: Link[]) {
            this.items = this.viewModelFactory.getItems(value,
                this.state === CollectionViewState.Table,
                this.routeData,
                this);

            const totalCount = this.listRep.pagination().totalCount;
            this.allSelected = _.every(this.items, item => item.selected);
            const count = this.items.length;
            this.size = count;
            if (count > 0) {
                this.description = () => pageMessage(this.page, this.numPages, count, totalCount);
            } else {
                this.description = () => noItemsFound;
            }
        }

        private hasTableData = () => {
            const valueLinks = this.listRep.value();
            return valueLinks && _.some(valueLinks, (i: Link) => i.members());
        }

      
        refresh(routeData: PaneRouteData) {

            this.routeData = routeData;
            if (this.state !== routeData.state) {
                this.state = routeData.state;
                if (this.state === CollectionViewState.Table && !this.hasTableData()) {
                    this.recreate(this.page, this.pageSize).
                        then(list => {
                            this.listRep = list;
                            this.updateItems(list.value());
                        }).
                        catch((reject: ErrorWrapper) => {
                            this.error.handleError(reject);
                        });
                } else {
                    this.updateItems(this.listRep.value());
                }
            }
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
            this.allSelected = false;
            this.selectAll();
            this.context.clearCachedList(this.onPaneId, this.routeData.page, this.routeData.pageSize);
            this.setPage(this.page, this.state);
        };

        disableActions = () => !this.actions || this.actions.length === 0 || !this.items || this.items.length === 0;
        
        actionsTooltip = () => actionsTooltip(this, !!this.routeData.actionsOpen);

        actionMember = (id: string) => {
            const actionViewModel = _.find(this.actions, a => a.actionRep.actionId() === id);
            return actionViewModel.actionRep;
        }
    }

    export class CollectionViewModel extends ContributedActionParentViewModel implements ICollectionViewModel  {

        id : string;
        title: string;
        details: string;
        pluralName: string;
        color: string;
        mayHaveItems: boolean;
        editing: boolean;
        
        header: string[];
     
        currentState: CollectionViewState;
        presentationHint: string;
        template: string;
        actions: IActionViewModel[];
        menuItems: IMenuItemViewModel[];
        messages: string;
        collectionRep: CollectionMember | CollectionRepresentation;
       
        doSummary: () => void;
        doTable: () => void;
        doList: () => void;

        description = () => this.details.toString();      
        refresh: (routeData: PaneRouteData, resetting: boolean) => void;
        
        disableActions = () => this.editing || !this.actions || this.actions.length === 0;

        actionMember = (id: string) => {
            const actionViewModel = _.find(this.actions, a => a.actionRep.actionId() === id);
            return actionViewModel ? actionViewModel.actionRep : null;
        }

        setActions(actions: _.Dictionary<ActionMember>, routeData : PaneRouteData) {
            this.actions = _.map(actions, action => this.viewModelFactory.actionViewModel(action, this, routeData));
            this.menuItems = createMenuItems(this.actions);
            _.forEach(this.actions, a => this.decorate(a));
        }

        hasMatchingLocallyContributedAction(id: string) {
            return id && this.actions && this.actions.length > 0 && !!this.actionMember(id);
        }

       
    }

    export class MenusViewModel implements IMenusViewModel {
        constructor(private viewModelFactory: IViewModelFactory) {}

        reset(menusRep: MenusRepresentation, routeData: PaneRouteData) {
            this.menusRep = menusRep;
            this.onPaneId = routeData.paneId;
            this.items = _.map(this.menusRep.value(), link => this.viewModelFactory.linkViewModel(link, this.onPaneId));
            return this;
        }

        menusRep: MenusRepresentation;
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
        value: scalarValueType | Date;
        formattedValue: string;
        title: string;
        id: string;
    }

    export class PlaceHolderTableRowColumnViewModel implements ITableRowColumnViewModel {

        constructor(public id: string) { }

        type: "scalar";
        returnType: "string";
        value: "";
        formattedValue: "";
        title: "";
    }


    export class TableRowViewModel implements ITableRowViewModel {
        title: string;
        hasTitle: boolean;
        properties: ITableRowColumnViewModel[];

        conformColumns(columns: string[]) {
            if (columns) {
                this.properties =
                    _.map(columns, c => _.find(this.properties, tp => tp.id === c) || new PlaceHolderTableRowColumnViewModel(c));
            }
        }
    }

    export class ApplicationPropertiesViewModel implements IApplicationPropertiesViewModel {
        serverVersion: IVersionRepresentation;
        user: IUserRepresentation;
        serverUrl: string;
        clientVersion: string;
    }

    export class ToolBarViewModel implements IToolBarViewModel{
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

    export class RecentItemsViewModel implements IRecentItemsViewModel{
        onPaneId: number;
        items: IRecentItemViewModel[];
    }

    export class DomainObjectViewModel extends MessageViewModel implements IDomainObjectViewModel, IDraggableViewModel {

        constructor(private colorService: IColor,
            private contextService: IContext,
            private viewModelFactory: IViewModelFactory,
            private urlManager: IUrlManager,
            private focusManager: IFocusManager,
            private error: IError,
            private $q: ng.IQService) {
            super();
        }

        private routeData: PaneRouteData;
        private props: _.Dictionary<Value>;
        private instanceId: string;
        private unsaved: boolean;

        // IDraggableViewModel
        value: string;
        reference: string;
        selectedChoice: IChoiceViewModel;
        color: string;
        draggableType: string;

        domainObject: DomainObjectRepresentation;
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

        private handleWrappedError = (reject: ErrorWrapper) => {
            const display = (em: ErrorMap) => this.viewModelFactory.handleErrorResponse(em, this, this.properties);
            this.error.handleErrorAndDisplayMessages(reject, display);
        };

        private propertyMap = () => {
            const pps = _.filter(this.properties, property => property.isEditable);
            return _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Value>;
        };

        private wrapAction(a: IActionViewModel) {
            const wrappedInvoke = a.execute;
            a.execute = (pps: IParameterViewModel[], right?: boolean) => {
                this.setProperties();
                const pairs = _.map(this.editProperties(), p => [p.id, p.getValue()]);
                const prps = _.fromPairs(pairs) as _.Dictionary<Value>;

                const parmValueMap = _.mapValues(a.invokableActionRep.parameters(), p => ({ parm: p, value: prps[p.id()] }));
                const allpps = _.map(parmValueMap, o => this.viewModelFactory.parameterViewModel(o.parm, o.value, this.onPaneId));
                return wrappedInvoke(allpps, right).
                    catch((reject: ErrorWrapper) => {
                        this.handleWrappedError(reject);
                        return this.$q.reject(reject);
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

            this.title = this.title + dirtyMarker(this.contextService, this.domainObject.getOid());

            if (routeData.interactionMode === InteractionMode.Form) {
                _.forEach(this.actions, a => this.wrapAction(a));
            }

            // leave message from previous refresh 
            this.clearMessage();
        }

        reset(obj: DomainObjectRepresentation, routeData: PaneRouteData) : IDomainObjectViewModel {
            this.domainObject = obj;
            this.onPaneId = routeData.paneId;
            this.routeData = routeData;
            const iMode = this.domainObject.extensions().interactionMode();
            this.isInEdit = routeData.interactionMode !== InteractionMode.View || iMode === "form" || iMode === "transient";
            this.props = routeData.interactionMode !== InteractionMode.View ? this.contextService.getCurrentObjectValues(this.domainObject.id(), routeData.paneId) : {};

            const actions = _.values(this.domainObject.actionMembers()) as ActionMember[];
            this.actions = _.map(actions, action => this.viewModelFactory.actionViewModel(action, this, this.routeData));

            this.menuItems = createMenuItems(this.actions);

            this.properties = _.map(this.domainObject.propertyMembers(), (property, id) => this.viewModelFactory.propertyViewModel(property, id, this.props[id], this.onPaneId, this.propertyMap));
            this.collections = _.map(this.domainObject.collectionMembers(), collection => this.viewModelFactory.collectionViewModel(collection, this.routeData));

            this.unsaved = routeData.interactionMode === InteractionMode.Transient;

            this.title = this.unsaved ? `Unsaved ${this.domainObject.extensions().friendlyName()}` : this.domainObject.title();

            this.title = this.title + dirtyMarker(this.contextService, obj.getOid());

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
                    return new Value(link);
                }
                return null;
            };
            const sav = selfAsValue();

            this.value = sav ? sav.toString() : "";
            this.reference = sav ? sav.toValueString() : "";
            this.selectedChoice = sav ? ChoiceViewModel.create(sav, "") : null;

            this.colorService.toColorNumberFromType(this.domainObject.domainType()).
                then(c => this.color = `${objectColor}${c}`).
                catch((reject: ErrorWrapper) => this.error.handleError(reject));

            this.resetMessage();

            if (routeData.interactionMode === InteractionMode.Form) {
                _.forEach(this.actions, a => this.wrapAction(a));
            }
            return this as IDomainObjectViewModel;
        }

        concurrency() {
            return (event: ng.IAngularEvent, em: ErrorMap) => {
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
                    catch((reject: ErrorWrapper) => this.error.handleError(reject));
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
            this.contextService.updateValues(); // for other panes
            this.clearCachedFiles();
            this.contextService.clearObjectValues(this.onPaneId);
            this.contextService.getObjectForEdit(this.onPaneId, this.domainObject).
                then(updatedObject => {
                    this.reset(updatedObject, this.urlManager.getRouteData().pane()[this.onPaneId]);
                    this.urlManager.pushUrlState(this.onPaneId);
                    this.urlManager.setInteractionMode(InteractionMode.Edit, this.onPaneId);
                }).
                catch((reject: ErrorWrapper) => this.handleWrappedError(reject));
        };

        doReload = () => {
            this.contextService.updateValues();
            this.clearCachedFiles();
            this.contextService.reloadObject(this.onPaneId, this.domainObject)
                .then(updatedObject => this.reset(updatedObject, this.urlManager.getRouteData().pane()[this.onPaneId]))
                .catch((reject: ErrorWrapper) => this.handleWrappedError(reject));
        }

        hideEdit = () => this.isFormOrTransient() || _.every(this.properties, p => !p.isEditable);

        disableActions = () => !this.actions || this.actions.length === 0;

        canDropOn = (targetType: string) => this.contextService.isSubTypeOf(this.domainType, targetType);
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
        applicationPropertiesTemplate: string;
        multiLineDialogTemplate: string;

        propertiesTemplate: string;
        parametersTemplate: string;
        readOnlyParameterTemplate: string;
        propertyTemplate: string;
        parameterTemplate: string;

        menus: IMenusViewModel;
        object: IDomainObjectViewModel;
        menu: IMenuViewModel;
        dialog: IDialogViewModel;
        error: IErrorViewModel;
        recent: IRecentItemsViewModel;
        collection: IListViewModel;
        collectionPlaceholder: ICollectionPlaceholderViewModel;
        toolBar: IToolBarViewModel;
        cicero: ICiceroViewModel;
        attachment: IAttachmentViewModel;
        applicationProperties: IApplicationPropertiesViewModel;
        multiLineDialog : IMultiLineDialogViewModel;
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