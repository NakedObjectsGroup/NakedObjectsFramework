//Copyright 2014 Stef Cascarini, Dan Haywood, Richard Pawson
//Licensed under the Apache License, Version 2.0(the
//"License"); you may not use this file except in compliance
//with the License.You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing,
//software distributed under the License is distributed on an
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//KIND, either express or implied.See the License for the
//specific language governing permissions and limitations
//under the License.

/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />

// tested 
module NakedObjects.Angular.Gemini {

    app.controller("Pane1HomeController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager : IUrlManager) => {
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

    //Cicero
    app.controller("CiceroController", ($scope: INakedObjectsScope, handlers: IHandlers, urlManager: IUrlManager, context: IContext, viewModelFactory: IViewModelFactory) => {
        const routeData = urlManager.getRouteData();        
        const pane = routeData.pane1;

        if (pane.objectId) {
            var [dt, ...id] = pane.objectId.split("-");

            context.getObject(pane.paneId, dt, id).
                then((object: DomainObjectRepresentation) => {

                    const ovm = viewModelFactory.domainObjectViewModel(object, pane.collections, pane.paneId);
                    const cvm = viewModelFactory.ciceroViewModel(ovm);
                    $scope.cicero = cvm;

                    // cache
                    //cacheRecentlyViewed(object);

                }).catch(error => {
                    //setError(error);
                });
        } else { //home
            const cvm = viewModelFactory.ciceroViewModel(null);
            $scope.cicero = cvm;
        }
    });
}