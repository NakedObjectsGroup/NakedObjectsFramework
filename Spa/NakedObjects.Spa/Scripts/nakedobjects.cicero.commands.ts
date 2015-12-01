/// <reference path="nakedobjects.gemini.services.urlmanager.ts" />

module NakedObjects.Angular.Gemini {

    export abstract class Command {

        constructor(protected urlManager: IUrlManager,
            protected nglocation: ng.ILocationService,
            protected vm: CiceroViewModel,
            protected commandFactory: ICommandFactory,
            protected context: IContext) {
        }

        public fullCommand: string;
        public helpText: string;
        protected minArguments: number;
        protected maxArguments: number;

        abstract execute(args: string): void;

        public checkIsAvailableInCurrentContext(): void {
            if (!this.isAvailableInCurrentContext()) {
                throw new Error("The command: " + this.fullCommand + " is not available in the current context");
            }
        }

        public abstract isAvailableInCurrentContext(): boolean;
        
        //Helper methods follow
        protected clearInput(): void {
            this.vm.input = "";
        }

        protected setOutput(text: string): void {
            this.vm.output = text;
        }

        protected appendAsNewLineToOutput(text: string): void {
            this.vm.output.concat("/n" + text);
        }

        protected newPath(path: string): void {
            this.nglocation.path(path).search({});
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
        protected argumentAsString(args: string, argNo: number, optional: boolean = false): string {
            if (args == null) return null;
            if (!optional && args.split(",").length < argNo + 1) {
                throw new Error("Too few arguments provided");
            }
            var arg = args.split(",")[argNo].trim();
            if (!optional && (arg == null || arg == "")) {
                throw new Error("Required argument number " + (argNo + 1).toString + " is empty");
            }
            return arg.trim().toLowerCase();
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

        private pane1RouteData(): PaneRouteData {
            return this.urlManager.getRouteData().pane1;
        }
        //Helpers delegating to RouteData
        protected isHome(): boolean {
            return this.urlManager.isHome(1);
        }
        protected isObject(): boolean {
            return !!this.pane1RouteData().objectId;
        }
        protected isList(): boolean {
            return false; //TODO
        }
        protected isMenu(): boolean {
            return !!this.pane1RouteData().menuId;
        }
        protected isDialog(): boolean {
            return !!this.pane1RouteData().dialogId;
        }
        protected isCollection(): boolean {
            return false; //TODO
        }
        protected isTable(): boolean {
            return false; //TODO
        }
        protected isEdit(): boolean {
            return false; //TODO
        }
    }

    export class Action extends Command {

        public fullCommand = "action";
        public helpText = "Open an action from a Main Menu, or object actions menu. " +
        "Normally takes one argument: the name, or partial name, of the action." +
        "If the partial name matches more than one action, a list of matches is returned," +
        "but none opened. If no argument is provided, a full list of available action names is returned";
        protected minArguments = 0;
        protected maxArguments = 1;

        public isAvailableInCurrentContext(): boolean {
            return (this.isMenu() || this.isObject()) && !this.isDialog() && !this.isEdit(); //TODO add list
        }

        execute(args: string): void {
            const name = this.argumentAsString(args, 0);
            if (this.isObject()) {
                const oid = this.urlManager.getRouteData().pane1.objectId;
                this.context.getObjectByOid(1, oid)
                    .then((obj: DomainObjectRepresentation) => {
                        this.processActions(name, obj.actionMembers());
                    });
            }
            else if (this.isMenu()) {
                const menuId = this.urlManager.getRouteData().pane1.menuId;
                this.context.getMenu(menuId)
                    .then((menu: MenuRepresentation) => {
                        this.processActions(name, menu.actionMembers());
                    });
            }
            //TODO: handle list
        }

        private processActions(name: string, actionsMap: IActionMemberMap) {
            var actions = _.map(actionsMap, action => action);
            if (name) {
                actions = _.filter(actions, (action) => action.extensions().friendlyName.toLowerCase().indexOf(name) > -1);
            }
            switch (actions.length) {
                case 0:
                    this.setOutput(name + " does not match any actions");
                    break;
                case 1:
                    const actionId = actions[0].actionId();
                    this.urlManager.setDialog(actionId, 1);  //1 = pane 1
                    break;
                default:
                    var label = "Actions";
                    if (name) {
                        label = label + " matching " + name;
                    }
                    var s = _.reduce(actions, (s, t) => { return s + t.extensions().friendlyName + "; "; }, label + ": ");
                    this.setOutput(s);
            }
        }
    }
    export class Back extends Command {

        public fullCommand = "back";
        public helpText = "Move back to the previous context";
        protected minArguments = 0;
        protected maxArguments = 0;

        public isAvailableInCurrentContext(): boolean {
            return true;
        }

        execute(args: string): void {
            this.setOutput("Back command is not yet implemented"); //todo: temporary
        };
    }
    export class Cancel extends Command {

        public fullCommand = "cancel";
        public helpText = "Leave the current activity (action, or object edit), incomplete." +
        ". Does not take any arguments";;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isDialog() || this.isEdit();
        }

        execute(args: string): void {
            if (this.isEdit()) {
                this.urlManager.setObjectEdit(false, 1);
                this.setOutput("Edit cancelled"); //todo: temporary
                this.clearInput();
            }
            if (this.isDialog()) {
                this.urlManager.closeDialog(1);
                this.setOutput("Action cancelled"); //todo: temporary
                this.clearInput();
            }
        };
    }
    export class Clipboard extends Command {

