/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/angularjs/angular-route.d.ts" />
/// <reference path="nakedobjects.config.ts" />
var NakedObjects;
(function (NakedObjects) {
    // code constants - not for config
    var path = null;
    // path local server (ie ~) only needed if you want to hard code paths for some reason
    var svrPath = "";
    function getSvrPath() {
        if (!path) {
            var trimmedPath = svrPath.trim();
            if (trimmedPath.length === 0 || trimmedPath.charAt(svrPath.length - 1) === "/") {
                path = trimmedPath;
            }
            else {
                path = trimmedPath + "/";
            }
        }
        return path;
    }
    NakedObjects.getSvrPath = getSvrPath;
    NakedObjects.dialogTemplate = getSvrPath() + "Content/partials/dialog.html";
    NakedObjects.serviceTemplate = getSvrPath() + "Content/partials/service.html";
    NakedObjects.errorTemplate = getSvrPath() + "Content/partials/error.html";
    NakedObjects.concurrencyTemplate = getSvrPath() + "Content/partials/concurrencyError.html";
    NakedObjects.httpErrorTemplate = getSvrPath() + "Content/partials/httpError.html";
    NakedObjects.appBarTemplate = getSvrPath() + "Content/partials/appbar.html";
    NakedObjects.nullTemplate = getSvrPath() + "Content/partials/null.html";
    NakedObjects.blankTemplate = getSvrPath() + "Content/partials/blank.html";
    NakedObjects.homeTemplate = getSvrPath() + "Content/partials/home.html";
    NakedObjects.homePlaceholderTemplate = getSvrPath() + "Content/partials/homePlaceholder.html";
    NakedObjects.objectTemplate = getSvrPath() + "Content/partials/object.html";
    NakedObjects.objectViewTemplate = getSvrPath() + "Content/partials/objectView.html";
    NakedObjects.objectEditTemplate = getSvrPath() + "Content/partials/objectEdit.html";
    NakedObjects.formTemplate = getSvrPath() + "Content/partials/form.html";
    NakedObjects.expiredTransientTemplate = getSvrPath() + "Content/partials/expiredTransient.html";
    NakedObjects.listPlaceholderTemplate = getSvrPath() + "Content/partials/ListPlaceholder.html";
    NakedObjects.listTemplate = getSvrPath() + "Content/partials/List.html";
    NakedObjects.listAsTableTemplate = getSvrPath() + "Content/partials/ListAsTable.html";
    NakedObjects.footerTemplate = getSvrPath() + "Content/partials/footer.html";
    NakedObjects.actionsTemplate = getSvrPath() + "Content/partials/actions.html";
    NakedObjects.formActionsTemplate = getSvrPath() + "Content/partials/formActions.html";
    NakedObjects.collectionsTemplate = getSvrPath() + "Content/partials/collections.html";
    NakedObjects.collectionSummaryTemplate = getSvrPath() + "Content/partials/collectionSummary.html";
    NakedObjects.collectionListTemplate = getSvrPath() + "Content/partials/collectionList.html";
    NakedObjects.collectionTableTemplate = getSvrPath() + "Content/partials/collectionTable.html";
    NakedObjects.recentTemplate = getSvrPath() + "Content/partials/recent.html";
    NakedObjects.attachmentTemplate = getSvrPath() + "Content/partials/attachment.html";
    NakedObjects.ciceroTemplate = getSvrPath() + "Content/partials/cicero.html";
    // routing constants 
    NakedObjects.geminiPath = "gemini";
    NakedObjects.ciceroPath = "cicero";
    NakedObjects.homePath = "home";
    NakedObjects.objectPath = "object";
    NakedObjects.listPath = "list";
    NakedObjects.errorPath = "error";
    NakedObjects.recentPath = "recent";
    NakedObjects.attachmentPath = "attachment";
    //Restful Objects constants
    NakedObjects.roDomainType = "x-ro-domain-type";
    NakedObjects.roInvalidReason = "x-ro-invalidReason";
    NakedObjects.roSearchTerm = "x-ro-searchTerm";
    NakedObjects.roPage = "x-ro-page";
    NakedObjects.roPageSize = "x-ro-pageSize";
    NakedObjects.roInlinePropertyDetails = "x-ro-inline-property-details";
    NakedObjects.roValidateOnly = "x-ro-validate-only";
    NakedObjects.roInlineCollectionItems = "x-ro-inline-collection-items";
    //NOF custom RO constants  
    NakedObjects.nofWarnings = "x-ro-nof-warnings";
    NakedObjects.nofMessages = "x-ro-nof-messages";
    NakedObjects.supportedDateFormats = ["D/M/YYYY", "D/M/YY", "D MMM YYYY", "D MMMM YYYY", "D MMM YY", "D MMMM YY"];
    // events 
    NakedObjects.geminiFocusEvent = "nof-focus-on";
    NakedObjects.geminiLogoffEvent = "nof-logoff";
    NakedObjects.geminiPaneSwapEvent = "nof-pane-swap";
    NakedObjects.geminiDisplayErrorEvent = "nof-display-error";
    NakedObjects.geminiAjaxChangeEvent = "nof-ajax-change";
    NakedObjects.geminiWarningEvent = "nof-warning";
    NakedObjects.geminiMessageEvent = "nof-message";
    //Cicero commands and Help text
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
    NakedObjects.pageHelp = "Supports paging of returned lists.\n" +
        "The page command takes a single argument, which may be one of these four words:\n" +
        "first, previous, next, or last, \n" +
        "which may be abbreviated down to the first character.\n" +
        "Alternative, the argument may be a specific page number.";
    NakedObjects.propertyCommand = "property";
    NakedObjects.propertyHelp = "Display the name and content of one or more properties of an object.\n" +
        "Field may take 1 argument:  the partial field name.\n" +
        "If this matches more than one property, a list of matches is returned.\n" +
        "If no argument is provided, the full list of properties is returned. ";
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
    NakedObjects.showHelp = "Show one or more of the items from or a list view,\n" +
        "or an opened object collection. If no arguments are specified, \n" +
        "show will list all of the the items in the opened object collection,\n" +
        "or the first page of items if in a list view.\n" +
        "Alternatively, the command may be specified with an item number,\n" +
        "or a range such as 3-5.";
    NakedObjects.whereCommand = "where";
    NakedObjects.whereHelp = "Display a reminder of the current context.\n" +
        "The same can also be achieved by hitting the Return key on the empty input field.";
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.constants.js.map