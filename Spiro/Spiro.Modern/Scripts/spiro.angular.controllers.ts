/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.viewmodels.ts" />
/// <reference path="spiro.angular.app.ts" />
/// <reference path="spiro.angular.services.handlers.ts" />

// tested 
module Spiro.Angular {

    app.controller('BackgroundController', ($scope: ng.IScope, handlers: IHandlers) => {
	    handlers.handleBackground($scope); 
    });

    app.controller('ServicesController', ($scope : ng.IScope, handlers: IHandlers) => {
	    handlers.handleServices($scope);
    });

    app.controller('ServiceController', ($scope: ng.IScope, handlers: IHandlers) => {
	    handlers.handleService($scope);
    });

    app.controller('DialogController', ($scope: ng.IScope, $routeParams: ISpiroRouteParams, handlers: IHandlers) => {
	    if ($routeParams.action) {
		    handlers.handleActionDialog($scope);
	    }
    });

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

    app.controller('CollectionController', ($scope: ng.IScope, $routeParams: ISpiroRouteParams, handlers: IHandlers) => {
	    if ($routeParams.resultCollection) {
		    handlers.handleCollectionResult($scope);
	    }
	    else if ($routeParams.collection) {
		    handlers.handleCollection($scope);
	    }
    });

    app.controller('ObjectController', ($scope: ng.IScope, $routeParams: ISpiroRouteParams, handlers: IHandlers) => {
	    if ($routeParams.editMode) {
		    handlers.handleEditObject($scope);
	    }
	    else {
		    handlers.handleObject($scope);
	    }
    });

    app.controller('TransientObjectController', ($scope: ng.IScope, handlers: IHandlers) => {
	    handlers.handleTransientObject($scope);
    });

    app.controller('ErrorController', ($scope: ng.IScope, handlers: IHandlers) => {
	    handlers.handleError($scope);
    });

    app.controller('AppBarController', ($scope: ng.IScope, handlers: IHandlers) => {
	    handlers.handleAppBar($scope);    
    });
}