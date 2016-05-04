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
    var Command = (function () {
        function Command(urlManager, nglocation, commandFactory, context, navigation, $q, $route, mask) {
            this.urlManager = urlManager;
            this.nglocation = nglocation;
            this.commandFactory = commandFactory;
            this.context = context;
            this.navigation = navigation;
            this.$q = $q;
            this.$route = $route;
            this.mask = mask;
        }
        //Must be called after construction and before execute is called
        Command.prototype.initialiseWithViewModel = function (cvm) {
            this.vm = cvm;
        };
        Command.prototype.execute = function (argString, chained) {
            if (!this.isAvailableInCurrentContext()) {
                this.clearInputAndSetMessage("The command: " + this.fullCommand + " is not available in the current context");
                return;
            }
            //TODO: This could be moved into a pre-parse method as it does not depend on context
            if (argString == null) {
                if (this.minArguments > 0) {
                    this.clearInputAndSetMessage("No arguments provided");
                    return;
                }
            }
            else {
                var args = argString.split(",");
                if (args.length < this.minArguments) {
                    this.clearInputAndSetMessage("Too few arguments provided");
                    return;
                }
                else if (args.length > this.maxArguments) {
                    this.clearInputAndSetMessage("Too many arguments provided");
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
            this.clearInputAndSetMessage(this.fullCommand + " command may not be chained" + rider + ". Use Where command to see where execution stopped.");
        };
        Command.prototype.appendAsNewLineToOutput = function (text) {
            this.vm.output.concat("/n" + text);
        };
        Command.prototype.checkMatch = function (matchText) {
            if (this.fullCommand.indexOf(matchText) !== 0) {
                throw new Error("No such command: " + matchText);
            }
        };
        //argNo starts from 0.
        //If argument does not parse correctly, message will be passed to UI
        //and command aborted.
        Command.prototype.argumentAsString = function (argString, argNo, optional, toLower) {
            if (optional === void 0) { optional = false; }
            if (toLower === void 0) { toLower = true; }
            if (!argString)
                return undefined;
            if (!optional && argString.split(",").length < argNo + 1) {
                throw new Error("Too few arguments provided");
            }
            var args = argString.split(",");
            if (args.length < argNo + 1) {
                if (optional) {
                    return undefined;
                }
                else {
                    throw new Error("Required argument number " + (argNo + 1).toString + " is missing");
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
                throw new Error("Argument number " + (argNo + 1).toString() + " must be a number");
            }
            return number;
        };
        Command.prototype.parseInt = function (input) {
            if (!input) {
                return null;
            }
            var number = parseInt(input);
            if (isNaN(number)) {
                throw new Error(input + " is not a number");
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
                    throw new Error("Cannot have more than one dash in argument");
            }
            if ((range.start != null && range.start < 1) || (range.end != null && range.end < 1)) {
                throw new Error("Item number or range values must be greater than zero");
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
                    return _this.$q.when(obj); //To wrap a known object as a promise
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
                return _.every(clauses, function (clause) { return name.indexOf(clause) >= 0 ||
                    (!!path && path.toLowerCase().indexOf(clause) >= 0); });
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
            var msg = "Please complete or correct these fields:\n";
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
                if (reason === "Mandatory") {
                    msg += "required";
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
                        var rd = this.routeData().dialogFields[field.id()];
                        if (rd)
                            valuesFromRouteData_1 = rd.list(); //TODO: what if only one?
                    }
                    else if (field instanceof PropertyMember) {
                        var rd = this.routeData().props[field.id()];
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
        return Command;
    }());
    NakedObjects.Command = Command;
    var Action = (function (_super) {
        __extends(Action, _super);
        function Action() {
            _super.apply(this, arguments);
            this.fullCommand = "action";
            this.helpText = "Open the dialog for action from a menu, or from object actions.\n" +
                "A dialog is always opened for an action, even if it has no fields (parameters):\n" +
                "This is a safety mechanism, allowing the user to confirm that the action is the one intended.\n" +
                "Once the dialog fields have been completed, using the Enter command,\n" +
                "the action may then be invoked  with the OK command.\n" +
                "The action command takes two optional arguments.\n" +
                "The first is the name, or partial name, of the action.\n" +
                "If the partial name matches more than one action, a list of matches is returned but none opened.\n" +
                "If no argument is provided, a full list of available action names is returned.\n" +
                "The partial name may have more than one clause, separated by spaces.\n" +
                "these may match either parts of the action name or the sub-menu name if one exists.\n" +
                "If the action name matches a single action, then a question-mark may be added as a second\n" +
                "parameter, which will generate a more detailed description of the Action.";
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
            if (details && details != "?") {
                this.clearInputAndSetMessage("Second argument may only be a question mark -  to get action details");
                return;
            }
            if (this.isObject()) {
                this.getObject()
                    .then(function (obj) {
                    _this.processActions(match, obj.actionMembers(), details);
                });
            }
            else if (this.isMenu()) {
                this.getMenu()
                    .then(function (menu) {
                    _this.processActions(match, menu.actionMembers(), details);
                });
            }
            //TODO: handle list - CCAs
        };
        Action.prototype.processActions = function (match, actionsMap, details) {
            var actions = _.map(actionsMap, function (action) { return action; });
            if (actions.length === 0) {
                this.clearInputAndSetMessage("No actions available");
                return;
            }
            if (match) {
                actions = this.matchFriendlyNameAndOrMenuPath(actions, match);
            }
            switch (actions.length) {
                case 0:
                    this.clearInputAndSetMessage(match + " does not match any actions");
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
                    var output = match ? "Matching actions:\n" : "Actions:\n";
                    output += this.listActions(actions);
                    this.clearInputAndSetMessage(output);
            }
        };
        Action.prototype.disabledAction = function (action) {
            var output = "Action: ";
            output += action.extensions().friendlyName() + " is disabled. ";
            output += action.disabledReason();
            this.clearInputAndSetMessage(output);
        };
        Action.prototype.listActions = function (actions) {
            return _.reduce(actions, function (s, t) {
                var menupath = t.extensions().menuPath() ? t.extensions().menuPath() + " - " : "";
                var disabled = t.disabledReason() ? " (disabled: " + t.disabledReason() + ")" : "";
                return s + menupath + t.extensions().friendlyName() + disabled + "\n";
            }, "");
        };
        Action.prototype.openActionDialog = function (action) {
            var _this = this;
            this.urlManager.setDialog(action.actionId());
            this.context.getInvokableAction(action).then(function (invokable) {
                _.forEach(invokable.parameters(), function (p) {
                    var pVal = _this.valueForUrl(p.default(), p);
                    _this.urlManager.setFieldValue(action.actionId(), p, pVal);
                });
            });
        };
        Action.prototype.renderActionDetails = function (action) {
            var s = "Description for action: " + action.extensions().friendlyName();
            s += "\n" + (action.extensions().description() || "No description provided");
            this.clearInputAndSetMessage(s);
        };
        return Action;
    }(Command));
    NakedObjects.Action = Action;
    var Back = (function (_super) {
        __extends(Back, _super);
        function Back() {
            _super.apply(this, arguments);
            this.fullCommand = "back";
            this.helpText = "Move back to the previous context.";
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
            this.fullCommand = "cancel";
            this.helpText = "Leave the current activity (action dialog, or object edit), incomplete.";
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
                this.urlManager.closeDialog();
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
            this.fullCommand = "clipboard";
            this.helpText = "The clipboard command is used for temporarily\n" +
                "holding a reference to an object, so that it may be used later\n" +
                "to enter into a field.\n" +
                "Clipboard requires one argument, which may take one of four values:\n" +
                "copy, show, go, or discard\n" +
                "each of which may be abbreviated down to one character.\n" +
                "Copy copies a reference to the object being viewed into the clipboard,\n" +
                "overwriting any existing reference.\n" +
                "Show displays the content of the clipboard without using it.\n" +
                "Go takes you directly to the object held in the clipboard.\n" +
                "Discard removes any existing reference from the clipboard.\n" +
                "The reference held in the clipboard may be used within the Enter command.";
            this.minArguments = 1;
            this.maxArguments = 1;
        }
        Clipboard.prototype.isAvailableInCurrentContext = function () {
            return true;
        };
        Clipboard.prototype.doExecute = function (args, chained) {
            var sub = this.argumentAsString(args, 0);
            if ("copy".indexOf(sub) === 0) {
                this.copy();
            }
            else if ("show".indexOf(sub) === 0) {
                this.show();
            }
            else if ("go".indexOf(sub) === 0) {
                this.go();
            }
            else if ("discard".indexOf(sub) === 0) {
                this.discard();
            }
            else {
                this.clearInputAndSetMessage("Clipboard command may only be followed by copy, show, go, or discard");
            }
        };
        ;
        Clipboard.prototype.copy = function () {
            var _this = this;
            if (!this.isObject()) {
                this.clearInputAndSetMessage("Clipboard copy may only be used in the context of viewing an object");
                return;
            }
            this.getObject().then(function (obj) {
                _this.vm.clipboard = obj;
                _this.show();
            });
        };
        Clipboard.prototype.show = function () {
            if (this.vm.clipboard) {
                var label = TypePlusTitle(this.vm.clipboard);
                this.clearInputAndSetMessage("Clipboard contains: " + label);
            }
            else {
                this.clearInputAndSetMessage("Clipboard is empty");
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
            this.fullCommand = "edit";
            this.helpText = "Put an object into Edit mode.";
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
            this.fullCommand = "enter";
            this.helpText = "Enter a value into a field,\n" +
                "meaning a parameter in an action dialog,\n" +
                "or  a property on an object being edited.\n" +
                "Enter requires 2 arguments.\n" +
                "The first argument is the partial field name, which must match a single field.\n" +
                "The second optional argument specifies the value, or selection, to be entered.\n" +
                "If a question mark is provided as the second argument, the field will not be\n" +
                "updated but further details will be provided about that input field.\n" +
                "If the word paste is used as the second argument, then, provided that the field is\n" +
                "a reference field, the object reference in the clipboard will be pasted into the field.\n";
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
            this.getObject()
                .then(function (obj) {
                var fields = _this.matchingProperties(obj, fieldName);
                var s;
                switch (fields.length) {
                    case 0:
                        s = fieldName + " does not match any properties";
                        break;
                    case 1:
                        var field = fields[0];
                        if (fieldEntry === "?") {
                            //TODO: does this work in edit mode i.e. show entered value
                            s = _this.renderFieldDetails(field, field.value());
                        }
                        else {
                            _this.setField(field, fieldEntry);
                            return;
                        }
                        break;
                    default:
                        s = fieldName + " matches multiple fields:\n";
                        s += _.reduce(fields, function (s, prop) {
                            return s + prop.extensions().friendlyName() + "\n";
                        }, "");
                }
                _this.clearInputAndSetMessage(s);
            });
        };
        Enter.prototype.fieldEntryForDialog = function (fieldName, fieldEntry) {
            var _this = this;
            this.getActionForCurrentDialog().then(function (action) {
                //TODO: error -  need to get invokable action to get the params.
                var params = _.map(action.parameters(), function (param) { return param; });
                params = _this.matchFriendlyNameAndOrMenuPath(params, fieldName);
                switch (params.length) {
                    case 0:
                        _this.clearInputAndSetMessage(fieldName + " does not match any fields in the dialog");
                        break;
                    case 1:
                        if (fieldEntry === "?") {
                            var p = params[0];
                            var value = _this.routeData().dialogFields[p.id()];
                            var s = _this.renderFieldDetails(p, value);
                            _this.clearInputAndSetMessage(s);
                        }
                        else {
                            _this.setField(params[0], fieldEntry);
                        }
                        break;
                    default:
                        _this.clearInputAndSetMessage("Multiple fields match " + fieldName); //TODO: list them
                        break;
                }
            });
        };
        Enter.prototype.setField = function (field, fieldEntry) {
            if (field instanceof PropertyMember && field.disabledReason()) {
                this.clearInputAndSetMessage(field.extensions().friendlyName() + " is not modifiable");
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
                    throw new Error("Invalid case");
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
                this.urlManager.setFieldValue(this.routeData().dialogId, field, urlVal);
            }
            else if (field instanceof PropertyMember) {
                var parent_1 = field.parent;
                if (parent_1 instanceof DomainObjectRepresentation) {
                    this.urlManager.setPropertyValue(parent_1, field, urlVal);
                }
            }
        };
        Enter.prototype.handleReferenceField = function (field, fieldEntry) {
            if (this.isPaste(fieldEntry)) {
                this.handleClipboard(field);
            }
            else {
                this.clearInputAndSetMessage("Invalid entry for a reference field. Use clipboard or clip");
            }
        };
        Enter.prototype.isPaste = function (fieldEntry) {
            return "paste".indexOf(fieldEntry) === 0;
        };
        Enter.prototype.handleClipboard = function (field) {
            var _this = this;
            var ref = this.vm.clipboard;
            if (!ref) {
                this.clearInputAndSetMessage("Cannot use Clipboard as it is empty");
                return;
            }
            var paramType = field.extensions().returnType();
            var refType = ref.domainType();
            this.context.isSubTypeOf(refType, paramType)
                .then(function (isSubType) {
                if (isSubType) {
                    var obj = _this.vm.clipboard;
                    var selfLink = obj.selfLink();
                    //Need to add a title to the SelfLink as not there by default
                    selfLink.setTitle(obj.title());
                    var value = new Value(selfLink);
                    _this.setFieldValue(field, value);
                }
                else {
                    _this.clearInputAndSetMessage("Contents of Clipboard are not compatible with the field");
                }
            });
        };
        Enter.prototype.handleAutoComplete = function (field, fieldEntry) {
            var _this = this;
            //TODO: Need to check that the minimum number of characters has been entered or fail validation
            if (!field.isScalar() && this.isPaste(fieldEntry)) {
                this.handleClipboard(field);
            }
            else {
                this.context.autoComplete(field, field.id(), null, fieldEntry).then(function (choices) {
                    var matches = _this.findMatchingChoicesForRef(choices, fieldEntry);
                    _this.switchOnMatches(field, fieldEntry, matches);
                });
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
                    this.clearInputAndSetMessage("None of the choices matches " + fieldEntry);
                    break;
                case 1:
                    this.setFieldValue(field, matches[0]);
                    break;
                default:
                    var msg_1 = "Multiple matches:\n";
                    _.forEach(matches, function (m) { return msg_1 += m.toString() + "\n"; });
                    this.clearInputAndSetMessage(msg_1);
                    break;
            }
        };
        Enter.prototype.handleConditionalChoices = function (field, fieldEntry) {
            var _this = this;
            //TODO: need to cover both dialog fields and editable properties!
            var enteredFields = this.routeData().dialogFields;
            // fromPairs definition is faulty
            var args = _.fromPairs(_.map(field.promptLink().arguments(), function (v, key) { return [key, new Value(v.value)]; }));
            _.forEach(_.keys(args), function (key) {
                args[key] = enteredFields[key];
            });
            this.context.conditionalChoices(field, field.id(), null, args)
                .then(function (choices) {
                var matches = _this.findMatchingChoicesForRef(choices, fieldEntry);
                _this.switchOnMatches(field, fieldEntry, matches);
            });
        };
        Enter.prototype.renderFieldDetails = function (field, value) {
            var s = "Field name: " + field.extensions().friendlyName();
            var desc = field.extensions().description();
            s += desc ? "\nDescription: " + desc : "";
            s += "\nType: " + FriendlyTypeName(field.extensions().returnType());
            if (field instanceof PropertyMember && field.disabledReason()) {
                s += "\nUnmodifiable: " + field.disabledReason();
            }
            else {
                s += field.extensions().optional() ? "\nOptional" : "\nMandatory";
                if (field.choices()) {
                    var label = "\nChoices: ";
                    s += _.reduce(field.choices(), function (s, cho) {
                        return s + cho + " ";
                    }, label);
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
            this.fullCommand = "forward";
            this.helpText = "Move forward to next context in the history\n" +
                "(if you have previously moved back).";
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
            this.fullCommand = "gemini";
            this.helpText = "Switch to the Gemini (graphical) user interface\n" +
                "preserving the current context.";
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
            this.fullCommand = "goto";
            this.helpText = "Go to the object referenced in a property,\n" +
                "or to a collection within an object,\n" +
                "or to an object within an open list or collection.\n" +
                "Goto takes one argument.  In the context of an object\n" +
                "that is the name or partial name of the property or collection.\n" +
                "In the context of an open list or collection, it is the\n" +
                "number of the item within the list or collection (starting at 1). ";
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
                var itemNo_1 = parseInt(arg0);
                if (isNaN(itemNo_1)) {
                    this.clearInputAndSetMessage(arg0 + " is not a valid number");
                    return;
                }
                this.getList().then(function (list) {
                    _this.attemptGotoLinkNumber(itemNo_1, list.value());
                    return;
                });
                return;
            }
            if (this.isObject) {
                this.getObject()
                    .then(function (obj) {
                    if (_this.isCollection()) {
                        var itemNo_2 = _this.argumentAsNumber(args, 0);
                        var openCollIds = NakedObjects.openCollectionIds(_this.routeData());
                        var coll = obj.collectionMember(openCollIds[0]);
                        //Safe to assume always a List (Cicero doesn't support tables as such & must be open)
                        _this.context.getCollectionDetails(coll, NakedObjects.CollectionViewState.List).then(function (coll) {
                            _this.attemptGotoLinkNumber(itemNo_2, coll.value());
                            return;
                        });
                    }
                    else {
                        var matchingProps = _this.matchingProperties(obj, arg0);
                        var matchingRefProps = _.filter(matchingProps, function (p) { return !p.isScalar(); });
                        var matchingColls = _this.matchingCollections(obj, arg0);
                        var s = "";
                        switch (matchingRefProps.length + matchingColls.length) {
                            case 0:
                                s = arg0 + " does not match any reference fields or collections";
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
                });
            }
        };
        ;
        Goto.prototype.attemptGotoLinkNumber = function (itemNo, links) {
            if (itemNo < 1 || itemNo > links.length) {
                this.clearInputAndSetMessage(itemNo.toString() + " is out of range for displayed items");
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
            this.fullCommand = "help";
            this.helpText = "If no argument is specified, help provides a basic explanation of how to use Cicero.\n" +
                "If help is followed by a question mark as an argument, this lists the commands available\n" +
                "in the current context. If help is followed by another command word as an argument\n" +
                "(or an abbreviation of it), a description of the specified Command is returned.";
            this.minArguments = 0;
            this.maxArguments = 1;
            this.basicHelp = "Cicero is a user interface purpose-designed to work with speech screen reader such as JAWS.\n" +
                "The display has only two fields: a read-only output field, and a single input field.\n" +
                "The input field always has the focus.\n" +
                "Commands are types into the input field followed by the Enter key.\n" +
                "When the output field updates (either instantaneously or after the server has responded)\n" +
                "its contents will be read out automatically.\n" +
                "The user should never have to navigate around the screen.\n" +
                "Commands, such as 'action', 'field' and 'save', may be typed in full\n" +
                "or abbreviated to the first two characters or more.\n" +
                "Some commands take one or more arguments.\n" +
                "There must be a space between the command word and the first argument.\n" +
                "Arguments may contain spaces if needed.\n" +
                "If more than one argument is specified they must be separated by commas.\n" +
                "Commands are not case sensitive. The commands available to the user vary according to the context.\n" +
                "The command 'help ?' will list the commands available to the user in the current context.\n" +
                "‘help’ followed by another command word (in full or abbreviated) will give more details about  that command.\n" +
                "Some commands will change the context, for example using the Go command to navigate to an associated object\n" +
                "in which case the new context will be read out.\n" +
                "Other commands - help being an example - do not change the context, but will read out information to the user.\n" +
                "If the user needs a reminder of the current context, the 'Where' command will read the context out again.\n" +
                "Hitting Enter on the empty input field has the same effect.\n" +
                "When the user enters a command and the output has been updated, the input field will  be cleared\n" +
                "- ready for the next command. The user may recall the previous command, by hitting the up- arrow key.\n" +
                "The user might then edit or extend that previous command and hit Enter to run it again.\n" +
                "For advanced users: commands may be chained using a semi-colon between them,\n" +
                "however commands that do, or might, result in data updates cannot be chained.";
        }
        Help.prototype.isAvailableInCurrentContext = function () {
            return true;
        };
        Help.prototype.doExecute = function (args, chained) {
            var arg = this.argumentAsString(args, 0);
            if (!arg) {
                this.clearInputAndSetMessage(this.basicHelp);
            }
            else if (arg == "?") {
                var commands = this.commandFactory.allCommandsForCurrentContext();
                this.clearInputAndSetMessage(commands);
            }
            else {
                try {
                    var c = this.commandFactory.getCommand(arg);
                    this.clearInputAndSetMessage(c.fullCommand + " command:\n" + c.helpText);
                }
                catch (Error) {
                    this.clearInputAndSetMessage(Error.message);
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
            this.fullCommand = "menu";
            this.helpText = "Open a named main menu, from any context.\n" +
                "Menu takes one optional argument: the name, or partial name, of the menu.\n" +
                "If the partial name matches more than one menu, a list of matches is returned\n" +
                "but no menu is opened; if no argument is provided a list of all the menus\n" +
                "is returned.";
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
                        _this.clearInputAndSetMessage(name + " does not match any menu");
                        break;
                    case 1:
                        var menuId = links[0].rel().parms[0].value;
                        _this.urlManager.setHome();
                        _this.urlManager.clearUrlState();
                        _this.urlManager.setMenu(menuId);
                        break;
                    default:
                        var label = name ? "Matching menus:\n" : "Menus:\n";
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
            this.fullCommand = "ok";
            this.helpText = "Invoke the action currently open as a dialog.\n" +
                "Fields in the dialog should be completed before this.";
            this.minArguments = 0;
            this.maxArguments = 0;
        }
        OK.prototype.isAvailableInCurrentContext = function () {
            return this.isDialog();
        };
        OK.prototype.doExecute = function (args, chained) {
            var _this = this;
            this.getActionForCurrentDialog().then(function (action) {
                if (chained && action.invokeLink().method() !== "GET") {
                    _this.mayNotBeChained(" unless the action is query-only");
                    return;
                }
                var fieldMap;
                if (_this.isForm()) {
                    fieldMap = _this.routeData().props; //Props passed in as pseudo-params to action
                }
                else {
                    fieldMap = _this.routeData().dialogFields;
                }
                _this.context.invokeAction(action, 1, fieldMap)
                    .then(function (result) {
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
                    _this.urlManager.closeDialog();
                }).
                    catch(function (reject) {
                    var display = function (em) {
                        var paramFriendlyName = function (paramId) { return FriendlyNameForParam(action, paramId); };
                        _this.handleErrorResponse(em, paramFriendlyName);
                    };
                    _this.context.handleWrappedError(reject, null, function () { }, display);
                });
            });
        };
        return OK;
    }(Command));
    NakedObjects.OK = OK;
    var Page = (function (_super) {
        __extends(Page, _super);
        function Page() {
            _super.apply(this, arguments);
            this.fullCommand = "page";
            this.helpText = "Supports paging of returned lists.\n" +
                "The page command takes a single argument, which may be one of these four words:\n" +
                "first, previous, next, or last, \n" +
                "which may be abbreviated down to the first character.\n" +
                "Alternative, the argument may be a specific page number.";
            this.minArguments = 1;
            this.maxArguments = 1;
        }
        Page.prototype.isAvailableInCurrentContext = function () {
            return this.isList();
        };
        Page.prototype.doExecute = function (args, chained) {
            var _this = this;
            var arg = this.argumentAsString(args, 0);
            this.getList().then(function (listRep) {
                var numPages = listRep.pagination().numPages;
                var page = _this.routeData().page;
                var pageSize = _this.routeData().pageSize;
                if ("first".indexOf(arg) === 0) {
                    _this.setPage(1);
                    return;
                }
                else if ("previous".indexOf(arg) === 0) {
                    if (page === 1) {
                        _this.clearInputAndSetMessage("List is already showing the first page");
                    }
                    else {
                        _this.setPage(page - 1);
                    }
                }
                else if ("next".indexOf(arg) === 0) {
                    if (page === numPages) {
                        _this.clearInputAndSetMessage("List is already showing the last page");
                    }
                    else {
                        _this.setPage(page + 1);
                    }
                }
                else if ("last".indexOf(arg) === 0) {
                    _this.setPage(numPages);
                }
                else {
                    var number = parseInt(arg);
                    if (isNaN(number)) {
                        _this.clearInputAndSetMessage("The argument must match: first, previous, next, last, or a single number");
                        return;
                    }
                    if (number < 1 || number > numPages) {
                        _this.clearInputAndSetMessage("Specified page number must be between 1 and " + numPages);
                        return;
                    }
                    _this.setPage(number);
                }
            });
        };
        Page.prototype.setPage = function (page) {
            var pageSize = this.routeData().pageSize;
            this.urlManager.setListPaging(page, pageSize, NakedObjects.CollectionViewState.List);
        };
        return Page;
    }(Command));
    NakedObjects.Page = Page;
    var Property = (function (_super) {
        __extends(Property, _super);
        function Property() {
            _super.apply(this, arguments);
            this.fullCommand = "property";
            this.helpText = "Display the name and content of one or more properties of an object.\n" +
                "Field may take 1 argument:  the partial field name.\n" +
                "If this matches more than one property, a list of matches is returned.\n" +
                "If no argument is provided, the full list of properties is returned. ";
            this.minArguments = 0;
            this.maxArguments = 1;
        }
        Property.prototype.isAvailableInCurrentContext = function () {
            return this.isObject();
        };
        Property.prototype.doExecute = function (args, chained) {
            var _this = this;
            var fieldName = this.argumentAsString(args, 0);
            this.getObject()
                .then(function (obj) {
                var props = _this.matchingProperties(obj, fieldName);
                var colls = _this.matchingCollections(obj, fieldName);
                //TODO -  include these
                var s;
                switch (props.length + colls.length) {
                    case 0:
                        if (!fieldName) {
                            s = "No visible properties";
                        }
                        else {
                            s = fieldName + " does not match any properties";
                        }
                        break;
                    case 1:
                        if (props.length > 0) {
                            s = _this.renderPropNameAndValue(props[0]);
                        }
                        else {
                            s = _this.renderColl(colls[0]);
                        }
                        break;
                    default:
                        s = _.reduce(props, function (s, prop) {
                            return s + _this.renderPropNameAndValue(prop);
                        }, "");
                        s += _.reduce(colls, function (s, coll) {
                            return s + _this.renderColl(coll);
                        }, "");
                }
                _this.clearInputAndSetMessage(s);
            });
        };
        Property.prototype.renderPropNameAndValue = function (pm) {
            var name = pm.extensions().friendlyName();
            var value;
            var propInUrl = this.routeData().props[pm.id()];
            if (this.isEdit() && !pm.disabledReason() && propInUrl) {
                value = propInUrl.toString() + " (modified)";
            }
            else {
                value = NakedObjects.renderFieldValue(pm, pm.value(), this.mask);
            }
            return name + ": " + value + "\n";
        };
        Property.prototype.renderColl = function (coll) {
            var output = coll.extensions().friendlyName() + " (collection): ";
            switch (coll.size()) {
                case 0:
                    output += "empty";
                    break;
                case 1:
                    output += "1 item";
                    break;
                default:
                    output += coll.size() + " items";
            }
            return output + "\n";
        };
        return Property;
    }(Command));
    NakedObjects.Property = Property;
    var Reload = (function (_super) {
        __extends(Reload, _super);
        function Reload() {
            _super.apply(this, arguments);
            this.fullCommand = "reload";
            this.helpText = "Not yet implemented. Reload the data from the server for an object or a list.\n" +
                "Note that for a list, which was generated by an action, reload runs the action again, \n" +
                "thus ensuring that the list is up to date. However, reloading a list does not reload the\n" +
                "individual objects in that list, which may still be cached. Invoking Reload on an\n" +
                "individual object, however, will ensure that its fields show the latest server data.";
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
            this.fullCommand = "root";
            this.helpText = "From within an opend collection context, the root command returns\n" +
                " to the root object that owns the collection. Does not take any arguments.\n";
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
            this.fullCommand = "save";
            this.helpText = "Save the updated fields on an object that is being edited,\n" +
                "and return from edit mode to a normal view of that object";
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
            this.getObject().then(function (obj) {
                var props = obj.propertyMembers();
                var newValsFromUrl = _this.routeData().props;
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
                var saveOrUpdate = (mode === "form" || mode === "transient") ? _this.context.saveObject : _this.context.updateObject;
                saveOrUpdate(obj, propMap, 1, true).
                    catch(function (reject) {
                    var display = function (em) { return _this.handleError(em, obj); };
                    _this.context.handleWrappedError(reject, null, function () { }, display);
                });
            });
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
            this.fullCommand = "selection";
            this.helpText = "Not fully implemented. Select one or more items from a list,\n" +
                "prior to invoking an action on the selection.\n" +
                "Selection has one mandatory argument, which must be one of these words,\n" +
                "add, remove, all, clear, show.\n" +
                "The Add and Remove options must be followed by a second argument specifying\n" +
                "the item number, or range, to be added or removed.\n";
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
            this.getList().then(function (list) {
                _this.selectItems(list, start, end);
            });
        };
        ;
        Selection.prototype.selectItems = function (list, startNo, endNo) {
            var itemNo;
            for (itemNo = startNo; itemNo <= endNo; itemNo++) {
                this.urlManager.setListItem(itemNo - 1, true);
            }
        };
        return Selection;
    }(Command));
    NakedObjects.Selection = Selection;
    var Show = (function (_super) {
        __extends(Show, _super);
        function Show() {
            _super.apply(this, arguments);
            this.fullCommand = "show";
            this.helpText = "Show one or more of the items from or a list view,\n" +
                "or an opened object collection. If no arguments are specified, \n" +
                "show will list all of the the items in the opened object collection,\n" +
                "or the first page of items if in a list view.\n" +
                "Alternatively, the command may be specified with an item number,\n" +
                "or a range such as 3-5.";
            this.minArguments = 0;
            this.maxArguments = 1;
        }
        Show.prototype.isAvailableInCurrentContext = function () {
            return this.isCollection() || this.isList();
        };
        Show.prototype.doExecute = function (args, chained) {
            var _this = this;
            var arg = this.argumentAsString(args, 0, true);
            var _a = this.parseRange(arg), start = _a.start, end = _a.end;
            if (this.isCollection()) {
                this.getObject().then(function (obj) {
                    var openCollIds = NakedObjects.openCollectionIds(_this.routeData());
                    var coll = obj.collectionMember(openCollIds[0]);
                    _this.renderCollectionItems(coll, start, end);
                });
                return;
            }
            //must be List
            this.getList().then(function (list) {
                _this.renderItems(list, start, end);
            });
        };
        ;
        Show.prototype.renderCollectionItems = function (coll, startNo, endNo) {
            var _this = this;
            if (coll.value()) {
                this.renderItems(coll, startNo, endNo);
            }
            else {
                this.context.getCollectionDetails(coll, NakedObjects.CollectionViewState.List).then(function (details) {
                    _this.renderItems(details, startNo, endNo);
                });
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
                this.clearInputAndSetMessage("The highest numbered item is " + source.value().length);
                return;
            }
            if (startNo > endNo) {
                this.clearInputAndSetMessage("Starting item number cannot be greater than the ending item number");
                return;
            }
            var output = "";
            var i;
            var links = source.value();
            for (i = startNo; i <= endNo; i++) {
                output += "Item " + i + ": " + links[i - 1].title() + "\n";
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
            this.fullCommand = "where";
            this.helpText = "Display a reminder of the current context.\n" +
                "The same can also be achieved by hitting the Return key on the empty input field.";
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