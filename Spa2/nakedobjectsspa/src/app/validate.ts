import * as Models from './models';
import * as Msg from './user-messages';
import { ILocalFilter } from './mask.service';
import * as momentNs from 'moment';
import * as Constants from './constants';

const moment = momentNs;

function isInteger(value: number) {
    return typeof value === "number" && isFinite(value) && Math.floor(value) === value;
}

function getDate(val: string): Date | null {
    const dt1 = moment(val, Constants.fixedDateFormat, true);
    return dt1.isValid() ? dt1.toDate() : null;
}

export function validateNumber(model: Models.IHasExtensions, newValue: number, filter: ILocalFilter): string {
    const format = model.extensions().format();

    switch (format) {
        case ("int"):
            if (!isInteger(newValue)) {
                return "Not an integer";
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

    return "";
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
        return regex.test(newValue) ? "" : Msg.noPatternMatch;
    }
    return "";
}

export function validateDateTimeFormat(model: Models.IHasExtensions, newValue: Date): string {
    return "";
}

export function validateDateFormat(model: Models.IHasExtensions, newValue: Date | string, filter: ILocalFilter): string {
    const range = model.extensions().range();
    const newDate = (newValue instanceof Date) ? newValue : getDate(newValue);

    if (range && newDate) {
        const min = range.min ? getDate(range.min as string) : null;
        const max = range.max ? getDate(range.max as string) : null;

        if (min && newDate < min) {
            return Msg.outOfRange(Models.toDateString(newDate), Models.getUtcDate(range.min as string), Models.getUtcDate(range.max as string), filter);
        }

        if (max && newDate > max) {
            return Msg.outOfRange(Models.toDateString(newDate), Models.getUtcDate(range.min as string), Models.getUtcDate(range.max as string), filter);
        }
    }

    return "";
}

export function validateTimeFormat(model: Models.IHasExtensions, newValue: Date): string {
    return "";
}

export function validateString(model: Models.IHasExtensions, newValue: any, filter: ILocalFilter): string {
    const format = model.extensions().format();

    switch (format) {
        case ("string"):
            return validateStringFormat(model, newValue as string);
        case ("date-time"):
            return validateDateTimeFormat(model, newValue as Date);
        case ("date"):
            return validateDateFormat(model, newValue as Date | string, filter);
        case ("time"):
            return validateTimeFormat(model, newValue as Date);
        default:
            return "";
    }
}

export function validateMandatory(model: Models.IHasExtensions, viewValue: string): string {
    // first check
    const isMandatory = !model.extensions().optional();

    if (isMandatory && (viewValue === "" || viewValue == null)) {
        return Msg.mandatory;
    }

    return "";
}

export function validateMandatoryAgainstType(model: Models.IHasExtensions, viewValue: string, filter: ILocalFilter): string {

    // check type
    const returnType = model.extensions().returnType();

    switch (returnType) {
    case ("number"):
        const valueAsNumber = parseFloat(viewValue);
        if (Number.isFinite(valueAsNumber)) {
            return validateNumber(model, valueAsNumber, filter);
        }
        return Msg.notANumber;
    case ("string"):
        return validateString(model, viewValue, filter);
    case ("boolean"):
        return "";
    default:
        return "";
    }
}

export function validateDate(newValue: string, validInputFormats: string[]) {

    for (const f of validInputFormats) {
        const dt = moment.utc(newValue, f, true);
        if (dt.isValid()) {
            return dt;
        }
    }

    return null;
}
