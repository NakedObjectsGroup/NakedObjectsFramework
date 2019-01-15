import { ILocalFilter } from '@nakedobjects/services';

// user message constants
export const noResultMessage = 'no result found';
export const obscuredText = '*****';
export const tooShort = 'Too short';
export const tooLong = 'Too long';
export const notAnInteger = 'Not an integer';
export const notANumber = 'Not a number';
export const mandatory = 'Mandatory';
export const optional = 'Optional';
export const choices = 'Choices';
export const pendingAutoComplete = 'Pending auto-complete...';
export const noPatternMatch = 'Invalid entry';
export const closeActions = 'Close actions';
export const noActions = 'No actions available';
export const openActions = 'Open actions (Alt-a)';
export const mandatoryFieldsPrefix = 'Missing mandatory fields: ';
export const invalidFieldsPrefix = 'Invalid fields: ';
export const unknownFileTitle = 'UnknownFile';
export const unknownCollectionSize = '';
export const emptyCollectionSize = 'Empty';
export const noItemsFound = 'No items found';
export const noItemsSelected = 'Must select items for collection contributed action';
export const dropPrompt = '(drop here)';
export const autoCompletePrompt = '(auto-complete or drop)';
export const concurrencyMessage = 'The object has been reloaded to pick up changes by another user. Please review, and re-enter any changes you still require.';
export const loadingMessage = 'Loading...';
export const submittedMessage = 'Submitted';

export const today = 'Today';
export const clear = 'Clear';
export const recentDisabledMessage = 'Nothing to clear';
export const recentMessage = 'Clear recent history';
export const recentTitle = 'Recently Viewed Objects';
export const noUserMessage = '\'No user set\'';
export const noImageMessage = 'No image';

export const invalidDate = 'Invalid date';
export const invalidTime = 'Invalid time';

export const outOfRange = (val: any, min: any, max: any, filter: ILocalFilter) => {
    const minVal = filter ? filter.filter(min) : min;
    const maxVal = filter ? filter.filter(max) : max;

    return `Value is outside the range ${minVal || 'unlimited'} to ${maxVal || 'unlimited'}`;
};

export const pageMessage = (p: number, tp: number, c: number, tc: number) => `Page ${p} of ${tp}; viewing ${c} of ${tc} items`;

export const logOffMessage = (u: string) => `Please confirm logoff of user: ${u}`;

export const submittedCount = (c: number) => ` with ${c} lines submitted.`;

export const commandNotAvailable = (c: string) => `The command: ${c} is not available in the current context`;

export const startHigherEnd = 'Starting item number cannot be greater than the ending item number';

export const highestItem = (n: number) => `The highest numbered item is ${n}`;

export const item = 'item';
export const empty = 'empty';
export const numberOfItems = (n: number) => `${n} items`;
export const on = 'on';
export const collection = 'Collection';
export const modified = 'modified';
export const properties = 'properties';
export const modifiedProperties = `Modified ${properties}`;
export const page = 'Page';

export const noVisible = 'No visible properties';

export const doesNotMatch = (name: string) => `${name} does not match any properties`;

export const cannotPage = 'Cannot page list';

export const alreadyOnFirst = 'List is already showing the first page';

export const alreadyOnLast = 'List is already showing the last page';

export const pageArgumentWrong = 'The argument must match: first, previous, next, last, or a single number';

export const pageNumberWrong = (max: number) => `Specified page number must be between 1 and ${max}`;

export const mayNotbeChainedMessage = (c: string, r: string) => `${c} command may not be chained${r}. Use Where command to see where execution stopped.`;

export const queryOnlyRider = ' unless the action is query-only';

export const noSuchCommand = (c: string) => `No such command: ${c}`;

export const missingArgument = (i: number) => `Required argument number ${i} is missing`;

export const wrongTypeArgument = (i: number) => `Argument number ${i} must be a number`;

export const isNotANumber = (s: string) => `${s} is not a number`;

export const tooManyDashes = 'Cannot have more than one dash in argument';

export const mustBeGreaterThanZero = 'Item number or range values must be greater than zero';

export const pleaseCompleteOrCorrect = 'Please complete or correct these fields:\n';

export const required = 'required';

export const mustbeQuestionMark = 'Second argument may only be a question mark -  to get action details';

export const noActionsAvailable = 'No actions available';

export const doesNotMatchActions = (a: string | undefined) => `${a} does not match any actions`;

export const matchingActions = 'Matching actions:\n';
export const actionsMessage = 'Actions:\n';
export const actionPrefix = 'Action:';
export const disabledPrefix = 'disabled:';

export const isDisabled = 'is disabled.';

export const noDescription = 'No description provided';

export const descriptionPrefix = 'Description for action:';

export const clipboardError = 'Clipboard command may only be followed by copy, show, go, or discard';

export const clipboardContextError = 'Clipboard copy may only be used in the context of viewing an object';

export const clipboardContents = (contents: string) => `Clipboard contains: ${contents}`;

export const clipboardEmpty = 'Clipboard is empty';

export const doesNotMatchProperties = (name: string | undefined) => `${name} does not match any properties`;

export const matchesMultiple = 'matches multiple fields:\n';

export const doesNotMatchDialog = (name: string | undefined) => `${name} does not match any fields in the dialog`;

export const multipleFieldMatches = 'Multiple fields match';

export const isNotModifiable = 'is not modifiable';

export const invalidCase = 'Invalid case';

export const invalidRefEntry = 'Invalid entry for a reference field. Use clipboard or clip';

export const emptyClipboard = 'Cannot use Clipboard as it is empty';
export const incompatibleClipboard = 'Contents of Clipboard are not compatible with the field';
export const noMatch = (s: string) => `None of the choices matches ${s}`;
export const multipleMatches = 'Multiple matches:\n';
export const fieldName = (name: string) => `Field name: ${name}`;
export const descriptionFieldPrefix = 'Description:';
export const typePrefix = 'Type:';

export const unModifiablePrefix = (reason: string) => `Unmodifiable: ${reason}`;
export const outOfItemRange = (n: number | undefined) => `${n} is out of range for displayed items`;
export const doesNotMatchMenu = (name: string | undefined) => `${name} does not match any menu`;
export const matchingMenus = 'Matching menus:';
export const menuTitle = (title: string) => `${title} menu`;
export const allMenus = 'Menus:';
export const noRefFieldMatch = (s: string) => `${s} does not match any reference fields or collections`;
export const unsaved = 'Unsaved';
export const editing = 'Editing';

// Error messages
export const errorUnknown = 'Unknown software error';
export const errorExpiredTransient = 'The requested view of unsaved object details has expired';
export const errorWrongType = 'An unexpected type of result was returned';
export const errorNotImplemented = 'The requested software feature is not implemented';
export const errorSoftware = 'A software error occurred';
export const errorConnection = 'The client failed to connect to the server';
export const errorClient = 'Client Error';
