/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular.Gemini {

    export interface IHandlers {
        handleBackground($scope: INakedObjectsScope): void;
        handleError($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleToolBar($scope: INakedObjectsScope): void;
        handleObject($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleHome($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleList($scope: INakedObjectsScope, routeData: PaneRouteData): void;
    }

    app.service("handlers", function ($routeParams: ng.route.IRouteParamsService, $location: ng.ILocationService, $q: ng.IQService, $cacheFactory: ng.ICacheFactoryService, repLoader: IRepLoader, context: IContext, viewModelFactory: IViewModelFactory, color: IColor, navigation: INavigation, urlManager: IUrlManager, focusManager: IFocusManager) {
        const handlers = <IHandlers>this;

        const perPaneListViews = [, new ListViewModel(color, context, viewModelFactory, urlManager, focusManager, $q),
            new ListViewModel(color, context, viewModelFactory, urlManager, focusManager, $q)];

        const perPaneObjectViews = [, new DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager),
            new DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager)];

        const perPaneDialogViews = [, new DialogViewModel(color, context, viewModelFactory, urlManager, focusManager),
                                      new DialogViewModel(color, context, viewModelFactory, urlManager, focusManager)];

        const perPaneMenusViews = [, new MenusViewModel(viewModelFactory),
                                    new MenusViewModel(viewModelFactory)];

        function setVersionError(error : string) {
            context.setError(new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.SoftwareError,  error));
            urlManager.setError(ErrorCategory.ClientError, ClientErrorCode.SoftwareError);
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

        class DeReg {

            private deRegers : (() =>void)[];

            add(newF: () => void) {
                this.deRegers.push(newF);
            }
            deReg() {   
                _.forEach(this.deRegers, d => d());
                this.deRegers = [];
            }
        }

        const deRegDialog = [, new DeReg(), new DeReg()];
        const deRegObject = [, new DeReg(), new DeReg()];

        function setDialog($scope: INakedObjectsScope, action: ActionMember | ActionViewModel, routeData: PaneRouteData) {
            deRegDialog[routeData.paneId].deReg();

            $scope.dialogTemplate = dialogTemplate;
            const dialogViewModel = perPaneDialogViews[routeData.paneId];
            const actionViewModel = action instanceof ActionMember ? viewModelFactory.actionViewModel(action, dialogViewModel, routeData) : action as ActionViewModel;

            dialogViewModel.reset(actionViewModel, routeData);
            $scope.dialog = dialogViewModel; 

            deRegDialog[routeData.paneId].add($scope.$on("$locationChangeStart", dialogViewModel.setParms) as () => void);
            deRegDialog[routeData.paneId].add($scope.$watch(() => $location.search(), dialogViewModel.setParms, true) as () => void);
        }

        let versionValidated = false;


        handlers.handleBackground = ($scope: INakedObjectsScope) => {
            $scope.backgroundColor = color.toColorFromHref($location.absUrl());

            navigation.push();

            // Just do once - cached but still pointless repeating each page refresh

            if (versionValidated) {
                return;
            }

            context.getVersion().then((v: VersionRepresentation) => {
                const specVersion = parseFloat(v.specVersion());
                const domainModel = v.optionalCapabilities().domainModel;

                if (specVersion < 1.1) {
                    setVersionError("Restful Objects server must support spec version 1.1 or greater for NakedObjects Gemini\r\n (8.2:specVersion)");
                }
                else if (domainModel !== "simple" && domainModel !== "selectable") {
                    setVersionError(`NakedObjects Gemini requires domain metadata representation to be simple or selectable not "${domainModel}"\r\n (8.2:optionalCapabilities)`);
                } else {
                    versionValidated = true;
                }
            });
        };

        handlers.handleHome = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

            context.clearWarnings();
            context.clearMessages();

            context.getMenus().
                then((menus: MenusRepresentation) => {
                    $scope.menus = perPaneMenusViews[routeData.paneId].reset(menus, routeData);
                    $scope.homeTemplate = homeTemplate;

                    if (routeData.menuId) {
                        context.getMenu(routeData.menuId).
                            then((menu: MenuRepresentation) => {
                                $scope.actionsTemplate = actionsTemplate;
                                $scope.menu = viewModelFactory.menuViewModel(menu, routeData);

                                const focusTarget = routeData.dialogId ? FocusTarget.Dialog : FocusTarget.SubAction;

                                if (routeData.dialogId) {                               
                                    const action = menu.actionMember(routeData.dialogId);
                                    setDialog($scope, action, routeData);
                                }

                                focusManager.focusOn(focusTarget, 0, routeData.paneId);
                            }).catch((reject : ErrorWrapper) => {
                                context.handleWrappedError(reject, null, () => {}, () => {}, ()=> false);
                            });
                    } else {
                        focusManager.focusOn(FocusTarget.Menu, 0, routeData.paneId);
                    }
                }).catch((reject: ErrorWrapper) => {
                    context.handleWrappedError(reject, null, () => { }, () => { }, () => false);
                });
        };       

        handlers.handleList = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

            const cachedList = context.getCachedList(routeData.paneId, routeData.page, routeData.pageSize);

            const getActionExtensions = routeData.objectId ?
                () => context.getActionExtensionsFromObject(routeData.paneId, routeData.objectId, routeData.actionId) :
                () => context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);
            

            if (cachedList) {
                $scope.listTemplate = routeData.state === CollectionViewState.List ? listTemplate : listAsTableTemplate;
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

                focusManager.focusOn(focusTarget, 0, routeData.paneId);
                getActionExtensions().then((ext: Extensions) => $scope.title = ext.friendlyName());
            } else {
                $scope.listTemplate = listPlaceholderTemplate;
                $scope.collectionPlaceholder = viewModelFactory.listPlaceholderViewModel(routeData);
                getActionExtensions().then((ext: Extensions) => $scope.title = ext.friendlyName());
                focusManager.focusOn(FocusTarget.Action, 0, routeData.paneId);       
            }
        };

        handlers.handleError = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {
            const evm = viewModelFactory.errorViewModel(context.getError());
            $scope.error = evm;

            if (evm.isConcurrencyError) {
                $scope.errorTemplate = concurrencyTemplate;
            }
            else if (routeData.errorCategory === ErrorCategory.HttpClientError) {
                $scope.errorTemplate = httpErrorTemplate;
            }
            else if (routeData.errorCategory === ErrorCategory.ClientError) {
                $scope.errorTemplate = errorTemplate;
            }
            else if (routeData.errorCategory === ErrorCategory.HttpServerError) {
                $scope.errorTemplate = errorTemplate;
            }
        };

        handlers.handleToolBar = ($scope: INakedObjectsScope) => {
            $scope.toolBar = viewModelFactory.toolBarViewModel();
        };     

        handlers.handleObject = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

            const [dt, ...id] = routeData.objectId.split("-");

            // to ease transition 
            $scope.objectTemplate = blankTemplate;
            $scope.actionsTemplate = nullTemplate;
            $scope.backgroundColor =  color.toColorFromType(dt); 

            deRegObject[routeData.paneId].deReg();

            context.getObject(routeData.paneId, dt, id, routeData.interactionMode === InteractionMode.Transient).
                then((object: DomainObjectRepresentation) => {
                    
                    const ovm = perPaneObjectViews[routeData.paneId].reset(object, routeData);

                    $scope.object = ovm;

                    if (routeData.interactionMode === InteractionMode.Form) {
                        $scope.objectTemplate = formTemplate;
                        $scope.actionsTemplate = formActionsTemplate;
                    }
                    else if (routeData.interactionMode === InteractionMode.View) {
                        $scope.objectTemplate = objectViewTemplate;
                        $scope.actionsTemplate = routeData.actionsOpen ? actionsTemplate : nullTemplate;
                    } else {
                        $scope.objectTemplate = objectEditTemplate;
                        $scope.actionsTemplate = nullTemplate;
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
                    } else if (ovm.isInEdit) {
                        focusTarget = FocusTarget.Property;
                    } else {
                        focusTarget = FocusTarget.ObjectTitle;
                    }

                    focusManager.focusOn(focusTarget, 0, routeData.paneId);

                    deRegObject[routeData.paneId].add($scope.$on("$locationChangeStart", ovm.setProperties) as () => void);
                    deRegObject[routeData.paneId].add($scope.$watch(() => $location.search(), ovm.setProperties, true) as () => void);
                    deRegObject[routeData.paneId].add($scope.$on("pane-swap", ovm.setProperties) as () => void);

                }).catch((reject : ErrorWrapper) => {
                 
                    const handler =  (cc: ClientErrorCode) => {
                        if (cc === ClientErrorCode.ExpiredTransient) {
                            $scope.objectTemplate = expiredTransientTemplate;
                            return true;
                        }
                        return false;
                    }

                    context.handleWrappedError(reject, null, () => { }, () => { }, handler);                                       
                });

        };
    });
}