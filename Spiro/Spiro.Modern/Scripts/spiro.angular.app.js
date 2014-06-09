/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/angularjs/angular-route.d.ts" />
/// <reference path="spiro.config.ts" />
/// <reference path="spiro.angular.config.ts" />
var Spiro;
(function (Spiro) {
    (function (Angular) {
        function getSvrPath() {
            var trimmedPath = Spiro.svrPath.trim();

            if (trimmedPath.length == 0 || trimmedPath.charAt(Spiro.svrPath.length - 1) == '/') {
                return trimmedPath;
            }
            return trimmedPath + '/';
        }

        // templates
        Angular.nestedCollectionTemplate = getSvrPath() + "Content/partials/nestedCollection.html";
        Angular.nestedCollectionTableTemplate = getSvrPath() + "Content/partials/nestedCollectionTable.html";
        Angular.nestedObjectTemplate = getSvrPath() + "Content/partials/nestedObject.html";
        Angular.dialogTemplate = getSvrPath() + "Content/partials/dialog.html";
        Angular.servicesTemplate = getSvrPath() + "Content/partials/services.html";
        Angular.serviceTemplate = getSvrPath() + "Content/partials/service.html";
        Angular.actionTemplate = getSvrPath() + "Content/partials/actions.html";
        Angular.errorTemplate = getSvrPath() + "Content/partials/error.html";
        Angular.appBarTemplate = getSvrPath() + "Content/partials/appbar.html";
        Angular.objectTemplate = getSvrPath() + "Content/partials/object.html";
        Angular.viewPropertiesTemplate = getSvrPath() + "Content/partials/viewProperties.html";
        Angular.editPropertiesTemplate = getSvrPath() + "Content/partials/editProperties.html";

        var servicesPageTemplate = getSvrPath() + 'Content/partials/servicesPage.html';
        var servicePageTemplate = getSvrPath() + 'Content/partials/servicePage.html';
        var objectPageTemplate = getSvrPath() + 'Content/partials/objectPage.html';
        var transientObjectPageTemplate = getSvrPath() + 'Content/partials/transientObjectPage.html';
        var errorPageTemplate = getSvrPath() + 'Content/partials/errorPage.html';

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
