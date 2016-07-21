/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />

namespace NakedObjects {

    export interface ILocalFilter {
        filter(val: any): string;
    }

    export interface IMaskMap {
        [index: string]: { [index: string]: ILocalFilter };
    }

    export type formatType = "string" | "date-time" | "date" | "time" | "utc-millsec" | "big-integer" | "big-decimal" | "blob"  | "clob" | "decimal" | "int";

    export interface IMask {
        toLocalFilter(remoteMask: string, format: formatType): ILocalFilter;
        defaultLocalFilter(format: formatType): ILocalFilter;

        // use angular number mask to format
        setNumberMaskMapping(customMask: string, format: formatType, fractionSize?: number): void;

        // use angular date mask to format
        setDateMaskMapping(customMask: string, format: formatType, mask?: string, tz?: string): void;

        // use angular currency mask to format
        setCurrencyMaskMapping(customMask: string, format: formatType, symbol?: string, fractionSize?: number): void;
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

            filter(val : any): string {
                return val ? val.toString() : "";
            }
        }

        class LocalCurrencyFilter implements ILocalFilter {

            constructor(private symbol?: string, private fractionSize?: number) {}

            filter(val : any): string {
                return $filter("currency")(val, this.symbol, this.fractionSize);
            }
        }

        class LocalDateFilter implements ILocalFilter {

            constructor(private mask?: string, private tz?: string) {}

            filter(val : any): string {
                return $filter("date")(val, this.mask, this.tz);
            }
        }

        class LocalNumberFilter implements ILocalFilter {

            constructor(private fractionSize?: number) {}

            filter(val : any): string {
                return $filter("number")(val, this.fractionSize);
            }
        }

        maskService.defaultLocalFilter = (format: formatType): ILocalFilter => {
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

        function customFilter(format: formatType, remoteMask: string) {
            if (maskMap[format as string] && remoteMask) {
                return maskMap[format as string ][remoteMask];
            }
            return undefined;
        }

        maskService.toLocalFilter = (remoteMask: string, format: formatType) => {
            return customFilter(format, remoteMask) || maskService.defaultLocalFilter(format);
        };

        maskService.setNumberMaskMapping = (customMask: string, format: formatType, fractionSize?: number) => {
            maskMap[format as string][customMask] = new LocalNumberFilter(fractionSize);
        };

        maskService.setDateMaskMapping = (customMask: string, format: formatType, mask: string, tz: string) => {
            maskMap[format as string][customMask] = new LocalDateFilter(mask, tz);
        };

        maskService.setCurrencyMaskMapping = (customMask: string, format: formatType, symbol ?: string, fractionSize ?: number) => {
            maskMap[format as string][customMask] = new LocalCurrencyFilter(symbol, fractionSize);
        };
    });
}