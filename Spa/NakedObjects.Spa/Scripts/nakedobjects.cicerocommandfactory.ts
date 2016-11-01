/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />


namespace NakedObjects {

    export interface ICommandFactory {

        initialiseCommands(cvm: ICiceroViewModel): void;

        parseInput(input: string, cvm: ICiceroViewModel): void;

        processSingleCommand(command: string, cvm: ICiceroViewModel, chained: boolean): void;

        autoComplete(partialCommand: string, cvm: ICiceroViewModel): void;

        //Returns all commands that may be invoked in the current context
        allCommandsForCurrentContext(): string;

        getCommand(commandWord: string): Command;
    }

    app.service("commandFactory", function(
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
        error: IError) {

        var commandFactory = <ICommandFactory>this;

        let commandsInitialised = false;

        const commands: _.Dictionary<Command> = {
            "ac": new Action(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "ba": new Back(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "ca": new Cancel(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "cl": new Clipboard(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "ed": new Edit(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "en": new Enter(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "fo": new Forward(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "ge": new Gemini(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "go": new Goto(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "he": new Help(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "me": new Menu(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "ok": new OK(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "pa": new Page(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "re": new Reload(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "ro": new Root(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "sa": new Save(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "se": new Selection(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "sh": new Show(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error),
            "wh": new Where(urlManager, $location, commandFactory, context, navigation, $q, $route, mask, error)
        };
        commandFactory.initialiseCommands = (cvm: ICiceroViewModel) => {
            if (!commandsInitialised) {
                _.forEach(commands, command => command.initialiseWithViewModel(cvm));
                commandsInitialised = true;
            }
        };

        commandFactory.parseInput = (input: string, cvm: ICiceroViewModel) => {
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
                const first = commands[0];
                commands.splice(0, 1);
                cvm.chainedCommands = commands;
                commandFactory.processSingleCommand(first, cvm, false);
            } else {
                commandFactory.processSingleCommand(input, cvm, false);
            }
        };

        commandFactory.processSingleCommand = (input: string, cvm: ICiceroViewModel, chained: boolean) => {
            try {
                input = input.trim();
                const firstWord = input.split(" ")[0].toLowerCase();
                const command = commandFactory.getCommand(firstWord);
                let argString: string = null;
                const index = input.indexOf(" ");
                if (index >= 0) {
                    argString = input.substr(index + 1);
                }
                command.execute(argString, chained);
            } catch (e) {
                cvm.output = e.message;
                cvm.input = "";
            }
        };

        //TODO: change the name & functionality to pre-parse or somesuch as could do more than auto
        //complete e.g. reject unrecognised action or one not available in context.
        commandFactory.autoComplete = (input: string, cvm: ICiceroViewModel) => {
            if (!input) return;
            let lastInChain = _.last(input.split(";")).toLowerCase();
            const charsTyped = lastInChain.length;
            lastInChain = lastInChain.trim();
            if (lastInChain.length === 0 || lastInChain.indexOf(" ") >= 0) { //i.e. not the first word
                cvm.input += " ";
                return;
            }
            try {
                const command = commandFactory.getCommand(lastInChain);
                const earlierChain = input.substr(0, input.length - charsTyped);
                cvm.input = earlierChain + command.fullCommand + " ";
            } catch (e) {
                cvm.output = e.message;
            }
        };

        commandFactory.getCommand = (commandWord: string) => {
            if (commandWord.length < 2) {
                throw new Error(commandTooShort);
            }
            const abbr = commandWord.substr(0, 2);
            const command = commands[abbr];
            if (command == null) {
                throw new Error(noCommandMatch(abbr));
            }
            command.checkMatch(commandWord);
            return command;
        };

        commandFactory.allCommandsForCurrentContext = () => {
            const commandsInContext = _.filter(commands, c => c.isAvailableInCurrentContext());
            return _.reduce(commandsInContext, (r, c) => r + c.fullCommand + "\n" , commandsAvailable);
        };
    });
}