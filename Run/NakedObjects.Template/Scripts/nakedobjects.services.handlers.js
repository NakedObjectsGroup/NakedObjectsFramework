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
    NakedObjects.app.service("handlers", function ($routeParams, $location, $q, $cacheFactory, $rootScope, repLoader, context, viewModelFactory, color, navigation, urlManager, focusManager, template, error) {
        var handlers = this;
        var perPaneListViews = [,
            new NakedObjects.ListViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $q),
            new NakedObjects.ListViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $q)
        ];
        var perPaneObjectViews = [,
            new NakedObjects.DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $q),
            new NakedObjects.DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $q)
        ];
        var perPaneDialogViews = [,
            new NakedObjects.DialogViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $rootScope),
            new NakedObjects.DialogViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $rootScope)
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
        var deRegObject = [, new DeReg(), new DeReg()];
        function clearDialog($scope, paneId) {
            $scope.dialogTemplate = null;
            $scope.dialog = null;
            context.clearParmUpdater(paneId);
        }
        function setDialog($scope, action, routeData) {
            context.clearParmUpdater(routeData.paneId);
            $scope.dialogTemplate = NakedObjects.dialogTemplate;
            var dialogViewModel = perPaneDialogViews[routeData.paneId];
            var isAlreadyViewModel = action instanceof NakedObjects.ActionViewModel;
            var actionViewModel = !isAlreadyViewModel
                ? viewModelFactory.actionViewModel(action, dialogViewModel, routeData)
                : action;
            dialogViewModel.reset(actionViewModel, routeData);
            $scope.dialog = dialogViewModel;
            context.setParmUpdater(dialogViewModel.setParms, routeData.paneId);
            dialogViewModel.deregister = function () { return context.clearParmUpdater(routeData.paneId); };
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
                error.handleError(reject);
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
            clearDialog($scope, routeData.paneId);
            focusManager.focusOn(focusTarget, 0, routeData.paneId);
        }
        function logoff() {
            for (var pane = 1; pane <= 2; pane++) {
                context.clearParmUpdater(pane);
                context.clearObjectUpdater(pane);
                perPaneListViews[pane] = new NakedObjects.ListViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $q);
                perPaneObjectViews[pane] = new NakedObjects.DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $q);
                perPaneDialogViews[pane] = new NakedObjects.DialogViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $rootScope);
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
                else if ($scope.dialog) {
                    $scope.dialog.refresh();
                }
            }
            else {
                $scope.actionsTemplate = null;
                $scope.menu = null;
                clearDialog($scope, routeData.paneId);
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
                error.handleError(reject);
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
            else if ($scope.dialog) {
                $scope.dialog.refresh();
            }
        }
        function handleListSearchChanged($scope, routeData) {
            // only update templates if changed 
            var listViewModel = $scope.collection;
            var newListTemplate = template.getTemplateName(listViewModel.listRep.extensions().elementType(), NakedObjects.TemplateType.List, routeData.state);
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
                $scope.listTemplate = template.getTemplateName(cachedList.extensions().elementType(), NakedObjects.TemplateType.List, routeData.state);
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
            error.displayError($scope, routeData);
        };
        handlers.handleToolBar = function ($scope) {
            $scope.toolBar = viewModelFactory.toolBarViewModel();
        };
        function handleNewObjectSearch($scope, routeData) {
            var ovm = $scope.object;
            var newActionsTemplate;
            var newObjectTemplate = template.getTemplateName(ovm.domainType, NakedObjects.TemplateType.Object, routeData.interactionMode);
            if (routeData.interactionMode === NakedObjects.InteractionMode.Form) {
                newActionsTemplate = NakedObjects.formActionsTemplate;
            }
            else if (routeData.interactionMode === NakedObjects.InteractionMode.View) {
                newActionsTemplate = routeData.actionsOpen ? NakedObjects.actionsTemplate : NakedObjects.nullTemplate;
            }
            else {
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
                context.setObjectUpdater(ovm.setProperties, routeData.paneId);
                focusTarget = NakedObjects.FocusTarget.Property;
            }
            else {
                focusTarget = NakedObjects.FocusTarget.ObjectTitle;
            }
            if (currentDialogId !== newDialogId) {
                setNewDialog($scope, ovm.domainObject, newDialogId, routeData, focusTarget);
            }
            else {
                if ($scope.dialog) {
                    $scope.dialog.refresh();
                }
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
            context.clearObjectUpdater(routeData.paneId);
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
                deRegObject[routeData.paneId].add($scope.$on(NakedObjects.geminiConcurrencyEvent, ovm.concurrency()));
            }).catch(function (reject) {
                if (reject.category === ErrorCategory.ClientError && reject.clientErrorCode === ClientErrorCode.ExpiredTransient) {
                    context.setError(reject);
                    $scope.objectTemplate = NakedObjects.expiredTransientTemplate;
                }
                else {
                    error.handleError(reject);
                }
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
        handlers.handleApplicationProperties = function ($scope, routeData) {
            context.clearWarnings();
            context.clearMessages();
            $scope.applicationPropertiesTemplate = NakedObjects.applicationPropertiesTemplate;
            var apvm = new NakedObjects.ApplicationPropertiesViewModel();
            $scope.applicationProperties = apvm;
            context.getUser().then(function (u) { return apvm.user = u.wrapped(); });
            context.getVersion().then(function (v) { return apvm.serverVersion = v.wrapped(); });
            apvm.serverUrl = NakedObjects.getAppPath();
            apvm.clientVersion = NakedObjects["version"] || "Failed to write version";
        };
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.handlers.js.map