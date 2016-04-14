/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.services.handlers.ts" />
/// <reference path="nakedobjects.viewmodels.ts" />
/// <reference path="nakedobjects.app.ts" />
// tested 
var NakedObjects;
(function (NakedObjects) {
    NakedObjects.app.controller("Pane1HomeController", function ($scope, handlers, urlManager) {
        var routeData = urlManager.getRouteData();
        handlers.handleHome($scope, routeData.pane1);
    });
    NakedObjects.app.controller("Pane2HomeController", function ($scope, handlers, urlManager) {
        var routeData = urlManager.getRouteData();
        handlers.handleHome($scope, routeData.pane2);
    });
    NakedObjects.app.controller("Pane1ObjectController", function ($scope, handlers, urlManager) {
        var routeData = urlManager.getRouteData();
        handlers.handleObject($scope, routeData.pane1);
    });
    NakedObjects.app.controller("Pane2ObjectController", function ($scope, handlers, urlManager) {
        var routeData = urlManager.getRouteData();
        handlers.handleObject($scope, routeData.pane2);
    });
    NakedObjects.app.controller("Pane1ListController", function ($scope, handlers, urlManager) {
        var routeData = urlManager.getRouteData();
        handlers.handleList($scope, routeData.pane1);
    });
    NakedObjects.app.controller("Pane2ListController", function ($scope, handlers, urlManager) {
        var routeData = urlManager.getRouteData();
        handlers.handleList($scope, routeData.pane2);
    });
    NakedObjects.app.controller("Pane1RecentController", function ($scope, handlers, urlManager) {
        var routeData = urlManager.getRouteData();
        handlers.handleRecent($scope, routeData.pane1);
    });
    NakedObjects.app.controller("Pane2RecentController", function ($scope, handlers, urlManager) {
        var routeData = urlManager.getRouteData();
        handlers.handleRecent($scope, routeData.pane2);
    });
    NakedObjects.app.controller("BackgroundController", function ($scope, handlers) {
        handlers.handleBackground($scope);
    });
    NakedObjects.app.controller("ErrorController", function ($scope, handlers, urlManager) {
        var routeData = urlManager.getRouteData();
        handlers.handleError($scope, routeData.pane1);
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
        // todo is this supposed to be an assignment ?
        cvm.chainedCommands == null;
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.controllers.js.map