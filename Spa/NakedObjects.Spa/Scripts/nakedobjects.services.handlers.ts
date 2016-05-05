/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.services.viewmodelfactory.ts" />

module NakedObjects {
    import ErrorWrapper = Models.ErrorWrapper;
    import ErrorCategory = Models.ErrorCategory;
    import ClientErrorCode = Models.ClientErrorCode;
    import DomainObjectRepresentation = Models.DomainObjectRepresentation;
    import ActionMember = Models.ActionMember;
    import VersionRepresentation = Models.VersionRepresentation;
    import MenusRepresentation = Models.MenusRepresentation;
    import MenuRepresentation = Models.MenuRepresentation;
    import Extensions = Models.Extensions;
    import ActionRepresentation = Models.ActionRepresentation;
    import IInvokableAction = Models.IInvokableAction;
    import ObjectIdWrapper = Models.ObjectIdWrapper;

    export interface IHandlers {
        handleBackground($scope: INakedObjectsScope): void;
        handleError($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleToolBar($scope: INakedObjectsScope): void;
        handleObject($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleObjectSearch($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleHome($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleHomeSearch($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleList($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleListSearch($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleRecent($scope: INakedObjectsScope, routeData: PaneRouteData): void;
    }

    app.service("handlers",
        function($routeParams: ng.route.IRouteParamsService,
            $location: ng.ILocationService,
            $q: ng.IQService,
            $cacheFactory: ng.ICacheFactoryService,
            repLoader: IRepLoader,
            context: IContext,
            viewModelFactory: IViewModelFactory,
            color: IColor,
            navigation: INavigation,
            urlManager: IUrlManager,
            focusManager: IFocusManager) {
            const handlers = <IHandlers>this;

            const perPaneListViews = [
                , new ListViewModel(color, context, viewModelFactory, urlManager, focusManager, $q),
                new ListViewModel(color, context, viewModelFactory, urlManager, focusManager, $q)
            ];

            const perPaneObjectViews = [
                , new DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager, $q),
                new DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager, $q)
            ];

            const perPaneDialogViews = [
                , new DialogViewModel(color, context, viewModelFactory, urlManager, focusManager),
                new DialogViewModel(color, context, viewModelFactory, urlManager, focusManager)
            ];

            const perPaneMenusViews = [
                , new MenusViewModel(viewModelFactory),
                new MenusViewModel(viewModelFactory)
            ];

            function setVersionError(error: string) {
                context.setError(new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.SoftwareError, error));
                urlManager.setError(ErrorCategory.ClientError, ClientErrorCode.SoftwareError);
            }


            class DeReg {

                private deRegers: (() => void)[];

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

            function setDialog($scope: INakedObjectsScope,
                action: ActionMember | ActionRepresentation | ActionViewModel,
                routeData: PaneRouteData) {
                deRegDialog[routeData.paneId].deReg();

                $scope.dialogTemplate = dialogTemplate;
                const dialogViewModel = perPaneDialogViews[routeData.paneId];
                const isAlreadyViewModel = action instanceof ActionViewModel;
                const actionViewModel = !isAlreadyViewModel
                    ? viewModelFactory.actionViewModel(action as ActionMember | ActionRepresentation,
                        dialogViewModel,
                        routeData)
                    : action as ActionViewModel;

                dialogViewModel.reset(actionViewModel, routeData);
                $scope.dialog = dialogViewModel;

                deRegDialog[routeData.paneId].add($scope
                    .$on("$locationChangeStart", dialogViewModel.setParms) as () => void);
                deRegDialog[routeData.paneId].add($scope
                    .$watch(() => $location.search(), dialogViewModel.setParms, true) as () => void);

                dialogViewModel.deregister = () => deRegDialog[routeData.paneId].deReg();
            }

            let versionValidated = false;

            handlers.handleBackground = ($scope: INakedObjectsScope) => {
                color.toColorNumberFromHref($location.absUrl())
                    .then((c: number) => {
                        $scope.backgroundColor = `${objectColor}${c}`;
                    });

                navigation.push();

                // Just do once - cached but still pointless repeating each page refresh

                if (versionValidated) {
                    return;
                }

                context.getVersion()
                    .then((v: VersionRepresentation) => {
                        const specVersion = parseFloat(v.specVersion());
                        const domainModel = v.optionalCapabilities().domainModel;

                        if (specVersion < 1.1) {
                            setVersionError("Restful Objects server must support spec version 1.1 or greater for NakedObjects Gemini\r\n (8.2:specVersion)");
                        } else if (domainModel !== "simple" && domainModel !== "selectable") {
                            setVersionError(`NakedObjects Gemini requires domain metadata representation to be simple or selectable not "${domainModel}"\r\n (8.2:optionalCapabilities)`);
                        } else {
                            versionValidated = true;
                        }
                    });
            };

            function setNewMenu($scope: INakedObjectsScope, newMenuId: string, routeData: PaneRouteData) {
                context.getMenu(newMenuId)
                    .then((menu: MenuRepresentation) => {
                        $scope.actionsTemplate = actionsTemplate;
                        $scope.menu = viewModelFactory.menuViewModel(menu, routeData);
                        setNewDialog($scope, menu, routeData.dialogId, routeData);
                    })
                    .catch((reject: ErrorWrapper) => {
                        context.handleWrappedError(reject, null, () => {}, () => {});
                    });
            }

            function setNewDialog($scope: INakedObjectsScope,
                menu: MenuRepresentation,
                newDialogId: string,
                routeData: PaneRouteData) {
                if (newDialogId) {
                    const action = menu.actionMember(routeData.dialogId);
                    context.getInvokableAction(action)
                        .then(details => {
                            setDialog($scope, details, routeData);
                            focusManager.focusOn(FocusTarget.Dialog, 0, routeData.paneId);
                        });
                    return;
                }
                $scope.dialogTemplate = null;
                $scope.dialog = null;
                focusManager.focusOn(FocusTarget.SubAction, 0, routeData.paneId);
            }


            handlers.handleHomeSearch = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

                context.clearWarnings();
                context.clearMessages();

                if (routeData.menuId) {

                    const currentMenu = $scope.menu;
                    const currentMenuId = currentMenu ? currentMenu.id : null;
                    const newMenuId = routeData.menuId;

                    const currentDialog = $scope.dialog;
                    const currentDialogId = currentDialog ? currentDialog.id : null;
                    const newDialogId = routeData.dialogId;

                    if (currentMenuId !== newMenuId) {
                        // menu changed set new menu and if necessary new dialog
                        setNewMenu($scope, newMenuId, routeData);
                    } else if (currentDialogId !== newDialogId) {
                        // dialog changed set new dialog only 
                        setNewDialog($scope, currentMenu.menuRep, newDialogId, routeData);
                    }
                } else {
                    $scope.actionsTemplate = null;
                    $scope.menu = null;
                    focusManager.focusOn(FocusTarget.Menu, 0, routeData.paneId);
                }
            };


            handlers.handleHome = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

                $scope.homeTemplate = homePlaceholderTemplate;

                context.getMenus()
                    .then((menus: MenusRepresentation) => {
                        $scope.menus = perPaneMenusViews[routeData.paneId].reset(menus, routeData);
                        $scope.homeTemplate = homeTemplate;

                        handlers.handleHomeSearch($scope, routeData);
                    })
                    .catch((reject: ErrorWrapper) => {
                        context.handleWrappedError(reject, null, () => {}, () => {});
                    });
            };

            function getActionExtensions(routeData: PaneRouteData) {
                return routeData.objectId ?
                    context.getActionExtensionsFromObject(routeData.paneId, ObjectIdWrapper.fromObjectId(routeData.objectId), routeData.actionId) :
                    context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);
            }



        handlers.handleListSearch = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

            const listKey = urlManager.getListCacheIndex(routeData.paneId, routeData.page, routeData.pageSize);
            const collectionViewModel = $scope.collection;

            if (listKey !== collectionViewModel.id) {
                handlers.handleList($scope, routeData);
            } else {
                $scope.listTemplate = routeData.state === CollectionViewState.List ? listTemplate : listAsTableTemplate;
                collectionViewModel.refresh(routeData);
                $scope.actionsTemplate = routeData.actionsOpen ? actionsTemplate : nullTemplate;

                let focusTarget = routeData.actionsOpen ? FocusTarget.SubAction : FocusTarget.ListItem;

                if (routeData.dialogId) {
                    const actionViewModel = _.find(collectionViewModel.actions,
                        a => a.actionRep.actionId() === routeData.dialogId);

                    context.getInvokableAction(actionViewModel.actionRep)
                        .then((details: IInvokableAction) => {
                            actionViewModel.makeInvokable(details);
                            setDialog($scope, actionViewModel, routeData);
                        });

                    focusTarget = FocusTarget.Dialog;
                } else {
                    $scope.dialogTemplate = null;
                }

                focusManager.focusOn(focusTarget, 0, routeData.paneId);
            }        
        };


        handlers.handleList = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

            const cachedList = context.getCachedList(routeData.paneId, routeData.page, routeData.pageSize);

            if (cachedList) {
                $scope.listTemplate = routeData.state === CollectionViewState.List ? listTemplate : listAsTableTemplate;
                const collectionViewModel = perPaneListViews[routeData.paneId];
                collectionViewModel.reset(cachedList, routeData);
                $scope.collection = collectionViewModel;
                $scope.actionsTemplate = routeData.actionsOpen ? actionsTemplate : nullTemplate;
                let focusTarget = routeData.actionsOpen ? FocusTarget.SubAction : FocusTarget.ListItem;

                if (routeData.dialogId) {
                    const actionViewModel = _.find(collectionViewModel.actions,  a => a.actionRep.actionId() === routeData.dialogId);

                    context.getInvokableAction(actionViewModel.actionRep)
                        .then((details: IInvokableAction) => {
                            actionViewModel.makeInvokable(details);
                            setDialog($scope, actionViewModel, routeData);
                        });

                    focusTarget = FocusTarget.Dialog;
                } else {
                    $scope.dialogTemplate = null;
                }

                focusManager.focusOn(focusTarget, 0, routeData.paneId);
                getActionExtensions(routeData).then((ext: Extensions) => $scope.title = ext.friendlyName());
            } else {
                $scope.listTemplate = listPlaceholderTemplate;
                $scope.collectionPlaceholder = viewModelFactory.listPlaceholderViewModel(routeData);
                getActionExtensions(routeData).then((ext: Extensions) => $scope.title = ext.friendlyName());
                focusManager.focusOn(FocusTarget.Action, 0, routeData.paneId);
            }
        };

        handlers.handleRecent = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {
            context.clearWarnings();
            context.clearMessages();
            $scope.recentTemplate = recentTemplate;
            $scope.recent = viewModelFactory.recentItemsViewModel(routeData.paneId);
        };

        handlers.handleError = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {
            const evm = viewModelFactory.errorViewModel(context.getError());
            $scope.error = evm;

            if (evm.isConcurrencyError) {
                $scope.errorTemplate = concurrencyTemplate;
            } else {
                $scope.errorTemplate = errorTemplate;
            } 
        };

        handlers.handleToolBar = ($scope: INakedObjectsScope) => {
            $scope.toolBar = viewModelFactory.toolBarViewModel();
        };

        function handleNewObjectSearch ($scope: INakedObjectsScope, routeData: PaneRouteData)  {

            const ovm = $scope.object;

            let newObjectTemplate: string;
            let newActionsTemplate: string;

            if (routeData.interactionMode === InteractionMode.Form) {
                newObjectTemplate = formTemplate;
                newActionsTemplate = formActionsTemplate;
            } else if (routeData.interactionMode === InteractionMode.View) {
                newObjectTemplate = objectViewTemplate;
                newActionsTemplate = routeData.actionsOpen ? actionsTemplate : nullTemplate;
            } else {
                newObjectTemplate = objectEditTemplate;
                newActionsTemplate = nullTemplate;
            }

            // only update if changed
            if ($scope.objectTemplate !== newObjectTemplate) {
                $scope.objectTemplate = newObjectTemplate;
            }
            if ($scope.actionsTemplate !== newActionsTemplate) {
                $scope.actionsTemplate = newActionsTemplate;
            }

            let focusTarget: FocusTarget;

            if (routeData.dialogId) {
                const action = ovm.domainObject.actionMember(routeData.dialogId);
                focusTarget = FocusTarget.Dialog;
                context.getInvokableAction(action).then(details => setDialog($scope, details, routeData));
            } else if (routeData.actionsOpen) {
                $scope.dialogTemplate = null;
                focusTarget = FocusTarget.SubAction;
            } else if (ovm.isInEdit) {
                $scope.dialogTemplate = null;
                focusTarget = FocusTarget.Property;
            } else {
                $scope.dialogTemplate = null;
                focusTarget = FocusTarget.ObjectTitle;
            }

            focusManager.focusOn(focusTarget, 0, routeData.paneId);
        };

        function handleNewObject ($scope: INakedObjectsScope, routeData: PaneRouteData) {

            const oid = ObjectIdWrapper.fromObjectId(routeData.objectId);

            // to ease transition 
            $scope.objectTemplate = blankTemplate;
            $scope.actionsTemplate = nullTemplate;

            color.toColorNumberFromType(oid.domainType).then(c => $scope.backgroundColor = `${objectColor}${c}`);

            deRegObject[routeData.paneId].deReg();

            const wasDirty = context.getIsDirty(oid);

            context.getObject(routeData.paneId, oid, routeData.interactionMode).
                then((object: DomainObjectRepresentation) => {

                    const ovm = perPaneObjectViews[routeData.paneId].reset(object, routeData);
                    if (wasDirty) {
                        ovm.clearCachedFiles();
                    }

                    $scope.object = ovm;
                    $scope.collectionsTemplate = collectionsTemplate;

                    handleNewObjectSearch($scope, routeData);

                    deRegObject[routeData.paneId].add($scope.$on("$locationChangeStart", ovm.setProperties) as () => void);
                    deRegObject[routeData.paneId].add($scope.$watch(() => $location.search(), ovm.setProperties, true) as () => void);
                    deRegObject[routeData.paneId].add($scope.$on("pane-swap", ovm.setProperties) as () => void);

                }).catch((reject: ErrorWrapper) => {

                    const handler = (cc: ClientErrorCode) => {
                        if (cc === ClientErrorCode.ExpiredTransient) {
                            $scope.objectTemplate = expiredTransientTemplate;
                            return true;
                        }
                        return false;
                    };
                    context.handleWrappedError(reject, null, () => { }, () => { }, handler);
                });

        };


        handlers.handleObject = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {
            handleNewObject($scope, routeData);
        };

        handlers.handleObjectSearch = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

            const oid = ObjectIdWrapper.fromObjectId(routeData.objectId);
            const ovm = $scope.object;

            const newOrChangedObject = (obj: DomainObjectRepresentation) => {
                const oldOid = obj.getOid();
                return !oid.isSame(oldOid) || context.mustReload(oldOid);
            }

            if (!ovm || newOrChangedObject(ovm.domainObject)) {
                handleNewObject($scope, routeData);
            } else {
                ovm.refresh(routeData);
                handleNewObjectSearch($scope, routeData);
            }      
        };
    });
}