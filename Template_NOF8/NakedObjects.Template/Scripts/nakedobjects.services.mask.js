/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    NakedObjects.app.service("mask", function ($filter) {
        var maskService = this;
        var maskMap = {
            string: {},
            "date-time": {},
            date: {},
            time: {},
            "utc-millisec": {},
            "big-integer": {},
            "big-decimal": {},
            blob: {},
            clob: {},
            decimal: {},
            int: {}
        };
        var LocalStringFilter = (function () {
            function LocalStringFilter() {
            }
            LocalStringFilter.prototype.filter = function (val) {
                return val ? val.toString() : "";
            };
            return LocalStringFilter;
        }());
        var LocalCurrencyFilter = (function () {
            function LocalCurrencyFilter(symbol, fractionSize) {
                this.symbol = symbol;
                this.fractionSize = fractionSize;
            }
            LocalCurrencyFilter.prototype.filter = function (val) {
                return $filter("currency")(val, this.symbol, this.fractionSize);
            };
            return LocalCurrencyFilter;
        }());
        var LocalDateFilter = (function () {
            function LocalDateFilter(mask, tz) {
                this.mask = mask;
                this.tz = tz;
            }
            LocalDateFilter.prototype.filter = function (val) {
                return $filter("date")(val, this.mask, this.tz);
            };
            return LocalDateFilter;
        }());
        var LocalNumberFilter = (function () {
            function LocalNumberFilter(fractionSize) {
                this.fractionSize = fractionSize;
            }
            LocalNumberFilter.prototype.filter = function (val) {
                return $filter("number")(val, this.fractionSize);
            };
            return LocalNumberFilter;
        }());
        maskService.defaultLocalFilter = function (format) {
            switch (format) {
                case ("string"):
                    return new LocalStringFilter();
                case ("date-time"):
                    return new LocalDateFilter("d MMM yyyy HH:mm:ss");
                case ("date"):
                    return new LocalDateFilter("d MMM yyyy", "+0000");
                case ("time"):
                    return new LocalDateFilter("HH:mm", "+0000");
                case ("utc-millisec"):
                    return new LocalNumberFilter();
                case ("big-integer"):
                    return new LocalNumberFilter();
                case ("big-decimal"):
                    return new LocalNumberFilter();
                case ("blob"):
                    return new LocalStringFilter();
                case ("clob"):
                    return new LocalStringFilter();
                case ("decimal"):
                    return new LocalNumberFilter();
                case ("int"):
                    return new LocalNumberFilter();
                default:
                    return new LocalStringFilter();
            }
        };
        function customFilter(format, remoteMask) {
            if (maskMap[format] && remoteMask) {
                return maskMap[format][remoteMask];
            }
            return undefined;
        }
        maskService.toLocalFilter = function (remoteMask, format) {
            return customFilter(format, remoteMask) || maskService.defaultLocalFilter(format);
        };
        maskService.setNumberMaskMapping = function (customMask, format, fractionSize) {
            maskMap[format][customMask] = new LocalNumberFilter(fractionSize);
        };
        maskService.setDateMaskMapping = function (customMask, format, mask, tz) {
            maskMap[format][customMask] = new LocalDateFilter(mask, tz);
        };
        maskService.setCurrencyMaskMapping = function (customMask, format, symbol, fractionSize) {
            maskMap[format][customMask] = new LocalCurrencyFilter(symbol, fractionSize);
        };
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.mask.js.map