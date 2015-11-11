/// <reference path="nakedobjects.gemini.services.urlmanager.ts" />

module NakedObjects.Angular.Gemini {

    //TODO: Make class and appropriate members abstract
    export class Command {

        //TODO: make abstract
        protected fullCommand: string;
        protected helpText: string;
        protected minArgumnents: number;
        protected maxArgumnents: number;

        //todo: abstract
        execute(): void {
            //1. Check that first word is a sub-string of command, else call invalid command
            //2. Parse params & give appropriate error messages
            //3. Switch based on context info (new methods on Url Manager)
            //4. If appropriate, set up new Url
            //5. Set up output message including (or not) rendering of new context
        }

        //To be overridden by each sub-class that has restricted availability
        isAvailableInCurrentContext(): boolean {
            return true; //todo: temp
        }

        //Not abstract
        protected arguments: string[];

        constructor(private urlManager: IUrlManager,
            protected nglocation: ng.ILocationService,
            protected vm: CiceroViewModel,
            protected input: string) {

            this.checkFirstWordIsSubstringOfCommand();
            this.checkNumberOfArgumentsAndExtract();
            this.execute();
        }
        
        //Helper methods follow
        protected clearCommand(): void {
            this.vm.command = "";
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

        protected headingForWrappedVm(): string {
            //TODO: Possibly this could be moved onto the vm as standard method e.g. CiceroHeading()
            const result = this.vm.wrapped;
            if (result instanceof DomainObjectViewModel) {
                const fullName = (<DomainObjectViewModel> result).domainType;
                const shortName = fullName.substr(fullName.lastIndexOf(".") + 1);
                return shortName + ": " + result.title;
            }
            return "";
        }

        private checkFirstWordIsSubstringOfCommand() {
            //TODO: Implement
        }

        private checkNumberOfArgumentsAndExtract(): void {

        }

        //argNo starts from 1.
        //If argument does not parse correctly, message will be passed to UI
        //and command aborted.
        protected argumentAsString(argNo: number, optional: boolean = false): string {
            //todo
            return null;
        }

        //argNo starts from 1.
        protected argumentAsNumber(argNo: number, optional: boolean = false): number {
            //todo
            return null;
        }

        protected getContextDescription(): string {
            //todo
            return null;
        }

        protected takesNoArguments: string =  this.fullCommand + " command does not require or accept any arguments.";


        //Context helpers (delegate to Url Manager) 
        protected isHome(): boolean {
            return this.urlManager.isHome();
        }

        protected isObject(): boolean {
            return this.urlManager.isObject();
        }
        protected isList(): boolean {
            return this.urlManager.isList();
        }
        protected isMenuOpen(): boolean {
            return this.urlManager.isMenuOpen();
        }
        protected isActionOpen(): boolean {
            return this.urlManager.isActionOpen();
        }
        protected isCollectionOpen(): boolean {
            return this.urlManager.isCollectionOpen();
        }
        protected isTable(): boolean {
            return this.urlManager.isTable();
        }
        protected isEdit(): boolean {
            return this.urlManager.isEdit();
        }

        protected setObjectEdit(): void {
            this.urlManager.setObjectEdit(true, 1);
        }

        protected commandNotAvailable(): void {
            this.vm.output = this.fullCommand + " command is not available in the current context.";
        }

        protected invalidArgument(message: string): void {
            //todo
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

        execute(): void {
            this.setOutput("Action command invoked"); //todo: temporary
        };

    }
    export class Back extends Command {

        public fullCommand = "back";
        public helpText = "Move back to the previous context";
        protected minArguments = 0;
        protected maxArguments = 0;

        execute(): void {
            this.setOutput("Back command invoked"); //todo: temporary
        };
    }
    export class Cancel extends Command {

        public fullCommand = "cancel";
        public helpText = "Leave the current activity (action, or object edit), incomplete." +
            this.takesNoArguments;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isActionOpen() || this.isEdit();
        }

        execute(): void {
            if (this.isEdit()) {
                this.setOutput("Edit cancelled"); //todo: temporary
            }
            if (this.isActionOpen()) {
                this.setOutput("Action cancelled"); //todo: temporary
            }         
        };
    }
    export class Clipboard extends Command {

