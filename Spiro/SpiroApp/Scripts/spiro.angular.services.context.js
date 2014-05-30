/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.viewmodels.ts" />
/// <reference path="spiro.angular.app.ts" />
/// <reference path="spiro.angular.services.representationloader.ts" />
/// <reference path="spiro.angular.config.ts" />
var Spiro;
(function (Spiro) {
    (function (Angular) {
        Spiro.Angular.app.service('context', function ($q, repLoader) {
            var context = this;

            var currentHome = null;

            function isSameObject(object, type, id) {
                var sid = object.serviceId();
                return sid ? sid === type : (object.domainType() == type && object.instanceId() === id);
            }

            // exposed for testing
            context.getDomainObject = function (type, id) {
                var object = new Spiro.DomainObjectRepresentation();
                object.hateoasUrl = Spiro.appPath + "/objects/" + type + "/" + id;
                return repLoader.populate(object);
            };

            // exposed for testing
            context.getService = function (type) {
                var delay = $q.defer();

                this.getServices().then(function (services) {
                    var serviceLink = _.find(services.value().models, function (model) {
                        return model.rel().parms[0] === 'serviceId="' + type + '"';
                    });
                    var service = serviceLink.getTarget();
                    return repLoader.populate(service);
                }).then(function (service) {
                    currentObject = service;
                    delay.resolve(service);
                });
                return delay.promise;
            };

            context.getHome = function () {
                var delay = $q.defer();

                if (currentHome) {
                    delay.resolve(currentHome);
                } else {
                    var home = new Spiro.HomePageRepresentation();
                    repLoader.populate(home).then(function (home) {
                        currentHome = home;
                        delay.resolve(home);
                    });
                }

                return delay.promise;
            };

            var currentServices = null;

            context.getServices = function () {
                var delay = $q.defer();

                if (currentServices) {
                    delay.resolve(currentServices);
                } else {
                    this.getHome().then(function (home) {
                        var ds = home.getDomainServices();
                        return repLoader.populate(ds);
                    }).then(function (services) {
                        currentServices = services;
                        delay.resolve(services);
                    });
                }

                return delay.promise;
            };

            var currentObject = null;

            context.getObject = function (type, id) {
                var delay = $q.defer();

                if (currentObject && isSameObject(currentObject, type, id)) {
                    delay.resolve(currentObject);
                } else {
                    var promise = id ? this.getDomainObject(type, id) : this.getService(type);
                    promise.then(function (object) {
                        currentObject = object;
                        delay.resolve(object);
                    });
                }

                return delay.promise;
            };

            context.setObject = function (co) {
                currentObject = co;
            };

            var currentNestedObject = null;

            context.getNestedObject = function (type, id) {
                var delay = $q.defer();

                if (currentNestedObject && isSameObject(currentNestedObject, type, id)) {
                    delay.resolve(currentNestedObject);
                } else {
                    var object = new Spiro.DomainObjectRepresentation();
                    object.hateoasUrl = Spiro.appPath + "/objects/" + type + "/" + id;

                    repLoader.populate(object).then(function (object) {
                        currentNestedObject = object;
                        delay.resolve(object);
                    });
                }

                return delay.promise;
            };

            context.setNestedObject = function (cno) {
                currentNestedObject = cno;
            };

            var currentError = null;

            context.getError = function () {
                return currentError;
            };

            context.setError = function (e) {
                currentError = e;
            };

            var currentCollection = null;

            context.getCollection = function () {
                var delay = $q.defer();
                delay.resolve(currentCollection);
                return delay.promise;
            };

            context.setCollection = function (c) {
                currentCollection = c;
            };

            var currentTransient = null;

            context.getTransientObject = function () {
                var delay = $q.defer();
                delay.resolve(currentTransient);
                return delay.promise;
            };

            context.setTransientObject = function (t) {
                currentTransient = t;
            };

            var previousUrl = null;

            context.getPreviousUrl = function () {
                return previousUrl;
            };

            context.setPreviousUrl = function (url) {
                previousUrl = url;
            };

            var selectedChoice = null;

            context.getSelectedChoice = function (parm, search) {
                return selectedChoice ? selectedChoice[parm + ":" + search] : null;
            };

            context.setSelectedChoice = function (parm, search, cvm) {
                selectedChoice = selectedChoice || {};
                selectedChoice[parm + ":" + search] = cvm;
            };

            context.clearSelectedChoice = function () {
                return selectedChoice = null;
            };
        });
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular.services.context.js.map
