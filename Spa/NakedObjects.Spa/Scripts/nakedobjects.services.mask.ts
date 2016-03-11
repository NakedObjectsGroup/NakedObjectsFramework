/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects {

    export interface ILocalFilter {
        filter(val: any): string;
    }

    export interface IMaskMap {
        [index: string]: { [index: string]: ILocalFilter };
    }

    export interface IMask {
        toLocalFilter(remoteMask: string, format: string): ILocalFilter;
        defaultLocalFilter(format: string): ILocalFilter;

        // use angular number mask to format
        setNumberMaskMapping(customMask: string, format: string, fractionSize?: number);

        // use angular date mask to format
        setDateMaskMapping(customMask: string, format: string, mask?: string, tz?: string);

        // use angular currency mask to format
        setCurrencyMaskMapping(customMask: string, format: string, symbol?: string, fractionSize?: number);
    }


    app.service("mask", function($filter: ng.IFilterService) {
        const maskService = <IMask>this;

        const maskMap: IMaskMap = {
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

        class LocalStringFilter implements ILocalFilter {

            filter(val): string {
                return val ? val.toString() : "";
            }
        }

        class LocalCurrencyFilter implements ILocalFilter {

            constructor(private symbol?: string, private fractionSize?: number) {}

            filter(val): string {
                return $filter("currency")(val, this.symbol, this.fractionSize);
            }
        }

        class LocalDateFilter implements ILocalFilter {

            constructor(private mask?: string, private tz?: string) {}

            filter(val): string {
                return $filter("date")(val, this.mask, this.tz);
            }
        }

        class LocalNumberFilter implements ILocalFilter {

            constructor(private fractionSize?: number) {}

            filter(val): string {
                return $filter("number")(val, this.fractionSize);
            }
        }

        maskService.defaultLocalFilter = (format: string): ILocalFilter => {
            switch (format) {
            case ("string"):
                return new LocalStringFilter();
            case ("date-time"):
                return new LocalDateFilter("d MMM yyyy hh:mm:ss");
            case ("date"):
                return new LocalDateFilter("d MMM yyyy", "+0000");
            case ("time"):
                return new LocalDateFilter("hh:mm:ss", "+0000");
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

        function customFilter(format: string, remoteMask: string) {
            if (maskMap[format] && remoteMask) {
                return maskMap[format][remoteMask];
            }
            return undefined;
        }

        maskService.toLocalFilter = (remoteMask: string, format: string) => {
            return customFilter(format, remoteMask) || maskService.defaultLocalFilter(format);
        };
        maskService.setNumberMaskMapping = (customMask: string, format: string, fractionSize?: number) => {
            maskMap[format][customMask] = new LocalNumberFilter(fractionSize);
        };
        maskService.setDateMaskMapping = (customMask: string, format: string, mask: string, tz: string) => {
            maskMap[format][customMask] = new LocalDateFilter(mask, tz);
        };
        maskService.setCurrencyMaskMapping = (customMask: string, format: string, symbol ?: string, fractionSize ?: number) => {
            maskMap[format][customMask] = new LocalCurrencyFilter(symbol, fractionSize);
        };
    });
}