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

    app.service("handlers", function ($routeParams: INakedObjectsRouteParams, $location: ng.ILocationService, $q: ng.IQService, $cacheFactory: ng.ICacheFactoryService, repLoader: IRepLoader, context: IContext, viewModelFactory: IViewModelFactory, color: IColor, navigation: INavigation, urlManager: IUrlManager, focusManager: IFocusManager, $timeout: ng.ITimeoutService) {
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
                error = error || "unknown";
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

        function setDialog($scope: INakedObjectsScope, action: ActionMember | ActionViewModel, routeData: PaneRouteData) {
            $scope.dialogTemplate = dialogTemplate;          
            const actionViewModel = action instanceof ActionMember ? viewModelFactory.actionViewModel( action, routeData) : action as ActionViewModel;    
            $scope.dialog = viewModelFactory.dialogViewModel($scope, actionViewModel, routeData);
        }


        handlers.handleBackground = ($scope: INakedObjectsScope) => {
            $scope.backgroundColor = color.toColorFromHref($location.absUrl());

            navigation.push();

            // validate version 

            // todo just do once - cached but still pointless repeating each page refresh
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
                                const actions = { actions: _.map(menu.actionMembers(), am => viewModelFactory.actionViewModel( am, routeData)) };
                                $scope.object = actions;

                                const focusTarget = routeData.dialogId ? FocusTarget.Dialog : FocusTarget.SubAction;

                                if (routeData.dialogId) {                               
                                    const action = menu.actionMember(routeData.dialogId);
                                    setDialog($scope, action, routeData);
                                }

                                focusManager.focusOn(focusTarget, 0, urlManager.currentpane());
                            }).catch(error => {
                                setError(error);
                            });
                    } else {
                        focusManager.focusOn(FocusTarget.Menu, 0, urlManager.currentpane());
                    }
                }).catch(error => {
                    setError(error);
                });
        };

        const perPaneListViews = [, new ListViewModel(color, context, viewModelFactory, urlManager, focusManager),
                                    new ListViewModel(color, context, viewModelFactory, urlManager, focusManager)];

        handlers.handleList = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

            const cachedList = context.getCachedList(routeData.paneId, routeData.page, routeData.pageSize);

            const getFriendlyName = routeData.objectId ?
                () => context.getActionFriendlyNameFromObject(routeData.paneId, routeData.objectId, routeData.actionId) :
                () => context.getActionFriendlyNameFromMenu(routeData.menuId, routeData.actionId);
       
            if (cachedList) {
                $scope.listTemplate = routeData.state === CollectionViewState.List ? ListTemplate : ListAsTableTemplate;
                const collectionViewModel = perPaneListViews[routeData.paneId];
                collectionViewModel.reset(cachedList, routeData);
                $scope.collection = collectionViewModel;
                $scope.actionsTemplate = routeData.actionsOpen ? actionsTemplate : nullTemplate;
                let focusTarget = routeData.actionsOpen ? FocusTarget.SubAction : FocusTarget.ListItem;

                if (routeData.dialogId) {
                    const actionViewModel = _.find(collectionViewModel.actions, a => a.actionRep.actionId() === routeData.dialogId);
                    setDialog($scope, actionViewModel, routeData);
                    focusTarget = FocusTarget.Dialog;
                }

                focusManager.focusOn(focusTarget, 0, urlManager.currentpane());
                getFriendlyName().then((name: string) => $scope.title = name);
            } else {
                $scope.listTemplate = ListPlaceholderTemplate;
                $scope.collectionPlaceholder = viewModelFactory.collectionPlaceholderViewModel(routeData);
                getFriendlyName().then((name: string) => $scope.title = name);
                focusManager.focusOn(FocusTarget.Action, 0, urlManager.currentpane());       
            }
        };

        handlers.handleError = ($scope: INakedObjectsScope) => {
            const  error = context.getError();
            if (error) {
                const evm = viewModelFactory.errorViewModel(error);
                $scope.error = evm;
                $scope.errorTemplate = errorTemplate;
            }
        };

        handlers.handleToolBar = ($scope: INakedObjectsScope) => {
            $scope.toolBar = viewModelFactory.toolBarViewModel();
        };

        const perPaneObjectViews = [, new DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager),
                                      new DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager)];

        handlers.handleObject = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

            const [dt, ...id] = routeData.objectId.split("-");

            // to ease transition 
            $scope.objectTemplate = blankTemplate;
            $scope.actionsTemplate = nullTemplate;
            $scope.object = { color: color.toColorFromType(dt) }; 

            context.getObject(routeData.paneId, dt, id).
                then((object: DomainObjectRepresentation) => {

                    //const ovm = viewModelFactory.domainObjectViewModel($scope, object, routeData);
                    const ovm = perPaneObjectViews[routeData.paneId].reset(object, routeData);

                    $scope.object = ovm;

                    // todo can the object be transient ?
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
                        const action = object.actionMember(routeData.dialogId);
                        setDialog($scope, action, routeData);
                        focusTarget = FocusTarget.Dialog;
                    } else if (routeData.actionsOpen) {
                        focusTarget = FocusTarget.SubAction;
                    } else if (routeData.edit || ovm.isTransient) {
                        focusTarget = FocusTarget.Property;
                    } else {
                        focusTarget = FocusTarget.ObjectTitle;
                    }

                    focusManager.focusOn(focusTarget, 0, urlManager.currentpane());

                }).catch(error => {
                    setError(error);
                });

        };
    });
}