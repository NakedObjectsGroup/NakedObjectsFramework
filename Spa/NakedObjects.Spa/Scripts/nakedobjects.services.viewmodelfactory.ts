/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.userMessages.config.ts" />
/// <reference path="nakedobjects.app.ts" />

module NakedObjects {

    import ErrorWrapper = Models.ErrorWrapper;
    import ActionMember = Models.ActionMember;
    import CollectionMember = Models.CollectionMember;
    import DomainServicesRepresentation = Models.DomainServicesRepresentation;
    import DomainObjectRepresentation = Models.DomainObjectRepresentation;
    import MenuRepresentation = Models.MenuRepresentation;
    import Parameter = Models.Parameter;
    import Value = Models.Value;
    import PropertyMember = Models.PropertyMember;
    import ErrorMap = Models.ErrorMap;
    import Link = Models.Link;
    import ErrorCategory = Models.ErrorCategory;
    import HttpStatusCode = Models.HttpStatusCode;
    import EntryType = Models.EntryType;
    import Extensions = Models.Extensions;
    import ClientErrorCode = Models.ClientErrorCode;
    import ListRepresentation = Models.ListRepresentation;
    import ErrorRepresentation = Models.ErrorRepresentation;
    import IField = Models.IField;
    import IHasActions = Models.IHasActions;
    import TypePlusTitle = Models.typePlusTitle;
    import toUtcDate = Models.toUtcDate;
    import isDateOrDateTime = Models.isDateOrDateTime;
    import PlusTitle = Models.typePlusTitle;
    import Title = Models.typePlusTitle;
    import FriendlyNameForProperty = Models.friendlyNameForProperty;
    import FriendlyNameForParam = Models.friendlyNameForParam;
    import ActionRepresentation = Models.ActionRepresentation;
    import IInvokableAction = Models.IInvokableAction;
    import CollectionRepresentation = Models.CollectionRepresentation;
    import IHasExtensions = Models.IHasExtensions;
    import dirtyMarker = Models.dirtyMarker;
    import ObjectIdWrapper = Models.ObjectIdWrapper;
    import InvokableActionMember = Models.InvokableActionMember;
    import isTime = Models.isTime;
    import toTime = Models.toTime;

    export interface IViewModelFactory {
        toolBarViewModel(): ToolBarViewModel;
        errorViewModel(errorRep: ErrorWrapper): ErrorViewModel;
        actionViewModel(actionRep: ActionMember | ActionRepresentation, vm: MessageViewModel, routedata: PaneRouteData): ActionViewModel;
        collectionViewModel(collectionRep: CollectionMember, routeData: PaneRouteData): CollectionViewModel;
        listPlaceholderViewModel(routeData: PaneRouteData): CollectionPlaceholderViewModel;
        servicesViewModel(servicesRep: DomainServicesRepresentation): ServicesViewModel;
        serviceViewModel(serviceRep: DomainObjectRepresentation, routeData: PaneRouteData): ServiceViewModel;

        menuViewModel(menuRep: MenuRepresentation, routeData: PaneRouteData): MenuViewModel;

        tableRowViewModel(properties: _.Dictionary<PropertyMember>, paneId: number): TableRowViewModel;
        parameterViewModel(parmRep: Parameter, previousValue: Value, paneId: number): ParameterViewModel;
        propertyViewModel(propertyRep: PropertyMember, id: string, previousValue: Value, paneId: number, parentValues: () => _.Dictionary<Value>): PropertyViewModel;
        ciceroViewModel(): CiceroViewModel;
        handleErrorResponse(err: ErrorMap, vm: MessageViewModel, vms: ValueViewModel[]): void;
        getItems(links: Link[], tableView: boolean, routeData: PaneRouteData, collectionViewModel: CollectionViewModel | ListViewModel): ItemViewModel[];
        linkViewModel(linkRep: Link, paneId: number): LinkViewModel;
        recentItemsViewModel(paneId: number): RecentItemsViewModel;
        attachmentViewModel(propertyRep: PropertyMember, paneId: number): AttachmentViewModel;
    }

    interface IViewModelFactoryInternal extends IViewModelFactory {
        itemViewModel(linkRep: Link, paneId: number, selected: boolean): ItemViewModel;
        recentItemViewModel(obj: DomainObjectRepresentation, linkRep: Link, paneId: number, selected: boolean): RecentItemViewModel;
        propertyTableViewModel(propertyRep: PropertyMember, id: string, paneId: number): TableRowColumnViewModel;
    }

