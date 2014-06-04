/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.viewmodels.ts" />
/// <reference path="spiro.angular.app.ts" />
/// <reference path="spiro.angular.services.color.ts" />
/// <reference path="spiro.angular.services.representationloader.ts" />
/// <reference path="spiro.angular.services.urlhelper.ts" />
/// <reference path="spiro.angular.services.context.ts" />
/// <reference path="spiro.angular.services.viewmodelfactory.ts" />
/// <reference path="spiro.angular.services.navigation.browser.ts" />
var Spiro;
(function (Spiro) {
    (function (Angular) {
        Angular.app.service("handlers", function ($routeParams, $location, $q, $cacheFactory, repLoader, context, viewModelFactory, urlHelper, color, repHandlers, navigation) {
            var handlers = this;

            // tested
            handlers.handleBackground = function ($scope) {
                $scope.backgroundColor = color.toColorFromHref($location.absUrl());
                $scope.closeNestedObject = urlHelper.toAppUrl($location.path(), ["property", "collectionItem", "resultObject"]);
                $scope.closeCollection = urlHelper.toAppUrl($location.path(), ["collection", "resultCollection"]);
                navigation.push();
            };

            // tested
            handlers.handleCollectionResult = function ($scope) {
                context.getCollection().then(function (list) {
                    setNestedCollection($scope, list);
                }, function (error) {
                    setError(error);
                });
            };

            // tested
            handlers.handleCollection = function ($scope) {
                context.getObject($routeParams.dt, $routeParams.id).then(function (object) {
                    var collectionDetails = object.collectionMember($routeParams.collection).getDetails();
                    return repLoader.populate(collectionDetails);
                }).then(function (details) {
                    setNestedCollection($scope, details);
                }, function (error) {
                    setError(error);
                });
            };

            // tested
            handlers.handleActionDialog = function ($scope) {
                context.getObject($routeParams.sid || $routeParams.dt, $routeParams.id).then(function (object) {
                    var actionTarget = object.actionMember(urlHelper.action()).getDetails();
                    return repLoader.populate(actionTarget);
                }).then(function (action) {
                    if (action.extensions().hasParams) {
                        $scope.dialog = viewModelFactory.dialogViewModel(action, _.partial(repHandlers.invokeAction, $scope, action));
                        $scope.dialogTemplate = Angular.dialogTemplate;
                    }
                }, function (error) {
                    setError(error);
                });
            };

            // tested
            handlers.handleActionResult = function ($scope) {
                context.getObject($routeParams.sid || $routeParams.dt, $routeParams.id).then(function (object) {
                    var action = object.actionMember(urlHelper.action());

                    if (action.extensions().hasParams) {
                        var delay = $q.defer();
                        delay.reject();
                        return delay.promise;
                    }
                    var actionTarget = action.getDetails();
                    return repLoader.populate(actionTarget);
                }).then(function (action) {
                    var result = action.getInvoke();
                    return repLoader.populate(result, true);
                }).then(function (result) {
                    repHandlers.setResult(result);
                }, function (error) {
                    if (error) {
                        setError(error);
                    }
                    // otherwise just action with parms
                });
            };

            // tested
            handlers.handleProperty = function ($scope) {
                context.getObject($routeParams.dt, $routeParams.id).then(function (object) {
                    var propertyDetails = object.propertyMember($routeParams.property).getDetails();
                    return repLoader.populate(propertyDetails);
                }).then(function (details) {
                    var target = details.value().link().getTarget();
                    return repLoader.populate(target);
                }).then(function (object) {
                    setNestedObject(object, $scope);
                }, function (error) {
                    setError(error);
                });
            };

            //tested
            handlers.handleCollectionItem = function ($scope) {
                var collectionItemTypeKey = $routeParams.collectionItem.split("/");
                var collectionItemType = collectionItemTypeKey[0];
                var collectionItemKey = collectionItemTypeKey[1];

                context.getNestedObject(collectionItemType, collectionItemKey).then(function (object) {
                    setNestedObject(object, $scope);
                }, function (error) {
                    setError(error);
                });
            };

            // tested
            handlers.handleServices = function ($scope) {
                context.getServices().then(function (services) {
                    $scope.services = viewModelFactory.servicesViewModel(services);
                    $scope.servicesTemplate = Angular.servicesTemplate;
                    context.setObject(null);
                    context.setNestedObject(null);
                }, function (error) {
                    setError(error);
                });
            };

            // tested
            handlers.handleService = function ($scope) {
                context.getObject($routeParams.sid).then(function (service) {
                    $scope.object = viewModelFactory.serviceViewModel(service);
                    $scope.serviceTemplate = Angular.serviceTemplate;
                    $scope.actionTemplate = Angular.actionTemplate;
                }, function (error) {
                    setError(error);
                });
            };

            // tested
            handlers.handleResult = function ($scope) {
                var result = $routeParams.resultObject.split("-");
                var dt = result[0];
                var id = result[1];

                context.getNestedObject(dt, id).then(function (object) {
                    $scope.result = viewModelFactory.domainObjectViewModel(object); // todo rename result
                    $scope.nestedTemplate = Angular.nestedObjectTemplate;
                    context.setNestedObject(object);
                }, function (error) {
                    setError(error);
                });
            };

            // tested
            handlers.handleError = function ($scope) {
                var error = context.getError();
                if (error) {
                    var evm = viewModelFactory.errorViewModel(error);
                    $scope.error = evm;
                    $scope.errorTemplate = Angular.errorTemplate;
                }
            };

            // tested
            handlers.handleAppBar = function ($scope) {
                $scope.appBar = {};

                $scope.$on("ajax-change", function (event, count) {
                    if (count > 0) {
                        $scope.appBar.loading = "Loading...";
                    } else {
                        $scope.appBar.loading = "";
                    }
                });

                $scope.$on("back", function () {
                    navigation.back();
                });

                $scope.$on("forward", function () {
                    navigation.forward();
                });

                $scope.appBar.template = Angular.appBarTemplate;

                $scope.appBar.goHome = "#/";

                $scope.appBar.goBack = function () {
                    navigation.back();
                };

                $scope.appBar.goForward = function () {
                    navigation.forward();
                };

                $scope.appBar.hideEdit = true;

                // TODO create appbar viewmodel
                if ($routeParams.dt && $routeParams.id) {
                    context.getObject($routeParams.dt, $routeParams.id).then(function (object) {
                        var pms = _.toArray(object.propertyMembers());

                        var anyEditableField = _.any(pms, function (pm) {
                            return !pm.disabledReason();
                        });

                        $scope.appBar.hideEdit = !(object) || $routeParams.editMode || !anyEditableField;

                        // rework to use viewmodel code
                        $scope.appBar.doEdit = urlHelper.toAppUrl($location.path()) + "?editMode=true";
                    });
                }
            };

            //tested
            handlers.handleObject = function ($scope) {
                context.getObject($routeParams.dt, $routeParams.id).then(function (object) {
                    context.setNestedObject(null);
                    $scope.object = viewModelFactory.domainObjectViewModel(object);
                    $scope.objectTemplate = Angular.objectTemplate;
                    $scope.actionTemplate = Angular.actionTemplate;
                    $scope.propertiesTemplate = Angular.viewPropertiesTemplate;
                }, function (error) {
                    setError(error);
                });
            };

            handlers.handleTransientObject = function ($scope) {
                context.getTransientObject().then(function (object) {
                    if (object) {
                        $scope.backgroundColor = color.toColorFromType(object.domainType());

                        context.setNestedObject(null);
                        var obj = viewModelFactory.domainObjectViewModel(object, null, _.partial(repHandlers.saveObject, $scope, object));
                        obj.cancelEdit = urlHelper.toAppUrl(context.getPreviousUrl());

                        $scope.object = obj;
                        $scope.objectTemplate = Angular.objectTemplate;
                        $scope.actionTemplate = "";
                        $scope.propertiesTemplate = Angular.editPropertiesTemplate;
                    } else {
                        // transient has disappeared - return to previous page
                        //parent.history.back();
                        navigation.back();
                    }
                }, function (error) {
                    setError(error);
                });
            };

            // tested
            handlers.handleEditObject = function ($scope) {
                context.getObject($routeParams.dt, $routeParams.id).then(function (object) {
                    var detailPromises = _.map(object.propertyMembers(), function (pm) {
                        return repLoader.populate(pm.getDetails());
                    });

                    $q.all(detailPromises).then(function (details) {
                        context.setNestedObject(null);
                        $scope.object = viewModelFactory.domainObjectViewModel(object, details, _.partial(repHandlers.updateObject, $scope, object));
                        $scope.objectTemplate = Angular.objectTemplate;
                        $scope.actionTemplate = "";
                        $scope.propertiesTemplate = Angular.editPropertiesTemplate;
                    }, function (error) {
                        setError(error);
                    });
                }, function (error) {
                    setError(error);
                });
            };

            // helper functions
            function setNestedObject(object, $scope) {
                $scope.result = viewModelFactory.domainObjectViewModel(object); // todo rename result
                $scope.nestedTemplate = Angular.nestedObjectTemplate;
                context.setNestedObject(object);
            }

            function setNestedCollection($scope, listOrCollection) {
                if ($routeParams.tableMode) {
                    $scope.collection = viewModelFactory.collectionViewModel(listOrCollection, true);
                    $scope.modeCollection = urlHelper.toAppUrl($location.path(), []);
                    $scope.collectionTemplate = Angular.nestedCollectionTableTemplate;
                } else {
                    $scope.collection = viewModelFactory.collectionViewModel(listOrCollection);
                    $scope.modeCollection = urlHelper.toAppUrl($location.path(), []) + "&tableMode=true";
                    $scope.collectionTemplate = Angular.nestedCollectionTemplate;
                }
            }

            function setError(error) {
                var errorRep;
                if (error instanceof Spiro.ErrorRepresentation) {
                    errorRep = error;
                } else {
                    errorRep = new Spiro.ErrorRepresentation({ message: "an unrecognised error has occurred" });
                }
                context.setError(errorRep);
                $location.path(urlHelper.toErrorPath());
            }
        });
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular.services.handlers.js.map
