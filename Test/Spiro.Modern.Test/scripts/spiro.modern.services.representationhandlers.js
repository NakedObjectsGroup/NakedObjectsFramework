/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />
var Spiro;
(function (Spiro) {
    (function (Angular) {
        (function (Modern) {
            Angular.app.service('repHandlers', function ($q, $location, $cacheFactory, repLoader, context, urlHelper) {
                var repHandlers = this;

                repHandlers.prompt = function (promptRep, id, searchTerm) {
                    promptRep.reset();
                    promptRep.setSearchTerm(searchTerm);

                    return repLoader.populate(promptRep, true).then(function (p) {
                        var delay = $q.defer();

                        var cvms = _.map(p.choices(), function (v, k) {
                            return Modern.ChoiceViewModel.create(v, id, k, searchTerm);
                        });

                        delay.resolve(cvms);
                        return delay.promise;
                    });
                };

                repHandlers.conditionalChoices = function (promptRep, id, args) {
                    promptRep.reset();
                    promptRep.setArguments(args);

                    return repLoader.populate(promptRep, true).then(function (p) {
                        var delay = $q.defer();

                        var cvms = _.map(p.choices(), function (v, k) {
                            return Modern.ChoiceViewModel.create(v, id, k);
                        });

                        delay.resolve(cvms);
                        return delay.promise;
                    });
                };

                repHandlers.setResult = function (result, dvm) {
                    if (result.result().isNull() && result.resultType() !== "void") {
                        if (dvm) {
                            dvm.message = "no result found";
                        }
                        return;
                    }

                    var parms = "";

                    // transient object
                    if (result.resultType() === "object" && result.result().object().persistLink()) {
                        var resultObject = result.result().object();
                        var domainType = resultObject.extensions().domainType;

                        resultObject.set("domainType", domainType);
                        resultObject.set("instanceId", "0");
                        resultObject.hateoasUrl = "/" + domainType + "/0";

                        context.setTransientObject(resultObject);

                        context.setPreviousUrl($location.path());
                        $location.path(urlHelper.toTransientObjectPath(resultObject));
                    }

                    // persistent object
                    if (result.resultType() === "object" && !result.result().object().persistLink()) {
                        var resultObject = result.result().object();

                        // set the nested object here and then update the url. That should reload the page but pick up this object
                        // so we don't hit the server again.
                        context.setNestedObject(resultObject);
                        parms = urlHelper.updateParms(resultObject, dvm);
                    }

                    if (result.resultType() === "list") {
                        var resultList = result.result().list();
                        context.setCollection(resultList);
                        parms = urlHelper.updateParms(resultList, dvm);
                    }

                    $location.search(parms);
                };

                repHandlers.setInvokeUpdateError = function ($scope, error, vms, vm) {
                    if (error instanceof Spiro.ErrorMap) {
                        _.each(vms, function (vmi) {
                            var errorValue = error.valuesMap()[vmi.id];

                            if (errorValue) {
                                vmi.value = errorValue.value.toValueString();
                                vmi.message = errorValue.invalidReason;
                            }
                        });
                        vm.message = error.invalidReason();
                    } else if (error instanceof Spiro.ErrorRepresentation) {
                        context.setError(error);
                        $location.path(urlHelper.toErrorPath());
                    } else {
                        vm.message = error;
                    }
                };

                repHandlers.invokeAction = function ($scope, action, dvm) {
                    dvm.clearMessages();

                    var invoke = action.getInvoke();

                    var parameters = dvm.parameters;
                    _.each(parameters, function (parm) {
                        return invoke.setParameter(parm.id, parm.getValue());
                    });
                    _.each(parameters, function (parm) {
                        return parm.setSelectedChoice();
                    });

                    repLoader.populate(invoke, true).then(function (result) {
                        repHandlers.setResult(result, dvm);
                    }, function (error) {
                        repHandlers.setInvokeUpdateError($scope, error, parameters, dvm);
                    });
                };

                repHandlers.updateObject = function ($scope, object, ovm) {
                    var update = object.getUpdateMap();

                    var properties = _.filter(ovm.properties, function (property) {
                        return property.isEditable;
                    });
                    _.each(properties, function (property) {
                        return update.setProperty(property.id, property.getValue());
                    });

                    repLoader.populate(update, true, new Spiro.DomainObjectRepresentation()).then(function (updatedObject) {
                        // This is a kludge because updated object has no self link.
                        var rawLinks = object.get("links");
                        updatedObject.set("links", rawLinks);

                        // remove pre-changed object from cache
                        $cacheFactory.get('$http').remove(updatedObject.url());

                        context.setObject(updatedObject);
                        $location.search("");
                    }, function (error) {
                        repHandlers.setInvokeUpdateError($scope, error, properties, ovm);
                    });
                };

                repHandlers.saveObject = function ($scope, object, ovm) {
                    var persist = object.getPersistMap();

                    var properties = _.filter(ovm.properties, function (property) {
                        return property.isEditable;
                    });
                    _.each(properties, function (property) {
                        return persist.setMember(property.id, property.getValue());
                    });

                    repLoader.populate(persist, true, new Spiro.DomainObjectRepresentation()).then(function (updatedObject) {
                        context.setObject(updatedObject);
                        $location.path(urlHelper.toObjectPath(updatedObject));
                    }, function (error) {
                        repHandlers.setInvokeUpdateError($scope, error, properties, ovm);
                    });
                };
            });
        })(Angular.Modern || (Angular.Modern = {}));
        var Modern = Angular.Modern;
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.modern.services.representationhandlers.js.map
