/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular.Gemini{

    export interface ICommandFactory {

        parseInput(command: string, cvm: CiceroViewModel): void;

        //Returns all commands (as separated words) that may be invoked in the current context
        allCommandsForCurrentContext(): string;

        getCommand(commandWord: string): Command;
    }

    app.service('commandFactory', function (
        $q: ng.IQService,
        $location: ng.ILocationService,
        $filter: ng.IFilterService,
        $cacheFactory: ng.ICacheFactoryService,
        repLoader: IRepLoader,
        color: IColor,
        context: IContext,
        mask: IMask,    
        urlManager: IUrlManager,
        focusManager: IFocusManager,
        navigation: INavigation,
        clickHandler: IClickHandler) {

        var commandFactory = <ICommandFactory>this;

        let commandsInitialised = false;

        const commands: _.Dictionary<Command> = {
            "ac": new Action(urlManager, $location, commandFactory, context, navigation, $q),
            "ba": new Back(urlManager, $location, commandFactory, context, navigation, $q),
            "ca": new Cancel(urlManager, $location, commandFactory, context, navigation, $q),
            "co": new Copy(urlManager, $location, commandFactory, context, navigation, $q),
            "ed": new Edit(urlManager, $location, commandFactory, context, navigation, $q),
            "fi": new Field(urlManager, $location, commandFactory, context, navigation, $q),
            "fo": new Forward(urlManager, $location, commandFactory, context, navigation, $q),
            "ge": new Gemini(urlManager, $location, commandFactory, context, navigation, $q),
            "go": new Go(urlManager, $location, commandFactory, context, navigation, $q),
            "he": new Help(urlManager, $location, commandFactory, context, navigation, $q),
            "it": new Item(urlManager, $location, commandFactory, context, navigation, $q),
            "me": new Menu(urlManager, $location, commandFactory, context, navigation, $q),
            "ok": new OK(urlManager, $location, commandFactory, context, navigation, $q),
            "op": new Open(urlManager, $location, commandFactory, context, navigation, $q),
            "pa": new Paste(urlManager, $location, commandFactory, context, navigation, $q),
            "re": new Reload(urlManager, $location, commandFactory, context, navigation, $q),
            "ro": new Root(urlManager, $location, commandFactory, context, navigation, $q),
            "sa": new Save(urlManager, $location, commandFactory, context, navigation, $q),
            "ta": new Table(urlManager, $location, commandFactory, context, navigation, $q),
            "wh": new Where(urlManager, $location, commandFactory, context, navigation, $q)
        }
        
        commandFactory.parseInput = (input: string, cvm: CiceroViewModel) => {
            if (!commandsInitialised) {
                _.forEach(commands, command => command.initialiseWithViewModel(cvm));
                commandsInitialised = true;
            }
            try {
                var firstWord : string;
                if (input) {
                    input = input.toLowerCase();
                    firstWord = input.split(" ")[0];
                } else {
                    input = firstWord = "wh"; //Special case '[Enter]' = 'where'
                }
                const command: Command = commandFactory.getCommand(firstWord);
                command.checkIsAvailableInCurrentContext();
                var argString: string = null;
                const index = input.indexOf(" ");
                if (index >= 0 ) {
                    argString = input.substr(index + 1);
                }
                command.checkNumberOfArguments(argString);
                command.execute(argString);
            }
            catch (Error) {
                cvm.output = Error.message;
                cvm.input = "";
            }
        };

        commandFactory.getCommand = (commandWord: string) => {
            const abbr = commandWord.substr(0, 2);
            const command: Command = commands[abbr];
            if (command == null) {
                throw new Error("No command begins with "+abbr);
            }
            command.checkMatch(commandWord);
            return command;
        }

        commandFactory.allCommandsForCurrentContext = () => {
            var result = "Commands available in current context: ";
            for (var key in commands) {
                var c = commands[key];
                if (c.isAvailableInCurrentContext()) {
                    result = result + c.fullCommand + ", ";
                }
            }
            return result;
        }
    });

}