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
                var firstWord : string;
                if (input == "") {
                    input = firstWord = "wh"; //Special case '[Enter]' = 'where'
                } else {
                    input = input.toLowerCase();
                    firstWord = input.split(" ")[0];
                }
                const command: Command = commandFactory.getCommand(firstWord);
                command.checkIsAvailableInCurrentContext();
                var argString: string = null;
                const index = input.indexOf(" ");
                if (index >= 0 ) {
                    argString = input.substr(index + 1);
                }
                command.checkNumberOfArguments(argString);
                command.clearInput();
                command.execute(argString);
            }
            catch (Error) {
                context.getCiceroVM().output = Error.message;
            }
        };

        function cachedCvm(): CiceroViewModel { return context.setCiceroVMIfNecessary(commandFactory); }

        const commands = {
            "ac": new Action(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "ba": new Back(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "ca": new Cancel(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "cl": new Clipboard(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "co": new Copy(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "de": new Description(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "ed": new Edit(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "en": new Enter(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "fo": new Forward(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "ge": new Gemini(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "go": new Go(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "he": new Help(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "ho": new Home(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "it": new Item(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "me": new Menu(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "ok": new OK(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "op": new Open(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "pa": new Paste(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "pr": new Property(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "re": new Reload(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "ro": new Root(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "sa": new Save(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "se": new Select(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "ta": new Table(urlManager, $location, cachedCvm(), commandFactory, context, navigation),
            "wh": new Where(urlManager, $location, cachedCvm(), commandFactory, context, navigation)
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
                    result = result + c.fullCommand + "; ";
                }
            }
            return result;
        }
    });

}