    app.service("viewModelFactory", function ($q: ng.IQService,
        $timeout: ng.ITimeoutService,
        $location: ng.ILocationService,
        $filter: ng.IFilterService,
        $cacheFactory: ng.ICacheFactoryService,
        repLoader: IRepLoader,
        color: IColor,
        context: IContext,
        mask: IMask,
        urlManager: IUrlManager,
        focusManager: IFocusManager,
        navigation: INavigation,
        clickHandler: IClickHandler,
        commandFactory: ICommandFactory,
        $rootScope: ng.IRootScopeService,
        $route: ng.route.IRouteService,
        $http: ng.IHttpService) {

        var viewModelFactory = <IViewModelFactoryInternal>this;

        viewModelFactory.errorViewModel = (error: ErrorWrapper) => {
            const errorViewModel = new ErrorViewModel();

            if (error) {
                errorViewModel.title = error.title;
                errorViewModel.description = error.description;
                errorViewModel.code = error.errorCode;
                errorViewModel.message = error.message;
                const stackTrace = error.stackTrace;

                errorViewModel.stackTrace = stackTrace && stackTrace.length !== 0 ? stackTrace : null;

                errorViewModel.isConcurrencyError =
                    error.category === ErrorCategory.HttpClientError &&
                    error.httpErrorCode === HttpStatusCode.PreconditionFailed;
            }

            errorViewModel.description = errorViewModel.description || "No description available";
            errorViewModel.code = errorViewModel.code || "No code available";
            errorViewModel.message = errorViewModel.message || "No message available";
            errorViewModel.stackTrace = errorViewModel.stackTrace || ["No stack trace available"];

            return errorViewModel;
        };


        function initLinkViewModel(linkViewModel: LinkViewModel, linkRep: Link) {
            linkViewModel.title = linkRep.title();

            color.toColorNumberFromHref(linkRep.href()).then((c: number) => linkViewModel.color = `${linkColor}${c}`);

            linkViewModel.link = linkRep;

            linkViewModel.domainType = linkRep.type().domainType;
            linkViewModel.draggableType = linkViewModel.domainType;

            // for dropping 
            const value = new Value(linkRep);

            linkViewModel.value = value.toString();
            linkViewModel.reference = value.toValueString();
            linkViewModel.choice = ChoiceViewModel.create(value, "");

            linkViewModel.canDropOn = (targetType: string) => context.isSubTypeOf(linkViewModel.domainType, targetType);

            linkViewModel.title = linkViewModel.title + dirtyMarker(context, linkRep.getOid());
        }

        const createChoiceViewModels = (id: string, searchTerm: string, choices: _.Dictionary<Value>) =>
            $q.when(_.map(choices, (v, k) => ChoiceViewModel.create(v, id, k, searchTerm)));

        viewModelFactory.attachmentViewModel = (propertyRep: PropertyMember, paneId: number) => {
            const parent = propertyRep.parent as DomainObjectRepresentation;
            const avm = AttachmentViewModel.create(propertyRep.attachmentLink(), parent, context, paneId);
            avm.doClick = (right?: boolean) => urlManager.setAttachment(avm.link, clickHandler.pane(paneId, right));

            return avm;
        };

        viewModelFactory.linkViewModel = (linkRep: Link, paneId: number) => {
            const linkViewModel = new LinkViewModel();
            linkViewModel.doClick = () => {
                // because may be clicking on menu already open so want to reset focus             
                urlManager.setMenu(linkRep.rel().parms[0].value, paneId);
                focusManager.setCurrentPane(paneId);
                focusManager.focusOverrideOff();
                focusManager.focusOn(FocusTarget.SubAction, 0, paneId);
            };
            initLinkViewModel(linkViewModel, linkRep);
            return linkViewModel;
        };

        viewModelFactory.itemViewModel = (linkRep: Link, paneId: number, selected: boolean) => {
            const itemViewModel = new ItemViewModel();
            itemViewModel.doClick = (right?: boolean) => {
                const currentPane = clickHandler.pane(paneId, right);
                focusManager.setCurrentPane(currentPane);
                urlManager.setItem(linkRep, currentPane);
            };
            initLinkViewModel(itemViewModel, linkRep);

            itemViewModel.selected = selected;

            itemViewModel.checkboxChange = (index) => {
                context.updateParms();
                urlManager.setListItem(index, itemViewModel.selected, paneId);
                focusManager.focusOverrideOn(FocusTarget.CheckBox, index + 1, paneId);
            };

            const members = linkRep.members();

            if (members) {
                itemViewModel.target = viewModelFactory.tableRowViewModel(members, paneId);
                itemViewModel.target.title = itemViewModel.title;
            }

            return itemViewModel;
        };

        viewModelFactory.recentItemViewModel = (obj: DomainObjectRepresentation, linkRep: Link, paneId: number, selected: boolean) => {
            const recentItemViewModel = viewModelFactory.itemViewModel(linkRep, paneId, selected) as RecentItemViewModel;
            recentItemViewModel.friendlyName = obj.extensions().friendlyName();
            return recentItemViewModel;
        };

        viewModelFactory.parameterViewModel = (parmRep: Parameter, previousValue: Value, paneId: number) => {
            const parmViewModel = new ParameterViewModel();

            parmViewModel.parameterRep = parmRep;
            parmViewModel.type = parmRep.isScalar() ? "scalar" : "ref";
            parmViewModel.dflt = parmRep.default().toString();
            parmViewModel.optional = parmRep.extensions().optional();
            let required = parmViewModel.optional ? "" : "* ";
            parmViewModel.description = parmRep.extensions().description();
            parmViewModel.setMessage("");
            parmViewModel.id = parmRep.id();
            parmViewModel.argId = `${parmViewModel.id.toLowerCase()}`;
            parmViewModel.paneArgId = `${parmViewModel.argId}${paneId}`;
            parmViewModel.reference = "";

            parmViewModel.mask = parmRep.extensions().mask();
            parmViewModel.title = parmRep.extensions().friendlyName();
            parmViewModel.returnType = parmRep.extensions().returnType();
            parmViewModel.format = parmRep.extensions().format();
            parmViewModel.isCollectionContributed = parmRep.isCollectionContributed();
            parmViewModel.onPaneId = paneId;

            parmViewModel.multipleLines = parmRep.extensions().multipleLines() || 1;
            parmViewModel.password = parmRep.extensions().dataType() === "password";
            parmViewModel.clientValid = true;


            const fieldEntryType = parmRep.entryType();
            parmViewModel.entryType = fieldEntryType;

            parmViewModel.choices = [];

            if (fieldEntryType === EntryType.Choices || fieldEntryType === EntryType.MultipleChoices) {
                parmViewModel.choices = _.map(parmRep.choices(), (v, n) => ChoiceViewModel.create(v, parmRep.id(), n));
            }

            if (fieldEntryType === EntryType.AutoComplete) {
                parmViewModel.prompt = (searchTerm: string) => {
                    const createcvm = _.partial(createChoiceViewModels, parmViewModel.id, searchTerm);
                    return context.autoComplete(parmRep, parmViewModel.id, () => <_.Dictionary<Value>>{}, searchTerm).
                        then(createcvm);
                };
                parmViewModel.minLength = parmRep.promptLink().extensions().minLength();
                parmViewModel.description = parmViewModel.description || autoCompletePrompt;
            }

            if (fieldEntryType === EntryType.FreeForm && parmViewModel.type === "ref") {
                parmViewModel.description = parmViewModel.description || dropPrompt;

                parmViewModel.refresh = (newValue: Value) => {

                    const val = newValue && !newValue.isNull() ? newValue : parmRep.default();

                    if (!val.isNull() && val.isReference()) {
                        parmViewModel.reference = val.link().href();
                        parmViewModel.choice = ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);
                    }
                    parmViewModel.setColor(color);
                }

                parmViewModel.refresh(previousValue);
            }

            if (fieldEntryType === EntryType.ConditionalChoices || fieldEntryType === EntryType.MultipleConditionalChoices) {
                parmViewModel.conditionalChoices = (args: _.Dictionary<Value>) => {
                    const createcvm = _.partial(createChoiceViewModels, parmViewModel.id, null);
                    return context.conditionalChoices(parmRep, parmViewModel.id, () => <_.Dictionary<Value>>{}, args).
                        then(createcvm);
                };
                // fromPairs definition faulty
                parmViewModel.arguments = (<any>_).fromPairs(_.map(parmRep.promptLink().arguments(), (v: any, key: string) => [key, new Value(v.value)]));
            }

            if (fieldEntryType !== EntryType.FreeForm || parmViewModel.isCollectionContributed) {

                function setCurrentChoices(vals: Value) {

                    const choicesToSet = _.map(vals.list(), val => ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null));

                    if (fieldEntryType === EntryType.MultipleChoices) {
                        parmViewModel.multiChoices = _.filter(parmViewModel.choices, c => _.some(choicesToSet, choiceToSet => c.match(choiceToSet)));
                    } else {
                        parmViewModel.multiChoices = choicesToSet;
                    }
                }

                function setCurrentChoice(val: Value) {
                    const choiceToSet = ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);

                    if (fieldEntryType === EntryType.Choices) {
                        parmViewModel.choice = _.find(parmViewModel.choices, c => c.match(choiceToSet));
                    } else {
                        parmViewModel.choice = choiceToSet;
                    }
                }

                parmViewModel.refresh = (newValue: Value) => {

                    if (newValue || parmViewModel.dflt) {
                        const toSet = newValue || parmRep.default();
                        if (fieldEntryType === EntryType.MultipleChoices ||
                            fieldEntryType === EntryType.MultipleConditionalChoices ||
                            parmViewModel.isCollectionContributed) {
                            setCurrentChoices(toSet);
                        } else {
                            setCurrentChoice(toSet);
                        }
                    }
                    parmViewModel.setColor(color);
                }

                parmViewModel.refresh(previousValue);

            } else {
                const returnType = parmRep.extensions().returnType();

                parmViewModel.refresh = (newValue: Value) => {

                    if (returnType === "boolean") {
                        const valueToSet = (newValue ? newValue.toValueString() : null) ||
                            parmRep.default().scalar();
                        let bValueToSet = toTriStateBoolean(valueToSet);

                        parmViewModel.value = bValueToSet;
                        if (bValueToSet !== null) {
                            // reset required indicator
                            required = "";
                        }

                    } else if (isDateOrDateTime(parmRep)) {
                        parmViewModel.value = toUtcDate(newValue || new Value(parmViewModel.dflt));
                    } else if (isTime(parmRep)) {
                        parmViewModel.value = toTime(newValue || new Value(parmViewModel.dflt));
                    } else {
                        parmViewModel.value = (newValue ? newValue.toString() : null) || parmViewModel.dflt || "";
                    }
                    parmViewModel.setColor(color);
                }

                parmViewModel.refresh(previousValue);
            }

