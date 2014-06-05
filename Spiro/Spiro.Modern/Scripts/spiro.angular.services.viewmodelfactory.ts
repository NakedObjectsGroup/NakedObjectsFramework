/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.viewmodels.ts" />
/// <reference path="spiro.angular.app.ts" />
/// <reference path="spiro.angular.services.representationhandlers.ts" />
/// <reference path="spiro.angular.services.urlhelper.ts" />
/// <reference path="spiro.angular.services.context.ts" />

module Spiro.Angular {
    export interface IViewModelFactory {
        errorViewModel(errorRep: ErrorRepresentation): ErrorViewModel;
        linkViewModel(linkRep: Link): LinkViewModel;
        itemViewModel(linkRep: Link, parentHref: string): ItemViewModel;
        parameterViewModel(parmRep: Parameter, id: string, previousValue: string): ParameterViewModel;
        actionViewModel(actionRep: ActionMember): ActionViewModel;
        dialogViewModel(actionRep: ActionRepresentation, invoke: (dvm: DialogViewModel) => void): DialogViewModel;
        propertyViewModel(propertyRep: PropertyMember, id: string, propertyDetails?: PropertyRepresentation): PropertyViewModel;
        collectionViewModel(collection: any, populateItems?: boolean): CollectionViewModel;
        collectionViewModel(collection: CollectionMember, populateItems?: boolean): CollectionViewModel;
        collectionViewModel(collection: CollectionRepresentation, populateItems?: boolean): CollectionViewModel;
        collectionViewModel(collection: ListRepresentation, populateItems?: boolean): CollectionViewModel;
        servicesViewModel(servicesRep: DomainServicesRepresentation): ServicesViewModel;
        serviceViewModel(serviceRep: DomainObjectRepresentation): ServiceViewModel;
        domainObjectViewModel(objectRep: DomainObjectRepresentation, details?: PropertyRepresentation[], save?: (ovm: DomainObjectViewModel) => void, previousUrl? : string): DomainObjectViewModel;
    }

