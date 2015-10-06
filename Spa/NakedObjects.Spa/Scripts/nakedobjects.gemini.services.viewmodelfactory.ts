//Copyright 2014 Stef Cascarini, Dan Haywood, Richard Pawson
//Licensed under the Apache License, Version 2.0(the
//"License"); you may not use this file except in compliance
//with the License.You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing,
//software distributed under the License is distributed on an
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//KIND, either express or implied.See the License for the
//specific language governing permissions and limitations
//under the License.

/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular.Gemini{

    export interface IViewModelFactory {
        toolBarViewModel($scope) : ToolBarViewModel;

        errorViewModel(errorRep: ErrorRepresentation): ErrorViewModel;
        linkViewModel(linkRep: Link, paneId : number): LinkViewModel;
        itemViewModel(linkRep: Link, paneId : number): ItemViewModel;     
        actionViewModel(actionRep: ActionMember, paneId : number): ActionViewModel;
        dialogViewModel(actionRep: ActionMember, paneId : number): DialogViewModel;

        collectionViewModel(collection: CollectionMember, state: CollectionViewState, paneId : number): CollectionViewModel;
        collectionViewModel(collection: ListRepresentation, state: CollectionViewState, paneId : number): CollectionViewModel;

        parameterViewModel(parmRep: Parameter, previousValue: string): ParameterViewModel;
        propertyViewModel(propertyRep: PropertyMember, id: string, paneId): PropertyViewModel;

        servicesViewModel(servicesRep: DomainServicesRepresentation): ServicesViewModel;
        menusViewModel(menusRep: MenusRepresentation, paneId : number): MenusViewModel;
        serviceViewModel(serviceRep: DomainObjectRepresentation, paneId : number): ServiceViewModel;
        domainObjectViewModel(objectRep: DomainObjectRepresentation, collectionStates: { [index: string]: CollectionViewState }, paneId : number): DomainObjectViewModel;
        ciceroViewModel(wrapped: any): CiceroViewModel;
    }

    app.service('viewModelFactory', function ($q: ng.IQService,
        $location: ng.ILocationService,
        $filter: ng.IFilterService,
        $cacheFactory: ng.ICacheFactoryService,
        repLoader: IRepLoader,
        color: IColor,
        context: IContext,
        mask: IMask,    
        urlManager: IUrlManager,
        navigation: INavigation,
        clickHandler : IClickHandler) {

        var viewModelFactory = <IViewModelFactory>this;
        
        viewModelFactory.errorViewModel = (errorRep: ErrorRepresentation) => {
            const errorViewModel = new ErrorViewModel();
            errorViewModel.message = errorRep.message() || "An Error occurred";
            const stackTrace = errorRep.stacktrace();
            errorViewModel.stackTrace = !stackTrace || stackTrace.length === 0 ? ["Empty"] : stackTrace;
            return errorViewModel;
        };

        function initLinkViewModel(linkViewModel: LinkViewModel, linkRep: Link) {
            linkViewModel.title = linkRep.title();
            linkViewModel.color = color.toColorFromHref(linkRep.href());

            //temp comment out
            // linkViewModel.domainType = linkRep.type().domainType;

            // for dropping 
            const value = new Value(linkRep);

            linkViewModel.value = value.toString();
            linkViewModel.reference = value.toValueString();
            linkViewModel.choice = ChoiceViewModel.create(value, "");
        }


        viewModelFactory.linkViewModel = (linkRep: Link, paneId : number) => {
            const linkViewModel = new LinkViewModel();
            linkViewModel.doClick = () => urlManager.setMenu(linkRep.rel().parms[0].value, paneId);
            initLinkViewModel(linkViewModel, linkRep);
            return linkViewModel;
        };
   
        viewModelFactory.itemViewModel = (linkRep: Link, paneId : number) => {
            const itemViewModel = new ItemViewModel();
            itemViewModel.doClick = (right?: boolean) => urlManager.setItem(linkRep, clickHandler.pane(paneId, right));  
            initLinkViewModel(itemViewModel, linkRep);
            
            return itemViewModel;
        };
       
        function addAutoAutoComplete(valueViewModel: ValueViewModel, currentChoice : ChoiceViewModel, id : string, currentValue : Value) {
            valueViewModel.hasAutoAutoComplete = true;

            const cache = $cacheFactory.get("recentlyViewed");

            valueViewModel.choice = currentChoice;

            // make sure current value is cached so can be recovered ! 

            const { returnType: key, reference : subKey } = valueViewModel;
            const dict = <any> cache.get(key) || {}; // todo fix type !
            dict[subKey] = { value: currentValue, name : currentValue.toString() };
            cache.put(key, dict);

            // bind in autoautocomplete into prompt 

            valueViewModel.prompt = (st: string) => {
                const defer = $q.defer<ChoiceViewModel[]>();
                const filtered = _.filter(dict, (i: { value: Value, name : string }) =>
                    i.name.toString().toLowerCase().indexOf(st.toLowerCase()) > -1);
                const ccs = _.map(filtered, (i: { value: Value, name : string }) => ChoiceViewModel.create(i.value, id, i.name));

                defer.resolve(ccs);

                return defer.promise;
            }

        }

         // tested
        viewModelFactory.parameterViewModel = (parmRep: Parameter,  previousValue: string) => {
            var parmViewModel = new ParameterViewModel();

            parmViewModel.type = parmRep.isScalar() ? "scalar" : "ref";
            parmViewModel.dflt = parmRep.default().toValueString();
            parmViewModel.message = "";
            parmViewModel.id = parmRep.parameterId();
            parmViewModel.argId = parmViewModel.id.toLowerCase();
            parmViewModel.reference = "";

            parmViewModel.mask = parmRep.extensions()["x-ro-nof-mask"];
            parmViewModel.title = parmRep.extensions().friendlyName;
            parmViewModel.returnType = parmRep.extensions().returnType;
            parmViewModel.format = parmRep.extensions().format;

            parmViewModel.choices = _.map(parmRep.choices(), (v, n) => {
                return ChoiceViewModel.create(v, parmRep.parameterId(), n);
            });

            parmViewModel.hasChoices = parmViewModel.choices.length > 0;
            parmViewModel.hasPrompt = !!parmRep.promptLink() && !!parmRep.promptLink().arguments()["x-ro-searchTerm"];
            parmViewModel.hasConditionalChoices = !!parmRep.promptLink() && !parmViewModel.hasPrompt;
            parmViewModel.isMultipleChoices = (parmViewModel.hasChoices || parmViewModel.hasConditionalChoices) && parmRep.extensions().returnType === "list";

            if (parmViewModel.hasPrompt || parmViewModel.hasConditionalChoices) {

                var promptRep = parmRep.getPrompts();
                if (parmViewModel.hasPrompt) {
                    parmViewModel.prompt = <(st: string) => ng.IPromise<ChoiceViewModel[]>> _.partial(context.prompt, promptRep, parmViewModel.id);
                    parmViewModel.minLength = parmRep.promptLink().extensions().minLength;
                }

                if (parmViewModel.hasConditionalChoices) {
                    parmViewModel.conditionalChoices = <(args: IValueMap) => ng.IPromise<ChoiceViewModel[]>> _.partial(context.conditionalChoices, promptRep, parmViewModel.id);
                    parmViewModel.arguments = _.object<IValueMap>(_.map(<_.Dictionary<Object>>parmRep.promptLink().arguments(), (v: any, key) => [key, new Value(v.value)]));
                }
            }

            if (parmViewModel.hasChoices || parmViewModel.hasPrompt || parmViewModel.hasConditionalChoices) {
                
                if (parmViewModel.isMultipleChoices) {                
                    parmViewModel.setSelectedChoice = () => {
                        var search = parmViewModel.getMemento();
                        _.forEach(parmViewModel.multiChoices, (c) => {
                            context.setSelectedChoice(parmViewModel.id, search, c);
                        });  
                    };         
                } else {
                   
                    parmViewModel.setSelectedChoice = () =>  {
                        context.setSelectedChoice(parmViewModel.id, parmViewModel.getMemento(), parmViewModel.choice);
                    };
                }

                function setCurrentChoices(choices : ChoiceViewModel[]) {
                    if (parmViewModel.hasPrompt || parmViewModel.hasConditionalChoices) {
                        parmViewModel.multiChoices = choices;
                    } else {
                        parmViewModel.multiChoices = _.filter(parmViewModel.choices, (c: ChoiceViewModel) => {
                            return _.any(choices, (cvm: ChoiceViewModel) => {
                                return c.match(cvm);
                            });
                        });
                    }
                }

                function setCurrentChoice(choice: ChoiceViewModel) {
                    if (parmViewModel.hasPrompt || parmViewModel.hasConditionalChoices) {
                        parmViewModel.choice = choice;
                    } else {
                        parmViewModel.choice = _.find(parmViewModel.choices, (c: ChoiceViewModel) => {
                            return c.match(choice);
                        });
                    }
                }

                if (previousValue) {                            
                    if (parmViewModel.isMultipleChoices) {
                        var scs = context.getSelectedChoice(parmViewModel.id, previousValue);
                        setCurrentChoices(scs);
                    } else {
                        var sc = context.getSelectedChoice(parmViewModel.id, previousValue).pop();
                        setCurrentChoice(sc);
                    }
                } else if (parmViewModel.dflt) {
                    let dflt = parmRep.default();
                  
                    if (parmViewModel.isMultipleChoices) {
                        var dfltChoices = _.map(dflt.list(), (v) => {
                            return ChoiceViewModel.create(v, parmViewModel.id, v.link() ? v.link().title() : null);
                        });
                        setCurrentChoices(dfltChoices);
                    } else {
                        var dfltChoice = ChoiceViewModel.create(dflt, parmViewModel.id, dflt.link() ? dflt.link().title() : null);
                        setCurrentChoice(dfltChoice);
                    }
                }
                // clear any previous 
                context.clearSelectedChoice(parmViewModel.id);
               
            } else {
                if (parmRep.extensions().returnType === "boolean") {
                    parmViewModel.value = previousValue ? previousValue.toLowerCase() === 'true' : parmRep.default().scalar();
                } else {
                    parmViewModel.value = previousValue || parmViewModel.dflt;
                }
            }

            var remoteMask = parmRep.extensions()["x-ro-nof-mask"];

            if (remoteMask && parmRep.isScalar()) {
                var localFilter = mask.toLocalFilter(remoteMask);
                if (localFilter) {
                    parmViewModel.value = $filter(localFilter.name)(parmViewModel.value, localFilter.mask);
                }
            }

            if (parmViewModel.type === "ref" && !parmViewModel.hasPrompt && !parmViewModel.hasChoices && !parmViewModel.hasConditionalChoices) {

                var currentChoice : ChoiceViewModel = null;

                if (previousValue) {
                    currentChoice = context.getSelectedChoice(parmViewModel.id, previousValue).pop();
                }
                else if (parmViewModel.dflt) {
                    let dflt = parmRep.default();
                    currentChoice =  ChoiceViewModel.create(dflt, parmViewModel.id,  dflt.link().title());
                }
                context.clearSelectedChoice(parmViewModel.id);

                var currentValue = new Value( currentChoice ?  { href: currentChoice.value, title : currentChoice.name } : "");
              
                addAutoAutoComplete(parmViewModel, currentChoice, parmViewModel.id, currentValue);
            } 

            parmViewModel.color = parmViewModel.value ? color.toColorFromType(parmViewModel.returnType) : "";

            return parmViewModel;
        };

        // tested
        viewModelFactory.actionViewModel = (actionRep: ActionMember, paneId : number) => {
            var actionViewModel = new ActionViewModel();
            
            actionViewModel.title = actionRep.extensions().friendlyName;
            actionViewModel.menuPath = actionRep.extensions()["x-ro-nof-menuPath"] || "";

            // open dialog on current pane always - invoke action goes to pane indicated by click
            actionViewModel.doInvoke = actionRep.extensions().hasParams ? (right?: boolean) => urlManager.setDialog(actionRep.actionId(), paneId) : (right?: boolean) => context.invokeAction(actionRep, clickHandler.pane(paneId, right));

            return actionViewModel;
        };

        viewModelFactory.dialogViewModel = (actionMember: ActionMember, paneId : number) => {
            const dialogViewModel = new DialogViewModel();
            const parameters = actionMember.parameters();
            dialogViewModel.title = actionMember.extensions().friendlyName;
            dialogViewModel.isQuery = actionMember.invokeLink().method() === "GET";
            dialogViewModel.message = "";
            dialogViewModel.parameters = _.map(parameters, parm => viewModelFactory.parameterViewModel(parm, ""));

            dialogViewModel.doClose = () => urlManager.closeDialog(paneId);
            dialogViewModel.doInvoke = (right?: boolean) => context.invokeAction(actionMember, clickHandler.pane(paneId, right), dialogViewModel);

            return dialogViewModel;
        };

        viewModelFactory.propertyViewModel = (propertyRep: PropertyMember, id: string, paneId : number) => {
            var propertyViewModel = new PropertyViewModel();
            propertyViewModel.title = propertyRep.extensions().friendlyName;
            propertyViewModel.value = propertyRep.isScalar() ? propertyRep.value().scalar() : propertyRep.value().toString();
            propertyViewModel.type = propertyRep.isScalar() ? "scalar" : "ref";
            propertyViewModel.returnType = propertyRep.extensions().returnType;
            propertyViewModel.format = propertyRep.extensions().format;
            propertyViewModel.reference = propertyRep.isScalar() || propertyRep.value().isNull() ? "" : propertyRep.value().link().href();

            // todo populate this with list of compatible types - either from domain type call or custom extensions 
            propertyViewModel.possibleDropTypes = [propertyRep.extensions().returnType];

            propertyViewModel.doClick = (right? : boolean) => {
                urlManager.setProperty(propertyRep, clickHandler.pane(paneId, right));
            }

            if (propertyRep.attachmentLink() != null) {
                propertyViewModel.attachment = AttachmentViewModel.create(propertyRep.attachmentLink().href(),
                    propertyRep.attachmentLink().type().asString,
                    propertyRep.attachmentLink().title());
            }

            // only set color if has value 
            propertyViewModel.color = propertyViewModel.value ? color.toColorFromType(propertyRep.extensions().returnType) : "";

            propertyViewModel.id = id;
            propertyViewModel.argId = id.toLowerCase();
            propertyViewModel.isEditable = !propertyRep.disabledReason();
            propertyViewModel.choices = [];
            propertyViewModel.hasPrompt = propertyRep.hasPrompt();

            if (propertyRep.hasChoices()) {
                
                var choices =  propertyRep.choices();

                if (choices) {
                    propertyViewModel.choices = _.map(choices, (v, n) => {
                        return ChoiceViewModel.create(v, id, n);
                    });
                }
            }

            propertyViewModel.hasChoices = propertyViewModel.choices.length > 0;
            propertyViewModel.hasPrompt = !!propertyRep.promptLink() && propertyRep.promptLink().arguments()["x-ro-searchTerm"];
            propertyViewModel.hasConditionalChoices =  !!propertyRep.promptLink() && !propertyViewModel.hasPrompt;

            if (propertyViewModel.hasPrompt || propertyViewModel.hasConditionalChoices) {
                var promptRep: PromptRepresentation = propertyRep.getPrompts();

                if (propertyViewModel.hasPrompt) {         
                    propertyViewModel.prompt = <(st: string) => ng.IPromise<ChoiceViewModel[]>> _.partial(context.prompt, promptRep, id);
                    propertyViewModel.minLength = propertyRep.promptLink().extensions().minLength;
                } 

                if (propertyViewModel.hasConditionalChoices) {
                    propertyViewModel.conditionalChoices = <(args: IValueMap) => ng.IPromise<ChoiceViewModel[]>> _.partial(context.conditionalChoices, promptRep, id);
                    propertyViewModel.arguments = _.object<IValueMap>(_.map(<_.Dictionary<Object>>propertyRep.promptLink().arguments(), (v: any, key) => [key, new Value(v.value)]));        
                }
            }

            if (propertyViewModel.hasChoices || propertyViewModel.hasPrompt || propertyViewModel.hasConditionalChoices) {

                var currentChoice: ChoiceViewModel = ChoiceViewModel.create(propertyRep.value(), id);

                if (propertyViewModel.hasPrompt || propertyViewModel.hasConditionalChoices) {
                    propertyViewModel.choice = currentChoice;
                } else {
                    propertyViewModel.choice = _.find(propertyViewModel.choices, (c: ChoiceViewModel) => c.match(currentChoice));
                }
            } 

            if (propertyRep.isScalar()) {
                var remoteMask = propertyRep.extensions()["x-ro-nof-mask"];
                var localFilter = mask.toLocalFilter(remoteMask) || mask.defaultLocalFilter(propertyRep.extensions().format);
                if (localFilter) {
                    propertyViewModel.value = $filter(localFilter.name)(propertyViewModel.value, localFilter.mask);
                }
            }

            // if a reference and no way to set (ie not choices or autocomplete) use autoautocomplete
            if (propertyViewModel.type === "ref" && !propertyViewModel.hasPrompt && !propertyViewModel.hasChoices && !propertyViewModel.hasConditionalChoices) {
                addAutoAutoComplete(propertyViewModel, ChoiceViewModel.create(propertyRep.value(), id), id, propertyRep.value());            
            } 

            return propertyViewModel;
        };
        
        function getItems(cvm: CollectionViewModel, links: Link[],  populateItems: boolean) {

            if (populateItems) {
                return _.map(links, link => {
                    const ivm = viewModelFactory.itemViewModel(link, cvm.onPaneId);
                    const tempTgt = link.getTarget();
                    repLoader.populate<DomainObjectRepresentation>(tempTgt).
                        then((obj: DomainObjectRepresentation) => {
                            ivm.target = viewModelFactory.domainObjectViewModel(obj, {}, 1);

                            if (!cvm.header) {
                                cvm.header = _.map(ivm.target.properties, property => property.title);
                            }
                        });
                    return ivm;
                });
            } else {
                return _.map(links, link => viewModelFactory.itemViewModel(link, cvm.onPaneId));
            }
        }

        function create(collectionRep: CollectionMember, state: CollectionViewState, paneId : number) {
            const collectionViewModel = new CollectionViewModel();
            const links = collectionRep.value().models;

            collectionViewModel.onPaneId = paneId;

            collectionViewModel.title = collectionRep.extensions().friendlyName;
            collectionViewModel.size = links.length;
            collectionViewModel.pluralName = collectionRep.extensions().pluralName;
            collectionViewModel.color = color.toColorFromType(collectionRep.extensions().elementType);

            collectionViewModel.items = getItems(collectionViewModel, links, state === CollectionViewState.Table);

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

            return collectionViewModel;
        }
       
        function createFromList(listRep: ListRepresentation, state: CollectionViewState, paneId : number) {
            const collectionViewModel = new CollectionViewModel();
            const links = listRep.value().models;

            collectionViewModel.onPaneId = paneId;

            collectionViewModel.size = links.length;
            collectionViewModel.pluralName = "Objects";
            collectionViewModel.items = getItems(collectionViewModel, links, state === CollectionViewState.Table);

            return collectionViewModel;
        }

      
        viewModelFactory.collectionViewModel = (collection: any, state: CollectionViewState, paneId : number) => {
            let collectionVm: CollectionViewModel = null;
            let setState: (state: CollectionViewState) => void;

            if (collection instanceof CollectionMember) {
                collectionVm = create(collection, state, paneId);
                setState = <(state: CollectionViewState) => void> _.partial(urlManager.setCollectionMemberState, paneId, collection);
            }

            if (collection instanceof ListRepresentation) {
                collectionVm = createFromList(collection, state, paneId);
                setState = <(state: CollectionViewState) => void> _.partial(urlManager.setQueryState, paneId);
            }

            if (collectionVm) {
                

                collectionVm.doSummary = () => setState(CollectionViewState.Summary);
                collectionVm.doList = () => setState(CollectionViewState.List);
                collectionVm.doTable = () => setState(CollectionViewState.Table);            
            }

            return collectionVm;
        };

     
        viewModelFactory.servicesViewModel = (servicesRep: DomainServicesRepresentation) => {
            var servicesViewModel = new ServicesViewModel();

            // filter out contributed action services 
            var links = _.filter(servicesRep.value().models, (m: Link) => {
                var sid = m.rel().parms[0].value;
                return sid.indexOf("ContributedActions") === -1; 
            });
            
            servicesViewModel.title = "Services";
            servicesViewModel.color = "bg-color-darkBlue";
            servicesViewModel.items = _.map(links, link => viewModelFactory.linkViewModel(link, 1));
            return servicesViewModel;
        };

        viewModelFactory.menusViewModel = (menusRep: MenusRepresentation, paneId : number) => {
            var menusViewModel = new MenusViewModel();

            menusViewModel.title = "Menus";
            menusViewModel.color = "bg-color-darkBlue";
            menusViewModel.items = _.map(menusRep.value().models, link =>  viewModelFactory.linkViewModel(link, paneId));
            return menusViewModel;
        };



       
        viewModelFactory.serviceViewModel = (serviceRep: DomainObjectRepresentation, paneId : number) => {
            var serviceViewModel = new ServiceViewModel();
            var actions = serviceRep.actionMembers();
            serviceViewModel.serviceId = serviceRep.serviceId();
            serviceViewModel.title = serviceRep.title();
            serviceViewModel.actions = _.map(actions, action =>  viewModelFactory.actionViewModel(action, paneId));
            serviceViewModel.color = color.toColorFromType(serviceRep.serviceId());          

            return serviceViewModel;
        };
  
        viewModelFactory.domainObjectViewModel = (objectRep: DomainObjectRepresentation, collectionStates: { [index: string]: CollectionViewState }, paneId : number): DomainObjectViewModel => {
            const objectViewModel = new DomainObjectViewModel();

            objectViewModel.onPaneId = paneId;

            objectViewModel.isTransient = !!objectRep.persistLink();

            objectViewModel.color = color.toColorFromType(objectRep.domainType());

            const savehandler = objectViewModel.isTransient ? context.saveObject : context.updateObject;
       
            objectViewModel.doSave = () => savehandler(objectRep, objectViewModel);

            objectViewModel.doEdit = () => urlManager.setObjectEdit(true, paneId);
            objectViewModel.doEditCancel = objectViewModel.isTransient ? () => urlManager.popUrlState(paneId) : () => urlManager.setObjectEdit(false, paneId);

            const properties = objectRep.propertyMembers();
            const collections = objectRep.collectionMembers();
            const actions = objectRep.actionMembers();

            objectViewModel.domainType = objectRep.domainType();
            objectViewModel.title = objectViewModel.isTransient ? `Unsaved ${objectRep.extensions().friendlyName}` : objectRep.title();

            objectViewModel.message = "";

            objectViewModel.actions = _.map(actions, action => viewModelFactory.actionViewModel(action, paneId));
            objectViewModel.properties = _.map(properties, (property, id) =>  viewModelFactory.propertyViewModel(property, id, paneId));
            objectViewModel.collections = _.map(collections, collection => viewModelFactory.collectionViewModel(collection, collectionStates[collection.collectionId()], paneId ));

            objectViewModel.toggleActionMenu = () => {
                urlManager.toggleObjectMenu(paneId);
            }

            // for dropping 

            const link = objectRep.selfLink();
            if (link) {
                // not transient - can't drag transients so no need to set up IDraggable members
                link.set("title", objectViewModel.title);

                const value = new Value(link);

                objectViewModel.value = value.toString();
                objectViewModel.reference = value.toValueString();
                objectViewModel.choice = ChoiceViewModel.create(value, "");
            }

            return objectViewModel;
        };

        viewModelFactory.toolBarViewModel = ($scope) => {
            var tvm = new ToolBarViewModel();

            $scope.$on("ajax-change", (event, count) => {
                if (count > 0) {
                    tvm.loading = "Loading...";
                } else {
                    tvm.loading = "";
                }
            });

            $scope.$on("back", () => {
                navigation.back();
            });

            $scope.$on("forward", () => {
                navigation.forward();
            });

            tvm.template = appBarTemplate;

            tvm.footerTemplate = footerTemplate;

            tvm.goHome = (right? : boolean) => {
                urlManager.setHome(clickHandler.pane(1, right));
            }

            tvm.goBack = () => {
                navigation.back();
            };

            tvm.goForward = () => {
                navigation.forward();
            };

            tvm.swapPanes = () => {
                urlManager.swapPanes();
            };

            tvm.singlePane = (right?: boolean) => {
                urlManager.singlePane(clickHandler.pane(1, right));
            };

            return tvm;
        }

        //Cicero
        viewModelFactory.ciceroViewModel = (wrapped: any) => {
            const vm = new CiceroViewModel();
            vm.wrapped = wrapped;
            vm.processCommand = (input: string) => { 
                let command: NakedObjects.Cicero.Command;
                const abbr = input.toLowerCase().substr(0, 2);
                switch (abbr) {
                    case "ho":
                        command = new NakedObjects.Cicero.Home(input);
                        break;
                    case "ge":
                        command = new NakedObjects.Cicero.Gemini(input);
                        break;
                    default:
                        command = new NakedObjects.Cicero.Unrecognised(input) ;
                }
                //Standard processing for all commands
                const newPath = command.newPath();
                if (newPath) {
                    $location.path(newPath).search({});
                }
               //TODO: get direct output and put it in the announcement
            }
            return vm;
        }
    });

}