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
    import toUtcDate = Models.toUtcDate;
    import isDateOrDateTime = Models.isDateOrDateTime;
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
        actionViewModel(actionRep: ActionMember | ActionRepresentation, vm: IMessageViewModel, routedata: PaneRouteData): ActionViewModel;
        collectionViewModel(collectionRep: CollectionMember, routeData: PaneRouteData): CollectionViewModel;
        listPlaceholderViewModel(routeData: PaneRouteData): CollectionPlaceholderViewModel;
        servicesViewModel(servicesRep: DomainServicesRepresentation): ServicesViewModel;
        serviceViewModel(serviceRep: DomainObjectRepresentation, routeData: PaneRouteData): ServiceViewModel;

        menuViewModel(menuRep: MenuRepresentation, routeData: PaneRouteData): MenuViewModel;

        tableRowViewModel(properties: _.Dictionary<PropertyMember>, paneId: number): TableRowViewModel;
        parameterViewModel(parmRep: Parameter, previousValue: Value, paneId: number): ParameterViewModel;
        propertyViewModel(propertyRep: PropertyMember, id: string, previousValue: Value, paneId: number, parentValues: () => _.Dictionary<Value>): PropertyViewModel;
        ciceroViewModel(): CiceroViewModel;
        handleErrorResponse(err: ErrorMap, vm: IMessageViewModel, vms: ValueViewModel[]): void;
        getItems(links: Link[], tableView: boolean, routeData: PaneRouteData, collectionViewModel: CollectionViewModel | ListViewModel): IItemViewModel[];
        linkViewModel(linkRep: Link, paneId: number): ILinkViewModel;
        recentItemsViewModel(paneId: number): RecentItemsViewModel;
        attachmentViewModel(propertyRep: PropertyMember, paneId: number): IAttachmentViewModel;
    }

    interface IViewModelFactoryInternal extends IViewModelFactory {
        itemViewModel(linkRep: Link, paneId: number, selected: boolean): IItemViewModel;
        recentItemViewModel(obj: DomainObjectRepresentation, linkRep: Link, paneId: number, selected: boolean): IRecentItemViewModel;
        propertyTableViewModel(propertyRep: PropertyMember, id: string, paneId: number): TableRowColumnViewModel;
    }

    app.service("viewModelFactory", function ($q: ng.IQService,
        $timeout: ng.ITimeoutService,
        $location: ng.ILocationService,
        $filter: ng.IFilterService,
        $cacheFactory: ng.ICacheFactoryService,
        $rootScope: ng.IRootScopeService,
        $route: ng.route.IRouteService,
        $http: ng.IHttpService,
        repLoader: IRepLoader,
        color: IColor,
        context: IContext,
        mask: IMask,
        urlManager: IUrlManager,
        focusManager: IFocusManager,
        navigation: INavigation,
        clickHandler: IClickHandler,
        commandFactory: ICommandFactory,
        error: IError,
        ciceroRenderer: ICiceroRenderer) {

        var viewModelFactory = <IViewModelFactoryInternal>this;

        viewModelFactory.errorViewModel = (error: ErrorWrapper) => {
            const errorViewModel = new ErrorViewModel();

            errorViewModel.originalError = error;
            if (error) {
                errorViewModel.title = error.title;
                errorViewModel.description = error.description;
                errorViewModel.errorCode = error.errorCode;
                errorViewModel.message = error.message;
                const stackTrace = error.stackTrace;

                errorViewModel.stackTrace = stackTrace && stackTrace.length !== 0 ? stackTrace : null;

                errorViewModel.isConcurrencyError =
                    error.category === ErrorCategory.HttpClientError &&
                    error.httpErrorCode === HttpStatusCode.PreconditionFailed;
            }

            errorViewModel.description = errorViewModel.description || "No description available";
            errorViewModel.errorCode = errorViewModel.errorCode || "No code available";
            errorViewModel.message = errorViewModel.message || "No message available";
            errorViewModel.stackTrace = errorViewModel.stackTrace || ["No stack trace available"];

            return errorViewModel;
        };

        function initLinkViewModel(linkViewModel: LinkViewModel, linkRep: Link) {
            linkViewModel.title = linkRep.title() + dirtyMarker(context, linkRep.getOid());
            linkViewModel.link = linkRep;
            linkViewModel.domainType = linkRep.type().domainType;
                    
            // for dropping 
            const value = new Value(linkRep);

            linkViewModel.value = value.toString();
            linkViewModel.reference = value.toValueString();
            linkViewModel.choice = ChoiceViewModel.create(value, "");
            linkViewModel.draggableType = linkViewModel.domainType;
            color.toColorNumberFromHref(linkRep.href()).then(c => linkViewModel.color = `${linkColor}${c}`);
            linkViewModel.canDropOn = (targetType: string) => context.isSubTypeOf(linkViewModel.domainType, targetType);
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
            initLinkViewModel(linkViewModel, linkRep);

            linkViewModel.doClick = () => {
                // because may be clicking on menu already open so want to reset focus             
                urlManager.setMenu(linkRep.rel().parms[0].value, paneId);
                focusManager.setCurrentPane(paneId);
                focusManager.focusOverrideOff();
                focusManager.focusOn(FocusTarget.SubAction, 0, paneId);
            };
            
            return linkViewModel as ILinkViewModel;
        };

        viewModelFactory.itemViewModel = (linkRep: Link, paneId: number, selected: boolean) => {
            const itemViewModel = new ItemViewModel();    
            initLinkViewModel(itemViewModel, linkRep);

            itemViewModel.selected = selected;

            itemViewModel.selectionChange = (index) => {
                context.updateValues();
                urlManager.setListItem(index, itemViewModel.selected, paneId);
                focusManager.focusOverrideOn(FocusTarget.CheckBox, index + 1, paneId);
            };

            itemViewModel.doClick = (right?: boolean) => {
                const currentPane = clickHandler.pane(paneId, right);
                focusManager.setCurrentPane(currentPane);
                urlManager.setItem(linkRep, currentPane);
            };

            const members = linkRep.members();

            if (members) {
                itemViewModel.tableRowViewModel = viewModelFactory.tableRowViewModel(members, paneId);
                itemViewModel.tableRowViewModel.title = itemViewModel.title;
            }

            return itemViewModel;
        };

        viewModelFactory.recentItemViewModel = (obj: DomainObjectRepresentation, linkRep: Link, paneId: number, selected: boolean) => {
            const recentItemViewModel = viewModelFactory.itemViewModel(linkRep, paneId, selected) as ILinkViewModel;
            (recentItemViewModel as IRecentItemViewModel).friendlyName = obj.extensions().friendlyName();
            return recentItemViewModel as IRecentItemViewModel;
        };

        viewModelFactory.actionViewModel = (actionRep: ActionMember | ActionRepresentation, vm: IMessageViewModel, routeData: PaneRouteData) => {
            const actionViewModel = new ActionViewModel();

            const parms = routeData.actionParams;
            const paneId = routeData.paneId;

            actionViewModel.actionRep = actionRep;

            if (actionRep instanceof ActionRepresentation || actionRep instanceof InvokableActionMember) {
                actionViewModel.invokableActionRep = actionRep;
            }

            actionViewModel.title = actionRep.extensions().friendlyName();
            actionViewModel.presentationHint = actionRep.extensions().presentationHint();
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
                return context.getInvokableAction(actionViewModel.actionRep).then(details => context.invokeAction(details, parmMap, paneId, clickHandler.pane(paneId, right)));
            };

            // form actions should never show dialogs
            const showDialog = () => actionRep.extensions().hasParams() && (routeData.interactionMode !== InteractionMode.Form);

            // open dialog on current pane always - invoke action goes to pane indicated by click
            actionViewModel.doInvoke = showDialog() ?
                (right?: boolean) => {
                    focusManager.setCurrentPane(paneId);
                    focusManager.focusOverrideOff();
                    // clear any previous dialog 
                    context.clearDialogValues(paneId);
                    urlManager.setDialog(actionRep.actionId(), paneId);
                    focusManager.focusOn(FocusTarget.Dialog, 0, paneId); // in case dialog is already open
                } :
                (right?: boolean) => {
                    focusManager.focusOverrideOff();
                    const pps = actionViewModel.parameters();
                    actionViewModel.executeInvoke(pps, right).
                        catch((reject: ErrorWrapper) => {

                            const display = (em: ErrorMap) => vm.setMessage(em.invalidReason() || em.warningMessage);
                            error.handleErrorAndDisplayMessages(reject, display);
                        });
                };

            actionViewModel.makeInvokable = (details: IInvokableAction) => actionViewModel.invokableActionRep = details;

            return actionViewModel;
        };


        viewModelFactory.handleErrorResponse = (err: ErrorMap, messageViewModel: IMessageViewModel, valueViewModels: ValueViewModel[]) => {

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

                tableRowColumnViewModel.formattedValue = getCollectionDetails(size);
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
                        const choice = _.find(choices, c => c.valuesEqual(currentChoice));

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
            const propertyViewModel = new PropertyViewModel(propertyRep);

            propertyViewModel.id = id;
            propertyViewModel.onPaneId = paneId;
            propertyViewModel.argId = `${id.toLowerCase()}`;
            propertyViewModel.paneArgId = `${propertyViewModel.argId}${paneId}`;
  
            const required = propertyViewModel.optional ? "" : "* ";
           
            if (propertyRep.attachmentLink() != null) {
                propertyViewModel.attachment = viewModelFactory.attachmentViewModel(propertyRep, paneId);
            }

            const fieldEntryType = propertyViewModel.entryType;
            let setupChoice: (newValue: Value) => void;

            if (fieldEntryType === EntryType.Choices) {
                const choices = propertyRep.choices();
                propertyViewModel.choices = _.map(choices, (v, n) => ChoiceViewModel.create(v, id, n));

                setupChoice = (newValue: Value) => {
                    const currentChoice = ChoiceViewModel.create(newValue, id);
                    propertyViewModel.choice = _.find(propertyViewModel.choices, c => c.valuesEqual(currentChoice));
                }
            } else {
                // use choice for draggable/droppable references
                setupChoice = (newValue: Value) => propertyViewModel.choice = ChoiceViewModel.create(newValue, id);
            }


            if (fieldEntryType === EntryType.AutoComplete) {

                propertyViewModel.prompt = (searchTerm: string) => {
                    const createcvm = _.partial(createChoiceViewModels, id, searchTerm);
                    return context.autoComplete(propertyRep, id, parentValues, searchTerm).then(createcvm);
                };
                propertyViewModel.minLength = propertyRep.promptLink().extensions().minLength();
                propertyViewModel.description = propertyViewModel.description || autoCompletePrompt;
            }

            if (fieldEntryType === EntryType.ConditionalChoices) {

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
                propertyViewModel.type = "scalar";

                const remoteMask = propertyRep.extensions().mask();
                const localFilter = mask.toLocalFilter(remoteMask, propertyRep.extensions().format());
                propertyViewModel.localFilter = localFilter;
                // formatting also happens in in directive - at least for dates - value is now date in that case

                propertyViewModel.refresh = (newValue: Value) => callIfChanged(newValue,  (value: Value) => {

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

        function setupParameterChoices(parmViewModel: ParameterViewModel) {
            const parmRep = parmViewModel.parameterRep;
            parmViewModel.choices = _.map(parmRep.choices(), (v, n) => ChoiceViewModel.create(v, parmRep.id(), n));
        }

        function setupParameterAutocomplete(parmViewModel: ParameterViewModel) {
            const parmRep = parmViewModel.parameterRep;
            parmViewModel.prompt = (searchTerm: string) => {
                const createcvm = _.partial(createChoiceViewModels, parmViewModel.id, searchTerm);
                return context.autoComplete(parmRep, parmViewModel.id, () => <_.Dictionary<Value>>{}, searchTerm).
                    then(createcvm);
            };
            parmViewModel.minLength = parmRep.promptLink().extensions().minLength();
            parmViewModel.description = parmViewModel.description || autoCompletePrompt;
        }

        function setupParameterFreeformReference(parmViewModel: ParameterViewModel, previousValue: Value) {
            const parmRep = parmViewModel.parameterRep;
            parmViewModel.description = parmViewModel.description || dropPrompt;

            const val = previousValue && !previousValue.isNull() ? previousValue : parmRep.default();

            if (!val.isNull() && val.isReference()) {
                parmViewModel.reference = val.link().href();
                parmViewModel.choice = ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);
            }
        }

        function setupParameterConditionalChoices(parmViewModel: ParameterViewModel) {
            const parmRep = parmViewModel.parameterRep;
            parmViewModel.conditionalChoices = (args: _.Dictionary<Value>) => {
                const createcvm = _.partial(createChoiceViewModels, parmViewModel.id, null);
                return context.conditionalChoices(parmRep, parmViewModel.id, () => <_.Dictionary<Value>>{}, args).
                    then(createcvm);
            };
            // fromPairs definition faulty
            parmViewModel.arguments = (<any>_).fromPairs(_.map(parmRep.promptLink().arguments(), (v: any, key: string) => [key, new Value(v.value)]));
        }

        function setupParameterSelectedChoices(parmViewModel: ParameterViewModel, previousValue: Value) {
            const parmRep = parmViewModel.parameterRep;
            const fieldEntryType = parmViewModel.entryType;
            function setCurrentChoices(vals: Value) {

                const choicesToSet = _.map(vals.list(), val => ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null));

                if (fieldEntryType === EntryType.MultipleChoices) {
                    parmViewModel.multiChoices = _.filter(parmViewModel.choices, c => _.some(choicesToSet, choiceToSet => c.valuesEqual(choiceToSet)));
                } else {
                    parmViewModel.multiChoices = choicesToSet;
                }
            }

            function setCurrentChoice(val: Value) {
                const choiceToSet = ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);

                if (fieldEntryType === EntryType.Choices) {
                    parmViewModel.choice = _.find(parmViewModel.choices, c => c.valuesEqual(choiceToSet));
                } else {
                    if (!parmViewModel.choice || parmViewModel.choice.getValue().toValueString() !== choiceToSet.getValue().toValueString()) {
                        parmViewModel.choice = choiceToSet;
                    }
                }
            }

            parmViewModel.refresh = (newValue: Value) => {

                if (newValue || parmViewModel.dflt) {
                    const toSet = newValue || parmRep.default();
                    if (fieldEntryType === EntryType.MultipleChoices || fieldEntryType === EntryType.MultipleConditionalChoices ||
                        parmViewModel.isCollectionContributed) {
                        setCurrentChoices(toSet);
                    } else {
                        setCurrentChoice(toSet);
                    }
                }
                parmViewModel.setColor(color);
            }

            parmViewModel.refresh(previousValue);
            
        }

        function setupParameterSelectedValue(parmViewModel: ParameterViewModel, previousValue: Value) {
            const parmRep = parmViewModel.parameterRep;
            const returnType = parmRep.extensions().returnType();

            parmViewModel.refresh = (newValue: Value) => {

                if (returnType === "boolean") {
                    const valueToSet = (newValue ? newValue.toValueString() : null) || parmRep.default().scalar();
                    const bValueToSet = toTriStateBoolean(valueToSet);

                    parmViewModel.value = bValueToSet;
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

        function getRequiredIndicator(parmViewModel: ParameterViewModel) {
            return parmViewModel.optional || parmViewModel.value instanceof Boolean ? "" : "* ";
        }

        viewModelFactory.parameterViewModel = (parmRep: Parameter, previousValue: Value, paneId: number) => {
            const parmViewModel = new ParameterViewModel(parmRep);

            parmViewModel.onPaneId = paneId;

            const fieldEntryType = parmViewModel.entryType;
          
            if (fieldEntryType === EntryType.Choices || fieldEntryType === EntryType.MultipleChoices) {
                setupParameterChoices(parmViewModel);
            }

            if (fieldEntryType === EntryType.AutoComplete) {
                setupParameterAutocomplete(parmViewModel);
            }

            if (fieldEntryType === EntryType.FreeForm && parmViewModel.type === "ref") {
                setupParameterFreeformReference(parmViewModel, previousValue);
            }

            if (fieldEntryType === EntryType.ConditionalChoices || fieldEntryType === EntryType.MultipleConditionalChoices) {
                setupParameterConditionalChoices(parmViewModel);
            }

            if (fieldEntryType !== EntryType.FreeForm || parmViewModel.isCollectionContributed) {
                setupParameterSelectedChoices(parmViewModel, previousValue);
            } else {
                setupParameterSelectedValue(parmViewModel, previousValue);
            }

            const remoteMask = parmRep.extensions().mask();

            if (remoteMask && parmRep.isScalar()) {
                const localFilter = mask.toLocalFilter(remoteMask, parmRep.extensions().format());
                parmViewModel.localFilter = localFilter;
                // formatting also happens in in directive - at least for dates - value is now date in that case
                parmViewModel.formattedValue = parmViewModel.value ? localFilter.filter(parmViewModel.value.toString()) : "";
            }

            parmViewModel.description = getRequiredIndicator(parmViewModel) + parmViewModel.description;
            parmViewModel.validate = _.partial(validate, parmRep, parmViewModel) as (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;
            parmViewModel.drop = _.partial(drop, parmViewModel);

            return parmViewModel;
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
                            itemViewModel.tableRowViewModel.hasTitle = ext.tableViewTitle();
                            itemViewModel.tableRowViewModel.title = itemViewModel.title;
                        });

                        if (!listViewModel.header) {
                            const firstItem = items[0].tableRowViewModel;
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

        function getCollectionDetails(count: number) {
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
            collectionViewModel.presentationHint = collectionRep.extensions().presentationHint();
            collectionViewModel.pluralName = collectionRep.extensions().pluralName();
            color.toColorNumberFromType(collectionRep.extensions().elementType()).then((c: number) => collectionViewModel.color = `${linkColor}${c}`);

            collectionViewModel.refresh = (routeData: PaneRouteData, resetting: boolean) => {

                let state = size === 0 ? CollectionViewState.Summary : routeData.collections[collectionRep.collectionId()];

                if (state == null) {
                    state = getDefaultTableState(collectionRep.extensions());
                }

                if (resetting || state !== collectionViewModel.currentState) {

                    if (size > 0 || size == null) {
                        collectionViewModel.mayHaveItems = true;
                    }
                    collectionViewModel.details = getCollectionDetails(size);
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
                                collectionViewModel.details = getCollectionDetails(collectionViewModel.items.length);
                            })
                            .catch((reject: ErrorWrapper) => {
                                error.handleError(reject);
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
                        error.handleError(reject);
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
                    context.updateValues();
                    urlManager.setHome(clickHandler.pane(1, right));
                };
                tvm.goBack = () => {
                    focusManager.focusOverrideOff();
                    context.updateValues();
                    navigation.back();
                };
                tvm.goForward = () => {
                    focusManager.focusOverrideOff();
                    context.updateValues();
                    navigation.forward();
                };
                tvm.swapPanes = () => {
                    $rootScope.$broadcast(geminiPaneSwapEvent);
                    context.updateValues();
                    context.swapCurrentObjects();
                    urlManager.swapPanes();
                };
                tvm.singlePane = (right?: boolean) => {
                    context.updateValues();
                    urlManager.singlePane(clickHandler.pane(1, right));
                    focusManager.refresh(1);
                };
                tvm.cicero = () => {
                    context.updateValues();
                    urlManager.singlePane(clickHandler.pane(1));
                    urlManager.cicero();
                };

                tvm.recent = (right?: boolean) => {
                    context.updateValues();
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

                                // logoff server
                                $http(config);

                                // logoff client without waiting for server
                                $rootScope.$broadcast(geminiLogoffEvent);
                                $timeout(() => window.location.href = postLogoffUrl);
                            }
                        });
                };

                tvm.applicationProperties = () => {
                    context.updateValues();
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
                cvm.renderHome = _.partial(ciceroRenderer.renderHome, cvm) as (routeData: PaneRouteData) => void;
                cvm.renderObject = _.partial(ciceroRenderer.renderObject, cvm) as (routeData: PaneRouteData) => void;
                cvm.renderList = _.partial(ciceroRenderer.renderList, cvm) as (routeData: PaneRouteData) => void;
                cvm.renderError = _.partial(ciceroRenderer.renderError, cvm);
            }
            return cvm;
        };

        function logoff() {
            cvm = null;
        }

        $rootScope.$on(geminiLogoffEvent, () => logoff());
    });
}