        public fullCommand = "clipboard";
        public helpText = "Reminder of the object reference currently held in the clipboard, if any." +
        this.takesNoArguments;
        protected minArguments = 0;
        protected maxArguments = 0;

        execute(): void {
                this.setOutput("Clipboard command invoked"); //todo: temporary
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

        execute(): void {
            if (this.isObject()) {
                if (this.isCollectionOpen()) {
                    const item = this.argumentAsNumber(1);
                    this.setOutput("Copy item "+item);
                } else {
                    const arg = this.argumentAsString(1, true);
                    if (arg == null) {
                        this.setOutput("Copy object");
                    } else {
                        this.setOutput("Copy property" + arg);
                    }
                }
            }
            if (this.isList()) {
                const item = this.argumentAsNumber(1);
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

        execute(): void {
            const match = this.argumentAsString(1);
            this.setOutput("Property command invoked with argument: " + match); //todo: temporary
        };
    }
    export class Edit extends Command {

        public fullCommand = "edit";
        public helpText = "Put an object into Edit mode." +
        this.takesNoArguments;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() && !this.isEdit();
        }

        execute(): void {
            this.setObjectEdit();
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
            return this.isEdit() || this.isActionOpen();
        }

        execute(): void {
            const match = this.argumentAsString(1);
            if (this.isEdit()) {
                this.setOutput("Enter command invoked on property: " + match); //todo: temporary
            }
            if (this.isActionOpen) {
                this.setOutput("Enter command invoked on parameter: " + match); //todo: temporary
            }
        };

    }
    export class Forward extends Command {

        public fullCommand = "forward";
        public helpText = "Move forward to next context in history (having previously moved back).";
        protected minArguments = 0;
        protected maxArguments = 0;

        execute(): void {
            this.setOutput("Forward command invoked"); //todo: temporary
        };
    }
    export class Gemini extends Command {

        public fullCommand = "gemini";
        public helpText = "Switch to the Gemini (graphical) user interface displaying the same context. " +
        this.takesNoArguments;
        protected minArguments = 0;
        protected maxArguments = 0;

        execute(): void {
            this.newPath("/gemini/home");
        };
    }
    export class Go extends Command {

        public fullCommand = "go";
        public helpText = "Go to an object referenced in a property, or a list." +
        "Go takes one argument.  In the context of an object, that is the name or partial name" +
        "of the property holding the reference. In the context of a list, it is the "+
        "number of the item within the list (starting at 1). ";
        protected minArguments = 1;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() || this.isList();
        }

        execute(): void {
            if (this.isObject()) {
                const prop = this.argumentAsString(1);
                this.setOutput("Go to property"+prop+" invoked"); //todo: temporary
            }
            if (this.isList()) {
                const item = this.argumentAsNumber(1);
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

        execute(): void {
            var arg = this.argumentAsString(1);
            if (arg = null) {
                //todo
            } else {
                //todo
            }
            this.setOutput("OK Command invoked"); //temporary!
        };
    }
    export class Home extends Command {

        public fullCommand = "home";
        public helpText = "Return to Home location, where main menus may be accessed. " +
        this.takesNoArguments;
        protected minArguments = 0;
        protected maxArguments = 0;

        execute(): void {
            this.newPath("/cicero/home");
            this.clearCommand();
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
            return this.isCollectionOpen() || this.isList();
        }

        execute(): void {
            const startNo = this.argumentAsNumber(1, true);
            const endNo = this.argumentAsNumber(2, true);
            const pageNo = this.argumentAsNumber(3, true);
            if (this.isCollectionOpen()) {
                if (pageNo != null) {
                    this.invalidArgument("Item may not have a third argument (page number) in the context of an object collection");
                }
                this.setOutput("Item command invoked on Collection, from " + startNo + " to " + endNo); //todo: temporary

            } else {
                this.setOutput("Item command invoked on List, from "+startNo+" to "+endNo+" page "+pageNo); //todo: temporary
            }
        };

    }
    export class Menu extends Command {

        public fullCommand = "meny";
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

        execute(): void {
                const menu = this.argumentAsString(1);
                this.setOutput("Menu " + menu + " invoked"); //todo: temporary
        };
    }
    export class OK extends Command {

        public fullCommand = "ok";
        public helpText = "Invokes an action, assuming that any necessary parameters have already been set up. " +
        this.takesNoArguments;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isActionOpen();
        }

        execute(): void {
            if (!this.isActionOpen) {
                this.commandNotAvailable();
            } else {
                //todo
                //Attempt to invoke.
                //If successful 
            }
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

        execute(): void {
            const match = this.argumentAsString(1);
            this.setOutput("Open command invoked with argument: " + match); //todo: temporary
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
            return this.isEdit() || this.isActionOpen();
        }

        execute(): void {
            const match = this.argumentAsString(1);
            if (this.isEdit()) {
                this.setOutput("Paste command invoked on property: " + match); //todo: temporary
            }
            if (this.isActionOpen) {
                this.setOutput("Paste command invoked on parameter: " + match); //todo: temporary
            }
        };

    }
    export class Property extends Command {

        public fullCommand = "property";
        public helpText = "Display the name and value of a property or properties on an object being viewed or edited. " +
        "May take one argument: the name of a property, or name-match, for multiple properties." +
        "If the partial name matches more than one property, a list of matching properties is returned. " +
        "If no argument is provided, a full list of properties is returned";
        protected minArguments = 0;
        protected maxArguments = 1;

        isAvailableInCurrentContext(): boolean {
            return this.isObject();
        }

        execute(): void {
            const match = this.argumentAsString(1);
            this.setOutput("Property command invoked with argument: "+match); //todo: temporary
        };

    }
    export class Reload extends Command {

        public fullCommand = "reload";
        public helpText = "In the context of an object or a list, reloads the data from the server" +
        "to ensure it is up to date." + this.takesNoArguments;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isObject() || this.isList();
        }

        execute(): void {
            this.setOutput("Reload command invoked");
        };
    }
    export class Root extends Command {

        public fullCommand = "root";
        public helpText = "From within a collection context, the root command returns" +
        " to the 'root' object that owns the collection." +
        this.takesNoArguments;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isCollectionOpen();
        }

        execute(): void {
            this.setOutput("Object command invoked");
        };
    }
    export class Save extends Command {

