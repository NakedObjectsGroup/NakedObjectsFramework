import { ILocalFilter } from './mask.service';

export const tooLong = 'Too long';
export const notANumber = 'Not a number';
export const mandatory = 'Mandatory';
export const noPatternMatch = 'Invalid entry';

export const outOfRange = (val: any, min: any, max: any, filter: ILocalFilter) => {
    const minVal = filter ? filter.filter(min) : min;
    const maxVal = filter ? filter.filter(max) : max;

    return `Value is outside the range ${minVal || 'unlimited'} to ${maxVal || 'unlimited'}`;
};

// Error messages
export const errorUnknown = 'Unknown software error';
export const errorExpiredTransient = 'The requested view of unsaved object details has expired';
export const errorWrongType = 'An unexpected type of result was returned';
export const errorNotImplemented = 'The requested software feature is not implemented';
export const errorSoftware = 'A software error occurred';
export const errorConnection = 'The client failed to connect to the server';
export const errorClient = 'Client Error';
