/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular.Gemini{

    export interface ICommandFactory {

        parseInput(command: string): void;

        //Command may be abbreviated
        getHelpTextForCommand(input: string): string;

        //Returns all commands (as words) that may be invoked in the current context
        allCommandsForCurrentContext(): string[];
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
                const command: Command = getCommand(input);
                command.checkIsAvailableInCurrentContext();
                const paramString = input.split(" ")[1];
                command.execute(paramString);
            }
            catch (Error) {
                context.getCiceroVM().output = Error.message;
            }
        };

        function cachedCvm(): CiceroViewModel { return context.setCiceroVMIfNecessary(commandFactory); }

        const commands = {
            "ac": new Action(urlManager, $location, cachedCvm()),
            "ba": new Back(urlManager, $location, cachedCvm()),
            "ca": new Cancel(urlManager, $location, cachedCvm()),
            "cl": new Clipboard(urlManager, $location, cachedCvm()),
            "co": new Copy(urlManager, $location, cachedCvm()),
            "de": new Description(urlManager, $location, cachedCvm()),
            "ed": new Edit(urlManager, $location, cachedCvm()),
            "en": new Enter(urlManager, $location, cachedCvm()),
            "fo": new Forward(urlManager, $location, cachedCvm()),
            "ge": new Gemini(urlManager, $location, cachedCvm()),
            "go": new Go(urlManager, $location, cachedCvm()),
            "he": new Help(urlManager, $location, cachedCvm()),
            "ho": new Home(urlManager, $location, cachedCvm()),
            "it": new Item(urlManager, $location, cachedCvm()),
            "me": new Menu(urlManager, $location, cachedCvm()),
            "ok": new OK(urlManager, $location, cachedCvm()),
            "op": new Open(urlManager, $location, cachedCvm()),
            "pa": new Paste(urlManager, $location, cachedCvm()),
            "pr": new Property(urlManager, $location, cachedCvm()),
            "re": new Reload(urlManager, $location, cachedCvm()),
            "ro": new Root(urlManager, $location, cachedCvm()),
            "sa": new Save(urlManager, $location, cachedCvm()),
            "se": new Select(urlManager, $location, cachedCvm()),
            "ta": new Table(urlManager, $location, cachedCvm()),
        }

        //todo: implement this on the ICommandFactory interface so it can be
        //called from in Commands (i.e. in Help). It will also be necessary to pass 'this' into
        //the command constructor.
        function getCommand(input: string): Command {
            const abbr = input.substr(0, 2);
            const command = commands[abbr];
            if (command == null) {
                throw new Error("No command begins with "+abbr);
            }
            const firstWord = input.split(" ")[0];
            command.checkFirstWordIsSubstring(firstWord);
            return command;
        }
    });

}