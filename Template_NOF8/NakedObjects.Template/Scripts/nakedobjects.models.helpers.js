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
var NakedObjects;
(function (NakedObjects) {
    var Models;
    (function (Models) {
        function dirtyMarker(context, oid) {
            return (NakedObjects.showDirtyFlag && context.getIsDirty(oid)) ? "*" : "";
        }
        Models.dirtyMarker = dirtyMarker;
        function getOtherPane(paneId) {
            return paneId === 1 ? 2 : 1;
        }
        Models.getOtherPane = getOtherPane;
        function toDateString(dt) {
            var year = dt.getFullYear().toString();
            var month = (dt.getMonth() + 1).toString();
            var day = dt.getDate().toString();
            month = month.length === 1 ? "0" + month : month;
            day = day.length === 1 ? "0" + day : day;
            return year + "-" + month + "-" + day;
        }
        Models.toDateString = toDateString;
        function toTimeString(dt) {
            var hours = dt.getHours().toString();
            var minutes = dt.getMinutes().toString();
            var seconds = dt.getSeconds().toString();
            hours = hours.length === 1 ? "0" + hours : hours;
            minutes = minutes.length === 1 ? "0" + minutes : minutes;
            seconds = seconds.length === 1 ? "0" + seconds : seconds;
            return hours + ":" + minutes + ":" + seconds;
        }
        Models.toTimeString = toTimeString;
        function getUtcDate(rawDate) {
            if (!rawDate || rawDate.length === 0) {
                return null;
            }
            var year = parseInt(rawDate.substring(0, 4));
            var month = parseInt(rawDate.substring(5, 7)) - 1;
            var day = parseInt(rawDate.substring(8, 10));
            if (rawDate.length === 10) {
                return new Date(Date.UTC(year, month, day, 0, 0, 0));
            }
            if (rawDate.length >= 20) {
                var hours = parseInt(rawDate.substring(11, 13));
                var mins = parseInt(rawDate.substring(14, 16));
                var secs = parseInt(rawDate.substring(17, 19));
                return new Date(Date.UTC(year, month, day, hours, mins, secs));
            }
            return null;
        }
        Models.getUtcDate = getUtcDate;
        function getTime(rawTime) {
            if (!rawTime || rawTime.length === 0) {
                return null;
            }
            var hours = parseInt(rawTime.substring(0, 2));
            var mins = parseInt(rawTime.substring(3, 5));
            var secs = parseInt(rawTime.substring(6, 8));
            return new Date(1970, 0, 1, hours, mins, secs);
        }
        Models.getTime = getTime;
        function isDateOrDateTime(rep) {
            var returnType = rep.extensions().returnType();
            var format = rep.extensions().format();
            return (returnType === "string" && ((format === "date-time") || (format === "date")));
        }
        Models.isDateOrDateTime = isDateOrDateTime;
        function isTime(rep) {
            var returnType = rep.extensions().returnType();
            var format = rep.extensions().format();
            return returnType === "string" && format === "time";
        }
        Models.isTime = isTime;
        function toUtcDate(value) {
            var rawValue = value ? value.toString() : "";
            var dateValue = getUtcDate(rawValue);
            return dateValue ? dateValue : null;
        }
        Models.toUtcDate = toUtcDate;
        function toTime(value) {
            var rawValue = value ? value.toString() : "";
            var dateValue = getTime(rawValue);
            return dateValue ? dateValue : null;
        }
        Models.toTime = toTime;
        function compress(toCompress) {
            if (toCompress) {
                _.forEach(NakedObjects.urlShortCuts, function (sc, i) { return toCompress = toCompress.replace(sc, "" + NakedObjects.shortCutMarker + i); });
            }
            return toCompress;
        }
        Models.compress = compress;
        function decompress(toDecompress) {
            if (toDecompress) {
                _.forEach(NakedObjects.urlShortCuts, function (sc, i) { return toDecompress = toDecompress.replace("" + NakedObjects.shortCutMarker + i, sc); });
            }
            return toDecompress;
        }
        Models.decompress = decompress;
        function getClassName(obj) {
            var funcNameRegex = /function (.{1,})\(/;
            var results = (funcNameRegex).exec(obj.constructor.toString());
            return (results && results.length > 1) ? results[1] : "";
        }
        Models.getClassName = getClassName;
        function typeFromUrl(url) {
            var typeRegex = /(objects|services)\/([\w|\.]+)/;
            var results = (typeRegex).exec(url);
            return (results && results.length > 2) ? results[2] : "";
        }
        Models.typeFromUrl = typeFromUrl;
        function idFromUrl(href) {
            var urlRegex = /(objects|services)\/(.*?)\/([^\/]*)/;
            var results = (urlRegex).exec(href);
            return (results && results.length > 3) ? results[3] : "";
        }
        Models.idFromUrl = idFromUrl;
        function propertyIdFromUrl(href) {
            var urlRegex = /(objects)\/(.*)\/(.*)\/(properties)\/(.*)/;
            var results = (urlRegex).exec(href);
            return (results && results.length > 5) ? results[5] : "";
        }
        Models.propertyIdFromUrl = propertyIdFromUrl;
        function friendlyTypeName(fullName) {
            var shortName = _.last(fullName.split("."));
            var result = shortName.replace(/([A-Z])/g, " $1").trim();
            return result.charAt(0).toUpperCase() + result.slice(1);
        }
        Models.friendlyTypeName = friendlyTypeName;
        function friendlyNameForParam(action, parmId) {
            var param = _.find(action.parameters(), function (p) { return p.id() === parmId; });
            return param.extensions().friendlyName();
        }
        Models.friendlyNameForParam = friendlyNameForParam;
        function friendlyNameForProperty(obj, propId) {
            var prop = obj.propertyMember(propId);
            return prop.extensions().friendlyName();
        }
        Models.friendlyNameForProperty = friendlyNameForProperty;
        function typePlusTitle(obj) {
            var type = obj.extensions().friendlyName();
            var title = obj.title();
            return type + ": " + title;
        }
        Models.typePlusTitle = typePlusTitle;
        function isInteger(value) {
            return typeof value === "number" && isFinite(value) && Math.floor(value) === value;
        }
        function validateNumber(model, newValue, filter) {
            var format = model.extensions().format();
            switch (format) {
                case ("int"):
                    if (!isInteger(newValue)) {
                        return "Not an integer";
                    }
            }
            var range = model.extensions().range();
            if (range) {
                var min = range.min;
                var max = range.max;
                if (min && newValue < min) {
                    return NakedObjects.outOfRange(newValue, min, max, filter);
                }
                if (max && newValue > max) {
                    return NakedObjects.outOfRange(newValue, min, max, filter);
                }
            }
            return "";
        }
        function validateStringFormat(model, newValue) {
            var maxLength = model.extensions().maxLength();
            var pattern = model.extensions().pattern();
            var len = newValue ? newValue.length : 0;
            if (maxLength && len > maxLength) {
                return NakedObjects.tooLong;
            }
            if (pattern) {
                var regex = new RegExp(pattern);
                return regex.test(newValue) ? "" : NakedObjects.noPatternMatch;
            }
            return "";
        }
        function validateDateTimeFormat(model, newValue) {
            return "";
        }
        function getDate(val) {
            var dt1 = moment(val, "YYYY-MM-DD", "en-GB", true);
            if (dt1.isValid()) {
                return dt1.toDate();
            }
            return null;
        }
        function validateDateFormat(model, newValue, filter) {
            var range = model.extensions().range();
            if (range && newValue) {
                var min = range.min ? getDate(range.min) : null;
                var max = range.max ? getDate(range.max) : null;
                if (min && newValue < min) {
                    return NakedObjects.outOfRange(toDateString(newValue), getUtcDate(range.min), getUtcDate(range.max), filter);
                }
                if (max && newValue > max) {
                    return NakedObjects.outOfRange(toDateString(newValue), getUtcDate(range.min), getUtcDate(range.max), filter);
                }
            }
            return "";
        }
        function validateTimeFormat(model, newValue) {
            return "";
        }
        function validateString(model, newValue, filter) {
            var format = model.extensions().format();
            switch (format) {
                case ("string"):
                    return validateStringFormat(model, newValue);
                case ("date-time"):
                    return validateDateTimeFormat(model, newValue);
                case ("date"):
                    return validateDateFormat(model, newValue, filter);
                case ("time"):
                    return validateTimeFormat(model, newValue);
                default:
                    return "";
            }
        }
        function validateMandatory(model, viewValue) {
            // first check 
            var isMandatory = !model.extensions().optional();
            if (isMandatory && (viewValue === "" || viewValue == null)) {
                return NakedObjects.mandatory;
            }
            return "";
        }
        Models.validateMandatory = validateMandatory;
        function validate(model, modelValue, viewValue, filter) {
            // first check 
            var mandatory = validateMandatory(model, viewValue);
            if (mandatory) {
                return mandatory;
            }
            // if optional but empty always valid 
            if (modelValue == null || modelValue === "") {
                return "";
            }
            // check type 
            var returnType = model.extensions().returnType();
            switch (returnType) {
                case ("number"):
                    if (!$.isNumeric(modelValue)) {
                        return NakedObjects.notANumber;
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
        Models.validate = validate;
    })(Models = NakedObjects.Models || (NakedObjects.Models = {}));
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.models.helpers.js.map