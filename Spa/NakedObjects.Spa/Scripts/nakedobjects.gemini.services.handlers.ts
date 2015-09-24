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
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular.Gemini {

    // todo make state enum ?
    // todo improve error handling

    export interface IHandlers {
        handleBackground($scope: INakedObjectsScope): void;
        handleError($scope: INakedObjectsScope): void;
        handleToolBar($scope: INakedObjectsScope): void;
        handleObject($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleHome($scope: INakedObjectsScope, routeData : PaneRouteData): void;
        handleQuery($scope: INakedObjectsScope, routeData: PaneRouteData): void;
    }

    app.service("handlers", function($routeParams: INakedObjectsRouteParams, $location: ng.ILocationService, $q: ng.IQService, $cacheFactory: ng.ICacheFactoryService, repLoader: IRepLoader, context: IContext, viewModelFactory: IViewModelFactory, color: IColor, repHandlers: IRepHandlers, navigation: INavigation, urlManager : IUrlManager) {
        const handlers = <IHandlers>this;

        function setVersionError(error) {
            const errorRep = new ErrorRepresentation({ message: error });
            context.setError(errorRep);
            urlManager.setError();
        }

        function setError(error: ErrorRepresentation);
        function setError(error: ErrorMap);

        function setError(error: any) {
            if (error instanceof ErrorRepresentation) {
                context.setError(error);
            }
            else if (error instanceof ErrorMap) {
                let em = <ErrorMap>error;
                const errorRep = new ErrorRepresentation({ message: `unexpected error map: ${em.warningMessage}` });
                context.setError(errorRep);
            } else {
                const errorRep = new ErrorRepresentation({ message: `unexpected error: ${error.toString()}` });
                context.setError(errorRep);
            }

            urlManager.setError();
        }

        function cacheRecentlyViewed(object: DomainObjectRepresentation) {
            const cache = $cacheFactory.get("recentlyViewed");

            if (cache && object && !object.persistLink()) {
                const key = object.domainType();
                const subKey = object.selfLink().href();
                const dict = cache.get(key) || {};
                dict[subKey] = { value: new Value(object.selfLink()), name: object.title() };
                cache.put(key, dict);
            }
        }

        handlers.handleBackground = ($scope: INakedObjectsScope) => {
            $scope.backgroundColor = color.toColorFromHref($location.absUrl());

            navigation.push();

            // validate version 

            context.getVersion().then((v: VersionRepresentation) => {
                const specVersion = parseFloat(v.specVersion());
                const domainModel = v.optionalCapabilities().domainModel;

                if (specVersion < 1.1) {
                    setVersionError("Restful Objects server must support spec version 1.1 or greater for NakedObjects Gemini\r\n (8.2:specVersion)");
                }

                if (domainModel !== "simple" && domainModel !== "selectable") {
                    setVersionError(`NakedObjects Gemini requires domain metadata representation to be simple or selectable not "${domainModel}"\r\n (8.2:optionalCapabilities)`);
                }
            });
        };

        handlers.handleHome = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

            context.getMenus().
                then((menus: MenusRepresentation) => {
                    $scope.menus = viewModelFactory.menusViewModel(menus, routeData.paneId);
                    $scope.homeTemplate = homeTemplate;
                }).catch(error => {
                    setError(error);
                });

            if (routeData.menuId) {
                context.getMenu(routeData.menuId).
                    then((menu: MenuRepresentation) => {
                        $scope.actionsTemplate = actionsTemplate;
                        const actions = { actions: _.map(menu.actionMembers(), am => viewModelFactory.actionViewModel(am, routeData.paneId)) };
                        $scope.object = actions;

                        if (routeData.dialogId) {
                            $scope.dialogTemplate = dialogTemplate;
                            const action = menu.actionMember(routeData.dialogId);
                            $scope.dialog = viewModelFactory.dialogViewModel(action, routeData.paneId);
                        }
                    }).catch(error => {
                        setError(error);
                    });
            }

        };

        handlers.handleQuery = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

            var promise = routeData.objectId ? context.getQueryFromObject(routeData.paneId,  routeData.objectId, routeData.actionId, routeData.parms) :
                context.getQuery(routeData.paneId, routeData.menuId, routeData.actionId, routeData.parms);

            promise.
                then((list: ListRepresentation) => {
                    $scope.queryTemplate = routeData.state === CollectionViewState.List ? queryListTemplate : queryTableTemplate;
                    $scope.collection = viewModelFactory.collectionViewModel(list, routeData.state, routeData.paneId);
                    $scope.title = context.getLastActionFriendlyName();
                }).catch( error => {
                    setError(error);
                });
        };

        handlers.handleError = ($scope: INakedObjectsScope) => {
            var error = context.getError();
            if (error) {
                const evm = viewModelFactory.errorViewModel(error);
                $scope.error = evm;
                $scope.errorTemplate = errorTemplate;
            }
        };

        handlers.handleToolBar = ($scope: INakedObjectsScope) => {

           $scope.toolBar = viewModelFactory.toolBarViewModel($scope); 
        };

        handlers.handleObject = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

            var [dt, ...id] = routeData.objectId.split("-");

            // to ease transition 
            $scope.objectTemplate = objectViewTemplate;
            $scope.actionsTemplate = nullTemplate;
            $scope.object =  <any>{ color :  color.toColorFromType(dt)}

            context.getObject(routeData.paneId, dt, id).
                then((object: DomainObjectRepresentation) => {
                             
                    const ovm = viewModelFactory.domainObjectViewModel(object, routeData.collections, routeData.paneId);

                    $scope.object = ovm;
                  
                    if (routeData.edit || ovm.isTransient) {
                        $scope.objectTemplate = objectEditTemplate;
                        $scope.actionsTemplate = nullTemplate;                      
                    } else {
                        $scope.objectTemplate = objectViewTemplate;
                        $scope.actionsTemplate = routeData.actionsOpen ? actionsTemplate : nullTemplate;
                    }

                    $scope.collectionsTemplate = collectionsTemplate;

                    // cache
                    cacheRecentlyViewed(object);

                    if (routeData.dialogId) {
                        $scope.dialogTemplate = dialogTemplate;
                        const action = object.actionMember(routeData.dialogId);
                        $scope.dialog = viewModelFactory.dialogViewModel(action, routeData.paneId);
                    }

                }).catch(error => {
                    setError(error);
                });

        };
    });
}