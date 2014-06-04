/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.viewmodels.ts" />
/// <reference path="spiro.angular.app.ts" />
/// <reference path="spiro.angular.services.handlers.ts" />

// tested 
module Spiro.Angular {

	// tested
    app.controller('BackgroundController', ($scope: ng.IScope, handlers: IHandlers) => {
	    handlers.handleBackground($scope); 
    });

    // tested
    app.controller('ServicesController', ($scope : ng.IScope, handlers: IHandlers) => {
	    handlers.handleServices($scope);
    });

    // tested
    app.controller('ServiceController', ($scope: ng.IScope, handlers: IHandlers) => {
	    handlers.handleService($scope);
    });

    // tested
    app.controller('DialogController', ($scope: ng.IScope, $routeParams: ISpiroRouteParams, handlers: IHandlers) => {
	    if ($routeParams.action) {
		    handlers.handleActionDialog($scope);
	    }
    });

    // tested
    app.controller('NestedObjectController', ($scope: ng.IScope, $routeParams: ISpiroRouteParams, handlers: IHandlers) => {

	    // action takes priority 
	    if ($routeParams.action) {
		    handlers.handleActionResult($scope);
	    }
	    // action + one of  
	    if ($routeParams.property) {
		    handlers.handleProperty($scope);
	    }
	    else if ($routeParams.collectionItem) {
		    handlers.handleCollectionItem($scope);
	    }
	    else if ($routeParams.resultObject) {
		    handlers.handleResult($scope);
	    }
    });

    // tested
    app.controller('CollectionController', ($scope: ng.IScope, $routeParams: ISpiroRouteParams, handlers: IHandlers) => {
	    if ($routeParams.resultCollection) {
		    handlers.handleCollectionResult($scope);
	    }
	    else if ($routeParams.collection) {
		    handlers.handleCollection($scope);
	    }
    });

    // tested
    app.controller('ObjectController', ($scope: ng.IScope, $routeParams: ISpiroRouteParams, handlers: IHandlers) => {
	    if ($routeParams.editMode) {
		    handlers.handleEditObject($scope);
	    }
	    else {
		    handlers.handleObject($scope);
	    }
    });

    // tested
    app.controller('TransientObjectController', ($scope: ng.IScope, handlers: IHandlers) => {
	    handlers.handleTransientObject($scope);
    });

    // tested
    app.controller('ErrorController', ($scope: ng.IScope, handlers: IHandlers) => {
	    handlers.handleError($scope);
    });

    // tested
    app.controller('AppBarController', ($scope: ng.IScope, handlers: IHandlers) => {
	    handlers.handleAppBar($scope);    
    });
}