    app.service('viewModelFactory', function($q: ng.IQService, $location: ng.ILocationService, $filter: ng.IFilterService, urlHelper: IUrlHelper, repLoader: IRepLoader, color: IColor, context: IContext, repHandlers: IRepHandlers, mask: IMask) {

        var viewModelFactory = <IViewModelFactory>this;

        // tested
        viewModelFactory.errorViewModel = (errorRep: ErrorRepresentation) => {
            var errorViewModel = new ErrorViewModel();
            errorViewModel.message = errorRep.message() || "An Error occurred";
            var stackTrace = errorRep.stacktrace();

            errorViewModel.stackTrace = !stackTrace || stackTrace.length === 0 ? ["Empty"] : stackTrace;
            return errorViewModel;
        };

        // tested
        viewModelFactory.linkViewModel = (linkRep: Link) => {
            var linkViewModel = new LinkViewModel();
            linkViewModel.title = linkRep.title();
            linkViewModel.href = urlHelper.toAppUrl(linkRep.href());
            linkViewModel.color = color.toColorFromHref(linkRep.href());
            return linkViewModel;
        };

        // tested
        viewModelFactory.itemViewModel = (linkRep: Link, parentHref: string) => {
            var itemViewModel = new ItemViewModel();
            itemViewModel.title = linkRep.title();
            itemViewModel.href = urlHelper.toItemUrl(parentHref, linkRep.href());
            itemViewModel.color = color.toColorFromHref(linkRep.href());

            return itemViewModel;
        };

        viewModelFactory.parameterViewModel = (parmRep: Parameter, id: string, previousValue: string): any => {
            var parmViewModel = new ParameterViewModel();

            parmViewModel.type = parmRep.isScalar() ? "scalar" : "ref";
            parmViewModel.title = parmRep.extensions().friendlyName;
            parmViewModel.dflt = parmRep.default().toValueString();
            parmViewModel.message = "";
            parmViewModel.mask = parmRep.extensions["x-ro-nof-mask"];
            parmViewModel.id = id;
            parmViewModel.argId = id.toLowerCase();
            parmViewModel.returnType = parmRep.extensions().returnType;
            parmViewModel.format = parmRep.extensions().format;
            parmViewModel.reference = "";

            parmViewModel.choices = _.map(parmRep.choices(), (v, n) => {
                return ChoiceViewModel.create(v, id, n);
            });

            parmViewModel.hasChoices = parmViewModel.choices.length > 0;
            parmViewModel.hasPrompt = !!parmRep.promptLink() && parmRep.promptLink().arguments()["x-ro-searchTerm"];
            parmViewModel.hasConditionalChoices = !!parmRep.promptLink() && !parmViewModel.hasPrompt;
            parmViewModel.isMultipleChoices = (parmViewModel.hasChoices || parmViewModel.hasConditionalChoices) && parmRep.extensions().returnType == "list";

            if (parmViewModel.hasPrompt || parmViewModel.hasConditionalChoices) {

                var promptRep = parmRep.getPrompts();
                if (parmViewModel.hasPrompt) {
                    parmViewModel.prompt = <(st: string) => ng.IPromise<ChoiceViewModel[]>> _.partial(repHandlers.prompt, promptRep, id);
                    parmViewModel.minLength = parmRep.promptLink().extensions().minLength;
                }

                if (parmViewModel.hasConditionalChoices) {
                    parmViewModel.conditionalChoices = <(args: ValueMap) => ng.IPromise<ChoiceViewModel[]>> _.partial(repHandlers.conditionalChoices, promptRep, id);
                    parmViewModel.arguments = _.object<ValueMap>(_.map(<_.Dictionary<Object>>parmRep.promptLink().arguments(), (v: any, key) => [key, new Value(v.value)]));
                }
            }

            if (parmViewModel.hasChoices || parmViewModel.hasPrompt || parmViewModel.hasConditionalChoices) {

                
                if (parmViewModel.isMultipleChoices) {
                    var search = parmViewModel.getMemento();
                    parmViewModel.setSelectedChoice = () => {
                        
                        _.forEach(parmViewModel.multiChoices, (c) => {
                            context.setSelectedChoice(id, search, c);
                        });  
                    };         
                } else {
                   
                    parmViewModel.setSelectedChoice = () =>  {
                        context.setSelectedChoice(id, parmViewModel.getMemento(), parmViewModel.choice);
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
                        var scs = context.getSelectedChoice(id, previousValue);
                        setCurrentChoices(scs);
                    } else {
                        var sc = context.getSelectedChoice(id, previousValue).pop();
                        setCurrentChoice(sc);
                    }
                } else if (parmViewModel.dflt) {
                    var dflt = parmRep.default();
                  
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

            return parmViewModel;
        };

        // tested
        viewModelFactory.actionViewModel = (actionRep: ActionMember) => {
            var actionViewModel = new ActionViewModel();
            actionViewModel.title = actionRep.extensions().friendlyName;
            actionViewModel.href = urlHelper.toActionUrl(actionRep.detailsLink().href());
            return actionViewModel;
        };

        // tested
        viewModelFactory.dialogViewModel = (actionRep: ActionRepresentation, invoke: (dvm: DialogViewModel) => void) => {
            var dialogViewModel = new DialogViewModel();
            var parameters = actionRep.parameters();
            var parms = urlHelper.actionParms();

            dialogViewModel.title = actionRep.extensions().friendlyName;
            dialogViewModel.isQuery = actionRep.invokeLink().method() === "GET";

            dialogViewModel.message = "";

            dialogViewModel.close = urlHelper.toAppUrl(actionRep.upLink().href(), ["action"]);

            var i = 0;
            dialogViewModel.parameters = _.map(parameters, (parm, id?) => { return viewModelFactory.parameterViewModel(parm, id, parms[i++]); });

            dialogViewModel.doShow = () => {
                dialogViewModel.show = true;
                invoke(dialogViewModel);
            };
            dialogViewModel.doInvoke = () => {
                dialogViewModel.show = false;
                invoke(dialogViewModel);
            };

            return dialogViewModel;
        };

        viewModelFactory.propertyViewModel = (propertyRep: PropertyMember, id: string, propertyDetails?: PropertyRepresentation) => {
            var propertyViewModel = new PropertyViewModel();
            propertyViewModel.title = propertyRep.extensions().friendlyName;
            propertyViewModel.value = propertyRep.isScalar() ? propertyRep.value().scalar() : propertyRep.value().toString();
            propertyViewModel.type = propertyRep.isScalar() ? "scalar" : "ref";
            propertyViewModel.returnType = propertyRep.extensions().returnType;
            propertyViewModel.format = propertyRep.extensions().format;
            propertyViewModel.href = propertyRep.isScalar() || propertyRep.detailsLink() == null ? "" : urlHelper.toPropertyUrl(propertyRep.detailsLink().href());
            propertyViewModel.target = propertyRep.isScalar() || propertyRep.value().isNull() ? "" : urlHelper.toAppUrl(propertyRep.value().link().href());
            propertyViewModel.reference = propertyRep.isScalar() || propertyRep.value().isNull() ? "" : propertyRep.value().link().href();


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
                // if we have details get from that as it will alawys be there. If not choices may be on member
                var choices = propertyDetails ? propertyDetails.choices() : propertyRep.choices();

                if (choices) {
                    propertyViewModel.choices = _.map(choices, (v, n) => {
                        return ChoiceViewModel.create(v, id, n);
                    });
                }
            }

            propertyViewModel.hasChoices = propertyViewModel.choices.length > 0;
            propertyViewModel.hasPrompt = !!propertyDetails && !!propertyDetails.promptLink() && propertyDetails.promptLink().arguments()["x-ro-searchTerm"];
            propertyViewModel.hasConditionalChoices = !!propertyDetails && !!propertyDetails.promptLink() && !propertyViewModel.hasPrompt;

            if (propertyViewModel.hasPrompt || propertyViewModel.hasConditionalChoices) {
                var promptRep: PromptRepresentation = propertyDetails.getPrompts();

                if (propertyViewModel.hasPrompt) {         
                    propertyViewModel.prompt = <(st: string) => ng.IPromise<ChoiceViewModel[]>> _.partial(repHandlers.prompt, promptRep, id);
                    propertyViewModel.minLength = propertyDetails.promptLink().extensions().minLength;
                } 

                if (propertyViewModel.hasConditionalChoices) {
                    propertyViewModel.conditionalChoices = <(args: ValueMap) => ng.IPromise<ChoiceViewModel[]>> _.partial(repHandlers.conditionalChoices, promptRep, id);
                    propertyViewModel.arguments = _.object<ValueMap>(_.map(<_.Dictionary<Object>>propertyDetails.promptLink().arguments(), (v: any, key) => [key, new Value(v.value)]));        
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

            var remoteMask = propertyRep.extensions()["x-ro-nof-mask"];

            if (remoteMask && propertyRep.isScalar()) {
                var localFilter = mask.toLocalFilter(remoteMask);
                if (localFilter) {
                    propertyViewModel.value = $filter(localFilter.name)(propertyViewModel.value, localFilter.mask);
                }
            }

            // if a reference and no way to set (ie not choices or autocomplete) set editable to false
            if (propertyViewModel.type == "ref" && !propertyViewModel.hasPrompt && !propertyViewModel.hasChoices && !propertyViewModel.hasConditionalChoices) {
                propertyViewModel.isEditable = false;
            } 

            return propertyViewModel;
        };

        // tested
        function create(collectionRep: CollectionMember) {
            var collectionViewModel = new CollectionViewModel();

            collectionViewModel.title = collectionRep.extensions().friendlyName;
            collectionViewModel.size = collectionRep.size();
            collectionViewModel.pluralName = collectionRep.extensions().pluralName;

            collectionViewModel.href = collectionRep.detailsLink() ? urlHelper.toCollectionUrl(collectionRep.detailsLink().href()) : "";
            collectionViewModel.color = color.toColorFromType(collectionRep.extensions().elementType);

            collectionViewModel.items = [];

            return collectionViewModel;
        }

        function getItems(cvm : CollectionViewModel, links: Spiro.Link[], href: string, populateItems?: boolean) {

            if (populateItems) {
                return _.map(links, (link) => {
                    var ivm = viewModelFactory.itemViewModel(link, href);
                    var tempTgt = link.getTarget();
                    repLoader.populate<DomainObjectRepresentation>(tempTgt).then((obj: DomainObjectRepresentation) => {
                        ivm.target = viewModelFactory.domainObjectViewModel(obj);

                        if (!cvm.header) {
                            cvm.header = _.map(ivm.target.properties, (property: PropertyViewModel) => property.title);
                        }

                    });
                    return ivm;
                });
            } else {
                return _.map(links, (link) => { return viewModelFactory.itemViewModel(link, href); });
            }
        }

        // tested
        function createFromDetails(collectionRep: CollectionRepresentation, populateItems?: boolean) {
            var collectionViewModel = new CollectionViewModel();
            var links = collectionRep.value().models;

            collectionViewModel.title = collectionRep.extensions().friendlyName;
            collectionViewModel.size = links.length;
            collectionViewModel.pluralName = collectionRep.extensions().pluralName;

            collectionViewModel.href = urlHelper.toCollectionUrl(collectionRep.selfLink().href());
            collectionViewModel.color = color.toColorFromType(collectionRep.extensions().elementType);

            collectionViewModel.items = getItems(collectionViewModel, links, collectionViewModel.href, populateItems);

            return collectionViewModel;
        }

        // tested
        function createFromList(listRep: ListRepresentation, populateItems?: boolean) {
            var collectionViewModel = new CollectionViewModel();
            var links = listRep.value().models;
          
            collectionViewModel.size = links.length;
            collectionViewModel.pluralName = "Objects";

            collectionViewModel.items = getItems(collectionViewModel, links, $location.path(), populateItems);

            return collectionViewModel;
        }

        // tested
        viewModelFactory.collectionViewModel = (collection: any, populateItems?: boolean) => {
            if (collection instanceof CollectionMember) {
                return create(<CollectionMember>collection);
            }
            if (collection instanceof CollectionRepresentation) {
                return createFromDetails(<CollectionRepresentation>collection, populateItems);
            }
            if (collection instanceof ListRepresentation) {
                return createFromList(<ListRepresentation>collection, populateItems);
            }
            return null;
        };

        // tested
        viewModelFactory.servicesViewModel = (servicesRep: DomainServicesRepresentation) => {
            var servicesViewModel = new ServicesViewModel();

            // filter out contributed action services 
            var links = _.filter(servicesRep.value().models, (m: Link) => {
                var sid = m.rel().parms[0].split('=')[1];
                return sid.indexOf("ContributedActions") == -1; 
            });
            
            servicesViewModel.title = "Services";
            servicesViewModel.color = "bg-color-darkBlue";
            servicesViewModel.items = _.map(links, (link) => { return viewModelFactory.linkViewModel(link); });
            return servicesViewModel;
        };

        // tested
        viewModelFactory.serviceViewModel = (serviceRep: DomainObjectRepresentation) => {
            var serviceViewModel = new ServiceViewModel();
            var actions = serviceRep.actionMembers();
            serviceViewModel.serviceId = serviceRep.serviceId();
            serviceViewModel.title = serviceRep.title();
            serviceViewModel.actions = _.map(actions, (action) => { return viewModelFactory.actionViewModel(action); });
            serviceViewModel.color = color.toColorFromType(serviceRep.serviceId());
            serviceViewModel.href = urlHelper.toAppUrl(serviceRep.getUrl());
          

            return serviceViewModel;
        };

        // tested
        viewModelFactory.domainObjectViewModel = (objectRep: DomainObjectRepresentation, details?: PropertyRepresentation[], save?: (ovm: DomainObjectViewModel) => void) => {
            var objectViewModel = new DomainObjectViewModel();
            var isTransient = !!objectRep.persistLink();

            objectViewModel.href = urlHelper.toAppUrl(objectRep.getUrl());

            objectViewModel.cancelEdit =  isTransient ? ""  :  urlHelper.toAppUrl(objectRep.getUrl());

            objectViewModel.color = color.toColorFromType(objectRep.domainType());

            objectViewModel.doSave = save ? () => save(objectViewModel) : () => { };

            var properties = objectRep.propertyMembers();
            var collections = objectRep.collectionMembers();
            var actions = objectRep.actionMembers();

            objectViewModel.domainType = objectRep.domainType();
            objectViewModel.title = isTransient ? "Unsaved " + objectRep.extensions().friendlyName : objectRep.title();

            objectViewModel.message = "";
          
            objectViewModel.properties = _.map(properties, (property, id?) => { return viewModelFactory.propertyViewModel(property, id, _.find(details || [], (d : PropertyRepresentation) => { return d.instanceId() === id; })); });
            objectViewModel.collections = _.map(collections, (collection) => { return viewModelFactory.collectionViewModel(collection); });
            objectViewModel.actions = _.map(actions, (action) => { return viewModelFactory.actionViewModel(action); });

            return objectViewModel;
        };
    });

}