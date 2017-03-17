// Copyright 2013-2014 Naked Objects Group Ltd
// Licensed under the Apache License, Version 2.0(the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
var NakedObjects;
(function (NakedObjects) {
    // user message constants
    NakedObjects.noResultMessage = "no result found";
    NakedObjects.obscuredText = "*****";
    NakedObjects.tooShort = "Too short";
    NakedObjects.tooLong = "Too long";
    NakedObjects.notAnInteger = "Not an integer";
    NakedObjects.notANumber = "Not a number";
    NakedObjects.mandatory = "Mandatory";
    NakedObjects.optional = "Optional";
    NakedObjects.choices = "Choices";
    NakedObjects.pendingAutoComplete = "Pending auto-complete...";
    NakedObjects.noPatternMatch = "Invalid entry";
    NakedObjects.closeActions = "Close actions";
    NakedObjects.noActions = "No actions available";
    NakedObjects.openActions = "Open actions (Alt-a)";
    NakedObjects.mandatoryFieldsPrefix = "Missing mandatory fields: ";
    NakedObjects.invalidFieldsPrefix = "Invalid fields: ";
    NakedObjects.unknownFileTitle = "UnknownFile";
    NakedObjects.unknownCollectionSize = "";
    NakedObjects.emptyCollectionSize = "Empty";
    NakedObjects.noItemsFound = "No items found";
    NakedObjects.noItemsSelected = "Must select items for collection contributed action";
    NakedObjects.dropPrompt = "(drop here)";
    NakedObjects.autoCompletePrompt = "(auto-complete or drop)";
    NakedObjects.concurrencyMessage = "The object has been reloaded to pick up changes by another user. Please review, and re-enter any changes you still require.";
    NakedObjects.loadingMessage = "Loading...";
    NakedObjects.outOfRange = function (val, min, max, filter) {
        var minVal = filter ? filter.filter(min) : min;
        var maxVal = filter ? filter.filter(max) : max;
        return "Value is outside the range " + (minVal || "unlimited") + " to " + (maxVal || "unlimited");
    };
    NakedObjects.pageMessage = function (p, tp, c, tc) { return ("Page " + p + " of " + tp + "; viewing " + c + " of " + tc + " items"); };
    NakedObjects.logOffMessage = function (u) { return ("Please confirm logoff of user: " + u); };
    //Cicero commands and Help text
    NakedObjects.welcomeMessage = "Welcome to Cicero. Type 'help' and the Enter key for more information.";
    NakedObjects.basicHelp = "Cicero is a user interface purpose-designed to work with an audio screen-reader.\n" +
        "The display has only two fields: a read-only output field, and a single input field.\n" +
        "The input field always has the focus.\n" +
        "Commands are typed into the input field followed by the Enter key.\n" +
        "When the output field updates (either instantaneously or after the server has responded)\n" +
        "its contents are read out automatically, so \n" +
        "the user never has to navigate around the screen.\n" +
        "Commands, such as 'action', 'field' and 'save', may be typed in full\n" +
        "or abbreviated to the first two or more characters.\n" +
        "Commands are not case sensitive.\n" +
        "Some commands take one or more arguments.\n" +
        "There must be a space between the command word and the first argument,\n" +
        "and a comman between arguments.|n" +
        "Arguments may contain spaces if needed.\n" +
        "The commands available to the user vary according to the context.\n" +
        "The command 'help ?' (note that there is a space between help and '?')\n" +
        "will list the commands available to the user in the current context.\n" +
        "‘help’ followed by another command word (in full or abbreviated) will give more details about that command.\n" +
        "Some commands will change the context, for example using the Go command to navigate to an associated object, \n" +
        "in which case the new context will be read out.\n" +
        "Other commands - help being an example - do not change the context, but will read out information to the user.\n" +
        "If the user needs a reminder of the current context, the 'Where' command will read the context out again.\n" +
        "Hitting Enter on the empty input field has the same effect.\n" +
        "When the user enters a command and the output has been updated, the input field will  be cleared, \n" +
        "ready for the next command. The user may recall the previous command, by hitting the up- arrow key.\n" +
        "The user might then edit or extend that previous command and hit Enter to run it again.\n" +
        "For advanced users: commands may be chained using a semi-colon between them,\n" +
        "however commands that do, or might, result in data updates cannot be chained.";
    NakedObjects.actionCommand = "action";
    NakedObjects.actionHelp = "Open the dialog for action from a menu, or from object actions.\n" +
        "A dialog is always opened for an action, even if it has no fields (parameters):\n" +
        "This is a safety mechanism, allowing the user to confirm that the action is the one intended.\n" +
        "Once the dialog fields have been completed, using the Enter command,\n" +
        "the action may then be invoked  with the OK command.\n" +
        "The action command takes two optional arguments.\n" +
        "The first is the name, or partial name, of the action.\n" +
        "If the partial name matches more than one action, a list of matches is returned but none opened.\n" +
        "If no argument is provided, a full list of available action names is returned.\n" +
        "The partial name may have more than one clause, separated by spaces.\n" +
        "these may match either parts of the action name or the sub-menu name if one exists.\n" +
        "If the action name matches a single action, then a question-mark may be added as a second\n" +
        "parameter, which will generate a more detailed description of the Action.";
    NakedObjects.backCommand = "back";
    NakedObjects.backHelp = "Move back to the previous context.";
    NakedObjects.cancelCommand = "cancel";
    NakedObjects.cancelHelp = "Leave the current activity (action dialog, or object edit), incomplete.";
    NakedObjects.clipboardCommand = "clipboard";
    NakedObjects.clipboardCopy = "copy";
    NakedObjects.clipboardShow = "show";
    NakedObjects.clipboardGo = "go";
    NakedObjects.clipboardDiscard = "discard";
    NakedObjects.clipboardHelp = "The clipboard command is used for temporarily\n" +
        "holding a reference to an object, so that it may be used later\n" +
        "to enter into a field.\n" +
        "Clipboard requires one argument, which may take one of four values:\n" +
        "copy, show, go, or discard\n" +
        "each of which may be abbreviated down to one character.\n" +
        "Copy copies a reference to the object being viewed into the clipboard,\n" +
        "overwriting any existing reference.\n" +
        "Show displays the content of the clipboard without using it.\n" +
        "Go takes you directly to the object held in the clipboard.\n" +
        "Discard removes any existing reference from the clipboard.\n" +
        "The reference held in the clipboard may be used within the Enter command.";
    NakedObjects.editCommand = "edit";
    NakedObjects.editHelp = "Put an object into Edit mode.";
    NakedObjects.enterCommand = "enter";
    NakedObjects.enterHelp = "Enter a value into a field,\n" +
        "meaning a parameter in an action dialog,\n" +
        "or  a property on an object being edited.\n" +
        "Enter requires 2 arguments.\n" +
        "The first argument is the partial field name, which must match a single field.\n" +
        "The second optional argument specifies the value, or selection, to be entered.\n" +
        "If a question mark is provided as the second argument, the field will not be\n" +
        "updated but further details will be provided about that input field.\n" +
        "If the word paste is used as the second argument, then, provided that the field is\n" +
        "a reference field, the object reference in the clipboard will be pasted into the field.\n";
    NakedObjects.forwardCommand = "forward";
    NakedObjects.forwardHelp = "Move forward to next context in the history\n" +
        "(if you have previously moved back).";
    NakedObjects.geminiCommand = "gemini";
    NakedObjects.geminiHelp = "Switch to the Gemini (graphical) user interface\n" +
        "preserving the current context.";
    NakedObjects.gotoCommand = "goto";
    NakedObjects.gotoHelp = "Go to the object referenced in a property,\n" +
        "or to a collection within an object,\n" +
        "or to an object within an open list or collection.\n" +
        "Goto takes one argument.  In the context of an object\n" +
        "that is the name or partial name of the property or collection.\n" +
        "In the context of an open list or collection, it is the\n" +
        "number of the item within the list or collection (starting at 1). ";
    NakedObjects.helpCommand = "help";
    NakedObjects.helpHelp = "If no argument is specified, help provides a basic explanation of how to use Cicero.\n" +
        "If help is followed by a question mark as an argument, this lists the commands available\n" +
        "in the current context. If help is followed by another command word as an argument\n" +
        "(or an abbreviation of it), a description of the specified Command is returned.";
    NakedObjects.menuCommand = "menu";
    NakedObjects.menuHelp = "Open a named main menu, from any context.\n" +
        "Menu takes one optional argument: the name, or partial name, of the menu.\n" +
        "If the partial name matches more than one menu, a list of matches is returned\n" +
        "but no menu is opened; if no argument is provided a list of all the menus\n" +
        "is returned.";
    NakedObjects.okCommand = "ok";
    NakedObjects.okHelp = "Invoke the action currently open as a dialog.\n" +
        "Fields in the dialog should be completed before this.";
    NakedObjects.pageCommand = "page";
    NakedObjects.pageFirst = "first";
    NakedObjects.pagePrevious = "previous";
    NakedObjects.pageNext = "next";
    NakedObjects.pageLast = "last";
    NakedObjects.pageHelp = "Supports paging of returned lists.\n" +
        "The page command takes a single argument, which may be one of these four words:\n" +
        "first, previous, next, or last, \n" +
        "which may be abbreviated down to the first character.\n" +
        "Alternative, the argument may be a specific page number.";
    NakedObjects.reloadCommand = "reload";
    NakedObjects.reloadHelp = "Not yet implemented. Reload the data from the server for an object or a list.\n" +
        "Note that for a list, which was generated by an action, reload runs the action again, \n" +
        "thus ensuring that the list is up to date. However, reloading a list does not reload the\n" +
        "individual objects in that list, which may still be cached. Invoking Reload on an\n" +
        "individual object, however, will ensure that its fields show the latest server data.";
    NakedObjects.rootCommand = "root";
    NakedObjects.rootHelp = "From within an opend collection context, the root command returns\n" +
        " to the root object that owns the collection. Does not take any arguments.\n";
    NakedObjects.saveCommand = "save";
    NakedObjects.saveHelp = "Save the updated fields on an object that is being edited,\n" +
        "and return from edit mode to a normal view of that object";
    NakedObjects.selectionCommand = "selection";
    NakedObjects.selectionHelp = "Not fully implemented. Select one or more items from a list,\n" +
        "prior to invoking an action on the selection.\n" +
        "Selection has one mandatory argument, which must be one of these words,\n" +
        "add, remove, all, clear, show.\n" +
        "The Add and Remove options must be followed by a second argument specifying\n" +
        "the item number, or range, to be added or removed.\n";
    NakedObjects.showCommand = "show";
    NakedObjects.showHelp = "In the context of an object, shows the name and content of\n" +
        "one or more of the properties.\n" +
        "May take 1 argument: the partial field name.\n" +
        "If this matches more than one property, a list of matches is returned.\n" +
        "If no argument is provided, the full list of properties is returned.\n" +
        "In the context of an opened object collection, or a list,\n" +
        "shows one or more items from that collection or list.\n" +
        "If no arguments are specified, show will list all of the the items in the collection,\n" +
        "or the first page of items if in a list view.\n" +
        "Alternatively, the command may be specified with an item number, or a range such as 3- 5.";
    NakedObjects.whereCommand = "where";
    NakedObjects.whereHelp = "Display a reminder of the current context.\n" +
        "The same can also be achieved by hitting the Return key on the empty input field.";
    //Cicero feedback messages
    NakedObjects.commandTooShort = "Command word must have at least 2 characters";
    NakedObjects.noCommandMatch = function (a) { return ("No command begins with " + a); };
    NakedObjects.commandsAvailable = "Commands available in current context:\n";
    NakedObjects.noArguments = "No arguments provided";
    NakedObjects.tooFewArguments = "Too few arguments provided";
    NakedObjects.tooManyArguments = "Too many arguments provided";
    NakedObjects.commandNotAvailable = function (c) { return ("The command: " + c + " is not available in the current context"); };
    NakedObjects.startHigherEnd = "Starting item number cannot be greater than the ending item number";
    NakedObjects.highestItem = function (n) { return ("The highest numbered item is " + n); };
    NakedObjects.item = "item";
    NakedObjects.empty = "empty";
    NakedObjects.numberOfItems = function (n) { return (n + " items"); };
    NakedObjects.on = "on";
    NakedObjects.collection = "Collection";
    NakedObjects.modified = "modified";
    NakedObjects.properties = "properties";
    NakedObjects.modifiedProperties = "Modified " + NakedObjects.properties;
    NakedObjects.noVisible = "No visible properties";
    NakedObjects.doesNotMatch = function (name) { return (name + " does not match any properties"); };
    NakedObjects.alreadyOnFirst = "List is already showing the first page";
    NakedObjects.alreadyOnLast = "List is already showing the last page";
    NakedObjects.pageArgumentWrong = "The argument must match: first, previous, next, last, or a single number";
    NakedObjects.pageNumberWrong = function (max) { return ("Specified page number must be between 1 and " + max); };
    NakedObjects.mayNotbeChainedMessage = function (c, r) { return (c + " command may not be chained" + r + ". Use Where command to see where execution stopped."); };
    NakedObjects.queryOnlyRider = " unless the action is query-only";
    NakedObjects.noSuchCommand = function (c) { return ("No such command: " + c); };
    NakedObjects.missingArgument = function (i) { return ("Required argument number " + i + " is missing"); };
    NakedObjects.wrongTypeArgument = function (i) { return ("Argument number " + i + " must be a number"); };
    NakedObjects.isNotANumber = function (s) { return (s + " is not a number"); };
    NakedObjects.tooManyDashes = "Cannot have more than one dash in argument";
    NakedObjects.mustBeGreaterThanZero = "Item number or range values must be greater than zero";
    NakedObjects.pleaseCompleteOrCorrect = "Please complete or correct these fields:\n";
    NakedObjects.required = "required";
    NakedObjects.mustbeQuestionMark = "Second argument may only be a question mark -  to get action details";
    NakedObjects.noActionsAvailable = "No actions available";
    NakedObjects.doesNotMatchActions = function (a) { return (a + " does not match any actions"); };
    NakedObjects.matchingActions = "Matching actions:\n";
    NakedObjects.actionsMessage = "Actions:\n";
    NakedObjects.actionPrefix = "Action:";
    NakedObjects.disabledPrefix = "disabled:";
    NakedObjects.isDisabled = "is disabled.";
    NakedObjects.noDescription = "No description provided";
    NakedObjects.descriptionPrefix = "Description for action:";
    NakedObjects.clipboardError = "Clipboard command may only be followed by copy, show, go, or discard";
    NakedObjects.clipboardContextError = "Clipboard copy may only be used in the context of viewing an object";
    NakedObjects.clipboardContents = function (contents) { return ("Clipboard contains: " + contents); };
    NakedObjects.clipboardEmpty = "Clipboard is empty";
    NakedObjects.doesNotMatchProperties = function (name) { return (name + " does not match any properties"); };
    NakedObjects.matchesMultiple = "matches multiple fields:\n";
    NakedObjects.doesNotMatchDialog = function (name) { return (name + " does not match any fields in the dialog"); };
    NakedObjects.multipleFieldMatches = "Multiple fields match";
    NakedObjects.isNotModifiable = "is not modifiable";
    NakedObjects.invalidCase = "Invalid case";
    NakedObjects.invalidRefEntry = "Invalid entry for a reference field. Use clipboard or clip";
    NakedObjects.emptyClipboard = "Cannot use Clipboard as it is empty";
    NakedObjects.incompatibleClipboard = "Contents of Clipboard are not compatible with the field";
    NakedObjects.noMatch = function (s) { return ("None of the choices matches " + s); };
    NakedObjects.multipleMatches = "Multiple matches:\n";
    NakedObjects.fieldName = function (name) { return ("Field name: " + name); };
    NakedObjects.descriptionFieldPrefix = "Description:";
    NakedObjects.typePrefix = "Type:";
    NakedObjects.unModifiablePrefix = function (reason) { return ("Unmodifiable: " + reason); };
    NakedObjects.outOfItemRange = function (n) { return (n + " is out of range for displayed items"); };
    NakedObjects.doesNotMatchMenu = function (name) { return (name + " does not match any menu"); };
    NakedObjects.matchingMenus = "Matching menus:";
    NakedObjects.menuTitle = function (title) { return (title + " menu"); };
    NakedObjects.allMenus = "Menus:";
    NakedObjects.noRefFieldMatch = function (s) { return (s + " does not match any reference fields or collections"); };
    NakedObjects.unsaved = "Unsaved";
    NakedObjects.editing = "Editing";
    //Error messages
    NakedObjects.errorUnknown = "Unknown software error";
    NakedObjects.errorExpiredTransient = "The requested view of unsaved object details has expired";
    NakedObjects.errorWrongType = "An unexpected type of result was returned";
    NakedObjects.errorNotImplemented = "The requested software feature is not implemented";
    NakedObjects.errorSoftware = "A software error occurred";
    NakedObjects.errorConnection = "The client failed to connect to the server";
    NakedObjects.errorClient = "Client Error";
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.userMessages.config.js.map