        public fullCommand = "save";
        public helpText = "Saves the updated properties on an object that is being edited, and returns " +
        "from edit mode to a normal view of that object" +
        this.takesNoArguments;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isEdit();
        }
        execute(): void {
                this.setOutput("Object saved"); //todo: temporary
        };
    }
    export class Select extends Command {
        public fullCommand = "select";
        public helpText = "Select an option from a set of choices for a named property on an object that is in edit mode, " +
        "or for a named parameter on an opened action. The select command takes two arguments: the " +
        "name or partial name of the property or paramater, and the value or partial-match value to be selected."+
        "If either of the partial match arguments is ambiguous, the possible matches will be displayed to " +
        "but no selection will be made. If no second argument is provided, the full set of options will be " +
        "returned but none selected.";
        protected minArguments = 2;
        protected maxArguments = 2;

        isAvailableInCurrentContext(): boolean {
            return this.isEdit() || this.isActionOpen();
        }

        execute(): void {
            const name = this.argumentAsString(1);
            const option = this.argumentAsString(2, true);
            if (this.isEdit()) {
                this.setOutput("Select command invoked on property: " + name +" for option"+option); //todo: temporary
            }
            if (this.isActionOpen) {
                this.setOutput("Select command invoked on parameter: " + name + " for option" + option); //todo: temporary
            }
        };

    }
    export class Table extends Command {
        public fullCommand = "table";
        public helpText = "In the context of a list or an opened object collection, the table command" +
        "switches to table mode. Items then accessed via the item command, will be presented as table rows" +
        this.takesNoArguments;
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.isCollectionOpen() || this.isList();
        }

        execute(): void {
            const match = this.argumentAsString(1);
            this.setOutput("Open command invoked with argument: " + match); //todo: temporary
        };

    }
    //todo: quit (or logoff)
}