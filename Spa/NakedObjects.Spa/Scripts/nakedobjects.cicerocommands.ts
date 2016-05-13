/// <reference path="nakedobjects.models.ts" />

module NakedObjects {
    import IHasExtensions = Models.IHasExtensions;
    import DomainObjectRepresentation = Models.DomainObjectRepresentation;
    import ListRepresentation = Models.ListRepresentation;
    import MenuRepresentation = Models.MenuRepresentation;
    import ActionMember = Models.ActionMember;
    import ErrorWrapper = Models.ErrorWrapper;
    import ErrorCategory = Models.ErrorCategory;
    import ClientErrorCode = Models.ClientErrorCode;
    import PropertyMember = Models.PropertyMember;
    import CollectionMember = Models.CollectionMember;
    import Parameter = Models.Parameter;
    import Value = Models.Value;
    import ErrorMap = Models.ErrorMap;
    import ErrorValue = Models.ErrorValue;
    import IField = Models.IField;
    import EntryType = Models.EntryType;
    import MenusRepresentation = Models.MenusRepresentation;
    import ActionResultRepresentation = Models.ActionResultRepresentation;
    import IHasLinksAsValue = Models.IHasLinksAsValue;
    import TypePlusTitle = Models.typePlusTitle;
    import FriendlyTypeName = Models.friendlyTypeName;
    import FriendlyNameForParam = Models.friendlyNameForParam;
    import FriendlyNameForProperty = Models.friendlyNameForProperty;
    import isDateOrDateTime = Models.isDateOrDateTime;
    import toDateString = Models.toDateString;
    import CollectionRepresentation = Models.CollectionRepresentation;
    import ObjectIdWrapper = Models.ObjectIdWrapper;
    import InvokableActionMember = Models.InvokableActionMember;
    import IInvokableAction = Models.IInvokableAction;

    export abstract class Command {

        constructor(protected urlManager: IUrlManager,
            protected nglocation: ng.ILocationService,
            protected commandFactory: ICommandFactory,
            protected context: IContext,
            protected navigation: INavigation,
            protected $q: ng.IQService,
            protected $route: ng.route.IRouteService,
            protected mask: IMask
        ) { }

        fullCommand: string;
        helpText: string;
        protected minArguments: number;
        protected maxArguments: number;
        protected vm: CiceroViewModel;

        //Must be called after construction and before execute is called
        initialiseWithViewModel(cvm: CiceroViewModel) {
            this.vm = cvm;
        }

        execute(argString: string, chained: boolean): void {
            if (!this.isAvailableInCurrentContext()) {
                this.clearInputAndSetMessage(`The command: ${this.fullCommand} is not available in the current context`);
                return;
            }
            //TODO: This could be moved into a pre-parse method as it does not depend on context
            if (argString == null) {
                if (this.minArguments > 0) {
                    this.clearInputAndSetMessage("No arguments provided");
                    return;
                }
            } else {
                const args = argString.split(",");
                if (args.length < this.minArguments) {
                    this.clearInputAndSetMessage("Too few arguments provided");
                    return;
                } else if (args.length > this.maxArguments) {
                    this.clearInputAndSetMessage("Too many arguments provided");
                    return;
                }
            }
            this.doExecute(argString, chained);
        }

        abstract doExecute(args: string, chained: boolean): void;

        abstract isAvailableInCurrentContext(): boolean;

        //Helper methods follow
        protected clearInputAndSetMessage(text: string): void {
            this.vm.clearInput();
            this.vm.message = text;
            this.$route.reload();
        }

        protected mayNotBeChained(rider: string = ""): void {
            this.clearInputAndSetMessage(this.fullCommand + " command may not be chained" + rider + ". Use Where command to see where execution stopped.");
        }

        protected appendAsNewLineToOutput(text: string): void {
            this.vm.output.concat(`/n${text}`);
        }

        checkMatch(matchText: string): void {
            if (this.fullCommand.indexOf(matchText) !== 0) {
                throw new Error(`No such command: ${matchText}`);
            }
        }

        //argNo starts from 0.
        //If argument does not parse correctly, message will be passed to UI
        //and command aborted.
        protected argumentAsString(argString: string, argNo: number, optional: boolean = false, toLower: boolean = true): string {
            if (!argString) return undefined;
            if (!optional && argString.split(",").length < argNo + 1) {
                throw new Error("Too few arguments provided");
            }
            const args = argString.split(",");
            if (args.length < argNo + 1) {
                if (optional) {
                    return undefined;
                } else {
                    throw new Error(`Required argument number ${(argNo + 1).toString} is missing`);
                }
            }
            return toLower ? args[argNo].trim().toLowerCase() : args[argNo].trim(); // which may be "" if argString ends in a ','
        }

        //argNo starts from 0.
        protected argumentAsNumber(args: string, argNo: number, optional: boolean = false): number {
            const arg = this.argumentAsString(args, argNo, optional);
            if (!arg && optional) return null;
            const number = parseInt(arg);
            if (isNaN(number)) {
                throw new Error(`Argument number ${(argNo + 1).toString() } must be a number`);
            }
            return number;
        }

        protected parseInt(input: string): number {
            if (!input) {
                return null;
            }
            const number = parseInt(input);
            if (isNaN(number)) {
                throw new Error(input + " is not a number");
            }
            return number;
        }

        //Parses '17, 3-5, -9, 6-' into two numbers 
        protected parseRange(arg: string): { start: number, end: number } {
            if (!arg) {
                arg = "1-";
            }
            const clauses = arg.split("-");
            const range = { start: null as number, end: null as number };
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
        }

        protected getContextDescription(): string {
            //todo
            return null;
        }

        protected routeData(): PaneRouteData {
            return this.urlManager.getRouteData().pane1;
        }

        //Helpers delegating to RouteData
        protected isHome(): boolean {
            return this.vm.viewType === ViewType.Home;
        }

        protected isObject(): boolean {
            return this.vm.viewType === ViewType.Object;
        }

