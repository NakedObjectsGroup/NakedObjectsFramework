/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.viewmodels.ts" />
/// <reference path="spiro.angular.app.ts" />
/// <reference path="spiro.angular.services.handlers.ts" />
var Spiro;
(function (Spiro) {
    // tested
    (function (Angular) {
        Spiro.Angular.app.controller('BackgroundController', function ($scope, handlers) {
            handlers.handleBackground($scope);
        });

        Spiro.Angular.app.controller('ServicesController', function ($scope, handlers) {
            handlers.handleServices($scope);
        });

        Spiro.Angular.app.controller('ServiceController', function ($scope, handlers) {
            handlers.handleService($scope);
        });

        Spiro.Angular.app.controller('DialogController', function ($scope, $routeParams, handlers) {
            if ($routeParams.action) {
                handlers.handleActionDialog($scope);
            }
        });

        Spiro.Angular.app.controller('NestedObjectController', function ($scope, $routeParams, handlers) {
            // action takes priority
            if ($routeParams.action) {
                handlers.handleActionResult($scope);
            }

            // action + one of
            if ($routeParams.property) {
                handlers.handleProperty($scope);
            } else if ($routeParams.collectionItem) {
                handlers.handleCollectionItem($scope);
            } else if ($routeParams.resultObject) {
                handlers.handleResult($scope);
            }
        });

        Spiro.Angular.app.controller('CollectionController', function ($scope, $routeParams, handlers) {
            if ($routeParams.resultCollection) {
                handlers.handleCollectionResult($scope);
            } else if ($routeParams.collection) {
                handlers.handleCollection($scope);
            }
        });

        Spiro.Angular.app.controller('ObjectController', function ($scope, $routeParams, handlers) {
            if ($routeParams.editMode) {
                handlers.handleEditObject($scope);
            } else {
                handlers.handleObject($scope);
            }
        });

        Spiro.Angular.app.controller('TransientObjectController', function ($scope, handlers) {
            handlers.handleTransientObject($scope);
        });

        Spiro.Angular.app.controller('ErrorController', function ($scope, handlers) {
            handlers.handleError($scope);
        });

        Spiro.Angular.app.controller('AppBarController', function ($scope, handlers) {
            handlers.handleAppBar($scope);
        });
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular.controllers.js.map
