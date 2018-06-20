import { ILocalFilter } from './mask.service';

// user message constants
export const noResultMessage = "no result found";
export const obscuredText = "*****";
export const tooShort = "Too short";
export const tooLong = "Too long";
export const notAnInteger = "Not an integer";
export const notANumber = "Not a number";
export const mandatory = "Mandatory";
export const optional = "Optional";
export const choices = "Choices";
export const pendingAutoComplete = "Pending auto-complete...";
export const noPatternMatch = "Invalid entry";
export const closeActions = "Close actions";
export const noActions = "No actions available";
export const openActions = "Open actions (Alt-a)";
export const mandatoryFieldsPrefix = "Missing mandatory fields: ";
export const invalidFieldsPrefix = "Invalid fields: ";
export const unknownFileTitle = "UnknownFile";
export const unknownCollectionSize = "";
export const emptyCollectionSize = "Empty";
export const noItemsFound = "No items found";
export const noItemsSelected = "Must select items for collection contributed action";
export const dropPrompt = "(drop here)";
export const autoCompletePrompt = "(auto-complete or drop)";
export const concurrencyMessage = "The object has been reloaded to pick up changes by another user. Please review, and re-enter any changes you still require.";
export const loadingMessage = "Loading...";
export const submittedMessage = "Submitted";

export const today = "Today";
export const clear = "Clear";
export const recentDisabledMessage = "Nothing to clear";
export const recentMessage = "Clear recent history";
export const recentTitle = "Recently Viewed Objects";
export const noUserMessage = "'No user set'";
export const noImageMessage = "No image";

export const invalidDate = "Invalid date";
export const invalidTime = "Invalid time";

export const outOfRange = (val: any, min: any, max: any, filter: ILocalFilter) => {
    const minVal = filter ? filter.filter(min) : min;
    const maxVal = filter ? filter.filter(max) : max;

    return `Value is outside the range ${minVal || "unlimited"} to ${maxVal || "unlimited"}`;
};

export const pageMessage = (p: number, tp: number, c: number, tc: number) => `Page ${p} of ${tp}; viewing ${c} of ${tc} items`;

export const logOffMessage = (u: string) => `Please confirm logoff of user: ${u}`;

export const submittedCount = (c: number) => ` with ${c} lines submitted.`;

// Cicero commands and Help text

export const welcomeMessage = "Welcome to Cicero. Type 'help' and the Enter key for more information.";
export const basicHelp = "Cicero is a user interface purpose-designed to work with an audio screen-reader.\n" +
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
    "and a comma between arguments.\n" +
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
    "ready for the next command. The user may recall the previous command by hitting the up-arrow key.\n" +
    "The user might then edit or extend that previous command and hit Enter to run it again.\n" +
    "For advanced users: commands may be chained using a semi-colon between them,\n" +
    "however commands that do, or might, result in data updates cannot be chained.";
export const actionCommand = "action";
export const actionHelp = "Open the dialog for action from a menu, or from object actions.\n" +
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
export const backCommand = "back";
export const backHelp = "Move back to the previous context.";
export const cancelCommand = "cancel";
export const cancelHelp = "Leave the current activity (action dialog, or object edit), incomplete.";
export const clipboardCommand = "clipboard";
export const clipboardCopy = "copy";
export const clipboardShow = "show";
export const clipboardGo = "go";
export const clipboardDiscard = "discard";
export const clipboardHelp = "The clipboard command is used for temporarily\n" +
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
export const editCommand = "edit";
export const editHelp = "Put an object into Edit mode.";
export const enterCommand = "enter";
export const enterHelp = "Enter a value into a field,\n" +
    "meaning a parameter in an action dialog,\n" +
    "or  a property on an object being edited.\n" +
    "Enter requires 2 arguments.\n" +
    "The first argument is the partial field name, which must match a single field.\n" +
    "The second optional argument specifies the value, or selection, to be entered.\n" +
    "If a question mark is provided as the second argument, the field will not be\n" +
    "updated but further details will be provided about that input field.\n" +
    "If the word paste is used as the second argument, then, provided that the field is\n" +
    "a reference field, the object reference in the clipboard will be pasted into the field.\n";
export const forwardCommand = "forward";
export const forwardHelp = "Move forward to next context in the history\n" +
    "(if you have previously moved back).";