            const remoteMask = parmRep.extensions().mask();

            if (remoteMask && parmRep.isScalar()) {
                const localFilter = mask.toLocalFilter(remoteMask, parmRep.extensions().format());
                parmViewModel.localFilter = localFilter;
                // formatting also happens in in directive - at least for dates - value is now date in that case
                parmViewModel.formattedValue = parmViewModel.value ? localFilter.filter(parmViewModel.value.toString()) : "";
            }

            parmViewModel.setColor(color);

            parmViewModel.validate = _.partial(validate, parmRep, parmViewModel) as (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;

            parmViewModel.drop = _.partial(drop, parmViewModel);

            parmViewModel.description = required + parmViewModel.description;

            parmViewModel.refresh = parmViewModel.refresh || ((newValue: Value) => { });

            return parmViewModel;
        };


        viewModelFactory.actionViewModel = (actionRep: ActionMember | ActionRepresentation, vm: MessageViewModel, routeData: PaneRouteData) => {
            const actionViewModel = new ActionViewModel();

            const parms = routeData.actionParams;
            const paneId = routeData.paneId;

            actionViewModel.actionRep = actionRep;

            if (actionRep instanceof ActionRepresentation || actionRep instanceof InvokableActionMember) {
                actionViewModel.invokableActionRep = actionRep;
            }

            actionViewModel.title = actionRep.extensions().friendlyName();
            actionViewModel.menuPath = actionRep.extensions().menuPath() || "";
            actionViewModel.disabled = () => !!actionRep.disabledReason();
            actionViewModel.description = actionViewModel.disabled() ? actionRep.disabledReason() : actionRep.extensions().description();

            actionViewModel.parameters = () => {
                // don't use actionRep directly as it may change and we've closed around the original value
                const parameters = _.pickBy(actionViewModel.invokableActionRep.parameters(), p => !p.isCollectionContributed()) as _.Dictionary<Parameter>;
                return _.map(parameters, parm => viewModelFactory.parameterViewModel(parm, parms[parm.id()], paneId));
            };

            actionViewModel.executeInvoke = (pps: ParameterViewModel[], right?: boolean) => {
                const parmMap = _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Value>;
                _.forEach(pps, p => urlManager.setParameterValue(actionRep.actionId(), p.parameterRep, p.getValue(), paneId));
                return context.getInvokableAction(actionViewModel.actionRep).then(details => context.invokeAction(details, clickHandler.pane(paneId, right), parmMap));
            };

            // form actions should never show dialogs
            const showDialog = () => actionRep.extensions().hasParams() && (routeData.interactionMode !== InteractionMode.Form);

            // open dialog on current pane always - invoke action goes to pane indicated by click
            actionViewModel.doInvoke = showDialog() ?
                (right?: boolean) => {
                    focusManager.setCurrentPane(paneId);
                    focusManager.focusOverrideOff();
                    // clear any previous dialog 
                    context.clearDialog(paneId);
                    urlManager.setDialog(actionRep.actionId(), paneId);
                    focusManager.focusOn(FocusTarget.Dialog, 0, paneId); // in case dialog is already open
                } :
                (right?: boolean) => {
                    focusManager.focusOverrideOff();
                    const pps = actionViewModel.parameters();
                    actionViewModel.executeInvoke(pps, right).
                        catch((reject: ErrorWrapper) => {
                            const parent = actionRep.parent as DomainObjectRepresentation;
                            const reset = (updatedObject: DomainObjectRepresentation) => this.reset(updatedObject, urlManager.getRouteData().pane()[this.onPaneId]);
                            const display = (em: ErrorMap) => vm.setMessage(em.invalidReason() || em.warningMessage);
                            context.handleWrappedError(reject, parent, reset, display);
                        });
                };

            actionViewModel.makeInvokable = (details: IInvokableAction) => actionViewModel.invokableActionRep = details;

            return actionViewModel;
        };


