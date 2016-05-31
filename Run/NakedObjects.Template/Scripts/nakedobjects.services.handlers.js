/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.services.viewmodelfactory.ts" />
var NakedObjects;
(function (NakedObjects) {
    var ErrorWrapper = NakedObjects.Models.ErrorWrapper;
    var ErrorCategory = NakedObjects.Models.ErrorCategory;
    var ClientErrorCode = NakedObjects.Models.ClientErrorCode;
    var ObjectIdWrapper = NakedObjects.Models.ObjectIdWrapper;
    NakedObjects.app.service("handlers", function ($routeParams, $location, $q, $cacheFactory, $rootScope, repLoader, context, viewModelFactory, color, navigation, urlManager, focusManager) {
        var handlers = this;
        var perPaneListViews = [,
            new NakedObjects.ListViewModel(color, context, viewModelFactory, urlManager, focusManager, $q),
            new NakedObjects.ListViewModel(color, context, viewModelFactory, urlManager, focusManager, $q)
        ];
        var perPaneObjectViews = [,
            new NakedObjects.DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager, $q),
            new NakedObjects.DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager, $q)
        ];
        var perPaneDialogViews = [,
            new NakedObjects.DialogViewModel(color, context, viewModelFactory, urlManager, focusManager, $rootScope),
            new NakedObjects.DialogViewModel(color, context, viewModelFactory, urlManager, focusManager, $rootScope)
        ];
        var perPaneMenusViews = [,
            new NakedObjects.MenusViewModel(viewModelFactory),
            new NakedObjects.MenusViewModel(viewModelFactory)
        ];
        function setVersionError(error) {
            context.setError(new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.SoftwareError, error));
            urlManager.setError(ErrorCategory.ClientError, ClientErrorCode.SoftwareError);
        }
        var DeReg = (function () {
            function DeReg() {
            }
            DeReg.prototype.add = function (newF) {
                this.deRegers.push(newF);
            };
            DeReg.prototype.deReg = function () {
                _.forEach(this.deRegers, function (d) { return d(); });
                this.deRegers = [];
            };
            return DeReg;
        }());
        var deRegDialog = [, new DeReg(), new DeReg()];
        var deRegObject = [, new DeReg(), new DeReg()];
        function setDialog($scope, action, routeData) {
            deRegDialog[routeData.paneId].deReg();
            $scope.dialogTemplate = NakedObjects.dialogTemplate;
            var dialogViewModel = perPaneDialogViews[routeData.paneId];
            var isAlreadyViewModel = action instanceof NakedObjects.ActionViewModel;
            var actionViewModel = !isAlreadyViewModel
                ? viewModelFactory.actionViewModel(action, dialogViewModel, routeData)
                : action;
            dialogViewModel.reset(actionViewModel, routeData);
            $scope.dialog = dialogViewModel;
            deRegDialog[routeData.paneId].add($scope
                .$on("$locationChangeStart", dialogViewModel.setParms));
            deRegDialog[routeData.paneId].add($scope
                .$watch(function () { return $location.search(); }, dialogViewModel.setParms, true));
            dialogViewModel.deregister = function () { return deRegDialog[routeData.paneId].deReg(); };
        }
        var versionValidated = false;
        handlers.handleBackground = function ($scope) {
            color.toColorNumberFromHref($location.absUrl())
                .then(function (c) {
                $scope.backgroundColor = "" + NakedObjects.objectColor + c;
            });
            navigation.push();
            // Just do once - cached but still pointless repeating each page refresh
            if (versionValidated) {
                return;
            }
            context.getVersion()
                .then(function (v) {
                var specVersion = parseFloat(v.specVersion());
                var domainModel = v.optionalCapabilities().domainModel;
                if (specVersion < 1.1) {
                    setVersionError("Restful Objects server must support spec version 1.1 or greater for NakedObjects Gemini\r\n (8.2:specVersion)");
                }
                else if (domainModel !== "simple" && domainModel !== "selectable") {
                    setVersionError("NakedObjects Gemini requires domain metadata representation to be simple or selectable not \"" + domainModel + "\"\r\n (8.2:optionalCapabilities)");
                }
                else {
                    versionValidated = true;
                }
            });
        };
        function setNewMenu($scope, newMenuId, routeData) {
            context.getMenu(newMenuId)
                .then(function (menu) {
                $scope.actionsTemplate = NakedObjects.actionsTemplate;
                $scope.menu = viewModelFactory.menuViewModel(menu, routeData);
                setNewDialog($scope, menu, routeData.dialogId, routeData, NakedObjects.FocusTarget.SubAction);
            })
                .catch(function (reject) {
                context.handleWrappedError(reject, null, function () { }, function () { });
            });
        }
        function setNewDialog($scope, holder, newDialogId, routeData, focusTarget, actionViewModel) {
            if (newDialogId) {
                var action = holder.actionMember(routeData.dialogId);
                context.getInvokableAction(action)
                    .then(function (details) {
                    if (actionViewModel) {
                        actionViewModel.makeInvokable(details);
                    }
                    setDialog($scope, actionViewModel || details, routeData);
                    focusManager.focusOn(NakedObjects.FocusTarget.Dialog, 0, routeData.paneId);
                });
                return;
            }
            $scope.dialogTemplate = null;
            $scope.dialog = null;
            focusManager.focusOn(focusTarget, 0, routeData.paneId);
        }
        function logoff() {
            for (var pane = 1; pane <= 2; pane++) {
                deRegDialog[pane].deReg();
                deRegObject[pane].deReg();
                deRegDialog[pane] = new DeReg();
                deRegObject[pane] = new DeReg();
                perPaneListViews[pane] = new NakedObjects.ListViewModel(color, context, viewModelFactory, urlManager, focusManager, $q);
                perPaneObjectViews[pane] = new NakedObjects.DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager, $q);
                perPaneDialogViews[pane] = new NakedObjects.DialogViewModel(color, context, viewModelFactory, urlManager, focusManager, $rootScope);
                perPaneMenusViews[pane] = new NakedObjects.MenusViewModel(viewModelFactory);
            }
        }
        $rootScope.$on(NakedObjects.geminiLogoffEvent, function () { return logoff(); });
        handlers.handleHomeSearch = function ($scope, routeData) {
            context.clearWarnings();
            context.clearMessages();
            if (routeData.menuId) {
                var currentMenu = $scope.menu;
                var currentMenuId = currentMenu ? currentMenu.id : null;
                var newMenuId = routeData.menuId;
                var currentDialog = $scope.dialog;
                var currentDialogId = currentDialog ? currentDialog.id : null;
                var newDialogId = routeData.dialogId;
                if (currentMenuId !== newMenuId) {
                    // menu changed set new menu and if necessary new dialog
                    setNewMenu($scope, newMenuId, routeData);
                }
                else if (currentDialogId !== newDialogId) {
                    // dialog changed set new dialog only 
                    setNewDialog($scope, currentMenu.menuRep, newDialogId, routeData, NakedObjects.FocusTarget.SubAction);
                }
            }
            else {
                $scope.actionsTemplate = null;
                $scope.menu = null;
                $scope.dialogTemplate = null;
                $scope.dialog = null;
                focusManager.focusOn(NakedObjects.FocusTarget.Menu, 0, routeData.paneId);
            }
        };
        handlers.handleHome = function ($scope, routeData) {
            $scope.homeTemplate = NakedObjects.homePlaceholderTemplate;
            context.getMenus()
                .then(function (menus) {
                $scope.menus = perPaneMenusViews[routeData.paneId].reset(menus, routeData);
                $scope.homeTemplate = NakedObjects.homeTemplate;
                handlers.handleHomeSearch($scope, routeData);
            })
                .catch(function (reject) {
                context.handleWrappedError(reject, null, function () { }, function () { });
            });
        };
        function getActionExtensions(routeData) {
            return routeData.objectId ?
                context.getActionExtensionsFromObject(routeData.paneId, ObjectIdWrapper.fromObjectId(routeData.objectId), routeData.actionId) :
                context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);
        }
        function handleListActionsAndDialog($scope, routeData) {
            var newActionsTemplate = routeData.actionsOpen ? NakedObjects.actionsTemplate : NakedObjects.nullTemplate;
            if ($scope.actionsTemplate !== newActionsTemplate) {
                $scope.actionsTemplate = newActionsTemplate;
            }
            var focusTarget = routeData.actionsOpen ? NakedObjects.FocusTarget.SubAction : NakedObjects.FocusTarget.ListItem;
            var currentDialog = $scope.dialog;
            var currentDialogId = currentDialog ? currentDialog.id : null;
            var newDialogId = routeData.dialogId;
            if (currentDialogId !== newDialogId) {
                var listViewModel = $scope.collection;
                var actionViewModel = _.find(listViewModel.actions, function (a) { return a.actionRep.actionId() === newDialogId; });
                setNewDialog($scope, listViewModel, newDialogId, routeData, focusTarget, actionViewModel);
            }
        }
        function handleListSearchChanged($scope, routeData) {
            // only update templates if changed 
            var newListTemplate = routeData.state === NakedObjects.CollectionViewState.List ? NakedObjects.listTemplate : NakedObjects.listAsTableTemplate;
            var listViewModel = $scope.collection;
            if ($scope.listTemplate !== newListTemplate) {
                $scope.listTemplate = newListTemplate;
                listViewModel.refresh(routeData);
            }
            handleListActionsAndDialog($scope, routeData);
        }
        handlers.handleListSearch = function ($scope, routeData) {
            var listKey = urlManager.getListCacheIndex(routeData.paneId, routeData.page, routeData.pageSize);
            var listViewModel = $scope.collection;
            if (listKey !== listViewModel.id) {
                handlers.handleList($scope, routeData);
            }
            else {
                handleListSearchChanged($scope, routeData);
            }
        };
        handlers.handleList = function ($scope, routeData) {
            var cachedList = context.getCachedList(routeData.paneId, routeData.page, routeData.pageSize);
            if (cachedList) {
                var listViewModel = perPaneListViews[routeData.paneId];
                $scope.listTemplate = routeData.state === NakedObjects.CollectionViewState.List ? NakedObjects.listTemplate : NakedObjects.listAsTableTemplate;
                listViewModel.reset(cachedList, routeData);
                $scope.collection = listViewModel;
                getActionExtensions(routeData).then(function (ext) { return $scope.title = ext.friendlyName(); });
                handleListActionsAndDialog($scope, routeData);
            }
            else {
                $scope.listTemplate = NakedObjects.listPlaceholderTemplate;
                $scope.collectionPlaceholder = viewModelFactory.listPlaceholderViewModel(routeData);
                getActionExtensions(routeData).then(function (ext) { return $scope.title = ext.friendlyName(); });
                focusManager.focusOn(NakedObjects.FocusTarget.Action, 0, routeData.paneId);
            }
        };
        handlers.handleRecent = function ($scope, routeData) {
            context.clearWarnings();
            context.clearMessages();
            $scope.recentTemplate = NakedObjects.recentTemplate;
            $scope.recent = viewModelFactory.recentItemsViewModel(routeData.paneId);
        };
        handlers.handleError = function ($scope, routeData) {
            var evm = viewModelFactory.errorViewModel(context.getError());
            $scope.error = evm;
            if (evm.isConcurrencyError) {
                $scope.errorTemplate = NakedObjects.concurrencyTemplate;
            }
            else {
                $scope.errorTemplate = NakedObjects.errorTemplate;
            }
        };
        handlers.handleToolBar = function ($scope) {
            $scope.toolBar = viewModelFactory.toolBarViewModel();
        };
        function handleNewObjectSearch($scope, routeData) {
            var ovm = $scope.object;
            var newObjectTemplate;
            var newActionsTemplate;
            if (routeData.interactionMode === NakedObjects.InteractionMode.Form) {
                newObjectTemplate = NakedObjects.formTemplate;
                newActionsTemplate = NakedObjects.formActionsTemplate;
            }
            else if (routeData.interactionMode === NakedObjects.InteractionMode.View) {
                newObjectTemplate = NakedObjects.objectViewTemplate;
                newActionsTemplate = routeData.actionsOpen ? NakedObjects.actionsTemplate : NakedObjects.nullTemplate;
            }
            else {
                newObjectTemplate = NakedObjects.objectEditTemplate;
                newActionsTemplate = NakedObjects.nullTemplate;
            }
            // only update if changed
            if ($scope.objectTemplate !== newObjectTemplate) {
                $scope.objectTemplate = newObjectTemplate;
            }
            if ($scope.actionsTemplate !== newActionsTemplate) {
                $scope.actionsTemplate = newActionsTemplate;
            }
            var focusTarget;
            var currentDialog = $scope.dialog;
            var currentDialogId = currentDialog ? currentDialog.id : null;
            var newDialogId = routeData.dialogId;
            if (routeData.dialogId) {
                focusTarget = NakedObjects.FocusTarget.Dialog;
            }
            else if (routeData.actionsOpen) {
                focusTarget = NakedObjects.FocusTarget.SubAction;
            }
            else if (ovm.isInEdit) {
                focusTarget = NakedObjects.FocusTarget.Property;
            }
            else {
                focusTarget = NakedObjects.FocusTarget.ObjectTitle;
            }
            if (currentDialogId !== newDialogId) {
                setNewDialog($scope, ovm.domainObject, newDialogId, routeData, focusTarget);
            }
            else {
                focusManager.focusOn(focusTarget, 0, routeData.paneId);
            }
        }
        ;
        function handleNewObject($scope, routeData) {
            var oid = ObjectIdWrapper.fromObjectId(routeData.objectId);
            // to ease transition 
            $scope.objectTemplate = NakedObjects.blankTemplate;
            $scope.actionsTemplate = NakedObjects.nullTemplate;
            color.toColorNumberFromType(oid.domainType).then(function (c) { return $scope.backgroundColor = "" + NakedObjects.objectColor + c; });
            deRegObject[routeData.paneId].deReg();
            var wasDirty = context.getIsDirty(oid);
            context.getObject(routeData.paneId, oid, routeData.interactionMode).
                then(function (object) {
                var ovm = perPaneObjectViews[routeData.paneId].reset(object, routeData);
                if (wasDirty) {
                    ovm.clearCachedFiles();
                }
                $scope.object = ovm;
                $scope.collectionsTemplate = NakedObjects.collectionsTemplate;
                handleNewObjectSearch($scope, routeData);
                deRegObject[routeData.paneId].add($scope.$on("$locationChangeStart", ovm.setProperties));
                deRegObject[routeData.paneId].add($scope.$watch(function () { return $location.search(); }, ovm.setProperties, true));
                deRegObject[routeData.paneId].add($scope.$on(NakedObjects.geminiPaneSwapEvent, ovm.setProperties));
                deRegObject[routeData.paneId].add($scope.$on(NakedObjects.geminiDisplayErrorEvent, ovm.displayError()));
            }).catch(function (reject) {
                var handler = function (cc) {
                    if (cc === ClientErrorCode.ExpiredTransient) {
                        $scope.objectTemplate = NakedObjects.expiredTransientTemplate;
                        return true;
                    }
                    return false;
                };
                context.handleWrappedError(reject, null, function () { }, function () { }, handler);
            });
        }
        ;
        handlers.handleObject = function ($scope, routeData) {
            handleNewObject($scope, routeData);
        };
        handlers.handleObjectSearch = function ($scope, routeData) {
            var oid = ObjectIdWrapper.fromObjectId(routeData.objectId);
            var ovm = $scope.object;
            var newOrChangedObject = function (obj) {
                var oldOid = obj.getOid();
                return !oid.isSame(oldOid) || context.mustReload(oldOid);
            };
            if (!ovm || newOrChangedObject(ovm.domainObject)) {
                handleNewObject($scope, routeData);
            }
            else {
                ovm.refresh(routeData);
                handleNewObjectSearch($scope, routeData);
            }
        };
        handlers.handleAttachment = function ($scope, routeData) {
            context.clearWarnings();
            context.clearMessages();
            var oid = ObjectIdWrapper.fromObjectId(routeData.objectId);
            $scope.attachmentTemplate = NakedObjects.attachmentTemplate;
            context.getObject(routeData.paneId, oid, routeData.interactionMode)
                .then(function (object) {
                var attachmentId = routeData.attachmentId;
                var attachment = object.propertyMember(attachmentId);
                if (attachment && attachment.attachmentLink()) {
                    var avm = viewModelFactory.attachmentViewModel(attachment, routeData.paneId);
                    $scope.attachment = avm;
                }
            });
        };
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.handlers.js.map