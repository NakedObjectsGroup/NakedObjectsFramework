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

module Spiro.Angular {


    export interface IHandlers {
        handleBackground($scope): void;
        handleCollectionResult($scope): void;
        handleCollection($scope): void;
        handleActionDialog($scope): void;
        handleActionResult($scope): void;
        handleProperty($scope): void;
        handleCollectionItem($scope): void;
        handleError(error: any): void;
        handleServices($scope): void;
        handleService($scope): void;
        handleResult($scope): void;
        handleEditObject($scope): void;
        handleTransientObject($scope): void;
        handleObject($scope): void;
        handleAppBar($scope): void;
    }

    app.service("handlers", function ($routeParams: ISpiroRouteParams, $location: ng.ILocationService, $q: ng.IQService, $cacheFactory: ng.ICacheFactoryService, repLoader: IRepLoader, context: IContext, viewModelFactory: IViewModelFactory, urlHelper: IUrlHelper, color : IColor, repHandlers : IRepHandlers, navigation : INavigation,  $route) {

        var handlers = <IHandlers>this;

        handlers.handleBackground = function ($scope) {
            $scope.backgroundColor = color.toColorFromHref($location.absUrl());
            $scope.closeNestedObject = urlHelper.toAppUrl($location.path(), ["property", "collectionItem", "resultObject"]);
            $scope.closeCollection = urlHelper.toAppUrl($location.path(), ["collection", "resultCollection"]);
            navigation.push();
        };

        // tested
        handlers.handleCollectionResult = function ($scope) {
            context.getCollection().
                then(function (list: ListRepresentation) {
                    $scope.collection = viewModelFactory.collectionViewModel(list);
                    $scope.collectionTemplate = nestedCollectionTemplate;             
                }, function (error) {
                    setError(error);
                });
        };

        // tested
        handlers.handleCollection = function ($scope) {
            context.getObject($routeParams.dt, $routeParams.id).
                then(function (object: DomainObjectRepresentation) {
                    var collectionDetails = object.collectionMember($routeParams.collection).getDetails();
                    return repLoader.populate(collectionDetails);
                }).
                then(function (details: CollectionRepresentation) {
                    $scope.collection = viewModelFactory.collectionViewModel(details);
                    $scope.collectionTemplate = nestedCollectionTemplate;
                }, function (error) {
                    setError(error);
                });
        };

        // tested
        handlers.handleActionDialog = function ($scope) {
           
            context.getObject($routeParams.sid || $routeParams.dt, $routeParams.id).
                then(function (object: DomainObjectRepresentation) {
                    var actionTarget = object.actionMember(urlHelper.action()).getDetails();
                    return repLoader.populate(actionTarget);
                }).
                then(function (action: ActionRepresentation) {
                    if (action.extensions().hasParams) {      
                        $scope.dialog = viewModelFactory.dialogViewModel(action, <(dvm: DialogViewModel) => void > _.partial(repHandlers.invokeAction, $scope, action));
                        $scope.dialogTemplate = dialogTemplate;
                    }
                }, function (error) {
                    setError(error);
                });
        };

        // tested
        handlers.handleActionResult = function ($scope) {
            context.getObject($routeParams.sid || $routeParams.dt, $routeParams.id).
                then(function (object: DomainObjectRepresentation) {
                    var action = object.actionMember(urlHelper.action());

                    if (action.extensions().hasParams) {
                        var delay = $q.defer();
                        delay.reject();
                        return delay.promise;
                    }
                    var actionTarget = action.getDetails();
                    return repLoader.populate(actionTarget);
                }).
                then(function (action: ActionRepresentation) {
                    var result = action.getInvoke();
                    return repLoader.populate(result, true);
                }).
                then(function (result: ActionResultRepresentation) {
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
            context.getObject($routeParams.dt, $routeParams.id).
                then(function (object: DomainObjectRepresentation) {
                    var propertyDetails = object.propertyMember($routeParams.property).getDetails();
                    return repLoader.populate(propertyDetails);
                }).
                then(function (details: PropertyRepresentation) {
                    var target = details.value().link().getTarget();
                    return repLoader.populate(target);
                }).
                then(function (object: DomainObjectRepresentation) {
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

            context.getNestedObject(collectionItemType, collectionItemKey).
                then(function (object: DomainObjectRepresentation) {
                    setNestedObject(object, $scope);
                }, function (error) {
                    setError(error);
                });
        };

        // tested
        handlers.handleServices = function ($scope) {       
            context.getServices().
                then(function (services: DomainServicesRepresentation) {
                    $scope.services = viewModelFactory.servicesViewModel(services);
                    $scope.servicesTemplate = servicesTemplate;
                    context.setObject(null);
                    context.setNestedObject(null);
                }, function (error) {
                    setError(error);
                });
        };

        // tested
       handlers.handleService = function ($scope) {      
            context.getObject($routeParams.sid).
                then(function (service: DomainObjectRepresentation) {
                    $scope.object = viewModelFactory.serviceViewModel(service);
                    $scope.serviceTemplate = serviceTemplate;
                    $scope.actionTemplate = actionTemplate;           
                }, function (error) {
                    setError(error);
                });

        };

        // tested
        handlers.handleResult = function ($scope) {
           
            var result = $routeParams.resultObject.split("-");
            var dt = result[0];
            var id = result[1];

            context.getNestedObject(dt, id).
                then(function (object: DomainObjectRepresentation) {
                    $scope.result = viewModelFactory.domainObjectViewModel(object); // todo rename result
                    $scope.nestedTemplate = nestedObjectTemplate;
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
                $scope.errorTemplate = errorTemplate;
            }
        };

        // tested
        handlers.handleAppBar = function ($scope) {

            $scope.appBar = {};

            $scope.$on("ajax-change", (event, count) => {
                if (count > 0) {
                    $scope.appBar.loading = "Loading...";
                }
                else {
                    $scope.appBar.loading = "";
                }
            });

            $scope.$on("back", () => {
                navigation.back();
            });

            $scope.$on("forward", () => {
                navigation.forward();
            });

            $scope.appBar.template = appBarTemplate;

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
                context.getObject($routeParams.dt, $routeParams.id).
                    then(function (object: DomainObjectRepresentation) {

                        $scope.appBar.hideEdit = !(object) || $routeParams.editMode || false;

                        // rework to use viewmodel code
                       
                        $scope.appBar.doEdit = urlHelper.toAppUrl($location.path()) + "?editMode=true";
                    });
            }
        };

        //tested
        handlers.handleObject = function ($scope) {
        
            context.getObject($routeParams.dt, $routeParams.id).
                then(function (object: DomainObjectRepresentation) {
                    context.setNestedObject(null);
                    $scope.object = viewModelFactory.domainObjectViewModel(object);
                    $scope.objectTemplate = objectTemplate;
                    $scope.actionTemplate = actionTemplate;
                    $scope.propertiesTemplate = viewPropertiesTemplate;
                }, function (error) {
                    setError(error);
                });

        };

        handlers.handleTransientObject = function ($scope) {

            context.getTransientObject().
                then(function (object: DomainObjectRepresentation) {

                    if (object) {

                        $scope.backgroundColor = color.toColorFromType(object.domainType());

                        context.setNestedObject(null);
                        var obj = viewModelFactory.domainObjectViewModel(object, null, <(ovm: DomainObjectViewModel) => void> _.partial(repHandlers.saveObject, $scope, object));
                        obj.cancelEdit =  urlHelper.toAppUrl(context.getPreviousUrl()); 

                        $scope.object = obj;
                        $scope.objectTemplate = objectTemplate;
                        $scope.actionTemplate = "";
                        $scope.propertiesTemplate = editPropertiesTemplate;

                    }
                    else {
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

            context.getObject($routeParams.dt, $routeParams.id).
                then(function (object: DomainObjectRepresentation) {

                    var detailPromises = _.map(object.propertyMembers(), (pm: PropertyMember) => { return repLoader.populate(pm.getDetails()) });

                    $q.all(detailPromises).then(function (details: PropertyRepresentation[]) {
                        context.setNestedObject(null);
                        $scope.object = viewModelFactory.domainObjectViewModel(object, details, <(ovm: DomainObjectViewModel) => void> _.partial(repHandlers.updateObject, $scope, object));
                        $scope.objectTemplate = objectTemplate;
                        $scope.actionTemplate = "";
                        $scope.propertiesTemplate = editPropertiesTemplate;
                    }, function (error) {
                            setError(error);
                        });

                }, function (error) {
                    setError(error);
                });
        };

        // helper functions 

        function setNestedObject(object: DomainObjectRepresentation, $scope) {
            $scope.result = viewModelFactory.domainObjectViewModel(object); // todo rename result
            $scope.nestedTemplate = nestedObjectTemplate;
            context.setNestedObject(object);
        }

        function setError(error) {

            var errorRep: ErrorRepresentation;
            if (error instanceof ErrorRepresentation) {
                errorRep = <ErrorRepresentation>error;
            }
            else {
                errorRep = new ErrorRepresentation({ message: "an unrecognised error has occurred" });
            }
            context.setError(errorRep);
            $location.path(urlHelper.toErrorPath());
        }

    });
}