        protected getObject(): ng.IPromise<DomainObjectRepresentation> {
            const oid =  ObjectIdWrapper.fromObjectId(this.routeData().objectId);
            //TODO: Consider view model & transient modes?

            return this.context.getObject(1, oid, this.routeData().interactionMode).then((obj: DomainObjectRepresentation) => {
                if (this.routeData().interactionMode === InteractionMode.Edit) {
                    return this.context.getObjectForEdit(1, obj);
                } else {
                    return this.$q.when(obj); //To wrap a known object as a promise
                }
            });
        }

        protected isList(): boolean {
            return this.vm.viewType === ViewType.List;
        }

        protected getList(): ng.IPromise<ListRepresentation> {
            const routeData = this.routeData();
            //TODO: Currently covers only the list-from-menu; need to cover list from object action
            return this.context.getListFromMenu(1, routeData, routeData.page, routeData.pageSize);
        }

        protected isMenu(): boolean {
            return !!this.routeData().menuId;
        }

        protected getMenu(): ng.IPromise<MenuRepresentation> {
            return this.context.getMenu(this.routeData().menuId);
        }

        protected isDialog(): boolean {
            return !!this.routeData().dialogId;
        }

        protected getActionForCurrentDialog(): ng.IPromise<Models.IInvokableAction> {
            const dialogId = this.routeData().dialogId;
            if (this.isObject()) {
                return this.getObject().then((obj: DomainObjectRepresentation) => this.context.getInvokableAction(obj.actionMember(dialogId)));
            } else if (this.isMenu()) {
                return this.getMenu().then((menu: MenuRepresentation) => this.context.getInvokableAction(menu.actionMember(dialogId))); //i.e. return a promise
            }
            return this.$q.reject(new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.NotImplemented, "List actions not implemented yet"));
        }

        //Tests that at least one collection is open (should only be one). 
        //TODO: assumes that closing collection removes it from routeData NOT sets it to Summary
        protected isCollection(): boolean {
            return this.isObject() && _.some(this.routeData().collections);
        }

        protected closeAnyOpenCollections() {
            const open = openCollectionIds(this.routeData());
            _.forEach(open, id => {
                this.urlManager.setCollectionMemberState(id, CollectionViewState.Summary);
            });
        }

        protected isTable(): boolean {
            return false; //TODO
        }

        protected isEdit(): boolean {
            return this.routeData().interactionMode === InteractionMode.Edit;
        }

        protected isForm(): boolean {
            return this.routeData().interactionMode === InteractionMode.Form;
        }

        protected isTransient(): boolean {
            return this.routeData().interactionMode === InteractionMode.Transient;
        }

        protected matchingProperties(
            obj: DomainObjectRepresentation,
            match: string): PropertyMember[] {
            let props = _.map(obj.propertyMembers(), prop => prop);
            if (match) {
                props = this.matchFriendlyNameAndOrMenuPath(props, match);
            }
            return props;
        }

        protected matchingCollections(
            obj: DomainObjectRepresentation,
            match: string): CollectionMember[] {
            const allColls = _.map(obj.collectionMembers(), action => action);
            if (match) {
                return this.matchFriendlyNameAndOrMenuPath<CollectionMember>(allColls, match);
            } else {
                return allColls;
            }
        }

        protected matchingParameters(action: InvokableActionMember, match: string): Parameter[] {
            let params = _.map(action.parameters(), p => p);
            if (match) {
                params = this.matchFriendlyNameAndOrMenuPath(params, match);
            }
            return params;
        }

        protected matchFriendlyNameAndOrMenuPath<T extends IHasExtensions>(
            reps: T[], match: string): T[] {
            const clauses = match.split(" ");
            //An exact match has preference over any partial match
            const exactMatches = _.filter(reps, (rep) => {
                const path = rep.extensions().menuPath();
                const name = rep.extensions().friendlyName().toLowerCase();
                return match === name ||
                    (!!path && match === path.toLowerCase() + " " + name) ||
                    _.every(clauses, clause => name === clause || (!!path && path.toLowerCase() === clause));
            });
            if (exactMatches.length > 0) return exactMatches;
            return _.filter(reps, (rep) => {
                const path = rep.extensions().menuPath();
                const name = rep.extensions().friendlyName().toLowerCase();
                return _.every(clauses, clause => name.indexOf(clause) >= 0 ||
                    (!!path && path.toLowerCase().indexOf(clause) >= 0));
            });
        }

        protected findMatchingChoicesForRef(choices: _.Dictionary<Value>, titleMatch: string): Value[] {
            return _.filter(choices, v => v.toString().toLowerCase().indexOf(titleMatch.toLowerCase()) >= 0);
        }

