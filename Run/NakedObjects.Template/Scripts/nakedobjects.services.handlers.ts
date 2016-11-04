/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.services.viewmodelfactory.ts" />

namespace NakedObjects {
    import ErrorWrapper = Models.ErrorWrapper;
    import ErrorCategory = Models.ErrorCategory;
    import ClientErrorCode = Models.ClientErrorCode;
    import DomainObjectRepresentation = Models.DomainObjectRepresentation;
    import ActionMember = Models.ActionMember;
    import MenusRepresentation = Models.MenusRepresentation;
    import MenuRepresentation = Models.MenuRepresentation;
    import ActionRepresentation = Models.ActionRepresentation;
    import ObjectIdWrapper = Models.ObjectIdWrapper;


    export interface IHandlers {
        handleBackground($scope: INakedObjectsScope): void;
        handleError($scope: INakedObjectsScope): void;
        handleToolBar($scope: INakedObjectsScope): void;
        handleObject($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleObjectSearch($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleHome($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleHomeSearch($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleList($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleListSearch($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleRecent($scope: INakedObjectsScope, routeData: PaneRouteData): void;
        handleAttachment(nakedObjectsScope: INakedObjectsScope, paneRouteData: PaneRouteData): void;
        handleApplicationProperties(nakedObjectsScope: INakedObjectsScope, paneRouteData: PaneRouteData): void;
        handleMultiLineDialog(nakedObjectsScope: INakedObjectsScope, paneRouteData: PaneRouteData): void;
    }

    app.service("handlers",
        function ($routeParams: ng.route.IRouteParamsService,
            $location: ng.ILocationService,
            $q: ng.IQService,
            $cacheFactory: ng.ICacheFactoryService,
            $rootScope: ng.IRootScopeService,
            repLoader: IRepLoader,
            context: IContext,
            viewModelFactory: IViewModelFactory,
            color: IColor,
            navigation: INavigation,
            urlManager: IUrlManager,
            focusManager: IFocusManager,
            template: ITemplate,
            error : IError) {
            const handlers = <IHandlers>this;

            const perPaneListViews = [,
                new ListViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $q),
                new ListViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $q)
            ];

            const perPaneObjectViews = [,
                new DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $q),
                new DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $q)
            ];

            const perPaneDialogViews = [,
                new DialogViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $rootScope),
                new DialogViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $rootScope)
            ];

            const perPaneMenusViews = [,
                new MenusViewModel(viewModelFactory),
                new MenusViewModel(viewModelFactory)
            ];

            const perPaneMultiLineDialogViews = [,
                new MultiLineDialogViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $rootScope)                
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

            const deRegObject = [, new DeReg(), new DeReg()];

            function clearDialog($scope : INakedObjectsScope, paneId : number) {
                $scope.dialogTemplate = null;
                $scope.dialog = null;
                context.clearParmUpdater(paneId);
            }

            function setDialog($scope: INakedObjectsScope,
                                action: ActionMember | ActionRepresentation | IActionViewModel,
                                routeData: PaneRouteData,
                                matchingCollectionId : string) {
                context.clearParmUpdater(routeData.paneId);

                $scope.dialogTemplate = dialogTemplate;
                $scope.parametersTemplate = parametersTemplate;
                $scope.parameterTemplate = parameterTemplate;

                const dialogViewModel = perPaneDialogViews[routeData.paneId];
                const isAlreadyViewModel = action instanceof ActionViewModel;
                const actionViewModel = !isAlreadyViewModel
                    ? viewModelFactory.actionViewModel(action as ActionMember | ActionRepresentation,
                        dialogViewModel,
                        routeData)
                    : action as IActionViewModel;

                dialogViewModel.matchingCollectionId = matchingCollectionId;

                dialogViewModel.reset(actionViewModel, routeData.paneId);
                $scope.dialog = dialogViewModel;

                context.setParmUpdater(dialogViewModel.setParms, routeData.paneId);
                dialogViewModel.deregister = () => context.clearParmUpdater(routeData.paneId);
            }

            let versionValidated = false;

            handlers.handleBackground = ($scope: INakedObjectsScope) => {
                color.toColorNumberFromHref($location.absUrl()).
                    then(c => $scope.backgroundColor = `${objectColor}${c}`).
                    catch((reject: ErrorWrapper) => error.handleError(reject));

                navigation.push();

                // Just do once - cached but still pointless repeating each page refresh

                if (versionValidated) {
                    return;
                }

                context.getVersion().
                    then(v => {
                        const specVersion = parseFloat(v.specVersion());
                        const domainModel = v.optionalCapabilities().domainModel;

                        if (specVersion < 1.1) {
                            setVersionError("Restful Objects server must support spec version 1.1 or greater for NakedObjects Gemini\r\n (8.2:specVersion)");
                        } else if (domainModel !== "simple" && domainModel !== "selectable") {
                            setVersionError(`NakedObjects Gemini requires domain metadata representation to be simple or selectable not "${domainModel}"\r\n (8.2:optionalCapabilities)`);
                        } else {
                            versionValidated = true;
                        }
                    }).
                    catch((reject: ErrorWrapper) => error.handleError(reject));
            };

            function setNewMenu($scope: INakedObjectsScope, newMenuId: string, routeData: PaneRouteData) {
            
                context.getMenu(newMenuId)
                    .then((menu: MenuRepresentation) => {
                        $scope.actionsTemplate = actionsTemplate;
                        $scope.menu = viewModelFactory.menuViewModel(menu, routeData);
                        setNewDialog($scope, menu, routeData.dialogId, routeData, FocusTarget.SubAction);
                    })
                    .catch((reject: ErrorWrapper) => {
                        error.handleError(reject);
                    });
            }

            function setNewDialog($scope: INakedObjectsScope,
                                  holder: MenuRepresentation | DomainObjectRepresentation | IListViewModel | ICollectionViewModel,
                                  newDialogId: string,
                                  routeData: PaneRouteData,
                                  focusTarget: FocusTarget,
                                  actionViewModel?: IActionViewModel) {
                if (newDialogId) {
                    const action = holder.actionMember(routeData.dialogId);
                    context.getInvokableAction(action).
                        then(details => {
                            if (actionViewModel) {
                                actionViewModel.makeInvokable(details);
                            }

                            const matchingCollectionId = holder instanceof CollectionViewModel ? holder.id : "";
                        
                            setDialog($scope, actionViewModel || details, routeData, matchingCollectionId);

                            focusManager.focusOn(FocusTarget.Dialog, 0, routeData.paneId);
                        }).
                        catch((reject: ErrorWrapper) => error.handleError(reject));
                    return;
                }
                clearDialog($scope, routeData.paneId);      
                focusManager.focusOn(focusTarget, 0, routeData.paneId);
            }

            function logoff() {
                for (let pane = 1; pane <= 2; pane++) {
                    context.clearParmUpdater(pane);
                    context.clearObjectUpdater(pane);

                    perPaneListViews[pane] = new ListViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $q);
                    perPaneObjectViews[pane] = new DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $q);
                    perPaneDialogViews[pane] = new DialogViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $rootScope);
                    perPaneMenusViews[pane] = new MenusViewModel(viewModelFactory);
                }
            }

            $rootScope.$on(geminiLogoffEvent, () => logoff());


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
                        setNewDialog($scope, currentMenu.menuRep, newDialogId, routeData, FocusTarget.SubAction);
                    } else if ($scope.dialog) {
                        $scope.dialog.refresh();
                    }
                } else {
                    $scope.actionsTemplate = null;
                    $scope.menu = null;
                    clearDialog($scope, routeData.paneId);
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
                        error.handleError(reject);
                    });
            };

            function getActionExtensions(routeData: PaneRouteData) {
                return routeData.objectId ?
                    context.getActionExtensionsFromObject(routeData.paneId, ObjectIdWrapper.fromObjectId(routeData.objectId), routeData.actionId) :
                    context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);
            }

            function handleListActionsAndDialog($scope: INakedObjectsScope, routeData: PaneRouteData) {

                const newActionsTemplate = routeData.actionsOpen ? actionsTemplate : nullTemplate;
                if ($scope.actionsTemplate !== newActionsTemplate) {
                    $scope.actionsTemplate = newActionsTemplate;
                }

                const focusTarget = routeData.actionsOpen ? FocusTarget.SubAction : FocusTarget.ListItem;

                const currentDialog = $scope.dialog;
                const currentDialogId = currentDialog ? currentDialog.id : null;
                const newDialogId = routeData.dialogId;

                // deliberate == to catch undefined == null 
                if (!(currentDialogId == newDialogId)) {
                    const listViewModel = $scope.collection;
                    const actionViewModel = _.find(listViewModel.actions, a => a.actionRep.actionId() === newDialogId);
                    setNewDialog($scope, listViewModel, newDialogId, routeData, focusTarget, actionViewModel);
                } else if ($scope.dialog) {
                    $scope.dialog.refresh();
                    $scope.dialog.resetMessage();
                }

                $scope.collection.resetMessage();
            }

            function handleListSearchChanged($scope: INakedObjectsScope, routeData: PaneRouteData) {
                // only update templates if changed 
                const listViewModel = $scope.collection;

                const newListTemplate = template.getTemplateName(listViewModel.listRep.extensions().elementType(), TemplateType.List, routeData.state);

                if ($scope.listTemplate !== newListTemplate) {
                    $scope.listTemplate = newListTemplate;
                    listViewModel.refresh(routeData);
                }

                handleListActionsAndDialog($scope, routeData);
            }


            handlers.handleListSearch = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

                const listKey = urlManager.getListCacheIndex(routeData.paneId, routeData.page, routeData.pageSize);
                const listViewModel = $scope.collection;

                if (listKey !== listViewModel.id) {
                    handlers.handleList($scope, routeData);
                } else {
                    handleListSearchChanged($scope, routeData);
                }
            };


            handlers.handleList = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {

                const cachedList = context.getCachedList(routeData.paneId, routeData.page, routeData.pageSize);

                const setFriendlyName = () =>
                    getActionExtensions(routeData).
                        then(ext => $scope.title = ext.friendlyName()).
                        catch((reject: ErrorWrapper) => error.handleError(reject));

                if (cachedList) {
                    const listViewModel = perPaneListViews[routeData.paneId];
                    $scope.listTemplate = template.getTemplateName(cachedList.extensions().elementType(), TemplateType.List, routeData.state);
                    listViewModel.reset(cachedList, routeData);
                    $scope.collection = listViewModel;
                    setFriendlyName();
                    handleListActionsAndDialog($scope, routeData);
                } else {
                    $scope.listTemplate = listPlaceholderTemplate;
                    $scope.collectionPlaceholder = viewModelFactory.listPlaceholderViewModel(routeData);
                    setFriendlyName();
                    focusManager.focusOn(FocusTarget.Action, 0, routeData.paneId);
                }
            };

            handlers.handleRecent = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {
                context.clearWarnings();
                context.clearMessages();
                $scope.recentTemplate = recentTemplate;
                $scope.recent = viewModelFactory.recentItemsViewModel(routeData.paneId);
            };

            handlers.handleError = ($scope: INakedObjectsScope) => {
                const evm = viewModelFactory.errorViewModel(context.getError());
                $scope.error = evm;
                error.displayError($scope);
            };

            handlers.handleToolBar = ($scope: INakedObjectsScope) => {
                $scope.toolBar = viewModelFactory.toolBarViewModel();
            };

            function handleNewObjectSearch($scope: INakedObjectsScope, routeData: PaneRouteData) {

                const ovm = $scope.object;

                let newActionsTemplate: string;

                const newObjectTemplate = template.getTemplateName(ovm.domainType, TemplateType.Object,  routeData.interactionMode);

                if (routeData.interactionMode === InteractionMode.Form) {
                    newActionsTemplate = formActionsTemplate;
                } else if (routeData.interactionMode === InteractionMode.View) {
                    newActionsTemplate = routeData.actionsOpen ? actionsTemplate : nullTemplate;
                } else {
                    newActionsTemplate = nullTemplate;
                }

                // only update if changed
                if ($scope.objectTemplate !== newObjectTemplate) {
                    $scope.objectTemplate = newObjectTemplate;
                }
                if ($scope.actionsTemplate !== newActionsTemplate) {
                    $scope.actionsTemplate = newActionsTemplate;
                }
                if ($scope.propertiesTemplate !== propertiesTemplate) {
                    $scope.propertiesTemplate = propertiesTemplate;
                }
                if ($scope.propertyTemplate !== propertyTemplate) {
                    $scope.propertyTemplate = propertyTemplate;
                }

                let focusTarget: FocusTarget;

                const currentDialog = $scope.dialog;
                const currentDialogId = currentDialog ? currentDialog.id : null;
                const newDialogId = routeData.dialogId;

                if (routeData.dialogId) {
                    focusTarget = FocusTarget.Dialog;
                } else if (routeData.actionsOpen) {
                    focusTarget = FocusTarget.SubAction;
                } else if (ovm.isInEdit) {
                    context.setObjectUpdater(ovm.setProperties, routeData.paneId);
                    focusTarget = FocusTarget.Property;
                } else {
                    focusTarget = FocusTarget.ObjectTitle;
                }

                 // deliberate == to catch undefined == null 
                if (!(currentDialogId == newDialogId)) {

                    // need to match Locally Contributed Actions 

                    const lcaCollection = _.find(ovm.collections, c => c.hasMatchingLocallyContributedAction(newDialogId));

                    if (lcaCollection) {            
                        const actionViewModel = _.find(lcaCollection.actions, a => a.actionRep.actionId() === newDialogId);
                        setNewDialog($scope, lcaCollection, newDialogId, routeData, focusTarget, actionViewModel);
                    } else {
                        setNewDialog($scope, ovm.domainObject, newDialogId, routeData, focusTarget);
                    }
                } else {
                    if ($scope.dialog) {
                        $scope.dialog.refresh();
                    }
                    focusManager.focusOn(focusTarget, 0, routeData.paneId);
                }
            };

            function handleNewObject($scope: INakedObjectsScope, routeData: PaneRouteData) {

                const oid = ObjectIdWrapper.fromObjectId(routeData.objectId);

                // to ease transition 
                $scope.objectTemplate = blankTemplate;
                $scope.actionsTemplate = nullTemplate;

                color.toColorNumberFromType(oid.domainType).
                    then(c => $scope.backgroundColor = `${objectColor}${c}`).
                    catch((reject: ErrorWrapper) => error.handleError(reject));

                deRegObject[routeData.paneId].deReg();
                context.clearObjectUpdater(routeData.paneId);

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

                        deRegObject[routeData.paneId].add($scope.$on(geminiConcurrencyEvent, ovm.concurrency()) as () => void);
                    }).
                    catch((reject: ErrorWrapper) => {
                        if (reject.category === ErrorCategory.ClientError && reject.clientErrorCode === ClientErrorCode.ExpiredTransient) {
                            context.setError(reject);
                            $scope.objectTemplate = expiredTransientTemplate;
                        } else {
                            error.handleError(reject);
                        }
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

            handlers.handleAttachment = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {
                context.clearWarnings();
                context.clearMessages();

                const oid = ObjectIdWrapper.fromObjectId(routeData.objectId);
                $scope.attachmentTemplate = attachmentTemplate;

                context.getObject(routeData.paneId, oid, routeData.interactionMode).
                    then((object: DomainObjectRepresentation) => {

                        const attachmentId = routeData.attachmentId;
                        const attachment = object.propertyMember(attachmentId);

                        if (attachment && attachment.attachmentLink()) {
                            const avm = viewModelFactory.attachmentViewModel(attachment, routeData.paneId);
                            $scope.attachment = avm;
                        }
                    }).
                    catch((reject: ErrorWrapper) => error.handleError(reject));
            }

            handlers.handleApplicationProperties = ($scope: INakedObjectsScope) => {
                context.clearWarnings();
                context.clearMessages();

                $scope.applicationPropertiesTemplate = applicationPropertiesTemplate;

                const apvm = new ApplicationPropertiesViewModel();
                $scope.applicationProperties = apvm;

                context.getUser().
                    then(u => apvm.user = u.wrapped()).
                    catch((reject: ErrorWrapper) => error.handleError(reject));

                context.getVersion().
                    then(v => apvm.serverVersion = v.wrapped()).
                    catch((reject: ErrorWrapper) => error.handleError(reject));

                apvm.serverUrl = getAppPath();

                apvm.clientVersion = (NakedObjects as any)["version"] || "Failed to write version";
            }


            function setMultiLineDialog($scope: INakedObjectsScope,
                                        holder: MenuRepresentation | DomainObjectRepresentation | ICollectionViewModel,
                                        newDialogId: string,
                                        routeData: PaneRouteData,
                                        actionViewModel? : IActionViewModel) {

                const action = holder.actionMember(newDialogId);
                context.getInvokableAction(action).
                    then(details => {

                        if (actionViewModel) {
                            actionViewModel.makeInvokable(details);
                        }
                
                        $scope.multiLineDialogTemplate = multiLineDialogTemplate;
                        $scope.parametersTemplate = parametersTemplate;
                        $scope.parameterTemplate = parameterTemplate;
                        $scope.readOnlyParameterTemplate = readOnlyParameterTemplate;

                        const dialogViewModel = perPaneMultiLineDialogViews[routeData.paneId];                   
                        dialogViewModel.reset(routeData, details);
                       
                        if (holder instanceof DomainObjectRepresentation) {
                            dialogViewModel.objectTitle = holder.title();
                            dialogViewModel.objectFriendlyName = holder.extensions().friendlyName();        
                        } else {
                            dialogViewModel.objectFriendlyName = "";
                            dialogViewModel.objectTitle = "";
                        }

                        $scope.multiLineDialog = dialogViewModel;
                    }).
                    catch((reject: ErrorWrapper) => error.handleError(reject));
            }

            handlers.handleMultiLineDialog = ($scope: INakedObjectsScope, routeData: PaneRouteData) => {
                if (routeData.menuId) {
                    context.getMenu(routeData.menuId)
                        .then((menu: MenuRepresentation) => {                        
                            setMultiLineDialog($scope, menu, routeData.dialogId, routeData);
                        })
                        .catch((reject: ErrorWrapper) => {
                            error.handleError(reject);
                        });
                } 
                else if (routeData.objectId) {
                    const oid = ObjectIdWrapper.fromObjectId(routeData.objectId);
                    context.getObject(routeData.paneId, oid, routeData.interactionMode).
                        then((object: DomainObjectRepresentation) => {

                            const ovm = perPaneObjectViews[routeData.paneId].reset(object, routeData);
                            const newDialogId = routeData.dialogId;

                            const lcaCollection = _.find(ovm.collections, c => c.hasMatchingLocallyContributedAction(newDialogId));

                            if (lcaCollection) {
                                const actionViewModel = _.find(lcaCollection.actions, a => a.actionRep.actionId() === newDialogId);
                                setMultiLineDialog($scope, lcaCollection, newDialogId, routeData, actionViewModel);
                            } else {
                                setMultiLineDialog($scope, object, newDialogId, routeData);
                            }
                            
                        }).
                        catch((reject: ErrorWrapper) => {
                            error.handleError(reject);
                        });
                }

            }
        });
}