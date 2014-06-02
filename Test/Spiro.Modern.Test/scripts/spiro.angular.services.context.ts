/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.viewmodels.ts" />
/// <reference path="spiro.angular.app.ts" />
/// <reference path="spiro.angular.services.representationloader.ts" />
/// <reference path="spiro.angular.config.ts" />

module Spiro.Angular {

    export interface IContext {
        getHome: () => ng.IPromise<HomePageRepresentation>;
        getServices: () => ng.IPromise<DomainServicesRepresentation>;
        getObject: (type: string, id?: string) => ng.IPromise<DomainObjectRepresentation>;
        setObject: (object: DomainObjectRepresentation) => void;
        getNestedObject: (type: string, id: string) => ng.IPromise<DomainObjectRepresentation>;
        setNestedObject: (object: DomainObjectRepresentation) => void;
        getCollection: () => ng.IPromise<ListRepresentation>;
        setCollection: (list: ListRepresentation) => void;
        getError: () => ErrorRepresentation;
        setError: (object: ErrorRepresentation) => void;
        getTransientObject: () => ng.IPromise<DomainObjectRepresentation>;
        setTransientObject: (object: DomainObjectRepresentation) => void;
        getPreviousUrl: () => string;
        setPreviousUrl: (url: string) => void;
        getSelectedChoice: (parm: string, search: string) => ChoiceViewModel[];
        clearSelectedChoice: (parm: string) => void;
        setSelectedChoice: (parm: string, search: string, cvm: ChoiceViewModel) => void;
    }

    interface IContextInternal extends IContext {
        getDomainObject: (type: string, id: string) => ng.IPromise<DomainObjectRepresentation>;
        getService: (type: string) => ng.IPromise<DomainObjectRepresentation>;
    }

    app.service('context', function ($q: ng.IQService, repLoader: IRepLoader) {

        var context = <IContextInternal>this;

        var currentHome: HomePageRepresentation = null;

        function isSameObject(object: DomainObjectRepresentation, type: string, id?: string) {
            var sid = object.serviceId();
            return sid ? sid === type : (object.domainType() == type && object.instanceId() === id);
        }

        // exposed for testing
        context.getDomainObject = (type: string, id: string): ng.IPromise<DomainObjectRepresentation> => {
            var object = new DomainObjectRepresentation();
            object.hateoasUrl = appPath + "/objects/" + type + "/" + id;
            return repLoader.populate<DomainObjectRepresentation>(object);
        };

        // exposed for testing
        context.getService = function (type: string): ng.IPromise<DomainObjectRepresentation> {
            var delay = $q.defer<DomainObjectRepresentation>();

            this.getServices().
                then((services: DomainServicesRepresentation) => {
                    var serviceLink = _.find(services.value().models, (model: Link) => { return model.rel().parms[0] === 'serviceId="' + type + '"'; });
                    var service = serviceLink.getTarget();
                    return repLoader.populate(service);
                }).
                then((service: DomainObjectRepresentation) => {
                    currentObject = service;
                    delay.resolve(service);
                }, error => delay.reject(error));
            return delay.promise;
        };


        context.getHome = () => {
            var delay = $q.defer<HomePageRepresentation>();

            if (currentHome) {
                delay.resolve(currentHome);
            }
            else {
                //var home = new HomePageRepresentation();
                repLoader.populate<HomePageRepresentation>(new HomePageRepresentation()).then((home: HomePageRepresentation) => {
                    currentHome = home;
                    delay.resolve(home);
                }, error => delay.reject(error));
            }

            return delay.promise;
        };

        var currentServices: DomainServicesRepresentation = null;

        context.getServices = function () {
            var delay = $q.defer<DomainServicesRepresentation>();

            if (currentServices) {
                delay.resolve(currentServices);
            }
            else {
                this.getHome().
                    then((home: HomePageRepresentation) => {
                        var ds = home.getDomainServices();
                        return repLoader.populate<DomainServicesRepresentation>(ds);
                    }).
                    then((services: DomainServicesRepresentation) => {
                        currentServices = services;
                        delay.resolve(services);
                    }, error => delay.reject(error));
            }

            return delay.promise;
        };

        var currentObject: DomainObjectRepresentation = null;

        context.getObject = function (type: string, id?: string) {
            var delay = $q.defer<DomainObjectRepresentation>();

            if (currentObject && isSameObject(currentObject, type, id)) {
                delay.resolve(currentObject);
            }
            else {
                var promise = id ? this.getDomainObject(type, id) : this.getService(type);
                promise.then((object: DomainObjectRepresentation) => {
                    currentObject = object;
                    delay.resolve(object);
                }, error => delay.reject(error));
            }

            return delay.promise;
        };

        context.setObject = co => currentObject = co;

        var currentNestedObject: DomainObjectRepresentation = null;

        context.getNestedObject = (type: string, id: string) => {
            var delay = $q.defer<DomainObjectRepresentation>();

            if (currentNestedObject && isSameObject(currentNestedObject, type, id)) {
                delay.resolve(currentNestedObject);
            }
            else {
                var domainObjectRepresentation = new DomainObjectRepresentation();
                domainObjectRepresentation.hateoasUrl = appPath + "/objects/" + type + "/" + id;

                repLoader.populate<DomainObjectRepresentation>(domainObjectRepresentation).
                    then((dor: DomainObjectRepresentation) => {
                        currentNestedObject = dor;
                        delay.resolve(dor);
                    }, error => delay.reject(error));
            }

            return delay.promise;
        };

        context.setNestedObject = cno => currentNestedObject = cno;

        var currentError: ErrorRepresentation = null;

        context.getError = () => currentError;

        context.setError = (e: ErrorRepresentation) => currentError = e;

        var currentCollection: ListRepresentation = null;

        context.getCollection = () => {
            var delay = $q.defer<ListRepresentation>();
            delay.resolve(currentCollection);
            return delay.promise;
        };

        context.setCollection = (c: ListRepresentation) => currentCollection = c;

        var currentTransient: DomainObjectRepresentation = null;

        context.getTransientObject = () => {
            var delay = $q.defer<DomainObjectRepresentation>();
            delay.resolve(currentTransient);
            return delay.promise;
        };

        context.setTransientObject = (t: DomainObjectRepresentation) => currentTransient = t;

        var previousUrl: string = null;

        context.getPreviousUrl = () => previousUrl;

        context.setPreviousUrl = (url: string) => previousUrl = url;

        var selectedChoice: { [parm: string]: { [search: string]: ChoiceViewModel[] } } = {};

        context.getSelectedChoice = (parm: string, search: string) => selectedChoice[parm] ? selectedChoice[parm][search] : null;

        context.setSelectedChoice = (parm: string, search: string, cvm: ChoiceViewModel) => {
            selectedChoice[parm] = selectedChoice[parm] || {};
            selectedChoice[parm][search] = selectedChoice[parm][search] || [];
            selectedChoice[parm][search].push(cvm);
        }

        context.clearSelectedChoice = (parm: string) => selectedChoice[parm] = null;
    });

}