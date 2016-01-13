/// <reference path="nakedobjects.gemini.services.urlmanager.ts" />

module NakedObjects.Angular.Gemini {

    import IResourceRepresentation = NakedObjects.RoInterfaces.IResourceRepresentation;

    export abstract class Command {

        constructor(protected urlManager: IUrlManager,
            protected nglocation: ng.ILocationService,
            protected commandFactory: ICommandFactory,
            protected context: IContext,
            protected navigation: INavigation,
            protected $q: ng.IQService
        ) {
        }

        public fullCommand: string;
        public helpText: string;
        protected minArguments: number;
        protected maxArguments: number;
        protected vm: CiceroViewModel;

        //Must be called after construction and before execute is called
        initialiseWithViewModel(cvm: CiceroViewModel) {
            this.vm = cvm;
        }

        abstract execute(args: string): void;

        public checkIsAvailableInCurrentContext(): void {
            if (!this.isAvailableInCurrentContext()) {
                throw new Error("The command: " + this.fullCommand + " is not available in the current context");
            }
        }

        public abstract isAvailableInCurrentContext(): boolean;

        protected clearInput(): void {
            this.vm.input = "";
        }
        
        //Helper methods follow
        protected clearInputAndSetOutputTo(text: string): void {
            this.clearInput();
            this.vm.output = text;
        }

        protected appendAsNewLineToOutput(text: string): void {
            this.vm.output.concat("/n" + text);
        }

        public checkMatch(matchText: string): void {
            if (this.fullCommand.indexOf(matchText) != 0) {
                throw new Error("No such command: " + matchText);
            }
        }

        public checkNumberOfArguments(argString: string): void {
            if (argString == null) {
                if (this.minArguments === 0) return;
                throw new Error("No arguments provided.");
            }
            const args = argString.split(",");
            if (args.length < this.minArguments) {
                throw new Error("Too few arguments provided.");
            }
            else if (args.length > this.maxArguments) {
                throw new Error("Too many arguments provided.");
            }
        }

        //argNo starts from 0.
        //If argument does not parse correctly, message will be passed to UI
        //and command aborted.
        //Always returns argument trimmed and as lower case
        protected argumentAsString(argString: string, argNo: number, optional: boolean = false): string {
            if (!argString) return undefined;
            if (!optional && argString.split(",").length < argNo + 1) {
                throw new Error("Too few arguments provided");
            }
            var args = argString.split(",");
            if (args.length < argNo + 1) {
                if (optional) {
                    return undefined;
                } else {
                    throw new Error("Required argument number " + (argNo + 1).toString + " is missing");
                }
            }
            return args[argNo].trim().toLowerCase();  // which may be "" if argString ends in a ','
        }

        //argNo starts from 0.
        protected argumentAsNumber(args: string, argNo: number, optional: boolean = false): number {
            const arg = this.argumentAsString(args, argNo, optional);
            if (!arg && optional === true) return null;
            const number = parseInt(arg);
            if (isNaN(number)) {
                throw new Error("Argument number " + (argNo + 1).toString() + " must be a number");
            }
            return number;
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
            const oid = this.routeData().objectId;
            return this.context.getObjectByOid(1, oid);
        }
        protected isList(): boolean {
            return this.vm.viewType === ViewType.List;
        }
        protected getList(): ng.IPromise<ListRepresentation> {
            const routeData = this.routeData();
            //TODO: Currently covers only the list-from-menu; need to cover list from object action
            return this.context.getListFromMenu(1, routeData.menuId, routeData.actionId, routeData.actionParams, routeData.page, routeData.pageSize);
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

        protected getActionForCurrentDialog(): ng.IPromise<ActionMember> {
            const dialogId = this.routeData().dialogId;
            if (this.isObject()) {
                return this.getObject().then((obj: DomainObjectRepresentation) => {
                    return this.$q.when(obj.actionMember(dialogId));
                });
            } else if (this.isMenu()) {
                return this.getMenu().then((menu: MenuRepresentation) => {
                    return this.$q.when(menu.actionMember(dialogId)); //i.e. return a promise
                });
            }
            return this.$q.reject("List actions not implemented yet");
        }
        //Tests that at least one collection is open (should only be one). 
        //TODO: assumes that closing collection removes it from routeData NOT sets it to Summary
        protected isCollection(): boolean {
            return this.isObject() && _.any(this.routeData().collections);
        }
        protected closeAnyOpenCollections() {
            const open = openCollectionIds(this.routeData());
            _.forEach(open, id => {
                this.urlManager.setCollectionMemberState(1, id, CollectionViewState.Summary);
            });
        }
        protected isTable(): boolean {
            return false; //TODO
        }
        protected isEdit(): boolean {
            return this.routeData().edit;
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

        protected matchingParameters(
            action: ActionMember,
            match: string): Parameter[] {
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
                    _.all(clauses, clause => {
                        name === clause ||
                        (!!path && path.toLowerCase() === clause)
                    });
            });
            if (exactMatches.length > 0) return exactMatches;
            return _.filter(reps, (rep) => {
                const path = rep.extensions().menuPath();
                const name = rep.extensions().friendlyName().toLowerCase();
                return _.all(clauses, clause => name.indexOf(clause) >= 0 ||
                    (!!path && path.toLowerCase().indexOf(clause) >= 0));
            });
        }

