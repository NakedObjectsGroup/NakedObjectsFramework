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
    var dirtyMarker = NakedObjects.Models.dirtyMarker;
    var toTimeString = NakedObjects.Models.toTimeString;
    function tooltip(onWhat, fields) {
        if (onWhat.clientValid()) {
            return "";
        }
        var missingMandatoryFields = _.filter(fields, function (p) { return !p.clientValid && !p.getMessage(); });
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
    function getMenuForLevel(menupath, level) {
        var menu = "";
        if (menupath && menupath.length > 0) {
            var menus = menupath.split("_");
            if (menus.length > level) {
                menu = menus[level];
            }
        }
        return menu || "";
    }
    function removeDuplicateMenus(menus) {
        return _.uniqWith(menus, function (m1, m2) {
            if (m1.name && m2.name) {
                return m1.name === m2.name;
            }
            return false;
        });
    }
    function createSubmenuItems(avms, menu, level) {
        // if not root menu aggregate all actions with same name
        if (menu.name) {
            var actions = _.filter(avms, function (a) { return getMenuForLevel(a.menuPath, level) === menu.name && !getMenuForLevel(a.menuPath, level + 1); });
            menu.actions = actions;
            //then collate submenus 
            var submenuActions_1 = _.filter(avms, function (a) { return getMenuForLevel(a.menuPath, level) === menu.name && getMenuForLevel(a.menuPath, level + 1); });
            var menus = _
                .chain(submenuActions_1)
                .map(function (a) { return new MenuItemViewModel(getMenuForLevel(a.menuPath, level + 1), null, null); })
                .value();
            menus = removeDuplicateMenus(menus);
            menu.menuItems = _.map(menus, function (m) { return createSubmenuItems(submenuActions_1, m, level + 1); });
        }
        return menu;
    }
    NakedObjects.createSubmenuItems = createSubmenuItems;
    function createMenuItems(avms) {
        // first create a top level menu for each action 
        // note at top level we leave 'un-menued' actions
        var menus = _
            .chain(avms)
            .map(function (a) { return new MenuItemViewModel(getMenuForLevel(a.menuPath, 0), [a], null); })
            .value();
        // remove non unique submenus 
        menus = removeDuplicateMenus(menus);
        // update submenus with all actions under same submenu
        return _.map(menus, function (m) { return createSubmenuItems(avms, m, 0); });
    }
    NakedObjects.createMenuItems = createMenuItems;
    var AttachmentViewModel = (function () {
        function AttachmentViewModel() {
            var _this = this;
            this.downloadFile = function () { return _this.context.getFile(_this.parent, _this.href, _this.mimeType); };
            this.clearCachedFile = function () { return _this.context.clearCachedFile(_this.href); };
            this.displayInline = function () {
                return _this.mimeType === "image/jpeg" ||
                    _this.mimeType === "image/gif" ||
                    _this.mimeType === "application/octet-stream";
            };
        }
        AttachmentViewModel.create = function (attachmentLink, parent, context, paneId) {
            var attachmentViewModel = new AttachmentViewModel();
            attachmentViewModel.link = attachmentLink;
            attachmentViewModel.href = attachmentLink.href();
            attachmentViewModel.mimeType = attachmentLink.type().asString;
            attachmentViewModel.title = attachmentLink.title() || NakedObjects.unknownFileTitle;
            attachmentViewModel.parent = parent;
            attachmentViewModel.context = context;
            attachmentViewModel.onPaneId = paneId;
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
            choiceViewModel.search = searchTerm || choiceViewModel.name;
            choiceViewModel.isEnum = !value.isReference() && (choiceViewModel.name !== choiceViewModel.getValue().toValueString());
            return choiceViewModel;
        };
        ChoiceViewModel.prototype.getValue = function () {
            return this.wrapped;
        };
        ChoiceViewModel.prototype.equals = function (other) {
            return other instanceof ChoiceViewModel &&
                this.id === other.id &&
                this.name === other.name &&
                this.wrapped.toValueString() === other.wrapped.toValueString();
        };
        ChoiceViewModel.prototype.valuesEqual = function (other) {
            if (other instanceof ChoiceViewModel) {
                var thisValue = this.isEnum ? this.wrapped.toValueString().trim() : this.search.trim();
                var otherValue = this.isEnum ? other.wrapped.toValueString().trim() : other.search.trim();
                return thisValue === otherValue;
            }
            return false;
        };
        ChoiceViewModel.prototype.toString = function () {
            return this.name;
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
    }(LinkViewModel));
    NakedObjects.RecentItemViewModel = RecentItemViewModel;
    var MessageViewModel = (function () {
        function MessageViewModel() {
            var _this = this;
            this.previousMessage = "";
            this.message = "";
            this.clearMessage = function () {
                if (_this.message === _this.previousMessage) {
                    _this.resetMessage();
                }
                else {
                    _this.previousMessage = _this.message;
                }
            };
            this.resetMessage = function () { return _this.message = _this.previousMessage = ""; };
            this.setMessage = function (msg) { return _this.message = msg; };
            this.getMessage = function () { return _this.message; };
        }
        return MessageViewModel;
    }());
    var ValueViewModel = (function (_super) {
        __extends(ValueViewModel, _super);
        function ValueViewModel(ext, color, error) {
            _super.call(this);
            this.clientValid = true;
            this.reference = "";
            this.choices = [];
            this.optional = ext.optional();
            this.description = ext.description();
            this.presentationHint = ext.presentationHint();
            this.mask = ext.mask();
            this.title = ext.friendlyName();
            this.returnType = ext.returnType();
            this.format = ext.format();
            this.multipleLines = ext.multipleLines() || 1;
            this.password = ext.dataType() === "password";
            this.updateColor = _.partial(this.setColor, color);
            this.error = error;
        }
        Object.defineProperty(ValueViewModel.prototype, "selectedChoice", {
            get: function () {
                return this.currentChoice;
            },
            set: function (newChoice) {
                // type guard because angular pushes string value here until directive finds 
                // choice
                if (newChoice instanceof ChoiceViewModel || newChoice == null) {
                    this.currentChoice = newChoice;
                    this.update();
                }
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(ValueViewModel.prototype, "value", {
            get: function () {
                return this.currentRawValue;
            },
            set: function (newValue) {
                this.currentRawValue = newValue;
                this.update();
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(ValueViewModel.prototype, "selectedMultiChoices", {
            get: function () {
                return this.currentMultipleChoices;
            },
            set: function (choices) {
                this.currentMultipleChoices = choices;
                this.update();
            },
            enumerable: true,
            configurable: true
        });
        ValueViewModel.prototype.setNewValue = function (newValue) {
            this.selectedChoice = newValue.selectedChoice;
            this.value = newValue.value;
            this.reference = newValue.reference;
        };
        ValueViewModel.prototype.clear = function () {
            this.selectedChoice = null;
            this.value = null;
            this.reference = "";
        };
        ValueViewModel.prototype.update = function () {
            this.updateColor();
        };
        ValueViewModel.prototype.setColor = function (color) {
            var _this = this;
            if (this.entryType === EntryType.AutoComplete && this.selectedChoice && this.type === "ref") {
                var href = this.selectedChoice.getValue().href();
                if (href) {
                    color.toColorNumberFromHref(href).
                        then(function (c) { return _this.color = "" + NakedObjects.linkColor + c; }).
                        catch(function (reject) { return _this.error.handleError(reject); });
                    return;
                }
            }
            else if (this.entryType !== EntryType.AutoComplete && this.value) {
                color.toColorNumberFromType(this.returnType).
                    then(function (c) { return _this.color = "" + NakedObjects.linkColor + c; }).
                    catch(function (reject) { return _this.error.handleError(reject); });
                return;
            }
            this.color = "";
        };
        ValueViewModel.prototype.getValue = function () {
            if (this.entryType === EntryType.File) {
                return new Value(this.file);
            }
            if (this.entryType !== EntryType.FreeForm || this.isCollectionContributed) {
                if (this.entryType === EntryType.MultipleChoices || this.entryType === EntryType.MultipleConditionalChoices || this.isCollectionContributed) {
                    var selections = this.selectedMultiChoices || [];
                    if (this.type === "scalar") {
                        var selValues = _.map(selections, function (cvm) { return cvm.getValue().scalar(); });
                        return new Value(selValues);
                    }
                    var selRefs = _.map(selections, function (cvm) { return ({ href: cvm.getValue().href(), title: cvm.name }); }); // reference 
                    return new Value(selRefs);
                }
                var choiceValue = this.selectedChoice ? this.selectedChoice.getValue() : null;
                if (this.type === "scalar") {
                    return new Value(choiceValue && choiceValue.scalar() != null ? choiceValue.scalar() : "");
                }
                // reference 
                return new Value(choiceValue && choiceValue.isReference() ? { href: choiceValue.href(), title: this.selectedChoice.name } : null);
            }
            if (this.type === "scalar") {
                if (this.value == null) {
                    return new Value("");
                }
                if (this.value instanceof Date) {
                    if (this.format === "time") {
                        // time format
                        return new Value(toTimeString(this.value));
                    }
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
    var ParameterViewModel = (function (_super) {
        __extends(ParameterViewModel, _super);
        function ParameterViewModel(parmRep, paneId, color, error) {
            _super.call(this, parmRep.extensions(), color, error);
            this.parmRep = parmRep;
            this.parameterRep = parmRep;
            this.onPaneId = paneId;
            this.type = parmRep.isScalar() ? "scalar" : "ref";
            this.dflt = parmRep.default().toString();
            this.id = parmRep.id();
            this.argId = "" + this.id.toLowerCase();
            this.paneArgId = "" + this.argId + this.onPaneId;
            this.isCollectionContributed = parmRep.isCollectionContributed();
            this.entryType = parmRep.entryType();
        }
        ParameterViewModel.prototype.setAsRow = function (i) {
            this.paneArgId = "" + this.argId + i;
        };
        ParameterViewModel.prototype.update = function () {
            _super.prototype.update.call(this);
            switch (this.entryType) {
                case (EntryType.FreeForm):
                    if (this.type === "scalar") {
                        if (this.localFilter) {
                            this.formattedValue = this.value ? this.localFilter.filter(this.value) : "";
                        }
                        else {
                            this.formattedValue = this.value ? this.value.toString() : "";
                        }
                        break;
                    }
                // fall through 
                case (EntryType.AutoComplete):
                case (EntryType.Choices):
                case (EntryType.ConditionalChoices):
                    this.formattedValue = this.selectedChoice ? this.selectedChoice.toString() : "";
                    break;
                case (EntryType.MultipleChoices):
                case (EntryType.MultipleConditionalChoices):
                    var count = !this.selectedMultiChoices ? 0 : this.selectedMultiChoices.length;
                    this.formattedValue = count + " selected";
                    break;
                default:
                    this.formattedValue = this.value ? this.value.toString() : "";
            }
        };
        return ParameterViewModel;
    }(ValueViewModel));
    NakedObjects.ParameterViewModel = ParameterViewModel;
    var ActionViewModel = (function () {
        function ActionViewModel() {
            this.gotoResult = true;
        }
        return ActionViewModel;
    }());
    NakedObjects.ActionViewModel = ActionViewModel;
    var MenuItemViewModel = (function () {
        function MenuItemViewModel(name, actions, menuItems) {
            this.name = name;
            this.actions = actions;
            this.menuItems = menuItems;
        }
        return MenuItemViewModel;
    }());
    NakedObjects.MenuItemViewModel = MenuItemViewModel;
    var DialogViewModel = (function (_super) {
        __extends(DialogViewModel, _super);
        function DialogViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $rootScope) {
            var _this = this;
            _super.call(this);
            this.color = color;
            this.context = context;
            this.viewModelFactory = viewModelFactory;
            this.urlManager = urlManager;
            this.focusManager = focusManager;
            this.error = error;
            this.$rootScope = $rootScope;
            this.actionMember = function () { return _this.actionViewModel.actionRep; };
            this.execute = function (right) {
                var pps = _this.parameters;
                _this.context.updateValues();
                return _this.actionViewModel.execute(pps, right);
            };
            this.parameters = [];
            this.submitted = false;
            this.clientValid = function () { return _.every(_this.parameters, function (p) { return p.clientValid; }); };
            this.tooltip = function () { return tooltip(_this, _this.parameters); };
            this.setParms = function () { return _.forEach(_this.parameters, function (p) { return _this.context.setFieldValue(_this.actionMember().actionId(), p.parameterRep.id(), p.getValue(), _this.onPaneId); }); };
            this.doInvoke = function (right) {
                return _this.execute(right).
                    then(function (actionResult) {
                    if (actionResult.shouldExpectResult()) {
                        _this.setMessage(actionResult.warningsOrMessages() || NakedObjects.noResultMessage);
                    }
                    else if (actionResult.resultType() === "void") {
                        // dialog staying on same page so treat as cancel 
                        // for url replacing purposes
                        _this.doCloseReplaceHistory();
                    }
                    else if (!_this.isQueryOnly) {
                        // not query only - always close
                        _this.doCloseReplaceHistory();
                    }
                    else if (!right) {
                        // query only going to new page close dialog and keep history
                        _this.doCloseKeepHistory();
                    }
                    // else query only going to other tab leave dialog open
                    _this.doCompleteButLeaveOpen();
                }).
                    catch(function (reject) {
                    var display = function (em) { return _this.viewModelFactory.handleErrorResponse(em, _this, _this.parameters); };
                    _this.error.handleErrorAndDisplayMessages(reject, display);
                });
            };
            this.doCloseKeepHistory = function () {
                _this.deregister();
                _this.urlManager.closeDialogKeepHistory(_this.onPaneId);
            };
            this.doCloseReplaceHistory = function () {
                _this.deregister();
                _this.urlManager.closeDialogReplaceHistory(_this.onPaneId);
            };
            this.doCompleteButLeaveOpen = function () {
            };
            this.clearMessages = function () {
                _this.resetMessage();
                _.each(_this.actionViewModel.parameters, function (parm) { return parm.clearMessage(); });
            };
        }
        DialogViewModel.prototype.reset = function (actionViewModel, paneId) {
            var _this = this;
            this.actionViewModel = actionViewModel;
            this.onPaneId = paneId;
            var fields = this.context.getCurrentDialogValues(this.actionMember().actionId(), this.onPaneId);
            var parameters = _.pickBy(actionViewModel.invokableActionRep.parameters(), function (p) { return !p.isCollectionContributed(); });
            this.parameters = _.map(parameters, function (p) { return _this.viewModelFactory.parameterViewModel(p, fields[p.id()], _this.onPaneId); });
            this.title = this.actionMember().extensions().friendlyName();
            this.isQueryOnly = actionViewModel.invokableActionRep.invokeLink().method() === "GET";
            this.resetMessage();
            this.id = actionViewModel.actionRep.actionId();
            this.submitted = false;
        };
        DialogViewModel.prototype.refresh = function () {
            var fields = this.context.getCurrentDialogValues(this.actionMember().actionId(), this.onPaneId);
            _.forEach(this.parameters, function (p) { return p.refresh(fields[p.id]); });
        };
        return DialogViewModel;
    }(MessageViewModel));
    NakedObjects.DialogViewModel = DialogViewModel;
    var MultiLineDialogViewModel = (function () {
        function MultiLineDialogViewModel(color, context, viewModelFactory, urlManager, focusManager, error, $rootScope) {
            this.color = color;
            this.context = context;
            this.viewModelFactory = viewModelFactory;
            this.urlManager = urlManager;
            this.focusManager = focusManager;
            this.error = error;
            this.$rootScope = $rootScope;
            this.title = "";
            this.dialogs = [];
        }
        MultiLineDialogViewModel.prototype.createRow = function (i) {
            var dialogViewModel = new DialogViewModel(this.color, this.context, this.viewModelFactory, this.urlManager, this.focusManager, this.error, this.$rootScope);
            var actionViewModel = this.viewModelFactory.actionViewModel(this.action, dialogViewModel, this.routeData);
            actionViewModel.gotoResult = false;
            dialogViewModel.reset(actionViewModel, 1);
            dialogViewModel.doCloseKeepHistory = function () {
                dialogViewModel.submitted = true;
            };
            dialogViewModel.doCloseReplaceHistory = function () {
                dialogViewModel.submitted = true;
            };
            dialogViewModel.doCompleteButLeaveOpen = function () {
                dialogViewModel.submitted = true;
            };
            dialogViewModel.parameters.forEach(function (p) { return p.setAsRow(i); });
            return dialogViewModel;
        };
        MultiLineDialogViewModel.prototype.reset = function (routeData, action) {
            var _this = this;
            this.action = action;
            this.routeData = routeData;
            this.action.parent.etagDigest = "*";
            var initialCount = action.extensions().multipleLines() || 1;
            this.dialogs = _.map(_.range(initialCount), function (i) { return _this.createRow(i); });
            this.title = this.dialogs[0].title;
            return this;
        };
        MultiLineDialogViewModel.prototype.header = function () {
            return this.dialogs.length === 0 ? [] : _.map(this.dialogs[0].parameters, function (p) { return p.title; });
        };
        MultiLineDialogViewModel.prototype.clientValid = function () {
            return _.every(this.dialogs, function (d) { return d.clientValid(); });
        };
        MultiLineDialogViewModel.prototype.tooltip = function () {
            var tooltips = _.map(this.dialogs, function (d, i) { return ("row: " + i + " " + (d.tooltip() || "OK")); });
            return _.reduce(tooltips, function (s, t) { return (s + "\n" + t); });
        };
        MultiLineDialogViewModel.prototype.invokeAndAdd = function (index) {
            this.dialogs[index].doInvoke();
            this.add(index);
            this.focusManager.focusOn(NakedObjects.FocusTarget.MultiLineDialogRow, 1, 1);
        };
        MultiLineDialogViewModel.prototype.add = function (index) {
            if (index === this.dialogs.length - 1) {
                // if this is last dialog always add another
                this.dialogs.push(this.createRow(this.dialogs.length));
            }
            else if (_.takeRight(this.dialogs)[0].submitted) {
                // if the last dialog is submitted add another 
                this.dialogs.push(this.createRow(this.dialogs.length));
            }
        };
        MultiLineDialogViewModel.prototype.clear = function (index) {
            _.pullAt(this.dialogs, [index]);
        };
        MultiLineDialogViewModel.prototype.submitAll = function () {
            if (this.clientValid()) {
                _.each(this.dialogs, function (d) {
                    if (!d.submitted) {
                        d.doInvoke();
                    }
                });
            }
        };
        MultiLineDialogViewModel.prototype.submittedCount = function () {
            return (_.filter(this.dialogs, function (d) { return d.submitted; })).length;
        };
        MultiLineDialogViewModel.prototype.close = function () {
            this.urlManager.popUrlState();
        };
        return MultiLineDialogViewModel;
    }());
    NakedObjects.MultiLineDialogViewModel = MultiLineDialogViewModel;
    var PropertyViewModel = (function (_super) {
        __extends(PropertyViewModel, _super);
        function PropertyViewModel(propertyRep, color, error) {
            _super.call(this, propertyRep.extensions(), color, error);
            this.draggableType = propertyRep.extensions().returnType();
            this.propertyRep = propertyRep;
            this.entryType = propertyRep.entryType();
            this.isEditable = !propertyRep.disabledReason();
            this.entryType = propertyRep.entryType();
        }
        return PropertyViewModel;
    }(ValueViewModel));
    NakedObjects.PropertyViewModel = PropertyViewModel;
    var CollectionPlaceholderViewModel = (function () {
        function CollectionPlaceholderViewModel() {
        }
        return CollectionPlaceholderViewModel;
    }());
    NakedObjects.CollectionPlaceholderViewModel = CollectionPlaceholderViewModel;
    var ContributedActionParentViewModel = (function (_super) {
        __extends(ContributedActionParentViewModel, _super);
        function ContributedActionParentViewModel(context, viewModelFactory, urlManager, focusManager, error, $q) {
            var _this = this;
            _super.call(this);
            this.context = context;
            this.viewModelFactory = viewModelFactory;
            this.urlManager = urlManager;
            this.focusManager = focusManager;
            this.error = error;
            this.$q = $q;
            this.selectAll = function () { return _.each(_this.items, function (item, i) {
                item.selected = _this.allSelected;
                item.selectionChange(i);
            }); };
        }
        ContributedActionParentViewModel.prototype.isLocallyContributed = function (action) {
            return _.some(action.parameters(), function (p) { return p.isCollectionContributed(); });
        };
        ContributedActionParentViewModel.prototype.collectionContributedActionDecorator = function (actionViewModel) {
            var _this = this;
            var wrappedInvoke = actionViewModel.execute;
            actionViewModel.execute = function (pps, right) {
                var selected = _.filter(_this.items, function (i) { return i.selected; });
                var rejectAsNeedSelection = function (action) {
                    if (_this.isLocallyContributed(action)) {
                        if (selected.length === 0) {
                            var em = new ErrorMap({}, 0, NakedObjects.noItemsSelected);
                            var rp = new ErrorWrapper(ErrorCategory.HttpClientError, HttpStatusCode.UnprocessableEntity, em);
                            return rp;
                        }
                    }
                    return null;
                };
                var getParms = function (action) {
                    var parms = _.values(action.parameters());
                    var contribParm = _.find(parms, function (p) { return p.isCollectionContributed(); });
                    if (contribParm) {
                        var parmValue = new Value(_.map(selected, function (i) { return i.link; }));
                        var collectionParmVm = _this.viewModelFactory.parameterViewModel(contribParm, parmValue, _this.onPaneId);
                        var allpps = _.clone(pps);
                        allpps.push(collectionParmVm);
                        return allpps;
                    }
                    return pps;
                };
                if (actionViewModel.invokableActionRep) {
                    var rp = rejectAsNeedSelection(actionViewModel.invokableActionRep);
                    return rp ? _this.$q.reject(rp) : wrappedInvoke(getParms(actionViewModel.invokableActionRep), right).
                        then(function (result) {
                        // clear selected items on void actions 
                        _this.clearSelected(result);
                        return result;
                    });
                }
                return _this.context.getActionDetails(actionViewModel.actionRep).
                    then(function (details) {
                    var rp = rejectAsNeedSelection(details);
                    if (rp) {
                        return _this.$q.reject(rp);
                    }
                    return wrappedInvoke(getParms(details), right);
                }).
                    then(function (result) {
                    // clear selected items on void actions 
                    _this.clearSelected(result);
                    return result;
                });
            };
        };
        ContributedActionParentViewModel.prototype.collectionContributedInvokeDecorator = function (actionViewModel) {
            var _this = this;
            var showDialog = function () {
                return _this.context.getInvokableAction(actionViewModel.actionRep).
                    then(function (invokableAction) {
                    var keyCount = _.keys(invokableAction.parameters()).length;
                    return keyCount > 1 || keyCount === 1 && !_.toArray(invokableAction.parameters())[0].isCollectionContributed();
                });
            };
            // make sure not null while waiting for promise to assign correct function 
            actionViewModel.doInvoke = function () { };
            var invokeWithDialog = function (right) {
                _this.context.clearDialogValues(_this.onPaneId);
                _this.focusManager.focusOverrideOff();
                _this.urlManager.setDialogOrMultiLineDialog(actionViewModel.actionRep, _this.onPaneId);
            };
            var invokeWithoutDialog = function (right) {
                return actionViewModel.execute([], right).
                    then(function (result) {
                    _this.setMessage(result.shouldExpectResult() ? result.warningsOrMessages() || NakedObjects.noResultMessage : "");
                    // clear selected items on void actions 
                    _this.clearSelected(result);
                }).
                    catch(function (reject) {
                    var display = function (em) { return _this.setMessage(em.invalidReason() || em.warningMessage); };
                    _this.error.handleErrorAndDisplayMessages(reject, display);
                });
            };
            showDialog().
                then(function (show) { return actionViewModel.doInvoke = show ? invokeWithDialog : invokeWithoutDialog; }).
                catch(function (reject) { return _this.error.handleError(reject); });
        };
        ContributedActionParentViewModel.prototype.decorate = function (actionViewModel) {
            this.collectionContributedActionDecorator(actionViewModel);
            this.collectionContributedInvokeDecorator(actionViewModel);
        };
        ContributedActionParentViewModel.prototype.clearSelected = function (result) {
            if (result.resultType() === "void") {
                this.allSelected = false;
                this.selectAll();
            }
        };
        return ContributedActionParentViewModel;
    }(MessageViewModel));
    var ListViewModel = (function (_super) {
        __extends(ListViewModel, _super);
        function ListViewModel(colorService, context, viewModelFactory, urlManager, focusManager, error, $q) {
            var _this = this;
            _super.call(this, context, viewModelFactory, urlManager, focusManager, error, $q);
            this.colorService = colorService;
            this.recreate = function (page, pageSize) {
                return _this.routeData.objectId ?
                    _this.context.getListFromObject(_this.routeData.paneId, _this.routeData, page, pageSize) :
                    _this.context.getListFromMenu(_this.routeData.paneId, _this.routeData, page, pageSize);
            };
            this.pageOrRecreate = function (newPage, newPageSize, newState) {
                _this.recreate(newPage, newPageSize).
                    then(function (list) {
                    _this.urlManager.setListPaging(newPage, newPageSize, newState || _this.routeData.state, _this.onPaneId);
                    _this.routeData = _this.urlManager.getRouteData().pane()[_this.onPaneId];
                    _this.reset(list, _this.routeData);
                }).
                    catch(function (reject) {
                    var display = function (em) { return _this.setMessage(em.invalidReason() || em.warningMessage); };
                    _this.error.handleErrorAndDisplayMessages(reject, display);
                });
            };
            this.setPage = function (newPage, newState) {
                _this.context.updateValues();
                _this.focusManager.focusOverrideOff();
                _this.pageOrRecreate(newPage, _this.pageSize, newState);
            };
            this.earlierDisabled = function () { return _this.page === 1 || _this.numPages === 1; };
            this.laterDisabled = function () { return _this.page === _this.numPages || _this.numPages === 1; };
            this.pageFirstDisabled = this.earlierDisabled;
            this.pageLastDisabled = this.laterDisabled;
            this.pageNextDisabled = this.laterDisabled;
            this.pagePreviousDisabled = this.earlierDisabled;
            this.hasTableData = function () {
                var valueLinks = _this.listRep.value();
                return valueLinks && _.some(valueLinks, function (i) { return i.members(); });
            };
            this.toggleActionMenu = function () {
                _this.focusManager.focusOverrideOff();
                _this.urlManager.toggleObjectMenu(_this.onPaneId);
            };
            this.pageNext = function () { return _this.setPage(_this.page < _this.numPages ? _this.page + 1 : _this.page, _this.state); };
            this.pagePrevious = function () { return _this.setPage(_this.page > 1 ? _this.page - 1 : _this.page, _this.state); };
            this.pageFirst = function () { return _this.setPage(1, _this.state); };
            this.pageLast = function () { return _this.setPage(_this.numPages, _this.state); };
            this.doSummary = function () {
                _this.context.updateValues();
                _this.urlManager.setListState(NakedObjects.CollectionViewState.Summary, _this.onPaneId);
            };
            this.doList = function () {
                _this.context.updateValues();
                _this.urlManager.setListState(NakedObjects.CollectionViewState.List, _this.onPaneId);
            };
            this.doTable = function () {
                _this.context.updateValues();
                _this.urlManager.setListState(NakedObjects.CollectionViewState.Table, _this.onPaneId);
            };
            this.reload = function () {
                _this.allSelected = false;
                _this.selectAll();
                _this.context.clearCachedList(_this.onPaneId, _this.routeData.page, _this.routeData.pageSize);
                _this.setPage(_this.page, _this.state);
            };
            this.disableActions = function () { return !_this.actions || _this.actions.length === 0 || !_this.items || _this.items.length === 0; };
            this.actionsTooltip = function () { return actionsTooltip(_this, !!_this.routeData.actionsOpen); };
            this.actionMember = function (id) {
                var actionViewModel = _.find(_this.actions, function (a) { return a.actionRep.actionId() === id; });
                return actionViewModel.actionRep;
            };
        }
        ListViewModel.prototype.updateItems = function (value) {
            var _this = this;
            this.items = this.viewModelFactory.getItems(value, this.state === NakedObjects.CollectionViewState.Table, this.routeData, this);
            var totalCount = this.listRep.pagination().totalCount;
            this.allSelected = _.every(this.items, function (item) { return item.selected; });
            var count = this.items.length;
            this.size = count;
            if (count > 0) {
                this.description = function () { return NakedObjects.pageMessage(_this.page, _this.numPages, count, totalCount); };
            }
            else {
                this.description = function () { return NakedObjects.noItemsFound; };
            }
        };
        ListViewModel.prototype.refresh = function (routeData) {
            var _this = this;
            this.routeData = routeData;
            if (this.state !== routeData.state) {
                this.state = routeData.state;
                if (this.state === NakedObjects.CollectionViewState.Table && !this.hasTableData()) {
                    this.recreate(this.page, this.pageSize).
                        then(function (list) {
                        _this.listRep = list;
                        _this.updateItems(list.value());
                    }).
                        catch(function (reject) {
                        _this.error.handleError(reject);
                    });
                }
                else {
                    this.updateItems(this.listRep.value());
                }
            }
        };
        ListViewModel.prototype.reset = function (list, routeData) {
            var _this = this;
            this.listRep = list;
            this.routeData = routeData;
            this.id = this.urlManager.getListCacheIndex(routeData.paneId, routeData.page, routeData.pageSize);
            this.onPaneId = routeData.paneId;
            this.pluralName = "Objects";
            this.page = this.listRep.pagination().page;
            this.pageSize = this.listRep.pagination().pageSize;
            this.numPages = this.listRep.pagination().numPages;
            this.state = routeData.state;
            this.updateItems(list.value());
            var actions = this.listRep.actionMembers();
            this.actions = _.map(actions, function (action) { return _this.viewModelFactory.actionViewModel(action, _this, routeData); });
            this.menuItems = createMenuItems(this.actions);
            _.forEach(this.actions, function (a) { return _this.decorate(a); });
        };
        return ListViewModel;
    }(ContributedActionParentViewModel));
    NakedObjects.ListViewModel = ListViewModel;
    var CollectionViewModel = (function (_super) {
        __extends(CollectionViewModel, _super);
        function CollectionViewModel() {
            var _this = this;
            _super.apply(this, arguments);
            this.description = function () { return _this.details.toString(); };
            this.disableActions = function () { return _this.editing || !_this.actions || _this.actions.length === 0; };
            this.actionMember = function (id) {
                var actionViewModel = _.find(_this.actions, function (a) { return a.actionRep.actionId() === id; });
                return actionViewModel ? actionViewModel.actionRep : null;
            };
        }
        CollectionViewModel.prototype.setActions = function (actions, routeData) {
            var _this = this;
            this.actions = _.map(actions, function (action) { return _this.viewModelFactory.actionViewModel(action, _this, routeData); });
            this.menuItems = createMenuItems(this.actions);
            _.forEach(this.actions, function (a) { return _this.decorate(a); });
        };
        CollectionViewModel.prototype.hasMatchingLocallyContributedAction = function (id) {
            return id && this.actions && this.actions.length > 0 && !!this.actionMember(id);
        };
        return CollectionViewModel;
    }(ContributedActionParentViewModel));
    NakedObjects.CollectionViewModel = CollectionViewModel;
    var MenusViewModel = (function () {
        function MenusViewModel(viewModelFactory) {
            this.viewModelFactory = viewModelFactory;
        }
        MenusViewModel.prototype.reset = function (menusRep, routeData) {
            var _this = this;
            this.menusRep = menusRep;
            this.onPaneId = routeData.paneId;
            this.items = _.map(this.menusRep.value(), function (link) { return _this.viewModelFactory.linkViewModel(link, _this.onPaneId); });
            return this;
        };
        return MenusViewModel;
    }());
    NakedObjects.MenusViewModel = MenusViewModel;
    var MenuViewModel = (function (_super) {
        __extends(MenuViewModel, _super);
        function MenuViewModel() {
            _super.apply(this, arguments);
        }
        return MenuViewModel;
    }(MessageViewModel));
    NakedObjects.MenuViewModel = MenuViewModel;
    var TableRowColumnViewModel = (function () {
        function TableRowColumnViewModel() {
        }
        return TableRowColumnViewModel;
    }());
    NakedObjects.TableRowColumnViewModel = TableRowColumnViewModel;
    var PlaceHolderTableRowColumnViewModel = (function () {
        function PlaceHolderTableRowColumnViewModel(id) {
            this.id = id;
        }
        return PlaceHolderTableRowColumnViewModel;
    }());
    NakedObjects.PlaceHolderTableRowColumnViewModel = PlaceHolderTableRowColumnViewModel;
    var TableRowViewModel = (function () {
        function TableRowViewModel() {
        }
        TableRowViewModel.prototype.conformColumns = function (columns) {
            var _this = this;
            if (columns) {
                this.properties =
                    _.map(columns, function (c) { return _.find(_this.properties, function (tp) { return tp.id === c; }) || new PlaceHolderTableRowColumnViewModel(c); });
            }
        };
        return TableRowViewModel;
    }());
    NakedObjects.TableRowViewModel = TableRowViewModel;
    var ApplicationPropertiesViewModel = (function () {
        function ApplicationPropertiesViewModel() {
        }
        return ApplicationPropertiesViewModel;
    }());
    NakedObjects.ApplicationPropertiesViewModel = ApplicationPropertiesViewModel;
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
    var DomainObjectViewModel = (function (_super) {
        __extends(DomainObjectViewModel, _super);
        function DomainObjectViewModel(colorService, contextService, viewModelFactory, urlManager, focusManager, error, $q) {
            var _this = this;
            _super.call(this);
            this.colorService = colorService;
            this.contextService = contextService;
            this.viewModelFactory = viewModelFactory;
            this.urlManager = urlManager;
            this.focusManager = focusManager;
            this.error = error;
            this.$q = $q;
            this.editProperties = function () { return _.filter(_this.properties, function (p) { return p.isEditable && p.isDirty(); }); };
            this.isFormOrTransient = function () { return _this.domainObject.extensions().interactionMode() === "form" || _this.domainObject.extensions().interactionMode() === "transient"; };
            this.cancelHandler = function () { return _this.isFormOrTransient() ?
                function () { return _this.urlManager.popUrlState(_this.onPaneId); } :
                function () { return _this.urlManager.setInteractionMode(NakedObjects.InteractionMode.View, _this.onPaneId); }; };
            this.saveHandler = function () { return _this.domainObject.isTransient() ? _this.contextService.saveObject : _this.contextService.updateObject; };
            this.validateHandler = function () { return _this.domainObject.isTransient() ? _this.contextService.validateSaveObject : _this.contextService.validateUpdateObject; };
            this.handleWrappedError = function (reject) {
                var display = function (em) { return _this.viewModelFactory.handleErrorResponse(em, _this, _this.properties); };
                _this.error.handleErrorAndDisplayMessages(reject, display);
            };
            this.propertyMap = function () {
                var pps = _.filter(_this.properties, function (property) { return property.isEditable; });
                return _.zipObject(_.map(pps, function (p) { return p.id; }), _.map(pps, function (p) { return p.getValue(); }));
            };
            this.editComplete = function () {
                _this.contextService.updateValues();
                _this.contextService.clearObjectUpdater(_this.onPaneId);
            };
            this.clientValid = function () { return _.every(_this.properties, function (p) { return p.clientValid; }); };
            this.tooltip = function () { return tooltip(_this, _this.properties); };
            this.actionsTooltip = function () { return actionsTooltip(_this, !!_this.routeData.actionsOpen); };
            this.toggleActionMenu = function () {
                _this.focusManager.focusOverrideOff();
                _this.contextService.updateValues();
                _this.urlManager.toggleObjectMenu(_this.onPaneId);
            };
            this.setProperties = function () { return _.forEach(_this.editProperties(), function (p) { return _this.contextService.setPropertyValue(_this.domainObject, p.propertyRep, p.getValue(), _this.onPaneId); }); };
            this.doEditCancel = function () {
                _this.editComplete();
                _this.contextService.clearObjectValues(_this.onPaneId);
                _this.cancelHandler()();
            };
            this.clearCachedFiles = function () { return _.forEach(_this.properties, function (p) { return p.attachment ? p.attachment.clearCachedFile() : null; }); };
            this.doSave = function (viewObject) {
                _this.clearCachedFiles();
                _this.contextService.updateValues();
                var propMap = _this.propertyMap();
                _this.contextService.clearObjectUpdater(_this.onPaneId);
                _this.saveHandler()(_this.domainObject, propMap, _this.onPaneId, viewObject).
                    then(function (obj) { return _this.reset(obj, _this.urlManager.getRouteData().pane()[_this.onPaneId]); }).
                    catch(function (reject) { return _this.handleWrappedError(reject); });
            };
            this.doSaveValidate = function () {
                var propMap = _this.propertyMap();
                return _this.validateHandler()(_this.domainObject, propMap).
                    then(function () {
                    _this.resetMessage();
                    return true;
                }).
                    catch(function (reject) {
                    _this.handleWrappedError(reject);
                    return _this.$q.reject(false);
                });
            };
            this.doEdit = function () {
                _this.contextService.updateValues(); // for other panes
                _this.clearCachedFiles();
                _this.contextService.clearObjectValues(_this.onPaneId);
                _this.contextService.getObjectForEdit(_this.onPaneId, _this.domainObject).
                    then(function (updatedObject) {
                    _this.reset(updatedObject, _this.urlManager.getRouteData().pane()[_this.onPaneId]);
                    _this.urlManager.pushUrlState(_this.onPaneId);
                    _this.urlManager.setInteractionMode(NakedObjects.InteractionMode.Edit, _this.onPaneId);
                }).
                    catch(function (reject) { return _this.handleWrappedError(reject); });
            };
            this.doReload = function () {
                _this.contextService.updateValues();
                _this.clearCachedFiles();
                _this.contextService.reloadObject(_this.onPaneId, _this.domainObject)
                    .then(function (updatedObject) { return _this.reset(updatedObject, _this.urlManager.getRouteData().pane()[_this.onPaneId]); })
                    .catch(function (reject) { return _this.handleWrappedError(reject); });
            };
            this.hideEdit = function () { return _this.isFormOrTransient() || _.every(_this.properties, function (p) { return !p.isEditable; }); };
            this.disableActions = function () { return !_this.actions || _this.actions.length === 0; };
            this.canDropOn = function (targetType) { return _this.contextService.isSubTypeOf(_this.domainType, targetType); };
        }
        DomainObjectViewModel.prototype.wrapAction = function (a) {
            var _this = this;
            var wrappedInvoke = a.execute;
            a.execute = function (pps, right) {
                _this.setProperties();
                var pairs = _.map(_this.editProperties(), function (p) { return [p.id, p.getValue()]; });
                var prps = _.fromPairs(pairs);
                var parmValueMap = _.mapValues(a.invokableActionRep.parameters(), function (p) { return ({ parm: p, value: prps[p.id()] }); });
                var allpps = _.map(parmValueMap, function (o) { return _this.viewModelFactory.parameterViewModel(o.parm, o.value, _this.onPaneId); });
                return wrappedInvoke(allpps, right).
                    catch(function (reject) {
                    _this.handleWrappedError(reject);
                    return _this.$q.reject(reject);
                });
            };
        };
        // must be careful with this - OK for changes on client but after server updates should use  reset
        // because parameters may have appeared or disappeared etc and refesh just updates existing views. 
        // So OK for view state changes but not eg for a parameter that disappears after saving
        DomainObjectViewModel.prototype.refresh = function (routeData) {
            var _this = this;
            this.routeData = routeData;
            var iMode = this.domainObject.extensions().interactionMode();
            this.isInEdit = routeData.interactionMode !== NakedObjects.InteractionMode.View || iMode === "form" || iMode === "transient";
            this.props = routeData.interactionMode !== NakedObjects.InteractionMode.View ? this.contextService.getCurrentObjectValues(this.domainObject.id(), routeData.paneId) : {};
            _.forEach(this.properties, function (p) { return p.refresh(_this.props[p.id]); });
            _.forEach(this.collections, function (c) { return c.refresh(_this.routeData, false); });
            this.unsaved = routeData.interactionMode === NakedObjects.InteractionMode.Transient;
            this.title = this.unsaved ? "Unsaved " + this.domainObject.extensions().friendlyName() : this.domainObject.title();
            this.title = this.title + dirtyMarker(this.contextService, this.domainObject.getOid());
            if (routeData.interactionMode === NakedObjects.InteractionMode.Form) {
                _.forEach(this.actions, function (a) { return _this.wrapAction(a); });
            }
            // leave message from previous refresh 
            this.clearMessage();
        };
        DomainObjectViewModel.prototype.reset = function (obj, routeData) {
            var _this = this;
            this.domainObject = obj;
            this.onPaneId = routeData.paneId;
            this.routeData = routeData;
            var iMode = this.domainObject.extensions().interactionMode();
            this.isInEdit = routeData.interactionMode !== NakedObjects.InteractionMode.View || iMode === "form" || iMode === "transient";
            this.props = routeData.interactionMode !== NakedObjects.InteractionMode.View ? this.contextService.getCurrentObjectValues(this.domainObject.id(), routeData.paneId) : {};
            var actions = _.values(this.domainObject.actionMembers());
            this.actions = _.map(actions, function (action) { return _this.viewModelFactory.actionViewModel(action, _this, _this.routeData); });
            this.menuItems = createMenuItems(this.actions);
            this.properties = _.map(this.domainObject.propertyMembers(), function (property, id) { return _this.viewModelFactory.propertyViewModel(property, id, _this.props[id], _this.onPaneId, _this.propertyMap); });
            this.collections = _.map(this.domainObject.collectionMembers(), function (collection) { return _this.viewModelFactory.collectionViewModel(collection, _this.routeData); });
            this.unsaved = routeData.interactionMode === NakedObjects.InteractionMode.Transient;
            this.title = this.unsaved ? "Unsaved " + this.domainObject.extensions().friendlyName() : this.domainObject.title();
            this.title = this.title + dirtyMarker(this.contextService, obj.getOid());
            this.friendlyName = this.domainObject.extensions().friendlyName();
            this.presentationHint = this.domainObject.extensions().presentationHint();
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
            this.selectedChoice = sav ? ChoiceViewModel.create(sav, "") : null;
            this.colorService.toColorNumberFromType(this.domainObject.domainType()).
                then(function (c) { return _this.color = "" + NakedObjects.objectColor + c; }).
                catch(function (reject) { return _this.error.handleError(reject); });
            this.resetMessage();
            if (routeData.interactionMode === NakedObjects.InteractionMode.Form) {
                _.forEach(this.actions, function (a) { return _this.wrapAction(a); });
            }
            return this;
        };
        DomainObjectViewModel.prototype.concurrency = function () {
            var _this = this;
            return function (event, em) {
                _this.routeData = _this.urlManager.getRouteData().pane()[_this.onPaneId];
                _this.contextService.getObject(_this.onPaneId, _this.domainObject.getOid(), _this.routeData.interactionMode).
                    then(function (obj) {
                    // cleared cached values so all values are from reloaded representation 
                    _this.contextService.clearObjectValues(_this.onPaneId);
                    return _this.contextService.reloadObject(_this.onPaneId, obj);
                }).
                    then(function (reloadedObj) {
                    if (_this.routeData.dialogId) {
                        _this.urlManager.closeDialogReplaceHistory(_this.onPaneId);
                    }
                    _this.reset(reloadedObj, _this.routeData);
                    _this.viewModelFactory.handleErrorResponse(em, _this, _this.properties);
                }).
                    catch(function (reject) { return _this.error.handleError(reject); });
            };
        };
        return DomainObjectViewModel;
    }(MessageViewModel));
    NakedObjects.DomainObjectViewModel = DomainObjectViewModel;
    var CiceroViewModel = (function () {
        function CiceroViewModel() {
            var _this = this;
            this.alert = ""; //Alert is appended before the output
            this.selectPreviousInput = function () {
                _this.input = _this.previousInput;
            };
            this.clearInput = function () {
                _this.input = null;
            };
        }
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