        viewModelFactory.handleErrorResponse = (err: ErrorMap, messageViewModel: MessageViewModel, valueViewModels: ValueViewModel[]) => {

            let requiredFieldsMissing = false; // only show warning message if we have nothing else 
            let fieldValidationErrors = false;

            _.each(valueViewModels, valueViewModel => {
                const errorValue = err.valuesMap()[valueViewModel.id];

                if (errorValue) {
                    const reason = errorValue.invalidReason;
                    if (reason) {
                        if (reason === "Mandatory") {
                            const r = "REQUIRED";
                            requiredFieldsMissing = true;
                            valueViewModel.description = valueViewModel.description.indexOf(r) === 0 ? valueViewModel.description : `${r} ${valueViewModel.description}`;
                        } else {
                            valueViewModel.setMessage(reason);
                            fieldValidationErrors = true;
                        }
                    }
                }
            });

            let msg = err.invalidReason() || "";
            if (requiredFieldsMissing) msg = `${msg} Please complete REQUIRED fields. `;
            if (fieldValidationErrors) msg = `${msg} See field validation message(s). `;

            if (!msg) msg = err.warningMessage;
            messageViewModel.setMessage(msg);
        };

        function drop(vm: ValueViewModel, newValue: IDraggableViewModel) {
            context.isSubTypeOf(newValue.draggableType, vm.returnType).
                then((canDrop: boolean) => {
                    if (canDrop) {
                        vm.setNewValue(newValue);
                    }
                });
        };

        function validate(rep: IHasExtensions, vm: ValueViewModel, modelValue: any, viewValue: string, mandatoryOnly: boolean) {
            const message = mandatoryOnly ? Models.validateMandatory(rep, viewValue) : Models.validate(rep, modelValue, viewValue, vm.localFilter);

            if (message !== mandatory) {
                vm.setMessage(message);
            }

            vm.clientValid = !message;
            return vm.clientValid;
        };

        function setupReference(vm: PropertyViewModel, value: Value, rep: IHasExtensions) {
            vm.type = "ref";
            if (value.isNull()) {
                vm.reference = "";
                vm.value = vm.description;
                vm.formattedValue = "";
                vm.refType = "null";
            } else {
                vm.reference = value.link().href();
                vm.value = value.toString();
                vm.formattedValue = value.toString();
                vm.refType = rep.extensions().notNavigable() ? "notNavigable" : "navigable";
            }
            if (vm.entryType === EntryType.FreeForm) {
                vm.description = vm.description || dropPrompt;
            }
        }

        viewModelFactory.propertyTableViewModel = (propertyRep: PropertyMember | CollectionMember, id: string, paneId: number) => {
            const tableRowColumnViewModel = new TableRowColumnViewModel();

            tableRowColumnViewModel.title = propertyRep.extensions().friendlyName();

            if (propertyRep instanceof CollectionMember) {
                const size = propertyRep.size();

                tableRowColumnViewModel.formattedValue = getCollectionCount(size);
                tableRowColumnViewModel.value = "";
                tableRowColumnViewModel.type = "scalar";
                tableRowColumnViewModel.returnType = "string";
            }

            if (propertyRep instanceof PropertyMember) {
                const isPassword = propertyRep.extensions().dataType() === "password";
                const value = propertyRep.value();
                tableRowColumnViewModel.returnType = propertyRep.extensions().returnType();

                if (propertyRep.isScalar()) {
                    if (isDateOrDateTime(propertyRep)) {
                        tableRowColumnViewModel.value = toUtcDate(value);
                    } else {
                        tableRowColumnViewModel.value = value.scalar();
                    }
                    tableRowColumnViewModel.type = "scalar";

                    const remoteMask = propertyRep.extensions().mask();
                    const localFilter = mask.toLocalFilter(remoteMask, propertyRep.extensions().format());

                    if (propertyRep.entryType() === EntryType.Choices) {
                        const currentChoice = ChoiceViewModel.create(value, id);
                        const choices = _.map(propertyRep.choices(), (v, n) => ChoiceViewModel.create(v, id, n));
                        const choice = _.find(choices, (c: ChoiceViewModel) => c.match(currentChoice));

                        if (choice) {
                            tableRowColumnViewModel.value = choice.name;
                            tableRowColumnViewModel.formattedValue = choice.name;
                        }
                    } else if (isPassword) {
                        tableRowColumnViewModel.formattedValue = obscuredText;
                    } else {
                        tableRowColumnViewModel.formattedValue = localFilter.filter(tableRowColumnViewModel.value);
                    }
                } else {
                    // is reference   
                    tableRowColumnViewModel.type = "ref";
                    tableRowColumnViewModel.formattedValue = value.isNull() ? "" : value.toString();
                }
            }

            return tableRowColumnViewModel;
        };