        protected findMatchingChoices(choices: _.Dictionary<Value>, titleMatch: string): Value[] {
            return _.filter(choices, v => v.toString().toLowerCase().indexOf(titleMatch) >= 0);
        }
    }

    export class Action extends Command {

        public fullCommand = "action";
        public helpText = "Open the dialog for action from a menu, or from object actions. " +
        "Note that a dialog is always opened for an action, even if it has no fields (parameters) - " +
        "this is a safety mechanism, allowing the user to confirm that the action is the one intended." +
        "Once any fields have been completed, using the Field command, the action may then be invoked " +
        "with the OK command." +
        "The action command takes two optional arguments. " +
        "The first is the name, or partial name, of the action. " +
        "If the partial name matches more than one action, a list of matches is returned," +
        "but none opened. If no argument is provided, a full list of available action names is returned. " +
        "The partial name may have more than one clause, separated by spaces, and these may match either " +
        "part(s) of the action name or the sub-menu name if one exists. " +
        "Not yet implemented: if the action name matches a single action, then a question-mark may be added as a second "
        "parameter - which will generate a more detailed description of the Action.";

        protected minArguments = 0;
        protected maxArguments = 2;

        public isAvailableInCurrentContext(): boolean {
            return (this.isMenu() || this.isObject()) && !this.isDialog() && !this.isEdit(); //TODO add list
        }

        execute(args: string): void {
            const match = this.argumentAsString(args, 0);
            const p1 = this.argumentAsString(args, 1, true);
            if (p1) {
                this.clearInputAndSetOutputTo("Second argument for action is not yet supported.");
                return;
            }
            if (this.isObject()) {
                this.getObject()
                    .then((obj: DomainObjectRepresentation) => {
                        this.processActions(match, obj.actionMembers());
                    });
            }
            else if (this.isMenu()) {
                this.getMenu()
                    .then((menu: MenuRepresentation) => {
                        this.processActions(match, menu.actionMembers());
                    });
            }
            //TODO: handle list
        }

        private processActions(match: string, actionsMap: _.Dictionary<ActionMember>) {
            var actions = _.map(actionsMap, action => action);
            if (actions.length === 0) {
                this.clearInputAndSetOutputTo("No actions available");
                return;
            }
            if (match) {
                actions = this.matchFriendlyNameAndOrMenuPath(actions, match);
            }
            switch (actions.length) {
                case 0:
                    this.clearInputAndSetOutputTo(match + " does not match any actions");
                    break;
                case 1:
                    this.openActionDialog(actions[0]);
                    break;
                default:
                    let label = match ? " Matching actions: " : "Actions: ";
                    var s = _.reduce(actions, (s, t) => {
                        const menupath = t.extensions().menuPath() ? t.extensions().menuPath() + " - " : "";
                        return s + menupath + t.extensions().friendlyName() + ", ";
                    }, label);
                    this.clearInputAndSetOutputTo(s);
            }
        }