export const geminiCommand = "gemini";
export const geminiHelp = "Switch to the Gemini (graphical) user interface\n" +
    "preserving the current context.";
export const gotoCommand = "goto";
export const gotoHelp = "Go to the object referenced in a property,\n" +
    "or to a collection within an object,\n" +
    "or to an object within an open list or collection.\n" +
    "Goto takes one argument.  In the context of an object\n" +
    "that is the name or partial name of the property or collection.\n" +
    "In the context of an open list or collection, it is the\n" +
    "number of the item within the list or collection (starting at 1). ";
export const helpCommand = "help";
export const helpHelp = "If no argument is specified, help provides a basic explanation of how to use Cicero.\n" +
    "If help is followed by a question mark as an argument, this lists the commands available\n" +
    "in the current context. If help is followed by another command word as an argument\n" +
    "(or an abbreviation of it), a description of the specified Command is returned.";
export const menuCommand = "menu";
export const menuHelp = "Open a named main menu, from any context.\n" +
    "Menu takes one optional argument: the name, or partial name, of the menu.\n" +
    "If the partial name matches more than one menu, a list of matches is returned\n" +
    "but no menu is opened; if no argument is provided a list of all the menus\n" +
    "is returned.";
export const okCommand = "ok";
export const okHelp = "Invoke the action currently open as a dialog.\n" +
    "Fields in the dialog should be completed before this.";
export const pageCommand = "page";
export const pageFirst = "first";
export const pagePrevious = "previous";
export const pageNext = "next";
export const pageLast = "last";
export const pageHelp = "Supports paging of returned lists.\n" +
    "The page command takes a single argument, which may be one of these four words:\n" +
    "first, previous, next, or last, \n" +
    "which may be abbreviated down to the first character.\n" +
    "Alternative, the argument may be a specific page number.";
export const reloadCommand = "reload";
export const reloadHelp = "Not yet implemented. Reload the data from the server for an object or a list.\n" +
    "Note that for a list, which was generated by an action, reload runs the action again, \n" +
    "thus ensuring that the list is up to date. However, reloading a list does not reload the\n" +
    "individual objects in that list, which may still be cached. Invoking Reload on an\n" +
    "individual object, however, will ensure that its fields show the latest server data.";
export const rootCommand = "root";
export const rootHelp = "From within an opend collection context, the root command returns\n" +
    " to the root object that owns the collection. Does not take any arguments.\n";
export const saveCommand = "save";
export const saveHelp = "Save the updated fields on an object that is being edited,\n" +
    "and return from edit mode to a normal view of that object";
export const selectionCommand = "selection";
export const selectionHelp = "Not fully implemented. Select one or more items from a list,\n" +
    "prior to invoking an action on the selection.\n" +
    "Selection has one mandatory argument, which must be one of these words,\n" +
    "add, remove, all, clear, show.\n" +
    "The Add and Remove options must be followed by a second argument specifying\n" +
    "the item number, or range, to be added or removed.\n";
export const showCommand = "show";
export const showHelp = "In the context of an object, shows the name and content of\n" +
    "one or more of the properties.\n" +
    "May take 1 argument: the partial field name.\n" +
    "If this matches more than one property, a list of matches is returned.\n" +
    "If no argument is provided, the full list of properties is returned.\n" +
    "In the context of an opened object collection, or a list,\n" +
    "shows one or more items from that collection or list.\n" +
    "If no arguments are specified, show will list all of the the items in the collection,\n" +
    "or the first page of items if in a list view.\n" +
    "Alternatively, the command may be specified with an item number, or a range such as 3- 5.";
export const whereCommand = "where";
export const whereHelp = "Display a reminder of the current context.\n" +
    "The same can also be achieved by hitting the Return key on the empty input field.";

// Cicero feedback messages
export const commandTooShort = "Command word must have at least 2 characters";
export const noCommandMatch = (a: string) => `No command begins with ${a}`;
export const commandsAvailable = "Commands available in current context:\n";

export const noArguments = "No arguments provided";
export const tooFewArguments = "Too few arguments provided";
export const tooManyArguments = "Too many arguments provided";

export const commandNotAvailable = (c: string) => `The command: ${c} is not available in the current context`;