        public fullCommand = "clipboard";
        public helpText = "Reminder of the object reference currently held in the clipboard, if any. " +
        "Does not take any arguments";;
        protected minArguments = 0;
        protected maxArguments = 0;

        public isAvailableInCurrentContext(): boolean {
            return true;
        }

        execute(args: string): void {
            this.setOutput("Clipboard command is not yet implemented"); //todo: temporary
        };
    }
    export class Copy extends Command {

        public fullCommand = "copy";
        public helpText = "Copy a reference to an object into the clipboard. If the current context is " +
        "an object and no argument is specified, the object is copiedl; alternatively the name of a property " +
        "that contains an object reference may be specified. If the context is a list view, then the number of the item " +
        "in that list should be specified, and, optionally, if the item is not on the first page of the list, "
        "the page number may be specified as a second argument.";

        protected minArguments = 0;
        protected maxArguments = 2;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() || this.isList();
        }

        execute(args: string): void {
            if (this.isObject()) {
                if (this.isCollection()) {
                    const item = this.argumentAsNumber(args, 1);
                    this.setOutput("Copy item " + item);
                } else {
                    const arg = this.argumentAsString(args, 1, true);
                    if (arg == null) {
                        this.setOutput("Copy object");
                    } else {
                        this.setOutput("Copy property" + arg);
                    }
                }
            }
            if (this.isList()) {
                const item = this.argumentAsNumber(args, 1);
                this.setOutput("Copy item " + item);
            }
        };
    }
    export class Description extends Command {

        public fullCommand = "description";
        public helpText = "Display the name and value of a property or properties on an object being viewed or edited. " +
        "May take one argument: the name of a property, or name-match, for multiple properties." +
        "If the partial name matches more than one property, a list of matching properties is returned. " +
        "If no argument is provided, a full list of properties is returned";
        protected minArguments = 0;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isObject();
        }

