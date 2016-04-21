/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.userMessages.config.ts" />
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var NakedObjects;
(function (NakedObjects) {
    var Value = NakedObjects.Models.Value;
    var EntryType = NakedObjects.Models.EntryType;
    var ErrorWrapper = NakedObjects.Models.ErrorWrapper;
    var ErrorMap = NakedObjects.Models.ErrorMap;
    var ErrorCategory = NakedObjects.Models.ErrorCategory;
    var HttpStatusCode = NakedObjects.Models.HttpStatusCode;
    var DateString = NakedObjects.Models.toDateString;
    function tooltip(onWhat, fields) {
        if (onWhat.clientValid()) {
            return "";
        }
        var missingMandatoryFields = _.filter(fields, function (p) { return !p.clientValid && !p.message; });
        if (missingMandatoryFields.length > 0) {
            return _.reduce(missingMandatoryFields, function (s, t) { return s + t.title + "; "; }, NakedObjects.mandatoryFieldsPrefix);
        }
        var invalidFields = _.filter(fields, function (p) { return !p.clientValid; });
        if (invalidFields.length > 0) {
            return _.reduce(invalidFields, function (s, t) { return s + t.title + "; "; }, NakedObjects.invalidFieldsPrefix);
        }
        return "";
    }
    function actionsTooltip(onWhat, actionsOpen) {
        if (actionsOpen) {
            return NakedObjects.closeActions;
        }
        return onWhat.disableActions() ? NakedObjects.noActions : NakedObjects.openActions;
    }
    function createActionSubmenuMap(avms, menu) {
        // if not root menu aggregate all actions with same name
        if (menu.name) {
            var actions = _.filter(avms, function (a) { return a.menuPath === menu.name; });
            menu.actions = actions;
        }
        return menu;
    }
    NakedObjects.createActionSubmenuMap = createActionSubmenuMap;
    function toTriStateBoolean(valueToSet) {
        // looks stupid but note type checking
        if (valueToSet === true || valueToSet === "true") {
            return true;
        }
        if (valueToSet === false || valueToSet === "false") {
            return false;
        }
        return null;
    }
    NakedObjects.toTriStateBoolean = toTriStateBoolean;
    function createActionMenuMap(avms) {
        // first create a menu for each action 
        var menus = _
            .chain(avms)
            .map(function (a) { return ({ name: a.menuPath, actions: [a] }); })
            .value();
        // remove non unique submenus 
        menus = _.uniqWith(menus, function (a, b) {
            if (a.name && b.name) {
                return a.name === b.name;
            }
            return false;
        });
        // update submenus with all actions under same submenu
        return _.map(menus, function (m) { return createActionSubmenuMap(avms, m); });
    }
    NakedObjects.createActionMenuMap = createActionMenuMap;
    var AttachmentViewModel = (function () {
        function AttachmentViewModel() {
        }
        AttachmentViewModel.create = function (href, mimeType, title) {
            var attachmentViewModel = new AttachmentViewModel();
            attachmentViewModel.href = href;
            attachmentViewModel.mimeType = mimeType;
            attachmentViewModel.title = title || NakedObjects.unknownFileTitle;
            return attachmentViewModel;
        };
        return AttachmentViewModel;
    }());
    NakedObjects.AttachmentViewModel = AttachmentViewModel;
    var ChoiceViewModel = (function () {
        function ChoiceViewModel() {
        }
        ChoiceViewModel.create = function (value, id, name, searchTerm) {
            var choiceViewModel = new ChoiceViewModel();
            choiceViewModel.wrapped = value;
            choiceViewModel.id = id;
            choiceViewModel.name = name || value.toString();
            choiceViewModel.value = value.isReference() ? value.link().href() : value.toValueString();
            choiceViewModel.search = searchTerm || choiceViewModel.name;
            choiceViewModel.isEnum = !value.isReference() && (choiceViewModel.name !== choiceViewModel.value);
            return choiceViewModel;
        };
        ChoiceViewModel.prototype.equals = function (other) {
            return this.id === other.id &&
                this.name === other.name &&
                this.value === other.value;
        };
        ChoiceViewModel.prototype.match = function (other) {
            var thisValue = this.isEnum ? this.value.trim() : this.search.trim();
            var otherValue = this.isEnum ? other.value.trim() : other.search.trim();
            return thisValue === otherValue;
        };
        return ChoiceViewModel;
    }());
    NakedObjects.ChoiceViewModel = ChoiceViewModel;
    var ErrorViewModel = (function () {
        function ErrorViewModel() {
        }
        return ErrorViewModel;
    }());
    NakedObjects.ErrorViewModel = ErrorViewModel;
    var LinkViewModel = (function () {
        function LinkViewModel() {
        }
        return LinkViewModel;
    }());
    NakedObjects.LinkViewModel = LinkViewModel;
    var ItemViewModel = (function (_super) {
        __extends(ItemViewModel, _super);
        function ItemViewModel() {
            _super.apply(this, arguments);
        }
        return ItemViewModel;
    }(LinkViewModel));
    NakedObjects.ItemViewModel = ItemViewModel;
    var RecentItemViewModel = (function (_super) {
        __extends(RecentItemViewModel, _super);
        function RecentItemViewModel() {
            _super.apply(this, arguments);
        }
        return RecentItemViewModel;
    }(ItemViewModel));
    NakedObjects.RecentItemViewModel = RecentItemViewModel;
    var MessageViewModel = (function () {
        function MessageViewModel() {
        }
        MessageViewModel.prototype.clearMessage = function () {
            this.message = "";
        };
        return MessageViewModel;
    }());
    NakedObjects.MessageViewModel = MessageViewModel;
    var ValueViewModel = (function (_super) {
        __extends(ValueViewModel, _super);
        function ValueViewModel() {
            _super.apply(this, arguments);
            this.isDirty = function () { return false; };
        }
        ValueViewModel.prototype.prompt = function (searchTerm) {
            return null;
        };
        ValueViewModel.prototype.conditionalChoices = function (args) {
            return null;
        };
        ValueViewModel.prototype.getMemento = function () {
            if (this.entryType === EntryType.Choices) {
                return (this.choice && this.choice.search) ? this.choice.search : this.getValue().toString();
            }
            if (this.entryType === EntryType.MultipleChoices) {
                var ss = _.map(this.multiChoices, function (c) {
                    return c.search;
                });
                if (ss.length === 0) {
                    return "";
                }
                return _.reduce(ss, function (m, s) {
                    return m + "-" + s;
                });
            }
            return this.getValue().toString();
        };
        ValueViewModel.prototype.setNewValue = function (newValue) {
            this.value = newValue.value;
            this.reference = newValue.reference;
            this.choice = newValue.choice;
            this.color = newValue.color;
        };
        ValueViewModel.prototype.clear = function () {
            this.value = null;
            this.reference = "";
            this.choice = null;
            this.color = "";
        };
        ValueViewModel.prototype.getValue = function () {
            if (this.entryType !== EntryType.FreeForm || this.isCollectionContributed) {
                if (this.entryType === EntryType.MultipleChoices || this.entryType === EntryType.MultipleConditionalChoices || this.isCollectionContributed) {
                    var selections = this.multiChoices || [];
                    if (this.type === "scalar") {
                        var selValues = _.map(selections, function (cvm) { return cvm.value; });
                        return new Value(selValues);
                    }
                    var selRefs = _.map(selections, function (cvm) { return ({ href: cvm.value, title: cvm.name }); }); // reference 
                    return new Value(selRefs);
                }
                if (this.type === "scalar") {
                    return new Value(this.choice && this.choice.value != null ? this.choice.value : "");
                }
                // reference 
                return new Value(this.choice && this.choice.value ? { href: this.choice.value, title: this.choice.name } : null);
            }
            if (this.type === "scalar") {
                if (this.value == null) {
                    return new Value("");
                }
                if (this.value instanceof Date) {
                    if (this.format === "date") {
                        // truncate time;
                        return new Value(DateString(this.value));
                    }
                    // date-time
                    return new Value(this.value.toISOString());
                }
                return new Value(this.value);
            }
            // reference
            return new Value(this.reference ? { href: this.reference, title: this.value.toString() } : null);
        };
        return ValueViewModel;
    }(MessageViewModel));
    NakedObjects.ValueViewModel = ValueViewModel;
    var ParameterViewModel = (function (_super) {
        __extends(ParameterViewModel, _super);
        function ParameterViewModel() {
            _super.apply(this, arguments);
        }
        return ParameterViewModel;
    }(ValueViewModel));
    NakedObjects.ParameterViewModel = ParameterViewModel;
    var ActionViewModel = (function () {
        function ActionViewModel() {
        }
        ActionViewModel.prototype.disabled = function () { return false; };
        return ActionViewModel;
    }());
    NakedObjects.ActionViewModel = ActionViewModel;
    var DialogViewModel = (function (_super) {
        __extends(DialogViewModel, _super);
        function DialogViewModel(color, context, viewModelFactory, urlManager, focusManager) {
            var _this = this;
            _super.call(this);
            this.color = color;
            this.context = context;
            this.viewModelFactory = viewModelFactory;
            this.urlManager = urlManager;
            this.focusManager = focusManager;
            this.actionMember = function () { return _this.actionViewModel.actionRep; };
            this.clientValid = function () { return _.every(_this.parameters, function (p) { return p.clientValid; }); };
            this.tooltip = function () { return tooltip(_this, _this.parameters); };
            this.setParms = function () { return _.forEach(_this.parameters, function (p) { return _this.urlManager.setFieldValue(_this.actionMember().actionId(), p.parameterRep, p.getValue(), _this.onPaneId); }); };
            this.executeInvoke = function (right) {
                var pps = _this.parameters;
                _.forEach(pps, function (p) { return _this.urlManager.setFieldValue(_this.actionMember().actionId(), p.parameterRep, p.getValue(), _this.onPaneId); });
                return _this.actionViewModel.executeInvoke(pps, right);
            };
            this.doInvoke = function (right) {
                return _this.executeInvoke(right).
                    then(function (actionResult) {
                    if (actionResult.shouldExpectResult()) {
                        _this.message = actionResult.warningsOrMessages() || NakedObjects.noResultMessage;
                    }
                    else if (actionResult.resultType() === "void") {
                        // dialog staying on same page so treat as cancel 
                        // for url replacing purposes
                        _this.doCancel();
                    }
                    else if (!right) {
                        // going to new page close dialog (and do not replace url)
                        _this.doClose();
                    }
                    // else leave open if opening on other pane and dialog has result
                }).
                    catch(function (reject) {
                    var parent = _this.actionMember().parent;
                    var display = function (em) { return _this.viewModelFactory.handleErrorResponse(em, _this, _this.parameters); };
                    _this.context.handleWrappedError(reject, parent, function () { }, display);
                });
            };
            this.doClose = function () { return _this.urlManager.closeDialog(_this.onPaneId); };
            this.doCancel = function () { return _this.urlManager.cancelDialog(_this.onPaneId); };
            this.clearMessages = function () {
                _this.message = "";
                _.each(_this.actionViewModel.parameters, function (parm) { return parm.clearMessage(); });
            };
        }
        DialogViewModel.prototype.reset = function (actionViewModel, routeData) {
            var _this = this;
            this.actionViewModel = actionViewModel;
            this.onPaneId = routeData.paneId;
            var fields = routeData.dialogFields;
            var parameters = _.filter(actionViewModel.parameters(), function (p) { return !p.isCollectionContributed; });
            this.parameters = _.map(parameters, function (p) { return _this.viewModelFactory.parameterViewModel(p.parameterRep, fields[p.parameterRep.id()], _this.onPaneId); });
            this.title = this.actionMember().extensions().friendlyName();
            this.isQueryOnly = this.actionMember().invokeLink().method() === "GET";
            this.message = "";
            return this;
        };
        DialogViewModel.prototype.isSame = function (paneId, otherAction) {
            return this.onPaneId === paneId && this.actionMember().invokeLink().href() === otherAction.invokeLink().href();
        };
        return DialogViewModel;
    }(MessageViewModel));
    NakedObjects.DialogViewModel = DialogViewModel;
    var PropertyViewModel = (function (_super) {
        __extends(PropertyViewModel, _super);
        function PropertyViewModel() {
            _super.apply(this, arguments);
        }
        PropertyViewModel.prototype.doClick = function (right) { };
        return PropertyViewModel;
    }(ValueViewModel));
    NakedObjects.PropertyViewModel = PropertyViewModel;
    var CollectionPlaceholderViewModel = (function () {
        function CollectionPlaceholderViewModel() {
        }
        return CollectionPlaceholderViewModel;
    }());
    NakedObjects.CollectionPlaceholderViewModel = CollectionPlaceholderViewModel;
    var ListViewModel = (function (_super) {
        __extends(ListViewModel, _super);
        function ListViewModel(colorService, contextService, viewModelFactory, urlManager, focusManager, $q) {
            var _this = this;
            _super.call(this);
            this.colorService = colorService;
            this.contextService = contextService;
            this.viewModelFactory = viewModelFactory;
            this.urlManager = urlManager;
            this.focusManager = focusManager;
            this.$q = $q;
            this.toggleActionMenu = function () {
                _this.focusManager.focusOverrideOff();
                _this.urlManager.toggleObjectMenu(_this.onPaneId);
            };
            this.recreate = function (page, pageSize) {
                return _this.routeData.objectId ?
                    _this.contextService.getListFromObject(_this.routeData.paneId, _this.routeData.objectId, _this.routeData.actionId, _this.routeData.actionParams, _this.routeData.state, page, pageSize) :
                    _this.contextService.getListFromMenu(_this.routeData.paneId, _this.routeData.menuId, _this.routeData.actionId, _this.routeData.actionParams, _this.routeData.state, page, pageSize);
            };
            this.pageOrRecreate = function (newPage, newPageSize, newState) {
                _this.recreate(newPage, newPageSize).
                    then(function (list) {
                    _this.routeData.state = newState || _this.routeData.state;
                    _this.reset(list, _this.routeData);
                    _this.urlManager.setListPaging(newPage, newPageSize, _this.routeData.state, _this.onPaneId);
                }).
                    catch(function (reject) {
                    var display = function (em) { return _this.message = em.invalidReason() || em.warningMessage; };
                    _this.contextService.handleWrappedError(reject, null, function () { }, display);
                });
            };
            this.setPage = function (newPage, newState) {
                _this.focusManager.focusOverrideOff();
                _this.pageOrRecreate(newPage, _this.pageSize, newState);
            };
            this.pageNext = function () { return _this.setPage(_this.page < _this.numPages ? _this.page + 1 : _this.page, _this.state); };
            this.pagePrevious = function () { return _this.setPage(_this.page > 1 ? _this.page - 1 : _this.page, _this.state); };
            this.pageFirst = function () { return _this.setPage(1, _this.state); };
            this.pageLast = function () { return _this.setPage(_this.numPages, _this.state); };
            this.earlierDisabled = function () { return _this.page === 1 || _this.numPages === 1; };
            this.laterDisabled = function () { return _this.page === _this.numPages || _this.numPages === 1; };
            this.pageFirstDisabled = this.earlierDisabled;
            this.pageLastDisabled = this.laterDisabled;
            this.pageNextDisabled = this.laterDisabled;
            this.pagePreviousDisabled = this.earlierDisabled;
            this.doSummary = function () { return _this.urlManager.setListState(NakedObjects.CollectionViewState.Summary, _this.onPaneId); };
            this.doList = function () { return _this.urlManager.setListState(NakedObjects.CollectionViewState.List, _this.onPaneId); };
            this.doTable = function () { return _this.urlManager.setListState(NakedObjects.CollectionViewState.Table, _this.onPaneId); };
            this.reload = function () {
                _this.contextService.clearCachedList(_this.onPaneId, _this.routeData.page, _this.routeData.pageSize);
                _this.setPage(_this.page, _this.state);
            };
            this.selectAll = function () { return _.each(_this.items, function (item, i) {
                item.selected = _this.allSelected;
                item.checkboxChange(i);
            }); };
            this.actionsTooltip = function () { return actionsTooltip(_this, !!_this.routeData.actionsOpen); };
        }
        ListViewModel.prototype.reset = function (list, routeData) {
            var _this = this;
            this.listRep = list;
            this.routeData = routeData;
            this.state = routeData.state;
            this.id = this.urlManager.getListCacheIndex(routeData.paneId, routeData.page, routeData.pageSize, routeData.state);
            this.onPaneId = routeData.paneId;
            this.pluralName = "Objects";
            this.page = this.listRep.pagination().page;
            this.pageSize = this.listRep.pagination().pageSize;
            this.numPages = this.listRep.pagination().numPages;
            var totalCount = this.listRep.pagination().totalCount;
            if (this.state === NakedObjects.CollectionViewState.Table) {
                this.recreate(this.page, this.pageSize).then(function (list) {
                    _this.items = _this.viewModelFactory.getItems(list.value(), _this.state === NakedObjects.CollectionViewState.Table, routeData, _this);
                    _this.allSelected = _.every(_this.items, function (item) { return item.selected; });
                    var count = _this.items.length;
                    _this.size = count;
                    _this.description = function () { return ("Page " + _this.page + " of " + _this.numPages + "; viewing " + count + " of " + totalCount + " items"); };
                });
            }
            else {
                this.items = this.viewModelFactory.getItems(list.value(), this.state === NakedObjects.CollectionViewState.Table, routeData, this);
                this.allSelected = _.every(this.items, function (item) { return item.selected; });
                var count_1 = this.items.length;
                this.size = count_1;
                this.description = function () { return ("Page " + _this.page + " of " + _this.numPages + "; viewing " + count_1 + " of " + totalCount + " items"); };
            }
            var actions = this.listRep.actionMembers();
            this.actions = _.map(actions, function (action) { return _this.viewModelFactory.actionViewModel(action, _this, routeData); });
            this.actionsMap = createActionMenuMap(this.actions);
            _.forEach(this.actions, function (a) {
                var wrappedInvoke = a.executeInvoke;
                a.executeInvoke = function (pps, right) {
                    var selected = _.filter(_this.items, function (i) { return i.selected; });
                    if (selected.length === 0) {
                        var em = new ErrorMap({}, 0, "Must select items for collection contributed action");
                        var rp = new ErrorWrapper(ErrorCategory.HttpClientError, HttpStatusCode.UnprocessableEntity, em);
                        return _this.$q.reject(rp);
                    }
                    var getParms = function (action) {
                        var parms = _.values(action.parameters());
                        var contribParm = _.find(parms, function (p) { return p.isCollectionContributed(); });
                        var parmValue = new Value(_.map(selected, function (i) { return i.link; }));
                        var collectionParmVm = _this.viewModelFactory.parameterViewModel(contribParm, parmValue, _this.onPaneId);
                        var allpps = _.clone(pps);
                        allpps.push(collectionParmVm);
                        return allpps;
                    };
                    if (a.actionRep.invokeLink()) {
                        return wrappedInvoke(getParms(a.actionRep), right);
                    }
                    return _this.contextService.getActionDetails(a.actionRep).
                        then(function (details) { return wrappedInvoke(getParms(details), right); });
                };
                // show dialog if more than 1 parm (single parm is collection itself)
                var showDialog = function () { return _this.contextService.getInvokableAction(a.actionRep).
                    then(function (ia) { return _.keys(ia.parameters()).length > 1; }); };
                a.doInvoke = function () { };
                showDialog().
                    then(function (show) { return a.doInvoke = show ?
                    function (right) {
                        _this.focusManager.focusOverrideOff();
                        _this.urlManager.setDialog(a.actionRep.actionId(), _this.onPaneId);
                    } :
                    function (right) {
                        a.executeInvoke([], right).
                            then(function (result) {
                            _this.message = result.shouldExpectResult() ? result.warningsOrMessages() || NakedObjects.noResultMessage : "";
                        }).
                            catch(function (reject) {
                            var display = function (em) { return _this.message = em.invalidReason() || em.warningMessage; };
                            _this.contextService.handleWrappedError(reject, null, function () { }, display);
                        });
                    }; });
            });
            return this;
        };
        ListViewModel.prototype.description = function () { return this.size ? this.size.toString() : ""; };
        ListViewModel.prototype.disableActions = function () {
            return !this.actions || this.actions.length === 0 || !this.items || this.items.length === 0;
        };
        ListViewModel.prototype.isSame = function (paneId, key) {
            return this.id === key;
        };
        return ListViewModel;
    }(MessageViewModel));
    NakedObjects.ListViewModel = ListViewModel;
    var CollectionViewModel = (function () {
        function CollectionViewModel() {
        }
        CollectionViewModel.prototype.doSummary = function () { };
        CollectionViewModel.prototype.doTable = function () { };
        CollectionViewModel.prototype.doList = function () { };
        CollectionViewModel.prototype.description = function () { return this.size.toString(); };
        return CollectionViewModel;
    }());
    NakedObjects.CollectionViewModel = CollectionViewModel;
    var ServicesViewModel = (function () {
        function ServicesViewModel() {
        }
        return ServicesViewModel;
    }());
    NakedObjects.ServicesViewModel = ServicesViewModel;
    var MenusViewModel = (function () {
        function MenusViewModel(viewModelFactory) {
            this.viewModelFactory = viewModelFactory;
        }
        MenusViewModel.prototype.reset = function (menusRep, routeData) {
            var _this = this;
            this.menusRep = menusRep;
            this.onPaneId = routeData.paneId;
            this.title = "Menus";
            this.color = "bg-color-darkBlue";
            this.items = _.map(this.menusRep.value(), function (link) { return _this.viewModelFactory.linkViewModel(link, _this.onPaneId); });
            return this;
        };
        return MenusViewModel;
    }());
    NakedObjects.MenusViewModel = MenusViewModel;
    var ServiceViewModel = (function (_super) {
        __extends(ServiceViewModel, _super);
        function ServiceViewModel() {
            _super.apply(this, arguments);
        }
        return ServiceViewModel;
    }(MessageViewModel));
    NakedObjects.ServiceViewModel = ServiceViewModel;
    var MenuViewModel = (function (_super) {
        __extends(MenuViewModel, _super);
        function MenuViewModel() {
            _super.apply(this, arguments);
        }
        return MenuViewModel;
    }(MessageViewModel));
    NakedObjects.MenuViewModel = MenuViewModel;
    var TableRowViewModel = (function () {
        function TableRowViewModel() {
        }
        return TableRowViewModel;
    }());
    NakedObjects.TableRowViewModel = TableRowViewModel;
    var DomainObjectViewModel = (function (_super) {
        __extends(DomainObjectViewModel, _super);
        function DomainObjectViewModel(colorService, contextService, viewModelFactory, urlManager, focusManager, $q) {
            var _this = this;
            _super.call(this);
            this.colorService = colorService;
            this.contextService = contextService;
            this.viewModelFactory = viewModelFactory;
            this.urlManager = urlManager;
            this.focusManager = focusManager;
            this.$q = $q;
            this.propertyMap = function () {
                var pps = _.filter(_this.properties, function (property) { return property.isEditable; });
                return _.zipObject(_.map(pps, function (p) { return p.id; }), _.map(pps, function (p) { return p.getValue(); }));
            };
            this.clientValid = function () { return _.every(_this.properties, function (p) { return p.clientValid; }); };
            this.tooltip = function () { return tooltip(_this, _this.properties); };
            this.actionsTooltip = function () { return actionsTooltip(_this, !!_this.routeData.actionsOpen); };
            this.toggleActionMenu = function () {
                _this.focusManager.focusOverrideOff();
                _this.urlManager.toggleObjectMenu(_this.onPaneId);
            };
            this.editProperties = function () { return _.filter(_this.properties, function (p) { return p.isEditable && p.isDirty(); }); };
            this.setProperties = function () {
                return _.forEach(_this.editProperties(), function (p) { return _this.urlManager.setPropertyValue(_this.domainObject, p.propertyRep, p.getValue(), _this.onPaneId); });
            };
            this.cancelHandler = function () { return _this.domainObject.extensions().interactionMode() === "form" || _this.domainObject.extensions().interactionMode() === "transient" ?
                function () { return _this.urlManager.popUrlState(_this.onPaneId); } :
                function () { return _this.urlManager.setInteractionMode(NakedObjects.InteractionMode.View, _this.onPaneId); }; };
            this.editComplete = function () {
                _this.setProperties();
            };
            this.doEditCancel = function () {
                _this.editComplete();
                _this.cancelHandler()();
            };
            this.saveHandler = function () { return _this.domainObject.isTransient() ? _this.contextService.saveObject : _this.contextService.updateObject; };
            this.validateHandler = function () { return _this.domainObject.isTransient() ? _this.contextService.validateSaveObject : _this.contextService.validateUpdateObject; };
            this.handleWrappedError = function (reject) {
                var reset = function (updatedObject) { return _this.reset(updatedObject, _this.urlManager.getRouteData().pane()[_this.onPaneId]); };
                var display = function (em) { return _this.viewModelFactory.handleErrorResponse(em, _this, _this.properties); };
                _this.contextService.handleWrappedError(reject, _this.domainObject, reset, display);
            };
            this.doSave = function (viewObject) {
                _this.setProperties();
                var propMap = _this.propertyMap();
                _this.saveHandler()(_this.domainObject, propMap, _this.onPaneId, viewObject).
                    catch(function (reject) { return _this.handleWrappedError(reject); });
            };
            this.doSaveValidate = function () {
                //this.setProperties();
                var propMap = _this.propertyMap();
                return _this.validateHandler()(_this.domainObject, propMap).
                    then(function () {
                    _this.message = "";
                    return true;
                }).
                    catch(function (reject) {
                    _this.handleWrappedError(reject);
                    return _this.$q.reject(false);
                });
            };
            this.doEdit = function () {
                _this.contextService.getObjectForEdit(_this.onPaneId, _this.domainObject).
                    then(function (updatedObject) {
                    _this.reset(updatedObject, _this.urlManager.getRouteData().pane()[_this.onPaneId]);
                    _this.urlManager.pushUrlState(_this.onPaneId);
                    _this.urlManager.setInteractionMode(NakedObjects.InteractionMode.Edit, _this.onPaneId);
                }).
                    catch(function (reject) { return _this.handleWrappedError(reject); });
            };
            this.doReload = function () {
                return _this.contextService.reloadObject(_this.onPaneId, _this.domainObject).
                    then(function (updatedObject) {
                    _this.reset(updatedObject, _this.urlManager.getRouteData().pane()[_this.onPaneId]);
                }).
                    catch(function (reject) { return _this.handleWrappedError(reject); });
            };
            this.hideEdit = function () { return _this.domainObject.extensions().interactionMode() === "form" ||
                _this.domainObject.extensions().interactionMode() === "transient" ||
                _.every(_this.properties, function (p) { return !p.isEditable; }); };
            this.canDropOn = function (targetType) { return _this.contextService.isSubTypeOf(targetType, _this.domainType); };
        }
        DomainObjectViewModel.prototype.reset = function (obj, routeData) {
            var _this = this;
            this.domainObject = obj;
            this.onPaneId = routeData.paneId;
            this.routeData = routeData;
            var iMode = this.domainObject.extensions().interactionMode();
            this.isInEdit = routeData.interactionMode !== NakedObjects.InteractionMode.View || iMode === "form" || iMode === "transient";
            this.props = routeData.interactionMode !== NakedObjects.InteractionMode.View ? routeData.props : {};
            var actions = _.values(this.domainObject.actionMembers());
            this.actions = _.map(actions, function (action) { return _this.viewModelFactory.actionViewModel(action, _this, _this.routeData); });
            this.actionsMap = createActionMenuMap(this.actions);
            this.properties = _.map(this.domainObject.propertyMembers(), function (property, id) { return _this.viewModelFactory.propertyViewModel(property, id, _this.props[id], _this.onPaneId, _this.propertyMap); });
            this.collections = _.map(this.domainObject.collectionMembers(), function (collection) { return _this.viewModelFactory.collectionViewModel(collection, _this.routeData); });
            this.unsaved = routeData.interactionMode === NakedObjects.InteractionMode.Transient;
            this.title = this.unsaved ? "Unsaved " + this.domainObject.extensions().friendlyName() : this.domainObject.title();
            this.friendlyName = this.domainObject.extensions().friendlyName();
            this.domainType = this.domainObject.domainType();
            this.instanceId = this.domainObject.instanceId();
            this.draggableType = this.domainObject.domainType();
            var selfAsValue = function () {
                var link = _this.domainObject.selfLink();
                if (link) {
                    // not transient - can't drag transients so no need to set up IDraggable members on transients
                    link.setTitle(_this.title);
                    return new Value(link);
                }
                return null;
            };
            var sav = selfAsValue();
            this.value = sav ? sav.toString() : "";
            this.reference = sav ? sav.toValueString() : "";
            this.choice = sav ? ChoiceViewModel.create(sav, "") : null;
            this.colorService.toColorNumberFromType(this.domainObject.domainType()).then(function (c) {
                _this.color = "" + NakedObjects.objectColor + c;
            });
            this.message = "";
            if (routeData.interactionMode === NakedObjects.InteractionMode.Form) {
                _.forEach(this.actions, function (a) {
                    var wrappedInvoke = a.executeInvoke;
                    a.executeInvoke = function (pps, right) {
                        _this.setProperties();
                        var pairs = _.map(_this.editProperties(), function (p) { return [p.id, p.getValue()]; });
                        var prps = _.fromPairs(pairs);
                        var parmValueMap = _.mapValues(a.actionRep.parameters(), function (p) { return ({ parm: p, value: prps[p.id()] }); });
                        var allpps = _.map(parmValueMap, function (o) { return _this.viewModelFactory.parameterViewModel(o.parm, o.value, _this.onPaneId); });
                        return wrappedInvoke(allpps, right).
                            catch(function (reject) {
                            _this.handleWrappedError(reject);
                            return _this.$q.reject(reject);
                        });
                    };
                });
            }
            return this;
        };
        DomainObjectViewModel.prototype.disableActions = function () {
            return !this.actions || this.actions.length === 0;
        };
        return DomainObjectViewModel;
    }(MessageViewModel));
    NakedObjects.DomainObjectViewModel = DomainObjectViewModel;
    var ToolBarViewModel = (function () {
        function ToolBarViewModel() {
        }
        return ToolBarViewModel;
    }());
    NakedObjects.ToolBarViewModel = ToolBarViewModel;
    var RecentItemsViewModel = (function () {
        function RecentItemsViewModel() {
        }
        return RecentItemsViewModel;
    }());
    NakedObjects.RecentItemsViewModel = RecentItemsViewModel;
    var CiceroViewModel = (function () {
        function CiceroViewModel() {
            this.alert = ""; //Alert is appended before the output
        }
        CiceroViewModel.prototype.selectPreviousInput = function () {
            this.input = this.previousInput;
        };
        CiceroViewModel.prototype.clearInput = function () {
            this.input = null;
        };
        CiceroViewModel.prototype.outputMessageThenClearIt = function () {
            this.output = this.message;
            this.message = null;
        };
        CiceroViewModel.prototype.popNextCommand = function () {
            if (this.chainedCommands) {
                var next = this.chainedCommands[0];
                this.chainedCommands.splice(0, 1);
                return next;
            }
            return null;
        };
        CiceroViewModel.prototype.clearInputRenderOutputAndAppendAlertIfAny = function (output) {
            this.clearInput();
            this.output = output;
            if (this.alert) {
                this.output += this.alert;
                this.alert = "";
            }
        };
        return CiceroViewModel;
    }());
    NakedObjects.CiceroViewModel = CiceroViewModel;
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.viewmodels.js.map