export const startHigherEnd = "Starting item number cannot be greater than the ending item number";

export const highestItem = (n: number) => `The highest numbered item is ${n}`;

export const item = "item";
export const empty = "empty";
export const numberOfItems = (n: number) => `${n} items`;
export const on = "on";
export const collection = "Collection";
export const modified = "modified";
export const properties = "properties";
export const modifiedProperties = `Modified ${properties}`;
export const page = "Page";

export const noVisible = "No visible properties";

export const doesNotMatch = (name: string) => `${name} does not match any properties`;

export const cannotPage = "Cannot page list";

export const alreadyOnFirst = "List is already showing the first page";

export const alreadyOnLast = "List is already showing the last page";

export const pageArgumentWrong = "The argument must match: first, previous, next, last, or a single number";

export const pageNumberWrong = (max: number) => `Specified page number must be between 1 and ${max}`;

export const mayNotbeChainedMessage = (c: string, r: string) => `${c} command may not be chained${r}. Use Where command to see where execution stopped.`;

export const queryOnlyRider = " unless the action is query-only";

export const noSuchCommand = (c: string) => `No such command: ${c}`;

export const missingArgument = (i: number) => `Required argument number ${i} is missing`;

export const wrongTypeArgument = (i: number) => `Argument number ${i} must be a number`;

export const isNotANumber = (s: string) => `${s} is not a number`;

export const tooManyDashes = "Cannot have more than one dash in argument";

export const mustBeGreaterThanZero = "Item number or range values must be greater than zero";

export const pleaseCompleteOrCorrect = "Please complete or correct these fields:\n";

export const required = "required";

export const mustbeQuestionMark = "Second argument may only be a question mark -  to get action details";

export const noActionsAvailable = "No actions available";

export const doesNotMatchActions = (a: string | undefined) => `${a} does not match any actions`;

export const matchingActions = "Matching actions:\n";
export const actionsMessage = "Actions:\n";
export const actionPrefix = "Action:";
export const disabledPrefix = "disabled:";

export const isDisabled = "is disabled.";

export const noDescription = "No description provided";

export const descriptionPrefix = "Description for action:";

export const clipboardError = "Clipboard command may only be followed by copy, show, go, or discard";

export const clipboardContextError = "Clipboard copy may only be used in the context of viewing an object";

export const clipboardContents = (contents: string) => `Clipboard contains: ${contents}`;

export const clipboardEmpty = "Clipboard is empty";

export const doesNotMatchProperties = (name: string | undefined) => `${name} does not match any properties`;

export const matchesMultiple = "matches multiple fields:\n";

export const doesNotMatchDialog = (name: string | undefined) => `${name} does not match any fields in the dialog`;

export const multipleFieldMatches = "Multiple fields match";

export const isNotModifiable = "is not modifiable";

export const invalidCase = "Invalid case";

export const invalidRefEntry = "Invalid entry for a reference field. Use clipboard or clip";

export const emptyClipboard = "Cannot use Clipboard as it is empty";
export const incompatibleClipboard = "Contents of Clipboard are not compatible with the field";
export const noMatch = (s: string) => `None of the choices matches ${s}`;
export const multipleMatches = "Multiple matches:\n";
export const fieldName = (name: string) => `Field name: ${name}`;
export const descriptionFieldPrefix = "Description:";
export const typePrefix = "Type:";

export const unModifiablePrefix = (reason: string) => `Unmodifiable: ${reason}`;
export const outOfItemRange = (n: number | undefined) => `${n} is out of range for displayed items`;
export const doesNotMatchMenu = (name: string | undefined) => `${name} does not match any menu`;
export const matchingMenus = "Matching menus:";
export const menuTitle = (title: string) => `${title} menu`;
export const allMenus = "Menus:";
export const noRefFieldMatch = (s: string) => `${s} does not match any reference fields or collections`;
export const unsaved = "Unsaved";
export const editing = "Editing";

// Error messages
export const errorUnknown = "Unknown software error";
export const errorExpiredTransient = "The requested view of unsaved object details has expired";
export const errorWrongType = "An unexpected type of result was returned";
export const errorNotImplemented = "The requested software feature is not implemented";
export const errorSoftware = "A software error occurred";
export const errorConnection = "The client failed to connect to the server";
export const errorClient = "Client Error";
