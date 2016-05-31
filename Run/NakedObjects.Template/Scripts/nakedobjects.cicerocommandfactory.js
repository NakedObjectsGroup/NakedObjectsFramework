/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    NakedObjects.app.service("commandFactory", function ($q, $location, $filter, $route, $cacheFactory, repLoader, color, context, mask, urlManager, focusManager, navigation) {
        var commandFactory = this;
        var commandsInitialised = false;
        var commands = {
            "ac": new NakedObjects.Action(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "ba": new NakedObjects.Back(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "ca": new NakedObjects.Cancel(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "cl": new NakedObjects.Clipboard(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "ed": new NakedObjects.Edit(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "en": new NakedObjects.Enter(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "fo": new NakedObjects.Forward(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "ge": new NakedObjects.Gemini(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "go": new NakedObjects.Goto(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "he": new NakedObjects.Help(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "me": new NakedObjects.Menu(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "ok": new NakedObjects.OK(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "pa": new NakedObjects.Page(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "pr": new NakedObjects.Property(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "re": new NakedObjects.Reload(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "ro": new NakedObjects.Root(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "sa": new NakedObjects.Save(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "se": new NakedObjects.Selection(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "sh": new NakedObjects.Show(urlManager, $location, commandFactory, context, navigation, $q, $route, mask),
            "wh": new NakedObjects.Where(urlManager, $location, commandFactory, context, navigation, $q, $route, mask)
        };
        commandFactory.initialiseCommands = function (cvm) {
            if (!commandsInitialised) {
                _.forEach(commands, function (command) { return command.initialiseWithViewModel(cvm); });
                commandsInitialised = true;
            }
        };
        commandFactory.parseInput = function (input, cvm) {
            cvm.chainedCommands = null; //TODO: Maybe not needed if unexecuted commands are cleared down upon error?
            if (!input) {
                commandFactory.getCommand("wh").execute(null, false);
                return;
            }
            commandFactory.autoComplete(input, cvm);
            cvm.input = cvm.input.trim();
            cvm.previousInput = cvm.input;
            var commands = input.split(";");
            if (commands.length > 1) {
                var first = commands[0];
                commands.splice(0, 1);
                cvm.chainedCommands = commands;
                commandFactory.processSingleCommand(first, cvm, false);
            }
            else {
                commandFactory.processSingleCommand(input, cvm, false);
            }
        };
        commandFactory.processSingleCommand = function (input, cvm, chained) {
            try {
                input = input.trim();
                var firstWord = input.split(" ")[0].toLowerCase();
                var command = commandFactory.getCommand(firstWord);
                var argString = null;
                var index = input.indexOf(" ");
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
        commandFactory.autoComplete = function (input, cvm) {
            if (!input)
                return;
            var lastInChain = _.last(input.split(";")).toLowerCase();
            var charsTyped = lastInChain.length;
            lastInChain = lastInChain.trim();
            if (lastInChain.length === 0 || lastInChain.indexOf(" ") >= 0) {
                cvm.input += " ";
                return;
            }
            try {
                var command = commandFactory.getCommand(lastInChain);
                var earlierChain = input.substr(0, input.length - charsTyped);
                cvm.input = earlierChain + command.fullCommand + " ";
            }
            catch (Error) {
                cvm.output = Error.message;
            }
        };
        commandFactory.getCommand = function (commandWord) {
            if (commandWord.length < 2) {
                throw new Error("Command word must have at least 2 characters");
            }
            var abbr = commandWord.substr(0, 2);
            var command = commands[abbr];
            if (command == null) {
                throw new Error("No command begins with " + abbr);
            }
            command.checkMatch(commandWord);
            return command;
        };
        commandFactory.allCommandsForCurrentContext = function () {
            var result = "Commands available in current context:\n";
            for (var key in commands) {
                var c = commands[key];
                if (c.isAvailableInCurrentContext()) {
                    result = result + c.fullCommand + "\n";
                }
            }
            return result;
        };
    });
})(NakedObjects || (NakedObjects = {}));
