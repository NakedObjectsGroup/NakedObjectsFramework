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

    app.service("handlers", function ($routeParams: ISpiroRouteParams, $location: ng.ILocationService, $q: ng.IQService, $cacheFactory: ng.ICacheFactoryService, repLoader: IRepLoader, context: IContext, viewModelFactory: IViewModelFactory, urlHelper: IUrlHelper, color : IColor, repHandlers : IRepHandlers, navigation : INavigation) {

        var handlers = <IHandlers>this;

        handlers.handleBackground = $scope => {
            $scope.backgroundColor = color.toColorFromHref($location.absUrl());
            $scope.closeNestedObject = urlHelper.toAppUrl($location.path(), ["property", "collectionItem", "resultObject"]);
            $scope.closeCollection = urlHelper.toAppUrl($location.path(), ["collection", "resultCollection"]);
            navigation.push();
        };

        

        // tested
        handlers.handleCollectionResult = $scope => {
            context.getCollection().
                then((list: ListRepresentation) => {
                setNestedCollection($scope, list);
            }, error => {
                setError(error);
            });
        };

        // tested
        handlers.handleCollection = $scope => {
            context.getObject($routeParams.dt, $routeParams.id).
                then((object: DomainObjectRepresentation) => {
                    var collectionDetails = object.collectionMember($routeParams.collection).getDetails();
                    return repLoader.populate(collectionDetails);
                }).
                then((details: CollectionRepresentation) => {
                setNestedCollection($scope, details);
            }, error => {
                setError(error);
            });
        };

        // tested
        handlers.handleActionDialog = $scope => {
           
            context.getObject($routeParams.sid || $routeParams.dt, $routeParams.id).
                then((object: DomainObjectRepresentation) => {
                    var actionTarget = object.actionMember(urlHelper.action()).getDetails();
                    return repLoader.populate(actionTarget);
                }).
                then((action: ActionRepresentation) => {
                if (action.extensions().hasParams) {      
                    $scope.dialog = viewModelFactory.dialogViewModel(action, <(dvm: DialogViewModel) => void > _.partial(repHandlers.invokeAction, $scope, action));
                    $scope.dialogTemplate = dialogTemplate;
                }
            }, error => {
                setError(error);
            });
        };

        // tested
        handlers.handleActionResult = $scope => {
            context.getObject($routeParams.sid || $routeParams.dt, $routeParams.id).
                then((object: DomainObjectRepresentation) => {
                    var action = object.actionMember(urlHelper.action());

                    if (action.extensions().hasParams) {
                        var delay = $q.defer();
                        delay.reject();
                        return delay.promise;
                    }
                    var actionTarget = action.getDetails();
                    return repLoader.populate(actionTarget);
                }).
                then((action: ActionRepresentation) => {
                    var result = action.getInvoke();
                    return repLoader.populate(result, true);
                }).
                then((result: ActionResultRepresentation) => {
                repHandlers.setResult(result);
            }, error => {
                if (error) {
                    setError(error);
                }
                // otherwise just action with parms 
            });
        };

        // tested
        handlers.handleProperty = $scope => {
            context.getObject($routeParams.dt, $routeParams.id).
                then((object: DomainObjectRepresentation) => {
                    var propertyDetails = object.propertyMember($routeParams.property).getDetails();
                    return repLoader.populate(propertyDetails);
                }).
                then((details: PropertyRepresentation) => {
                    var target = details.value().link().getTarget();
                    return repLoader.populate(target);
                }).
                then((object: DomainObjectRepresentation) => {
                setNestedObject(object, $scope);
            }, error => {
                setError(error);
            });
        };

        //tested
        handlers.handleCollectionItem = $scope => {
            var collectionItemTypeKey = $routeParams.collectionItem.split("/");
            var collectionItemType = collectionItemTypeKey[0];
            var collectionItemKey = collectionItemTypeKey[1];

            context.getNestedObject(collectionItemType, collectionItemKey).
                then((object: DomainObjectRepresentation) => {
                setNestedObject(object, $scope);
            }, error => {
                setError(error);
            });
        };

        // tested
        handlers.handleServices = $scope => {       
            context.getServices().
                then((services: DomainServicesRepresentation) => {
                $scope.services = viewModelFactory.servicesViewModel(services);
                $scope.servicesTemplate = servicesTemplate;
                context.setObject(null);
                context.setNestedObject(null);
            }, error => {
                setError(error);
            });
        };

        // tested
       handlers.handleService = $scope => {      
           context.getObject($routeParams.sid).
               then((service: DomainObjectRepresentation) => {
               $scope.object = viewModelFactory.serviceViewModel(service);
               $scope.serviceTemplate = serviceTemplate;
               $scope.actionTemplate = actionTemplate;           
           }, error => {
               setError(error);
           });

       };

        // tested
        handlers.handleResult = $scope => {
           
            var result = $routeParams.resultObject.split("-");
            var dt = result[0];
            var id = result[1];

            context.getNestedObject(dt, id).
                then((object: DomainObjectRepresentation) => {
                $scope.result = viewModelFactory.domainObjectViewModel(object); // todo rename result
                $scope.nestedTemplate = nestedObjectTemplate;
                context.setNestedObject(object);
            }, error => {
                setError(error);
            });

        };

        // tested
        handlers.handleError = $scope => {          
            var error = context.getError();
            if (error) {
                var evm = viewModelFactory.errorViewModel(error);
                $scope.error = evm;
                $scope.errorTemplate = errorTemplate;
            }
        };

        // tested
        handlers.handleAppBar = $scope => {

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

            $scope.appBar.goBack = () => {
                navigation.back();      
            };

            $scope.appBar.goForward = () => {
                navigation.forward();
            };

            $scope.appBar.hideEdit = true;

            // TODO create appbar viewmodel 

            if ($routeParams.dt && $routeParams.id) {
                context.getObject($routeParams.dt, $routeParams.id).
                    then((object: DomainObjectRepresentation) => {

                    var pms = <PropertyMember[]>  _.toArray(object.propertyMembers());

                    var anyEditableField = _.any(pms, (pm : PropertyMember) => !pm.disabledReason());

                    $scope.appBar.hideEdit = !(object) || $routeParams.editMode || !anyEditableField;

                    // rework to use viewmodel code
                       
                    $scope.appBar.doEdit = urlHelper.toAppUrl($location.path()) + "?editMode=true";
                });
            }
        };

        //tested
        handlers.handleObject = $scope => {
        
            context.getObject($routeParams.dt, $routeParams.id).
                then((object: DomainObjectRepresentation) => {
                context.setNestedObject(null);
                $scope.object = viewModelFactory.domainObjectViewModel(object);
                $scope.objectTemplate = objectTemplate;
                $scope.actionTemplate = actionTemplate;
                $scope.propertiesTemplate = viewPropertiesTemplate;
            }, error => {
                setError(error);
            });

        };

        handlers.handleTransientObject = $scope => {

            context.getTransientObject().
                then((object: DomainObjectRepresentation) => {

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

            }, error => {
                setError(error);
            });
        };


        // tested
        handlers.handleEditObject = $scope => {

            context.getObject($routeParams.dt, $routeParams.id).
                then((object: DomainObjectRepresentation) => {

                var detailPromises = _.map(object.propertyMembers(), (pm: PropertyMember) => { return repLoader.populate(pm.getDetails()); });

                $q.all(detailPromises).then((details: PropertyRepresentation[]) => {
                    context.setNestedObject(null);
                    $scope.object = viewModelFactory.domainObjectViewModel(object, details, <(ovm: DomainObjectViewModel) => void> _.partial(repHandlers.updateObject, $scope, object));
                    $scope.objectTemplate = objectTemplate;
                    $scope.actionTemplate = "";
                    $scope.propertiesTemplate = editPropertiesTemplate;
                }, error => {
                    setError(error);
                });

            }, error => {
                setError(error);
            });
        };

        // helper functions 

        function setNestedObject(object: DomainObjectRepresentation, $scope) {
            $scope.result = viewModelFactory.domainObjectViewModel(object); // todo rename result
            $scope.nestedTemplate = nestedObjectTemplate;
            context.setNestedObject(object);
        }

        function setNestedCollection($scope, listOrCollection: ListOrCollection) {
            

            if ($routeParams.tableMode) {
                $scope.collection = viewModelFactory.collectionViewModel(listOrCollection, true);
                $scope.modeCollection = urlHelper.toAppUrl($location.path(), []);
                $scope.collectionTemplate = nestedCollectionTableTemplate;
            } else {
                $scope.collection = viewModelFactory.collectionViewModel(listOrCollection);
                $scope.modeCollection = urlHelper.toAppUrl($location.path(), []) + "&tableMode=true";
                $scope.collectionTemplate = nestedCollectionTemplate;
            }
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