        protected findMatchingChoicesForScalar(choices: _.Dictionary<Value>, titleMatch: string): Value[] {
            const labels = _.keys(choices);
            const matchingLabels = _.filter(labels, l => l.toString().toLowerCase().indexOf(titleMatch.toLowerCase()) >= 0);
            const result = new Array<Value>();
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
                    _.forEach(matchingLabels, label => result.push(new Value(label)));
                    break;
            }
            return result;
        }

        protected handleErrorResponse(err: ErrorMap, getFriendlyName: (id: string) => string) {
            if (err.invalidReason()) {
                this.clearInputAndSetMessage(err.invalidReason());
                return;
            }
            let msg = "Please complete or correct these fields:\n";
            _.each(err.valuesMap(), (errorValue, fieldId) => {
                msg += this.fieldValidationMessage(errorValue, () => getFriendlyName(fieldId));
            });
            this.clearInputAndSetMessage(msg);
        }

        private fieldValidationMessage(errorValue: ErrorValue, fieldFriendlyName: () => string): string {
            let msg = "";
            const reason = errorValue.invalidReason;
            const value = errorValue.value;
            if (reason) {
                msg += fieldFriendlyName() + ": ";
                if (reason === "Mandatory") {
                    msg += "required";
                } else {
                    msg += value + " " + reason;
                }
                msg += "\n";
            }
            return msg;
        }

        protected valueForUrl(val: Value, field: IField): Value {
            if (val.isNull()) return val;
            const fieldEntryType = field.entryType();

            if (fieldEntryType !== EntryType.FreeForm || field.isCollectionContributed()) {

                if (fieldEntryType === EntryType.MultipleChoices || field.isCollectionContributed()) {
                    let valuesFromRouteData = new Array<Value>();
                    if (field instanceof Parameter) {
                        const rd = this.routeData().dialogFields[field.id()];
                        if (rd) valuesFromRouteData = rd.list(); //TODO: what if only one?
                    } else if (field instanceof PropertyMember) {
                        const rd = this.routeData().props[field.id()];
                        if (rd) valuesFromRouteData = rd.list(); //TODO: what if only one?
                    }
                    let vals: Value[] = [];
                    if (val.isReference() || val.isScalar()) {
                        vals = new Array<Value>(val);
                    } else if (val.isList()) { //Should be!
                        vals = val.list();
                    }
                    _.forEach(vals, v => {
                        this.addOrRemoveValue(valuesFromRouteData, v);
                    });
                    if (vals[0].isScalar()) { //then all must be scalar
                        const scalars = _.map(valuesFromRouteData, v => v.scalar());
                        return new Value(scalars);
                    } else { //assumed to be links
                        const links = _.map(valuesFromRouteData, v => (
                            { href: v.link().href(), title: v.link().title() }
                        ));
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
                //TODO: consider these options:
                //    if (from.value instanceof Date) {
                //        return new Value((from.value as Date).toISOString());
                //    }

                //    return new Value(from.value as number | string | boolean);
            }
            if (val.isReference()) {
                return this.leanLink(val);
            }
            return null;
        }

        private leanLink(val: Value): Value {
            return new Value({ href: val.link().href(), title: val.link().title() });
        }

        private addOrRemoveValue(valuesFromRouteData: Value[], val: Value) {
            let index: number;
            let valToAdd: Value;
            if (val.isScalar()) {
                valToAdd = val;
                index = _.findIndex(valuesFromRouteData, v => v.scalar() === val.scalar());
            } else { //Must be reference
                valToAdd = this.leanLink(val);
                index = _.findIndex(valuesFromRouteData, v => v.link().href() === valToAdd.link().href());
            }
            if (index > -1) {
                valuesFromRouteData.splice(index, 1);
            } else {
                valuesFromRouteData.push(valToAdd);
            }
        }
    }

    export class Action extends Command {

        fullCommand = "action";
        helpText = "Open the dialog for action from a menu, or from object actions.\n" +
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

        protected minArguments = 0;
        protected maxArguments = 2;

        isAvailableInCurrentContext(): boolean {
            return (this.isMenu() || this.isObject() || this.isForm()) && !this.isDialog() && !this.isEdit(); //TODO add list
        }

        doExecute(args: string, chained: boolean): void {
            const match = this.argumentAsString(args, 0);
            const details = this.argumentAsString(args, 1, true);
            if (details && details != "?") {
                this.clearInputAndSetMessage("Second argument may only be a question mark -  to get action details");
                return;
            }
            if (this.isObject()) {
                this.getObject()
                    .then((obj: DomainObjectRepresentation) => {
                        this.processActions(match, obj.actionMembers(), details);
                    });
            } else if (this.isMenu()) {
                this.getMenu()
                    .then((menu: MenuRepresentation) => {
                        this.processActions(match, menu.actionMembers(), details);
                    });
            }
            //TODO: handle list - CCAs
        }

        private processActions(match: string, actionsMap: _.Dictionary<ActionMember>, details: string) {
            let actions = _.map(actionsMap, action => action);
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
                    const action = actions[0];
                    if (details) {
                        this.renderActionDetails(action);
                    } else if (action.disabledReason()) {
                        this.disabledAction(action);
                    } else {
                        this.openActionDialog(action);
                    }
                    break;
                default:
                    let output = match ? "Matching actions:\n" : "Actions:\n";
                    output += this.listActions(actions);
                    this.clearInputAndSetMessage(output);
            }
        }

        private disabledAction(action: ActionMember) {
            let output = "Action: ";
            output += action.extensions().friendlyName() + " is disabled. ";
            output += action.disabledReason();
            this.clearInputAndSetMessage(output);
        }

        private listActions(actions: ActionMember[]): string {
            return _.reduce(actions, (s, t) => {
                const menupath = t.extensions().menuPath() ? t.extensions().menuPath() + " - " : "";
                const disabled = t.disabledReason() ? ` (disabled: ${t.disabledReason() })` : "";
                return s + menupath + t.extensions().friendlyName() + disabled + "\n";
            }, "");
        }

        private openActionDialog(action: ActionMember) {
            this.urlManager.setDialog(action.actionId());
            this.context.getInvokableAction(action).then((invokable: Models.IInvokableAction) => {
                _.forEach(invokable.parameters(), (p) => {
                    const pVal = this.valueForUrl(p.default(), p);
                    this.urlManager.setFieldValue(action.actionId(), p, pVal);
                });
            });
        }

        private renderActionDetails(action: ActionMember) {
            let s = `Description for action: ${action.extensions().friendlyName() }`;
            s += `\n${action.extensions().description() || "No description provided"}`;
            this.clearInputAndSetMessage(s);
        }
    }

    export class Back extends Command {

        fullCommand = "back";
        helpText = "Move back to the previous context.";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        doExecute(args: string, chained: boolean): void {
            this.navigation.back();
        };
    }

    export class Cancel extends Command {

        fullCommand = "cancel";
        helpText = "Leave the current activity (action dialog, or object edit), incomplete.";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isDialog() || this.isEdit();
        }

        doExecute(args: string, chained: boolean): void {
            if (this.isEdit()) {
                this.urlManager.setInteractionMode(InteractionMode.View);
            }
            if (this.isDialog()) {
                this.urlManager.closeDialog();
            }
        };
    }

    export class Clipboard extends Command {

        fullCommand = "clipboard";
        helpText = "The clipboard command is used for temporarily\n" +
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

        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        doExecute(args: string, chained: boolean): void {
            const sub = this.argumentAsString(args, 0);
            if ("copy".indexOf(sub) === 0) {
                this.copy();
            } else if ("show".indexOf(sub) === 0) {
                this.show();
            } else if ("go".indexOf(sub) === 0) {
                this.go();
            } else if ("discard".indexOf(sub) === 0) {
                this.discard();
            } else {
                this.clearInputAndSetMessage("Clipboard command may only be followed by copy, show, go, or discard");
            }
        };

        private copy(): void {
            if (!this.isObject()) {
                this.clearInputAndSetMessage("Clipboard copy may only be used in the context of viewing an object");
                return;
            }
            this.getObject().then((obj: DomainObjectRepresentation) => {
                this.vm.clipboard = obj;
                this.show();
            });
        }

        private show(): void {
            if (this.vm.clipboard) {
                const label = TypePlusTitle(this.vm.clipboard);
                this.clearInputAndSetMessage(`Clipboard contains: ${label}`);
            } else {
                this.clearInputAndSetMessage("Clipboard is empty");
            }
        }

        private go(): void {
            const link = this.vm.clipboard.selfLink();
            if (link) {
                this.urlManager.setItem(link);
            } else {
                this.show();
            }
        }

        private discard(): void {
            this.vm.clipboard = null;
            this.show();
        }
    }

    export class Edit extends Command {

        fullCommand = "edit";
        helpText = "Put an object into Edit mode.";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() && !this.isEdit();
        }

        doExecute(args: string, chained: boolean): void {
            if (chained) {
                this.mayNotBeChained();
                return;
            }
            this.urlManager.setInteractionMode(InteractionMode.Edit);
        };
    }

    export class Enter extends Command {

        fullCommand = "enter";
        helpText = "Enter a value into a field,\n" +
        "meaning a parameter in an action dialog,\n" +
        "or  a property on an object being edited.\n" +
        "Enter requires 2 arguments.\n" +
        "The first argument is the partial field name, which must match a single field.\n" +
        "The second optional argument specifies the value, or selection, to be entered.\n" +
        "If a question mark is provided as the second argument, the field will not be\n" +
        "updated but further details will be provided about that input field.\n" +
        "If the word paste is used as the second argument, then, provided that the field is\n" +
        "a reference field, the object reference in the clipboard will be pasted into the field.\n";
        protected minArguments = 2;
        protected maxArguments = 2;

        isAvailableInCurrentContext(): boolean {
            return this.isDialog() || this.isEdit() || this.isTransient() || this.isForm();
        }

        doExecute(args: string, chained: boolean): void {
            const fieldName = this.argumentAsString(args, 0);
            const fieldEntry = this.argumentAsString(args, 1, false, false);
            if (this.isDialog()) {
                this.fieldEntryForDialog(fieldName, fieldEntry);
            } else {
                this.fieldEntryForEdit(fieldName, fieldEntry);
            }
        };

        private fieldEntryForEdit(fieldName: string, fieldEntry: string) {
            this.getObject()
                .then((obj: DomainObjectRepresentation) => {
                    const fields = this.matchingProperties(obj, fieldName);
                    let s: string;
                    switch (fields.length) {
                        case 0:
                            s = fieldName + " does not match any properties";
                            break;
                        case 1:
                            const field = fields[0];
                            if (fieldEntry === "?") {
                                //TODO: does this work in edit mode i.e. show entered value
                                s = this.renderFieldDetails(field, field.value());
                            } else {
                                this.setField(field, fieldEntry);
                                return;
                            }
                            break;
                        default:
                            s = fieldName + " matches multiple fields:\n";
                            s += _.reduce(fields, (s, prop) => {
                                return s + prop.extensions().friendlyName() + "\n";
                            }, "");
                    }
                    this.clearInputAndSetMessage(s);
                });
        }

        private fieldEntryForDialog(fieldName: string, fieldEntry: string) {
            this.getActionForCurrentDialog().then((action: Models.IInvokableAction) => {
                //TODO: error -  need to get invokable action to get the params.
                let params = _.map(action.parameters(), param => param);
                params = this.matchFriendlyNameAndOrMenuPath(params, fieldName);
                switch (params.length) {
                    case 0:
                        this.clearInputAndSetMessage(fieldName + " does not match any fields in the dialog");
                        break;
                    case 1:
                        if (fieldEntry === "?") {
                            const p = params[0];
                            const value = this.routeData().dialogFields[p.id()];
                            const s = this.renderFieldDetails(p, value);
                            this.clearInputAndSetMessage(s);
                        } else {
                            this.setField(params[0], fieldEntry);
                        }
                        break;
                    default:
                        this.clearInputAndSetMessage(`Multiple fields match ${fieldName}`); //TODO: list them
                        break;
                }
            });
        }

        private setField(field: IField, fieldEntry: string): void {
            if (field instanceof PropertyMember && field.disabledReason()) {
                this.clearInputAndSetMessage(field.extensions().friendlyName() + " is not modifiable");
                return;
            }
            const entryType = field.entryType();
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
        }

        private handleFreeForm(field: IField, fieldEntry: string): void {
            if (field.isScalar()) {
                let value: Value = new Value(fieldEntry);
                //TODO: handle a non-parsable date
                if (isDateOrDateTime(field)) {
                    const m = moment(fieldEntry, supportedDateFormats, "en-GB", true);

                    if (m.isValid()) {
                        const dt = m.toDate();
                        value = new Value(toDateString(dt));
                    }
                }
                this.setFieldValue(field, value);
            } else {
                this.handleReferenceField(field, fieldEntry);
            }
        }

        private setFieldValue(field: IField, value: Value): void {
            const urlVal = this.valueForUrl(value, field);
            if (field instanceof Parameter) {
                this.urlManager.setFieldValue(this.routeData().dialogId, field, urlVal);
            } else if (field instanceof PropertyMember) {
                const parent = field.parent;
                if (parent instanceof DomainObjectRepresentation) {
                    this.urlManager.setPropertyValue(parent, field, urlVal);
                }
            }
        }

        private handleReferenceField(field: IField, fieldEntry: string): void {
            if (this.isPaste(fieldEntry)) {
                this.handleClipboard(field);
            } else {
                this.clearInputAndSetMessage("Invalid entry for a reference field. Use clipboard or clip");
            }
        }

        private isPaste(fieldEntry: string) {
            return "paste".indexOf(fieldEntry) === 0;
        }

        private handleClipboard(field: IField): void {
            const ref = this.vm.clipboard;
            if (!ref) {
                this.clearInputAndSetMessage("Cannot use Clipboard as it is empty");
                return;
            }
            const paramType = field.extensions().returnType();
            const refType = ref.domainType();
            this.context.isSubTypeOf(refType, paramType)
                .then((isSubType: boolean) => {
                    if (isSubType) {
                        const obj = this.vm.clipboard;
                        const selfLink = obj.selfLink();
                        //Need to add a title to the SelfLink as not there by default
                        selfLink.setTitle(obj.title());
                        const value = new Value(selfLink);
                        this.setFieldValue(field, value);
                    } else {
                        this.clearInputAndSetMessage("Contents of Clipboard are not compatible with the field");
                    }
                });
        }

        private handleAutoComplete(field: IField, fieldEntry: string): void {
            //TODO: Need to check that the minimum number of characters has been entered or fail validation
            if (!field.isScalar() && this.isPaste(fieldEntry)) {
                this.handleClipboard(field);
            } else {
                this.context.autoComplete(field, field.id(), null, fieldEntry).then(
                    (choices: _.Dictionary<Value>) => {
                        const matches = this.findMatchingChoicesForRef(choices, fieldEntry);
                        this.switchOnMatches(field, fieldEntry, matches);
                    });
            }
        }

        private handleChoices(field: IField, fieldEntry: string): void {
            let matches: Value[];
            if (field.isScalar()) {
                matches = this.findMatchingChoicesForScalar(field.choices(), fieldEntry);
            } else {
                matches = this.findMatchingChoicesForRef(field.choices(), fieldEntry);
            }
            this.switchOnMatches(field, fieldEntry, matches);
        }

        private switchOnMatches(field: IField, fieldEntry: string, matches: Value[]) {
            switch (matches.length) {
                case 0:
                    this.clearInputAndSetMessage(`None of the choices matches ${fieldEntry}`);
                    break;
                case 1:
                    this.setFieldValue(field, matches[0]);
                    break;
                default:
                    let msg = "Multiple matches:\n";
                    _.forEach(matches, m => msg += m.toString() + "\n");
                    this.clearInputAndSetMessage(msg);
                    break;
            }
        }

        private handleConditionalChoices(field: IField, fieldEntry: string): void {
            //TODO: need to cover both dialog fields and editable properties!
            const enteredFields = this.routeData().dialogFields;

            // fromPairs definition is faulty
            const args = (<any>_).fromPairs(_.map(field.promptLink().arguments(), (v: any, key : string) => [key, new Value(v.value)])) as _.Dictionary<Value>;
            _.forEach(_.keys(args), key => {
                args[key] = enteredFields[key];
            });
            this.context.conditionalChoices(field, field.id(), null, args)
                .then((choices: _.Dictionary<Value>) => {
                    const matches = this.findMatchingChoicesForRef(choices, fieldEntry);
                    this.switchOnMatches(field, fieldEntry, matches);
                });
        }

        private renderFieldDetails(field: IField, value: Value): string {
            let s = `Field name: ${field.extensions().friendlyName() }`;
            const desc = field.extensions().description();
            s += desc ? `\nDescription: ${desc}` : "";
            s += `\nType: ${FriendlyTypeName(field.extensions().returnType()) }`;
            if (field instanceof PropertyMember && field.disabledReason()) {
                s += `\nUnmodifiable: ${field.disabledReason() }`;
            } else {
                s += field.extensions().optional() ? "\nOptional" : "\nMandatory";
                if (field.choices()) {
                    const label = "\nChoices: ";
                    s += _.reduce(field.choices(), (s, cho) => {
                        return s + cho + " ";
                    }, label);
                }
            }
            return s;
        }
    }

    export class Forward extends Command {

        fullCommand = "forward";
        helpText = "Move forward to next context in the history\n" +
        "(if you have previously moved back).";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        doExecute(args: string, chained: boolean): void {
            this.vm.clearInput(); //To catch case where can't go any further forward and hence url does not change.
            this.navigation.forward();
        };
    }

    export class Gemini extends Command {

        fullCommand = "gemini";
        helpText = "Switch to the Gemini (graphical) user interface\n" +
        "preserving the current context.";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        doExecute(args: string, chained: boolean): void {
            const newPath = `/gemini/${this.nglocation.path().split("/")[2]}`;
            this.nglocation.path(newPath);
        };
    }

    export class Goto extends Command {

        fullCommand = "goto";
        helpText = "Go to the object referenced in a property,\n" +
        "or to a collection within an object,\n" +
        "or to an object within an open list or collection.\n" +
        "Goto takes one argument.  In the context of an object\n" +
        "that is the name or partial name of the property or collection.\n" +
        "In the context of an open list or collection, it is the\n" +
        "number of the item within the list or collection (starting at 1). ";
        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() || this.isList();
        }

        doExecute(args: string, chained: boolean): void {
            const arg0 = this.argumentAsString(args, 0);
            if (this.isList()) {
                const itemNo = parseInt(arg0);
                if (isNaN(itemNo)) {
                    this.clearInputAndSetMessage(arg0 + " is not a valid number");
                    return;
                }
                this.getList().then((list: ListRepresentation) => {
                    this.attemptGotoLinkNumber(itemNo, list.value());
                    return;
                });
                return;
            }
            if (this.isObject) {
                this.getObject()
                    .then((obj: DomainObjectRepresentation) => {
                        if (this.isCollection()) {
                            const itemNo = this.argumentAsNumber(args, 0);
                            const openCollIds = openCollectionIds(this.routeData());
                            const coll = obj.collectionMember(openCollIds[0]);
                            //Safe to assume always a List (Cicero doesn't support tables as such & must be open)
                            this.context.getCollectionDetails(coll, CollectionViewState.List).then((coll: CollectionRepresentation) => {
                                this.attemptGotoLinkNumber(itemNo, coll.value());
                                return;
                        });
                        } else {
                            const matchingProps = this.matchingProperties(obj, arg0);
                            const matchingRefProps = _.filter(matchingProps, (p) => { return !p.isScalar() });
                            const matchingColls = this.matchingCollections(obj, arg0);
                            let s = "";
                            switch (matchingRefProps.length + matchingColls.length) {
                                case 0:
                                    s = arg0 + " does not match any reference fields or collections";
                                    break;
                                case 1:
                                    //TODO: Check for any empty reference
                                    if (matchingRefProps.length > 0) {
                                        const link = matchingRefProps[0].value().link();
                                        this.urlManager.setItem(link);
                                    } else { //Must be collection
                                        this.openCollection(matchingColls[0]);
                                    }
                                    break;
                                default:
                                    const props = _.reduce(matchingRefProps, (s, prop) => {
                                        return s + prop.extensions().friendlyName() + "\n";
                                    }, "");
                                    const colls = _.reduce(matchingColls, (s, coll) => {
                                        return s + coll.extensions().friendlyName() + "\n";
                                    }, "");
                                    s = `Multiple matches for ${arg0}:\n${props}${colls}`;
                            }
                            this.clearInputAndSetMessage(s);
                        }
                    });
            }
        };

        private attemptGotoLinkNumber(itemNo: number, links: Models.Link[]): void {
            if (itemNo < 1 || itemNo > links.length) {
                this.clearInputAndSetMessage(itemNo.toString() + " is out of range for displayed items");
            } else {
                const link = links[itemNo - 1]; // On UI, first item is '1'
                this.urlManager.setItem(link);
            }
        }

        private openCollection(collection: CollectionMember): void {
            this.closeAnyOpenCollections();
            this.vm.clearInput();
            this.urlManager.setCollectionMemberState(collection.collectionId(), CollectionViewState.List);
        }
    }

    export class Help extends Command {

        fullCommand = "help";
        helpText = "If no argument is specified, help provides a basic explanation of how to use Cicero.\n" +
        "If help is followed by a question mark as an argument, this lists the commands available\n" +
        "in the current context. If help is followed by another command word as an argument\n" +
        "(or an abbreviation of it), a description of the specified Command is returned.";
        protected minArguments = 0;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        doExecute(args: string, chained: boolean): void {
            const arg = this.argumentAsString(args, 0);
            if (!arg ) {
                this.clearInputAndSetMessage(this.basicHelp);
            } else if (arg == "?") {
                const commands = this.commandFactory.allCommandsForCurrentContext();
                this.clearInputAndSetMessage(commands);
            } else {
                try {
                    const c = this.commandFactory.getCommand(arg);
                    this.clearInputAndSetMessage(c.fullCommand + " command:\n" + c.helpText);
                } catch (Error) {
                    this.clearInputAndSetMessage(Error.message);
                }
            }
        };

        basicHelp = "Cicero is a user interface purpose-designed to work with an audio screen-reader.\n" +
        "The display has only two fields: a read-only output field, and a single input field.\n" +
        "The input field always has the focus.\n"+
        "Commands are typed into the input field followed by the Enter key.\n"+
        "When the output field updates (either instantaneously or after the server has responded)\n" + 
        "its contents are read out automatically, so \n" +
        "the user never has to navigate around the screen.\n" +
        "Commands, such as 'action', 'field' and 'save', may be typed in full\n" +
        "or abbreviated to the first two or more characters.\n" +
        "Commands are not case sensitive.\n" +
        "Some commands take one or more arguments.\n" +
        "There must be a space between the command word and the first argument,\n" +
        "and a comman between arguments.|n"+
        "Arguments may contain spaces if needed.\n"+ 
        "The commands available to the user vary according to the context.\n" +
        "The command 'help ?' (note that there is a space between help and '?')\n" +
        "will list the commands available to the user in the current context.\n"+
        "‘help’ followed by another command word (in full or abbreviated) will give more details about that command.\n"+
        "Some commands will change the context, for example using the Go command to navigate to an associated object, \n" +
        "in which case the new context will be read out.\n"+
        "Other commands - help being an example - do not change the context, but will read out information to the user.\n"+
        "If the user needs a reminder of the current context, the 'Where' command will read the context out again.\n" +
        "Hitting Enter on the empty input field has the same effect.\n" +
        "When the user enters a command and the output has been updated, the input field will  be cleared, \n" +
        "ready for the next command. The user may recall the previous command, by hitting the up- arrow key.\n"+
        "The user might then edit or extend that previous command and hit Enter to run it again.\n" +
        "For advanced users: commands may be chained using a semi-colon between them,\n" +
        "however commands that do, or might, result in data updates cannot be chained.";
    }

    export class Menu extends Command {

        fullCommand = "menu";
        helpText = "Open a named main menu, from any context.\n" +
        "Menu takes one optional argument: the name, or partial name, of the menu.\n" +
        "If the partial name matches more than one menu, a list of matches is returned\n" +
        "but no menu is opened; if no argument is provided a list of all the menus\n" +
        "is returned.";
        protected minArguments = 0;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        doExecute(args: string, chained: boolean): void {
            const name = this.argumentAsString(args, 0);
            this.context.getMenus()
                .then((menus: MenusRepresentation) => {
                    var links = menus.value();
                    if (name) {
                        //TODO: do multi-clause match
                        const exactMatches = _.filter(links, (t) => { return t.title().toLowerCase() === name; });
                        const partialMatches = _.filter(links, (t) => { return t.title().toLowerCase().indexOf(name) > -1; });
                        links = exactMatches.length === 1 ? exactMatches : partialMatches;
                    }
                    switch (links.length) {
                        case 0:
                            this.clearInputAndSetMessage(name + " does not match any menu");
                            break;
                        case 1:
                            const menuId = links[0].rel().parms[0].value;
                            this.urlManager.setHome();
                            this.urlManager.clearUrlState();
                            this.urlManager.setMenu(menuId);
                            break;
                        default:
                            const label = name ? "Matching menus:\n" : "Menus:\n";
                            const s = _.reduce(links, (s, t) => { return s + t.title() + "\n"; }, label);
                            this.clearInputAndSetMessage(s);
                    }
                });
        }
    }

    export class OK extends Command {

        fullCommand = "ok";
        helpText = "Invoke the action currently open as a dialog.\n" +
        "Fields in the dialog should be completed before this.";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isDialog();
        }

        doExecute(args: string, chained: boolean): void {

            this.getActionForCurrentDialog().then((action: IInvokableAction) => {

                if (chained && action.invokeLink().method() !== "GET") {
                    this.mayNotBeChained(" unless the action is query-only");
                    return;
                }
                let fieldMap: _.Dictionary<Value>;
                if (this.isForm()) {
                    fieldMap = this.routeData().props; //Props passed in as pseudo-params to action
                } else {
                    fieldMap = this.routeData().dialogFields;
                }
                this.context.invokeAction(action, 1, fieldMap)
                    .then((result: ActionResultRepresentation) => {
                        // todo handle case where result is empty - this is no longer handled 
                        // by reject below
                        const warnings = result.extensions().warnings();
                        if (warnings) {
                            _.forEach(warnings, w => this.vm.alert += `\nWarning: ${w}`);
                        }
                        const messages = result.extensions().messages();
                        if (messages) {
                            _.forEach(messages, m => this.vm.alert += `\n${m}`);
                        }
                        this.urlManager.closeDialog();
                    }).
                    catch((reject: ErrorWrapper) => {

                        const display = (em: ErrorMap) => {
                            const paramFriendlyName = (paramId: string) => FriendlyNameForParam(action, paramId);
                            this.handleErrorResponse(em, paramFriendlyName);
                        };
                        this.context.handleWrappedError(reject, null, () => { }, display);
                    });
            });
        }
    }

    export class Page extends Command {
        fullCommand = "page";
        helpText = "Supports paging of returned lists.\n" +
        "The page command takes a single argument, which may be one of these four words:\n" +
        "first, previous, next, or last, \n" +
        "which may be abbreviated down to the first character.\n" +
        "Alternative, the argument may be a specific page number.";
        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isList();
        }

        doExecute(args: string, chained: boolean): void {
            const arg = this.argumentAsString(args, 0);
            this.getList().then((listRep: ListRepresentation) => {
                const numPages = listRep.pagination().numPages;
                const page = this.routeData().page;
                const pageSize = this.routeData().pageSize;
                if ("first".indexOf(arg) === 0) {
                    this.setPage(1);
                    return;
                } else if ("previous".indexOf(arg) === 0) {
                    if (page === 1) {
                        this.clearInputAndSetMessage("List is already showing the first page");
                    } else {
                        this.setPage(page - 1);
                    }
                } else if ("next".indexOf(arg) === 0) {
                    if (page === numPages) {
                        this.clearInputAndSetMessage("List is already showing the last page");
                    } else {
                        this.setPage(page + 1);
                    }
                } else if ("last".indexOf(arg) === 0) {
                    this.setPage(numPages);
                } else {
                    const number = parseInt(arg);
                    if (isNaN(number)) {
                        this.clearInputAndSetMessage("The argument must match: first, previous, next, last, or a single number");
                        return;
                    }
                    if (number < 1 || number > numPages) {
                        this.clearInputAndSetMessage(`Specified page number must be between 1 and ${numPages}`);
                        return;
                    }
                    this.setPage(number);
                }
            });
        }

        private setPage(page : number) {
            const pageSize = this.routeData().pageSize;
            this.urlManager.setListPaging(page, pageSize, CollectionViewState.List);
        }
    }

    export class Property extends Command {

        fullCommand = "property";
        helpText = "Display the name and content of one or more properties of an object.\n" +
        "Field may take 1 argument:  the partial field name.\n" +
        "If this matches more than one property, a list of matches is returned.\n" +
        "If no argument is provided, the full list of properties is returned. ";
        protected minArguments = 0;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isObject();
        }

        doExecute(args: string, chained: boolean): void {
            const fieldName = this.argumentAsString(args, 0);
            this.getObject()
                .then((obj: DomainObjectRepresentation) => {
                    const props = this.matchingProperties(obj, fieldName);
                    const colls = this.matchingCollections(obj, fieldName);
                    //TODO -  include these
                    let s: string;
                    switch (props.length + colls.length) {
                        case 0:
                            if (!fieldName) {
                                s = "No visible properties";
                            } else {
                                s = fieldName + " does not match any properties";
                            }
                            break;
                        case 1:
                            if (props.length > 0) {
                                s = this.renderPropNameAndValue(props[0]);
                            } else {
                                s = this.renderColl(colls[0]);
                            }
                            break;
                        default:
                            s = _.reduce(props, (s, prop) => {
                                return s + this.renderPropNameAndValue(prop);
                            }, "");
                            s += _.reduce(colls, (s, coll) => {
                                return s + this.renderColl(coll);
                            }, "");
                    }
                    this.clearInputAndSetMessage(s);
                });
        }

        private renderPropNameAndValue(pm: PropertyMember): string {
            const name = pm.extensions().friendlyName();
            let value: string;
            const propInUrl = this.routeData().props[pm.id()];
            if (this.isEdit() && !pm.disabledReason() && propInUrl) {
                value = propInUrl.toString() + " (modified)";
            } else {
                value = renderFieldValue(pm, pm.value(), this.mask);
            }
            return name + ": " + value + "\n";
        }

        private renderColl(coll: CollectionMember): string {
            let output = coll.extensions().friendlyName() + " (collection): ";
            switch (coll.size()) {
                case 0:
                    output += "empty";
                    break;
                case 1:
                    output += "1 item";
                    break;
                default:
                    output += `${coll.size() } items`;
            }
            return output + "\n";
        }
    }

    export class Reload extends Command {

        fullCommand = "reload";
        helpText = "Not yet implemented. Reload the data from the server for an object or a list.\n" +
        "Note that for a list, which was generated by an action, reload runs the action again, \n" +
        "thus ensuring that the list is up to date. However, reloading a list does not reload the\n" +
        "individual objects in that list, which may still be cached. Invoking Reload on an\n" +
        "individual object, however, will ensure that its fields show the latest server data.";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() || this.isList();
        }

        doExecute(args: string, chained: boolean): void {
            this.clearInputAndSetMessage("Reload command is not yet implemented");
        };
    }

    export class Root extends Command {

        fullCommand = "root";
        helpText = "From within an opend collection context, the root command returns\n" +
        " to the root object that owns the collection. Does not take any arguments.\n";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isCollection();
        }

        doExecute(args: string, chained: boolean): void {
            this.closeAnyOpenCollections();
        };
    }

    export class Save extends Command {

        fullCommand = "save";
        helpText = "Save the updated fields on an object that is being edited,\n" +
        "and return from edit mode to a normal view of that object";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isEdit() || this.isTransient();
        }

        doExecute(args: string, chained: boolean): void {
            if (chained) {
                this.mayNotBeChained();
                return;
            }
            this.getObject().then((obj: DomainObjectRepresentation) => {
                const props = obj.propertyMembers();
                const newValsFromUrl = this.routeData().props;
                const propIds = new Array<string>();
                const values = new Array<Value>();
                _.forEach(props, (propMember, propId) => {
                    if (!propMember.disabledReason()) {
                        propIds.push(propId);
                        const newVal = newValsFromUrl[propId];
                        if (newVal) {
                            values.push(newVal);
                        } else if (propMember.value().isNull() &&
                            propMember.isScalar()) {
                            values.push(new Value(""));
                        } else {
                            values.push(propMember.value());
                        }
                    }
                });
                const propMap = _.zipObject(propIds, values) as _.Dictionary<Value>;
                const mode = obj.extensions().interactionMode();

                const saveOrUpdate = (mode === "form" || mode === "transient") ? this.context.saveObject : this.context.updateObject;

                saveOrUpdate(obj, propMap, 1, true).
                    catch((reject: ErrorWrapper) => {
                        const display = (em: ErrorMap) => this.handleError(em, obj);
                        this.context.handleWrappedError(reject, null, () => { }, display);
                    });
            });
        };

        private handleError(err: ErrorMap, obj: DomainObjectRepresentation) {
            if (err.containsError()) {
                const propFriendlyName = (propId: string) => FriendlyNameForProperty(obj, propId);
                this.handleErrorResponse(err, propFriendlyName);
            } else {
                this.urlManager.setInteractionMode(InteractionMode.View);
            }
        }
    }

    export class Selection extends Command {

        fullCommand = "selection";
        helpText = "Not fully implemented. Select one or more items from a list,\n" +
        "prior to invoking an action on the selection.\n" +
        "Selection has one mandatory argument, which must be one of these words,\n" +
        "add, remove, all, clear, show.\n" +
        "The Add and Remove options must be followed by a second argument specifying\n" +
        "the item number, or range, to be added or removed.\n";
        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isList();
        }

        doExecute(args: string, chained: boolean): void {
            //TODO: Add in sub-commands: Add, Remove, All, Clear & Show
            const arg = this.argumentAsString(args, 0);
            const { start, end } = this.parseRange(arg); //'destructuring'
            this.getList().then((list: ListRepresentation) => {
                this.selectItems(list, start, end);
            });
        };

        private selectItems(list: ListRepresentation, startNo: number, endNo: number): void {
            let itemNo: number;
            for (itemNo = startNo; itemNo <= endNo; itemNo++) {
                this.urlManager.setListItem(itemNo - 1, true);
            }
        }
    }

    export class Show extends Command {

        fullCommand = "show";
        helpText = "Show one or more of the items from or a list view,\n" +
        "or an opened object collection. If no arguments are specified, \n" +
        "show will list all of the the items in the opened object collection,\n" +
        "or the first page of items if in a list view.\n" +
        "Alternatively, the command may be specified with an item number,\n" +
        "or a range such as 3-5.";
        protected minArguments = 0;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isCollection() || this.isList();
        }

        doExecute(args: string, chained: boolean): void {
            const arg = this.argumentAsString(args, 0, true);
            const { start, end } = this.parseRange(arg);
            if (this.isCollection()) {
                this.getObject().then((obj: DomainObjectRepresentation) => {
                    const openCollIds = openCollectionIds(this.routeData());
                    const coll = obj.collectionMember(openCollIds[0]);
                    this.renderCollectionItems(coll, start, end);
                });
                return;
            }
            //must be List
            this.getList().then((list: ListRepresentation) => {
                this.renderItems(list, start, end);
            });
        };

        private renderCollectionItems(coll: CollectionMember, startNo: number, endNo: number) {
            if (coll.value()) {
                this.renderItems(coll, startNo, endNo);
            } else {
                this.context.getCollectionDetails(coll, CollectionViewState.List).then((details: CollectionRepresentation) => {
                    this.renderItems(details, startNo, endNo);
                });
            }
        }

        private renderItems(source: IHasLinksAsValue, startNo: number, endNo: number): void {
            //TODO: problem here is that unless collections are in-lined value will be null.
            const max = source.value().length;
            if (!startNo) {
                startNo = 1;
            }
            if (!endNo) {
                endNo = max;
            }
            if (startNo > max || endNo > max) {
                this.clearInputAndSetMessage(`The highest numbered item is ${source.value().length}`);
                return;
            }
            if (startNo > endNo) {
                this.clearInputAndSetMessage("Starting item number cannot be greater than the ending item number");
                return;
            }
            let output = "";
            let i: number;
            const links = source.value();
            for (i = startNo; i <= endNo; i++) {
                output += `Item ${i}: ${links[i - 1].title() }\n`;
            }
            this.clearInputAndSetMessage(output);
        }
    }

    export class Where extends Command {

        fullCommand = "where";
        helpText = "Display a reminder of the current context.\n" +
        "The same can also be achieved by hitting the Return key on the empty input field.";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        doExecute(args: string, chained: boolean): void {
            this.$route.reload();
        };
    }
}