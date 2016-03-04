/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular.Gemini {

    export interface ICommandFactory {

        initialiseCommands(cvm: CiceroViewModel): void;

        parseInput(input: string, cvm: CiceroViewModel): void;

        processSingleCommand(command: string, cvm: CiceroViewModel, chained: boolean): void;

        autoComplete(partialCommand: string, cvm: CiceroViewModel): void;

        //Returns all commands that may be invoked in the current context
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
        navigation: INavigation) {

        var commandFactory = <ICommandFactory>this;

        let commandsInitialised = false;

        const commands: _.Dictionary<Command> = {
            "ac": new Action(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "ba": new Back(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "ca": new Cancel(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "cl": new Clipboard(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "ed": new Edit(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "en": new Enter(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "fo": new Forward(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "ge": new Gemini(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "go": new Goto(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "he": new Help(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "me": new Menu(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "ok": new OK(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "pa": new Page(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "pr": new Property(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "re": new Reload(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "ro": new Root(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "sa": new Save(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "se": new Selection(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "sh": new Show(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "wh": new Where(urlManager, $location, commandFactory, context, navigation, $q, $route, mask)
        }

        commandFactory.initialiseCommands = (cvm: CiceroViewModel) => {
            if (!commandsInitialised) {
                _.forEach(commands, command => command.initialiseWithViewModel(cvm));
                commandsInitialised = true;
            }
        };

        commandFactory.parseInput = (input: string, cvm: CiceroViewModel) => {
            cvm.chainedCommands = null; //TODO: Maybe not needed if unexecuted commands are cleared down upon error?
            if (!input) { //Special case for hitting Enter with no input
                commandFactory.getCommand("wh").execute(null, false);
                return;
            }
            commandFactory.autoComplete(input, cvm);
            cvm.input = cvm.input.trim();
            cvm.previousInput = cvm.input;
            const commands = input.split(";");
            if (commands.length > 1) {
                let first = commands[0];
                commands.splice(0, 1);
                cvm.chainedCommands = commands;
                commandFactory.processSingleCommand(first, cvm, false);
            } else {
                commandFactory.processSingleCommand(input, cvm, false);
            }
        };

        commandFactory.processSingleCommand = (input: string, cvm: CiceroViewModel, chained: boolean) => {
            try {
                input = input.trim();
                const firstWord = input.split(" ")[0].toLowerCase();
                const command: Command = commandFactory.getCommand(firstWord);
                var argString: string = null;
                const index = input.indexOf(" ");
                if (index >= 0) {
                    argString = input.substr(index + 1);
                }
                command.execute(argString, chained);
            }
            catch (Error) {
                cvm.output = Error.message;
                cvm.input = "";
            }
        };

        //TODO: change the name & functionality to pre-parse or somesuch as could do more than auto
        //complete e.g. reject unrecognised action or one not available in context.
        commandFactory.autoComplete = (input: string, cvm: CiceroViewModel) => {
            if (!input) return;
            let lastInChain = _.last(input.split(";")).toLowerCase();
            const charsTyped = lastInChain.length;
            lastInChain = lastInChain.trim();
            if (lastInChain.length === 0 || lastInChain.indexOf(" ") >= 0) { //i.e. not the first word
                cvm.input += " ";
                return;
            }
            try {
                const command: Command = commandFactory.getCommand(lastInChain);
                const earlierChain = input.substr(0, input.length - charsTyped);
                cvm.input = earlierChain + command.fullCommand + " ";
            }
            catch (Error) {
                cvm.output = Error.message;
            }
        };

        commandFactory.getCommand = (commandWord: string) => {
            if (commandWord.length < 2) {
                throw new Error("Command word must have at least 2 characters");
            }
            const abbr = commandWord.substr(0, 2);
            const command: Command = commands[abbr];
            if (command == null) {
                throw new Error("No command begins with " + abbr);
            }
            command.checkMatch(commandWord);
            return command;
        }

        commandFactory.allCommandsForCurrentContext = () => {
            var result = "Commands available in current context:\n";
            for (var key in commands) {
                var c = commands[key];
                if (c.isAvailableInCurrentContext()) {
                    result = result + c.fullCommand + "\n";
                }
            }
            return result;
        }
    });
}