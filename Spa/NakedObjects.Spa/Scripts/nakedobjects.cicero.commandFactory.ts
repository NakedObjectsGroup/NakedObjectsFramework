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
        $route: ng.route.IRouteService,
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
            "ac": new Action(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "ba": new Back(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "ca": new Cancel(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "co": new Collection(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "cl": new Clipboard(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "ed": new Edit(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "fi": new Field(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "fo": new Forward(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "ge": new Gemini(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "go": new Go(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "he": new Help(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "me": new Menu(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "ok": new OK(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "pa": new Page(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "re": new Reload(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "ro": new Root(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "sa": new Save(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "se": new Selection(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "sh": new Show(urlManager, $location, commandFactory, context, navigation, $q, $route),
            "wh": new Where(urlManager, $location, commandFactory, context, navigation, $q, $route)
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