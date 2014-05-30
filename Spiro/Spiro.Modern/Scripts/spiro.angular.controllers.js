/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.viewmodels.ts" />
/// <reference path="spiro.angular.app.ts" />
/// <reference path="spiro.angular.services.handlers.ts" />
var Spiro;
(function (Spiro) {
    // tested
    (function (Angular) {
        Angular.app.controller('BackgroundController', function ($scope, handlers) {
            handlers.handleBackground($scope);
        });

        Angular.app.controller('ServicesController', function ($scope, handlers) {
            handlers.handleServices($scope);
        });

        Angular.app.controller('ServiceController', function ($scope, handlers) {
            handlers.handleService($scope);
        });

        Angular.app.controller('DialogController', function ($scope, $routeParams, handlers) {
            if ($routeParams.action) {
                handlers.handleActionDialog($scope);
            }
        });

        Angular.app.controller('NestedObjectController', function ($scope, $routeParams, handlers) {
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

        Angular.app.controller('CollectionController', function ($scope, $routeParams, handlers) {
            if ($routeParams.resultCollection) {
                handlers.handleCollectionResult($scope);
            } else if ($routeParams.collection) {
                handlers.handleCollection($scope);
            }
        });

        Angular.app.controller('ObjectController', function ($scope, $routeParams, handlers) {
            if ($routeParams.editMode) {
                handlers.handleEditObject($scope);
            } else {
                handlers.handleObject($scope);
            }
        });

        Angular.app.controller('TransientObjectController', function ($scope, handlers) {
            handlers.handleTransientObject($scope);
        });

        Angular.app.controller('ErrorController', function ($scope, handlers) {
            handlers.handleError($scope);
        });

        Angular.app.controller('AppBarController', function ($scope, handlers) {
            handlers.handleAppBar($scope);
        });
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular.controllers.js.map
