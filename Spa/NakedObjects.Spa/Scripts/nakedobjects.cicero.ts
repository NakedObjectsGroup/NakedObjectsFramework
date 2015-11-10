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

        constructor(protected urlManager: IUrlManager,
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

        protected takesNoArguments() {
            return this.fullCommand + " command does not require or accept any arguments.";
        }

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

        protected commandNotAvailable(): void {
            this.vm.output = this.fullCommand + " command is not available in the current context.";
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

    //cancel
    //clipboard
    //collection
    //description
    //edit
    //enter
    //forward

    export class Gemini extends Command {

        public fullCommand = "gemini";
        public helpText = "Switch to the Gemini (graphical) user interface displaying the same context. " +
        this.takesNoArguments();
        protected minArguments = 0;
        protected maxArguments = 0;

        execute(): void {
            this.newPath("/gemini/home");
        };
    }

    //go
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
        this.takesNoArguments();
        protected minArguments = 0;
        protected maxArguments = 0;

        execute(): void {
            this.newPath("/cicero/home");
            this.clearCommand();
            this.setOutput("home");
        };
    }

    //list
    //menu
    //object

    export class OK extends Command {

        public fullCommand = "ok";
        public helpText = "Invokes an action, assuming that any necessary parameters have already been set up. " +
        this.takesNoArguments();
        protected minArguments = 0;
        protected maxArguments = 0;

        isAvailableInCurrentContext(): boolean {
            return this.urlManager.isActionOpen();
        }

        execute(): void {
            if (!this.urlManager.isActionOpen) {
                this.commandNotAvailable();
            } else {
                //todo
                //Attempt to invoke.
                //If successful 
            }
        };
    }


    //paste
    //property
    //quit
    //refresh
    //save
    //select
    //table
    //validate

}