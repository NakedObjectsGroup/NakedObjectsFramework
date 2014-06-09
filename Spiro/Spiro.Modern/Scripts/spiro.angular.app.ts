/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/angularjs/angular-route.d.ts" />
/// <reference path="spiro.config.ts" />
/// <reference path="spiro.angular.config.ts" />

module Spiro.Angular {

    function getSvrPath() {
        var trimmedPath = svrPath.trim();

        if (trimmedPath.length == 0 || trimmedPath.charAt(svrPath.length - 1) == '/') {
            return trimmedPath;
        }
        return trimmedPath + '/';
    }


    // templates 
    export var nestedCollectionTemplate = getSvrPath() + "Content/partials/nestedCollection.html";
    export var nestedCollectionTableTemplate = getSvrPath() + "Content/partials/nestedCollectionTable.html";
    export var nestedObjectTemplate = getSvrPath() + "Content/partials/nestedObject.html";
    export var dialogTemplate = getSvrPath() + "Content/partials/dialog.html";
    export var servicesTemplate = getSvrPath() + "Content/partials/services.html";
    export var serviceTemplate = getSvrPath() + "Content/partials/service.html";
    export var actionTemplate = getSvrPath() + "Content/partials/actions.html";
    export var errorTemplate = getSvrPath() + "Content/partials/error.html";
    export var appBarTemplate = getSvrPath() + "Content/partials/appbar.html";
    export var objectTemplate = getSvrPath() + "Content/partials/object.html";
    export var viewPropertiesTemplate = getSvrPath() + "Content/partials/viewProperties.html";
    export var editPropertiesTemplate = getSvrPath() + "Content/partials/editProperties.html";

    var servicesPageTemplate = getSvrPath() + 'Content/partials/servicesPage.html';
    var servicePageTemplate = getSvrPath() + 'Content/partials/servicePage.html';
    var objectPageTemplate = getSvrPath() + 'Content/partials/objectPage.html';
    var transientObjectPageTemplate = getSvrPath() + 'Content/partials/transientObjectPage.html';
    var errorPageTemplate = getSvrPath() + 'Content/partials/errorPage.html';

    /* Declare app level module */
   
    export var app = angular.module('app', ['ngRoute', 'ngTouch']);

    app.config(($routeProvider: ng.route.IRouteProvider) => {
        $routeProvider.
            when('/services', {
                templateUrl: servicesPageTemplate, 
                controller: 'BackgroundController'
            }).
            when('/services/:sid', {
                templateUrl: servicePageTemplate,
                controller: 'BackgroundController'
            }).
            when('/objects/:dt/:id', {
                templateUrl: objectPageTemplate,
                controller: 'BackgroundController'
            }).
            when('/objects/:dt', {
                templateUrl: transientObjectPageTemplate, 
                controller: 'BackgroundController'
            }).
            when('/error', {
                templateUrl: errorPageTemplate,
                controller: 'BackgroundController'
            }).
            otherwise({
                redirectTo: '/services'
            });
    });

    export interface ISpiroRouteParams extends ng.route.IRouteParamsService {
        action: string;
        property: string;
        collectionItem: string;
        resultObject: string; 
        resultCollection: string; 
        collection: string; 
        editMode: string; 
        tableMode: string; 
        dt: string; 
        id: string; 
        sid: string; 
    }
}