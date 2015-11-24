/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular.Gemini {

    // todo improve error handling

    export interface IHandlers {
        handleBackground($scope: INakedObjectsScope): void;
        handleError($scope: INakedObjectsScope): void;
        handleToolBar($scope: INakedObjectsScope): void;
        handleObject($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleHome($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleList($scope: INakedObjectsScope, routeData: PaneRouteData): void;
    }

    app.service("handlers", function($routeParams: INakedObjectsRouteParams, $location: ng.ILocationService, $q: ng.IQService, $cacheFactory: ng.ICacheFactoryService, repLoader: IRepLoader, context: IContext, viewModelFactory: IViewModelFactory, color: IColor, navigation: INavigation, urlManager: IUrlManager, focusManager: IFocusManager) {
        const handlers = <IHandlers>this;

        function setVersionError(error) {
            const errorRep = ErrorRepresentation.create(error);
            context.setError(errorRep);
            urlManager.setError();
        }

        function setError(error: ErrorRepresentation);
        function setError(error: ErrorMap);
        function setError(error: any) {
            if (error instanceof ErrorRepresentation) {
                context.setError(error);
            } else if (error instanceof ErrorMap) {
                const em = <ErrorMap>error;
                const errorRep = ErrorRepresentation.create(`unexpected error map: ${em.warningMessage}`);
                context.setError(errorRep);
            } else {
                const errorRep = ErrorRepresentation.create(`unexpected error: ${error.toString()}`);
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

                    if (routeData.menuId) {
                        context.getMenu(routeData.menuId).
                            then((menu: MenuRepresentation) => {
                                $scope.actionsTemplate = actionsTemplate;
                                const actions = { actions: _.map(menu.actionMembers(), am => viewModelFactory.actionViewModel(am, routeData.paneId)) };
                                $scope.object = actions;

                                const focusTarget = routeData.dialogId ? FocusTarget.Dialog : FocusTarget.FirstSubAction;

                                if (routeData.dialogId) {
                                    $scope.dialogTemplate = dialogTemplate;
                                    const action = menu.actionMember(routeData.dialogId);
                                    $scope.dialog = viewModelFactory.dialogViewModel($scope, action, routeData.parms, routeData.paneId);
                                }

                                focusManager.focusOn(focusTarget, urlManager.currentpane());
                            }).catch(error => {
                                setError(error);
                            });
                    } else {
                        focusManager.focusOn(FocusTarget.FirstMenu, urlManager.currentpane());
                    }
                }).catch(error => {
                    setError(error);
                });
        };

        handlers.handleList = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

            const cachedList = context.getCachedList(routeData.paneId, routeData.page, routeData.pageSize);

            const recreate = (page: number, pageSize: number) => {
                return routeData.objectId ?
                    context.getListFromObject(routeData.paneId, routeData.objectId, routeData.actionId, routeData.parms, page, pageSize) :
                    context.getListFromMenu(routeData.paneId, routeData.menuId, routeData.actionId, routeData.parms, page, pageSize);
            }

            const getFriendlyName = routeData.objectId ?
                () => context.getActionFriendlyNameFromObject(routeData.paneId, routeData.objectId, routeData.actionId) :
                () => context.getActionFriendlyNameFromMenu(routeData.menuId, routeData.actionId);

            const pageOrRecreate = (newPage: number, newPageSize, newState? : CollectionViewState) => {
                recreate(newPage, newPageSize).then((list: ListRepresentation) => {

                    const state = newState || routeData.state;
                    
                    $scope.listTemplate = state === CollectionViewState.List ? ListTemplate : ListAsTableTemplate;
                    $scope.collection = viewModelFactory.collectionViewModel($scope, list, state, routeData.paneId, pageOrRecreate);

                    getFriendlyName().then((name: string) => $scope.title = name);
                
                    focusManager.focusOn(FocusTarget.FirstListItem, urlManager.currentpane());       
                    urlManager.setListPaging(routeData.paneId, newPage, newPageSize, state);           
                }).catch(error => {
                    setError(error);
                });
            }

            getFriendlyName().then((name: string) => $scope.title = name);

            if (cachedList) {
                $scope.listTemplate = routeData.state === CollectionViewState.List ? ListTemplate : ListAsTableTemplate;
                $scope.collection = viewModelFactory.collectionViewModel($scope, cachedList, routeData.state, routeData.paneId, pageOrRecreate);
                focusManager.focusOn(FocusTarget.FirstListItem, urlManager.currentpane());
            } else {
                $scope.listTemplate = ListPlaceholderTemplate;
                $scope.collectionPlaceholder = viewModelFactory.collectionPlaceholderViewModel(routeData.page, () => pageOrRecreate(routeData.page, routeData.pageSize));
                focusManager.focusOn(FocusTarget.FirstAction, urlManager.currentpane());       
            }
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
            $scope.objectTemplate = blankTemplate;
            $scope.actionsTemplate = nullTemplate;
            $scope.object = <any>{ color: color.toColorFromType(dt) }; 
            
            // only pass previous values if editing 
            const previousValues: _.Dictionary<Value> = routeData.edit ? routeData.props : {};

            context.getObject(routeData.paneId, dt, id).
                then((object: DomainObjectRepresentation) => {

                    const ovm = viewModelFactory.domainObjectViewModel($scope, object, routeData.collections, previousValues, routeData.edit, routeData.paneId);

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

                    let focusTarget: FocusTarget;

                    if (routeData.dialogId) {
                        $scope.dialogTemplate = dialogTemplate;
                        const action = object.actionMember(routeData.dialogId);
                        $scope.dialog = viewModelFactory.dialogViewModel($scope, action, routeData.parms, routeData.paneId, ovm);
                        focusTarget = FocusTarget.Dialog;
                    } else if (routeData.actionsOpen) {
                        focusTarget = FocusTarget.FirstSubAction;
                    } else if (routeData.edit || ovm.isTransient) {
                        focusTarget = FocusTarget.FirstProperty;
                    } else {
                        focusTarget = FocusTarget.ObjectTitle;
                    }

                    focusManager.focusOn(focusTarget, urlManager.currentpane());

                }).catch(error => {
                    setError(error);
                });

        };
    });
}