        viewModelFactory.propertyViewModel = (propertyRep: PropertyMember, id: string, previousValue: Value, paneId: number, parentValues: () => _.Dictionary<Value>) => {
            const propertyViewModel = new PropertyViewModel();

            propertyViewModel.onPaneId = paneId;
            propertyViewModel.propertyRep = propertyRep;
            propertyViewModel.entryType = propertyRep.entryType();
            propertyViewModel.id = id;
            propertyViewModel.argId = `${id.toLowerCase()}`;
            propertyViewModel.paneArgId = `${propertyViewModel.argId}${paneId}`;
            propertyViewModel.isEditable = !propertyRep.disabledReason();
            propertyViewModel.title = propertyRep.extensions().friendlyName();
            propertyViewModel.optional = propertyRep.extensions().optional();
            propertyViewModel.returnType = propertyRep.extensions().returnType();
            propertyViewModel.draggableType = propertyRep.extensions().returnType();
            propertyViewModel.format = propertyRep.extensions().format();
            propertyViewModel.multipleLines = propertyRep.extensions().multipleLines() || 1;
            propertyViewModel.password = propertyRep.extensions().dataType() === "password";

            propertyViewModel.clientValid = true;

            const required = propertyViewModel.optional ? "" : "* ";
            propertyViewModel.description = propertyRep.extensions().description();

            if (propertyRep.attachmentLink() != null) {
                propertyViewModel.attachment = viewModelFactory.attachmentViewModel(propertyRep, paneId);
            }

            let setupChoice: (newValue: Value) => void;

            if (propertyRep.entryType() === EntryType.Choices) {
                const choices = propertyRep.choices();
                propertyViewModel.choices = _.map(choices, (v, n) => ChoiceViewModel.create(v, id, n));

                setupChoice = (newValue: Value) => {
                    const currentChoice = ChoiceViewModel.create(newValue, id);
                    propertyViewModel.choice = _.find(propertyViewModel.choices, (c: ChoiceViewModel) => c.match(currentChoice));
                }
            } else {
                // use choice for draggable/droppable references
                propertyViewModel.choices = [];

                setupChoice = (newValue: Value) => propertyViewModel.choice = ChoiceViewModel.create(newValue, id);
            }


            if (propertyRep.entryType() === EntryType.AutoComplete) {

                propertyViewModel.prompt = (searchTerm: string) => {
                    const createcvm = _.partial(createChoiceViewModels, id, searchTerm);
                    return context.autoComplete(propertyRep, id, parentValues, searchTerm).then(createcvm);
                };
                propertyViewModel.minLength = propertyRep.promptLink().extensions().minLength();
                propertyViewModel.description = propertyViewModel.description || autoCompletePrompt;
            }

            if (propertyRep.entryType() === EntryType.ConditionalChoices) {

                propertyViewModel.conditionalChoices = (args: _.Dictionary<Value>) => {
                    const createcvm = _.partial(createChoiceViewModels, id, null);
                    return context.conditionalChoices(propertyRep, id, () => <_.Dictionary<Value>>{}, args).then(createcvm);
                };
                // fromPairs definition faulty
                propertyViewModel.arguments = (<any>_).fromPairs(_.map(propertyRep.promptLink().arguments(), (v: any, key: string) => [key, new Value(v.value)]));
            }

            function callIfChanged(newValue: Value, doRefresh: (newValue: Value) => void) {
                const value = newValue || propertyRep.value();

                if (propertyViewModel.currentValue == null || value.toValueString() !== propertyViewModel.currentValue.toValueString()) {
                    doRefresh(value);
                    propertyViewModel.currentValue = value;
                }
            }

            if (propertyRep.isScalar()) {
                propertyViewModel.reference = "";
                propertyViewModel.type = "scalar";

                const remoteMask = propertyRep.extensions().mask();
                const localFilter = mask.toLocalFilter(remoteMask, propertyRep.extensions().format());
                propertyViewModel.localFilter = localFilter;
                // formatting also happens in in directive - at least for dates - value is now date in that case

                propertyViewModel.refresh = (newValue: Value) => callIfChanged(newValue,
                    (value: Value) => {

                        setupChoice(value);
                        if (isDateOrDateTime(propertyRep)) {
                            propertyViewModel.value = toUtcDate(value);
                        } else if (isTime(propertyRep)) {
                            propertyViewModel.value = toTime(value);
                        } else {
                            propertyViewModel.value = value.scalar();
                        }

                        if (propertyRep.entryType() === EntryType.Choices) {
                            if (propertyViewModel.choice) {
                                propertyViewModel.value = propertyViewModel.choice.name;
                                propertyViewModel.formattedValue = propertyViewModel.choice.name;
                            }
                        } else if (propertyViewModel.password) {
                            propertyViewModel.formattedValue = obscuredText;
                        } else {
                            propertyViewModel.formattedValue = localFilter.filter(propertyViewModel.value);
                        }
                    });

            } else {
                // is reference

                propertyViewModel.refresh = (newValue: Value) => callIfChanged(newValue, (value: Value) => {
                    setupChoice(value);
                    setupReference(propertyViewModel, value, propertyRep);
                });
            }

            propertyViewModel.refresh(previousValue);

            // only set color if has value 
            propertyViewModel.setColor(color);

            if (!previousValue) {
                propertyViewModel.originalValue = propertyViewModel.getValue();
            }

            propertyViewModel.description = required + propertyViewModel.description;

            propertyViewModel.isDirty = () => !!previousValue || propertyViewModel.getValue().toValueString() !== propertyViewModel.originalValue.toValueString();
            propertyViewModel.validate = _.partial(validate, propertyRep, propertyViewModel) as (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;
            propertyViewModel.canDropOn = (targetType: string) => context.isSubTypeOf(propertyViewModel.returnType, targetType);
            propertyViewModel.drop = _.partial(drop, propertyViewModel);
            propertyViewModel.doClick = (right?: boolean) => urlManager.setProperty(propertyRep, clickHandler.pane(paneId, right));


            return propertyViewModel;
        };


        viewModelFactory.getItems = (links: Link[], tableView: boolean, routeData: PaneRouteData, listViewModel: ListViewModel | CollectionViewModel) => {
            const selectedItems = routeData.selectedItems;

            const items = _.map(links, (link, i) => viewModelFactory.itemViewModel(link, routeData.paneId, selectedItems[i]));

            if (tableView) {

                const getActionExtensions = routeData.objectId ?
                    () => context.getActionExtensionsFromObject(routeData.paneId, ObjectIdWrapper.fromObjectId(routeData.objectId), routeData.actionId) :
                    () => context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);

                const getExtensions = listViewModel instanceof CollectionViewModel ? () => $q.when(listViewModel.collectionRep.extensions()) : getActionExtensions;

                // clear existing header 
                listViewModel.header = null;

                if (items.length > 0) {

                    getExtensions().then((ext: Extensions) => {
                        _.forEach(items, itemViewModel => {
                            itemViewModel.target.hasTitle = ext.tableViewTitle();
                            itemViewModel.target.title = itemViewModel.title;
                        });

                        if (!listViewModel.header) {
                            const firstItem = items[0].target;
                            const propertiesHeader = _.map(firstItem.properties, property => property.title);

                            listViewModel.header = firstItem.hasTitle ? [""].concat(propertiesHeader) : propertiesHeader;

                            focusManager.focusOverrideOff();
                            focusManager.focusOn(FocusTarget.TableItem, 0, routeData.paneId);
                        }

                    });
                }
            }

            return items;
        };