        private openActionDialog(action: ActionMember) {
            this.urlManager.setDialog(action.actionId(), 1);  //1 = pane 1
            _.forEach(action.parameters(), (p) => {
                let pVal = p.default();
                this.urlManager.setFieldValue(action.actionId(), p, pVal, 1, false);
            });
        }
    }
    export class Back extends Command {

        public fullCommand = "back";
        public helpText = "Move back to the previous context.";
        protected minArguments = 0;
        protected maxArguments = 0;

        public isAvailableInCurrentContext(): boolean {
            return true;
        }

        execute(args: string): void {
            this.navigation.back();
        };
    }
    export class Cancel extends Command {

        public fullCommand = "cancel";
        public helpText = "Leave the current activity (action dialog, or object edit), incomplete.";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isDialog() || this.isEdit();
        }

        execute(args: string): void {
            if (this.isEdit()) {
                this.urlManager.setObjectEdit(false, 1);
            }
            if (this.isDialog()) {
                this.urlManager.closeDialog(1);
            }
        };
    }
    export class Collection extends Command {

        public fullCommand = "collection";
        public helpText = "Open a view of a specific collection within an object, from which " +
        "individual items may be read using the item command. Collection command takes one optional argument: " +
        "the name, or partial name, of the collection.  If the partial name matches more than one " +
        "collection, the list of matches will be returned, but none will be opened. " +
        "If no argument is specified, collection lists the names of all collections visible on the object.";
        protected minArguments = 0;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isObject();
        }

        execute(args: string): void {
            const match = this.argumentAsString(args, 0, true);
            this.getObject()
                .then((obj: DomainObjectRepresentation) => {
                    this.processCollections(match, obj.collectionMembers());
                });
        };

        private processCollections(match: string, collsMap: _.Dictionary<CollectionMember>) {

            const allColls = _.map(collsMap, action => action);
            let matchingColls = allColls;
            if (matchingColls.length === 0) {
                this.clearInputAndSetOutputTo("No collections visible");
                return;
            }
            if (match) {
                matchingColls = this.matchFriendlyNameAndOrMenuPath(matchingColls, match);
            }
            switch (matchingColls.length) {
                case 0:
                    this.clearInputAndSetOutputTo(match + " does not match any collections");
                    break;
                case 1:
                    this.openCollection(matchingColls[0]);
                    break;
                default:
                    let label = match ? " Matching collections: " : "Collections: ";
                    var s = _.reduce(matchingColls, (s, t) => {
                        const menupath = t.extensions().menuPath() ? t.extensions().menuPath() + " - " : "";
                        return s + menupath + t.extensions().friendlyName() + ", ";
                    }, label);
                    this.clearInputAndSetOutputTo(s);
            }
        }


        private openCollection(collection: CollectionMember): void {
            this.closeAnyOpenCollections();
            this.clearInput();
            this.urlManager.setCollectionMemberState(1, collection.collectionId(), CollectionViewState.List);
        }
    }
    export class Clipboard extends Command {

        public fullCommand = "clipboard";
        public helpText = "The clipboard command is used for temporarily " +
        "holding a reference to an object, so that it may be used later to enter into a field. " +
        "Clipboard requires one argument, which may take one of four values: " +
        "copy, show, go, or discard, each of which may be abbreviated down to one character. " +
        "Copy copies a reference to the object being viewed into the clipboard, overwriting any existing reference." +
        "Show displays the content of the clipboard without using it or changing context." +
        "Go takes you directly to the object held in the clipboard."
        "Discard removes any existing reference from the clipboard."
        "The reference held in the clipboard may be used within the Field command.";

        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        execute(args: string): void {
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
                this.clearInputAndSetOutputTo("Clipboard command may only be followed by copy, show, go, or discard");
            }
        };

        private copy(): void {
            if (!this.isObject()) {
                this.clearInputAndSetOutputTo("Clipboard copy may only be used in the context of viewing and object");
                return;
            }
            this.getObject().then((obj: DomainObjectRepresentation) => {
                this.vm.clipboard = obj;
                this.show();
            });
        }
        private show(): void {
            if (this.vm.clipboard) {
                const label = Helpers.typePlusTitle(this.vm.clipboard);
                this.clearInputAndSetOutputTo("Clipboard contains: " + label);
            } else {
                this.clearInputAndSetOutputTo("Clipboard is empty");
            }
        }
        private go(): void {
            const link = this.vm.clipboard.selfLink();
            if (link) {
                this.urlManager.setItem(link, 1);
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

        public fullCommand = "edit";
        public helpText = "Put an object into Edit mode.";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() && !this.isEdit();
        }

        execute(args: string): void {
            this.urlManager.setObjectEdit(true, 1);
        };
    }
    export class Field extends Command {

        public fullCommand = "field";
        public helpText = "Display the name and content of a field or fields, or enter a value into a field. " +
        "In the context of an object, a field is a property; in the context of an action dialog a field is a parameter." +
        "Field may take 2 arguments, both of which are optional. " +
        "The first argument is the partial field name. " +
        "If this matches more than one field, a list of matches is returned. " +
        "If no argument is provided, the full list of fields is returned. " +
        "The second optional argument applies only to fields in an action dialog, or " +
        "in an object beign edited (not yet implemented), and specifies the value, or selection, to be entered " +
        "into the field. If a ? is provided as the second argument, the field will not be " +
        "updated but further details will be provided about that input field." +
        "If the word paste is used as the second argument, then, provided that the field is " +
        "a reference field, the object reference in the clipboard will be pasted into the field.";
        protected minArguments = 0;
        protected maxArguments = 2;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() || this.isDialog();
        }

        execute(args: string): void {
            const fieldName = this.argumentAsString(args, 0);
            const fieldEntry = this.argumentAsString(args, 1, true);
            if (!fieldEntry) {
                this.renderFields(fieldName);
                return;
            }
            if (fieldEntry === "?") {
                this.renderFields(fieldName, true);
                return;
            }
            if (this.isDialog) {
                this.fieldEntryForDialog(fieldName, fieldEntry);
                return;
            }
            else if (this.isEdit()) {
                this.clearInputAndSetOutputTo("Modifying fields on an object in edit mode is not yet supported.");
                return;
            }
            //Must be object but not in Edit mode
            this.clearInputAndSetOutputTo("Fields may only be modified if object is in edit mode.");
        };

        private fieldEntryForDialog(fieldName: string, fieldEntry: string) {
            this.getActionForCurrentDialog().then((action: ActionMember) => {
                let params = _.map(action.parameters(), param => param);
                params = this.matchFriendlyNameAndOrMenuPath(params, fieldName);
                switch (params.length) {
                    case 0:
                        this.clearInputAndSetOutputTo("No fields in the current context match " + fieldName);
                        break;
                    case 1:
                        this.setField(params[0], fieldEntry);
                        break;
                    default:
                        this.clearInputAndSetOutputTo("Multiple fields match " + fieldName); //TODO: list them
                        break;
                }
            });
        }

        private setField(param: Parameter, fieldEntry: string): void {
            if (_.any(param.choices())) {
                this.handleChoices(param, fieldEntry);
                return;
            }
            if (param.isScalar()) {
                this.handleScalarField(param, fieldEntry);
            } else {
                this.handleReferenceField(param, fieldEntry);
            }
        }

        private handleScalarField(param: Parameter, fieldEntry: string) {
            this.setFieldValue(param, new Value(fieldEntry));
        }

        private setFieldValue(param: Parameter, value: Value) {
            this.urlManager.setFieldValue(this.routeData().dialogId, param, value, 1);
        }

        private handleReferenceField(param: Parameter, fieldEntry: string) {
            if ("paste".indexOf(fieldEntry) === 0) {
                this.handleClipboard(param);
            } else {
                this.clearInputAndSetOutputTo("Invalid entry for a reference field. Use clipboard or clip");
            }
        }

        private handleClipboard(param: Parameter) {
            const ref = this.vm.clipboard;
            if (!ref) {
                this.clearInputAndSetOutputTo("Cannot use Clipboard as it is empty");
                return;
            }
            const paramType = param.extensions().returnType();
            const refType = ref.domainType();
            this.context.isSubTypeOf(refType, paramType)
                .then((isSubType: boolean) => {
                    if (isSubType) {
                        const obj = this.vm.clipboard;
                        const selfLink = obj.selfLink();
                        //Need to add a title to the SelfLink as not there by default
                        selfLink.setTitle(obj.title());
                        const value = new Value(selfLink);
                        this.setFieldValue(param, value);
                    } else {
                        this.clearInputAndSetOutputTo("Contents of Clipboard are not compatible with the field.");
                    }
                });
        }

        private handleChoices(param: Parameter, fieldEntry: string): void {
            const matches = this.findMatchingChoices(param.choices(), fieldEntry);
            switch (matches.length) {
                case 0:
                    this.clearInputAndSetOutputTo("None of the choices matches " + fieldEntry);
                    break;
                case 1:
                    this.setFieldValue(param, matches[0]);
                    break;
                default:
                    let msg = "Multiple matches: ";
                    _.forEach(matches, m => msg += m.toString() + ", ");
                    this.clearInputAndSetOutputTo(msg);
                    break;
            }
        }

        private renderFields(fieldName: string, details: boolean = false) {
            if (this.isDialog()) {
                //TODO: duplication with function on ViewModelFactory for rendering dialog ???
                this.getActionForCurrentDialog().then((action: ActionMember) => {
                    let output = "";
                    _.forEach(this.routeData().dialogFields, (value, key) => {
                        output += Helpers.friendlyNameForParam(action, key) + ": ";
                        output += value.toValueString() || "empty";
                        output += ", ";
                    });
                    this.clearInputAndSetOutputTo(output);
                });
                return;
            }
            if (this.isObject) {
                this.getObject()
                    .then((obj: DomainObjectRepresentation) => {
                        var fields = this.matchingProperties(obj, fieldName);
                        var s: string = "";
                        switch (fields.length) {
                            case 0:
                                if (!fieldName) {
                                    s = "No visible fields";
                                } else {
                                    s = fieldName + " does not match any fields";
                                }
                                break;
                            case 1:
                                const field = fields[0];
                                if (!details) {
                                    s = this.renderProp(field);
                                } else {
                                    s = "Field name: " + field.extensions().friendlyName();
                                    s += ", Value: ";
                                    s += field.value().toString() || "empty";
                                    s += ", Type: " + Helpers.friendlyTypeName(field.extensions().returnType());
                                    if (field.disabledReason()) {
                                        s += ", Unmodifiable: " + field.disabledReason();
                                    } else {
                                        s += field.extensions().optional() ? ", Optional" : ", Mandatory";
                                        if (field.choices()) {
                                            var label = ", Choices: ";
                                            s += _.reduce(field.choices(), (s, cho) => {
                                                return s + cho + " ";
                                            }, label);
                                        }
                                        const desc = field.extensions().description()
                                        s += desc ? ", Description: " + desc : "";
                                        //TODO:  Add a Can Paste if clipboard has compatible type
                                    }
                                }
                                break;
                            default:
                                s = _.reduce(fields, (s, prop) => {
                                    return s + this.renderProp(prop);
                                }, "");
                        }
                        this.clearInputAndSetOutputTo(s);
                    });
            }
        }

        private renderProp(pm: PropertyMember): string {
            const name = pm.extensions().friendlyName();
            let value: string = pm.value().toString() || "empty";
            return name + ": " + value + ", ";
        }
    }
    export class Forward extends Command {

        public fullCommand = "forward";
        public helpText = "Move forward to next context in the history (if you have previously moved back).";
        protected minArguments = 0;
        protected maxArguments = 0;

        public isAvailableInCurrentContext(): boolean {
            return true;
        }
        execute(args: string): void {
            this.clearInput();  //To catch case where can't go any further forward and hence url does not change.
            this.navigation.forward();
        };
    }
    export class Gemini extends Command {

        public fullCommand = "gemini";
        public helpText = "Switch to the Gemini (graphical) user interface preserving " +
        "the current context.";
        protected minArguments = 0;
        protected maxArguments = 0;

        public isAvailableInCurrentContext(): boolean {
            return true;
        }
        execute(args: string): void {
            const newPath = "/gemini/" + this.nglocation.path().split("/")[2];
            this.nglocation.path(newPath);
        };
    }
    export class Go extends Command {

        public fullCommand = "go";
        public helpText = "Go to an object referenced in a property, or a list." +
        "Go takes one argument.  In the context of an object, that is the name or partial name" +
        "of the property holding the reference. In the context of a list, it is the " +
        "number of the item within the list (starting at 1). ";
        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() || this.isList();
        }

        execute(args: string): void {
            const arg0 = this.argumentAsString(args, 0);
            if (this.isList()) {
                const itemNo: number = parseInt(arg0);
                if (isNaN(itemNo)) {
                    this.clearInputAndSetOutputTo(arg0 + " is not a valid number");
                    return;
                }
                this.getList().then((list: ListRepresentation) => {
                    if (itemNo < 1 || itemNo > list.value().length) {
                        this.clearInputAndSetOutputTo(arg0 + " is out of range for displayed items");
                        return;
                    }
                    const link = list.value()[itemNo - 1]; // On UI, first item is '1'
                    this.urlManager.setItem(link, 1);
                });
                return;
            }
            if (this.isObject) {
                this.getObject()
                    .then((obj: DomainObjectRepresentation) => {
                        if (this.isCollection()) {
                            const item = this.argumentAsNumber(args, 0);
                            //TODO: validate range
                            const openCollIds = openCollectionIds(this.routeData());
                            const coll = obj.collectionMember(openCollIds[0]);
                            const link = coll.value()[item - 1];
                            this.urlManager.setItem(link, 1);
                            return;
                        } else {
                            const allFields = this.matchingProperties(obj, arg0);
                            const refFields = _.filter(allFields, (p) => { return !p.isScalar() });
                            var s: string = "";
                            switch (refFields.length) {
                                case 0:
                                    if (!arg0) {
                                        s = "No visible fields";
                                    } else {
                                        s = arg0 + " does not match any reference fields";
                                    }
                                    break;
                                case 1:
                                    //TODO: Check for any empty reference
                                    let link = refFields[0].value().link();
                                    this.urlManager.setItem(link, 1);
                                    break;
                                default:
                                    var label = "Multiple reference fields match " + arg0 + ": ";
                                    s = _.reduce(refFields, (s, prop) => {
                                        return s + prop.extensions().friendlyName();
                                    }, label);
                            }
                            this.clearInputAndSetOutputTo(s);
                        }
                    });
            }
        };
    }
    export class Help extends Command {

        public fullCommand = "help";
        public helpText = "If no argument specified, help lists the commands available in the current context." +
        "If help is followed by another command word as an argument (or an abbreviation of it), a description of that " +
        "specified Command will be returned.";
        protected minArguments = 0;
        protected maxArguments = 1;

        public isAvailableInCurrentContext(): boolean {
            return true;
        }

        execute(args: string): void {
            var arg = this.argumentAsString(args, 0);
            if (arg) {
                try {
                    const c = this.commandFactory.getCommand(arg);
                    this.clearInputAndSetOutputTo(c.fullCommand + " command: " + c.helpText);
                } catch (Error) {
                    this.clearInputAndSetOutputTo(Error.message);
                }
            } else {
                const commands = this.commandFactory.allCommandsForCurrentContext();
                this.clearInputAndSetOutputTo(commands);
            }
        };
    }
    export class Menu extends Command {

        public fullCommand = "menu";
        public helpText = "Open a named main menu, from any context. " +
        "Menu takes one optional argument: the name, or partial name, of the menu. " +
        "If the partial name matches more than one menu, a list of matches is returned " +
        "but no menu is opened; if no argument is provided a list of all the menus " +
        "is returned.";
        protected minArguments = 0;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        execute(args: string): void {
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
                            this.clearInputAndSetOutputTo(name + " does not match any menu");
                            break;
                        case 1:
                            const menuId = links[0].rel().parms[0].value;
                            this.urlManager.setHome(1);
                            this.urlManager.clearUrlState(1);
                            this.urlManager.setMenu(menuId, 1);
                            break;
                        default:
                            var label = name ? "Matching menus: " : "Menus: ";
                            var s = _.reduce(links, (s, t) => { return s + t.title() + ", "; }, label);
                            this.clearInputAndSetOutputTo(s);
                    }
                });
        }
    }
    export class OK extends Command {

        public fullCommand = "ok";
        public helpText = "Invoke the action currently open as a dialog. " +
        "Fields in the dialog should be completed before this.";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isDialog();
        }

        execute(args: string): void {
            let fieldMap = this.routeData().dialogFields;
            this.getActionForCurrentDialog().then((action: ActionMember) => {
                this.context.invokeAction(action, 1, fieldMap)
                    .then((err: ErrorMap) => {
                        if (err.containsError()) {
                            this.handleErrorResponse(err, action);
                        } else {
                            this.urlManager.closeDialog(1);
                        }
                    });
            });
        };

        handleErrorResponse(err: ErrorMap, action: ActionMember) {
            //TODO: Not currently covering co-validation errors
            //TODO: Factor out commonality for errors on saving object
            let msg = "Please complete or correct these fields: "
            _.each(err.valuesMap(), (errorValue, paramId) => {
                const reason = errorValue.invalidReason;
                const value = errorValue.value;
                if (reason) {
                    msg += Helpers.friendlyNameForParam(action, paramId) + ": ";
                    if (reason === "Mandatory") {
                        msg += "required";
                    } else {
                        msg += value + " " + reason;
                    }
                    msg += ", ";
                }
            });
            this.clearInputAndSetOutputTo(msg);
        }
    }
    export class Page extends Command {
        public fullCommand = "page";
        public helpText = "Supports paging of returned lists." +
        "The page command takes a single argument, which may be one of these four words: " +
        "first, previous, next, or last, which may be abbreviated down to the one character. " +
        "Alternative, the argument may be a specific page number.";
        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isList();
        }

        execute(args: string): void {
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
                        this.clearInputAndSetOutputTo("List is already showing the first page");
                    } else {
                        this.setPage(page - 1);
                    }
                } else if ("next".indexOf(arg) === 0) {
                    if (page === numPages) {
                        this.clearInputAndSetOutputTo("List is already showing the last page");
                    } else {
                        this.setPage(page + 1);
                    }
                } else if ("last".indexOf(arg) === 0) {
                    this.setPage(numPages);
                } else {
                    const number = parseInt(arg);
                    if (isNaN(number)) {
                        this.clearInputAndSetOutputTo("The argument must match: first, previous, next, last, or a single number");
                        return;
                    }
                    if (number < 1 || number > numPages) {
                        this.clearInputAndSetOutputTo("Specified page number must be between 1 and " + numPages);
                        return;
                    }
                    this.setPage(number);
                }
            });
        }

        private setPage(page) {
            const pageSize = this.routeData().pageSize;
            this.urlManager.setListPaging(1, page, pageSize, CollectionViewState.List);
        }
    }
    export class Reload extends Command {

        public fullCommand = "reload";
        public helpText = "Not yet implemented. Reload the data from the server for an object or a list. " +
        "Note that for a list, which was generated by an action, reload runs the action again - " +
        "thus ensuring that the list is up to date. However, reloading a list does not reload the " +
        "individual objects in that list, which may still be cached. Invoking Reload on an " +
        "individual object, however, will ensure that its fields show the latest server data."
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() || this.isList();
        }

        execute(args: string): void {
            this.clearInputAndSetOutputTo("Reload command is not yet implemented");
        };
    }
    export class Root extends Command {

        public fullCommand = "root";
        public helpText = "From within an opend collection context, the root command returns" +
        " to the root object that owns the collection. Does not take any arguments";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isCollection();
        }

        execute(args: string): void {
            this.closeAnyOpenCollections();
        };
    }
    export class Save extends Command {

        public fullCommand = "save";
        public helpText = "Not yet implemented. Save the updated fields on an object that is being edited, and return " +
        "from edit mode to a normal view of that object";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isEdit();
        }
        execute(args: string): void {
            this.clearInputAndSetOutputTo("save command is not yet implemented");
        };
    }
    export class Selection extends Command {

        public fullCommand = "selection";
        public helpText = "Not yet implemented. Select one or more items from" +
        "a list, prior to invoking an action on the selection." +
        "Selection has one mandatory argument, which must be one of these words, " +
        "though it may be abbreviated: add, remove, all, clear, show. " +
        "The Add and Remove options must be followed by a second argument specifying " +
        "the item number, or range, to be added or removed.";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isList();
        }
        execute(args: string): void {
            this.clearInputAndSetOutputTo("save command is not yet implemented");
        };
    }
    export class Show extends Command {

        public fullCommand = "show";
        public helpText = "Show one or more of the items from or a list view, or " +
        "an opened object collection. If no arguments are specified, show will list all of the " +
        "the items in the opened object collection, or the first page of items if in a list view. " +
        "Alternatively, the command may be specified with an item number, or a range such as 3-5. " +
        "Not yet implemented: Show can take additional parameters to specify table view, and/or to " +
        "specify logical matches for items to be shown e.g. status='pending'"
        protected minArguments = 0;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isCollection() || this.isList();
        }

        execute(args: string): void {
            let arg = this.argumentAsString(args, 0, true);
            if (!arg) {
                arg = "1-";
            }
            let startNo, endNo: number;
            const clauses = arg.split("-");
            switch (clauses.length) {
                case 1:
                    startNo = this.parseInt(clauses[0]);
                    endNo = startNo;
                    break;
                case 2:
                    startNo = this.parseInt(clauses[0]);
                    endNo = this.parseInt(clauses[1]);
                    break;
                default:
                    this.clearInputAndSetOutputTo("Cannot have more than one dash in argument");
                    return;
            }
            if ((startNo != null && startNo < 1) || (endNo != null && endNo < 1)) {
                this.clearInputAndSetOutputTo("Item number or range values must be greater than zero");
                return;
            }
            if (this.isCollection()) {
                this.getObject().then((obj: DomainObjectRepresentation) => {
                    const openCollIds = openCollectionIds(this.routeData());
                    const coll = obj.collectionMember(openCollIds[0]);
                    this.renderItems(coll, startNo, endNo);
                });
                return;
            }
            //must be List
            this.getList().then((list: ListRepresentation) => {
                this.renderItems(list, startNo, endNo);
            });
        };

        private parseInt(input: string): number {
            if (!input) {
                return null;
            }
            const number = parseInt(input);
            if (isNaN(number)) {
                throw new Error("Argument must be a single number or number range such as 3-5");
            }
            return number;
        }

        private renderItems(source: IHasLinksAsValue, startNo: number, endNo: number): void {
            const max = source.value().length;
            if (!startNo) {
                startNo = 1;
            }
            if (!endNo) {
                endNo = max;
            }
            if (startNo > max || endNo > max) {
                this.clearInputAndSetOutputTo("The highest numbered item is " + source.value().length);
                return;
            }
            if (startNo > endNo) {
                this.clearInputAndSetOutputTo("Starting item number cannot be greater than the ending item number");
                return;
            }
            let output = "";
            let i: number;
            const links = source.value();
            for (i = startNo; i <= endNo; i++) {
                output += "Item " + i + ": " + links[i - 1].title() + "; ";
            }
            this.clearInputAndSetOutputTo(output);
        }
    }
    export class Where extends Command {

        public fullCommand = "where";
        public helpText = "Display a reminder of the current context.";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        execute(args: string): void {
            this.vm.renderForViewType(this.routeData());
        };

    }
}