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

export const pageMessage = (p: number, tp: number, c: number, tc: number) => `Page ${p} of ${tp}; viewing ${c} of ${tc} items`;

export const logOffMessage = (u: string) => `Please confirm logoff of user: ${u}`;

export const submittedCount = (c: number) => ` with ${c} lines submitted.`;

// Error messages
export const errorUnknown = 'Unknown software error';
export const errorExpiredTransient = 'The requested view of unsaved object details has expired';
export const errorWrongType = 'An unexpected type of result was returned';
export const errorNotImplemented = 'The requested software feature is not implemented';
export const errorSoftware = 'A software error occurred';
export const errorConnection = 'The client failed to connect to the server';
export const errorClient = 'Client Error';