        function getCollectionCount(count: number) {
            if (count == null) {
                return unknownCollectionSize;
            }

            if (count === 0) {
                return emptyCollectionSize;
            }

            const postfix = count === 1 ? "Item" : "Items";

            return `${count} ${postfix}`;
        }

        function getDefaultTableState(exts: Extensions) {
            if (exts.renderEagerly()) {
                return exts.tableViewColumns() || exts.tableViewTitle() ? CollectionViewState.Table : CollectionViewState.List;
            }
            return CollectionViewState.Summary;
        }

        viewModelFactory.collectionViewModel = (collectionRep: CollectionMember, routeData: PaneRouteData) => {
            const collectionViewModel = new CollectionViewModel();

            const itemLinks = collectionRep.value();
            const paneId = routeData.paneId;
            const size = collectionRep.size();

            collectionViewModel.collectionRep = collectionRep;
            collectionViewModel.onPaneId = paneId;
            collectionViewModel.title = collectionRep.extensions().friendlyName();
            collectionViewModel.pluralName = collectionRep.extensions().pluralName();
            color.toColorNumberFromType(collectionRep.extensions().elementType()).then((c: number) => collectionViewModel.color = `${linkColor}${c}`);

            collectionViewModel.refresh = (routeData: PaneRouteData, resetting: boolean) => {

                let state = size === 0 ? CollectionViewState.Summary : routeData.collections[collectionRep.collectionId()];

                if (state == null) {
                    state = getDefaultTableState(collectionRep.extensions());
                }

                if (resetting || state !== collectionViewModel.currentState) {

                    collectionViewModel.size = getCollectionCount(size);

                    const getDetails = itemLinks == null || state === CollectionViewState.Table;

                    if (state === CollectionViewState.Summary) {
                        collectionViewModel.items = [];
                    } else if (getDetails) {
                        // TODO - there was a missing catch here make sure all top level promises have a catch !

                        context.getCollectionDetails(collectionRep, state, resetting)
                            .then((details: CollectionRepresentation) => {
                                collectionViewModel.items = viewModelFactory.getItems(details.value(),
                                    state === CollectionViewState.Table,
                                    routeData,
                                    collectionViewModel);
                                collectionViewModel.size = getCollectionCount(collectionViewModel.items.length);
                            })
                            .catch((reject: ErrorWrapper) => {
                                context.handleWrappedError(reject, null, () => { }, () => { });
                            });
                    } else {
                        collectionViewModel.items = viewModelFactory.getItems(itemLinks, state === CollectionViewState.Table, routeData, collectionViewModel);
                    }

                    switch (state) {
                        case CollectionViewState.List:
                            collectionViewModel.template = collectionListTemplate;
                            break;
                        case CollectionViewState.Table:
                            collectionViewModel.template = collectionTableTemplate;
                            break;
                        default:
                            collectionViewModel.template = collectionSummaryTemplate;
                    }
                    collectionViewModel.currentState = state;
                }
            }

            collectionViewModel.refresh(routeData, true);

            collectionViewModel.doSummary = () => urlManager.setCollectionMemberState(collectionRep.collectionId(), CollectionViewState.Summary, paneId);
            collectionViewModel.doList = () => urlManager.setCollectionMemberState(collectionRep.collectionId(), CollectionViewState.List, paneId);
            collectionViewModel.doTable = () => urlManager.setCollectionMemberState(collectionRep.collectionId(), CollectionViewState.Table, paneId);

            return collectionViewModel;
        };


        viewModelFactory.listPlaceholderViewModel = (routeData: PaneRouteData) => {
            const collectionPlaceholderViewModel = new CollectionPlaceholderViewModel();

            collectionPlaceholderViewModel.description = () => `Page ${routeData.page}`;

            const recreate = () =>
                routeData.objectId ?
                    context.getListFromObject(routeData.paneId, routeData, routeData.page, routeData.pageSize) :
                    context.getListFromMenu(routeData.paneId, routeData, routeData.page, routeData.pageSize);


            collectionPlaceholderViewModel.reload = () =>
                recreate().
                    then(() => $route.reload()).
                    catch((reject: ErrorWrapper) => {
                        context.handleWrappedError(reject, null, () => { }, () => { });
                    });

            return collectionPlaceholderViewModel;
        };
        viewModelFactory.servicesViewModel = (servicesRep: DomainServicesRepresentation) => {
            const servicesViewModel = new ServicesViewModel();

            // filter out contributed action services 
            const links = _.filter(servicesRep.value(), m => {
                const sid = m.rel().parms[0].value;
                return sid.indexOf("ContributedActions") === -1;
            });

            servicesViewModel.title = "Services";
            servicesViewModel.color = "bg-color-darkBlue";
            servicesViewModel.items = _.map(links, link => viewModelFactory.linkViewModel(link, 1));
            return servicesViewModel;
        };

        viewModelFactory.serviceViewModel = (serviceRep: DomainObjectRepresentation, routeData: PaneRouteData) => {
            const serviceViewModel = new ServiceViewModel();
            const actions = serviceRep.actionMembers();
            serviceViewModel.serviceId = serviceRep.serviceId();
            serviceViewModel.title = serviceRep.title();
            serviceViewModel.actions = _.map(actions, action => viewModelFactory.actionViewModel(action, serviceViewModel, routeData));
            serviceViewModel.menuItems = createMenuItems(serviceViewModel.actions);

            color.toColorNumberFromType(serviceRep.serviceId()).then((c: number) => {
                serviceViewModel.color = `${objectColor}${c}`;
            });

            return serviceViewModel;
        };

        viewModelFactory.menuViewModel = (menuRep: MenuRepresentation, routeData: PaneRouteData) => {
            const menuViewModel = new MenuViewModel();

            menuViewModel.id = menuRep.menuId();
            menuViewModel.menuRep = menuRep;

            const actions = menuRep.actionMembers();
            menuViewModel.title = menuRep.title();
            menuViewModel.actions = _.map(actions, action => viewModelFactory.actionViewModel(action, menuViewModel, routeData));

            menuViewModel.menuItems = createMenuItems(menuViewModel.actions);


            return menuViewModel;
        };

