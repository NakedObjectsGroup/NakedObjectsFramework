import * as Models from '@nakedobjects/restful-objects';
import { DateTime } from 'luxon';
import { ILocalFilter } from '@nakedobjects/services';
import * as Msg from './user-messages';
import { fixedDateFormat } from '@nakedobjects/services';


function isInteger(value: number) {
    return typeof value === 'number' && isFinite(value) && Math.floor(value) === value;
}

function getDate(val: string): Date | null {
    const dt1 = DateTime.fromFormat(val, fixedDateFormat); 
    return dt1.isValid ? dt1.toJSDate() : null;
}

export function validateNumber(model: Models.IHasExtensions, newValue: number, filter: ILocalFilter): string {
    const format = model.extensions().format();

    switch (format) {
        case ('int'):
            if (!isInteger(newValue)) {
                return 'Not an integer';
            }
    }

    const range = model.extensions().range();

    if (range) {
        const min = range.min;
        const max = range.max;

        if (min && newValue < min) {
            return Msg.outOfRange(newValue, min, max, filter);
        }

        if (max && newValue > max) {
            return Msg.outOfRange(newValue, min, max, filter);
        }
    }

    return '';
}

export function validateStringFormat(model: Models.IHasExtensions, newValue: string): string {

    const maxLength = model.extensions().maxLength();
    const pattern = model.extensions().pattern();
    const len = newValue ? newValue.length : 0;

    if (maxLength && len > maxLength) {
        return Msg.tooLong;
    }

    if (pattern) {
        const regex = new RegExp(pattern);
        return regex.test(newValue) ? '' : Msg.noPatternMatch;
    }
    return '';
}

export function validateDateTimeFormat(model: Models.IHasExtensions, newValue: string): string {
    return '';
}

export function validateDateFormat(model: Models.IHasExtensions, newValue: string, filter: ILocalFilter): string {
    const range = model.extensions().range();
    const newDate = getDate(newValue);

    if (range && newDate) {
        const min = range.min ? getDate(range.min as string) : null;
        const max = range.max ? getDate(range.max as string) : null;

        if (min && newDate < min) {
            return Msg.outOfRange(Models.toDateString(newDate), min.toISOString(), max?.toISOString(), filter);
        }

        if (max && newDate > max) {
            return Msg.outOfRange(Models.toDateString(newDate), min?.toISOString(), max.toISOString(), filter);
        }
    }

    return '';
}

export function validateTimeFormat(model: Models.IHasExtensions, newValue: string): string {
    return '';
}

export function validateString(model: Models.IHasExtensions, newValue: string, filter: ILocalFilter): string {
    const format = model.extensions().format();

    switch (format) {
        case ('string'):
            return validateStringFormat(model, newValue);
        case ('date-time'):
            return validateDateTimeFormat(model, newValue);
        case ('date'):
            return validateDateFormat(model, newValue, filter);
        case ('time'):
            return validateTimeFormat(model, newValue);
        default:
            return '';
    }
}

export function validateMandatory(model: Models.IHasExtensions, viewValue: string): string {
    // first check
    const isMandatory = !model.extensions().optional();

    if (isMandatory && (viewValue === '' || viewValue == null)) {
        return Msg.mandatory;
    }

    return '';
}

export function validateMandatoryAgainstType(model: Models.IHasExtensions, viewValue: string, filter: ILocalFilter): string {

    // check type
    const returnType = model.extensions().returnType();

    switch (returnType) {
        case ('number'):
            const valueAsNumber = parseFloat(viewValue);
            if (Number.isFinite(valueAsNumber)) {
                return validateNumber(model, valueAsNumber, filter);
            }
            return Msg.notANumber;
        case ('string'):
            return validateString(model, viewValue, filter);
        case ('boolean'):
            return '';
        default:
            return '';
    }
}

export function validateDate(newValue: string, validInputFormats: string[]) {

    if (newValue) {
        for (const f of validInputFormats) {
            const dt = DateTime.fromFormat(newValue, f);
            if (dt.isValid) {
                return dt;
            }
        }
    }

    return null;
}
