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
                if (this.minArguments == 0) return;
                throw new Error("No arguments provided.");
            }
            const args = argString.split(",");
            if (args.length < this.minArguments || args.length > this.maxArguments) {
                throw new Error("Wrong number of arguments provided.");
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
            const number = parseInt(arg);
            if (number == NaN) {
                throw new Error("Argument number " + +(argNo + 1).toString + + " must be a number");
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
            return this.urlManager.isHome(1);
        }
        protected isObject(): boolean {
            return !!this.routeData().objectId;
        }
        protected getObject(): ng.IPromise<DomainObjectRepresentation> {
            const oid = this.routeData().objectId;
            return this.context.getObjectByOid(1, oid);
        }
        protected isList(): boolean {
            return false;  //TODO
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
            return this.isObject && _.any(this.routeData().collections);
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
            return _.filter(reps, (rep) => {
                const path = rep.extensions().menuPath();
                const name = rep.extensions().friendlyName().toLowerCase();
                return _.all(clauses, clause => name.indexOf(clause) >= 0 ||
                    (!!path && path.toLowerCase().indexOf(clause) >= 0));
            });
        }
    }

    export class Action extends Command {

        public fullCommand = "action";
        public helpText = "Open an action from a Main Menu, or object actions. " +
        "The first (optional) argument is the name, or partial name, of the action. " +
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
            if (actions.length == 0) {
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
        public helpText = "Leave the current activity (action, or object edit), incomplete.";
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
    export class Copy extends Command {

        public fullCommand = "copy";
        public helpText = "Not yet implemented.  Copy a reference to an object into the clipboard. If the current context is " +
        "an object and no argument is specified, the object is copied; alternatively the name of a property " +
        "that contains an object reference may be specified. If the context is a list view, then the number of the item " +
        "in that list should be specified.";

        protected minArguments = 0;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() || this.isList();
        }

        execute(args: string): void {
            if (this.isObject()) {
                if (this.isCollection()) {
                    const item = this.argumentAsNumber(args, 1);
                    this.clearInputAndSetOutputTo("Copy item " + item);
                } else {
                    const arg = this.argumentAsString(args, 1, true);
                    if (arg == null) {
                        this.clearInputAndSetOutputTo("Copy object");
                    } else {
                        this.clearInputAndSetOutputTo("Copy property" + arg);
                    }
                }
            }
            if (this.isList()) {
                const item = this.argumentAsNumber(args, 1);
                this.clearInputAndSetOutputTo("Copy item " + item);
            }
        };
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
        public helpText = "Display the name and content of a field or fields. " +
        "In the context of an object, a field is a property; in the context of an action dialog a field is a parameter." +
        "Field may take 2 arguments, both of which are optional. " +
        "The argument is the partial field name. " +
        "If this matches more than one field, a list of matches is returned. " +
        "If no argument is provided, the full list of fields is returned. " +
        "Not yet implemented: the second optional argument applies only to fields in an action dialog, or " +
        "in an object beign edited, and specifies the value, or selection, to be entered " +
        "into the field.  If a ? is provided as the second argument, the field will not be " +
        "updated but further details will be provided about that input field.";
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
            if (fieldEntry == "?") {
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
                        const value = new Value(fieldEntry);
                        const param = params[0];
                        this.urlManager.setFieldValue(this.routeData().dialogId, param, value, 1);
                        break;
                    default:
                        this.clearInputAndSetOutputTo("Multiple fields match " + fieldName); //TODO: list them
                        break;
                }

            });
        }

        private renderFields(fieldName: string, details: boolean = false) {
            if (this.isDialog()) {
                //TODO: duplication with function on ViewModelFactory for rendering dialog ???
                //Is this needed at all, or should the fields always be rendered? i.e. if in dialog
                //you must provide a name arg for field?
                this.getActionForCurrentDialog().then((action: ActionMember) => {
                    let output = "";
                    _.forEach(this.routeData().parms, (value, key) => {
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
        public helpText = "Not yet implemented: Go to an object referenced in a property, or a list." +
        "Go takes one argument.  In the context of an object, that is the name or partial name" +
        "of the property holding the reference. In the context of a list, it is the " +
        "number of the item within the list (starting at 1). ";
        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() || this.isList();
        }

        execute(args: string): void {
            const name = this.argumentAsString(args, 0);
            if (this.isList() || this.isCollection()) {
                const item = this.argumentAsNumber(args, 1);
                this.clearInputAndSetOutputTo("The go command is not yet implemented for lists or collections");
                return;
            }
            this.getObject()
                .then((obj: DomainObjectRepresentation) => {
                    const allFields = this.matchingProperties(obj, name);
                    const refFields = _.filter(allFields, (p) => { return !p.isScalar() });
                    var s: string = "";
                    switch (refFields.length) {
                        case 0:
                            if (!name) {
                                s = "No visible fields";
                            } else {
                                s = name + " does not match any reference fields";
                            }
                            break;
                        case 1:
                            this.urlManager.setProperty(refFields[0], 1);
                            break;
                        default:
                            var label = "Multiple reference fields match " + name + ": ";
                            s = _.reduce(refFields, (s, prop) => {
                                return s + prop.extensions().friendlyName();
                            }, label);
                    }
                    this.clearInputAndSetOutputTo(s);
                });

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
    export class Item extends Command {

        public fullCommand = "item";
        public helpText = "Not yet implemented. In the context of an opened object collection, or a list view, the item command" +
        "will display one or more of the items. If no arguments are specified, item will list all of the " +
        "the items in the object collection, or the first page of items if in a list view. Alternatively, " +
        "the command may be specified with a starting item number and/or an ending item number, for example " +
        "item 3,5 will display items 3,4, and 5.  In the context of a list view only, Item may have a third " +
        "argument to specify a page number greater than 1. See also the Table command.";
        protected minArguments = 0;
        protected maxArguments = 3;

        isAvailableInCurrentContext(): boolean {
            return this.isCollection() || this.isList();
        }

        execute(args: string): void {
            this.clearInputAndSetOutputTo("Item command is not yet implemented");
            //const startNo = this.argumentAsNumber(args, 1, true);
            //const endNo = this.argumentAsNumber(args, 2, true);
            //const pageNo = this.argumentAsNumber(args, 3, true);
            //if (this.isCollection()) {
            //    if (pageNo != null) {
            //        throw new Error("Item may not have a third argument (page number) in the context of an object collection");
            //    }

            //} else {
            //}
        };

    }
    export class Menu extends Command {

        public fullCommand = "menu";
        public helpText = "From any context, Menu opens a named main menu. This " +
        "command normally takes one argument: the name, or partial name, of the menu. " +
        "If the partial name matches more than one menu, a list of matches will be returned " +
        "but no menu will be opened; if no argument is provided a list of all the menus " +
        "will be returned.";
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
                        links = _.filter(links, (t) => { return t.title().toLowerCase().indexOf(name) > -1; });
                    }
                    switch (links.length) {
                        case 0:
                            this.clearInputAndSetOutputTo(name + " does not match any menu");
                            break;
                        case 1:
                            const menuId = links[0].rel().parms[0].value;
                            this.urlManager.setMenu(menuId, 1);  //1 = pane 1  Resolving promise
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
        public helpText = "Invokes an action, assuming that any necessary parameters have already been set up. ";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isDialog();
        }

        execute(args: string): void {
            this.getActionForCurrentDialog().then((action: ActionMember) => {
                this.context.invokeAction(action, 1, {});
            });
        };
    }
    export class Open extends Command {

        public fullCommand = "open";
        public helpText = "Not yet implemented. Opens a view of a specific collection within an object, from which " +
        "individual items may be read using the item command. Open command takes one argument: " +
        "the name, or partial name, of the collection.  If the partial name matches more than one " +
        "collection, the list of matches will be returned, but none will be opened.";
        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isObject();
        }

        execute(args: string): void {
            const match = this.argumentAsString(args, 0);
            this.getObject()
                .then((obj: DomainObjectRepresentation) => {
                    this.processCollections(match, obj.collectionMembers());
                });
        };

        //TODO: Get commonality out of processing actions (properties?, fields?, menus?)
        private processCollections(match: string, collsMap: _.Dictionary<CollectionMember>) {
            var colls = _.map(collsMap, action => action);
            if (colls.length == 0) {
                this.clearInputAndSetOutputTo("No collections visible");
                return;
            }
            if (match) {
                colls = this.matchFriendlyNameAndOrMenuPath(colls, match);
            }
            switch (colls.length) {
                case 0:
                    this.clearInputAndSetOutputTo(match + " does not match any collections");
                    break;
                case 1:
                    this.openCollectionAsList(colls[0]);
                    break;
                default:
                    let label = match ? " Matching collections: " : "Collections: ";
                    var s = _.reduce(colls, (s, t) => {
                        const menupath = t.extensions().menuPath() ? t.extensions().menuPath() + " - " : "";
                        return s + menupath + t.extensions().friendlyName() + ", ";
                    }, label);
                    this.clearInputAndSetOutputTo(s);
            }
        }


        private openCollectionAsList(collection: CollectionMember): void {
            //TODO: Must close all other collections!!
            this.clearInput();
            this.urlManager.setCollectionMemberState(1, collection, CollectionViewState.List);
        }

    }
    export class Paste extends Command {

        public fullCommand = "paste";
        public helpText = "Not yet implemented. Pastes the object reference from the clipboard into a named field " +
        "on an object that is in edit mode, or in an opened action dialog. The paste command takes one argument: the " +
        "name or partial name of the field. If the partial name is ambigious the " +
        "list of matching fields will be returned but the reference will not have been pasted. " +
        "Paste ? will provide a reminder of the object currently held in the clipboard without pasting it anywhere.";
        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isEdit() || this.isDialog();
        }

        execute(args: string): void {
            this.clearInputAndSetOutputTo("Paste command is not yet implemented");
            //const match = this.argumentAsString(args, 0);
            //if (this.isEdit()) {
            //}
            //if (this.isDialog) {
            //}
        };

    }
    export class Reload extends Command {

        public fullCommand = "reload";
        public helpText = "Not yet implemented. In the context of an object or a list, reloads the data from the server" +
        "to ensure it is up to date.";
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
        public helpText = "Not yet implemented. From within a collection context, the root command returns" +
        " to the 'root' object that owns the collection." +
        ". Does not take any arguments";;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isCollection();
        }

        execute(args: string): void {
            this.clearInputAndSetOutputTo("Root command is not yet implemented");
        };
    }
    export class Save extends Command {

        public fullCommand = "save";
        public helpText = "Not yet implemented. Saves the updated properties on an object that is being edited, and returns " +
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
    export class Table extends Command {
        public fullCommand = "table";
        public helpText = "Not yet implemented. In the context of a list or an opened object collection, the table command" +
        "switches that context to table mode. Items then accessed via the item command, will be presented as table rows." +
        "Invoking table when the context is already in table mode will return the system to list mode.";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isCollection() || this.isList();
        }

        execute(args: string): void {
            const match = this.argumentAsString(args, 0);
            this.clearInputAndSetOutputTo("Table command is not yet implemented");
        };

    }
    export class Where extends Command {

        public fullCommand = "where";
        public helpText = "Reminds the user of the current context.";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        execute(args: string): void {
            this.vm.setOutputToSummaryOfRepresentation(this.routeData());
        };

    }
}