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
    import IsDateOrDateTime = Models.isDateOrDateTime;
    import toUtcDate = Models.toUtcDate;
    import isDateOrDateTime = Models.isDateOrDateTime;
    import FriendlyTypeName = Models.friendlyTypeName;
    import PlusTitle = Models.typePlusTitle;
    import Title = Models.typePlusTitle;
    import FriendlyNameForProperty = Models.friendlyNameForProperty;
    import FriendlyNameForParam = Models.friendlyNameForParam;
    import ActionRepresentation = Models.ActionRepresentation;
    import IInvokableAction = Models.IInvokableAction;
    import CollectionRepresentation = Models.CollectionRepresentation;

    export interface IViewModelFactory {
        toolBarViewModel(): ToolBarViewModel;
        errorViewModel(errorRep: ErrorWrapper): ErrorViewModel;
        actionViewModel(actionRep: ActionMember | ActionRepresentation, vm: MessageViewModel, routedata: PaneRouteData): ActionViewModel;
        collectionViewModel(collectionRep: CollectionMember, routeData: PaneRouteData): CollectionViewModel;
        listPlaceholderViewModel(routeData: PaneRouteData): CollectionPlaceholderViewModel;
        servicesViewModel(servicesRep: DomainServicesRepresentation): ServicesViewModel;
        serviceViewModel(serviceRep: DomainObjectRepresentation, routeData: PaneRouteData): ServiceViewModel;

        menuViewModel(menuRep: MenuRepresentation, routeData: PaneRouteData): MenuViewModel;

        tableRowViewModel(objectRep: DomainObjectRepresentation, routedata: PaneRouteData, idsToShow?: string[]): TableRowViewModel;
        parameterViewModel(parmRep: Parameter, previousValue: Value, paneId: number): ParameterViewModel;
        propertyViewModel(propertyRep: PropertyMember, id: string, previousValue: Value, paneId: number, parentValues: () => _.Dictionary<Value>): PropertyViewModel;
        ciceroViewModel(): CiceroViewModel;
        handleErrorResponse(err: ErrorMap, vm: MessageViewModel, vms: ValueViewModel[]): void;
        getItems(links: Link[], populateItems: boolean, routeData: PaneRouteData, collectionViewModel: CollectionViewModel | ListViewModel): ItemViewModel[];
        linkViewModel(linkRep: Link, paneId: number): LinkViewModel;
        recentItemsViewModel(paneId: number): RecentItemsViewModel;
    }

    interface IViewModelFactoryInternal extends IViewModelFactory {
        itemViewModel(linkRep: Link, paneId: number, selected: boolean): ItemViewModel;
        recentItemViewModel(obj: DomainObjectRepresentation, linkRep: Link, paneId: number, selected: boolean): RecentItemViewModel;
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
        $route: any) {

        var viewModelFactory = <IViewModelFactoryInternal>this;

        viewModelFactory.errorViewModel = (error: ErrorWrapper) => {
            const errorViewModel = new ErrorViewModel();

            errorViewModel.message = error ? error.message : "Unknown";
            const stackTrace = error ? error.stackTrace : null;
            errorViewModel.stackTrace = !stackTrace || stackTrace.length === 0 ? ["Empty"] : stackTrace;

            errorViewModel.description = "";

            errorViewModel.code = error.errorCode;

            errorViewModel.isConcurrencyError = error.category === ErrorCategory.HttpClientError && error.httpErrorCode === HttpStatusCode.PreconditionFailed;

            return errorViewModel;
        };


        function initLinkViewModel(linkViewModel: LinkViewModel, linkRep: Link) {
            linkViewModel.title = linkRep.title();

            color.toColorNumberFromHref(linkRep.href()).then((c: number) => {
                linkViewModel.color = `${linkColor}${c}`;
            });

            linkViewModel.link = linkRep;

            linkViewModel.domainType = linkRep.type().domainType;
            linkViewModel.draggableType = linkViewModel.domainType;

            // for dropping 
            const value = new Value(linkRep);

            linkViewModel.value = value.toString();
            linkViewModel.reference = value.toValueString();
            linkViewModel.choice = ChoiceViewModel.create(value, "");

            linkViewModel.canDropOn = (targetType: string) => context.isSubTypeOf(targetType, linkViewModel.domainType);
        }

        const createChoiceViewModels = (id: string, searchTerm: string, choices: _.Dictionary<Value>) =>
            $q.when(_.map(choices, (v, k) => ChoiceViewModel.create(v, id, k, searchTerm)));

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
                urlManager.setListItem(index, itemViewModel.selected, paneId);
                focusManager.focusOverrideOn(FocusTarget.CheckBox, index + 1, paneId);
            };
            return itemViewModel;
        };

        viewModelFactory.recentItemViewModel = (obj: DomainObjectRepresentation, linkRep: Link, paneId: number, selected: boolean) => {
            const recentItemViewModel = viewModelFactory.itemViewModel(linkRep, paneId, selected) as RecentItemViewModel;
            recentItemViewModel.friendlyName = obj.extensions().friendlyName();
            return recentItemViewModel;
        }

        viewModelFactory.parameterViewModel = (parmRep: Parameter, previousValue: Value, paneId: number) => {
            const parmViewModel = new ParameterViewModel();

            parmViewModel.parameterRep = parmRep;
            parmViewModel.type = parmRep.isScalar() ? "scalar" : "ref";
            parmViewModel.dflt = parmRep.default().toValueString();
            parmViewModel.optional = parmRep.extensions().optional();
            const required = parmViewModel.optional ? "" : "* ";
            parmViewModel.description = required + parmRep.extensions().description();
            parmViewModel.message = "";
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

            parmViewModel.validate = (modelValue: any, viewValue: string, mandatoryOnly: boolean) => {
                const message = mandatoryOnly ? Models.validateMandatory(parmRep, viewValue) : Models.validate(parmRep, modelValue, viewValue, parmViewModel.localFilter);

                if (message !== mandatory) {
                    parmViewModel.message = message;
                }

                parmViewModel.clientValid = !message;
                return parmViewModel.clientValid;
            };

            parmViewModel.drop = (newValue: IDraggableViewModel) => {
                context.isSubTypeOf(newValue.draggableType, parmViewModel.returnType).
                    then((canDrop: boolean) => {
                        if (canDrop) {
                            parmViewModel.setNewValue(newValue);
                        }
                    });
            };

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

                if (previousValue || parmViewModel.dflt) {
                    const toSet = previousValue || parmRep.default();
                    if (fieldEntryType === EntryType.MultipleChoices || fieldEntryType === EntryType.MultipleConditionalChoices || parmViewModel.isCollectionContributed) {
                        setCurrentChoices(toSet);
                    } else {
                        setCurrentChoice(toSet);
                    }
                }
            } else {
                const returnType = parmRep.extensions().returnType();

                if (returnType === "boolean") {
                    parmViewModel.value = previousValue ? previousValue.toString().toLowerCase() === "true" : parmRep.default().scalar();
                } else if (IsDateOrDateTime(parmRep)) {
                    parmViewModel.value = toUtcDate(previousValue || new Value(parmViewModel.dflt));
                } else {
                    parmViewModel.value = (previousValue ? previousValue.toString() : null) || parmViewModel.dflt || "";
                }
            }

            const remoteMask = parmRep.extensions().mask();

            if (remoteMask && parmRep.isScalar()) {
                const localFilter = mask.toLocalFilter(remoteMask, parmRep.extensions().format());
                parmViewModel.localFilter = localFilter;
                // formatting also happens in in directive - at least for dates - value is now date in that case
                parmViewModel.formattedValue = parmViewModel.value ? localFilter.filter(parmViewModel.value.toString()) : "";
            }

            if (parmViewModel.value) {
                color.toColorNumberFromType(parmViewModel.returnType).then((c: number) => {
                    parmViewModel.color = `${linkColor}${c}`;
                });
            } else {
                parmViewModel.color = "";
            }

            return parmViewModel;
        };

        viewModelFactory.actionViewModel = (actionRep: ActionMember | ActionRepresentation, vm: MessageViewModel, routeData: PaneRouteData) => {
            const actionViewModel = new ActionViewModel();

            const parms = routeData.actionParams;
            const paneId = routeData.paneId;

            actionViewModel.actionRep = actionRep;
            actionViewModel.title = actionRep.extensions().friendlyName();
            actionViewModel.menuPath = actionRep.extensions().menuPath() || "";
            actionViewModel.disabled = () => !!actionRep.disabledReason();

            if (actionViewModel.disabled()) {
                actionViewModel.description = actionRep.disabledReason();
            } else {
                actionViewModel.description = actionRep.extensions().description();
            }

            actionViewModel.parameters = () => {
                // don't use actionRep directly as it may change and we've closed around the original value
                const parameters = _.pickBy(actionViewModel.actionRep.parameters(), p => !p.isCollectionContributed()) as _.Dictionary<Parameter>;
                return _.map(parameters, parm => viewModelFactory.parameterViewModel(parm, parms[parm.id()], paneId));
            };

            actionViewModel.executeInvoke = (pps: ParameterViewModel[], right?: boolean) => {
                const parmMap = _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Value>;
                _.forEach(pps, p => urlManager.setParameterValue(actionRep.actionId(), p.parameterRep, p.getValue(), paneId));
                return context.invokeAction(actionRep, clickHandler.pane(paneId, right), parmMap);
            };

            // form actions should never show dialogs
            const showDialog = () => actionRep.extensions().hasParams() && (routeData.interactionMode !== InteractionMode.Form);

            // open dialog on current pane always - invoke action goes to pane indicated by click
            actionViewModel.doInvoke = showDialog() ?
                (right?: boolean) => {
                    focusManager.setCurrentPane(paneId);
                    focusManager.focusOverrideOff();
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
                            const display = (em: ErrorMap) => vm.message = em.invalidReason() || em.warningMessage;
                            context.handleWrappedError(reject, parent, reset, display);
                        });
                };

            actionViewModel.makeInvokable = (details: IInvokableAction) => {
                actionViewModel.actionRep = details;
            }

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
                            valueViewModel.message = reason;
                            fieldValidationErrors = true;
                        }
                    }
                }
            });

            let msg = err.invalidReason() || "";
            if (requiredFieldsMissing) msg = `${msg} Please complete REQUIRED fields. `;
            if (fieldValidationErrors) msg = `${msg} See field validation message(s). `;

            if (!msg) msg = err.warningMessage;
            messageViewModel.message = msg;
        };

        viewModelFactory.propertyViewModel = (propertyRep: PropertyMember, id: string, previousValue: Value, paneId: number, parentValues: () => _.Dictionary<Value>) => {
            const propertyViewModel = new PropertyViewModel();


            propertyViewModel.title = propertyRep.extensions().friendlyName();
            propertyViewModel.optional = propertyRep.extensions().optional();
            propertyViewModel.onPaneId = paneId;
            propertyViewModel.propertyRep = propertyRep;
            propertyViewModel.multipleLines = propertyRep.extensions().multipleLines() || 1;
            propertyViewModel.password = propertyRep.extensions().dataType() === "password";
            propertyViewModel.clientValid = true;

            const required = propertyViewModel.optional ? "" : "* ";

            propertyViewModel.description = required + propertyRep.extensions().description();

            const value = previousValue || propertyRep.value();

            if (isDateOrDateTime(propertyRep)) {
                propertyViewModel.value = toUtcDate(value);
            } else if (propertyRep.isScalar()) {
                propertyViewModel.value = value.scalar();
            } else if (value.isNull()) {
                // ie null reference 
                propertyViewModel.value = propertyViewModel.description;
            } else {
                propertyViewModel.value = value.toString();
            }

            propertyViewModel.validate = (modelValue: any, viewValue: string, mandatoryOnly: boolean) => {
                const message = mandatoryOnly ? Models.validateMandatory(propertyRep, viewValue) : Models.validate(propertyRep, modelValue, viewValue, propertyViewModel.localFilter);

                if (message !== mandatory) {
                    propertyViewModel.message = message;
                }

                propertyViewModel.clientValid = !message;
                return propertyViewModel.clientValid;
            };

            const returnType = propertyRep.extensions().returnType();
            const format = propertyRep.extensions().format();
            propertyViewModel.type = propertyRep.isScalar() ? "scalar" : "ref";
            propertyViewModel.returnType = returnType;
            propertyViewModel.format = format;
            propertyViewModel.reference = propertyRep.isScalar() || value.isNull() ? "" : value.link().href();
            propertyViewModel.draggableType = propertyViewModel.returnType;

            propertyViewModel.canDropOn = (targetType: string) => context.isSubTypeOf(propertyViewModel.returnType, targetType);

            propertyViewModel.drop = (newValue: IDraggableViewModel) => {
                context.isSubTypeOf(newValue.draggableType, propertyViewModel.returnType).
                    then((canDrop: boolean) => {
                        if (canDrop) {
                            propertyViewModel.setNewValue(newValue);
                        }
                    });
            };

            propertyViewModel.doClick = (right?: boolean) => urlManager.setProperty(propertyRep, clickHandler.pane(paneId, right));
            if (propertyRep.attachmentLink() != null) {
                propertyViewModel.attachment = AttachmentViewModel.create(propertyRep.attachmentLink().href(),
                    propertyRep.attachmentLink().type().asString,
                    propertyRep.attachmentLink().title());
            }

            // only set color if has value 

            if (propertyViewModel.value) {
                color.toColorNumberFromType(propertyRep.extensions().returnType()).then((c: number) => {
                    propertyViewModel.color = `${linkColor}${c}`;
                });
            } else {
                propertyViewModel.color = "";
            }


            propertyViewModel.id = id;
            propertyViewModel.argId = `${id.toLowerCase()}`;
            propertyViewModel.paneArgId = `${propertyViewModel.argId}${paneId}`;
            propertyViewModel.isEditable = !propertyRep.disabledReason();
            propertyViewModel.choices = [];

            const fieldEntryType = propertyRep.entryType();
            propertyViewModel.entryType = fieldEntryType;

            if (fieldEntryType === EntryType.Choices) {
                const choices = propertyRep.choices();
                propertyViewModel.choices = _.map(choices, (v, n) => ChoiceViewModel.create(v, id, n));
            }

            if (fieldEntryType === EntryType.AutoComplete) {

                propertyViewModel.prompt = (searchTerm: string) => {
                    const createcvm = _.partial(createChoiceViewModels, id, searchTerm);
                    return context.autoComplete(propertyRep, id, parentValues, searchTerm).
                        then(createcvm);
                };
                propertyViewModel.minLength = propertyRep.promptLink().extensions().minLength();
            }

            if (fieldEntryType === EntryType.ConditionalChoices) {

                propertyViewModel.conditionalChoices = (args: _.Dictionary<Value>) => {
                    const createcvm = _.partial(createChoiceViewModels, id, null);
                    return context.conditionalChoices(propertyRep, id, () => <_.Dictionary<Value>>{}, args).
                        then(createcvm);
                };
                // fromPairs definition faulty
                propertyViewModel.arguments = (<any>_).fromPairs(_.map(propertyRep.promptLink().arguments(), (v: any, key: string) => [key, new Value(v.value)]));
            }

            if (fieldEntryType !== EntryType.FreeForm) {

                const currentChoice = ChoiceViewModel.create(value, id);

                if (fieldEntryType === EntryType.Choices) {
                    propertyViewModel.choice = _.find(propertyViewModel.choices, (c: ChoiceViewModel) => c.match(currentChoice));
                } else {
                    propertyViewModel.choice = currentChoice;
                }
            }

            if (propertyRep.isScalar()) {
                const remoteMask = propertyRep.extensions().mask();
                const localFilter = mask.toLocalFilter(remoteMask, propertyRep.extensions().format());
                propertyViewModel.localFilter = localFilter;
                // formatting also happens in in directive - at least for dates - value is now date in that case

                if (propertyViewModel.choice) {
                    propertyViewModel.value = propertyViewModel.choice.name;
                    propertyViewModel.formattedValue = propertyViewModel.choice.name;
                } else if (propertyViewModel.password) {
                    propertyViewModel.formattedValue = obscuredText;
                } else {
                    propertyViewModel.formattedValue = localFilter.filter(propertyViewModel.value);
                }
            } else if (value.isNull()) {
                // ie null reference 
                propertyViewModel.formattedValue = "";
            } else {
                propertyViewModel.formattedValue = value.toString();
            }

            if (!previousValue) {
                propertyViewModel.originalValue = propertyViewModel.getValue();
            }

            if (!propertyRep.isScalar()) {
                if (value.isNull()) {
                    propertyViewModel.refType = "null";
                }
                else if (propertyRep.extensions().notNavigable()) {
                    propertyViewModel.refType = "notNavigable";
                } else {
                    propertyViewModel.refType = "navigable";
                }
            }


            propertyViewModel.isDirty = () => {
                return !!previousValue || propertyViewModel.getValue().toValueString() !== propertyViewModel.originalValue.toValueString();
            };
            return propertyViewModel;
        };

        viewModelFactory.getItems = (links: Link[], populateItems: boolean, routeData: PaneRouteData, listViewModel: ListViewModel | CollectionViewModel) => {
            const selectedItems = routeData.selectedItems;

            const items = _.map(links, (link, i) => viewModelFactory.itemViewModel(link, routeData.paneId, selectedItems[i]));

            if (populateItems) {

                const getActionExtensions = routeData.objectId ?
                    () => context.getActionExtensionsFromObject(routeData.paneId, routeData.objectId, routeData.actionId) :
                    () => context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);

                const getExtensions = listViewModel instanceof CollectionViewModel ? () => $q.when(listViewModel.collectionRep.extensions()) : getActionExtensions;


                // clear existing header 
                listViewModel.header = null;

                getExtensions().then((ext: Extensions) => {
                    _.forEach(items, itemViewModel => {
                        const tempTgt = itemViewModel.link.getTarget() as DomainObjectRepresentation;

                        context.getObject(routeData.paneId, tempTgt.getDtId().dt, tempTgt.getDtId().id, false).
                            then((obj: DomainObjectRepresentation) => {

                                itemViewModel.target = viewModelFactory.tableRowViewModel(obj, routeData, ext.tableViewColumns());
                                itemViewModel.target.hasTitle = ext.tableViewTitle();
                                itemViewModel.target.title = obj.title();

                                if (!listViewModel.header) {

                                    const propertiesHeader = _.map(itemViewModel.target.properties, property => property.title);

                                    if (itemViewModel.target.hasTitle) {
                                        listViewModel.header = [""].concat(propertiesHeader);
                                    } else {
                                        listViewModel.header = propertiesHeader;
                                    }

                                    focusManager.focusOverrideOff();
                                    focusManager.focusOn(FocusTarget.TableItem, 0, routeData.paneId);
                                }

                            });
                    });
                });
            }

            return items;
        };

        viewModelFactory.collectionViewModel = (collectionRep: CollectionMember, routeData: PaneRouteData) => {
            const collectionViewModel = new CollectionViewModel();

            const links = collectionRep.value();
            const paneId = routeData.paneId;
            const state = routeData.collections[collectionRep.collectionId()];

            collectionViewModel.collectionRep = collectionRep;
            collectionViewModel.onPaneId = paneId;

            collectionViewModel.title = collectionRep.extensions().friendlyName();

            const size = collectionRep.size();

            if (size == null) {
                collectionViewModel.size = unknownCollectionSize;
            } else {
                collectionViewModel.size = `${size} Items(s)`;
            }

            collectionViewModel.pluralName = collectionRep.extensions().pluralName();

            color.toColorNumberFromType(collectionRep.extensions().elementType()).then((c: number) => {
                collectionViewModel.color = `${linkColor}${c}`;
            });

            if (links) {
                collectionViewModel.items = viewModelFactory.getItems(links, state === CollectionViewState.Table, routeData, collectionViewModel);
            }
            else if (state === CollectionViewState.List || state === CollectionViewState.Table) {

                context.getCollectionDetails(collectionRep).then((details: CollectionRepresentation) => {
                    collectionViewModel.items = viewModelFactory.getItems(details.value(), state === CollectionViewState.Table, routeData, collectionViewModel);
                    collectionViewModel.size = `${collectionViewModel.items.length} Items(s)`;
                });
            } else {
                collectionViewModel.items = [];
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
                    context.getListFromObject(routeData.paneId, routeData.objectId, routeData.actionId, routeData.actionParams, routeData.page, routeData.pageSize) :
                    context.getListFromMenu(routeData.paneId, routeData.menuId, routeData.actionId, routeData.actionParams, routeData.page, routeData.pageSize);


            collectionPlaceholderViewModel.reload = () =>
                recreate().then(() => {
                    $route.reload();
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
            serviceViewModel.actionsMap = createActionMenuMap(serviceViewModel.actions);

            color.toColorNumberFromType(serviceRep.serviceId()).then((c: number) => {
                serviceViewModel.color = `${objectColor}${c}`;
            });

            return serviceViewModel;
        };

        viewModelFactory.menuViewModel = (menuRep: MenuRepresentation, routeData: PaneRouteData) => {
            const menuViewModel = new MenuViewModel();
            const actions = menuRep.actionMembers();
            menuViewModel.title = menuRep.title();
            menuViewModel.actions = _.map(actions, action => viewModelFactory.actionViewModel(action, menuViewModel, routeData));

            menuViewModel.actionsMap = createActionMenuMap(menuViewModel.actions);


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
        }


        viewModelFactory.tableRowViewModel = (objectRep: DomainObjectRepresentation, routeData: PaneRouteData, idsToShow?: string[]): TableRowViewModel => {
            const tableRowViewModel = new TableRowViewModel();
            const properties = idsToShow ? _.pick(objectRep.propertyMembers(), idsToShow) as _.Dictionary<PropertyMember> : objectRep.propertyMembers();
            tableRowViewModel.properties = _.map(properties, (property, id) => viewModelFactory.propertyViewModel(property, id, null, routeData.paneId, () => <_.Dictionary<Value>>{}));

            return tableRowViewModel;
        };

        let cachedToolBarViewModel: ToolBarViewModel;

        function getToolBarViewModel() {
            if (!cachedToolBarViewModel) {
                const tvm = new ToolBarViewModel();
                tvm.goHome = (right?: boolean) => {
                    focusManager.focusOverrideOff();
                    urlManager.setHome(clickHandler.pane(1, right));
                };
                tvm.goBack = () => {
                    focusManager.focusOverrideOff();
                    navigation.back();
                };
                tvm.goForward = () => {
                    focusManager.focusOverrideOff();
                    navigation.forward();
                };
                tvm.swapPanes = () => {
                    $rootScope.$broadcast("pane-swap");
                    context.swapCurrentObjects();
                    urlManager.swapPanes();
                };
                tvm.singlePane = (right?: boolean) => {
                    urlManager.singlePane(clickHandler.pane(1, right));
                    focusManager.refresh(1);
                };
                tvm.cicero = () => {
                    urlManager.singlePane(clickHandler.pane(1));
                    urlManager.cicero();
                };

                tvm.recent = (right?: boolean) => {
                    focusManager.focusOverrideOff();
                    urlManager.setRecent(clickHandler.pane(1, right));
                };

                tvm.template = appBarTemplate;
                tvm.footerTemplate = footerTemplate;

                $rootScope.$on("ajax-change", (event, count) =>
                    tvm.loading = count > 0 ? "Loading..." : "");

                $rootScope.$on("nof-warning", (event, warnings) =>
                    tvm.warnings = warnings);

                $rootScope.$on("nof-message", (event, messages) =>
                    tvm.messages = messages);

                $rootScope.$on("back", () => {
                    focusManager.focusOverrideOff();
                    navigation.back();
                });
                $rootScope.$on("forward", () => {
                    focusManager.focusOverrideOff();
                    navigation.forward();
                });

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
                        if (routeData.menuId) {
                            context.getMenu(routeData.menuId)
                                .then((menu: MenuRepresentation) => {                                  
                                    cvm.output = menu.title() + " menu" + "\n";                                                                      
                                    return routeData.dialogId ? context.getInvokableAction(menu.actionMember(routeData.dialogId)) : $q.when(null);
                                }).then((details: IInvokableAction) => {
                                    if (details) {                                       
                                        cvm.output += renderActionDialog(details, routeData, mask);
                                    }                                  
                                }).finally(() => {
                                    cvm.clearInput();                            
                                    appendAlertIfAny(cvm);
                                });
                        } else {
                            cvm.clearInput();
                            cvm.output = "Welcome to Cicero";
                        }
                    }
                };
                cvm.renderObject = (routeData: PaneRouteData) => {
                    if (cvm.message) {
                        cvm.outputMessageThenClearIt();
                    } else {
                        const [domainType, ...id] = routeData.objectId.split(keySeparator);
                        const transient = routeData.interactionMode === InteractionMode.Transient;
                        context.getObject(1, domainType, id, transient) //TODO: move following code out into a ICireroRenderers service with methods for rendering each context type
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
                                    //TODO: These three lines (or similar) repeated. Extract into function.
                                    cvm.clearInput();
                                    cvm.output = output;
                                    appendAlertIfAny(cvm);
                                } else {
                                    if (obj.isTransient()) {
                                        output += "Unsaved ";
                                        output += FriendlyTypeName(obj.domainType()) + "\n";
                                        output += renderModifiedProperties(obj, routeData, mask);
                                        cvm.clearInput();
                                        cvm.output = output;
                                        appendAlertIfAny(cvm);
                                    } else if (routeData.interactionMode === InteractionMode.Edit ||
                                        routeData.interactionMode === InteractionMode.Form) {
                                        cvm.output = "Editing ";
                                        cvm.output += PlusTitle(obj) + "\n";
                                        if (routeData.dialogId) {
                                            context.getInvokableAction(obj.actionMember(routeData.dialogId))
                                                .then((details: IInvokableAction) => {
                                                    cvm.clearInput();
                                                    cvm.output += renderActionDialog(details, routeData, mask);
                                                    appendAlertIfAny(cvm);
                                                });
                                        } else {
                                            cvm.clearInput();
                                            cvm.output += renderModifiedProperties(obj, routeData, mask);
                                            appendAlertIfAny(cvm);
                                        }
                                    } else {
                                        cvm.output = Title(obj) + "\n";
                                        if (routeData.dialogId) {
                                            context.getInvokableAction(obj.actionMember(routeData.dialogId))
                                                .then((details: IInvokableAction) => {
                                                    cvm.clearInput();
                                                    cvm.output += renderActionDialog(details, routeData, mask);
                                                    appendAlertIfAny(cvm);
                                                });
                                        }
                                        else {
                                            cvm.clearInput();
                                            appendAlertIfAny(cvm);
                                        }

                                    }
                                }
                                //cvm.clearInput();
                                //cvm.output = output;
                                //appendAlertIfAny(cvm);
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
                        const listPromise = context.getListFromMenu(1, routeData.menuId, routeData.actionId, routeData.actionParams, routeData.page, routeData.pageSize);
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
                                cvm.clearInput();
                                cvm.output = `Result from ${actionName}:\n${description}`;
                                appendAlertIfAny(cvm);
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
    });

    function appendAlertIfAny(cvm: CiceroViewModel) {
        if (cvm.alert) {
            cvm.output += cvm.alert;
            cvm.alert = "";
        }
    }

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
        const remoteMask = field.extensions().mask();
        const format = field.extensions().format();
        let formattedValue = mask.toLocalFilter(remoteMask, format).filter(properScalarValue);
        return formattedValue || "empty";
    }

    function renderActionDialogIfOpen(
        repWithActions: IHasActions,
        routeData: PaneRouteData,
        mask: IMask): string {
        let output = "";
        if (routeData.dialogId) {
            const actionMember = repWithActions.actionMember(routeData.dialogId);
            const actionName = actionMember.extensions().friendlyName();
            output += `Action dialog: ${actionName}\n`;
            _.forEach(routeData.dialogFields, (value, paramId) => {
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
        _.forEach(routeData.dialogFields, (value, paramId) => {
            output += FriendlyNameForParam(invokable, paramId) + ": ";
            const param = invokable.parameters()[paramId];
            output += renderFieldValue(param, value, mask);
            output += "\n";
        });
        return output;
    }

}