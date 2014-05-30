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
        itemViewModel(linkRep: Link, parentHref: string, index: number): ItemViewModel;
        parameterViewModel(parmRep: Parameter, id: string, previousValue: string): ParameterViewModel;
        actionViewModel(actionRep: ActionMember): ActionViewModel;
        dialogViewModel(actionRep: ActionRepresentation, invoke: (dvm: DialogViewModel) => void): DialogViewModel;
        propertyViewModel(propertyRep: PropertyMember, id: string, propertyDetails?: PropertyRepresentation): PropertyViewModel;
        collectionViewModel(collection: CollectionMember): CollectionViewModel;
        collectionViewModel(collection: CollectionRepresentation): CollectionViewModel;
        collectionViewModel(collection: ListRepresentation): CollectionViewModel;
        servicesViewModel(servicesRep: DomainServicesRepresentation): ServicesViewModel;
        serviceViewModel(serviceRep: DomainObjectRepresentation): ServiceViewModel;
        domainObjectViewModel(objectRep: DomainObjectRepresentation, details?: PropertyRepresentation[], save?: (ovm: DomainObjectViewModel) => void, previousUrl? : string): DomainObjectViewModel;
    }

    app.service('viewModelFactory', function ($q : ng.IQService, $location : ng.ILocationService, $filter : ng.IFilterService, urlHelper: IUrlHelper, repLoader: IRepLoader, color : IColor, context : IContext, repHandlers : IRepHandlers, mask : IMask) {

        var viewModelFactory = <IViewModelFactory>this;


        viewModelFactory.errorViewModel = function (errorRep: ErrorRepresentation) {
            var errorViewModel = new ErrorViewModel();
            errorViewModel.message = errorRep.message() || "An Error occurred";
            var stackTrace = errorRep.stacktrace();

            errorViewModel.stackTrace = !stackTrace || stackTrace.length === 0 ? ["Empty"] : stackTrace;
            return errorViewModel;
        };

        viewModelFactory.linkViewModel = function (linkRep: Link) {
            var linkViewModel = new LinkViewModel();
            linkViewModel.title = linkRep.title();
            linkViewModel.href = urlHelper.toAppUrl(linkRep.href());
            linkViewModel.color = color.toColorFromHref(linkRep.href());
            return linkViewModel;
        };

        viewModelFactory.itemViewModel = function (linkRep: Link, parentHref: string, index: number) {
            var linkViewModel = new LinkViewModel();
            linkViewModel.title = linkRep.title();
            linkViewModel.href = urlHelper.toItemUrl(parentHref, linkRep.href());
            linkViewModel.color = color.toColorFromHref(linkRep.href());
            return linkViewModel;
        };

        viewModelFactory.parameterViewModel = function (parmRep: Parameter, id: string, previousValue: string): any {
            var parmViewModel = new ParameterViewModel();

            parmViewModel.type = parmRep.isScalar() ? "scalar" : "ref";

            parmViewModel.title = parmRep.extensions().friendlyName;
            parmViewModel.dflt = parmRep.default().toValueString();
            parmViewModel.message = "";
            parmViewModel.mask = parmRep.extensions["x-ro-nof-mask"];

            if (parmRep.extensions().returnType === "boolean") {
                parmViewModel.value = previousValue ? previousValue.toLowerCase() === 'true' : parmRep.default().scalar();
            }
            else {
                parmViewModel.value = previousValue || parmViewModel.dflt
            }

            parmViewModel.id = id;
            parmViewModel.argId = id.toLowerCase();
            parmViewModel.returnType = parmRep.extensions().returnType;
            parmViewModel.format = parmRep.extensions().format;

            parmViewModel.reference = "";

            parmViewModel.choices = _.map(parmRep.choices(), (v, n) => {
                return ChoiceViewModel.create(v, id, n);
            });
            
            parmViewModel.hasChoices = parmViewModel.choices.length > 0;

            if (parmViewModel.hasChoices && parmViewModel.value) {
                if (parmViewModel.type == "scalar") {
                    // TODO fix so this works same way for both scalar and ref !
                    parmViewModel.choice = _.find(parmViewModel.choices, (c: ChoiceViewModel) => c.value === parmViewModel.value);
                }
                else {
                    parmViewModel.choice = _.find(parmViewModel.choices, (c: ChoiceViewModel) => c.name === parmViewModel.value);
                }
            }

            parmViewModel.hasPrompt = !!parmRep.promptLink() && parmRep.promptLink().arguments()["x-ro-searchTerm"]; 
            parmViewModel.hasConditionalChoices = !!parmRep.promptLink() && !parmViewModel.hasPrompt;

            if (parmViewModel.hasPrompt) {
                var promptRep = parmRep.getPrompts();
                parmViewModel.prompt = <(st: string) => ng.IPromise<ChoiceViewModel[]>> _.partial(repHandlers.prompt, promptRep, id);

                // TODO and at least investigate if we can do all choices/prompt caching the same way !!
                if (previousValue) {
                    parmViewModel.choice = context.getSelectedChoice(id, previousValue);
                }
                else {
                    // clear any previous 
                    context.clearSelectedChoice();
                }
            }

            if (parmViewModel.hasConditionalChoices) {
                var promptRep = parmRep.getPrompts();
                parmViewModel.conditionalChoices = <(args: ValueMap) => ng.IPromise<ChoiceViewModel[]>> _.partial(repHandlers.conditionalChoices, promptRep, id);

                parmViewModel.arguments = _.object<ValueMap>(_.map(<_.Dictionary<Object>>parmRep.promptLink().arguments(), (v : any, key) => [key, new Value(v.value)]));

                // TODO and at least investigate if we can do all choices/prompt caching the same way !!
                if (previousValue) {
                    parmViewModel.choice = context.getSelectedChoice(id, previousValue);
                }
                else {
                    // clear any previous 
                    context.clearSelectedChoice();
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

        viewModelFactory.actionViewModel = function (actionRep: ActionMember) {
            var actionViewModel = new ActionViewModel();
            actionViewModel.title = actionRep.extensions().friendlyName;
            actionViewModel.href = urlHelper.toActionUrl(actionRep.detailsLink().href());
            return actionViewModel;
        };

        viewModelFactory.dialogViewModel = function (actionRep: ActionRepresentation, invoke: (dvm: DialogViewModel) => void) {
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
                invoke(dialogViewModel)
            };
            dialogViewModel.doInvoke = () => {
                dialogViewModel.show = false;
                invoke(dialogViewModel)
            };

            return dialogViewModel;
        };

        viewModelFactory.propertyViewModel = function (propertyRep: PropertyMember, id: string, propertyDetails?: PropertyRepresentation) {
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
      
            if (propertyDetails && propertyRep.hasChoices()) {
                propertyViewModel.choices = _.map(propertyDetails.choices(), (v, n) => {
                    return ChoiceViewModel.create(v, id, n);
                });
            }
         
            propertyViewModel.hasChoices = propertyViewModel.choices.length > 0;
       
            if (propertyViewModel.hasChoices) {
                propertyViewModel.choice = _.find(propertyViewModel.choices, (c : ChoiceViewModel) => c.name === (propertyViewModel.value ? propertyViewModel.value.toString() : ""));
                if (propertyViewModel.choice) {
                    propertyViewModel.value = propertyViewModel.choice.name;
                }
            }
            else if (propertyViewModel.type === "ref") {
                propertyViewModel.choice = ChoiceViewModel.create(propertyRep.value(), id);
            }
            else {
                propertyViewModel.choice = null;
            }

            propertyViewModel.hasPrompt = !!propertyDetails && !!propertyDetails.promptLink() && propertyDetails.promptLink().arguments()["x-ro-searchTerm"];
            propertyViewModel.hasConditionalChoices = !!propertyDetails && !!propertyDetails.promptLink() && !propertyViewModel.hasPrompt;

            if (propertyViewModel.hasPrompt && propertyDetails) {
                var list = propertyDetails.getPrompts();
                propertyViewModel.prompt = <(st: string) => ng.IPromise<ChoiceViewModel[]>> _.partial(repHandlers.prompt, list, id);
            }
            else {
                propertyViewModel.prompt = (st: string) => {
                    return $q.when(<ChoiceViewModel[]>[]);
                };
            }

            if (propertyViewModel.hasConditionalChoices) {
                var promptRep = propertyDetails.getPrompts();
                propertyViewModel.conditionalChoices = <(args: ValueMap) => ng.IPromise<ChoiceViewModel[]>> _.partial(repHandlers.conditionalChoices, promptRep, id);

                propertyViewModel.arguments = _.object<ValueMap>(_.map(<_.Dictionary<Object>>propertyDetails.promptLink().arguments(), (v: any, key) => [key, new Value(v.value)]));

                propertyViewModel.choice = ChoiceViewModel.create(propertyRep.value(), id);
            }

            var remoteMask = propertyRep.extensions()["x-ro-nof-mask"];

            if (remoteMask && propertyRep.isScalar()) {
                var localFilter = mask.toLocalFilter(remoteMask); 
                if (localFilter) {
                    propertyViewModel.value = $filter(localFilter.name)(propertyViewModel.value, localFilter.mask);
                }
            }

            return propertyViewModel;
        };

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

        function createFromDetails(collectionRep: CollectionRepresentation) {
            var collectionViewModel = new CollectionViewModel();
            var links = collectionRep.value().models;

            collectionViewModel.title = collectionRep.extensions().friendlyName;
            collectionViewModel.size = links.length;
            collectionViewModel.pluralName = collectionRep.extensions().pluralName;

            collectionViewModel.href = urlHelper.toCollectionUrl(collectionRep.selfLink().href());
            collectionViewModel.color = color.toColorFromType(collectionRep.extensions().elementType);

            var i = 0;
            collectionViewModel.items = _.map(links, (link) => { return viewModelFactory.itemViewModel(link, collectionViewModel.href, i++); });

            return collectionViewModel;
        }

        function createFromList(listRep: ListRepresentation) {
            var collectionViewModel = new CollectionViewModel();
            var links = listRep.value().models;
          
            collectionViewModel.size = links.length;
            collectionViewModel.pluralName = "Objects";

            var i = 0;
            collectionViewModel.items = _.map(links, (link) => { return viewModelFactory.itemViewModel(link, $location.path(), i++); });

            return collectionViewModel;
        }

        viewModelFactory.collectionViewModel = function (collection: any) {
            if (collection instanceof CollectionMember) {
                return create(<CollectionMember>collection);
            }
            if (collection instanceof CollectionRepresentation) {
                return createFromDetails(<CollectionRepresentation>collection);
            }
            if (collection instanceof ListRepresentation) {
                return createFromList(<ListRepresentation>collection);
            }
            return null;
        };

        viewModelFactory.servicesViewModel = function (servicesRep: DomainServicesRepresentation) {
            var servicesViewModel = new ServicesViewModel();
            var links = servicesRep.value().models;
            servicesViewModel.title = "Services";
            servicesViewModel.color = "bg-color-darkBlue";
            servicesViewModel.items = _.map(links, (link) => { return viewModelFactory.linkViewModel(link); });
            return servicesViewModel;
        };

        viewModelFactory.serviceViewModel = function (serviceRep: DomainObjectRepresentation) {
            var serviceViewModel = new ServiceViewModel();
            var actions = serviceRep.actionMembers();
            serviceViewModel.serviceId = serviceRep.serviceId();
            serviceViewModel.title = serviceRep.title();
            serviceViewModel.actions = _.map(actions, (action) => { return viewModelFactory.actionViewModel(action); });
            serviceViewModel.color = color.toColorFromType(serviceRep.serviceId());
            serviceViewModel.href = urlHelper.toAppUrl(serviceRep.getUrl());
          

            return serviceViewModel;
        };

        viewModelFactory.domainObjectViewModel = function (objectRep: DomainObjectRepresentation, details?: PropertyRepresentation[], save?: (ovm: DomainObjectViewModel) => void) {
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

            objectViewModel.properties = _.map(properties, (property, id?) => { return viewModelFactory.propertyViewModel(property, id, _.find(details, (d : PropertyRepresentation) => { return d.instanceId() === id })); });
            objectViewModel.collections = _.map(collections, (collection) => { return viewModelFactory.collectionViewModel(collection); });
            objectViewModel.actions = _.map(actions, (action) => { return viewModelFactory.actionViewModel(action); });


            return objectViewModel;
        };
    });

}