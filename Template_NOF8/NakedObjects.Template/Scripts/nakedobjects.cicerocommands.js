/// <reference path="nakedobjects.models.ts" />
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var NakedObjects;
(function (NakedObjects) {
    var DomainObjectRepresentation = NakedObjects.Models.DomainObjectRepresentation;
    var ErrorWrapper = NakedObjects.Models.ErrorWrapper;
    var ErrorCategory = NakedObjects.Models.ErrorCategory;
    var ClientErrorCode = NakedObjects.Models.ClientErrorCode;
    var PropertyMember = NakedObjects.Models.PropertyMember;
    var Parameter = NakedObjects.Models.Parameter;
    var Value = NakedObjects.Models.Value;
    var EntryType = NakedObjects.Models.EntryType;
    var TypePlusTitle = NakedObjects.Models.typePlusTitle;
    var FriendlyTypeName = NakedObjects.Models.friendlyTypeName;
    var FriendlyNameForParam = NakedObjects.Models.friendlyNameForParam;
    var FriendlyNameForProperty = NakedObjects.Models.friendlyNameForProperty;
    var isDateOrDateTime = NakedObjects.Models.isDateOrDateTime;
    var toDateString = NakedObjects.Models.toDateString;
    var ObjectIdWrapper = NakedObjects.Models.ObjectIdWrapper;
    var isIInvokableAction = NakedObjects.Models.isIInvokableAction;
    function getParametersAndCurrentValue(action, context) {
        if (isIInvokableAction(action)) {
            var parms = action.parameters();
            var values_1 = context.getCurrentDialogValues(action.actionId());
            return _.mapValues(parms, function (p) {
                var value = values_1[p.id()];
                if (value === undefined) {
                    return p.default();
                }
                return value;
            });
        }
        return {};
    }
    NakedObjects.getParametersAndCurrentValue = getParametersAndCurrentValue;
    var Command = (function () {
        function Command(urlManager, nglocation, commandFactory, context, navigation, $q, $route, mask, error) {
            this.urlManager = urlManager;
            this.nglocation = nglocation;
            this.commandFactory = commandFactory;
            this.context = context;
            this.navigation = navigation;
            this.$q = $q;
            this.$route = $route;
            this.mask = mask;
            this.error = error;
        }
        //Must be called after construction and before execute is called
        Command.prototype.initialiseWithViewModel = function (cvm) {
            this.vm = cvm;
        };
        Command.prototype.execute = function (argString, chained) {
            if (!this.isAvailableInCurrentContext()) {
                this.clearInputAndSetMessage(NakedObjects.commandNotAvailable(this.fullCommand));
                return;
            }
            //TODO: This could be moved into a pre-parse method as it does not depend on context
            if (argString == null) {
                if (this.minArguments > 0) {
                    this.clearInputAndSetMessage(NakedObjects.noArguments);
                    return;
                }
            }
            else {
                var args = argString.split(",");
                if (args.length < this.minArguments) {
                    this.clearInputAndSetMessage(NakedObjects.tooFewArguments);
                    return;
                }
                else if (args.length > this.maxArguments) {
                    this.clearInputAndSetMessage(NakedObjects.tooManyArguments);
                    return;
                }
            }
            this.doExecute(argString, chained);
        };
        //Helper methods follow
        Command.prototype.clearInputAndSetMessage = function (text) {
            this.vm.clearInput();
            this.vm.message = text;
            this.$route.reload();
        };
        Command.prototype.mayNotBeChained = function (rider) {
            if (rider === void 0) { rider = ""; }
            this.clearInputAndSetMessage(NakedObjects.mayNotbeChainedMessage(this.fullCommand, rider));
        };
        Command.prototype.appendAsNewLineToOutput = function (text) {
            this.vm.output.concat("/n" + text);
        };
        Command.prototype.checkMatch = function (matchText) {
            if (this.fullCommand.indexOf(matchText) !== 0) {
                throw new Error(NakedObjects.noSuchCommand(matchText));
            }
        };
        //argNo starts from 0.
        //If argument does not parse correctly, message will be passed to UI and command aborted.
        Command.prototype.argumentAsString = function (argString, argNo, optional, toLower) {
            if (optional === void 0) { optional = false; }
            if (toLower === void 0) { toLower = true; }
            if (!argString)
                return undefined;
            if (!optional && argString.split(",").length < argNo + 1) {
                throw new Error(NakedObjects.tooFewArguments);
            }
            var args = argString.split(",");
            if (args.length < argNo + 1) {
                if (optional) {
                    return undefined;
                }
                else {
                    throw new Error(NakedObjects.missingArgument(argNo + 1));
                }
            }
            return toLower ? args[argNo].trim().toLowerCase() : args[argNo].trim(); // which may be "" if argString ends in a ','
        };
        //argNo starts from 0.
        Command.prototype.argumentAsNumber = function (args, argNo, optional) {
            if (optional === void 0) { optional = false; }
            var arg = this.argumentAsString(args, argNo, optional);
            if (!arg && optional)
                return null;
            var number = parseInt(arg);
            if (isNaN(number)) {
                throw new Error(NakedObjects.wrongTypeArgument(argNo + 1));
            }
            return number;
        };
        Command.prototype.parseInt = function (input) {
            if (!input) {
                return null;
            }
            var number = parseInt(input);
            if (isNaN(number)) {
                throw new Error(NakedObjects.isNotANumber(input));
            }
            return number;
        };
        //Parses '17, 3-5, -9, 6-' into two numbers 
        Command.prototype.parseRange = function (arg) {
            if (!arg) {
                arg = "1-";
            }
            var clauses = arg.split("-");
            var range = { start: null, end: null };
            switch (clauses.length) {
                case 1:
                    range.start = this.parseInt(clauses[0]);
                    range.end = range.start;
                    break;
                case 2:
                    range.start = this.parseInt(clauses[0]);
                    range.end = this.parseInt(clauses[1]);
                    break;
                default:
                    throw new Error(NakedObjects.tooManyDashes);
            }
            if ((range.start != null && range.start < 1) || (range.end != null && range.end < 1)) {
                throw new Error(NakedObjects.mustBeGreaterThanZero);
            }
            return range;
        };
        Command.prototype.getContextDescription = function () {
            //todo
            return null;
        };
        Command.prototype.routeData = function () {
            return this.urlManager.getRouteData().pane1;
        };
        //Helpers delegating to RouteData
        Command.prototype.isHome = function () {
            return this.vm.viewType === NakedObjects.ViewType.Home;
        };
        Command.prototype.isObject = function () {
            return this.vm.viewType === NakedObjects.ViewType.Object;
        };
        Command.prototype.getObject = function () {
            var _this = this;
            var oid = ObjectIdWrapper.fromObjectId(this.routeData().objectId);
            //TODO: Consider view model & transient modes?
            return this.context.getObject(1, oid, this.routeData().interactionMode).then(function (obj) {
                if (_this.routeData().interactionMode === NakedObjects.InteractionMode.Edit) {
                    return _this.context.getObjectForEdit(1, obj);
                }
                else {
                    return obj; //To wrap a known object as a promise
                }
            });
        };
        Command.prototype.isList = function () {
            return this.vm.viewType === NakedObjects.ViewType.List;
        };
        Command.prototype.getList = function () {
            var routeData = this.routeData();
            //TODO: Currently covers only the list-from-menu; need to cover list from object action
            return this.context.getListFromMenu(1, routeData, routeData.page, routeData.pageSize);
        };
        Command.prototype.isMenu = function () {
            return !!this.routeData().menuId;
        };
        Command.prototype.getMenu = function () {
            return this.context.getMenu(this.routeData().menuId);
        };
        Command.prototype.isDialog = function () {
            return !!this.routeData().dialogId;
        };
        Command.prototype.getActionForCurrentDialog = function () {
            var _this = this;
            var dialogId = this.routeData().dialogId;
            if (this.isObject()) {
                return this.getObject().then(function (obj) { return _this.context.getInvokableAction(obj.actionMember(dialogId)); });
            }
            else if (this.isMenu()) {
                return this.getMenu().then(function (menu) { return _this.context.getInvokableAction(menu.actionMember(dialogId)); }); //i.e. return a promise
            }
            return this.$q.reject(new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.NotImplemented, "List actions not implemented yet"));
        };
        //Tests that at least one collection is open (should only be one). 
        //TODO: assumes that closing collection removes it from routeData NOT sets it to Summary
        Command.prototype.isCollection = function () {
            return this.isObject() && _.some(this.routeData().collections);
        };
        Command.prototype.closeAnyOpenCollections = function () {
            var _this = this;
            var open = NakedObjects.openCollectionIds(this.routeData());
            _.forEach(open, function (id) {
                _this.urlManager.setCollectionMemberState(id, NakedObjects.CollectionViewState.Summary);
            });
        };
        Command.prototype.isTable = function () {
            return false; //TODO
        };
        Command.prototype.isEdit = function () {
            return this.routeData().interactionMode === NakedObjects.InteractionMode.Edit;
        };
        Command.prototype.isForm = function () {
            return this.routeData().interactionMode === NakedObjects.InteractionMode.Form;
        };
        Command.prototype.isTransient = function () {
            return this.routeData().interactionMode === NakedObjects.InteractionMode.Transient;
        };
        Command.prototype.matchingProperties = function (obj, match) {
            var props = _.map(obj.propertyMembers(), function (prop) { return prop; });
            if (match) {
                props = this.matchFriendlyNameAndOrMenuPath(props, match);
            }
            return props;
        };
        Command.prototype.matchingCollections = function (obj, match) {
            var allColls = _.map(obj.collectionMembers(), function (action) { return action; });
            if (match) {
                return this.matchFriendlyNameAndOrMenuPath(allColls, match);
            }
            else {
                return allColls;
            }
        };
        Command.prototype.matchingParameters = function (action, match) {
            var params = _.map(action.parameters(), function (p) { return p; });
            if (match) {
                params = this.matchFriendlyNameAndOrMenuPath(params, match);
            }
            return params;
        };
        Command.prototype.matchFriendlyNameAndOrMenuPath = function (reps, match) {
            var clauses = match.split(" ");
            //An exact match has preference over any partial match
            var exactMatches = _.filter(reps, function (rep) {
                var path = rep.extensions().menuPath();
                var name = rep.extensions().friendlyName().toLowerCase();
                return match === name ||
                    (!!path && match === path.toLowerCase() + " " + name) ||
                    _.every(clauses, function (clause) { return name === clause || (!!path && path.toLowerCase() === clause); });
            });
            if (exactMatches.length > 0)
                return exactMatches;
            return _.filter(reps, function (rep) {
                var path = rep.extensions().menuPath();
                var name = rep.extensions().friendlyName().toLowerCase();
                return _.every(clauses, function (clause) { return name.indexOf(clause) >= 0 || (!!path && path.toLowerCase().indexOf(clause) >= 0); });
            });
        };
        Command.prototype.findMatchingChoicesForRef = function (choices, titleMatch) {
            return _.filter(choices, function (v) { return v.toString().toLowerCase().indexOf(titleMatch.toLowerCase()) >= 0; });
        };
        Command.prototype.findMatchingChoicesForScalar = function (choices, titleMatch) {
            var labels = _.keys(choices);
            var matchingLabels = _.filter(labels, function (l) { return l.toString().toLowerCase().indexOf(titleMatch.toLowerCase()) >= 0; });
            var result = new Array();
            switch (matchingLabels.length) {
                case 0:
                    break; //leave result empty
                case 1:
                    //Push the VALUE for the key
                    //For simple scalars they are the same, but not for Enums
                    result.push(choices[matchingLabels[0]]);
                    break;
                default:
                    //Push the matching KEYs, wrapped as (pseudo) Values for display in message to user
                    //For simple scalars the values would also be OK, but not for Enums
                    _.forEach(matchingLabels, function (label) { return result.push(new Value(label)); });
                    break;
            }
            return result;
        };
        Command.prototype.handleErrorResponse = function (err, getFriendlyName) {
            var _this = this;
            if (err.invalidReason()) {
                this.clearInputAndSetMessage(err.invalidReason());
                return;
            }
            var msg = NakedObjects.pleaseCompleteOrCorrect;
            _.each(err.valuesMap(), function (errorValue, fieldId) {
                msg += _this.fieldValidationMessage(errorValue, function () { return getFriendlyName(fieldId); });
            });
            this.clearInputAndSetMessage(msg);
        };
        Command.prototype.fieldValidationMessage = function (errorValue, fieldFriendlyName) {
            var msg = "";
            var reason = errorValue.invalidReason;
            var value = errorValue.value;
            if (reason) {
                msg += fieldFriendlyName() + ": ";
                if (reason === NakedObjects.mandatory) {
                    msg += NakedObjects.required;
                }
                else {
                    msg += value + " " + reason;
                }
                msg += "\n";
            }
            return msg;
        };
        Command.prototype.valueForUrl = function (val, field) {
            var _this = this;
            if (val.isNull())
                return val;
            var fieldEntryType = field.entryType();
            if (fieldEntryType !== EntryType.FreeForm || field.isCollectionContributed()) {
                if (fieldEntryType === EntryType.MultipleChoices || field.isCollectionContributed()) {
                    var valuesFromRouteData_1 = new Array();
                    if (field instanceof Parameter) {
                        var rd = getParametersAndCurrentValue(field.parent, this.context)[field.id()];
                        if (rd)
                            valuesFromRouteData_1 = rd.list(); //TODO: what if only one?
                    }
                    else if (field instanceof PropertyMember) {
                        var obj = field.parent;
                        var props = this.context.getCurrentObjectValues(obj.id());
                        var rd = props[field.id()];
                        if (rd)
                            valuesFromRouteData_1 = rd.list(); //TODO: what if only one?
                    }
                    var vals = [];
                    if (val.isReference() || val.isScalar()) {
                        vals = new Array(val);
                    }
                    else if (val.isList()) {
                        vals = val.list();
                    }
                    _.forEach(vals, function (v) {
                        _this.addOrRemoveValue(valuesFromRouteData_1, v);
                    });
                    if (vals[0].isScalar()) {
                        var scalars = _.map(valuesFromRouteData_1, function (v) { return v.scalar(); });
                        return new Value(scalars);
                    }
                    else {
                        var links = _.map(valuesFromRouteData_1, function (v) { return ({ href: v.link().href(), title: v.link().title() }); });
                        return new Value(links);
                    }
                }
                if (val.isScalar()) {
                    return val;
                }
                // reference 
                return this.leanLink(val);
            }
            if (val.isScalar()) {
                if (val.isNull()) {
                    return new Value("");
                }
                return val;
            }
            if (val.isReference()) {
                return this.leanLink(val);
            }
            return null;
        };
        Command.prototype.leanLink = function (val) {
            return new Value({ href: val.link().href(), title: val.link().title() });
        };
        Command.prototype.addOrRemoveValue = function (valuesFromRouteData, val) {
            var index;
            var valToAdd;
            if (val.isScalar()) {
                valToAdd = val;
                index = _.findIndex(valuesFromRouteData, function (v) { return v.scalar() === val.scalar(); });
            }
            else {
                valToAdd = this.leanLink(val);
                index = _.findIndex(valuesFromRouteData, function (v) { return v.link().href() === valToAdd.link().href(); });
            }
            if (index > -1) {
                valuesFromRouteData.splice(index, 1);
            }
            else {
                valuesFromRouteData.push(valToAdd);
            }
        };
        Command.prototype.setFieldValueInContextAndUrl = function (field, urlVal) {
            this.context.setFieldValue(this.routeData().dialogId, field.id(), urlVal);
            this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl();
        };
        Command.prototype.setPropertyValueinContextAndUrl = function (obj, property, urlVal) {
            this.context.setPropertyValue(obj, property, urlVal);
            this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl();
        };
        return Command;
    }());
    NakedObjects.Command = Command;
    var Action = (function (_super) {
        __extends(Action, _super);
        function Action() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.actionCommand;
            this.helpText = NakedObjects.actionHelp;
            this.minArguments = 0;
            this.maxArguments = 2;
        }
        Action.prototype.isAvailableInCurrentContext = function () {
            return (this.isMenu() || this.isObject() || this.isForm()) && !this.isDialog() && !this.isEdit(); //TODO add list
        };
        Action.prototype.doExecute = function (args, chained) {
            var _this = this;
            var match = this.argumentAsString(args, 0);
            var details = this.argumentAsString(args, 1, true);
            if (details && details !== "?") {
                this.clearInputAndSetMessage(NakedObjects.mustbeQuestionMark);
                return;
            }
            if (this.isObject()) {
                this.getObject()
                    .then(function (obj) { return _this.processActions(match, obj.actionMembers(), details); })
                    .catch(function (reject) { return _this.error.handleError(reject); });
            }
            else if (this.isMenu()) {
                this.getMenu()
                    .then(function (menu) { return _this.processActions(match, menu.actionMembers(), details); })
                    .catch(function (reject) { return _this.error.handleError(reject); });
            }
            //TODO: handle list - CCAs
        };
        Action.prototype.processActions = function (match, actionsMap, details) {
            var actions = _.map(actionsMap, function (action) { return action; });
            if (actions.length === 0) {
                this.clearInputAndSetMessage(NakedObjects.noActionsAvailable);
                return;
            }
            if (match) {
                actions = this.matchFriendlyNameAndOrMenuPath(actions, match);
            }
            switch (actions.length) {
                case 0:
                    this.clearInputAndSetMessage(NakedObjects.doesNotMatchActions(match));
                    break;
                case 1:
                    var action = actions[0];
                    if (details) {
                        this.renderActionDetails(action);
                    }
                    else if (action.disabledReason()) {
                        this.disabledAction(action);
                    }
                    else {
                        this.openActionDialog(action);
                    }
                    break;
                default:
                    var output = match ? NakedObjects.matchingActions : NakedObjects.actionsMessage;
                    output += this.listActions(actions);
                    this.clearInputAndSetMessage(output);
            }
        };
        Action.prototype.disabledAction = function (action) {
            var output = NakedObjects.actionPrefix + " " + action.extensions().friendlyName() + " " + NakedObjects.isDisabled + " " + action.disabledReason();
            this.clearInputAndSetMessage(output);
        };
        Action.prototype.listActions = function (actions) {
            return _.reduce(actions, function (s, t) {
                var menupath = t.extensions().menuPath() ? t.extensions().menuPath() + " - " : "";
                var disabled = t.disabledReason() ? " (" + NakedObjects.disabledPrefix + " " + t.disabledReason() + ")" : "";
                return s + menupath + t.extensions().friendlyName() + disabled + "\n";
            }, "");
        };
        Action.prototype.openActionDialog = function (action) {
            var _this = this;
            this.context.clearDialogValues();
            this.urlManager.setDialog(action.actionId());
            this.context.getInvokableAction(action).
                then(function (invokable) { return _.forEach(invokable.parameters(), function (p) { return _this.setFieldValueInContextAndUrl(p, p.default()); }); }).
                catch(function (reject) { return _this.error.handleError(reject); });
        };
        Action.prototype.renderActionDetails = function (action) {
            var s = NakedObjects.descriptionPrefix + " " + action.extensions().friendlyName() + "\n" + (action.extensions().description() || NakedObjects.noDescription);
            this.clearInputAndSetMessage(s);
        };
        return Action;
    }(Command));
    NakedObjects.Action = Action;
    var Back = (function (_super) {
        __extends(Back, _super);
        function Back() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.backCommand;
            this.helpText = NakedObjects.backHelp;
            this.minArguments = 0;
            this.maxArguments = 0;
        }
        Back.prototype.isAvailableInCurrentContext = function () {
            return true;
        };
        Back.prototype.doExecute = function (args, chained) {
            this.navigation.back();
        };
        ;
        return Back;
    }(Command));
    NakedObjects.Back = Back;
    var Cancel = (function (_super) {
        __extends(Cancel, _super);
        function Cancel() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.cancelCommand;
            this.helpText = NakedObjects.cancelHelp;
            this.minArguments = 0;
            this.maxArguments = 0;
        }
        Cancel.prototype.isAvailableInCurrentContext = function () {
            return this.isDialog() || this.isEdit();
        };
        Cancel.prototype.doExecute = function (args, chained) {
            if (this.isEdit()) {
                this.urlManager.setInteractionMode(NakedObjects.InteractionMode.View);
            }
            if (this.isDialog()) {
                this.urlManager.closeDialogReplaceHistory();
            }
        };
        ;
        return Cancel;
    }(Command));
    NakedObjects.Cancel = Cancel;
    var Clipboard = (function (_super) {
        __extends(Clipboard, _super);
        function Clipboard() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.clipboardCommand;
            this.helpText = NakedObjects.clipboardHelp;
            this.minArguments = 1;
            this.maxArguments = 1;
        }
        Clipboard.prototype.isAvailableInCurrentContext = function () {
            return true;
        };
        Clipboard.prototype.doExecute = function (args, chained) {
            var sub = this.argumentAsString(args, 0);
            if (NakedObjects.clipboardCopy.indexOf(sub) === 0) {
                this.copy();
            }
            else if (NakedObjects.clipboardShow.indexOf(sub) === 0) {
                this.show();
            }
            else if (NakedObjects.clipboardGo.indexOf(sub) === 0) {
                this.go();
            }
            else if (NakedObjects.clipboardDiscard.indexOf(sub) === 0) {
                this.discard();
            }
            else {
                this.clearInputAndSetMessage(NakedObjects.clipboardError);
            }
        };
        ;
        Clipboard.prototype.copy = function () {
            var _this = this;
            if (!this.isObject()) {
                this.clearInputAndSetMessage(NakedObjects.clipboardContextError);
                return;
            }
            this.getObject().
                then(function (obj) {
                _this.vm.clipboard = obj;
                _this.show();
            }).
                catch(function (reject) { return _this.error.handleError(reject); });
        };
        Clipboard.prototype.show = function () {
            if (this.vm.clipboard) {
                var label = TypePlusTitle(this.vm.clipboard);
                this.clearInputAndSetMessage(NakedObjects.clipboardContents(label));
            }
            else {
                this.clearInputAndSetMessage(NakedObjects.clipboardEmpty);
            }
        };
        Clipboard.prototype.go = function () {
            var link = this.vm.clipboard.selfLink();
            if (link) {
                this.urlManager.setItem(link);
            }
            else {
                this.show();
            }
        };
        Clipboard.prototype.discard = function () {
            this.vm.clipboard = null;
            this.show();
        };
        return Clipboard;
    }(Command));
    NakedObjects.Clipboard = Clipboard;
    var Edit = (function (_super) {
        __extends(Edit, _super);
        function Edit() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.editCommand;
            this.helpText = NakedObjects.editHelp;
            this.minArguments = 0;
            this.maxArguments = 0;
        }
        Edit.prototype.isAvailableInCurrentContext = function () {
            return this.isObject() && !this.isEdit();
        };
        Edit.prototype.doExecute = function (args, chained) {
            if (chained) {
                this.mayNotBeChained();
                return;
            }
            this.context.clearObjectValues();
            this.urlManager.setInteractionMode(NakedObjects.InteractionMode.Edit);
        };
        ;
        return Edit;
    }(Command));
    NakedObjects.Edit = Edit;
    var Enter = (function (_super) {
        __extends(Enter, _super);
        function Enter() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.enterCommand;
            this.helpText = NakedObjects.enterHelp;
            this.minArguments = 2;
            this.maxArguments = 2;
        }
        Enter.prototype.isAvailableInCurrentContext = function () {
            return this.isDialog() || this.isEdit() || this.isTransient() || this.isForm();
        };
        Enter.prototype.doExecute = function (args, chained) {
            var fieldName = this.argumentAsString(args, 0);
            var fieldEntry = this.argumentAsString(args, 1, false, false);
            if (this.isDialog()) {
                this.fieldEntryForDialog(fieldName, fieldEntry);
            }
            else {
                this.fieldEntryForEdit(fieldName, fieldEntry);
            }
        };
        ;
        Enter.prototype.fieldEntryForEdit = function (fieldName, fieldEntry) {
            var _this = this;
            this.getObject().
                then(function (obj) {
                var fields = _this.matchingProperties(obj, fieldName);
                var s;
                switch (fields.length) {
                    case 0:
                        s = NakedObjects.doesNotMatchProperties(fieldName);
                        break;
                    case 1:
                        var field = fields[0];
                        if (fieldEntry === "?") {
                            //TODO: does this work in edit mode i.e. show entered value
                            s = _this.renderFieldDetails(field, field.value());
                        }
                        else {
                            _this.findAndClearAnyDependentFields(field.id(), obj.propertyMembers());
                            _this.setField(field, fieldEntry);
                            return;
                        }
                        break;
                    default:
                        s = fieldName + " " + NakedObjects.matchesMultiple;
                        s += _.reduce(fields, function (s, prop) { return s + prop.extensions().friendlyName() + "\n"; }, "");
                }
                _this.clearInputAndSetMessage(s);
            }).
                catch(function (reject) { return _this.error.handleError(reject); });
        };
        Enter.prototype.findAndClearAnyDependentFields = function (changingField, allFields) {
            var _this = this;
            _.forEach(allFields, function (field) {
                var promptLink = field.promptLink();
                if (promptLink) {
                    var pArgs = promptLink.arguments();
                    var argNames = _.keys(pArgs);
                    if (argNames.indexOf(changingField.toLowerCase()) >= 0) {
                        _this.clearField(field);
                    }
                }
            });
        };
        Enter.prototype.fieldEntryForDialog = function (fieldName, fieldEntry) {
            var _this = this;
            this.getActionForCurrentDialog().
                then(function (action) {
                //TODO: error -  need to get invokable action to get the params.
                var params = _.map(action.parameters(), function (param) { return param; });
                params = _this.matchFriendlyNameAndOrMenuPath(params, fieldName);
                switch (params.length) {
                    case 0:
                        _this.clearInputAndSetMessage(NakedObjects.doesNotMatchDialog(fieldName));
                        break;
                    case 1:
                        if (fieldEntry === "?") {
                            var p = params[0];
                            var value = getParametersAndCurrentValue(p.parent, _this.context)[p.id()];
                            var s = _this.renderFieldDetails(p, value);
                            _this.clearInputAndSetMessage(s);
                        }
                        else {
                            _this.findAndClearAnyDependentFields(fieldName, action.parameters());
                            _this.setField(params[0], fieldEntry);
                        }
                        break;
                    default:
                        _this.clearInputAndSetMessage(NakedObjects.multipleFieldMatches + " " + fieldName); //TODO: list them
                        break;
                }
            }).
                catch(function (reject) { return _this.error.handleError(reject); });
        };
        Enter.prototype.clearField = function (field) {
            this.context.setFieldValue(this.routeData().dialogId, field.id(), new Value(null));
            if (field instanceof Parameter) {
                this.context.setFieldValue(this.routeData().dialogId, field.id(), new Value(null));
            }
            else if (field instanceof PropertyMember) {
                var parent_1 = field.parent;
                this.context.setPropertyValue(parent_1, field, new Value(null));
            }
        };
        Enter.prototype.setField = function (field, fieldEntry) {
            if (field instanceof PropertyMember && field.disabledReason()) {
                this.clearInputAndSetMessage(field.extensions().friendlyName() + " " + NakedObjects.isNotModifiable);
                return;
            }
            var entryType = field.entryType();
            switch (entryType) {
                case EntryType.FreeForm:
                    this.handleFreeForm(field, fieldEntry);
                    return;
                case EntryType.AutoComplete:
                    this.handleAutoComplete(field, fieldEntry);
                    return;
                case EntryType.Choices:
                    this.handleChoices(field, fieldEntry);
                    return;
                case EntryType.MultipleChoices:
                    this.handleChoices(field, fieldEntry);
                    return;
                case EntryType.ConditionalChoices:
                    this.handleConditionalChoices(field, fieldEntry);
                    return;
                case EntryType.MultipleConditionalChoices:
                    this.handleConditionalChoices(field, fieldEntry);
                    return;
                default:
                    throw new Error(NakedObjects.invalidCase);
            }
        };
        Enter.prototype.handleFreeForm = function (field, fieldEntry) {
            if (field.isScalar()) {
                var value = new Value(fieldEntry);
                //TODO: handle a non-parsable date
                if (isDateOrDateTime(field)) {
                    var m = moment(fieldEntry, NakedObjects.supportedDateFormats, "en-GB", true);
                    if (m.isValid()) {
                        var dt = m.toDate();
                        value = new Value(toDateString(dt));
                    }
                }
                this.setFieldValue(field, value);
            }
            else {
                this.handleReferenceField(field, fieldEntry);
            }
        };
        Enter.prototype.setFieldValue = function (field, value) {
            var urlVal = this.valueForUrl(value, field);
            if (field instanceof Parameter) {
                this.setFieldValueInContextAndUrl(field, urlVal);
            }
            else if (field instanceof PropertyMember) {
                var parent_2 = field.parent;
                if (parent_2 instanceof DomainObjectRepresentation) {
                    this.setPropertyValueinContextAndUrl(parent_2, field, urlVal);
                }
            }
        };
        Enter.prototype.handleReferenceField = function (field, fieldEntry) {
            if (this.isPaste(fieldEntry)) {
                this.handleClipboard(field);
            }
            else {
                this.clearInputAndSetMessage(NakedObjects.invalidRefEntry);
            }
        };
        Enter.prototype.isPaste = function (fieldEntry) {
            return "paste".indexOf(fieldEntry) === 0;
        };
        Enter.prototype.handleClipboard = function (field) {
            var _this = this;
            var ref = this.vm.clipboard;
            if (!ref) {
                this.clearInputAndSetMessage(NakedObjects.emptyClipboard);
                return;
            }
            var paramType = field.extensions().returnType();
            var refType = ref.domainType();
            this.context.isSubTypeOf(refType, paramType).
                then(function (isSubType) {
                if (isSubType) {
                    var obj = _this.vm.clipboard;
                    var selfLink = obj.selfLink();
                    //Need to add a title to the SelfLink as not there by default
                    selfLink.setTitle(obj.title());
                    var value = new Value(selfLink);
                    _this.setFieldValue(field, value);
                }
                else {
                    _this.clearInputAndSetMessage(NakedObjects.incompatibleClipboard);
                }
            }).
                catch(function (reject) { return _this.error.handleError(reject); });
        };
        Enter.prototype.handleAutoComplete = function (field, fieldEntry) {
            var _this = this;
            //TODO: Need to check that the minimum number of characters has been entered or fail validation
            if (!field.isScalar() && this.isPaste(fieldEntry)) {
                this.handleClipboard(field);
            }
            else {
                this.context.autoComplete(field, field.id(), null, fieldEntry).
                    then(function (choices) {
                    var matches = _this.findMatchingChoicesForRef(choices, fieldEntry);
                    _this.switchOnMatches(field, fieldEntry, matches);
                }).
                    catch(function (reject) { return _this.error.handleError(reject); });
            }
        };
        Enter.prototype.handleChoices = function (field, fieldEntry) {
            var matches;
            if (field.isScalar()) {
                matches = this.findMatchingChoicesForScalar(field.choices(), fieldEntry);
            }
            else {
                matches = this.findMatchingChoicesForRef(field.choices(), fieldEntry);
            }
            this.switchOnMatches(field, fieldEntry, matches);
        };
        Enter.prototype.switchOnMatches = function (field, fieldEntry, matches) {
            switch (matches.length) {
                case 0:
                    this.clearInputAndSetMessage(NakedObjects.noMatch(fieldEntry));
                    break;
                case 1:
                    this.setFieldValue(field, matches[0]);
                    break;
                default:
                    var msg_1 = NakedObjects.multipleMatches;
                    _.forEach(matches, function (m) { return msg_1 += m.toString() + "\n"; });
                    this.clearInputAndSetMessage(msg_1);
                    break;
            }
        };
        Enter.prototype.getPropertiesAndCurrentValue = function (obj) {
            var props = obj.propertyMembers();
            var values = _.mapValues(props, function (p) { return p.value(); });
            var modifiedProps = this.context.getCurrentObjectValues(obj.id());
            _.forEach(values, function (v, k) {
                var newValue = modifiedProps[k];
                if (newValue) {
                    values[k] = newValue;
                }
            });
            return _.mapKeys(values, function (v, k) { return k.toLowerCase(); });
        };
        Enter.prototype.handleConditionalChoices = function (field, fieldEntry) {
            var _this = this;
            var enteredFields;
            if (field instanceof Parameter) {
                enteredFields = getParametersAndCurrentValue(field.parent, this.context);
            }
            if (field instanceof PropertyMember) {
                enteredFields = this.getPropertiesAndCurrentValue(field.parent);
            }
            var args = _.fromPairs(_.map(field.promptLink().arguments(), function (v, key) { return [key, new Value(v.value)]; }));
            _.forEach(_.keys(args), function (key) { return args[key] = enteredFields[key]; });
            this.context.conditionalChoices(field, field.id(), null, args).
                then(function (choices) {
                var matches = _this.findMatchingChoicesForRef(choices, fieldEntry);
                _this.switchOnMatches(field, fieldEntry, matches);
            }).
                catch(function (reject) { return _this.error.handleError(reject); });
        };
        Enter.prototype.renderFieldDetails = function (field, value) {
            var s = NakedObjects.fieldName(field.extensions().friendlyName());
            var desc = field.extensions().description();
            s += desc ? "\n" + NakedObjects.descriptionFieldPrefix + " " + desc : "";
            s += "\n" + NakedObjects.typePrefix + " " + FriendlyTypeName(field.extensions().returnType());
            if (field instanceof PropertyMember && field.disabledReason()) {
                s += "\n" + NakedObjects.unModifiablePrefix(field.disabledReason());
            }
            else {
                s += field.extensions().optional() ? "\n" + NakedObjects.optional : "\n" + NakedObjects.mandatory;
                if (field.choices()) {
                    var label = "\n" + NakedObjects.choices + ": ";
                    s += _.reduce(field.choices(), function (s, cho) { return s + cho + " "; }, label);
                }
            }
            return s;
        };
        return Enter;
    }(Command));
    NakedObjects.Enter = Enter;
    var Forward = (function (_super) {
        __extends(Forward, _super);
        function Forward() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.forwardCommand;
            this.helpText = NakedObjects.forwardHelp;
            this.minArguments = 0;
            this.maxArguments = 0;
        }
        Forward.prototype.isAvailableInCurrentContext = function () {
            return true;
        };
        Forward.prototype.doExecute = function (args, chained) {
            this.vm.clearInput(); //To catch case where can't go any further forward and hence url does not change.
            this.navigation.forward();
        };
        ;
        return Forward;
    }(Command));
    NakedObjects.Forward = Forward;
    var Gemini = (function (_super) {
        __extends(Gemini, _super);
        function Gemini() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.geminiCommand;
            this.helpText = NakedObjects.geminiHelp;
            this.minArguments = 0;
            this.maxArguments = 0;
        }
        Gemini.prototype.isAvailableInCurrentContext = function () {
            return true;
        };
        Gemini.prototype.doExecute = function (args, chained) {
            var newPath = "/gemini/" + this.nglocation.path().split("/")[2];
            this.nglocation.path(newPath);
        };
        ;
        return Gemini;
    }(Command));
    NakedObjects.Gemini = Gemini;
    var Goto = (function (_super) {
        __extends(Goto, _super);
        function Goto() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.gotoCommand;
            this.helpText = NakedObjects.gotoHelp;
            this.minArguments = 1;
            this.maxArguments = 1;
        }
        Goto.prototype.isAvailableInCurrentContext = function () {
            return this.isObject() || this.isList();
        };
        Goto.prototype.doExecute = function (args, chained) {
            var _this = this;
            var arg0 = this.argumentAsString(args, 0);
            if (this.isList()) {
                var itemNo_1;
                try {
                    itemNo_1 = this.parseInt(arg0);
                }
                catch (e) {
                    this.clearInputAndSetMessage(e.message);
                    return;
                }
                this.getList().then(function (list) {
                    _this.attemptGotoLinkNumber(itemNo_1, list.value());
                    return;
                });
                return;
            }
            if (this.isObject) {
                this.getObject().
                    then(function (obj) {
                    if (_this.isCollection()) {
                        var itemNo_2 = _this.argumentAsNumber(args, 0);
                        var openCollIds = NakedObjects.openCollectionIds(_this.routeData());
                        var coll = obj.collectionMember(openCollIds[0]);
                        //Safe to assume always a List (Cicero doesn't support tables as such & must be open)
                        _this.context.getCollectionDetails(coll, NakedObjects.CollectionViewState.List, false).
                            then(function (details) { return _this.attemptGotoLinkNumber(itemNo_2, details.value()); }).
                            catch(function (reject) { return _this.error.handleError(reject); });
                    }
                    else {
                        var matchingProps = _this.matchingProperties(obj, arg0);
                        var matchingRefProps = _.filter(matchingProps, function (p) { return !p.isScalar(); });
                        var matchingColls = _this.matchingCollections(obj, arg0);
                        var s = "";
                        switch (matchingRefProps.length + matchingColls.length) {
                            case 0:
                                s = NakedObjects.noRefFieldMatch(arg0);
                                break;
                            case 1:
                                //TODO: Check for any empty reference
                                if (matchingRefProps.length > 0) {
                                    var link = matchingRefProps[0].value().link();
                                    _this.urlManager.setItem(link);
                                }
                                else {
                                    _this.openCollection(matchingColls[0]);
                                }
                                break;
                            default:
                                var props = _.reduce(matchingRefProps, function (s, prop) {
                                    return s + prop.extensions().friendlyName() + "\n";
                                }, "");
                                var colls = _.reduce(matchingColls, function (s, coll) {
                                    return s + coll.extensions().friendlyName() + "\n";
                                }, "");
                                s = "Multiple matches for " + arg0 + ":\n" + props + colls;
                        }
                        _this.clearInputAndSetMessage(s);
                    }
                }).
                    catch(function (reject) { return _this.error.handleError(reject); });
            }
        };
        ;
        Goto.prototype.attemptGotoLinkNumber = function (itemNo, links) {
            if (itemNo < 1 || itemNo > links.length) {
                this.clearInputAndSetMessage(NakedObjects.outOfItemRange(itemNo));
            }
            else {
                var link = links[itemNo - 1]; // On UI, first item is '1'
                this.urlManager.setItem(link);
            }
        };
        Goto.prototype.openCollection = function (collection) {
            this.closeAnyOpenCollections();
            this.vm.clearInput();
            this.urlManager.setCollectionMemberState(collection.collectionId(), NakedObjects.CollectionViewState.List);
        };
        return Goto;
    }(Command));
    NakedObjects.Goto = Goto;
    var Help = (function (_super) {
        __extends(Help, _super);
        function Help() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.helpCommand;
            this.helpText = NakedObjects.helpHelp;
            this.minArguments = 0;
            this.maxArguments = 1;
        }
        Help.prototype.isAvailableInCurrentContext = function () {
            return true;
        };
        Help.prototype.doExecute = function (args, chained) {
            var arg = this.argumentAsString(args, 0);
            if (!arg) {
                this.clearInputAndSetMessage(NakedObjects.basicHelp);
            }
            else if (arg === "?") {
                var commands = this.commandFactory.allCommandsForCurrentContext();
                this.clearInputAndSetMessage(commands);
            }
            else {
                try {
                    var c = this.commandFactory.getCommand(arg);
                    this.clearInputAndSetMessage(c.fullCommand + " command:\n" + c.helpText);
                }
                catch (e) {
                    this.clearInputAndSetMessage(e.message);
                }
            }
        };
        ;
        return Help;
    }(Command));
    NakedObjects.Help = Help;
    var Menu = (function (_super) {
        __extends(Menu, _super);
        function Menu() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.menuCommand;
            this.helpText = NakedObjects.menuHelp;
            this.minArguments = 0;
            this.maxArguments = 1;
        }
        Menu.prototype.isAvailableInCurrentContext = function () {
            return true;
        };
        Menu.prototype.doExecute = function (args, chained) {
            var _this = this;
            var name = this.argumentAsString(args, 0);
            this.context.getMenus()
                .then(function (menus) {
                var links = menus.value();
                if (name) {
                    //TODO: do multi-clause match
                    var exactMatches = _.filter(links, function (t) { return t.title().toLowerCase() === name; });
                    var partialMatches = _.filter(links, function (t) { return t.title().toLowerCase().indexOf(name) > -1; });
                    links = exactMatches.length === 1 ? exactMatches : partialMatches;
                }
                switch (links.length) {
                    case 0:
                        _this.clearInputAndSetMessage(NakedObjects.doesNotMatchMenu(name));
                        break;
                    case 1:
                        var menuId = links[0].rel().parms[0].value;
                        _this.urlManager.setHome();
                        _this.urlManager.clearUrlState();
                        _this.urlManager.setMenu(menuId);
                        break;
                    default:
                        var label = name ? NakedObjects.matchingMenus + "\n" : NakedObjects.allMenus + "\n";
                        var s = _.reduce(links, function (s, t) { return s + t.title() + "\n"; }, label);
                        _this.clearInputAndSetMessage(s);
                }
            });
        };
        return Menu;
    }(Command));
    NakedObjects.Menu = Menu;
    var OK = (function (_super) {
        __extends(OK, _super);
        function OK() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.okCommand;
            this.helpText = NakedObjects.okHelp;
            this.minArguments = 0;
            this.maxArguments = 0;
        }
        OK.prototype.isAvailableInCurrentContext = function () {
            return this.isDialog();
        };
        OK.prototype.doExecute = function (args, chained) {
            var _this = this;
            this.getActionForCurrentDialog().
                then(function (action) {
                if (chained && action.invokeLink().method() !== "GET") {
                    _this.mayNotBeChained(NakedObjects.queryOnlyRider);
                    return;
                }
                var fieldMap;
                if (_this.isForm()) {
                    var obj = action.parent;
                    fieldMap = _this.context.getCurrentObjectValues(obj.id()); //Props passed in as pseudo-params to action
                }
                else {
                    fieldMap = getParametersAndCurrentValue(action, _this.context);
                }
                _this.context.invokeAction(action, fieldMap).
                    then(function (result) {
                    // todo handle case where result is empty - this is no longer handled 
                    // by reject below
                    var warnings = result.extensions().warnings();
                    if (warnings) {
                        _.forEach(warnings, function (w) { return _this.vm.alert += "\nWarning: " + w; });
                    }
                    var messages = result.extensions().messages();
                    if (messages) {
                        _.forEach(messages, function (m) { return _this.vm.alert += "\n" + m; });
                    }
                    _this.urlManager.closeDialogReplaceHistory();
                }).
                    catch(function (reject) {
                    var display = function (em) {
                        var paramFriendlyName = function (paramId) { return FriendlyNameForParam(action, paramId); };
                        _this.handleErrorResponse(em, paramFriendlyName);
                    };
                    _this.error.handleErrorAndDisplayMessages(reject, display);
                });
            }).
                catch(function (reject) { return _this.error.handleError(reject); });
        };
        return OK;
    }(Command));
    NakedObjects.OK = OK;
    var Page = (function (_super) {
        __extends(Page, _super);
        function Page() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.pageCommand;
            this.helpText = NakedObjects.pageHelp;
            this.minArguments = 1;
            this.maxArguments = 1;
        }
        Page.prototype.isAvailableInCurrentContext = function () {
            return this.isList();
        };
        Page.prototype.doExecute = function (args, chained) {
            var _this = this;
            var arg = this.argumentAsString(args, 0);
            this.getList().
                then(function (listRep) {
                var numPages = listRep.pagination().numPages;
                var page = _this.routeData().page;
                var pageSize = _this.routeData().pageSize;
                if (NakedObjects.pageFirst.indexOf(arg) === 0) {
                    _this.setPage(1);
                    return;
                }
                else if (NakedObjects.pagePrevious.indexOf(arg) === 0) {
                    if (page === 1) {
                        _this.clearInputAndSetMessage(NakedObjects.alreadyOnFirst);
                    }
                    else {
                        _this.setPage(page - 1);
                    }
                }
                else if (NakedObjects.pageNext.indexOf(arg) === 0) {
                    if (page === numPages) {
                        _this.clearInputAndSetMessage(NakedObjects.alreadyOnLast);
                    }
                    else {
                        _this.setPage(page + 1);
                    }
                }
                else if (NakedObjects.pageLast.indexOf(arg) === 0) {
                    _this.setPage(numPages);
                }
                else {
                    var number = parseInt(arg);
                    if (isNaN(number)) {
                        _this.clearInputAndSetMessage(NakedObjects.pageArgumentWrong);
                        return;
                    }
                    if (number < 1 || number > numPages) {
                        _this.clearInputAndSetMessage(NakedObjects.pageNumberWrong(numPages));
                        return;
                    }
                    _this.setPage(number);
                }
            }).
                catch(function (reject) { return _this.error.handleError(reject); });
        };
        Page.prototype.setPage = function (page) {
            var pageSize = this.routeData().pageSize;
            this.urlManager.setListPaging(page, pageSize, NakedObjects.CollectionViewState.List);
        };
        return Page;
    }(Command));
    NakedObjects.Page = Page;
    var Reload = (function (_super) {
        __extends(Reload, _super);
        function Reload() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.reloadCommand;
            this.helpText = NakedObjects.reloadHelp;
            this.minArguments = 0;
            this.maxArguments = 0;
        }
        Reload.prototype.isAvailableInCurrentContext = function () {
            return this.isObject() || this.isList();
        };
        Reload.prototype.doExecute = function (args, chained) {
            this.clearInputAndSetMessage("Reload command is not yet implemented");
        };
        ;
        return Reload;
    }(Command));
    NakedObjects.Reload = Reload;
    var Root = (function (_super) {
        __extends(Root, _super);
        function Root() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.rootCommand;
            this.helpText = NakedObjects.rootHelp;
            this.minArguments = 0;
            this.maxArguments = 0;
        }
        Root.prototype.isAvailableInCurrentContext = function () {
            return this.isCollection();
        };
        Root.prototype.doExecute = function (args, chained) {
            this.closeAnyOpenCollections();
        };
        ;
        return Root;
    }(Command));
    NakedObjects.Root = Root;
    var Save = (function (_super) {
        __extends(Save, _super);
        function Save() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.saveCommand;
            this.helpText = NakedObjects.saveHelp;
            this.minArguments = 0;
            this.maxArguments = 0;
        }
        Save.prototype.isAvailableInCurrentContext = function () {
            return this.isEdit() || this.isTransient();
        };
        Save.prototype.doExecute = function (args, chained) {
            var _this = this;
            if (chained) {
                this.mayNotBeChained();
                return;
            }
            this.getObject().
                then(function (obj) {
                var props = obj.propertyMembers();
                var newValsFromUrl = _this.context.getCurrentObjectValues(obj.id());
                var propIds = new Array();
                var values = new Array();
                _.forEach(props, function (propMember, propId) {
                    if (!propMember.disabledReason()) {
                        propIds.push(propId);
                        var newVal = newValsFromUrl[propId];
                        if (newVal) {
                            values.push(newVal);
                        }
                        else if (propMember.value().isNull() &&
                            propMember.isScalar()) {
                            values.push(new Value(""));
                        }
                        else {
                            values.push(propMember.value());
                        }
                    }
                });
                var propMap = _.zipObject(propIds, values);
                var mode = obj.extensions().interactionMode();
                var toSave = mode === "form" || mode === "transient";
                var saveOrUpdate = toSave ? _this.context.saveObject : _this.context.updateObject;
                saveOrUpdate(obj, propMap, 1, true).
                    catch(function (reject) {
                    var display = function (em) { return _this.handleError(em, obj); };
                    _this.error.handleErrorAndDisplayMessages(reject, display);
                });
            }).
                catch(function (reject) { return _this.error.handleError(reject); });
        };
        ;
        Save.prototype.handleError = function (err, obj) {
            if (err.containsError()) {
                var propFriendlyName = function (propId) { return FriendlyNameForProperty(obj, propId); };
                this.handleErrorResponse(err, propFriendlyName);
            }
            else {
                this.urlManager.setInteractionMode(NakedObjects.InteractionMode.View);
            }
        };
        return Save;
    }(Command));
    NakedObjects.Save = Save;
    var Selection = (function (_super) {
        __extends(Selection, _super);
        function Selection() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.selectionCommand;
            this.helpText = NakedObjects.selectionHelp;
            this.minArguments = 1;
            this.maxArguments = 1;
        }
        Selection.prototype.isAvailableInCurrentContext = function () {
            return this.isList();
        };
        Selection.prototype.doExecute = function (args, chained) {
            var _this = this;
            //TODO: Add in sub-commands: Add, Remove, All, Clear & Show
            var arg = this.argumentAsString(args, 0);
            var _a = this.parseRange(arg), start = _a.start, end = _a.end; //'destructuring'
            this.getList().
                then(function (list) { return _this.selectItems(list, start, end); }).
                catch(function (reject) { return _this.error.handleError(reject); });
        };
        ;
        Selection.prototype.selectItems = function (list, startNo, endNo) {
            var itemNo;
            for (itemNo = startNo; itemNo <= endNo; itemNo++) {
                this.urlManager.setItemSelected(itemNo - 1, true, "");
            }
        };
        return Selection;
    }(Command));
    NakedObjects.Selection = Selection;
    var Show = (function (_super) {
        __extends(Show, _super);
        function Show() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.showCommand;
            this.helpText = NakedObjects.showHelp;
            this.minArguments = 0;
            this.maxArguments = 1;
        }
        Show.prototype.isAvailableInCurrentContext = function () {
            return this.isObject() || this.isCollection() || this.isList();
        };
        Show.prototype.doExecute = function (args, chained) {
            var _this = this;
            if (this.isCollection()) {
                var arg = this.argumentAsString(args, 0, true);
                var _a = this.parseRange(arg), start_1 = _a.start, end_1 = _a.end;
                this.getObject().
                    then(function (obj) {
                    var openCollIds = NakedObjects.openCollectionIds(_this.routeData());
                    var coll = obj.collectionMember(openCollIds[0]);
                    _this.renderCollectionItems(coll, start_1, end_1);
                }).
                    catch(function (reject) { return _this.error.handleError(reject); });
                return;
            }
            else if (this.isList()) {
                var arg = this.argumentAsString(args, 0, true);
                var _b = this.parseRange(arg), start_2 = _b.start, end_2 = _b.end;
                this.getList().
                    then(function (list) { return _this.renderItems(list, start_2, end_2); }).
                    catch(function (reject) { return _this.error.handleError(reject); });
            }
            else if (this.isObject()) {
                var fieldName_1 = this.argumentAsString(args, 0);
                this.getObject().
                    then(function (obj) {
                    var props = _this.matchingProperties(obj, fieldName_1);
                    var colls = _this.matchingCollections(obj, fieldName_1);
                    //TODO -  include these
                    var s;
                    switch (props.length + colls.length) {
                        case 0:
                            s = fieldName_1 ? NakedObjects.doesNotMatch(fieldName_1) : NakedObjects.noVisible;
                            break;
                        case 1:
                            s = props.length > 0 ? _this.renderPropNameAndValue(props[0]) : NakedObjects.renderCollectionNameAndSize(colls[0]);
                            break;
                        default:
                            s = _.reduce(props, function (s, prop) { return s + _this.renderPropNameAndValue(prop); }, "");
                            s += _.reduce(colls, function (s, coll) { return s + NakedObjects.renderCollectionNameAndSize(coll); }, "");
                    }
                    _this.clearInputAndSetMessage(s);
                }).
                    catch(function (reject) { return _this.error.handleError(reject); });
            }
        };
        ;
        Show.prototype.renderPropNameAndValue = function (pm) {
            var name = pm.extensions().friendlyName();
            var value;
            var parent = pm.parent;
            var props = this.context.getCurrentObjectValues(parent.id());
            var modifiedValue = props[pm.id()];
            if (this.isEdit() && !pm.disabledReason() && modifiedValue) {
                value = NakedObjects.renderFieldValue(pm, modifiedValue, this.mask) + (" (" + NakedObjects.modified + ")");
            }
            else {
                value = NakedObjects.renderFieldValue(pm, pm.value(), this.mask);
            }
            return name + ": " + value + "\n";
        };
        Show.prototype.renderCollectionItems = function (coll, startNo, endNo) {
            var _this = this;
            if (coll.value()) {
                this.renderItems(coll, startNo, endNo);
            }
            else {
                this.context.getCollectionDetails(coll, NakedObjects.CollectionViewState.List, false).
                    then(function (details) { return _this.renderItems(details, startNo, endNo); }).
                    catch(function (reject) { return _this.error.handleError(reject); });
            }
        };
        Show.prototype.renderItems = function (source, startNo, endNo) {
            //TODO: problem here is that unless collections are in-lined value will be null.
            var max = source.value().length;
            if (!startNo) {
                startNo = 1;
            }
            if (!endNo) {
                endNo = max;
            }
            if (startNo > max || endNo > max) {
                this.clearInputAndSetMessage(NakedObjects.highestItem(source.value().length));
                return;
            }
            if (startNo > endNo) {
                this.clearInputAndSetMessage(NakedObjects.startHigherEnd);
                return;
            }
            var output = "";
            var i;
            var links = source.value();
            for (i = startNo; i <= endNo; i++) {
                output += NakedObjects.item + " " + i + ": " + links[i - 1].title() + "\n";
            }
            this.clearInputAndSetMessage(output);
        };
        return Show;
    }(Command));
    NakedObjects.Show = Show;
    var Where = (function (_super) {
        __extends(Where, _super);
        function Where() {
            _super.apply(this, arguments);
            this.fullCommand = NakedObjects.whereCommand;
            this.helpText = NakedObjects.whereHelp;
            this.minArguments = 0;
            this.maxArguments = 0;
        }
        Where.prototype.isAvailableInCurrentContext = function () {
            return true;
        };
        Where.prototype.doExecute = function (args, chained) {
            this.$route.reload();
        };
        ;
        return Where;
    }(Command));
    NakedObjects.Where = Where;
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.cicerocommands.js.map