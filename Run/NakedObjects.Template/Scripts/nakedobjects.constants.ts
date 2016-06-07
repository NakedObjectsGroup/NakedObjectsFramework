/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/angularjs/angular-route.d.ts" />
/// <reference path="nakedobjects.config.ts" />

module NakedObjects {

    // code constants - not for config
    let path: string = null;
    // path local server (ie ~) only needed if you want to hard code paths for some reason
    const svrPath = "";

    export function getSvrPath() {
        if (!path) {
            const trimmedPath = svrPath.trim();
            if (trimmedPath.length === 0 || trimmedPath.charAt(svrPath.length - 1) === "/") {
                path = trimmedPath;
            } else {
                path = trimmedPath + "/";
            }
        }
        return path;
    }

    export const dialogTemplate = getSvrPath() + "Content/partials/dialog.html";
    export const serviceTemplate = getSvrPath() + "Content/partials/service.html";
    export const errorTemplate = getSvrPath() + "Content/partials/error.html";
    export const concurrencyTemplate = getSvrPath() + "Content/partials/concurrencyError.html";
    export const httpErrorTemplate = getSvrPath() + "Content/partials/httpError.html";
    export const appBarTemplate = getSvrPath() + "Content/partials/appbar.html";
    export const nullTemplate = getSvrPath() + "Content/partials/null.html";
    export const blankTemplate = getSvrPath() + "Content/partials/blank.html";
    export const homeTemplate = getSvrPath() + "Content/partials/home.html";
    export const homePlaceholderTemplate = getSvrPath() + "Content/partials/homePlaceholder.html";
    export const objectTemplate = getSvrPath() + "Content/partials/object.html";
    export const objectViewTemplate = getSvrPath() + "Content/partials/objectView.html";
    export const objectEditTemplate = getSvrPath() + "Content/partials/objectEdit.html";
    export const formTemplate = getSvrPath() + "Content/partials/form.html";
    export const expiredTransientTemplate = getSvrPath() + "Content/partials/expiredTransient.html";

    export const listPlaceholderTemplate = getSvrPath() + "Content/partials/ListPlaceholder.html";
    export const listTemplate = getSvrPath() + "Content/partials/List.html";
    export const listAsTableTemplate = getSvrPath() + "Content/partials/ListAsTable.html";

    export const footerTemplate = getSvrPath() + "Content/partials/footer.html";
    export const actionsTemplate = getSvrPath() + "Content/partials/actions.html";
    export const formActionsTemplate = getSvrPath() + "Content/partials/formActions.html";
    export const collectionsTemplate = getSvrPath() + "Content/partials/collections.html";
    export const collectionSummaryTemplate = getSvrPath() + "Content/partials/collectionSummary.html";
    export const collectionListTemplate = getSvrPath() + "Content/partials/collectionList.html";
    export const collectionTableTemplate = getSvrPath() + "Content/partials/collectionTable.html";

    export const recentTemplate = getSvrPath() + "Content/partials/recent.html";
    export const attachmentTemplate = getSvrPath() + "Content/partials/attachment.html";
    export const ciceroTemplate = getSvrPath() + "Content/partials/cicero.html";
    export const applicationPropertiesTemplate = getSvrPath() + "Content/partials/applicationProperties.html";

    // routing constants 

    export const geminiPath = "gemini";
    export const ciceroPath = "cicero";
    export const homePath = "home";
    export const objectPath = "object";
    export const listPath = "list";
    export const errorPath = "error";
    export const recentPath = "recent";
    export const attachmentPath = "attachment";
    export const applicationPropertiesPath = "applicationProperties";

    //Restful Objects constants
    export const roDomainType = "x-ro-domain-type";
    export const roInvalidReason = "x-ro-invalidReason";
    export const roSearchTerm = "x-ro-searchTerm";
    export const roPage = "x-ro-page";
    export const roPageSize = "x-ro-pageSize";
    export const roInlinePropertyDetails = "x-ro-inline-property-details";
    export const roValidateOnly = "x-ro-validate-only";
    export const roInlineCollectionItems = "x-ro-inline-collection-items";

    //NOF custom RO constants  
    export const nofWarnings = "x-ro-nof-warnings";
    export const nofMessages = "x-ro-nof-messages";
   
  
    export const supportedDateFormats = ["D/M/YYYY", "D/M/YY", "D MMM YYYY", "D MMMM YYYY", "D MMM YY", "D MMMM YY"];

    // events 
    export const geminiFocusEvent = "nof-focus-on";
    export const geminiLogoffEvent = "nof-logoff";
    export const geminiPaneSwapEvent = "nof-pane-swap";
    export const geminiDisplayErrorEvent = "nof-display-error";
    export const geminiAjaxChangeEvent = "nof-ajax-change";
    export const geminiWarningEvent = "nof-warning";
    export const geminiMessageEvent = "nof-message";

    //Cicero commands and Help text
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
    export const pageHelp = "Supports paging of returned lists.\n" +
        "The page command takes a single argument, which may be one of these four words:\n" +
        "first, previous, next, or last, \n" +
        "which may be abbreviated down to the first character.\n" +
        "Alternative, the argument may be a specific page number.";
    export const propertyCommand = "property";
    export const propertyHelp = "Display the name and content of one or more properties of an object.\n" +
        "Field may take 1 argument:  the partial field name.\n" +
        "If this matches more than one property, a list of matches is returned.\n" +
        "If no argument is provided, the full list of properties is returned. ";
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
    export const showHelp = "Show one or more of the items from or a list view,\n" +
        "or an opened object collection. If no arguments are specified, \n" +
        "show will list all of the the items in the opened object collection,\n" +
        "or the first page of items if in a list view.\n" +
        "Alternatively, the command may be specified with an item number,\n" +
        "or a range such as 3-5.";
    export const whereCommand = "where";
    export const whereHelp = "Display a reminder of the current context.\n" +
        "The same can also be achieved by hitting the Return key on the empty input field.";
}