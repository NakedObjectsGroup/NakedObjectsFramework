/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular.Gemini{

    export interface ICommandFactory {

        parseInput(command: string): void;

        //Returns all commands (as separated words) that may be invoked in the current context
        allCommandsForCurrentContext(): string;

        getCommand(commandWord: string): Command;
    }

    app.service('commandFactory', function ($q: ng.IQService,
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
        
        commandFactory.parseInput = (input: string) => {
            try {
                input = input.toLowerCase();
                const firstWord = input.split(" ")[0];
                const command: Command = commandFactory.getCommand(firstWord);
                command.checkIsAvailableInCurrentContext();
                const argString = input.split(" ")[1];
                command.checkNumberOfArguments(argString);
                command.execute(argString);
            }
            catch (Error) {
                context.getCiceroVM().output = Error.message;
            }
        };

        function cachedCvm(): CiceroViewModel { return context.setCiceroVMIfNecessary(commandFactory); }

        const commands = {
            "ac": new Action(urlManager, $location, cachedCvm(), commandFactory),
            "ba": new Back(urlManager, $location, cachedCvm(), commandFactory),
            "ca": new Cancel(urlManager, $location, cachedCvm(), commandFactory),
            "cl": new Clipboard(urlManager, $location, cachedCvm(), commandFactory),
            "co": new Copy(urlManager, $location, cachedCvm(), commandFactory),
            "de": new Description(urlManager, $location, cachedCvm(), commandFactory),
            "ed": new Edit(urlManager, $location, cachedCvm(), commandFactory),
            "en": new Enter(urlManager, $location, cachedCvm(), commandFactory),
            "fo": new Forward(urlManager, $location, cachedCvm(), commandFactory),
            "ge": new Gemini(urlManager, $location, cachedCvm(), commandFactory),
            "go": new Go(urlManager, $location, cachedCvm(), commandFactory),
            "he": new Help(urlManager, $location, cachedCvm(), commandFactory),
            "ho": new Home(urlManager, $location, cachedCvm(), commandFactory),
            "it": new Item(urlManager, $location, cachedCvm(), commandFactory),
            "me": new Menu(urlManager, $location, cachedCvm(), commandFactory),
            "ok": new OK(urlManager, $location, cachedCvm(), commandFactory),
            "op": new Open(urlManager, $location, cachedCvm(), commandFactory),
            "pa": new Paste(urlManager, $location, cachedCvm(), commandFactory),
            "pr": new Property(urlManager, $location, cachedCvm(), commandFactory),
            "re": new Reload(urlManager, $location, cachedCvm(), commandFactory),
            "ro": new Root(urlManager, $location, cachedCvm(), commandFactory),
            "sa": new Save(urlManager, $location, cachedCvm(), commandFactory),
            "se": new Select(urlManager, $location, cachedCvm(), commandFactory),
            "ta": new Table(urlManager, $location, cachedCvm(), commandFactory),
        }

        commandFactory.getCommand = (commandWord: string) => {
            const abbr = commandWord.substr(0, 2);
            const command = commands[abbr];
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
                    result = result + c.fullCommand + " ";
                }
            }
            return result;
        }
    });

}