        function selfLinkWithTitle(o: DomainObjectRepresentation) {
            const link = o.selfLink();
            link.setTitle(o.title());
            return link;
        }

        viewModelFactory.recentItemsViewModel = (paneId: number) => {
            const recentItemsViewModel = new RecentItemsViewModel();
            recentItemsViewModel.onPaneId = paneId;
            const items = _.map(context.getRecentlyViewed(), o => ({ obj: o, link: selfLinkWithTitle(o) }));
            recentItemsViewModel.items = _.map(items, i => viewModelFactory.recentItemViewModel(i.obj, i.link, paneId, false));
            return recentItemsViewModel;
        };

        viewModelFactory.tableRowViewModel = (properties: _.Dictionary<PropertyMember>, paneId: number): TableRowViewModel => {
            const tableRowViewModel = new TableRowViewModel();
            tableRowViewModel.properties = _.map(properties, (property, id) => viewModelFactory.propertyTableViewModel(property, id, paneId));
            return tableRowViewModel;
        };


        let cachedToolBarViewModel: ToolBarViewModel;

        function getToolBarViewModel() {
            if (!cachedToolBarViewModel) {
                const tvm = new ToolBarViewModel();
                tvm.goHome = (right?: boolean) => {
                    focusManager.focusOverrideOff();
                    context.updateParms();
                    urlManager.setHome(clickHandler.pane(1, right));
                };
                tvm.goBack = () => {
                    focusManager.focusOverrideOff();
                    context.updateParms();
                    navigation.back();
                };
                tvm.goForward = () => {
                    focusManager.focusOverrideOff();
                    context.updateParms();
                    navigation.forward();
                };
                tvm.swapPanes = () => {
                    $rootScope.$broadcast(geminiPaneSwapEvent);
                    context.updateParms();
                    context.swapCurrentObjects();
                    urlManager.swapPanes();
                };
                tvm.singlePane = (right?: boolean) => {
                    context.updateParms();
                    urlManager.singlePane(clickHandler.pane(1, right));
                    focusManager.refresh(1);
                };
                tvm.cicero = () => {
                    context.updateParms();
                    urlManager.singlePane(clickHandler.pane(1));
                    urlManager.cicero();
                };

                tvm.recent = (right?: boolean) => {
                    context.updateParms();
                    focusManager.focusOverrideOff();
                    urlManager.setRecent(clickHandler.pane(1, right));
                };

                tvm.logOff = () => {
                    context.getUser()
                        .then(u => {
                            if (window.confirm(logOffMessage(u.userName() || "Unknown"))) {
                                const config = {
                                    withCredentials: true,
                                    url: logoffUrl,
                                    method: "POST",
                                    cache: false
                                };

                                $http(config)
                                    .finally(() => {
                                        $rootScope.$broadcast(geminiLogoffEvent);
                                        $timeout(() => window.location.href = postLogoffUrl);
                                    });
                            }
                        });
                };

                tvm.applicationProperties = () => {
                    context.updateParms();
                    urlManager.applicationProperties();
                };


                tvm.template = appBarTemplate;
                tvm.footerTemplate = footerTemplate;

                $rootScope.$on(geminiAjaxChangeEvent, (event, count) =>
                    tvm.loading = count > 0 ? loadingMessage : "");

                $rootScope.$on(geminiWarningEvent, (event, warnings) =>
                    tvm.warnings = warnings);

                $rootScope.$on(geminiMessageEvent, (event, messages) =>
                    tvm.messages = messages);

                context.getUser().then(user => tvm.userName = user.userName());

                cachedToolBarViewModel = tvm;
            }
            return cachedToolBarViewModel;
        }

        viewModelFactory.toolBarViewModel = () => getToolBarViewModel();

        let cvm: CiceroViewModel = null;

