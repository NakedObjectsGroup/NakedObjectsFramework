/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.services.viewmodelfactory.ts" />
var NakedObjects;
(function (NakedObjects) {
    var ErrorWrapper = NakedObjects.Models.ErrorWrapper;
    var ErrorCategory = NakedObjects.Models.ErrorCategory;
    var ClientErrorCode = NakedObjects.Models.ClientErrorCode;
    NakedObjects.app.service("handlers", function ($routeParams, $location, $q, $cacheFactory, repLoader, context, viewModelFactory, color, navigation, urlManager, focusManager) {
        var handlers = this;
        var perPaneListViews = [
            ,
            new NakedObjects.ListViewModel(color, context, viewModelFactory, urlManager, focusManager, $q),
            new NakedObjects.ListViewModel(color, context, viewModelFactory, urlManager, focusManager, $q)
        ];
        var perPaneObjectViews = [
            ,
            new NakedObjects.DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager, $q),
            new NakedObjects.DomainObjectViewModel(color, context, viewModelFactory, urlManager, focusManager, $q)
        ];
        var perPaneDialogViews = [
            ,
            new NakedObjects.DialogViewModel(color, context, viewModelFactory, urlManager, focusManager),
            new NakedObjects.DialogViewModel(color, context, viewModelFactory, urlManager, focusManager)
        ];
        var perPaneMenusViews = [
            ,
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
            var actionViewModel = !isAlreadyViewModel ? viewModelFactory.actionViewModel(action, dialogViewModel, routeData) : action;
            dialogViewModel.reset(actionViewModel, routeData);
            $scope.dialog = dialogViewModel;
            deRegDialog[routeData.paneId].add($scope.$on("$locationChangeStart", dialogViewModel.setParms));
            deRegDialog[routeData.paneId].add($scope.$watch(function () { return $location.search(); }, dialogViewModel.setParms, true));
        }
        var versionValidated = false;
        handlers.handleBackground = function ($scope) {
            color.toColorNumberFromHref($location.absUrl()).then(function (c) {
                $scope.backgroundColor = "" + NakedObjects.objectColor + c;
            });
            navigation.push();
            // Just do once - cached but still pointless repeating each page refresh
            if (versionValidated) {
                return;
            }
            context.getVersion().then(function (v) {
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
        handlers.handleHome = function ($scope, routeData) {
            context.clearWarnings();
            context.clearMessages();
            $scope.homeTemplate = NakedObjects.homePlaceholderTemplate;
            context.getMenus().
                then(function (menus) {
                $scope.menus = perPaneMenusViews[routeData.paneId].reset(menus, routeData);
                $scope.homeTemplate = NakedObjects.homeTemplate;
                if (routeData.menuId) {
                    context.getMenu(routeData.menuId).
                        then(function (menu) {
                        $scope.actionsTemplate = NakedObjects.actionsTemplate;
                        $scope.menu = viewModelFactory.menuViewModel(menu, routeData);
                        var focusTarget = routeData.dialogId ? NakedObjects.FocusTarget.Dialog : NakedObjects.FocusTarget.SubAction;
                        if (routeData.dialogId) {
                            var action = menu.actionMember(routeData.dialogId);
                            context.getInvokableAction(action).then(function (details) { return setDialog($scope, details, routeData); });
                        }
                        focusManager.focusOn(focusTarget, 0, routeData.paneId);
                    }).catch(function (reject) {
                        context.handleWrappedError(reject, null, function () { }, function () { });
                    });
                }
                else {
                    focusManager.focusOn(NakedObjects.FocusTarget.Menu, 0, routeData.paneId);
                }
            }).catch(function (reject) {
                context.handleWrappedError(reject, null, function () { }, function () { });
            });
        };
        handlers.handleList = function ($scope, routeData) {
            var cachedList = context.getCachedList(routeData.paneId, routeData.page, routeData.pageSize);
            var getActionExtensions = routeData.objectId ?
                function () { return context.getActionExtensionsFromObject(routeData.paneId, routeData.objectId, routeData.actionId); } :
                function () { return context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId); };
            if (cachedList) {
                $scope.listTemplate = routeData.state === NakedObjects.CollectionViewState.List ? NakedObjects.listTemplate : NakedObjects.listAsTableTemplate;
                var collectionViewModel = perPaneListViews[routeData.paneId];
                collectionViewModel.reset(cachedList, routeData);
                $scope.collection = collectionViewModel;
                $scope.actionsTemplate = routeData.actionsOpen ? NakedObjects.actionsTemplate : NakedObjects.nullTemplate;
                var focusTarget = routeData.actionsOpen ? NakedObjects.FocusTarget.SubAction : NakedObjects.FocusTarget.ListItem;
                if (routeData.dialogId) {
                    var actionViewModel_1 = _.find(collectionViewModel.actions, function (a) { return a.actionRep.actionId() === routeData.dialogId; });
                    context.getInvokableAction(actionViewModel_1.actionRep).then(function (details) {
                        actionViewModel_1.makeInvokable(details);
                        setDialog($scope, actionViewModel_1, routeData);
                    });
                    focusTarget = NakedObjects.FocusTarget.Dialog;
                }
                focusManager.focusOn(focusTarget, 0, routeData.paneId);
                getActionExtensions().then(function (ext) { return $scope.title = ext.friendlyName(); });
            }
            else {
                $scope.listTemplate = NakedObjects.listPlaceholderTemplate;
                $scope.collectionPlaceholder = viewModelFactory.listPlaceholderViewModel(routeData);
                getActionExtensions().then(function (ext) { return $scope.title = ext.friendlyName(); });
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
            else if (routeData.errorCategory === ErrorCategory.HttpClientError) {
                $scope.errorTemplate = NakedObjects.httpErrorTemplate;
            }
            else if (routeData.errorCategory === ErrorCategory.ClientError) {
                $scope.errorTemplate = NakedObjects.errorTemplate;
            }
            else if (routeData.errorCategory === ErrorCategory.HttpServerError) {
                $scope.errorTemplate = NakedObjects.errorTemplate;
            }
        };
        handlers.handleToolBar = function ($scope) {
            $scope.toolBar = viewModelFactory.toolBarViewModel();
        };
        handlers.handleObject = function ($scope, routeData) {
            var _a = routeData.objectId.split(NakedObjects.keySeparator), dt = _a[0], id = _a.slice(1);
            // to ease transition 
            $scope.objectTemplate = NakedObjects.blankTemplate;
            $scope.actionsTemplate = NakedObjects.nullTemplate;
            color.toColorNumberFromType(dt).then(function (c) {
                $scope.backgroundColor = "" + NakedObjects.objectColor + c;
            });
            deRegObject[routeData.paneId].deReg();
            context.getObject(routeData.paneId, dt, id, routeData.interactionMode).
                then(function (object) {
                var ovm = perPaneObjectViews[routeData.paneId].reset(object, routeData);
                $scope.object = ovm;
                if (routeData.interactionMode === NakedObjects.InteractionMode.Form) {
                    $scope.objectTemplate = NakedObjects.formTemplate;
                    $scope.actionsTemplate = NakedObjects.formActionsTemplate;
                }
                else if (routeData.interactionMode === NakedObjects.InteractionMode.View) {
                    $scope.objectTemplate = NakedObjects.objectViewTemplate;
                    $scope.actionsTemplate = routeData.actionsOpen ? NakedObjects.actionsTemplate : NakedObjects.nullTemplate;
                }
                else {
                    $scope.objectTemplate = NakedObjects.objectEditTemplate;
                    $scope.actionsTemplate = NakedObjects.nullTemplate;
                }
                $scope.collectionsTemplate = NakedObjects.collectionsTemplate;
                var focusTarget;
                if (routeData.dialogId) {
                    var action = object.actionMember(routeData.dialogId);
                    focusTarget = NakedObjects.FocusTarget.Dialog;
                    context.getInvokableAction(action).then(function (details) { return setDialog($scope, details, routeData); });
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
                focusManager.focusOn(focusTarget, 0, routeData.paneId);
                deRegObject[routeData.paneId].add($scope.$on("$locationChangeStart", ovm.setProperties));
                deRegObject[routeData.paneId].add($scope.$watch(function () { return $location.search(); }, ovm.setProperties, true));
                deRegObject[routeData.paneId].add($scope.$on("pane-swap", ovm.setProperties));
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
        };
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.handlers.js.map