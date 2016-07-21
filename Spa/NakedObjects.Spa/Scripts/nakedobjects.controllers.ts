/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.services.handlers.ts" />
/// <reference path="nakedobjects.viewmodels.ts" />

namespace NakedObjects {

    let pane1Dereg = () => {};
    let pane2Dereg = () => {};

    // Possible for race condition where $routeUpdate is called before we deregister the handler when location is changing
    // eg list to object. This means we could call the wrong search handler. This checks the page type matches the handler 
    // before calling the handler. If it's wrong just ignore and the handler will be deregistered by the new controller.
    function baseGuard(paneId: number, check: (paneId?: number) => boolean, handler: () => void) {      
        return () => {
            if (check(paneId)) {
                handler();
            }
        }
    }

    app.controller("Pane1HomeController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane1Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleHome($scope, routeData.pane1);
        const guard = _.partial(baseGuard, 1, urlManager.isHome);

        pane1Dereg = $scope.$on("$routeUpdate", guard(() => handlers.handleHomeSearch($scope, urlManager.getRouteData().pane1)));
    });

    app.controller("Pane2HomeController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane2Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleHome($scope, routeData.pane2);
        const guard = _.partial(baseGuard, 2, urlManager.isHome);

        pane2Dereg = $scope.$on("$routeUpdate", guard(() => handlers.handleHomeSearch($scope, urlManager.getRouteData().pane2)));
    });

    app.controller("Pane1ObjectController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane1Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleObject($scope, routeData.pane1);
        const guard = _.partial(baseGuard, 1, urlManager.isObject);

        pane1Dereg = $scope.$on("$routeUpdate", guard(() => handlers.handleObjectSearch($scope, urlManager.getRouteData().pane1)));
    });

    app.controller("Pane2ObjectController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane2Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleObject($scope, routeData.pane2);
        const guard = _.partial(baseGuard, 2, urlManager.isObject);

        pane2Dereg = $scope.$on("$routeUpdate", guard(() => handlers.handleObjectSearch($scope, urlManager.getRouteData().pane2)));
    });

    app.controller("Pane1ListController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane1Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleList($scope, routeData.pane1);
        const guard = _.partial(baseGuard, 1, urlManager.isList);

        pane1Dereg = $scope.$on("$routeUpdate", guard(() => handlers.handleListSearch($scope, urlManager.getRouteData().pane1)));
    });

    app.controller("Pane2ListController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane2Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleList($scope, routeData.pane2);
        const guard = _.partial(baseGuard, 2, urlManager.isList);

        pane2Dereg = $scope.$on("$routeUpdate", guard(() => handlers.handleListSearch($scope, urlManager.getRouteData().pane2)));
    });

    app.controller("Pane1RecentController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane1Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleRecent($scope, routeData.pane1);
        const guard = _.partial(baseGuard, 1, urlManager.isRecent);

        pane1Dereg = $scope.$on("$routeUpdate", guard(() => handlers.handleRecent($scope, urlManager.getRouteData().pane1)));
    });

    app.controller("Pane2RecentController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane2Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleRecent($scope, routeData.pane2);
        const guard = _.partial(baseGuard, 2, urlManager.isRecent);

        pane2Dereg = $scope.$on("$routeUpdate", guard(() => handlers.handleRecent($scope, urlManager.getRouteData().pane2)));
    });

    app.controller("Pane1AttachmentController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane1Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleAttachment($scope, routeData.pane1);
        const guard = _.partial(baseGuard, 1, urlManager.isAttachment);

        pane1Dereg = $scope.$on("$routeUpdate", guard(() => handlers.handleAttachment($scope, urlManager.getRouteData().pane1)));
    });

    app.controller("Pane2AttachmentController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane2Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleAttachment($scope, routeData.pane2);
        const guard = _.partial(baseGuard, 2, urlManager.isAttachment);

        pane2Dereg = $scope.$on("$routeUpdate", guard(() => handlers.handleAttachment($scope, urlManager.getRouteData().pane2)));
    });

    app.controller("ApplicationPropertiesController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        pane1Dereg();
        pane2Dereg();

        const routeData = urlManager.getRouteData();
        handlers.handleApplicationProperties($scope, routeData.pane1);
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
        cvm.chainedCommands = null;
    });
}