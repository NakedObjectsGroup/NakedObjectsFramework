//  Copyright 2013-2014 Naked Objects Group Ltd
//  Licensed under the Apache License, Version 2.0(the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

// helpers for common tasks with nakedobjects.models 

module NakedObjects.Models {

    export function toDateString(dt: Date) {

        const year = dt.getFullYear().toString();
        let month = (dt.getMonth() + 1).toString();
        let day = dt.getDate().toString();

        month = month.length === 1 ? `0${month}` : month;
        day = day.length === 1 ? `0${day}` : day;

        return `${year}-${month}-${day}`;
    }

    export function getUtcDate(rawDate: string) {
        if (!rawDate || rawDate.length === 0) {
            return null;
        }

        const year = parseInt(rawDate.substring(0, 4));
        const month = parseInt(rawDate.substring(5, 7)) - 1;
        const day = parseInt(rawDate.substring(8, 10));

        if (rawDate.length === 10) {
            return new Date(Date.UTC(year, month, day, 0, 0, 0));
        }

        if (rawDate.length >= 20) {
            const hours = parseInt(rawDate.substring(11, 13));
            const mins = parseInt(rawDate.substring(14, 16));
            const secs = parseInt(rawDate.substring(17, 19));

            return new Date(Date.UTC(year, month, day, hours, mins, secs));
        }

        return null;
    }

    export function isDateOrDateTime(rep: IHasExtensions) {
        const returnType = rep.extensions().returnType();
        const format = rep.extensions().format();

        return (returnType === "string" && ((format === "date-time") || (format === "date")));
    }

    export function toUtcDate(value: Value) {
        const rawValue = value ? value.toString() : "";
        const dateValue = getUtcDate(rawValue);
        return dateValue ? dateValue : null;
    }

    export function compress(toCompress: string) {
        if (toCompress) {
            _.forEach(urlShortCuts, (sc, i) => toCompress = toCompress.replace(sc, `${shortCutMarker}${i}`));
        }
        return toCompress;
    }

    export function decompress(toDecompress: string) {
        if (toDecompress) {
            _.forEach(urlShortCuts, (sc, i) => toDecompress = toDecompress.replace(`${shortCutMarker}${i}`, sc));
        }
        return toDecompress;
    }

    export function getClassName(obj: any) {
        const funcNameRegex = /function (.{1,})\(/;
        const results = (funcNameRegex).exec(obj.constructor.toString());
        return (results && results.length > 1) ? results[1] : "";
    }

    export function typeFromUrl(url: string) {
        const typeRegex = /(objects|services)\/([\w|\.]+)/;
        const results = (typeRegex).exec(url);
        return (results && results.length > 2) ? results[2] : "";
    }

    export function friendlyTypeName(fullName: string) {
        const shortName = _.last(fullName.split("."));
        const result = shortName.replace(/([A-Z])/g, " $1").trim();
        return result.charAt(0).toUpperCase() + result.slice(1);
    }

    export function friendlyNameForParam(action: Models.IInvokableAction, parmId: string) {
        const param = _.find(action.parameters(), (p) => p.id() === parmId);
        return param.extensions().friendlyName();
    }

    export function friendlyNameForProperty(obj: DomainObjectRepresentation, propId: string) {
        const prop = obj.propertyMember(propId);
        return prop.extensions().friendlyName();
    }

    export function typePlusTitle(obj: DomainObjectRepresentation) {
        const type = obj.extensions().friendlyName();
        const title = obj.title();
        return type + ": " + title;
    }

    function isInteger(value: number) {
        return typeof value === "number" && isFinite(value) && Math.floor(value) === value;
    }


    function validateNumber(model: IHasExtensions, newValue: number, filter: ILocalFilter): string {
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
                return outOfRange(newValue, min, max, filter);
            }

            if (max && newValue > max) {
                return outOfRange(newValue, min, max, filter);
            }
        }

        return "";
    }

    function validateStringFormat(model: IHasExtensions, newValue: string): string {
        
        const maxLength = model.extensions().maxLength();
        const pattern = model.extensions().pattern();
        const len = newValue ? newValue.length : 0;

        if (maxLength && len > maxLength) {
            return tooLong;
        }

        if (pattern) {
            const regex = new RegExp(pattern);
            return regex.test(newValue) ? "" : noPatternMatch;
        }
        return "";
    }

    function validateDateTimeFormat(model: IHasExtensions, newValue: Date): string {
        return "";
    }

    function validateDateFormat(model: IHasExtensions, newValue: Date, filter: ILocalFilter): string {
        const range = model.extensions().range();

        if (range && newValue) {
            const min = range.min ? getUtcDate(range.min as string) : null;
            const max = range.max ? getUtcDate(range.max as string) : null;

            if (min && newValue < min) {
                return outOfRange(toDateString(newValue), min, max, filter);
            }

            if (max && newValue > max) {
                return outOfRange(toDateString(newValue), min, max, filter);
            }
        }

        return "";
    }

    function validateTimeFormat(model: IHasExtensions, newValue: Date): string {
        return "";
    }

    function validateString(model: IHasExtensions, newValue: any, filter: ILocalFilter): string {
        const format = model.extensions().format();

        switch (format) {
            case ("string"):        
                return validateStringFormat(model, newValue as string);
            case ("date-time"):
                return validateDateTimeFormat(model, newValue as Date);
            case ("date"):
                return validateDateFormat(model, newValue as Date, filter);
            case ("time"):
                return validateTimeFormat(model, newValue as Date);
            default:
                return "";
        }
    }


    export function validateMandatory(model: IHasExtensions, viewValue: string | ChoiceViewModel): string {
        // first check 
        const isMandatory = !model.extensions().optional();

        if (isMandatory && (viewValue === "" || viewValue == null)) {
            return mandatory;
        }

        return "";
    }


    export function validate(model: IHasExtensions, modelValue: any, viewValue: string, filter: ILocalFilter): string {
        // first check 

        const mandatory = validateMandatory(model, viewValue);

        if (mandatory) {
            return mandatory;
        }

        // if optional but empty always valid 
        if (modelValue == null) {
            return "";
        }

        // check type 
        const returnType = model.extensions().returnType();

        switch (returnType) {
            case ("number"):
                if (!$.isNumeric(modelValue)) {
                    return notANumber;
                }
                return validateNumber(model, parseFloat(modelValue), filter);
            case ("string"):
                return validateString(model, modelValue, filter);
            case ("boolean"):
                return "";
            default:
                return "";
        }
    }
}