        viewModelFactory.ciceroViewModel = () => {
            if (cvm == null) {
                cvm = new CiceroViewModel();
                commandFactory.initialiseCommands(cvm);
                cvm.parseInput = (input: string) => {
                    commandFactory.parseInput(input, cvm);
                };
                cvm.executeNextChainedCommandIfAny = () => {
                    if (cvm.chainedCommands && cvm.chainedCommands.length > 0) {
                        const next = cvm.popNextCommand();
                        commandFactory.processSingleCommand(next, cvm, true);
                    }
                };
                cvm.autoComplete = (input: string) => {
                    commandFactory.autoComplete(input, cvm);
                };
                cvm.renderHome = (routeData: PaneRouteData) => {
                    //TODO: Could put this in a function passed into a render method on CVM
                    if (cvm.message) {
                        cvm.outputMessageThenClearIt();
                    } else {
                        let output = "";
                        if (routeData.menuId) {
                            context.getMenu(routeData.menuId)
                                .then((menu: MenuRepresentation) => {
                                    output += menu.title() + " menu" + "\n";
                                    return routeData.dialogId ? context.getInvokableAction(menu.actionMember(routeData.dialogId)) : $q.when(null);
                                }).then((details: IInvokableAction) => {
                                    if (details) {
                                        output += renderActionDialog(details, routeData, mask);
                                    }
                                }).finally(() => {
                                    cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                                });
                        } else {
                            cvm.clearInput();
                            cvm.output = "Welcome to Cicero. Type 'help' and the Enter key for more information.";
                        }
                    }
                };
                cvm.renderObject = (routeData: PaneRouteData) => {
                    if (cvm.message) {
                        cvm.outputMessageThenClearIt();
                    } else {
                        const oid = ObjectIdWrapper.fromObjectId(routeData.objectId);

                        context.getObject(1, oid, routeData.interactionMode) //TODO: move following code out into a ICireroRenderers service with methods for rendering each context type
                            .then((obj: DomainObjectRepresentation) => {
                                let output = "";
                                const openCollIds = openCollectionIds(routeData);
                                if (_.some(openCollIds)) {
                                    const id = openCollIds[0];
                                    const coll = obj.collectionMember(id);
                                    output += `Collection: ${coll.extensions().friendlyName()} on ${TypePlusTitle(obj)}\n`;
                                    switch (coll.size()) {
                                        case 0:
                                            output += "empty";
                                            break;
                                        case 1:
                                            output += "1 item";
                                            break;
                                        default:
                                            output += `${coll.size()} items`;
                                    }
                                    cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                                } else {
                                    if (obj.isTransient()) {
                                        output += "Unsaved ";
                                        output += obj.extensions().friendlyName() + "\n";
                                        output += renderModifiedProperties(obj, routeData, mask);
                                        cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                                    } else if (routeData.interactionMode === InteractionMode.Edit ||
                                        routeData.interactionMode === InteractionMode.Form) {
                                        let output = "Editing ";
                                        output += PlusTitle(obj) + "\n";
                                        if (routeData.dialogId) {
                                            context.getInvokableAction(obj.actionMember(routeData.dialogId))
                                                .then((details: IInvokableAction) => {
                                                    output += renderActionDialog(details, routeData, mask);
                                                    cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                                                });
                                        } else {
                                            output += renderModifiedProperties(obj, routeData, mask);
                                            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                                        }
                                    } else {
                                        let output = Title(obj) + "\n";
                                        if (routeData.dialogId) {
                                            context.getInvokableAction(obj.actionMember(routeData.dialogId))
                                                .then((details: IInvokableAction) => {
                                                    output += renderActionDialog(details, routeData, mask);
                                                    cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                                                });
                                        } else {
                                            cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                                        }

                                    }
                                }
                            }).catch((reject: ErrorWrapper) => {

                                const custom = (cc: ClientErrorCode) => {
                                    if (cc === ClientErrorCode.ExpiredTransient) {
                                        cvm.output = "The requested view of unsaved object details has expired";
                                        return true;
                                    }
                                    return false;
                                };

                                context.handleWrappedError(reject, null, () => { }, () => { }, custom);
                            });
                    }
                };
                cvm.renderList = (routeData: PaneRouteData) => {
                    if (cvm.message) {
                        cvm.outputMessageThenClearIt();
                    } else {
                        const listPromise = context.getListFromMenu(1, routeData, routeData.page, routeData.pageSize);
                        listPromise.then((list: ListRepresentation) => {
                            context.getMenu(routeData.menuId).then((menu: MenuRepresentation) => {
                                const count = list.value().length;
                                const numPages = list.pagination().numPages;
                                let description: string;
                                if (numPages > 1) {
                                    const page = list.pagination().page;
                                    const totalCount = list.pagination().totalCount;
                                    description = `Page ${page} of ${numPages} containing ${count} of ${totalCount} items`;
                                } else {
                                    description = `${count} items`;
                                }
                                const actionMember = menu.actionMember(routeData.actionId);
                                const actionName = actionMember.extensions().friendlyName();
                                const output = `Result from ${actionName}:\n${description}`;
                                cvm.clearInputRenderOutputAndAppendAlertIfAny(output);
                            });
                        });
                    }
                };
                cvm.renderError = () => {
                    const err = context.getError().error as ErrorRepresentation;
                    cvm.clearInput();
                    cvm.output = `Sorry, an application error has occurred. ${err.message()}`;
                };
            }
            return cvm;
        };

        function logoff() {
            cvm = null;
        }

        $rootScope.$on(geminiLogoffEvent, () => logoff());


        function renderActionDialogIfOpen(
            repWithActions: IHasActions,
            routeData: PaneRouteData,
            mask: IMask): string {
            let output = "";
            if (routeData.dialogId) {
                const actionMember = repWithActions.actionMember(routeData.dialogId) as InvokableActionMember;
                const actionName = actionMember.extensions().friendlyName();
                output += `Action dialog: ${actionName}\n`;
                _.forEach(context.getCurrentDialogValues(), (value, paramId) => {
                    output += FriendlyNameForParam(actionMember, paramId) + ": ";
                    const param = actionMember.parameters()[paramId];
                    output += renderFieldValue(param, value, mask);
                    output += "\n";
                });
            }
            return output;
        }

        function renderActionDialog(
            invokable: Models.IInvokableAction,
            routeData: PaneRouteData,
            mask: IMask): string {
            const actionName = invokable.extensions().friendlyName();
            let output = `Action dialog: ${actionName}\n`;
            _.forEach(context.getCurrentDialogValues(), (value, paramId) => {
                output += FriendlyNameForParam(invokable, paramId) + ": ";
                const param = invokable.parameters()[paramId];
                output += renderFieldValue(param, value, mask);
                output += "\n";
            });
            return output;
        }

    });

    //Returns collection Ids for any collections on an object that are currently in List or Table mode
    export function openCollectionIds(routeData: PaneRouteData): string[] {
        return _.filter(_.keys(routeData.collections), k => routeData.collections[k] !== CollectionViewState.Summary);
    }

    function renderModifiedProperties(obj: DomainObjectRepresentation, routeData: PaneRouteData, mask: IMask): string {
        let output = "";
        if (_.keys(routeData.props).length > 0) {
            output += "Modified properties:\n";
            _.each(routeData.props, (value, propId) => {
                output += FriendlyNameForProperty(obj, propId) + ": ";
                const pm = obj.propertyMember(propId);
                output += renderFieldValue(pm, value, mask);
                output += "\n";
            });
        }
        return output;
    }

    //Handles empty values, and also enum conversion
    export function renderFieldValue(field: IField, value: Value, mask: IMask): string {
        if (!field.isScalar()) { //i.e. a reference
            return value.isNull() ? "empty" : value.toString();
        }
        //Rest is for scalar fields only:
        if (value.toString()) { //i.e. not empty
            //This is to handle an enum: render it as text, not a number:           
            if (field.entryType() === EntryType.Choices) {
                const inverted = _.invert(field.choices());
                return (<any>inverted)[value.toValueString()];
            } else if (field.entryType() === EntryType.MultipleChoices && value.isList()) {
                const inverted = _.invert(field.choices());
                let output = "";
                const values = value.list();
                _.forEach(values, v => {
                    output += (<any>inverted)[v.toValueString()] + ",";
                });
                return output;
            }
        }
        let properScalarValue: number | string | boolean | Date;
        if (isDateOrDateTime(field)) {
            properScalarValue = toUtcDate(value);
        } else {
            properScalarValue = value.scalar();
        }
        if (properScalarValue === "" || properScalarValue == null) {
            return "empty";
        } else {
            const remoteMask = field.extensions().mask();
            const format = field.extensions().format();
            return mask.toLocalFilter(remoteMask, format).filter(properScalarValue);
        }
    }

    


}