/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />

// tested 
module NakedObjects.Angular.Gemini {

    app.controller("Pane1HomeController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        const routeData = urlManager.getRouteData();
        handlers.handleHome($scope, routeData.pane1);
    });

    app.controller("Pane2HomeController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        const routeData = urlManager.getRouteData();
        handlers.handleHome($scope, routeData.pane2);
    });

    app.controller("Pane1ObjectController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        const routeData = urlManager.getRouteData();
        handlers.handleObject($scope, routeData.pane1);
    });

    app.controller("Pane2ObjectController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        const routeData = urlManager.getRouteData();
        handlers.handleObject($scope, routeData.pane2);
    });

    app.controller("Pane1ListController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        const routeData = urlManager.getRouteData();
        handlers.handleList($scope, routeData.pane1);
    });

    app.controller("Pane2ListController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager) => {
        const routeData = urlManager.getRouteData();
        handlers.handleList($scope, routeData.pane2);
    });

    app.controller("BackgroundController", ($scope: INakedObjectsScope, handlers: IHandlers) => {
        handlers.handleBackground($scope);
    });

    app.controller("ErrorController", ($scope: INakedObjectsScope, handlers: IHandlers) => {
        handlers.handleError($scope);
    });

    app.controller("ToolBarController", ($scope: INakedObjectsScope, handlers: IHandlers) => {
        handlers.handleToolBar($scope);
    });

    app.controller("CiceroController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager, context: IContext, viewModelFactory: IViewModelFactory, commandFactory: ICommandFactory, focusManager: IFocusManager) => {
        const cvm = viewModelFactory.ciceroViewModel();
        $scope.cicero = cvm;
        cvm.setOutputToSummaryOfRepresentation(urlManager.getRouteData().pane1);
        focusManager.focusOn(FocusTarget.FirstInput, 1);
    });
}