        execute(args: string): void {
            const match = this.argumentAsString(args, 0);
            this.setOutput("Description command is not yet implemented with argument: " + match); //todo: temporary
        };
    }
    export class Edit extends Command {

        public fullCommand = "edit";
        public helpText = "Put an object into Edit mode." +
        ". Does not take any arguments";;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() && !this.isEdit();
        }

        execute(args: string): void {
            this.urlManager.setObjectEdit(true, 1);
            this.setOutput("Editing Object"); //todo: temporary
        };
    }
    export class Enter extends Command {

        public fullCommand = "enter";
        public helpText = "Enter a value into a named property on an object that is in edit mode, " +
        "or into a named parameter on an opened action. The enter command takes one argument: the " +
        "name or partial name of the property or paramater. If the partial name is ambigious the " +
        "list of matching properties or parameters will be returned but no value will have been entered.";
        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isEdit() || this.isDialog();
        }

        execute(args: string): void {
            const match = this.argumentAsString(args, 0);
            if (this.isEdit()) {
                this.setOutput("Enter command is not yet implemented on property: " + match); //todo: temporary
            }
            if (this.isDialog) {
                this.setOutput("Enter command is not yet implemented on parameter: " + match); //todo: temporary
            }
        };

    }
    export class Forward extends Command {

        public fullCommand = "forward";
        public helpText = "Move forward to next context in history (having previously moved back).";
        protected minArguments = 0;
        protected maxArguments = 0;

        public isAvailableInCurrentContext(): boolean {
            return true;
        }
        execute(args: string): void {
            this.setOutput("Forward command is not yet implemented"); //todo: temporary
        };
    }
    export class Gemini extends Command {

        public fullCommand = "gemini";
        public helpText = "Switch to the Gemini (graphical) user interface displaying the same context. " +
        ". Does not take any arguments";;
        protected minArguments = 0;
        protected maxArguments = 0;

        public isAvailableInCurrentContext(): boolean {
            return true;
        }
        execute(args: string): void {
            this.newPath("/gemini/home");
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
            if (this.isObject()) {
                const prop = this.argumentAsString(args, 0);
                this.setOutput("Go to property" + prop + " invoked"); //todo: temporary
            }
            if (this.isList()) {
                const item = this.argumentAsNumber(args, 1);
                this.setOutput("Go to list item" + item + " invoked"); //todo: temporary
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
                const c = this.commandFactory.getCommand(arg);
                this.setOutput(c.fullCommand + " command: " + c.helpText);
            } else {
                this.setOutput(this.commandFactory.allCommandsForCurrentContext());
            }
        };
    }
    export class Home extends Command {

        public fullCommand = "home";
        public helpText = "Return to Home location, where main menus may be accessed. " +
        ". Does not take any arguments";;
        protected minArguments = 0;
        protected maxArguments = 0;

        public isAvailableInCurrentContext(): boolean {
            return true;
        }

        execute(args: string): void {
            this.urlManager.setHome(1);
            this.clearInput();
            this.setOutput("home");
        };
    }
    export class Item extends Command {

        public fullCommand = "item";
        public helpText = "In the context of an opened object collection, or a list view, the item command" +
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
            const startNo = this.argumentAsNumber(args, 1, true);
            const endNo = this.argumentAsNumber(args, 2, true);
            const pageNo = this.argumentAsNumber(args, 3, true);
            if (this.isCollection()) {
                if (pageNo != null) {
                    throw new Error("Item may not have a third argument (page number) in the context of an object collection");
                }
                this.setOutput("Item command is not yet implemented on Collection, from " + startNo + " to " + endNo); //todo: temporary

            } else {
                this.setOutput("Item command is not yet implemented on List, from " + startNo + " to " + endNo + " page " + pageNo); //todo: temporary
            }
        };

    }
    export class Menu extends Command {

        public fullCommand = "menu";
        public helpText = "From the Home context, Menu opens a named main menu. This " +
        "command normally takes one argument: the name, or partial name, of the menu. " +
        "If the partial name matches more than one menu, a list of matches will be returned " +
        "but no menu will be opened; if no argument is provided a list of all the menus " +
        "will be returned.";
        protected minArguments = 0;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isHome();
        }

        execute(args: string): void {
            const menuName = this.argumentAsString(args, 0);
            this.context.getMenus()
                .then((menus: MenusRepresentation) => {
                    var links = menus.value();
                    if (menuName) {
                        links = _.filter(links, (t) => { return t.title().toLowerCase().indexOf(menuName) > -1; });
                    }
                    switch (links.length) {
                        case 0:
                            this.setOutput(menuName + " does not match any menu");
                            break;
                        case 1:
                            const menuId = links[0].rel().parms[0].value;
                            this.urlManager.setMenu(menuId, 1);  //1 = pane 1  Resolving promise
                            break;
                        default:
                            var label = "Menus";
                            if (menuName) {
                                label = label + " matching " + menuName;
                            }
                            var s = _.reduce(links, (s, t) => { return s + t.title() + "; "; }, label + ": ");
                            this.setOutput(s);
                    }
                });
        }
    }
    export class OK extends Command {

        public fullCommand = "ok";
        public helpText = "Invokes an action, assuming that any necessary parameters have already been set up. " +
        ". Does not take any arguments";;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isDialog();
        }

        execute(args: string): void {
            //TODO: Need to factor out more helper functions from common code in execute methods
            const dialogId = this.urlManager.getRouteData().pane1.dialogId;
            if (this.isObject()) {
                const oid = this.urlManager.getRouteData().pane1.objectId;
                this.context.getObjectByOid(1, oid)
                    .then((obj: DomainObjectRepresentation) => {
                        const action = obj.actionMember(dialogId);
                        this.context.invokeAction(action, 1);
                    });
            } else if (this.isMenu()) {
                const menuId = this.urlManager.getRouteData().pane1.menuId;
                this.context.getMenu(menuId)
                    .then((menu: MenuRepresentation) => {
                        const action = menu.actionMember(dialogId);
                        this.context.invokeAction(action, 1);
                    });
            } //TODO: List actions
        };
    }
    export class Open extends Command {

        public fullCommand = "open";
        public helpText = "Opens a view of a specific collection within an object, from which " +
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
            this.setOutput("Open command is not yet implemented with argument: " + match); //todo: temporary
        };

    }
    export class Paste extends Command {

        public fullCommand = "paste";
        public helpText = "Pastes the object reference from the clipboard into a named property on an object that is in edit mode, " +
        "or into a named parameter on an opened action. The paste command takes one argument: the " +
        "name or partial name of the property or paramater. If the partial name is ambigious the " +
        "list of matching properties or parameters will be returned but the reference will not have been pasted.";
        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isEdit() || this.isDialog();
        }

        execute(args: string): void {
            const match = this.argumentAsString(args, 0);
            if (this.isEdit()) {
                this.setOutput("Paste command is not yet implemented on property: " + match); //todo: temporary
            }
            if (this.isDialog) {
                this.setOutput("Paste command is not yet implemented on parameter: " + match); //todo: temporary
            }
        };

    }
    export class Property extends Command {

        public fullCommand = "property";
        public helpText = "Display the name and value of a property or properties on an object being viewed or edited. " +
        "One optional argument: the partial property name. " +
        "If this matches more than one property, a list of matches is returned. " +
        "If no argument is provided, the full list of properties is returned";
        protected minArguments = 0;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isObject();
        }

        execute(args: string): void {
            const name = this.argumentAsString(args, 0);
            const oid = this.urlManager.getRouteData().pane1.objectId;
            const obj = this.context.getObjectByOid(1, oid)
                .then((obj: DomainObjectRepresentation) => {
                    var props = _.map(obj.propertyMembers(), prop => prop);
                    if (name) {
                        var props = _.filter(props, (p) => { return p.extensions().friendlyName.toLowerCase().indexOf(name) > -1 });
                    }
                    //TODO render empty properties as e.g. 'empty'?
                    var s: string = "";
                    switch (props.length) {
                        case 0:
                            s = name + " does not match any properties";
                            break;
                        case 1:
                            s = "Property: " + props[0].extensions().friendlyName + ": " + props[0].value();
                            break;
                        default:
                            s = _.reduce(props, (s, t) => { return s + t.extensions().friendlyName + ": " + t.value() + "; "; }, "Properties: ");
                    }
                    this.setOutput(s);
                });
        };
    }
    export class Reload extends Command {

        public fullCommand = "reload";
        public helpText = "In the context of an object or a list, reloads the data from the server" +
        "to ensure it is up to date." + ". Does not take any arguments";;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() || this.isList();
        }

        execute(args: string): void {
            this.setOutput("Reload command is not yet implemented");
        };
    }
    export class Root extends Command {

        public fullCommand = "root";
        public helpText = "From within a collection context, the root command returns" +
        " to the 'root' object that owns the collection." +
        ". Does not take any arguments";;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isCollection();
        }

        execute(args: string): void {
            this.setOutput("Object command is not yet implemented");
        };
    }
    export class Save extends Command {

        public fullCommand = "save";
        public helpText = "Saves the updated properties on an object that is being edited, and returns " +
        "from edit mode to a normal view of that object" +
        ". Does not take any arguments";;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isEdit();
        }
        execute(args: string): void {
            this.setOutput("Object saved"); //todo: temporary
        };
    }
    export class Select extends Command {
        public fullCommand = "select";
        public helpText = "Select an option from a set of choices for a named property on an object that is in edit mode, " +
        "or for a named parameter on an opened action. The select command takes two arguments: the " +
        "name or partial name of the property or paramater, and the value or partial-match value to be selected." +
        "If either of the partial match arguments is ambiguous, the possible matches will be displayed to " +
        "but no selection will be made. If no second argument is provided, the full set of options will be " +
        "returned but none selected.";
        protected minArguments = 2;
        protected maxArguments = 2;

        isAvailableInCurrentContext(): boolean {
            return this.isEdit() || this.isDialog();
        }

        execute(args: string): void {
            const name = this.argumentAsString(args, 0);
            const option = this.argumentAsString(args, 2, true);
            if (this.isEdit()) {
                this.setOutput("Select command is not yet implemented on property: " + name + " for option" + option); //todo: temporary
            }
            if (this.isDialog) {
                this.setOutput("Select command is not yet implemented on parameter: " + name + " for option" + option); //todo: temporary
            }
        };

    }
    export class Table extends Command {
        public fullCommand = "table";
        public helpText = "In the context of a list or an opened object collection, the table command" +
        "switches to table mode. Items then accessed via the item command, will be presented as table rows" +
        ". Does not take any arguments";;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isCollection() || this.isList();
        }

        execute(args: string): void {
            const match = this.argumentAsString(args, 0);
            this.setOutput("Open command is not yet implemented with argument: " + match); //todo: temporary
        };

    }
    export class Where extends Command {

        public fullCommand = "where";
        public helpText = "Reminds the user of the current context.  May be invoked just " +
        "by hitting the Enter (Return) key in the empty Command field.";
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return true;
        }

        execute(args: string): void {
            this.setOutput("Where command is not yet implemented"); //todo: temporary
        };

    }
}