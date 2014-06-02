/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/angularjs/angular-route.d.ts" />
/// <reference path="spiro.config.ts" />
/// <reference path="spiro.angular.config.ts" />
var Spiro;
(function (Spiro) {
    (function (Angular) {
        // templates
        Angular.nestedCollectionTemplate = Spiro.svrPath + "Content/partials/nestedCollection.html";
        Angular.nestedCollectionTableTemplate = Spiro.svrPath + "Content/partials/nestedCollectionTable.html";
        Angular.nestedObjectTemplate = Spiro.svrPath + "Content/partials/nestedObject.html";
        Angular.dialogTemplate = Spiro.svrPath + "Content/partials/dialog.html";
        Angular.servicesTemplate = Spiro.svrPath + "Content/partials/services.html";
        Angular.serviceTemplate = Spiro.svrPath + "Content/partials/service.html";
        Angular.actionTemplate = Spiro.svrPath + "Content/partials/actions.html";
        Angular.errorTemplate = Spiro.svrPath + "Content/partials/error.html";
        Angular.appBarTemplate = Spiro.svrPath + "Content/partials/appbar.html";
        Angular.objectTemplate = Spiro.svrPath + "Content/partials/object.html";
        Angular.viewPropertiesTemplate = Spiro.svrPath + "Content/partials/viewProperties.html";
        Angular.editPropertiesTemplate = Spiro.svrPath + "Content/partials/editProperties.html";

        var servicesPageTemplate = Spiro.svrPath + 'Content/partials/servicesPage.html';
        var servicePageTemplate = Spiro.svrPath + 'Content/partials/servicePage.html';
        var objectPageTemplate = Spiro.svrPath + 'Content/partials/objectPage.html';
        var transientObjectPageTemplate = Spiro.svrPath + 'Content/partials/transientObjectPage.html';
        var errorPageTemplate = Spiro.svrPath + 'Content/partials/errorPage.html';

        /* Declare app level module */
        Angular.app = angular.module('app', ['ngRoute', 'ngTouch']);

        Angular.app.config(function ($routeProvider) {
            $routeProvider.when('/services', {
                templateUrl: servicesPageTemplate,
                controller: 'BackgroundController'
            }).when('/services/:sid', {
                templateUrl: servicePageTemplate,
                controller: 'BackgroundController'
            }).when('/objects/:dt/:id', {
                templateUrl: objectPageTemplate,
                controller: 'BackgroundController'
            }).when('/objects/:dt', {
                templateUrl: transientObjectPageTemplate,
                controller: 'BackgroundController'
            }).when('/error', {
                templateUrl: errorPageTemplate,
                controller: 'BackgroundController'
            }).otherwise({
                redirectTo: '/services'
            });
        });
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular.app.js.map
