/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular.Gemini {

    export interface IViewModelFactory {
        toolBarViewModel(): ToolBarViewModel;
        errorViewModel(errorRep: ErrorRepresentation): ErrorViewModel;
        actionViewModel($scope: ng.IScope, actionRep: ActionMember, routedata: PaneRouteData): ActionViewModel;
        dialogViewModel($scope: ng.IScope, actionViewModel: ActionViewModel, routedata: PaneRouteData): DialogViewModel;
        collectionViewModel($scope: ng.IScope, collection: CollectionMember, routeData: PaneRouteData, recreate: (page: number, newPageSize: number, newState?: CollectionViewState) => void): CollectionViewModel;
        collectionViewModel($scope: ng.IScope, collection: ListRepresentation, routeData: PaneRouteData, recreate: (page: number, newPageSize: number, newState?: CollectionViewState) => void): CollectionViewModel;
        collectionPlaceholderViewModel(page: number, reload: () => void): CollectionPlaceholderViewModel;
        servicesViewModel(servicesRep: DomainServicesRepresentation): ServicesViewModel;
        menusViewModel(menusRep: MenusRepresentation, paneId: number): MenusViewModel;
        serviceViewModel($scope: ng.IScope, serviceRep: DomainObjectRepresentation, routeData: PaneRouteData): ServiceViewModel;
        domainObjectViewModel($scope: ng.IScope, objectRep: DomainObjectRepresentation, routedata: PaneRouteData): DomainObjectViewModel;
        ciceroViewModel(): CiceroViewModel;
    }

    interface IViewModelFactoryInternal extends IViewModelFactory {
        linkViewModel(linkRep: Link, paneId: number): LinkViewModel;
        itemViewModel(linkRep: Link, paneId: number, selected: boolean): ItemViewModel;
        parameterViewModel(parmRep: Parameter, previousValue: Value, paneId: number): ParameterViewModel;
        propertyViewModel(propertyRep: PropertyMember, id: string, previousValue: Value, paneId: number): PropertyViewModel;
    }

    app.service('viewModelFactory', function ($q: ng.IQService,
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
        $rootScope: ng.IRootScopeService) {

        var viewModelFactory = <IViewModelFactoryInternal>this;

        viewModelFactory.errorViewModel = (errorRep: ErrorRepresentation) => {
            const errorViewModel = new ErrorViewModel();
            errorViewModel.message = errorRep.message() || "An Error occurred";
            const stackTrace = errorRep.stackTrace();
            errorViewModel.stackTrace = !stackTrace || stackTrace.length === 0 ? ["Empty"] : stackTrace;
            return errorViewModel;
        };

        function initLinkViewModel(linkViewModel: LinkViewModel, linkRep: Link) {
            linkViewModel.title = linkRep.title();
            linkViewModel.color = color.toColorFromHref(linkRep.href());
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


        viewModelFactory.linkViewModel = (linkRep: Link, paneId: number) => {
            const linkViewModel = new LinkViewModel();
            linkViewModel.doClick = () => {
                // because may be clicking on menu already open so want to reset focus             
                urlManager.setMenu(linkRep.rel().parms[0].value, paneId);
                focusManager.focusOverrideOff();
                focusManager.focusOn(FocusTarget.SubAction, 0, paneId);
            };
            initLinkViewModel(linkViewModel, linkRep);
            return linkViewModel;
        };

        viewModelFactory.itemViewModel = (linkRep: Link, paneId: number, selected: boolean) => {
            const itemViewModel = new ItemViewModel();
            itemViewModel.doClick = (right?: boolean) => urlManager.setItem(linkRep, clickHandler.pane(paneId, right));
            initLinkViewModel(itemViewModel, linkRep);

            itemViewModel.selected = selected;

            itemViewModel.checkboxChange = (index) => {
                urlManager.setListItem(paneId, index, itemViewModel.selected);
                focusManager.focusOverrideOn(FocusTarget.CheckBox, index + 1, paneId);
            }


            return itemViewModel;
        };

        function addAutoAutoComplete(valueViewModel: ValueViewModel, currentChoice: ChoiceViewModel, id: string, currentValue: Value) {
            valueViewModel.hasAutoAutoComplete = true;

            const cache = $cacheFactory.get("recentlyViewed");

            valueViewModel.choice = currentChoice;

            // make sure current value is cached so can be recovered ! 

            const { returnType: key, reference: subKey } = valueViewModel;
            const dict = cache.get(key) || {}; // todo fix type !
            dict[subKey] = { value: currentValue, name: currentValue.toString() };
            cache.put(key, dict);

            // bind in autoautocomplete into prompt 

            valueViewModel.prompt = (st: string) => {
                const defer = $q.defer<ChoiceViewModel[]>();
                const filtered = _.filter(dict, (i: { value: Value, name: string }) =>
                    i.name.toString().toLowerCase().indexOf(st.toLowerCase()) > -1);
                const ccs = _.map(filtered, (i: { value: Value, name: string }) => ChoiceViewModel.create(i.value, id, i.name));

                defer.resolve(ccs);

                return defer.promise;
            };
        }


        viewModelFactory.parameterViewModel = (parmRep: Parameter, previousValue: Value, paneId: number) => {
            var parmViewModel = new ParameterViewModel();

            parmViewModel.parameterRep = parmRep;
            parmViewModel.type = parmRep.isScalar() ? "scalar" : "ref";
            parmViewModel.dflt = parmRep.default().toValueString();
            parmViewModel.optional = parmRep.extensions().optional();
            var required = "";
            if (!parmViewModel.optional) {
                required = "* ";
            }
            parmViewModel.description = required + parmRep.extensions().description();
            parmViewModel.message = "";
            parmViewModel.id = parmRep.parameterId();
            parmViewModel.argId = `${parmViewModel.id.toLowerCase() }`;
            parmViewModel.paneArgId = `${parmViewModel.argId}${paneId}`;
            parmViewModel.reference = "";

            parmViewModel.mask = parmRep.extensions().mask();
            parmViewModel.title = parmRep.extensions().friendlyName();
            parmViewModel.returnType = parmRep.extensions().returnType();
            parmViewModel.format = parmRep.extensions().format();
            parmViewModel.isCollectionContributed = parmRep.isCollectionContributed();
            parmViewModel.onPaneId = paneId;

            parmViewModel.drop = (newValue: IDraggableViewModel) => {
                context.isSubTypeOf(newValue.draggableType, parmViewModel.returnType).
                    then((canDrop: boolean) => {
                        if (canDrop) {
                            parmViewModel.setNewValue(newValue);
                        }
                    }
                    );
            };

            parmViewModel.choices = _.map(parmRep.choices(), (v, n) => ChoiceViewModel.create(v, parmRep.parameterId(), n));
            parmViewModel.hasChoices = parmViewModel.choices.length > 0;
            parmViewModel.hasPrompt = !!parmRep.promptLink() && !!parmRep.promptLink().arguments()["x-ro-searchTerm"];
            parmViewModel.hasConditionalChoices = !!parmRep.promptLink() && !parmViewModel.hasPrompt;
            parmViewModel.isMultipleChoices = (parmViewModel.hasChoices || parmViewModel.hasConditionalChoices) && parmRep.extensions().returnType() === "list";

            if (parmViewModel.hasPrompt || parmViewModel.hasConditionalChoices) {

                const promptRep = parmRep.getPrompts();
                if (parmViewModel.hasPrompt) {
                    parmViewModel.prompt = _.partial(context.prompt, promptRep, parmViewModel.id);
                    parmViewModel.minLength = parmRep.promptLink().extensions().minLength();
                }

                if (parmViewModel.hasConditionalChoices) {
                    parmViewModel.conditionalChoices = _.partial(context.conditionalChoices, promptRep, parmViewModel.id);
                    parmViewModel.arguments = _.object<_.Dictionary<Value>>(_.map(parmRep.promptLink().arguments(), (v: any, key) => [key, new Value(v.value)]));
                }
            }

            if (parmViewModel.hasChoices || parmViewModel.hasPrompt || parmViewModel.hasConditionalChoices || parmViewModel.isCollectionContributed) {

                function setCurrentChoices(vals: Value) {

                    const choicesToSet = _.map(vals.list(), val => ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null));

                    if (parmViewModel.hasPrompt || parmViewModel.hasConditionalChoices || parmViewModel.isCollectionContributed) {
                        parmViewModel.multiChoices = choicesToSet;
                    } else {
                        parmViewModel.multiChoices = _.filter(parmViewModel.choices, c => _.any(choicesToSet, choiceToSet => c.match(choiceToSet)));
                    }
                }

                function setCurrentChoice(val: Value) {
                    const choiceToSet = ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);

                    if (parmViewModel.hasPrompt || parmViewModel.hasConditionalChoices) {
                        parmViewModel.choice = choiceToSet;
                    } else {
                        parmViewModel.choice = _.find(parmViewModel.choices, c => c.match(choiceToSet));
                    }
                }
                if (previousValue || parmViewModel.dflt) {
                    const toSet = previousValue || parmRep.default();
                    if (parmViewModel.isMultipleChoices || parmViewModel.isCollectionContributed) {
                        setCurrentChoices(toSet);
                    } else {
                        setCurrentChoice(toSet);
                    }
                }
            } else {
                if (parmRep.extensions().returnType() === "boolean") {
                    parmViewModel.value = previousValue ? previousValue.toString().toLowerCase() === "true" : parmRep.default().scalar();
                } else if (parmRep.extensions().returnType() === "string" && parmRep.extensions().format() === "date-time" ) {
                    const rawValue = (previousValue ? previousValue.toString() : "") || parmViewModel.dflt || "";
                    const dateValue = Date.parse(rawValue);
                    parmViewModel.value = dateValue ? new Date(rawValue) : null; 
                } else {
                    parmViewModel.value = (previousValue ? previousValue.toString() : null) || parmViewModel.dflt || "";
                }
            }

            var remoteMask = parmRep.extensions().mask();

            if (remoteMask && parmRep.isScalar()) {
                const localFilter = mask.toLocalFilter(remoteMask);
                if (localFilter) {
                    // todo formatting will have to happen in directive - at lesat for dates - value is now date in that case
                    //parmViewModel.value = $filter(localFilter.name)(parmViewModel.value, localFilter.mask);
                    parmViewModel.formattedValue = parmViewModel.value ? $filter(localFilter.name)(parmViewModel.value.toString(), localFilter.mask) : "";

                }
            }

            if (parmViewModel.type === "ref" && !parmViewModel.hasPrompt && !parmViewModel.hasChoices && !parmViewModel.hasConditionalChoices) {

                let currentChoice: ChoiceViewModel = null;

                if (previousValue) {
                    currentChoice = ChoiceViewModel.create(previousValue, parmViewModel.id, previousValue.link() ? previousValue.link().title() : null);
                }
                else if (parmViewModel.dflt) {
                    let dflt = parmRep.default();
                    currentChoice = ChoiceViewModel.create(dflt, parmViewModel.id, dflt.link().title());
                }

                const currentValue = new Value(currentChoice ? { href: currentChoice.value, title: currentChoice.name } : "");

                addAutoAutoComplete(parmViewModel, currentChoice, parmViewModel.id, currentValue);
            }

            parmViewModel.color = parmViewModel.value ? color.toColorFromType(parmViewModel.returnType) : "";

            return parmViewModel;
        };

        viewModelFactory.actionViewModel = ($scope: ng.IScope, actionRep: ActionMember, routeData: PaneRouteData) => {
            var actionViewModel = new ActionViewModel();

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

            const parameters = _.pick(actionRep.parameters(), p => !p.isCollectionContributed()) as _.Dictionary<Parameter>;
            actionViewModel.parameters = _.map(parameters, parm => viewModelFactory.parameterViewModel(parm, parms[parm.parameterId()], paneId));

            actionViewModel.executeInvoke = (pps: ParameterViewModel[], right?: boolean) => {
                const parmMap = _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Value>;
                _.forEach(pps, p => urlManager.setParameterValue(actionRep.actionId(), p.parameterRep, p.getValue(), paneId, false));
                return context.invokeAction(actionRep, clickHandler.pane(paneId, right), parmMap);
            }

            // open dialog on current pane always - invoke action goes to pane indicated by click
            actionViewModel.doInvoke = actionRep.extensions().hasParams() ?
                (right?: boolean) => {
                    focusManager.focusOverrideOff();
                    urlManager.setDialog(actionRep.actionId(), paneId);
                } :
                (right?: boolean) => {
                    focusManager.focusOverrideOff();
                    const pps = actionViewModel.parameters;
                    actionViewModel.executeInvoke(pps, right);
                    // todo display error if fails on parent ?
                };

            return actionViewModel;
        };

        const currentDvms: DialogViewModel[] = [];

        function getDialogViewModel(paneId: number, actionMember: ActionMember) {
            const currentDvm = currentDvms[paneId];
            if (currentDvm && currentDvm.isSame(paneId, actionMember)) {
                return { dialogViewModel: currentDvm, ret: true };
            }
            const dvm = new DialogViewModel();
            currentDvms[paneId] = dvm;
            return { dialogViewModel: dvm, ret: false };
        }


        const currentOvms: DomainObjectViewModel[] = [];

        function getObjectViewModel(obj: DomainObjectRepresentation, routeData: PaneRouteData) {

            const paneId = routeData.paneId;
            const editing = routeData.edit;
            const currentOvm = currentOvms[paneId];
            if (editing && currentOvm && currentOvm.isSameEditView(paneId, obj, editing)) {
                return { objectViewModel: currentOvm, ret: true };
            }
            const ovm = new DomainObjectViewModel();
            currentOvms[paneId] = ovm;
            return { objectViewModel: ovm, ret: false };
        }

        const currentLvms: CollectionViewModel[] = [];

        function getCollectionViewModel(obj: ListRepresentation, routeData: PaneRouteData) {

            const paneId = routeData.paneId;
            const currentLvm = currentLvms[paneId];
            if (currentLvm && currentLvm.isSame(paneId, collectionId(routeData))) {
                return { collectionViewModel: currentLvm, ret: true };
            }
            const lvm = new CollectionViewModel();
            currentLvms[paneId] = lvm;
            return { collectionViewModel: lvm, ret: false };
        }


        function clearDialog(paneId: number, actionMember: ActionMember) {
            const currentDvm = currentDvms[paneId];
            if (currentDvm && currentDvm.isSame(paneId, actionMember)) {
                currentDvms[paneId] = null;
            }
        }

        function handleErrorResponse(err: ErrorMap, vm: MessageViewModel, vms: ValueViewModel[]) {

            let requiredFieldsMissing = false; // only show warning message if we have nothing else 
            let fieldValidationErrors = false;

            _.each(vms, vmi => {
                const errorValue = err.valuesMap()[vmi.id];

                if (errorValue) {
                    vmi.value = errorValue.value.toValueString();

                    const reason = errorValue.invalidReason;
                    if (reason) {
                        if (reason === "Mandatory") {
                            const r = "REQUIRED";
                            requiredFieldsMissing = true;
                            vmi.description = vmi.description.indexOf(r) === 0 ? vmi.description : `${r} ${vmi.description}`;
                        } else {
                            vmi.message = reason;
                            fieldValidationErrors = true;
                        }
                    }
                }
            });

            let msg = "";
            if (err.invalidReason()) msg += err.invalidReason();
            if (requiredFieldsMissing) msg += "Please complete REQUIRED fields. ";
            if (fieldValidationErrors) msg += "See field validation message(s). ";
            if (!msg) msg = err.warningMessage;
            vm.message = msg;

        }


        viewModelFactory.dialogViewModel = ($scope: ng.IScope, actionViewModel: ActionViewModel, routeData: PaneRouteData) => {

            const paneId = routeData.paneId;
            const actionMember = actionViewModel.actionRep;
            const { dialogViewModel, ret } = getDialogViewModel(paneId, actionMember);

            if (ret) {
                return dialogViewModel;
            }

            dialogViewModel.actionViewModel = actionViewModel;

            const fields = routeData.dialogFields;
            const parameters = _.filter(actionViewModel.parameters, p => !p.isCollectionContributed);
            dialogViewModel.parameters = _.map(parameters, p => viewModelFactory.parameterViewModel(p.parameterRep, fields[p.parameterRep.parameterId()], paneId));


            dialogViewModel.action = actionMember;
            dialogViewModel.title = actionMember.extensions().friendlyName();
            dialogViewModel.isQueryOnly = actionMember.invokeLink().method() === "GET";
            dialogViewModel.message = "";

            dialogViewModel.onPaneId = paneId;

            const setParms = () =>
                _.forEach(dialogViewModel.parameters, p => urlManager.setFieldValue(actionMember.actionId(), p.parameterRep, p.getValue(), paneId, false));

            const executeInvoke = (right?: boolean) => {
                const pps = dialogViewModel.parameters;
                _.forEach(pps, p => urlManager.setFieldValue(actionMember.actionId(), p.parameterRep, p.getValue(), paneId, false));
                return actionViewModel.executeInvoke(pps, right);
            }

            dialogViewModel.doInvoke = (right?: boolean) =>
                executeInvoke(right).then((err: ErrorMap) => {
                    if (err.containsError()) {
                        handleErrorResponse(err, dialogViewModel, dialogViewModel.parameters);
                    } else {
                        dialogViewModel.doClose();
                    }
                });


            const deregisterLocationWatch = $scope.$on("$locationChangeStart", setParms);
            const deregisterSearchWatch = $scope.$watch(() => $location.search(), setParms, true);

            dialogViewModel.doClose = () => {
                deregisterLocationWatch();
                deregisterSearchWatch();
                clearDialog(paneId, actionMember);
                urlManager.closeDialog(paneId);
            };


            dialogViewModel.clearMessages = () => {
                this.message = "";
                _.each(actionViewModel.parameters, parm => parm.clearMessage());
            }

            return dialogViewModel;
        };


        viewModelFactory.propertyViewModel = (propertyRep: PropertyMember, id: string, previousValue: Value, paneId: number) => {
            const propertyViewModel = new PropertyViewModel();


            propertyViewModel.title = propertyRep.extensions().friendlyName();
            propertyViewModel.optional = propertyRep.extensions().optional();
            propertyViewModel.onPaneId = paneId;
            propertyViewModel.propertyRep = propertyRep;

            const required = propertyViewModel.optional ? "" : "* ";

            propertyViewModel.description = required + propertyRep.extensions().description();

            const value = previousValue || propertyRep.value();

            if (propertyRep.extensions().returnType() === "string" && propertyRep.extensions().format() === "date-time") {
                const rawValue = value ? value.toString() : "";
                const dateValue = Date.parse(rawValue);
                propertyViewModel.value = dateValue ? new Date(rawValue) : null;
            } else {
                propertyViewModel.value = propertyRep.isScalar() ? value.scalar() : value.isNull() ? propertyViewModel.description : value.toString();
            }


            propertyViewModel.type = propertyRep.isScalar() ? "scalar" : "ref";
            propertyViewModel.returnType = propertyRep.extensions().returnType();
            propertyViewModel.format = propertyRep.extensions().format();
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
            propertyViewModel.color = propertyViewModel.value ? color.toColorFromType(propertyRep.extensions().returnType()) : "";

            propertyViewModel.id = id;
            propertyViewModel.argId = `${id.toLowerCase() }`;
            propertyViewModel.paneArgId = `${propertyViewModel.argId}${paneId}`;
            propertyViewModel.isEditable = !propertyRep.disabledReason();
            propertyViewModel.choices = [];
            propertyViewModel.hasPrompt = propertyRep.hasPrompt();

            if (propertyRep.hasChoices()) {

                const choices = propertyRep.choices();

                if (choices) {
                    propertyViewModel.choices = _.map(choices, (v, n) => ChoiceViewModel.create(v, id, n));
                }
            }

            propertyViewModel.hasChoices = propertyViewModel.choices.length > 0;
            propertyViewModel.hasPrompt = !!propertyRep.promptLink() && !!propertyRep.promptLink().arguments()["x-ro-searchTerm"];
            propertyViewModel.hasConditionalChoices = !!propertyRep.promptLink() && !propertyViewModel.hasPrompt;

            if (propertyViewModel.hasPrompt || propertyViewModel.hasConditionalChoices) {
                const promptRep: PromptRepresentation = propertyRep.getPrompts();

                if (propertyViewModel.hasPrompt) {
                    propertyViewModel.prompt = _.partial(context.prompt, promptRep, id);
                    propertyViewModel.minLength = propertyRep.promptLink().extensions().minLength();
                }

                if (propertyViewModel.hasConditionalChoices) {
                    propertyViewModel.conditionalChoices = _.partial(context.conditionalChoices, promptRep, id);
                    propertyViewModel.arguments = _.object<_.Dictionary<Value>>(_.map(propertyRep.promptLink().arguments(), (v: any, key) => [key, new Value(v.value)]));
                }
            }

            if (propertyViewModel.hasChoices || propertyViewModel.hasPrompt || propertyViewModel.hasConditionalChoices) {

                var currentChoice: ChoiceViewModel = ChoiceViewModel.create(value, id);

                if (propertyViewModel.hasPrompt || propertyViewModel.hasConditionalChoices) {
                    propertyViewModel.choice = currentChoice;
                } else {
                    propertyViewModel.choice = _.find(propertyViewModel.choices, (c: ChoiceViewModel) => c.match(currentChoice));
                }
            }

            if (propertyRep.isScalar()) {
                const remoteMask = propertyRep.extensions().mask();
                const localFilter = mask.toLocalFilter(remoteMask) || mask.defaultLocalFilter(propertyRep.extensions().format());
                if (localFilter) {
                    propertyViewModel.formattedValue = $filter(localFilter.name)(propertyViewModel.value, localFilter.mask);
                } else {
                    propertyViewModel.formattedValue = propertyViewModel.value ? propertyViewModel.value.toString() : "";
                }
            }

            // if a reference and no way to set (ie not choices or autocomplete) use autoautocomplete
            if (propertyViewModel.type === "ref" && !propertyViewModel.hasPrompt && !propertyViewModel.hasChoices && !propertyViewModel.hasConditionalChoices) {
                addAutoAutoComplete(propertyViewModel, ChoiceViewModel.create(value, id), id, value);
            }

            return propertyViewModel;
        };

        function getItems($scope: ng.IScope, collectionViewModel: CollectionViewModel, links: Link[], populateItems: boolean, routeData: PaneRouteData) {
            const selectedItems = routeData.selectedItems;
            if (populateItems) {
                return _.map(links, (link, i) => {
                    const itemViewModel = viewModelFactory.itemViewModel(link, collectionViewModel.onPaneId, selectedItems[i]);
                    const tempTgt = link.getTarget();
                    repLoader.populate<DomainObjectRepresentation>(tempTgt).
                        then((obj: DomainObjectRepresentation) => {
                            itemViewModel.target = viewModelFactory.domainObjectViewModel($scope, obj, routeData);

                            if (!collectionViewModel.header) {
                                collectionViewModel.header = _.map(itemViewModel.target.properties, property => property.title);
                                focusManager.focusOverrideOff();
                                focusManager.focusOn(FocusTarget.TableItem, 0, urlManager.currentpane());
                            }
                        });
                    return itemViewModel;
                });
            } else {
                return _.map(links, (link, i) => viewModelFactory.itemViewModel(link, collectionViewModel.onPaneId, selectedItems[i]));
            }
        }

        function create($scope: ng.IScope, collectionRep: CollectionMember, routeData: PaneRouteData) {
            const collectionViewModel = new CollectionViewModel();

            const links = collectionRep.value();
            const paneId = routeData.paneId;
            const state = routeData.collections[collectionRep.collectionId()];

            collectionViewModel.collectionRep = collectionRep;
            collectionViewModel.onPaneId = paneId;

            collectionViewModel.title = collectionRep.extensions().friendlyName();
            collectionViewModel.size = links.length;
            collectionViewModel.pluralName = collectionRep.extensions().pluralName();
            collectionViewModel.color = color.toColorFromType(collectionRep.extensions().elementType());

            collectionViewModel.items = getItems($scope, collectionViewModel, links, state === CollectionViewState.Table, routeData);

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

            const setState = _.partial(urlManager.setCollectionMemberState, paneId, collectionRep.collectionId());
            collectionViewModel.doSummary = () => setState(CollectionViewState.Summary);
            collectionViewModel.doList = () => setState(CollectionViewState.List);
            collectionViewModel.doTable = () => setState(CollectionViewState.Table);

            return collectionViewModel;
        }

        function collectionId(routeData: PaneRouteData) {
            const parmString = _.reduce(routeData.actionParams, (result, n, k) => result + k + n.toValueString(), "");

            return `${routeData.menuId || "mid"}${routeData.actionId || "aid"}${routeData.paneId || "pid"}${routeData.page || "pg"}${routeData.pageSize || "ps"}${routeData.state || "st"}${parmString || "pms"} `;
        }

        function createFromList($scope: ng.IScope, listRep: ListRepresentation, routeData: PaneRouteData, recreate: (page: number, newPageSize: number, newState: CollectionViewState) => void) {

            const { collectionViewModel, ret } = getCollectionViewModel(listRep, routeData);
            if (ret) {
                return collectionViewModel;
            }

            const links = listRep.value();
            const paneId = routeData.paneId;
            const state = routeData.state;

            collectionViewModel.id = collectionId(routeData);

            collectionViewModel.collectionRep = listRep;
            collectionViewModel.onPaneId = paneId;

            collectionViewModel.pluralName = "Objects";
            collectionViewModel.items = getItems($scope, collectionViewModel, links, state === CollectionViewState.Table, routeData);

            const page = listRep.pagination().page;
            const pageSize = listRep.pagination().pageSize;
            const numPages = listRep.pagination().numPages;
            const totalCount = listRep.pagination().totalCount;
            const count = links.length;

            collectionViewModel.size = count;

            collectionViewModel.description = () => `Page ${page} of ${numPages}; viewing ${count} of ${totalCount} items`;


            const actions = listRep.actionMembers();
            collectionViewModel.actions = _.map(actions, action => viewModelFactory.actionViewModel($scope, action, routeData));

            // todo do more elegantly 

            _.forEach(collectionViewModel.actions, a => {

                const wrappedInvoke = a.executeInvoke;
                a.executeInvoke = (pps: ParameterViewModel[], right?: boolean) => {
                    const selected = _.filter(collectionViewModel.items, i => i.selected);

                    if (selected.length === 0) {
                        return $q.when(new ErrorMap({}, 0, "Must select items for collection contributed action"));
                    }
                    const parms = _.values(a.actionRep.parameters()) as Parameter[];
                    const contribParm = _.find(parms, p => p.isCollectionContributed());
                    const parmValue = new Value(_.map(selected, i => i.link));
                    const collectionParmVm = viewModelFactory.parameterViewModel(contribParm, parmValue, paneId);

                    const allpps = _.clone(pps);
                    allpps.push(collectionParmVm);

                    return wrappedInvoke(allpps, right);
                }

                a.doInvoke = _.keys(a.actionRep.parameters()).length > 1 ?
                    (right?: boolean) => {
                        focusManager.focusOverrideOff();
                        urlManager.setDialog(a.actionRep.actionId(), paneId);
                    } :
                    (right?: boolean) => {
                        a.executeInvoke([], right).then((errorMap: ErrorMap) => {
                            if (errorMap.containsError()) {
                                collectionViewModel.messages = errorMap.invalidReason() || errorMap.warningMessage;
                            } else {
                                collectionViewModel.messages = "";
                            };
                        });
                    };
            });


            collectionViewModel.toggleActionMenu = () => {
                focusManager.focusOverrideOff();
                urlManager.toggleObjectMenu(paneId);
            };

            const setPage = (newPage: number, newState: CollectionViewState) => {
                focusManager.focusOverrideOff();
                recreate(newPage, pageSize, newState);
            }

            collectionViewModel.pageNext = () => setPage(page < numPages ? page + 1 : page, state);
            collectionViewModel.pagePrevious = () => setPage(page > 1 ? page - 1 : page, state);
            collectionViewModel.pageFirst = () => setPage(1, state);
            collectionViewModel.pageLast = () => setPage(numPages, state);

            const earlierDisabled = () => page === 1 || numPages === 1;
            const laterDisabled = () => page === numPages || numPages === 1;

            collectionViewModel.pageFirstDisabled = earlierDisabled;
            collectionViewModel.pageLastDisabled = laterDisabled;
            collectionViewModel.pageNextDisabled = laterDisabled;
            collectionViewModel.pagePreviousDisabled = earlierDisabled;

            const setState = _.partial(urlManager.setListState, paneId);

            collectionViewModel.doSummary = () => setState(CollectionViewState.Summary);
            collectionViewModel.doList = () => setPage(page, CollectionViewState.List);
            collectionViewModel.doTable = () => setPage(page, CollectionViewState.Table);

            collectionViewModel.reload = () => {
                currentLvms[paneId] = null;
                context.clearCachedList(paneId, routeData.page, routeData.pageSize);
                setPage(page, state);
            };


            return collectionViewModel;
        }


        viewModelFactory.collectionViewModel = ($scope: ng.IScope, collection: CollectionMember | ListRepresentation, routeData: PaneRouteData, recreate: (page: number) => void) => {
            let collectionVm: CollectionViewModel = null;

            if (collection instanceof CollectionMember) {
                collectionVm = create($scope, collection, routeData);
            }

            if (collection instanceof ListRepresentation) {
                collectionVm = createFromList($scope, collection, routeData, recreate);
            }

            return collectionVm;
        };

        viewModelFactory.collectionPlaceholderViewModel = (page: number, reload: () => void) => {
            const collectionPlaceholderViewModel = new CollectionPlaceholderViewModel();

            collectionPlaceholderViewModel.description = () => `Page ${page}`;
            collectionPlaceholderViewModel.reload = reload;
            return collectionPlaceholderViewModel;
        }

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

        viewModelFactory.menusViewModel = (menusRep: MenusRepresentation, paneId: number) => {
            const menusViewModel = new MenusViewModel();

            menusViewModel.title = "Menus";
            menusViewModel.color = "bg-color-darkBlue";
            menusViewModel.items = _.map(menusRep.value(), link => viewModelFactory.linkViewModel(link, paneId));
            return menusViewModel;
        };


        viewModelFactory.serviceViewModel = ($scope: ng.IScope, serviceRep: DomainObjectRepresentation, routeData: PaneRouteData) => {
            const serviceViewModel = new ServiceViewModel();
            const actions = serviceRep.actionMembers();
            serviceViewModel.serviceId = serviceRep.serviceId();
            serviceViewModel.title = serviceRep.title();
            serviceViewModel.actions = _.map(actions, action => viewModelFactory.actionViewModel($scope, action, routeData));
            serviceViewModel.color = color.toColorFromType(serviceRep.serviceId());

            return serviceViewModel;
        };
  
        // seperate function so we can reuse in reload
        function setupDomainObjectViewModel(objectViewModel: DomainObjectViewModel, $scope: INakedObjectsScope, objectRep: DomainObjectRepresentation, routeData: PaneRouteData) {


            const props: _.Dictionary<Value> = routeData.edit ? routeData.props : {};
            const editing = routeData.edit;
            const paneId = routeData.paneId;

            objectViewModel.domainObject = objectRep;
            objectViewModel.onPaneId = paneId;
            objectViewModel.isTransient = !!objectRep.persistLink();
            objectViewModel.color = color.toColorFromType(objectRep.domainType());
            objectViewModel.domainType = objectRep.domainType();
            objectViewModel.instanceId = objectRep.instanceId();
            objectViewModel.draggableType = objectViewModel.domainType;

            objectViewModel.canDropOn = (targetType: string) => context.isSubTypeOf(targetType, objectViewModel.domainType);

            const properties = objectRep.propertyMembers();
            const collections = objectRep.collectionMembers();
            const actions = objectRep.actionMembers();

            objectViewModel.title = objectViewModel.isTransient ? `Unsaved ${objectRep.extensions().friendlyName() }` : objectRep.title();

            objectViewModel.message = "";

            objectViewModel.actions = _.map(actions, action => viewModelFactory.actionViewModel($scope, action, routeData));
            objectViewModel.properties = _.map(properties, (property, id) => viewModelFactory.propertyViewModel(property, id, props[id], paneId));
            objectViewModel.collections = _.map(collections, collection => viewModelFactory.collectionViewModel($scope, collection, routeData, (page: number, newPageSize: number) => { }));

           
            objectViewModel.toggleActionMenu = () => {
                focusManager.focusOverrideOff();
                urlManager.toggleObjectMenu(paneId);
            };

             // for dropping
            const link = objectRep.selfLink();
            if (link) {
                // not transient - can't drag transients so no need to set up IDraggable members on transients
                //link.set("title", objectViewModel.title);
                link.wrapped.title = objectViewModel.title;


                const value = new Value(link);

                objectViewModel.value = value.toString();
                objectViewModel.reference = value.toValueString();
                objectViewModel.choice = ChoiceViewModel.create(value, "");
            }

            if (editing || objectViewModel.isTransient) {

                const editProperties = _.filter(objectViewModel.properties, p => p.isEditable);
                const setProperties = () => _.forEach(editProperties, p => urlManager.setPropertyValue(objectRep, p.propertyRep, p.getValue(), paneId, false));
                const deregisterLocationWatch = $scope.$on("$locationChangeStart", setProperties);
                const deregisterSearchWatch = $scope.$watch(() => $location.search(), setProperties, true);

                const cancelHandler = objectViewModel.isTransient ?
                    () => urlManager.popUrlState(paneId) :
                    () => urlManager.setObjectEdit(false, paneId);


                objectViewModel.editComplete = () => {
                    deregisterLocationWatch();
                    deregisterSearchWatch();
                };

                objectViewModel.doEditCancel = () => {
                    objectViewModel.editComplete();
                    cancelHandler();
                };

                const saveHandler = objectViewModel.isTransient ? context.saveObject : context.updateObject;
                objectViewModel.doSave = viewObject => {

                    const pps = _.filter(objectViewModel.properties, property => property.isEditable);
                    const propMap = _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Value>;

                    saveHandler(objectRep, propMap, paneId, viewObject).then((err: ErrorMap) => {
                        if (err.containsError()) {
                            handleErrorResponse(err, objectViewModel, objectViewModel.properties);
                        }
                    });
                };
                objectViewModel.isInEdit = true;
            }

            objectViewModel.doEdit = () => {
                context.reloadObject(paneId, objectRep).
                    then((updatedObject: DomainObjectRepresentation) => {
                        setupDomainObjectViewModel(objectViewModel, $scope, updatedObject, routeData);
                        $scope.object = objectViewModel;
                        urlManager.pushUrlState(paneId);
                        urlManager.setObjectEdit(true, paneId);
                    });
            }

            objectViewModel.doReload = (refreshScope?: boolean) =>
                context.reloadObject(paneId, objectRep).
                    then((updatedObject: DomainObjectRepresentation) => {
                        setupDomainObjectViewModel(objectViewModel, $scope, updatedObject, routeData);
                        if (refreshScope) {
                            $scope.object = objectViewModel;
                        }
                    });

        };


        viewModelFactory.domainObjectViewModel = ($scope: INakedObjectsScope, objectRep: DomainObjectRepresentation, routeData: PaneRouteData): DomainObjectViewModel => {
            const {objectViewModel, ret} = getObjectViewModel(objectRep, routeData);
            if (ret) {
                return objectViewModel;
            }

            setupDomainObjectViewModel(objectViewModel, $scope, objectRep, routeData);
            return objectViewModel;
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
                tvm.swapPanes = () => urlManager.swapPanes();
                tvm.singlePane = (right?: boolean) => {
                    urlManager.singlePane(clickHandler.pane(1, right));
                    focusManager.refresh(1);
                };
                tvm.cicero = () => {
                    urlManager.singlePane(clickHandler.pane(1));
                    urlManager.cicero();
                }
                tvm.template = appBarTemplate;
                tvm.footerTemplate = footerTemplate;

                $rootScope.$on("ajax-change", (event, count) =>
                    tvm.loading = count > 0 ? "Loading..." : "");
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
                cvm.parseInput = (input: string) => {
                    cvm.previousInput = input;
                    commandFactory.parseInput(input, cvm);
                };
                cvm.renderHome = (routeData: PaneRouteData) => {
                    if (routeData.menuId) {
                        context.getMenu(routeData.menuId)
                            .then((menu: MenuRepresentation) => {
                                let output = ""; //TODO: use builder
                                output += menu.title() + " menu" + ". ";
                                output += renderActionDialogIfOpen(menu, routeData);
                                cvm.clearInput();
                                cvm.output = output;
                            });
                    }
                    else {
                        cvm.clearInput();
                        cvm.output = "Welcome to Cicero";
                    }

                };
                cvm.renderObject = (routeData: PaneRouteData) => {
                    const [domainType, ...id] = routeData.objectId.split("-");
                    context.getObject(1, domainType, id) //TODO: move following code out into a ICireroRenderers service with methods for rendering each context type
                        .then((obj: DomainObjectRepresentation) => {
                            let output = "";
                            const openCollIds = openCollectionIds(routeData);
                            if (_.any(openCollIds)) {
                                const id = openCollIds[0];
                                const coll = obj.collectionMember(id);
                                output += "Collection: " + coll.extensions().friendlyName();
                                output += " on " + Helpers.typePlusTitle(obj) + ",  "
                                  switch (coll.size()) {
                                    case 0:
                                        output += "empty";
                                        break;
                                    case 1:
                                        output += "1 item";
                                        break;
                                    default:
                                        output += coll.size() + " items";
                                }                             
                            } else {
                                if (routeData.edit) {
                                    output += "Editing ";
                                }
                                output += Helpers.typePlusTitle(obj) + ". ";
                                output += renderActionDialogIfOpen(obj, routeData);
                            }
                            cvm.clearInput();
                            cvm.output = output;
                        });
                };
                cvm.renderList = (routeData: PaneRouteData) => {
                    const listPromise = context.getListFromMenu(1, routeData.menuId, routeData.actionId, routeData.actionParams, routeData.page, routeData.pageSize);
                    listPromise.then((list: ListRepresentation) => {
                        const page = list.pagination().page;
                        const numPages = list.pagination().numPages;
                        const count = list.value().length;
                        const totalCount = list.pagination().totalCount;
                        const description = "Page " + page + " of " + numPages + " containing " + count + " of " + totalCount + " items";
                        context.getMenu(routeData.menuId).then((menu: MenuRepresentation) => {
                            const actionMember = menu.actionMember(routeData.actionId);
                            const actionName = actionMember.extensions().friendlyName();
                            cvm.clearInput();
                            cvm.output = "Result from "+actionName + ": " + description;
                        });
                    });
                };
                cvm.renderError = () => {
                    const err = context.getError();
                    cvm.clearInput();
                    cvm.output = "Sorry, an application error has occurred. " + err.message();
                };
            }
            return cvm;
        };


    });

    //Returns collection Ids for any collections on an object that are currently in List or Table mode
    export function openCollectionIds(routeData: PaneRouteData ): string[] {
        return _.filter(_.keys(routeData.collections), k => routeData.collections[k] != CollectionViewState.Summary);
    }

    function renderActionDialogIfOpen(
        repWithActions: IHasActions,
        routeData: PaneRouteData): string {
        let output = "";
        if (routeData.dialogId) {
            const actionMember = repWithActions.actionMember(routeData.dialogId);
            const actionName = actionMember.extensions().friendlyName();
            output += "Action dialog: " + actionName + ". ";
            _.forEach(routeData.dialogFields, (value, key) => {
                output += Helpers.friendlyNameForParam(actionMember, key) + ": ";
                output += value.toString() || "empty";
                output += ", ";
            });
        }
        return output;
    }

}