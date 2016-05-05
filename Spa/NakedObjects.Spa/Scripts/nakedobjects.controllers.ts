/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.services.handlers.ts" />
/// <reference path="nakedobjects.viewmodels.ts" />
/// <reference path="nakedobjects.app.ts" />

// tested 
module NakedObjects {

    let pane1Dereg = () => {};
    let pane2Dereg = () => { };

    app.controller("Pane1HomeController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane1Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleHome($scope, routeData.pane1);

        pane1Dereg = $scope.$on("$routeUpdate", () => handlers.handleHomeSearch($scope, urlManager.getRouteData().pane1)) as () => void;
    });

    app.controller("Pane2HomeController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane2Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleHome($scope, routeData.pane2);

        pane2Dereg = $scope.$on("$routeUpdate", () => handlers.handleHomeSearch($scope, urlManager.getRouteData().pane2)) as () => void;
    });

    app.controller("Pane1ObjectController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane1Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleObject($scope, routeData.pane1);

        pane1Dereg = $scope.$on("$routeUpdate", () => handlers.handleObjectSearch($scope, urlManager.getRouteData().pane1)) as () => void;
    });

    app.controller("Pane2ObjectController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane2Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleObject($scope, routeData.pane2);

        pane2Dereg = $scope.$on("$routeUpdate", () => handlers.handleObjectSearch($scope, urlManager.getRouteData().pane2)) as () => void;
    });

    app.controller("Pane1ListController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane1Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleList($scope, routeData.pane1);

        pane1Dereg = $scope.$on("$routeUpdate", () => handlers.handleListSearch($scope, urlManager.getRouteData().pane1)) as () => void;
    });

    app.controller("Pane2ListController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane2Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleList($scope, routeData.pane2);

        pane2Dereg = $scope.$on("$routeUpdate", () => handlers.handleListSearch($scope, urlManager.getRouteData().pane2)) as () => void;
    });

    app.controller("Pane1RecentController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane1Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleRecent($scope, routeData.pane1);

        pane1Dereg = $scope.$on("$routeUpdate", () => handlers.handleRecent($scope, urlManager.getRouteData().pane1)) as () => void;
    });

    app.controller("Pane2RecentController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane2Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleRecent($scope, routeData.pane2);

        pane2Dereg = $scope.$on("$routeUpdate", () => handlers.handleRecent($scope, urlManager.getRouteData().pane2)) as () => void;
    });

    app.controller("BackgroundController", ($scope: INakedObjectsScope, handlers: IHandlers) => {
        handlers.handleBackground($scope);
    });

    app.controller("ErrorController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane1Dereg();
        pane2Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleError($scope, routeData.pane1);
    });

    app.controller("ToolBarController", ($scope: INakedObjectsScope, handlers: IHandlers) => {
        handlers.handleToolBar($scope);
    });

    app.controller("CiceroHomeController", ($scope: INakedObjectsScope, urlManager: IUrlManager, context: IContext, viewModelFactory: IViewModelFactory, commandFactory: ICommandFactory, focusManager: IFocusManager) => {
        const cvm = viewModelFactory.ciceroViewModel();
        $scope.cicero = cvm;
        cvm.viewType = ViewType.Home;
        cvm.renderHome(urlManager.getRouteData().pane1);
        focusManager.focusOn(FocusTarget.Input, 0, 1);
        cvm.executeNextChainedCommandIfAny();
    });
    app.controller("CiceroObjectController", ($scope: INakedObjectsScope, urlManager: IUrlManager, context: IContext, viewModelFactory: IViewModelFactory, commandFactory: ICommandFactory, focusManager: IFocusManager) => {
        const cvm = viewModelFactory.ciceroViewModel();
        $scope.cicero = cvm;
        cvm.viewType = ViewType.Object;
        cvm.renderObject(urlManager.getRouteData().pane1);
        focusManager.focusOn(FocusTarget.Input, 0, 1);
        cvm.executeNextChainedCommandIfAny();
    });
    app.controller("CiceroListController", ($scope: INakedObjectsScope, urlManager: IUrlManager, context: IContext, viewModelFactory: IViewModelFactory, commandFactory: ICommandFactory, focusManager: IFocusManager) => {
        const cvm = viewModelFactory.ciceroViewModel();
        $scope.cicero = cvm;
        cvm.viewType = ViewType.List;
        cvm.renderList(urlManager.getRouteData().pane1);
        focusManager.focusOn(FocusTarget.Input, 0, 1);
        cvm.executeNextChainedCommandIfAny();
    });
    app.controller("CiceroErrorController", ($scope: INakedObjectsScope, urlManager: IUrlManager, context: IContext, viewModelFactory: IViewModelFactory, commandFactory: ICommandFactory, focusManager: IFocusManager) => {
        const cvm = viewModelFactory.ciceroViewModel();
        $scope.cicero = cvm;
        //cvm.viewType = ViewType.Error;
        cvm.renderError();
        focusManager.focusOn(FocusTarget.Input, 0, 1);
        // todo is this supposed to be an assignment ?
        cvm.chainedCommands == null;
    });
}