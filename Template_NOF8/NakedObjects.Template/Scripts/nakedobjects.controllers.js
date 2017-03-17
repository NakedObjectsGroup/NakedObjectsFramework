/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.services.handlers.ts" />
/// <reference path="nakedobjects.viewmodels.ts" />
var NakedObjects;
(function (NakedObjects) {
    var pane1Dereg = function () { };
    var pane2Dereg = function () { };
    // Possible for race condition where $routeUpdate is called before we deregister the handler when location is changing
    // eg list to object. This means we could call the wrong search handler. This checks the page type matches the handler 
    // before calling the handler. If it's wrong just ignore and the handler will be deregistered by the new controller.
    function baseGuard(paneId, check, handler) {
        return function () {
            if (check(paneId)) {
                handler();
            }
        };
    }
    NakedObjects.app.controller("Pane1HomeController", function ($scope, handlers, urlManager) {
        pane1Dereg();
        var routeData = urlManager.getRouteData();
        handlers.handleHome($scope, routeData.pane1);
        var guard = _.partial(baseGuard, 1, urlManager.isHome);
        pane1Dereg = $scope.$on("$routeUpdate", guard(function () { return handlers.handleHomeSearch($scope, urlManager.getRouteData().pane1); }));
    });
    NakedObjects.app.controller("Pane2HomeController", function ($scope, handlers, urlManager) {
        pane2Dereg();
        var routeData = urlManager.getRouteData();
        handlers.handleHome($scope, routeData.pane2);
        var guard = _.partial(baseGuard, 2, urlManager.isHome);
        pane2Dereg = $scope.$on("$routeUpdate", guard(function () { return handlers.handleHomeSearch($scope, urlManager.getRouteData().pane2); }));
    });
    NakedObjects.app.controller("Pane1ObjectController", function ($scope, handlers, urlManager) {
        pane1Dereg();
        var routeData = urlManager.getRouteData();
        handlers.handleObject($scope, routeData.pane1);
        var guard = _.partial(baseGuard, 1, urlManager.isObject);
        pane1Dereg = $scope.$on("$routeUpdate", guard(function () { return handlers.handleObjectSearch($scope, urlManager.getRouteData().pane1); }));
    });
    NakedObjects.app.controller("Pane2ObjectController", function ($scope, handlers, urlManager) {
        pane2Dereg();
        var routeData = urlManager.getRouteData();
        handlers.handleObject($scope, routeData.pane2);
        var guard = _.partial(baseGuard, 2, urlManager.isObject);
        pane2Dereg = $scope.$on("$routeUpdate", guard(function () { return handlers.handleObjectSearch($scope, urlManager.getRouteData().pane2); }));
    });
    NakedObjects.app.controller("Pane1ListController", function ($scope, handlers, urlManager) {
        pane1Dereg();
        var routeData = urlManager.getRouteData();
        handlers.handleList($scope, routeData.pane1);
        var guard = _.partial(baseGuard, 1, urlManager.isList);
        pane1Dereg = $scope.$on("$routeUpdate", guard(function () { return handlers.handleListSearch($scope, urlManager.getRouteData().pane1); }));
    });
    NakedObjects.app.controller("Pane2ListController", function ($scope, handlers, urlManager) {
        pane2Dereg();
        var routeData = urlManager.getRouteData();
        handlers.handleList($scope, routeData.pane2);
        var guard = _.partial(baseGuard, 2, urlManager.isList);
        pane2Dereg = $scope.$on("$routeUpdate", guard(function () { return handlers.handleListSearch($scope, urlManager.getRouteData().pane2); }));
    });
    NakedObjects.app.controller("Pane1RecentController", function ($scope, handlers, urlManager) {
        pane1Dereg();
        var routeData = urlManager.getRouteData();
        handlers.handleRecent($scope, routeData.pane1);
        var guard = _.partial(baseGuard, 1, urlManager.isRecent);
        pane1Dereg = $scope.$on("$routeUpdate", guard(function () { return handlers.handleRecent($scope, urlManager.getRouteData().pane1); }));
    });
    NakedObjects.app.controller("Pane2RecentController", function ($scope, handlers, urlManager) {
        pane2Dereg();
        var routeData = urlManager.getRouteData();
        handlers.handleRecent($scope, routeData.pane2);
        var guard = _.partial(baseGuard, 2, urlManager.isRecent);
        pane2Dereg = $scope.$on("$routeUpdate", guard(function () { return handlers.handleRecent($scope, urlManager.getRouteData().pane2); }));
    });
    NakedObjects.app.controller("Pane1AttachmentController", function ($scope, handlers, urlManager) {
        pane1Dereg();
        var routeData = urlManager.getRouteData();
        handlers.handleAttachment($scope, routeData.pane1);
        var guard = _.partial(baseGuard, 1, urlManager.isAttachment);
        pane1Dereg = $scope.$on("$routeUpdate", guard(function () { return handlers.handleAttachment($scope, urlManager.getRouteData().pane1); }));
    });
    NakedObjects.app.controller("Pane2AttachmentController", function ($scope, handlers, urlManager) {
        pane2Dereg();
        var routeData = urlManager.getRouteData();
        handlers.handleAttachment($scope, routeData.pane2);
        var guard = _.partial(baseGuard, 2, urlManager.isAttachment);
        pane2Dereg = $scope.$on("$routeUpdate", guard(function () { return handlers.handleAttachment($scope, urlManager.getRouteData().pane2); }));
    });
    NakedObjects.app.controller("ApplicationPropertiesController", function ($scope, handlers, urlManager) {
        pane1Dereg();
        pane2Dereg();
        var routeData = urlManager.getRouteData();
        handlers.handleApplicationProperties($scope, routeData.pane1);
    });
    NakedObjects.app.controller("MultiLineDialogController", function ($scope, handlers, urlManager) {
        pane1Dereg();
        pane2Dereg();
        var routeData = urlManager.getRouteData();
        handlers.handleMultiLineDialog($scope, routeData.pane1);
    });
    NakedObjects.app.controller("BackgroundController", function ($scope, handlers) {
        handlers.handleBackground($scope);
    });
    NakedObjects.app.controller("ErrorController", function ($scope, handlers) {
        pane1Dereg();
        pane2Dereg();
        handlers.handleError($scope);
    });
    NakedObjects.app.controller("ToolBarController", function ($scope, handlers) {
        handlers.handleToolBar($scope);
    });
    NakedObjects.app.controller("CiceroHomeController", function ($scope, urlManager, context, viewModelFactory, commandFactory, focusManager) {
        var cvm = viewModelFactory.ciceroViewModel();
        $scope.cicero = cvm;
        cvm.viewType = NakedObjects.ViewType.Home;
        cvm.renderHome(urlManager.getRouteData().pane1);
        focusManager.focusOn(NakedObjects.FocusTarget.Input, 0, 1);
        cvm.executeNextChainedCommandIfAny();
    });
    NakedObjects.app.controller("CiceroObjectController", function ($scope, urlManager, context, viewModelFactory, commandFactory, focusManager) {
        var cvm = viewModelFactory.ciceroViewModel();
        $scope.cicero = cvm;
        cvm.viewType = NakedObjects.ViewType.Object;
        cvm.renderObject(urlManager.getRouteData().pane1);
        focusManager.focusOn(NakedObjects.FocusTarget.Input, 0, 1);
        cvm.executeNextChainedCommandIfAny();
    });
    NakedObjects.app.controller("CiceroListController", function ($scope, urlManager, context, viewModelFactory, commandFactory, focusManager) {
        var cvm = viewModelFactory.ciceroViewModel();
        $scope.cicero = cvm;
        cvm.viewType = NakedObjects.ViewType.List;
        cvm.renderList(urlManager.getRouteData().pane1);
        focusManager.focusOn(NakedObjects.FocusTarget.Input, 0, 1);
        cvm.executeNextChainedCommandIfAny();
    });
    NakedObjects.app.controller("CiceroErrorController", function ($scope, urlManager, context, viewModelFactory, commandFactory, focusManager) {
        var cvm = viewModelFactory.ciceroViewModel();
        $scope.cicero = cvm;
        //cvm.viewType = ViewType.Error;
        cvm.renderError();
        focusManager.focusOn(NakedObjects.FocusTarget.Input, 0, 1);
        cvm.chainedCommands = null;
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.controllers.js.map